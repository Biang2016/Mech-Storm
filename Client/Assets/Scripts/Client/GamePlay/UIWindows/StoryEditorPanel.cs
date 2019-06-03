using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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

        foreach (string s in Enum.GetNames(typeof(LevelType)))
        {
            LevelType lt = (LevelType) Enum.Parse(typeof(LevelType), s);
            LevelContainerDict.Add(lt, LevelListTabControl.AddTab("StoryEditorPanel_" + lt + "TabButtonTitle"));
            MyLevelButtons.Add(lt, new List<StoryEditorPanel_LevelButton>());
        }

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
        foreach (KeyValuePair<LevelType, List<StoryEditorPanel_LevelButton>> kv in MyLevelButtons)
        {
            foreach (StoryEditorPanel_LevelButton btn in kv.Value)
            {
                btn.OnLanguageChange();
            }
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

    [SerializeField] private Transform ChapterMapContainer;
    private ChapterMap ChapterMap;

    private void GenerateChapterMap(int roundCount)
    {
        ChapterMap oldChapterMap = ChapterMap;
        ChapterMap = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ChapterMap].AllocateGameObject<ChapterMap>(ChapterMapContainer);
        float routeLength = 520f / (roundCount + 2);
        ChapterMap.Initialize(roundCount: roundCount, routeLength: routeLength, lineWidth: 4f);
        ChapterMap.transform.localScale = Vector3.zero;

        StartCoroutine(Co_ChapterMapAnimation(oldChapterMap, ChapterMap));
    }

    IEnumerator Co_ChapterMapAnimation(ChapterMap oldMap, ChapterMap newMap)
    {
        if (oldMap)
        {
            oldMap.transform.DOScale(Vector3.one * 0.05f, 1f);
            oldMap.transform.DORotate(new Vector3(0, 0, 270f), 1f, RotateMode.FastBeyond360);

            yield return new WaitForSeconds(0.4f);
        }

        newMap.transform.localScale = Vector3.one * 3f;
        newMap.transform.rotation = Quaternion.Euler(0, 0, 180f);

        newMap.transform.DOScale(Vector3.one, 0.8f);
        newMap.transform.DORotate(new Vector3(0, 0, 360f), 0.8f, RotateMode.FastBeyond360).OnComplete(
            delegate { newMap.transform.rotation = Quaternion.Euler(0, 0, 0); });

        yield return new WaitForSeconds(0.7f);
        oldMap?.PoolRecycle();
    }

    #endregion

    #region Right Level List

    public TabControl LevelListTabControl;
    private Dictionary<LevelType, Transform> LevelContainerDict = new Dictionary<LevelType, Transform>();
    private SortedDictionary<LevelType, List<StoryEditorPanel_LevelButton>> MyLevelButtons = new SortedDictionary<LevelType, List<StoryEditorPanel_LevelButton>>();

    public void InitializeLevelList()
    {
        SelectTab(LevelType.Enemy);
        foreach (KeyValuePair<LevelType, List<StoryEditorPanel_LevelButton>> kv in MyLevelButtons)
        {
            foreach (StoryEditorPanel_LevelButton btn in kv.Value)
            {
                btn.PoolRecycle();
            }

            kv.Value.Clear();
        }

        foreach (KeyValuePair<LevelType, SortedDictionary<string, Level>> kv in AllLevels.LevelDict)
        {
            foreach (KeyValuePair<string, Level> _kv in kv.Value)
            {
                StoryEditorPanel_LevelButton btn = StoryEditorPanel_LevelButton.BaseInitialize(
                    level: _kv.Value.Clone(),
                    parent: LevelContainerDict[kv.Key],
                    onEditButtonClick: delegate
                    {
                        UIManager.Instance.CloseUIForm<StoryEditorPanel>();
                        UIManager.Instance.ShowUIForms<LevelEditorPanel>().SetLevel(_kv.Value.Clone());
                    },
                    onDeleteButtonClick: delegate
                    {
                        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                        cp.Initialize(LanguageManager.Instance.GetText("StoryEditorPanel_DeleteLevelFormLibrary0"),
                            LanguageManager.Instance.GetText("Common_Yes"),
                            LanguageManager.Instance.GetText("Common_No"),
                            delegate
                            {
                                cp.CloseUIForm();
                                AllLevels.DeleteLevel(LevelType.Enemy, _kv.Value.LevelNames["en"]);
                                InitializeLevelList();
                            },
                            delegate { cp.CloseUIForm(); });
                    }
                );
                MyLevelButtons[kv.Key].Add(btn);
            }
        }
    }

    private void SelectTab(LevelType levelType)
    {
        LevelListTabControl.SelectTab("StoryEditorPanel_" + levelType + "TabButtonTitle");
    }

    #endregion
}