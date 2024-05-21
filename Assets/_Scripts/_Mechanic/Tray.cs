using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tray : MonoBehaviour,ITray
{
    #region Variables
    
    [Space]
    [BoxGroup("POCKET LIST", centerLabel: true)] public List<Pocket> Pockets;
    [BoxGroup("COLOR LIST", centerLabel: true)] [SerializeField] private List<ColorType> ColorTypeList;
    [BoxGroup("Speed", centerLabel: true)] [Range(0,80)] [SerializeField] private float timeTravel = 0f;
    [BoxGroup("Speed")] [Range(0,0.1f)] [SerializeField] private float shakeTime = 0f;
    [BoxGroup("Speed")] [Range(0,1f)] [SerializeField] private float moveDuration = 0f;
    [BoxGroup("BOOL", centerLabel: true)] [SerializeField] private bool isSelected;
    [BoxGroup("BOOL")] [SerializeField] private bool _isRayDisabled;
    [BoxGroup("LOCK FIELDS", centerLabel: true)] [SerializeField] private List<TMP_Text> _coinImageText;
    [BoxGroup("LOCK FIELDS")] [SerializeField] private GameObject indicatorObject;
    [BoxGroup("LOCK FIELDS")] [SerializeField] private GameObject SpawnCoin;
    [BoxGroup("LOCK FIELDS")] [SerializeField] private List<int> _enableAtScoreList;
    
    private bool _isFull;
    private ITray _faceTray;
    private RaycastHit _raycastHit;
    private ColorType.Color _selectedObjType;
    private Transform _highlight;
    private int _lastSelected;
    private float _elapsedTime;
    private float _percentageComplete;
    private List<GameObject> _selectedStack = new List<GameObject>();
    private List<GameObject> indicatorsToDestroy = new List<GameObject>();
    private List<int> _indexsOfSlotsToMerge = new List<int>();
    private List<int> _indicesToRemove = new List<int>();
    private List<GameObject> _pocketList = new List<GameObject>();
    #endregion

    public IScorer Scorer { get; set; }

    private void Update()
    {
        CastRay();
    }
    
    void ITray.Initialize()
    {
        FillTrayList();
        PocketInitialization();
    }

    #region PointRayCast

    private void CastRay()
    {
        if (!_isRayDisabled)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out _raycastHit))
            {
                _highlight = _raycastHit.transform;
                if (_highlight.CompareTag("Selectable") && isSelected != true)
                {
                    SelectPocket(_pocketList.IndexOf(_raycastHit.transform.gameObject));
                    
                }
                else if (_highlight.CompareTag("Selectable") && (isSelected))
                {
                    UnSelectPocket(_pocketList.IndexOf(_raycastHit.transform.gameObject));
                }
                else
                {
                    _highlight = null;
                }
            }
        }
    }
    
    void FillTrayList()
    {
        foreach (var pocket in Pockets)
        {
            _pocketList.Add(pocket.gameObject);
        }
    }

    #endregion

    #region SelectSlot

    private void SelectPocket(int pocketIndex)
    {
        if (Pockets[pocketIndex].Coins.Count != 0 && (!Pockets[pocketIndex].isLocked))
        {
            if (!isSelected)
            {
                PickStack(1f, pocketIndex, true);
            }

            _lastSelected = pocketIndex;
            isSelected = true;
        }
        else if (Pockets[pocketIndex].isDisabled == false && Pockets[pocketIndex].isLocked)
        {
            OnClickUnLock(pocketIndex);
        }

    }

    void PickStack(float offset, int pocketIndex, bool isOn)
    {
        
            int lastCoin = (Pockets[pocketIndex].CoinList.Count)-1;
        _selectedObjType = Pockets[pocketIndex].CoinList[lastCoin].ColorType;

        int pocketSize = (Pockets[pocketIndex].CoinList.Count) - 1;
        bool isDone=false;
        
        for (int i = pocketSize; i >= 0; i--)
        {        
                if (Pockets[pocketIndex].CoinList[i].ColorType.Equals(_selectedObjType))
                {
                    _selectedStack.Add(Pockets[pocketIndex].Coins[i]);
                    _indicesToRemove.Add(i);
                    isDone = isOn;
                }
                else
                {
                    break;
                }
        }

        if (_indicesToRemove.Count!=0 && isDone)
        {
            ClearCoinsFromLastSelected(pocketIndex,offset);
        }
       
        _indicesToRemove.Clear();
        _selectedStack.Reverse();
        isSelected = isOn;
    }
    
    void ClearCoinsFromLastSelected(int Pindex,float offset)
    {
        foreach (var index in _indicesToRemove)
        {    
            Vector3 currentPosition = Pockets[Pindex].Coins[index].transform.position;
            currentPosition.y += offset;
            Pockets[Pindex].Coins[index].transform.position = currentPosition;
            Pockets[Pindex].Coins.RemoveAt(index);
            Pockets[Pindex].CoinList.RemoveAt(index);
        }
    }

    #endregion

    #region UnSelectSlot
    public void UnSelectPocket(int pocketIndex)
    {    
        
        if (!Pockets[pocketIndex].isLocked)
        {
            if ((isSelected) && pocketIndex == _lastSelected)
            {
                UnPick(-1f, pocketIndex, false);
                isSelected = false;
                _selectedStack.Clear();
            }
            else if (pocketIndex != _lastSelected)
            {
                if (_selectedStack.Count != 0)
                {
                    MoveSlotToNext(Pockets, _selectedStack, _lastSelected, pocketIndex);
                }
            }
        }

    }

    void UnPick(float offset, int pocketIndex, bool isOn)
    {

        for (int i = 0; i < _selectedStack.Count; i++)
        {
            Vector3 currentPosition = _selectedStack[i].transform.position;
            currentPosition.y += offset;
            _selectedStack[i].transform.position = currentPosition;
            Pockets[pocketIndex].Coins.Add(_selectedStack[i]);
            Pockets[pocketIndex].CoinList.Add(Utils.LoadCoinByColorType("Coins",_selectedObjType));
        }

        isSelected = isOn;
        _selectedObjType = ColorType.Color.None;  
        _selectedStack.Clear();
    }

    #endregion

    #region PocketInitialization

    private void PocketInitialization()
    {
        InitializeEachPocket();
        SetLockState();
        SetCoinImageScoreText();
        EnablingSlot(Scorer.GetCurrentScore());
        Deal();
    }
    private void Deal()
    {
        if (!isSelected)
        {
            DrawCoins();
        }
        else if (isSelected)
        {
            MoveBackCoins(Pockets,_selectedStack,_lastSelected);
            DrawCoins();
        }

        void DrawCoins()
        {
            for (int i = 0; i < Pockets.Count; i++)
            {
                if (Pockets[i].Coins.Count < 10 && Pockets[i].isLocked == false)
                {    
                    Debug.Log("Deal !");
                    Pockets[i].AddCoinsToPocket(_faceTray, GetNumToDraw(Pockets[i].Coins.Count), RandomColor(), i);
                }
            }
        }
        int GetNumToDraw(int curCountOfList)
        {
            int res = 0;
            int x = (10 - curCountOfList) / 2;
            if (curCountOfList != 10)
            {
                x = (x >= 5) ? x -= 1 : x += 1;
                res = Random.Range(1, x);
            }

            return res;
        }
        ColorType.Color RandomColor()
        {
            ColorType.Color clr = ColorTypeList[Random.Range(0, ColorTypeList.Count)].typeOfColor;
            return clr;
        }
    }
    public void OnClickDeal()
    {    
        Scorer.OnDrawCoins(5);
        Deal();
        CheckForSlotsToMerge();
        DisablingSlot(Scorer.GetCurrentScore());
    }

    #endregion

    #region Lock & Unlock SLots

    void InitializeEachPocket()
    {
        foreach (var pocket in Pockets)
        {
            pocket.Initialize();
        }
    }
    void SetCoinImageScoreText()
    {
        for (int i = 0; i < _enableAtScoreList.Count; i++)
        {
            _coinImageText[i].text = _enableAtScoreList[i].ToString();
        }
    }
    void SetLockState()
    {
        
        for (int i = 5; i < Pockets.Count; i++)
        {
            Pockets[i].LockedBoth();
        }
    }

    void EnablingSlot(int currentScore)
    {
        for (int i = 0; i < _enableAtScoreList.Count; i++)
        {
            if (currentScore >= _enableAtScoreList[i] && _enableAtScoreList[i]!=0)
            {
                Pockets[5+i].EnableSlot();
                _coinImageText[i].transform.gameObject.SetActive(true);
            }
        }
    }

    void DisablingSlot(int currentScore)
    {
        for (int i = 0; i < _enableAtScoreList.Count; i++)
        {
            if(currentScore<_enableAtScoreList[i])
            {
                Pockets[5+i].DisableSlot();
                _coinImageText[i].transform.gameObject.SetActive(false);
            }
        }
    }
    
    public void OnClickUnLock(int id)
    {
        Pockets[id].UnlockSlot();
        Scorer.OnClickUnLockBtn(_enableAtScoreList[id-5]);
        _coinImageText[id-5].transform.gameObject.SetActive(false);
        _enableAtScoreList[id-5] = 0;
        DisablingSlot(Scorer.GetCurrentScore());
    }

    #endregion
    
    #region MoveSelectedStack

    private void MoveSlotToNext(List<Pocket> slotContainer, List<GameObject> stack, int selectedSlotIndex, int targetSlotIndex)
    {

        if (isSelected && slotContainer[targetSlotIndex].Coins.Count<10)
        {
            int noOfCoins = slotContainer[targetSlotIndex].Coins.Count;
            
            if (noOfCoins == 0)
            {    
                ClearIndicator();
                StartCoroutine(AnimateMove(slotContainer,stack,selectedSlotIndex,targetSlotIndex));
            }
            else if (slotContainer[targetSlotIndex].CoinList[noOfCoins - 1].ColorType == _selectedObjType)
            {    
                ClearIndicator();
                StartCoroutine(AnimateMove(slotContainer,stack,selectedSlotIndex,targetSlotIndex)); 
            } 
            else if (slotContainer[targetSlotIndex].CoinList[noOfCoins - 1].ColorType != _selectedObjType)
            {
                MoveBackCoins(Pockets,_selectedStack,selectedSlotIndex);
            }
        }
        else if (isSelected && slotContainer[targetSlotIndex].Coins.Count == 10)
        {
            MoveBackCoins(Pockets,_selectedStack,selectedSlotIndex);
        }
        
        
        void ClearIndicator()
        {
            foreach (var item in indicatorsToDestroy)
            {
                Destroy(item);
            }

            indicatorsToDestroy.Clear();
            _indexsOfSlotsToMerge.Clear();
        }
    }
    private IEnumerator AnimateMove(List<Pocket> slotContainer, List<GameObject> stack, int selectedSlotIndex, int targetSlotIndex)
        {    
            _isRayDisabled = true;
            int lastIndexOfTargetSlot = slotContainer[targetSlotIndex].Coins.Count;
            int rotationValue =  (selectedSlotIndex < targetSlotIndex) ? 180 : -180;
            int count = stack.Count;
            int noOfTimes=0;
            for (int i = count-1; i >= 0; i--)
            {
                if (slotContainer[targetSlotIndex].Coins.Count<10)    
                {
                    Vector3 startPosition = stack[i].transform.position;
                    Vector3 endPosition = slotContainer[targetSlotIndex]
                        .GetSlotPosition(targetSlotIndex, lastIndexOfTargetSlot);
                    lastIndexOfTargetSlot++;
                    slotContainer[targetSlotIndex].Coins.Add(stack[i]);
                    noOfTimes++;
                    slotContainer[targetSlotIndex].CoinList.Add(slotContainer[targetSlotIndex].GetObjByColorType(_selectedObjType));
                    if (i == 0)
                    {    
                        Vector3 currentRotation = stack[i].transform.rotation.eulerAngles;
                        float newYRotation = currentRotation.y + rotationValue;
                        stack[i].transform.DORotate(new Vector3(0,newYRotation,0),moveDuration,RotateMode.FastBeyond360);
                        stack[i].transform.DOJump(endPosition,1.5f,1,moveDuration,false).SetEase(Ease.OutCirc)  .OnComplete(() =>
                        {
                            _isRayDisabled = false;
                        });
                    }
                    else
                    {    
                        Vector3 currentRotation = stack[i].transform.rotation.eulerAngles;
                        float newYRotation = currentRotation.y + rotationValue;
                        stack[i].transform.DORotate(new Vector3(0,newYRotation,0),moveDuration,RotateMode.FastBeyond360);
                        stack[i].transform.DOJump(endPosition,1.5f,1,moveDuration,false).SetEase(Ease.OutCirc);
                    }
                    yield return new WaitForSeconds(0.03f);
                    _selectedStack.RemoveAt(i);
                }
            }

            yield return new WaitForSeconds(0.03f*noOfTimes);
            MoveBackCoins(Pockets,_selectedStack,selectedSlotIndex);
            CheckForSlotsToMerge();
            isSelected = false;
            _selectedObjType = ColorType.Color.None;
            
        }
    private void MoveBackCoins(List<Pocket> previousSlot, List<GameObject> selectedSlot, int indexOfSelectedSlot)
    {
        if (selectedSlot.Count != 0)
        {
            foreach (var chip in selectedSlot)
            {    
                chip.transform.position = new Vector3(chip.transform.position.x,chip.transform.position.y-1f,chip.transform.position.z);
                previousSlot[indexOfSelectedSlot].Coins.Add(chip);
                previousSlot[indexOfSelectedSlot].CoinList.Add(Utils.LoadCoinByColorType("Coins", _selectedObjType));
            }
            _selectedStack.Clear();
            _isRayDisabled = false;
        }
        _selectedObjType = ColorType.Color.None;
        isSelected = false;
    }
    
    #endregion

    #region Merge Stack
    private void CheckForSlotsToMerge()
    {
        List<int> mergeSlots = GetFilledPockets();

        foreach (int index in mergeSlots)
        {
            if (index != -1)
            {
                _indexsOfSlotsToMerge.Add(index);
            }
        }
    }
    private List<int> GetFilledPockets()
    {
        int targetIndex = -1;
        List<int> slotsToMerge=new List<int>();

        for (int i = 0; i < 10; i++)
        {
            GetSlotToMerge(i);
        }
       
        void GetSlotToMerge(int indexOfPocket)
        {
            if (Pockets[indexOfPocket].Coins.Count != 0)
            {
                int counter = 0;
                ColorType.Color coinColorType = Pockets[indexOfPocket].CoinList[0].ColorType;  
                foreach (var coin in Pockets[indexOfPocket].CoinList)
                {
                    if (coin.ColorType == coinColorType)
                    {
                        counter++;
                    }
                }

                if (counter == 10)
                {
                    targetIndex = indexOfPocket;
                    _isFull = true;
                }
                
                if (_isFull && targetIndex != -1 && (!_indexsOfSlotsToMerge.Contains(targetIndex)))
                {
                    Vector3 indicatorPos = Pockets[targetIndex].gameObject.transform.position;
                    indicatorPos.z -= 0f; indicatorPos.y = 0.42f;
                    GameObject indicator = Instantiate(indicatorObject, indicatorPos, quaternion.identity) as GameObject;
                    indicator.transform.rotation = Quaternion.Euler(-90f,0,0);
                    indicatorsToDestroy.Add(indicator);
                    slotsToMerge.Add(targetIndex);
                    _isFull = false;
                }
                targetIndex = -1;
            }
               
        } 
        
        return slotsToMerge;
    }
    public void OnClickMerge()
    {
        for (int i = 0; i < _indexsOfSlotsToMerge.Count; i++)
        {
            int tIndex = _indexsOfSlotsToMerge[i];
            if (tIndex != -1)
            {
                Scorer.OnMergeSlot(20);
                EnablingSlot(Scorer.GetCurrentScore());
                Scorer.OnMergeSlotForTarget(5);
                SpawnCoinAnimation(tIndex);
                StartCoroutine(AnimateMerge(tIndex));
                foreach (var indicator in indicatorsToDestroy)
                {
                    Destroy(indicator);
                }
                indicatorsToDestroy.Clear();
            }
        }
    }
    IEnumerator AnimateMerge(int targetIndex)
    {
        for (int i = Pockets[targetIndex].Coins.Count - 1; i >= 0; i--)
        {
            float elapsedTime = 0f;
            while (elapsedTime < 1)
            {
                elapsedTime += (Time.deltaTime) * timeTravel;
                Pockets[targetIndex].Coins[i].transform.position = Vector3.Lerp(
                    Pockets[targetIndex].Coins[i].transform.position,
                    Pockets[targetIndex].GetSlotPosition(targetIndex, 0),Mathf.SmoothStep(0,1,elapsedTime));
                yield return new WaitForSeconds(0.02f);
            }
        }

        foreach (var item in Pockets[targetIndex].Coins)
        {
            Destroy(item);
        }

        Pockets[targetIndex].CoinList.Clear();
        Pockets[targetIndex].Coins.Clear();
            
        _isFull = false;
        _indexsOfSlotsToMerge.Clear();
    }
    private void SpawnCoinAnimation(int targetIndex)
    {
        Color newColor = Pockets[targetIndex].CoinList[0].colorCode;
        Vector3 PocketPos = Pockets[targetIndex].transform.position;
        PocketPos.y = 2.51f;
        //PocketPos.x += 0.205f;
        GameObject coinObj = Instantiate(SpawnCoin, SpawnCoin.transform.position, transform.rotation);
        coinObj.transform.rotation = Quaternion.Euler(34,0,0);
        coinObj.transform.position = PocketPos;
        coinObj.GetComponent<MeshRenderer>().material.color = newColor;
        newColor = newColor*(Color.white*2.75f);
        coinObj.GetComponent<TrailRenderer>().material.color = newColor;
    }

    #endregion
    
}



















