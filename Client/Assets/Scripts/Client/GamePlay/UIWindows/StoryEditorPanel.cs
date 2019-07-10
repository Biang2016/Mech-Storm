using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
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
                (NextChapterButtonText, "StoryEditorPanel_NextChapterButtonText"),
                (PreviousChapterButtonText, "StoryEditorPanel_PreviousChapterButtonText"),
            });

        LanguageDropdown.ClearOptions();
        LanguageDropdown.AddOptions(LanguageManager.Instance.LanguageDescs);

        ReturnToGamerButton.onClick.AddListener(ReturnToGame);
        SaveChapterButton.onClick.AddListener(SaveChapter);
        NextChapterButton.onClick.AddListener(SwitchToNextChapter);
        PreviousChapterButton.onClick.AddListener(SwitchToPreviousChapter);

        InitializeCardPropertyForm();

        foreach (string s in Enum.GetNames(typeof(LevelType)))
        {
            LevelType lt = (LevelType) Enum.Parse(typeof(LevelType), s);
            LevelContainerDict.Add(lt, LevelListTabControl.AddTab(
                tabTitleStrKey: "StoryEditorPanel_" + lt + "TabButtonTitle",
                onAddButtonClick: delegate
                {
                    Level newLevel = Level.BaseGenerateEmptyLevel(lt);
                    InitializeLevelList();
                    SelectTab(lt);
                    UIManager.Instance.CloseUIForm<StoryEditorPanel>();
                    UIManager.Instance.ShowUIForms<LevelEditorPanel>().SetLevel(newLevel.Clone());
                }));
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

        if (controlPress)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveStory();
            }
        }

        if (CardSelectPanel.gameObject.activeInHierarchy)
        {
            CardSelectPanel.CardSelectPanelUpdate();
        }
    }

    public override void Display()
    {
        base.Display();
        LanguageDropdown.onValueChanged.RemoveAllListeners();
        LanguageDropdown.value = LanguageManager.Instance.LanguagesShorts.IndexOf(LanguageManager.Instance.GetCurrentLanguage());
        LanguageDropdown.onValueChanged.AddListener(LanguageManager.Instance.LanguageDropdownChange);
        LanguageDropdown.onValueChanged.AddListener(OnLanguageChange);
        OnLanguageChange(0);
        CardSelectPanel.Initialize(Editor_CardSelectModes.UpperLimit, SelectCard, UnSelectCard, SelectOneForEachActiveCards, UnSelectAllActiveCards, Row_CardSelection);
        CardSelectPanel.gameObject.SetActive(false);
        ChapterMapContainer.gameObject.SetActive(true);
    }

    public override void Hide()
    {
        CardSelectPanel.RecycleAllCards();
        base.Hide();
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
        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
        cp.Initialize(
            LanguageManager.Instance.GetText("Notice_ReturnWarningSave"),
            LanguageManager.Instance.GetText("Common_Yes"),
            LanguageManager.Instance.GetText("Common_No"),
            leftButtonClick: delegate
            {
                SceneManager.LoadScene("MainScene");
                cp.CloseUIForm();
            },
            rightButtonClick: delegate { cp.CloseUIForm(); });
    }

    [SerializeField] private Text StoryEditorWindowText;
    [SerializeField] private Text LanguageLabelText;
    [SerializeField] private Dropdown LanguageDropdown;
    [SerializeField] private Button ReturnToGamerButton;
    [SerializeField] private Button SaveChapterButton;
    [SerializeField] private Text ReturnToGamerButtonText;

    #region Left StoryProperties

    public Transform StoryPropertiesContainer;

    public Story Cur_Story;

    private List<PropertyFormRow> MyPropertiesRows = new List<PropertyFormRow>();
    private StoryPropertyForm_GamePlaySettings Row_GamePlaySettings;
    private StoryPropertyForm_Chapters Row_Chapters;
    private LevelPropertyForm_CardSelection Row_CardSelection;

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
        Row_CardSelection = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelPropertyForm_CardSelection].AllocateGameObject<LevelPropertyForm_CardSelection>(StoryPropertiesContainer);
        MyPropertiesRows.Add(Row_CardSelection);
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

    private Story resetStoryBackup;

    private void SetStory(Story story)
    {
        resetStoryBackup = story.Clone();
        Cur_Story = story;
        Row_GamePlaySettings.SetGamePlaySettings(story.StoryGamePlaySettings);
        SetStoryName(story.StoryName);
        Row_Chapters.Initialize(
            chapters: Cur_Story.Chapters,
            gotoAction: delegate
            {
                CardSelectPanel.gameObject.SetActive(false);
                ChapterMapContainer.gameObject.SetActive(true);
            },
            onChangeSelectedChapter:
            delegate(Chapter chapter, bool showAnimation)
            {
                //TODO
                NoticeManager.Instance.ShowInfoPanelCenter(chapter.ChapterNames[LanguageManager.Instance.GetCurrentLanguage()], 0, 1f);
                GenerateChapterMap(chapter, showAnimation);
            },
            onRefreshStory: delegate { SetStory(story); },
            onRefreshChapterTitle: OnRefreshChapterTitle);

        CardSelectPanel.SetBuildCards(
            buildCards: BuildCards,
            gotoAction: delegate
            {
                CardSelectPanel.gameObject.SetActive(true);
                ChapterMapContainer.gameObject.SetActive(false);
            });
    }

    [SerializeField] private Button SaveStoryButton;
    [SerializeField] private Button ResetStoryButton;
    [SerializeField] private Text SaveStoryButtonText;
    [SerializeField] private Text ResetStoryButtonText;

    private void SaveStory()
    {
        ChapterMap?.SaveChapter();
        AllStories.RefreshStoryXML(Cur_Story);
        AllStories.ReloadStoryXML();
        SetStory(AllStories.GetStory("DefaultStory", CloneVariantUtils.OperationType.Clone));
        NoticeManager.Instance.ShowInfoPanelCenter("Success", 0, 1f);
    }

    public void RefreshStory()
    {
        SetStory(AllStories.GetStory("DefaultStory", CloneVariantUtils.OperationType.Clone));
    }

    private void ResetStory()
    {
        NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_StoryEditorPanel_ResetSuccess"), 0f, 1f);
        SetStory(resetStoryBackup);
        StartCoroutine(ClientUtils.UpdateLayout((RectTransform) StoryPropertiesContainer));
    }

    #region Center ChapterMap

    [SerializeField] private Transform ChapterMapContainer;
    [SerializeField] private TextMeshProUGUI ChapterNameText;
    private ChapterMap ChapterMap;

    private void GenerateChapterMap(Chapter chapter, bool showAnimation = true)
    {
        OnRefreshChapterTitle(chapter);
        ChapterMap oldChapterMap = ChapterMap;
        ChapterMap = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ChapterMap].AllocateGameObject<ChapterMap>(ChapterMapContainer);
        ChapterMap.Initialize(chapter);
        if (showAnimation)
        {
            ChapterMap.transform.localScale = Vector3.zero;
            StartCoroutine(Co_ChapterMapAnimation(oldChapterMap, ChapterMap));
        }
        else
        {
            oldChapterMap?.PoolRecycle();
        }
    }

    private void OnRefreshChapterTitle(Chapter chapter)
    {
        string chapterTitle = string.Format(LanguageManager.Instance.GetText("StoryEditorPanel_ChapterTitle"), chapter.ChapterID + 1, chapter.ChapterNames[LanguageManager.Instance.GetCurrentLanguage()]);
        ChapterNameText.text = chapterTitle;
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

    private void SaveChapter()
    {
        ChapterMap?.SaveChapter();
        NoticeManager.Instance.ShowInfoPanelCenter("Success", 0, 1f);
    }

    [SerializeField] private Button NextChapterButton;
    [SerializeField] private Text NextChapterButtonText;
    [SerializeField] private Button PreviousChapterButton;
    [SerializeField] private Text PreviousChapterButtonText;

    private void SwitchToNextChapter()
    {
        if (ChapterMap.Cur_Chapter.ChapterID < Cur_Story.Chapters.Count - 1)
        {
            Row_Chapters.OnSelectedRow(ChapterMap.Cur_Chapter.ChapterID + 1);
        }
    }

    private void SwitchToPreviousChapter()
    {
        if (ChapterMap.Cur_Chapter.ChapterID > 0)
        {
            Row_Chapters.OnSelectedRow(ChapterMap.Cur_Chapter.ChapterID - 1);
        }
    }

    #endregion

    #region Center CardLibrary

    [SerializeField] private CardSelectPanel CardSelectPanel;

    private BuildCards BuildCards
    {
        get
        {
            if (Cur_Story?.PlayerBuildInfos != null && Cur_Story.PlayerBuildInfos.Count != 0)
            {
                BuildInfo startBuildInfo = Cur_Story.PlayerBuildInfos.Values.ToList()[0];
                if (startBuildInfo != null)
                {
                    return startBuildInfo.M_BuildCards;
                }
            }

            return null;
        }
    }

    private void SelectCard(CardBase card)
    {
        if (BuildCards != null)
        {
            BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectUpperLimit++;
            int count = BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectUpperLimit;
            CardSelectPanel.RefreshCard(card.CardInfo.CardID, count);
            Row_CardSelection.Refresh();
        }
    }

    private void SelectOneForEachActiveCards(List<int> activeCardIDs)
    {
        if (BuildCards != null)
        {
            foreach (int cardID in activeCardIDs)
            {
                BuildCards.CardSelectInfo csi = BuildCards.CardSelectInfos[cardID];
                csi.CardSelectUpperLimit++;
                int count = csi.CardSelectUpperLimit;
                CardSelectPanel.RefreshCard(cardID, count);
            }

            Row_CardSelection.Refresh();
        }
    }

    private void UnSelectCard(CardBase card)
    {
        if (BuildCards != null)
        {
            if (BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectUpperLimit == 0) return;
            BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectUpperLimit--;
            int count = BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectUpperLimit;
            CardSelectPanel.RefreshCard(card.CardInfo.CardID, count);
            Row_CardSelection.Refresh();
        }
    }

    private void UnSelectAllActiveCards(List<int> activeCardIDs)
    {
        if (BuildCards != null)
        {
            foreach (int cardID in activeCardIDs)
            {
                BuildCards.CardSelectInfo csi = BuildCards.CardSelectInfos[cardID];
                csi.CardSelectCount = 0;
                csi.CardSelectUpperLimit = 0;
                CardSelectPanel.RefreshCard(cardID, 0);
            }

            Row_CardSelection.Refresh();
        }
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
                if (_kv.Key.Equals("New" + kv.Key)) continue;
                StoryEditorPanel_LevelButton btn = StoryEditorPanel_LevelButton.BaseInitialize(
                    level: _kv.Value.Clone(),
                    parent: LevelContainerDict[kv.Key],
                    onSetButtonClick: delegate(Level level) { ChapterMap.SetCurrentNodeLevel(level); },
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
                                AllLevels.DeleteLevel(kv.Key, _kv.Value.LevelNames["en"]);
                                InitializeLevelList();
                                SelectTab(kv.Key);
                                SetStory(AllStories.GetStory("DefaultStory", CloneVariantUtils.OperationType.Clone));
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