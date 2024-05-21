using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Score: MonoBehaviour, IScorer
{
    [SerializeField] public TMP_Text _coinText;
    [SerializeField] public int _coins;
    [SerializeField] private Image targetImage;
    [SerializeField] private TMP_Text targetScoreText;

    
    void IScorer.Initialize()
    {
        RefreshCoins();
        ResetTargetBar();
    }
    
    private void RefreshCoins()
    {
        _coins = PlayerPrefs.GetInt("Coins");
        _coinText.text = _coins.ToString();
    }
    
    private void ResetTargetBar()
    {
        float startScore = PlayerPrefs.GetFloat("TargetScore",0);
        targetScoreText.text = startScore.ToString();
        targetImage.fillAmount = (startScore/100);
    }

    int IScorer.GetCurrentScore()
    {    
        _coins = PlayerPrefs.GetInt("Coins");
        return _coins;
    }
    
    void IScorer.OnClickUnLockBtn(int coins)
    {
        int _coins = PlayerPrefs.GetInt("Coins");
        _coins -= coins;
        if (_coins < 0)
        {
            _coins = 0;
        }
        PlayerPrefs.SetInt("Coins",_coins);
        _coinText.text = _coins.ToString();
    }

    void IScorer.OnMergeSlotForTarget(int addedScore)
    {
        float startScore = PlayerPrefs.GetFloat("TargetScore",0);
        float targetScore = ((startScore + addedScore) <= 100) ? startScore + addedScore : 100;
        
        DOTween.To(x => targetImage.fillAmount = x, (startScore/100), (targetScore/100), 1f)
            .OnComplete(() => {
                targetScoreText.text = targetScore.ToString();
            });

        PlayerPrefs.SetFloat("TargetScore", targetScore);
    }
    void IScorer.OnMergeSlot(int coins)
    {
        int startingCoins = PlayerPrefs.GetInt("Coins");
        int targetCoins = startingCoins + coins;

        DOTween.To(() => startingCoins, x => startingCoins = x, targetCoins, 1f)
            .OnUpdate(() => {
                _coinText.text = startingCoins.ToString();
            });
    
        PlayerPrefs.SetInt("Coins", targetCoins);
    }

    void IScorer.OnDrawCoins(int coins)
    {
        int startingCoins = PlayerPrefs.GetInt("Coins");
        int targetCoins = startingCoins - coins;
        if (targetCoins < 0)
        {
            targetCoins = 0;
        }
        
        DOTween.To(() => startingCoins, x => startingCoins = x, targetCoins, 1f)
            .OnUpdate(() => {
                _coinText.text = startingCoins.ToString();
            });

        PlayerPrefs.SetInt("Coins", targetCoins);
    }
}

