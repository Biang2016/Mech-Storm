using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
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

        SaveStoryButton.onClick.AddListener(SaveStory);
        ResetStoryButton.onClick.AddListener(ResetStory);

        LanguageManager.Instance.RegisterTextKeys(
            new List<(Text, string)>
            {
                (StoryEditorWindowText, "StoryEditorPanel_StoryEditorWindowText"),
                (LanguageLabelText, "SettingMenu_Languages"),
                (SaveStoryButtonText, "StoryEditorPanel_SaveStoryButtonText"),
                (ResetStoryButtonText, "StoryEditorPanel_ResetStoryButtonText"),
                (ReturnToGamerButtonText, "StoryEditorPanel_ReturnToGamerButtonText"),
            });

        LanguageDropdown.ClearOptions();
        LanguageDropdown.AddOptions(LanguageManager.Instance.LanguageDescs);

        ReturnToGamerButton.onClick.AddListener(ReturnToGame);

        InitializeCardPropertyForm();
        InitializeLevelList();
    }

    void Start()
    {
        SetStory(AllStories.GetStory("DefaultStory", CloneVariantUtils.OperationType.Clone));
    }

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
        foreach (StoryEditorPanel_EnemyButton btn in MyEnemyButtons)
        {
            btn.OnLanguageChange();
        }
    }

    private void ReturnToGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    [SerializeField] private Text StoryEditorWindowText;
    [SerializeField] private Text LanguageLabelText;
    [SerializeField] private Dropdown LanguageDropdown;
    [SerializeField] private Button ReturnToGamerButton;
    [SerializeField] private Text ReturnToGamerButtonText;

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

        PropertyFormRow Row_StoryName = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorPanel_StoryNameLabelText", OnStoryNameChange, out SetStoryName);
        Row_GamePlaySettings = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.StoryPropertyForm_GamePlaySettings].AllocateGameObject<StoryPropertyForm_GamePlaySettings>(StoryPropertiesContainer);
        Row_GamePlaySettings.Initialize();
        Row_Chapters = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.StoryPropertyForm_Chapters].AllocateGameObject<StoryPropertyForm_Chapters>(StoryPropertiesContainer);
    }

    private PropertyFormRow GeneralizeRow(PropertyFormRow.CardPropertyFormRowType type, string labelKey, UnityAction<string> onValueChange, out UnityAction<string> setValue, List<string> dropdownOptionList = null, UnityAction<string> onButtonClick = null)
    {
        PropertyFormRow row = PropertyFormRow.BaseInitialize(type, StoryPropertiesContainer, labelKey, onValueChange, out setValue, dropdownOptionList, onButtonClick);
        MyPropertiesRows.Add(row);
        return row;
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
            delegate(Chapter chapter)
            {
                //TODO
                NoticeManager.Instance.ShowInfoPanelCenter(chapter.ChapterNames["zh"], 0, 1f);
                GenerateChapterMap(chapter.ChapterMapRoundCount);
            },
            onChapterMapRoundCountChange: GenerateChapterMap);
        return false;
    }

    [SerializeField] private Button SaveStoryButton;
    [SerializeField] private Button ResetStoryButton;
    [SerializeField] private Text SaveStoryButtonText;
    [SerializeField] private Text ResetStoryButtonText;

    public void SaveStory()
    {
        AllStories.RefreshStoryXML(Cur_Story);
        AllStories.ReloadStoryXML();
        SetStory(AllStories.GetStory("DefaultStory", CloneVariantUtils.OperationType.Clone));
        NoticeManager.Instance.ShowInfoPanelCenter("Success", 0, 1f);
    }

    public void ResetStory()
    {
    }

    #region Center ChapterMap

    [SerializeField] private ChapterMap ChapterMap;

    private void GenerateChapterMap(int roundCount)
    {
        float routeLength = 480.0f / (roundCount + 2);
        ChapterMap.Initialize(roundCount: roundCount, routeLength: routeLength, lineWidth: 4f);
    }

    #endregion

    #region Right Level List

    public Transform LevelListContainer;

    private List<StoryEditorPanel_EnemyButton> MyEnemyButtons = new List<StoryEditorPanel_EnemyButton>();

    public void InitializeLevelList()
    {
        foreach (StoryEditorPanel_EnemyButton btn in MyEnemyButtons)
        {
            btn.PoolRecycle();
        }

        MyEnemyButtons.Clear();

        foreach (Enemy enemy in AllLevels.EnemyDict.Values)
        {
            StoryEditorPanel_EnemyButton enemyButton = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.StoryEditorPanel_EnemyButton].AllocateGameObject<StoryEditorPanel_EnemyButton>(LevelListContainer);
            enemyButton.Initialize((Enemy) enemy.Clone());
            MyEnemyButtons.Add(enemyButton);
        }
    }

    #endregion
}