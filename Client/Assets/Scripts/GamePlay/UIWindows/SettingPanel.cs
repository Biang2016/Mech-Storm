using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
        UIType.IsClearStack = false;
        UIType.UIForm_LucencyType = UIFormLucencyTypes.Blur;
        UIType.UIForms_ShowMode = UIFormShowModes.HideOther;
        UIType.UIForms_Type = UIFormTypes.Fixed;

        LanguageDropdown.ClearOptions();
        LanguageDropdown.AddOptions(LanguageManager.Instance.LanguageDescs);

        MasterSlider.onValueChanged.AddListener(OnMasterSliderValueChange);
        SoundSlider.onValueChanged.AddListener(OnSoundSliderValueChange);
        BGMSlider.onValueChanged.AddListener(OnBGMSliderValueChange);

        LanguageManager.Instance.RegisterTextKey(SettingText, "SettingMenu_Settings");
        LanguageManager.Instance.RegisterTextKey(LanguageDropdownText, "SettingMenu_Languages");
        LanguageManager.Instance.RegisterTextKey(MasterSliderText, "SettingMenu_MasterSliderText");
        LanguageManager.Instance.RegisterTextKey(SoundSliderText, "SettingMenu_SoundSliderText");
        LanguageManager.Instance.RegisterTextKey(BGMSliderText, "SettingMenu_BGMSliderText");
    }

    void Start()
    {
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

    public override void Display()
    {
        base.Display();
        LanguageDropdown.onValueChanged.RemoveAllListeners();
        LanguageDropdown.value = LanguageManager.Instance.LanguagesShorts.IndexOf(LanguageManager.Instance.GetCurrentLanguage());
        LanguageDropdown.onValueChanged.AddListener(LanguageManager.Instance.LanguageDropdownChange);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            CloseUIForm();
            return;
        }

        bool isClickElseWhere = (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) || Input.GetMouseButtonDown(1);
        if (isClickElseWhere)
        {
            CloseUIForm();
        }
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