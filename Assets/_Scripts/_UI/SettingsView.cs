using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SettingsView
{
   private ISetter _settingController;
   [SerializeField] private GameObject _settingsPanel;
   [SerializeField] private Button CrossButton;
   [SerializeField] private Button ResartButton;
   [SerializeField] private Button SettingsButton;
   [SerializeField] private GameObject BackgroundMusic;
   [SerializeField] private Button MusicOnBtn;
   [SerializeField] private Button MusicOffBtn;


   public void SetDependency(ISetter _Controller)
   {
      _settingController = _Controller;
   }

   public void InitializeSettingsView()
   {
      RegisterCrossBtn();
      RegisterRestartBtn();
      RegisterSettingsBtn();
      RegisterMusicOnBtn();
      RegisterMusicOffBtn();
   }

   private void RegisterCrossBtn()
   {
      CrossButton.onClick.AddListener(OnClickCrossButton);
   }
    
   private void UnRegisterCrossBtn()
   {
      CrossButton.onClick.RemoveListener(OnClickCrossButton);
   }
    
   private void RegisterSettingsBtn()
   {
      SettingsButton.onClick.AddListener(OnClickSettingsButton);
   }
    
   private void UnRegisterSettingsBtn()
   {
      SettingsButton.onClick.RemoveListener(OnClickSettingsButton);
   }
    
   private void RegisterRestartBtn()
   {
      ResartButton.onClick.AddListener(OnClickRestartButton);
   }
    
   private void UnRegisterRestartBtn()
   {
      ResartButton.onClick.RemoveListener(OnClickRestartButton);
   }
    
   private void RegisterMusicOnBtn()
   {
      MusicOnBtn.onClick.AddListener(OnClickMusicOnButton);
   }
    
   private void UnRegisterMusicONBtn()
   {
      MusicOnBtn.onClick.RemoveListener(OnClickMusicOnButton);
   }
    
   private void RegisterMusicOffBtn()
   {
      MusicOffBtn.onClick.AddListener(OnClickMusicOffButton);
   }
    
   private void UnRegisterMusicOffBtn()
   {
      MusicOffBtn.onClick.RemoveListener(OnClickMusicOffButton);
   }
   

   void OnClickCrossButton()
   {
      _settingController.OnClickCross(_settingsPanel);
   }

   void OnClickSettingsButton()
   {   
      _settingController.OnSettingsBtn(_settingsPanel);
      
   }

   void OnClickRestartButton()
   {   
      _settingController.OnClickReload();
   }

   void OnClickMusicOnButton()
   {   
      _settingController.OnClickMusicON(MusicOnBtn, BackgroundMusic);
   }

   void OnClickMusicOffButton()
   {
      _settingController.OnClickMusicOFF(MusicOnBtn, BackgroundMusic);
   }
   

}
