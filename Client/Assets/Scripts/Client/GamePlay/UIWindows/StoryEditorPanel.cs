using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoryEditorPanel : BaseUIForm
{
    private StoryEditorPanel()
    {
    }

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: false,
            isClickElsewhereClose: false,
            uiForms_Type: UIFormTypes.Normal,
            uiForms_ShowMode: UIFormShowModes.Normal,
            uiForm_LucencyType: UIFormLucencyTypes.ImPenetrable);

        SaveStoryButton.onClick.AddListener(delegate { SaveStory(); });
        ResetStoryButton.onClick.AddListener(ResetStory);

        LanguageManager.Instance.RegisterTextKeys(
            new List<(Text, string)>
            {
                (StoryEditorWindowText, "StoryEditorWindow_StoryEditorWindowText"),
                (LanguageLabelText, "SettingMenu_Languages"),
                (SaveStoryButtonText, "StoryEditorWindow_SaveStoryButtonText"),
                (ResetStoryButtonText, "StoryEditorWindow_ResetStoryButtonText"),
            });

        LanguageDropdown.ClearOptions();
        LanguageDropdown.AddOptions(LanguageManager.Instance.LanguageDescs);

        InitializeCardPropertyForm();
        InitializeLevelList();
    }

    void Start()
    {
        SetStory(BuildStoryDatabase.Instance.StoryStartDict["Story1"]);
    }

    public override void Display()
    {
        base.Display();
        LanguageDropdown.onValueChanged.RemoveAllListeners();
        LanguageDropdown.value = LanguageManager.Instance.LanguagesShorts.IndexOf(LanguageManager.Instance.GetCurrentLanguage());
        LanguageDropdown.onValueChanged.AddListener(LanguageManager.Instance.LanguageDropdownChange);
        LanguageDropdown.onValueChanged.AddListener(OnLanguageChange);
    }

    private void OnLanguageChange(int _)
    {
    }

    [SerializeField] private Text StoryEditorWindowText;
    [SerializeField] private Text LanguageLabelText;
    [SerializeField] private Dropdown LanguageDropdown;

    #region Left StoryProperties

    public Transform StoryPropertiesContainer;

    public Story Cur_Story;

    private List<PropertyFormRow> MyPropertiesRows = new List<PropertyFormRow>();
    private StoryPropertyForm_GamePlaySettings Row_GamePlaySettings;
    private StoryPropertyForm_Chapters Row_Chapters;

    private void InitializeCardPropertyForm()
    {
        foreach (PropertyFormRow pfr in MyPropertiesRows)
        {
            pfr.PoolRecycle();
        }

        MyPropertiesRows.Clear();
        Row_GamePlaySettings?.PoolRecycle();
        Row_Chapters?.PoolRecycle();
        MyPropertiesRows.Clear();

        PropertyFormRow Row_StoryName = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorWindow_StoryNameLabelText", OnStoryNameChange, out SetStoryName);
        Row_GamePlaySettings = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.StoryPropertyForm_GamePlaySettings].AllocateGameObject<StoryPropertyForm_GamePlaySettings>(StoryPropertiesContainer);
        Row_GamePlaySettings.Initialize();
        Row_Chapters = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.StoryPropertyForm_Chapters].AllocateGameObject<StoryPropertyForm_Chapters>(StoryPropertiesContainer);
    }

    private PropertyFormRow GeneralizeRow(PropertyFormRow.CardPropertyFormRowType type, string labelKey, UnityAction<string> onValueChange, out UnityAction<string> setValue, List<string> dropdownOptionList = null, UnityAction<string> onButtonClick = null)
    {
        PropertyFormRow cpfr = PropertyFormRow.BaseInitialize(type, StoryPropertiesContainer, labelKey, onValueChange, out setValue, dropdownOptionList, onButtonClick);
        MyPropertiesRows.Add(cpfr);
        return cpfr;
    }

    #endregion

    private UnityAction<string> SetStoryName;

    private void OnStoryNameChange(string value_str)
    {
        Cur_Story.StoryName = value_str;
    }

    public bool SetStory(Story story)
    {
        Cur_Story = story;
        Row_GamePlaySettings.SetGamePlaySettings(story.StoryGamePlaySettings);
        SetStoryName(story.StoryName);
        Row_Chapters.Initialize(Cur_Story.Chapters, onChangeSelectedChapter:
            delegate(Chapter chaper)
            {
                //TODO
                NoticeManager.Instance.ShowInfoPanelCenter(chaper.ChapterNames["zh"], 0, 1f);
            });
        return false;
    }

    public void SaveStory()
    {
    }

    public void ResetStory()
    {
    }

    #region Story Map

    [SerializeField] private Button SaveStoryButton;
    [SerializeField] private Button ResetStoryButton;
    [SerializeField] private Text SaveStoryButtonText;
    [SerializeField] private Text ResetStoryButtonText;

    void Update()
    {
#if PLATFORM_STANDALONE_OSX
        KeyCode controlKey = KeyCode.LeftCommand;
#elif PLATFORM_STANDALONE
        KeyCode controlKey = KeyCode.LeftControl;
#endif

        bool controlPress = Input.GetKey(controlKey);
        bool leftShiftPress = Input.GetKey(KeyCode.LeftShift);

        if (controlPress)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveStory();
            }
        }
    }

    #endregion

    #region Right Level List

    public Transform LevelListContainer;

    private List<StoryEditorPanel_LevelButton> MyLevelButtons = new List<StoryEditorPanel_LevelButton>();

    private void InitializeLevelList()
    {
        foreach (StoryEditorPanel_LevelButton btn in MyLevelButtons)
        {
            btn.PoolRecycle();
        }

        MyLevelButtons.Clear();

        foreach (BuildInfo bi in BuildStoryDatabase.Instance.BuildGroupDict[BuildGroups.EnemyBuilds].AllBuildInfo())
        {
            StoryEditorPanel_LevelButton levelButton = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.StoryEditorPanel_LevelButton].AllocateGameObject<StoryEditorPanel_LevelButton>(LevelListContainer);
            levelButton.Initialize(0, bi.BuildName, bi.CardCount, bi.Life, bi.Energy);
            MyLevelButtons.Add(levelButton);
        }
    }

    #endregion
}