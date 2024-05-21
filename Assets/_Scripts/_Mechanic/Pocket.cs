using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;

[Serializable]
public class Pocket: MonoBehaviour
{

    public int SlotIndex;
    [HideInInspector] public List<GameObject> Coins;
    [HideInInspector] public List<Coin> CoinList;
    public List<SlotPositions> SlotPos;
    public bool isLocked;
    public bool isDisabled;
    public GameObject disabledLockedImage;
    public GameObject enabledLockedImage;
    private int _lastIndex=0;
    private int CoinsToSpawn;
    private ColorType.Color ColorTypo;
    private ITray Tray;
    private List<Coin> CoinPrefabList= new List<Coin>();
    [SerializeField] private SelectTag.Tag selectTag;

    public void Initialize()
    {    
        LoadCoins();
        SetPositionOfLockImages();
    }

    #region GetSlotPosition

     public Vector3 GetSlotPosition(int index, int lastIndex)
        {
            var position = SlotPos[index].sloTransforms[lastIndex].position;
            return position;
        }

    #endregion

    #region AddCoinsToPocket

    public void AddCoinsToPocket(ITray trayInterface, int noOfCoins, ColorType.Color colorType,int pocketIndex)
    {
        Tray = trayInterface;
        CoinsToSpawn = noOfCoins;
        ColorTypo = colorType;

        if(pocketIndex == SlotIndex)
        {
            InitializeCoins(); 
        }

        //Tray.MutualFunction();
    }

    #endregion

    #region Initialize Coins

    public void InitializeCoins()
    {
        if (ColorTypo != ColorType.Color.None)
        {
            int currentIndex = Coins.Count;
            for (int i = currentIndex; i < (CoinsToSpawn+currentIndex); i++)
            {
                GameObject InstObject = Instantiate(Utils.LoadCoinByColorType("Coins", ColorTypo).gameObject, SlotPos[SlotIndex].sloTransforms[i].position, Quaternion.identity);
                InstObject.transform.SetParent(this.gameObject.transform);
                Coins.Add(InstObject);
                CoinList.Add(Utils.LoadCoinByColorType("Coins",ColorTypo));
            }
            _lastIndex = CoinsToSpawn;
        }
        CoinsToSpawn = 0;
        ColorTypo = ColorType.Color.None;
    }
    
    public Coin GetObjByColorType(ColorType.Color colorType)
    {
        foreach (var coin in CoinPrefabList)
        {
            if (coin.ColorType == colorType)
            {
                return coin;
            }
        }
        return null;
    }
    
    private void LoadCoins()
    {
        CoinPrefabList = Utils.LoadAllCoins("Coins").ToList();
    }

    #endregion

    #region LockSystem

    private void SetPositionOfLockImages()
    {    
        Vector3 lockPos1 = this.transform.position;
        Vector3 lockPos2 = this.transform.position;
        lockPos1.y = 0.38f;
        lockPos2.y = 0.39f;
        enabledLockedImage.transform.position = lockPos1;
        disabledLockedImage.transform.position = lockPos2;
    }

    public void UnlockSlot()
    {
        isLocked = false;
        //disabledLockedImage.SetActive(false);
        enabledLockedImage.SetActive(false);
    }

    public void EnableSlot()
    {
        isDisabled = false;
        enabledLockedImage.SetActive(true);
        disabledLockedImage.SetActive(false);
    }
    
    public void LockSlot()
    {
        isLocked = true;
        disabledLockedImage.SetActive(true);
        enabledLockedImage.SetActive(true);
    }

    public void DisableSlot()
    {
        isDisabled = true;
        disabledLockedImage.SetActive(true);
    }

    public void UnlockBoth()
    {
        isLocked = false;
        isDisabled = false;
        disabledLockedImage.SetActive(false);
        enabledLockedImage.SetActive(false);
    }
    
    public void LockedBoth()
    {
        isLocked = true;
        isDisabled = true;
        disabledLockedImage.SetActive(true);
        enabledLockedImage.SetActive(true);
    }

    public void SetTagSelectable()
    {
        selectTag = SelectTag.Tag.Selectable;
        string pocketTag = selectTag.ToString();
        gameObject.tag = pocketTag;
    }
    
    public void SetTagUnSelectable()
    {
        selectTag = SelectTag.Tag.UnSelectable;
        string pocketTag = selectTag.ToString();
        gameObject.tag = pocketTag;
    }

    #endregion
  
}












