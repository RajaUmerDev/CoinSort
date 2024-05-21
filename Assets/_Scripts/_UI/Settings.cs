using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour,ISetter
{
   
    [SerializeField] private SettingsView View;
    
    
    void  ISetter.Initialize()
    {    
        View.SetDependency(this);
        View.InitializeSettingsView();
    }

    public void OnClickMusicON(Button MusicOnSprite, GameObject BackgroundMusic)
    {
        MusicOnSprite.transform.gameObject.SetActive(false);
        BackgroundMusic.SetActive(false);
    }
    
    public void OnClickMusicOFF(Button MusicOnSprite, GameObject BackgroundMusic)
    {
        MusicOnSprite.transform.gameObject.SetActive(true);
        BackgroundMusic.SetActive(true);
    }
    
    public void OnClickReload()
    {
        SceneManager.LoadScene("Gameplay");
        Time.timeScale = 1;
    }

    public void OnClickCross(GameObject _settingsPanel)
    {
        _settingsPanel.SetActive(false);
        Time.timeScale = 1;
    }
    
    public void OnSettingsBtn(GameObject _settingsPanel)
    {
        Time.timeScale = 0;
        _settingsPanel.SetActive(true);
    }

}
