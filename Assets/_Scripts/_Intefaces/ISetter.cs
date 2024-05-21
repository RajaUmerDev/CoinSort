using UnityEngine;
using UnityEngine.UI;

public interface ISetter
{
    void Initialize();
    void OnClickMusicON(Button MusicOnSprite, GameObject BackgroundMusic);

    void OnClickMusicOFF(Button MusicOnSprite, GameObject BackgroundMusic);

    void OnClickReload();

    void OnClickCross(GameObject _settingsPanel);

    void OnSettingsBtn(GameObject _settingsPanel);
    
}
