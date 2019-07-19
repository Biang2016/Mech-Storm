using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BaseUIForm
{
    private SettingPanel()
    {
    }

    [SerializeField] private Text MasterSliderText;
    [SerializeField] private Text SoundSliderText;
    [SerializeField] private Text BGMSliderText;
    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider SoundSlider;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Text SettingText;

    [SerializeField] private Text LanguageDropdownText;
    public Dropdown LanguageDropdown;

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: true,
            isClickElsewhereClose: true,
            uiForms_Type: UIFormTypes.PopUp,
            uiForms_ShowMode: UIFormShowModes.ReturnHideOther,
            uiForm_LucencyType: UIFormLucencyTypes.Blur);

        LanguageDropdown.ClearOptions();
        LanguageDropdown.AddOptions(LanguageManager.Instance.LanguageDescs);

        MasterSlider.onValueChanged.AddListener(OnMasterSliderValueChange);
        SoundSlider.onValueChanged.AddListener(OnSoundSliderValueChange);
        BGMSlider.onValueChanged.AddListener(OnBGMSliderValueChange);

        LanguageManager.Instance.RegisterTextKeys(new List<ValueTuple<Text, string>>
        {
            (SettingText, "SettingMenu_Settings"),
            (LanguageDropdownText, "SettingMenu_Languages"),
            (MasterSliderText, "SettingMenu_MasterSliderText"),
            (SoundSliderText, "SettingMenu_SoundSliderText"),
            (BGMSliderText, "SettingMenu_BGMSliderText"),
        });

 float masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        float soundVolume = PlayerPrefs.GetFloat("SoundVolume");
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume");
        MasterSlider.value = masterVolume;
        OnMasterSliderValueChange(masterVolume);
        SoundSlider.value = soundVolume;
        OnSoundSliderValueChange(soundVolume);
        BGMSlider.value = bgmVolume;
        OnBGMSliderValueChange(soundVolume);
    }

    void Start()
    {
       
    }

    public override void Display()
    {
        base.Display();
        LanguageDropdown.onValueChanged.RemoveAllListeners();
        LanguageDropdown.value = LanguageManager.Instance.LanguagesShorts.IndexOf(LanguageManager.Instance.GetCurrentLanguage());
        LanguageDropdown.onValueChanged.AddListener(LanguageManager.Instance.LanguageDropdownChange);
        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.SettingMenu);
    }

    public override void Hide()
    {
        base.Hide();
        MouseHoverManager.Instance.M_StateMachine.ReturnToPreviousState();
    }

    private void OnMasterSliderValueChange(float value)
    {
        float volume = value;
        if (value.Equals(MasterSlider.minValue))
        {
            volume = -100;
        }

        AudioManager.Instance.AudioMixer.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    private void OnSoundSliderValueChange(float value)
    {
        float volume = value;
        if (value.Equals(SoundSlider.minValue))
        {
            volume = -100;
        }

        AudioManager.Instance.AudioMixer.SetFloat("SoundVolume", volume);
        PlayerPrefs.SetFloat("SoundVolume", volume);
    }

    private void OnBGMSliderValueChange(float value)
    {
        float volume = value;
        if (value.Equals(BGMSlider.minValue))
        {
            volume = -100;
        }

        AudioManager.Instance.AudioMixer.SetFloat("BGMVolume", volume);
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }
}