using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    [SerializeField] private Image _loadingFill;
    [SerializeField] private TMP_Text _loadingText;
    private void Start()
    {
        LoadGameScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void LoadGameScene(int sceneIndex)
    {
        DOTween.To(x => _loadingFill.fillAmount = x, 0, 1, 1.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => 
            {
                SceneManager.LoadSceneAsync(sceneIndex);
            });
    }

    private void Update()
    {
        _loadingText.text = Mathf.Round(_loadingFill.fillAmount*100)+" %";
    }
}
