using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelEditorPanel : BaseUIForm
{
    private LevelEditorPanel()
    {
    }

    [SerializeField] private PicSelectPanel PicSelectPanel;

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: false,
            isClickElsewhereClose: false,
            uiForms_Type: UIFormTypes.Normal,
            uiForms_ShowMode: UIFormShowModes.Normal,
            uiForm_LucencyType: UIFormLucencyTypes.ImPenetrable);

        SaveLevelButton.onClick.AddListener(SaveLevel);
        ResetLevelButton.onClick.AddListener(ResetLevel);
        ReturnToStoryEditorButton.onClick.AddListener(ReturnToStoryEditor);

        LanguageManager.Instance.RegisterTextKeys(
            new List<(Text, string)>
            {
                (LevelEditorWindowText, "LevelEditorPanel_LevelEditorWindowText"),
                (LanguageLabelText, "SettingMenu_Languages"),
                (SaveLevelButtonText, "LevelEditorPanel_SaveLevelButtonText"),
                (ResetLevelButtonText, "LevelEditorPanel_ResetLevelButtonText"),
                (ReturnToStoryEditorButtonText, "LevelEditorPanel_ReturnToStoryEditorButtonText"),
            });

        LanguageDropdown.ClearOptions();
        LanguageDropdown.AddOptions(LanguageManager.Instance.LanguageDescs);

        InitializeCardPropertyForm();
        InitializePreviewCardGrid();
        PicSelectPanel.OnClickPicAction = SetLevelPicID;
        PicSelectPanel.InitializePicSelectGrid("LevelEditorPanel_PicSelectGridLabel");
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
        LanguageDropdown.onValueChanged.AddListener(OnLanguageChange);

        OnLanguageChange(0);
    }

    private void OnLanguageChange(int _)
    {
        foreach (KeyValuePair<int, CardBase> kv in AllCards)
        {
            kv.Value.RefreshCardTextLanguage();
        }
    }

    private void ReturnToStoryEditor()
    {
        CloseUIForm();
        StoryEditorPanel sep = UIManager.Instance.ShowUIForms<StoryEditorPanel>();
        sep.InitializeLevelList();
    }

    [SerializeField] private Text LevelEditorWindowText;
    [SerializeField] private Text LanguageLabelText;
    [SerializeField] private Dropdown LanguageDropdown;
    [SerializeField] private Button ReturnToStoryEditorButton;
    [SerializeField] private Text ReturnToStoryEditorButtonText;

    #region Left LevelProperties

    public Transform LevelPropertiesContainer;

    public Level Cur_Level;

    private List<PropertyFormRow> MyPropertiesRows = new List<PropertyFormRow>();
    private Dictionary<LevelType, List<PropertyFormRow>> LevelTypePropertiesDict = new Dictionary<LevelType, List<PropertyFormRow>>();
    private Dictionary<EnemyType, List<PropertyFormRow>> EnemyTypePropertiesDict = new Dictionary<EnemyType, List<PropertyFormRow>>();
    private List<PropertyFormRow> LevelPropertiesCommon = new List<PropertyFormRow>();

    private LevelPropertyForm_CardSelection Row_CardSelection;

    private void InitializeCardPropertyForm()
    {
        foreach (PropertyFormRow pfr in MyPropertiesRows)
        {
            pfr.PoolRecycle();
        }

        MyPropertiesRows.Clear();

        IEnumerable<LevelType> types_level = Enum.GetValues(typeof(LevelType)) as IEnumerable<LevelType>;
        List<string> levelTypeList = new List<string>();
        foreach (LevelType levelType in types_level)
        {
            levelTypeList.Add(levelType.ToString());
            LevelTypePropertiesDict.Add(levelType, new List<PropertyFormRow>());
        }

        IEnumerable<EnemyType> types_enemy = Enum.GetValues(typeof(EnemyType)) as IEnumerable<EnemyType>;
        List<string> enemyTypeList = new List<string>();
        foreach (EnemyType enemyType in types_enemy)
        {
            enemyTypeList.Add(enemyType.ToString());
            EnemyTypePropertiesDict.Add(enemyType, new List<PropertyFormRow>());
        }

        PropertyFormRow Row_LevelType = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Dropdown, "LevelEditorPanel_LevelType", OnLevelTypeChange, out SetLevelType, levelTypeList);
        PropertyFormRow Row_LevelPicID = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_LevelPicIDLabelText", OnLevelPicIDChange, out SetLevelPicID);
        PropertyFormRow Row_LevelName_zh = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_LevelNameLabelText_zh", OnLevelNameChange_zh, out SetLevelName_zh);
        PropertyFormRow Row_LevelName_en = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_LevelNameLabelText_en", OnLevelNameChange_en, out SetLevelName_en);

        PropertyFormRow Row_EnemyType = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Dropdown, "LevelEditorPanel_EnemyType", OnEnemyTypeChange, out SetEnemyType, enemyTypeList);
        PropertyFormRow Row_EnemyDrawCardNum = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_EnemyDrawCardNum", OnEnemyDrawCardNumChange, out SetEnemyDrawCardNum);
        PropertyFormRow Row_EnemyLife = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_EnemyLife", OnEnemyLifeChange, out SetEnemyLife);
        PropertyFormRow Row_EnemyEnergy = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_EnemyEnergy", OnEnemyEnergyChange, out SetEnemyEnergy);
        PropertyFormRow Row_EnemyBeginMetal = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_EnemyBeginMetal", OnEnemyBeginMetalChange, out SetEnemyBeginMetal);

        Row_CardSelection = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelPropertyForm_CardSelection].AllocateGameObject<LevelPropertyForm_CardSelection>(LevelPropertiesContainer);

        LevelPropertiesCommon = new List<PropertyFormRow>
        {
            Row_LevelType,
            Row_LevelPicID,
            Row_LevelName_zh,
            Row_LevelName_en,
        };

        LevelTypePropertiesDict[LevelType.Enemy] = new List<PropertyFormRow>
        {
            Row_EnemyType,
            Row_EnemyDrawCardNum,
            Row_EnemyLife,
            Row_EnemyEnergy,
            Row_EnemyBeginMetal,
            Row_CardSelection,
        };

        LevelTypePropertiesDict[LevelType.Shop] = new List<PropertyFormRow>
        {
        };

        SetLevel(null);
    }

    private UnityAction<string> SetLevelType;

    private void OnLevelTypeChange(string value_str)
    {
        LevelType type = (LevelType) Enum.Parse(typeof(LevelType), value_str);
        foreach (PropertyFormRow lpfr in LevelPropertiesCommon)
        {
            lpfr.gameObject.SetActive(true);
        }

        List<PropertyFormRow> targets = LevelTypePropertiesDict[type];
        foreach (PropertyFormRow lpfr in MyPropertiesRows)
        {
            if (!LevelPropertiesCommon.Contains(lpfr))
            {
                lpfr.gameObject.SetActive(targets.Contains(lpfr));
            }
        }

        switch (type)
        {
            case LevelType.Enemy:
            {
                Cur_Level = new Enemy(
                    levelThemeCategory: LevelThemeCategory.Energy,
                    levelPicID: 0,
                    levelNames: new SortedDictionary<string, string> {{"zh", "新敌人"}, {"en", "newEnemy"}},
                    buildInfo: new BuildInfo(
                        buildID: -1,
                        buildName: "TempDeck",
                        buildCards: new BuildInfo.BuildCards(),
                        drawCardNum: 1,
                        life: 20,
                        energy: 10,
                        beginMetal: 1,
                        isHighLevelCardLocked: false,
                        gamePlaySettings: null),
                    enemyType: EnemyType.Soldier,
                    hardFactor: 100,
                    alwaysBonusGroup: new List<BonusGroup>(),
                    optionalBonusGroup: new List<BonusGroup>()
                );
                break;
            }
            case LevelType.Shop:
            {
                Cur_Level = new Shop(
                    levelThemeCategory: LevelThemeCategory.Energy,
                    levelPicId: 0,
                    levelNames: new SortedDictionary<string, string> {{"zh", "新商店"}, {"en", "newShop"}});
                break;
            }
        }

        SetLevel(Cur_Level);
    }

    private UnityAction<string> SetLevelPicID;

    private void OnLevelPicIDChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            Cur_Level.LevelPicID = value;
        }
    }

    private UnityAction<string> SetLevelName_zh;

    private void OnLevelNameChange_zh(string value_str)
    {
        Cur_Level.LevelNames["zh"] = value_str;
    }

    private UnityAction<string> SetLevelName_en;

    private void OnLevelNameChange_en(string value_str)
    {
        Cur_Level.LevelNames["en"] = value_str;
    }

    private UnityAction<string> SetEnemyType;

    private void OnEnemyTypeChange(string value_str)
    {
    }

    private UnityAction<string> SetEnemyDrawCardNum;

    private void OnEnemyDrawCardNumChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < 1)
            {
                SetEnemyDrawCardNum(GamePlaySettings.SystemMinDrawCardNum.ToString());
            }
            else if (value <= GamePlaySettings.SystemMaxDrawCardNum)
            {
                if (Cur_Level is Enemy enemy)
                {
                    enemy.BuildInfo.DrawCardNum = value;
                }
            }
            else
            {
                SetEnemyDrawCardNum(GamePlaySettings.SystemMaxDrawCardNum.ToString());
            }
        }
        else
        {
            SetEnemyDrawCardNum(GamePlaySettings.SystemMinDrawCardNum.ToString());
        }
    }

    private UnityAction<string> SetEnemyLife;

    private void OnEnemyLifeChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value <= 0)
            {
                SetEnemyLife(GamePlaySettings.SystemMinLife.ToString());
            }
            else if (value <= GamePlaySettings.SystemMaxLife)
            {
                if (Cur_Level is Enemy enemy)
                {
                    enemy.BuildInfo.Life = value;
                }
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(string.Format(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_EnemyLifeMax"), GamePlaySettings.SystemMaxLife), 0, 0.5f);
                SetEnemyLife(GamePlaySettings.SystemMaxLife.ToString());
            }
        }
        else
        {
            SetEnemyLife(GamePlaySettings.SystemMinLife.ToString());
        }
    }

    private UnityAction<string> SetEnemyEnergy;

    private void OnEnemyEnergyChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < 0)
            {
                SetEnemyEnergy(0.ToString());
            }
            else if (value <= GamePlaySettings.SystemMaxEnergy)
            {
                if (Cur_Level is Enemy enemy)
                {
                    enemy.BuildInfo.Energy = value;
                }
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(string.Format(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_EnemyEnergyMax"), GamePlaySettings.SystemMaxEnergy), 0, 0.5f);
                SetEnemyEnergy(GamePlaySettings.SystemMaxEnergy.ToString());
            }
        }
        else
        {
            SetEnemyEnergy(0.ToString());
        }
    }

    private UnityAction<string> SetEnemyBeginMetal;

    private void OnEnemyBeginMetalChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value <= 0)
            {
                SetEnemyBeginMetal(1.ToString());
            }
            else if (value <= GamePlaySettings.SystemMaxMetal)
            {
                if (Cur_Level is Enemy enemy)
                {
                    enemy.BuildInfo.BeginMetal = value;
                }
            }
            else
            {
                SetEnemyBeginMetal(GamePlaySettings.SystemMaxMetal.ToString());
            }
        }
        else
        {
            SetEnemyBeginMetal(1.ToString());
        }
    }

    private PropertyFormRow GeneralizeRow(PropertyFormRow.CardPropertyFormRowType type, string labelKey, UnityAction<string> onValueChange, out UnityAction<string> setValue, List<string> dropdownOptionList = null, UnityAction<string> onButtonClick = null)
    {
        PropertyFormRow cpfr = PropertyFormRow.BaseInitialize(type, LevelPropertiesContainer, labelKey, onValueChange, out setValue, dropdownOptionList, onButtonClick);
        MyPropertiesRows.Add(cpfr);
        return cpfr;
    }

    #endregion

    public bool SetLevel(Level level)
    {
        if (level == null)
        {
            SetLevelType(LevelType.Shop.ToString());
            SetLevelType(LevelType.Enemy.ToString());
        }
        else
        {
            Cur_Level = level;
            SetLevelName_en(Cur_Level.LevelNames["en"]);
            SetLevelName_zh(Cur_Level.LevelNames["zh"]);
            SetLevelPicID(Cur_Level.LevelPicID.ToString());
            switch (Cur_Level)
            {
                case Enemy enemy:
                {
                    SetEnemyType(enemy.EnemyType.ToString());
                    SetEnemyBeginMetal(enemy.BuildInfo.BeginMetal.ToString());
                    SetEnemyDrawCardNum(enemy.BuildInfo.DrawCardNum.ToString());
                    SetEnemyEnergy(enemy.BuildInfo.Energy.ToString());
                    SetEnemyLife(enemy.BuildInfo.Life.ToString());
                    Row_CardSelection.Initialize(enemy.BuildInfo.M_BuildCards, delegate { });
                    SelectCardsByBuildCards(enemy.BuildInfo.M_BuildCards);
                    break;
                }
                case Shop shop:
                {
                    break;
                }
            }
        }

        return false;
    }

    public void SaveLevel()
    {
        AllLevels.RefreshLevelXML(Cur_Level);
        AllLevels.ReloadLevelXML();
        NoticeManager.Instance.ShowInfoPanelCenter("Success", 0, 1f);
    }

    public void ResetLevel()
    {
    }

    #region Center CardLibrary

    [SerializeField] private Button SaveLevelButton;
    [SerializeField] private Button ResetLevelButton;
    [SerializeField] private Text SaveLevelButtonText;
    [SerializeField] private Text ResetLevelButtonText;

    private CardBase mouseLeftDownCard;
    private CardBase mouseRightDownCard;
    private Vector3 mouseDownPosition;

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
                SaveLevel();
            }
        }

        if (!PicSelectPanel.IsOpen && Input.GetMouseButtonDown(0))
        {
            Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 500f, GameManager.Instance.Layer_Cards);
            if (raycast.collider)
            {
                CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                if (card)
                {
                    mouseDownPosition = Input.mousePosition;
                    mouseLeftDownCard = card;
                }
            }
            else
            {
                mouseRightDownCard = null;
                mouseLeftDownCard = null;
            }
        }

        if (!PicSelectPanel.IsOpen && Input.GetMouseButtonDown(1))
        {
            Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 500f, GameManager.Instance.Layer_Cards);
            if (raycast.collider != null)
            {
                CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                if (card)
                {
                    mouseDownPosition = Input.mousePosition;
                    mouseRightDownCard = card;
                }
            }
            else
            {
                mouseRightDownCard = null;
                mouseLeftDownCard = null;
            }
        }

        if (!PicSelectPanel.IsOpen && Input.GetMouseButtonUp(0))
        {
            Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 500f, GameManager.Instance.Layer_Cards);
            if (raycast.collider != null)
            {
                CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                if (card)
                {
                    if ((Input.mousePosition - mouseDownPosition).magnitude < 50)
                    {
                        if (mouseLeftDownCard == card)
                        {
                            SelectCard(card);
                        }
                    }
                }
            }

            mouseLeftDownCard = null;
            mouseRightDownCard = null;
        }

        if (!PicSelectPanel.IsOpen && Input.GetMouseButtonUp(1))
        {
            Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 500f, GameManager.Instance.Layer_Cards);
            if (raycast.collider != null)
            {
                CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                if (Input.GetMouseButtonUp(1))
                {
                    if (card && mouseRightDownCard == card)
                    {
                        UnSelectCard(card);
                    }
                }
            }

            mouseLeftDownCard = null;
            mouseRightDownCard = null;
        }
    }

    private Dictionary<int, CardBase> AllCards = new Dictionary<int, CardBase>();
    public Dictionary<int, PoolObject> AllCardContainers = new Dictionary<int, PoolObject>(); // 每张卡片都有一个容器
    [SerializeField] private GridLayoutGroup CardLibraryGridLayout;

    private void InitializePreviewCardGrid()
    {
        foreach (CardInfo_Base cardInfo in global::AllCards.CardDict.Values)
        {
            if (cardInfo.CardID == 99999) continue;
            if (cardInfo.BaseInfo.IsHide) continue;
            if (cardInfo.BaseInfo.IsTemp) continue;
            AddCardIntoGridLayout(cardInfo.Clone());
        }
    }

    public void AddCardIntoGridLayout(CardInfo_Base cardInfo)
    {
        CardSelectWindowCardContainer newCardContainer = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CardSelectWindowCardContainer].AllocateGameObject<CardSelectWindowCardContainer>(CardLibraryGridLayout.transform);
        newCardContainer.Initialize(cardInfo);
        newCardContainer.M_ChildCard.BeDimColor();
        AllCards.Add(cardInfo.CardID, newCardContainer.M_ChildCard);
        AllCardContainers.Add(cardInfo.CardID, newCardContainer);
    }

    private BuildInfo.BuildCards BuildCards
    {
        get
        {
            if (Cur_Level is Enemy enemy)
            {
                return enemy.BuildInfo.M_BuildCards;
            }
            else if (Cur_Level is Shop shop)
            {
                return new BuildInfo.BuildCards();
            }

            return null;
        }
    }

    private void RefreshCard(CardBase card, int count)
    {
        card.SetBlockCountValue(count);
        if (count > 0)
        {
            card.BeBrightColor();
        }
        else
        {
            card.BeDimColor();
        }

        card.ShowCardBloom(count > 0);
    }

    private void SelectCard(CardBase card)
    {
        if (card.CardInfo.CardStatType == CardStatTypes.HeroMech && BuildCards.GetTypeCardCountDict()[CardStatTypes.HeroMech] >= 4)
        {
            return;
        }

        BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectCount++;
        int count = BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectCount;
        RefreshCard(card, count);
        Row_CardSelection.Refresh();
    }

    private void UnSelectCard(CardBase card)
    {
        if (BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectCount == 0) return;
        BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectCount--;
        int count = BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectCount;
        RefreshCard(card, count);
        Row_CardSelection.Refresh();
    }

    private void UnSelectAllCards(bool clearBuildCard)
    {
        if (clearBuildCard)
        {
            foreach (KeyValuePair<int, BuildInfo.BuildCards.CardSelectInfo> kv in BuildCards.CardSelectInfos)
            {
                kv.Value.CardSelectCount = 0;
            }
        }

        foreach (KeyValuePair<int, CardBase> kv in AllCards)
        {
            RefreshCard(kv.Value, 0);
        }

        if (clearBuildCard) Row_CardSelection.Refresh();
    }

    private void SelectCardsByBuildCards(BuildInfo.BuildCards buildCards)
    {
        UnSelectAllCards(false);
        foreach (KeyValuePair<int, BuildInfo.BuildCards.CardSelectInfo> kv in buildCards.CardSelectInfos)
        {
            if (AllCards.ContainsKey(kv.Key))
            {
                RefreshCard(AllCards[kv.Key], kv.Value.CardSelectCount);
            }
        }

        Row_CardSelection.Refresh();
    }

    #endregion

    #region Right Selection

    #endregion
}