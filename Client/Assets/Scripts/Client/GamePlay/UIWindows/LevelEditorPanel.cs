using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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
        CardEditorButton.onClick.AddListener(GoToCardEditorPanel);

        LanguageManager.Instance.RegisterTextKeys(
            new List<(Text, string)>
            {
                (LevelEditorWindowText, "LevelEditorPanel_LevelEditorWindowText"),
                (LanguageLabelText, "SettingMenu_Languages"),
                (SaveLevelButtonText, "LevelEditorPanel_SaveLevelButtonText"),
                (ResetLevelButtonText, "LevelEditorPanel_ResetLevelButtonText"),
                (ReturnToStoryEditorButtonText, "LevelEditorPanel_ReturnToStoryEditorButtonText"),
                (CardEditorButtonText, "StoryEditorPanel_CardEditorButtonText"),
            });

        LanguageDropdown.ClearOptions();
        LanguageDropdown.AddOptions(LanguageManager.Instance.LanguageDescs);

        InitializeCardPropertyForm();
        PicSelectPanel.OnClickPicAction = SetLevelPicID;
        PicSelectPanel.OnOpenPanelAction = delegate { CardSelectPanel.SetCardLibraryPanelEnable(false); };
        PicSelectPanel.OnClosePanelAction = delegate { CardSelectPanel.SetCardLibraryPanelEnable(true); };
        PicSelectPanel.InitializePicSelectGrid("LevelEditorPanel_PicSelectGridLabel");
    }

    public override void Display()
    {
        base.Display();
        LanguageDropdown.onValueChanged.RemoveAllListeners();
        LanguageDropdown.value = LanguageManager.Instance.LanguagesShorts.IndexOf(LanguageManager.Instance.GetCurrentLanguage());
        LanguageDropdown.onValueChanged.AddListener(LanguageManager.Instance.LanguageDropdownChange);
        LanguageDropdown.onValueChanged.AddListener(OnLanguageChange);

        OnLanguageChange(0);
        CardSelectPanel.Initialize(Editor_CardSelectModes.SelectCount, true, true, SelectCard, UnSelectCard, AddCardToCardPriorityComboList, SelectOneForEachActiveCards, UnSelectAllActiveCards, Row_CardSelection);
    }

    public override void Hide()
    {
        CardSelectPanel.RecycleAllCards();
        base.Hide();
    }

    private void OnLanguageChange(int _)
    {
        CardSelectPanel.OnLanguageChange(_);
        Row_ShopItems.OnLanguageChange();
        Row_BonusGroups.OnLanguageChange();
        LevelEditorPanel_CardComboList.OnLanguageChange();
        LevelEditorPanel_CardPriority.OnLanguageChange();
    }

    private void ReturnToStoryEditor()
    {
        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
        cp.Initialize(
            LanguageManager.Instance.GetText("Notice_ReturnWarningSave"),
            LanguageManager.Instance.GetText("Common_Yes"),
            LanguageManager.Instance.GetText("Common_No"),
            leftButtonClick: delegate
            {
                CloseUIForm();
                StoryEditorPanel sep = UIManager.Instance.ShowUIForms<StoryEditorPanel>();
                sep.InitializeLevelList();
                sep.RefreshStory();
                cp.CloseUIForm();
            },
            rightButtonClick: delegate { cp.CloseUIForm(); });
    }

    private void GoToCardEditorPanel()
    {
        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
        cp.Initialize(
            LanguageManager.Instance.GetText("Notice_ReturnWarningSave"),
            LanguageManager.Instance.GetText("Common_Yes"),
            LanguageManager.Instance.GetText("Common_No"),
            leftButtonClick: delegate
            {
                SceneManager.LoadScene("CardEditorScene");
                cp.CloseUIForm();
            },
            rightButtonClick: delegate { cp.CloseUIForm(); });
    }

    private void SelectOneForEachActiveCards(List<int> activeCardIDs)
    {
        if (Enemy_BuildCards != null)
        {
            foreach (int cardID in activeCardIDs)
            {
                BuildCards.CardSelectInfo csi = Enemy_BuildCards.CardSelectInfos[cardID];
                csi.CardSelectCount++;
                int count = csi.CardSelectCount;
                CardSelectPanel.RefreshCard(cardID, count);
            }

            Row_CardSelection.Refresh();
        }
    }

    private void UnSelectAllActiveCards(List<int> activeCardIDs)
    {
        if (Enemy_BuildCards != null)
        {
            foreach (int cardID in activeCardIDs)
            {
                BuildCards.CardSelectInfo csi = Enemy_BuildCards.CardSelectInfos[cardID];
                csi.CardSelectCount = 0;
                csi.CardSelectUpperLimit = 0;
                CardSelectPanel.RefreshCard(cardID, 0);
            }

            Row_CardSelection.Refresh();
        }
    }

    [SerializeField] private Text LevelEditorWindowText;
    [SerializeField] private Text LanguageLabelText;
    [SerializeField] private Dropdown LanguageDropdown;
    [SerializeField] private Button ReturnToStoryEditorButton;
    [SerializeField] private Text ReturnToStoryEditorButtonText;
    [SerializeField] private Button CardEditorButton;
    [SerializeField] private Text CardEditorButtonText;

    #region Left LevelProperties

    public Transform LevelPropertiesContainer;

    public Level Cur_Level;

    private List<PropertyFormRow> MyPropertiesRows = new List<PropertyFormRow>();
    private Dictionary<LevelTypes, List<PropertyFormRow>> LevelTypePropertiesDict = new Dictionary<LevelTypes, List<PropertyFormRow>>();
    private Dictionary<EnemyType, List<PropertyFormRow>> EnemyTypePropertiesDict = new Dictionary<EnemyType, List<PropertyFormRow>>();
    private List<PropertyFormRow> LevelPropertiesCommon = new List<PropertyFormRow>();

    private LevelPropertyForm_CardSelection Row_CardSelection;
    private LevelPropertyForm_ShopItems Row_ShopItems;
    private LevelPropertyForm_BonusGroups Row_BonusGroups;

    private void InitializeCardPropertyForm()
    {
        Clear();

        IEnumerable<LevelTypes> types_level = Enum.GetValues(typeof(LevelTypes)) as IEnumerable<LevelTypes>;
        List<string> levelTypeList = new List<string>();
        foreach (LevelTypes levelType in types_level)
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
        Row_LevelType.SetReadOnly(true);
        PropertyFormRow Row_LevelPicID = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_LevelPicIDLabelText", OnLevelPicIDChange, out SetLevelPicID);
        Row_LevelPicID.SetReadOnly(true);
        PropertyFormRow Row_LevelName_zh = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_LevelNameLabelText_zh", OnLevelNameChange_zh, out SetLevelName_zh);
        PropertyFormRow Row_LevelName_en = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_LevelNameLabelText_en", OnLevelNameChange_en, out SetLevelName_en);
        PropertyFormRow Row_LevelDifficultyLevel = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_LevelDifficultyLevel", OnLevelDifficultyLevelChange, out SetLevelDifficultyLevel);

        PropertyFormRow Row_EnemyType = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Dropdown, "LevelEditorPanel_EnemyType", OnEnemyTypeChange, out SetEnemyType, enemyTypeList);
        PropertyFormRow Row_EnemyDrawCardNum = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_EnemyDrawCardNum", OnEnemyDrawCardNumChange, out SetEnemyDrawCardNum);
        PropertyFormRow Row_EnemyLife = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_EnemyLife", OnEnemyLifeChange, out SetEnemyLife);
        PropertyFormRow Row_EnemyEnergy = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_EnemyEnergy", OnEnemyEnergyChange, out SetEnemyEnergy);
        PropertyFormRow Row_EnemyBeginMetal = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_EnemyBeginMetal", OnEnemyBeginMetalChange, out SetEnemyBeginMetal);

        PropertyFormRow Row_ShopItemCardCount = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_ShopItemCardCount", OnShopItemCardCountChange, out SetShopItemCardCount);
        PropertyFormRow Row_ShopItemOthersCount = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorPanel_ShopItemOthersCount", OnShopItemOthersCount, out SetShopItemOthersCount);

        Row_CardSelection = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelPropertyForm_CardSelection].AllocateGameObject<LevelPropertyForm_CardSelection>(LevelPropertiesContainer);
        MyPropertiesRows.Add(Row_CardSelection);
        Row_ShopItems = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelPropertyForm_ShopItems].AllocateGameObject<LevelPropertyForm_ShopItems>(LevelPropertiesContainer);
        MyPropertiesRows.Add(Row_ShopItems);
        Row_BonusGroups = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelPropertyForm_BonusGroups].AllocateGameObject<LevelPropertyForm_BonusGroups>(LevelPropertiesContainer);
        MyPropertiesRows.Add(Row_BonusGroups);

        LevelPropertiesCommon = new List<PropertyFormRow>
        {
            Row_LevelType,
            Row_LevelPicID,
            Row_LevelName_zh,
            Row_LevelName_en,
            Row_LevelDifficultyLevel,
        };

        LevelTypePropertiesDict[LevelTypes.Enemy] = new List<PropertyFormRow>
        {
            Row_EnemyType,
            Row_EnemyDrawCardNum,
            Row_EnemyLife,
            Row_EnemyEnergy,
            Row_EnemyBeginMetal,
            Row_CardSelection,
            Row_BonusGroups
        };

        LevelTypePropertiesDict[LevelTypes.Shop] = new List<PropertyFormRow>
        {
            Row_ShopItems,
            Row_ShopItemCardCount,
            Row_ShopItemOthersCount,
        };

        SetLevel(null);
    }

    private PropertyFormRow GeneralizeRow(PropertyFormRow.CardPropertyFormRowType type, string labelKey, UnityAction<string> onValueChange, out UnityAction<string, bool> setValue, List<string> dropdownOptionList = null, UnityAction<string> onButtonClick = null)
    {
        PropertyFormRow cpfr = PropertyFormRow.BaseInitialize(type, LevelPropertiesContainer, labelKey, onValueChange, out setValue, dropdownOptionList, onButtonClick);
        MyPropertiesRows.Add(cpfr);
        return cpfr;
    }

    private void Clear()
    {
        Cur_Level = null;
        Row_CardSelection = null;
        Row_ShopItems = null;
        Row_BonusGroups = null;

        foreach (PropertyFormRow pfr in MyPropertiesRows)
        {
            pfr.PoolRecycle();
        }

        MyPropertiesRows.Clear();
        LevelTypePropertiesDict.Clear();
        EnemyTypePropertiesDict.Clear();
        LevelPropertiesCommon.Clear();
    }

    private UnityAction<string, bool> SetLevelType;

    private void OnLevelTypeChange(string value_str)
    {
        LevelTypes type = (LevelTypes) Enum.Parse(typeof(LevelTypes), value_str);
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

        if (OnChangeLevelTypeByEdit)
        {
            OnChangeLevelTypeByEdit = false;
            LevelEditorPanel_CardComboList.gameObject.SetActive(type == LevelTypes.Enemy);

            switch (type)
            {
                case LevelTypes.Enemy:
                {
                    Cur_Level = new Enemy(
                        levelPicID: 0,
                        levelNames: new SortedDictionary<string, string> {{"zh", "新敌人"}, {"en", "newEnemy"}},
                        difficultyLevel: 1,
                        buildInfo: new BuildInfo(
                            buildID: -1,
                            buildName: "TempDeck",
                            buildCards: new BuildCards(BuildCards.DefaultCardLimitNumTypes.BasedOnCardBaseInfoLimitNum),
                            drawCardNum: 1,
                            life: 20,
                            energy: 10,
                            beginMetal: 1,
                            gamePlaySettings: null),
                        enemyType: EnemyType.Soldier,
                        bonusGroups: new List<BonusGroup>(),
                        cardComboList: new List<CardCombo>(),
                        cardPriority: new CardPriority(new List<int>())
                    );
                    LevelEditorPanel_CardPriority.Initialize(((Enemy) Cur_Level).CardPriority);
                    LevelEditorPanel_CardComboList.Initialize(((Enemy) Cur_Level).CardComboList, Initialize_Right);
                    break;
                }
                case LevelTypes.Shop:
                {
                    Cur_Level = new Shop(
                        levelPicId: 0,
                        levelNames: new SortedDictionary<string, string> {{"zh", "新商店"}, {"en", "newShop"}},
                        difficultyLevel: 1,
                        shopItems: new List<ShopItem>(),
                        5,
                        0);

                    break;
                }
            }

            SetLevel(Cur_Level);
            OnChangeLevelTypeByEdit = true;
        }
    }

    private UnityAction<string, bool> SetLevelPicID;

    private void OnLevelPicIDChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            Cur_Level.LevelPicID = value;
        }
    }

    private UnityAction<string, bool> SetLevelName_zh;

    private void OnLevelNameChange_zh(string value_str)
    {
        Cur_Level.LevelNames["zh"] = value_str;
    }

    private UnityAction<string, bool> SetLevelName_en;

    private void OnLevelNameChange_en(string value_str)
    {
        Cur_Level.LevelNames["en"] = value_str;
    }

    private UnityAction<string, bool> SetEnemyType;

    private void OnEnemyTypeChange(string value_str)
    {
        if (Cur_Level is Enemy enemy)
        {
            enemy.EnemyType = (EnemyType) Enum.Parse(typeof(EnemyType), value_str);
        }
    }

    private UnityAction<string, bool> SetLevelDifficultyLevel;

    private void OnLevelDifficultyLevelChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < 1)
            {
                SetLevelDifficultyLevel(1.ToString(), false);
            }
            else
            {
                Cur_Level.DifficultyLevel = value;
            }
        }
        else
        {
            SetLevelDifficultyLevel(1.ToString(), false);
        }
    }

    private UnityAction<string, bool> SetEnemyDrawCardNum;

    private void OnEnemyDrawCardNumChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < 1)
            {
                SetEnemyDrawCardNum(GamePlaySettings.SystemMinDrawCardNum.ToString(), false);
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
                SetEnemyDrawCardNum(GamePlaySettings.SystemMaxDrawCardNum.ToString(), false);
            }
        }
        else
        {
            SetEnemyDrawCardNum(GamePlaySettings.SystemMinDrawCardNum.ToString(), false);
        }
    }

    private UnityAction<string, bool> SetEnemyLife;

    private void OnEnemyLifeChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value <= 0)
            {
                SetEnemyLife(GamePlaySettings.SystemMinLife.ToString(), false);
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
                SetEnemyLife(GamePlaySettings.SystemMaxLife.ToString(), false);
            }
        }
        else
        {
            SetEnemyLife(GamePlaySettings.SystemMinLife.ToString(), false);
        }
    }

    private UnityAction<string, bool> SetEnemyEnergy;

    private void OnEnemyEnergyChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < 0)
            {
                SetEnemyEnergy(0.ToString(), false);
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
                SetEnemyEnergy(GamePlaySettings.SystemMaxEnergy.ToString(), false);
            }
        }
        else
        {
            SetEnemyEnergy(0.ToString(), false);
        }
    }

    private UnityAction<string, bool> SetEnemyBeginMetal;

    private void OnEnemyBeginMetalChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value <= 0)
            {
                SetEnemyBeginMetal(1.ToString(), false);
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
                SetEnemyBeginMetal(GamePlaySettings.SystemMaxMetal.ToString(), false);
            }
        }
        else
        {
            SetEnemyBeginMetal(1.ToString(), false);
        }
    }

    private UnityAction<string, bool> SetShopItemOthersCount;

    private void OnShopItemOthersCount(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value <= 0)
            {
                SetShopItemOthersCount(1.ToString(), false);
            }
            else if (value <= Shop.SYSTEM_SHOP_MAX_ITEM)
            {
                if (Cur_Level is Shop shop)
                {
                    shop.ShopItemCardCount = value;
                }
            }
            else
            {
                SetShopItemOthersCount(Shop.SYSTEM_SHOP_MAX_ITEM.ToString(), false);
            }
        }
        else
        {
            SetShopItemOthersCount(1.ToString(), false);
        }
    }

    private UnityAction<string, bool> SetShopItemCardCount;

    private void OnShopItemCardCountChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < 0)
            {
                SetShopItemCardCount(0.ToString(), false);
            }
            else if (value <= Shop.SYSTEM_SHOP_MAX_ITEM)
            {
                if (Cur_Level is Shop shop)
                {
                    shop.ShopItemOthersCount = value;
                }
            }
            else
            {
                SetShopItemCardCount(Shop.SYSTEM_SHOP_MAX_ITEM.ToString(), false);
            }
        }
        else
        {
            SetShopItemCardCount(0.ToString(), false);
        }
    }

    #endregion

    private bool OnChangeLevelTypeByEdit;

    private Level resetLevelBackup = null;

    public bool SetLevel(Level level)
    {
        if (level == null)
        {
            SetLevelType(LevelTypes.Shop.ToString(), false);
            SetLevelType(LevelTypes.Enemy.ToString(), false);
        }
        else
        {
            resetLevelBackup = level.Clone();
            Cur_Level = level;
            LastLevelEnglishName = Cur_Level.LevelNames["en"];
            OnChangeLevelTypeByEdit = false;
            SetLevelType(Cur_Level.LevelType.ToString(), false);
            OnChangeLevelTypeByEdit = true;
            SetLevelName_en(Cur_Level.LevelNames["en"], false);
            SetLevelName_zh(Cur_Level.LevelNames["zh"], false);
            SetLevelPicID(Cur_Level.LevelPicID.ToString(), false);
            SetLevelDifficultyLevel(Cur_Level.DifficultyLevel.ToString(), false);
            switch (Cur_Level)
            {
                case Enemy enemy:
                {
                    SetEnemyType(enemy.EnemyType.ToString(), false);
                    SetEnemyBeginMetal(enemy.BuildInfo.BeginMetal.ToString(), false);
                    SetEnemyDrawCardNum(enemy.BuildInfo.DrawCardNum.ToString(), false);
                    SetEnemyEnergy(enemy.BuildInfo.Energy.ToString(), false);
                    SetEnemyLife(enemy.BuildInfo.Life.ToString(), false);
                    Row_BonusGroups.Initialize(
                        enemy.BonusGroups,
                        ClientUtils.UpdateLayout(UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().RightPanel),
                        addAction: delegate
                        {
                            enemy.BonusGroups.Add(new BonusGroup(false, new List<Bonus>(), 1, false));
                            Row_BonusGroups.Refresh();
                            StartCoroutine(ClientUtils.UpdateLayout((RectTransform) LevelPropertiesContainer));
                        }, clearAction: delegate
                        {
                            ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                            cp.Initialize(
                                descText: LanguageManager.Instance.GetText("LevelEditorPanel_ClearConfirm"),
                                leftButtonText: LanguageManager.Instance.GetText("Common_Yes"),
                                rightButtonText: LanguageManager.Instance.GetText("Common_No"),
                                leftButtonClick: delegate
                                {
                                    cp.CloseUIForm();
                                    enemy.BonusGroups.Clear();
                                    Row_BonusGroups.Refresh();
                                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) LevelPropertiesContainer));
                                },
                                rightButtonClick: delegate { cp.CloseUIForm(); });
                        }, onStartSelectCard: OnStartSelectCard);
                    CardSelectPanel.SetBuildCards(enemy.BuildInfo.M_BuildCards, gotoAction: delegate
                    {
                        M_SelectCardContents = SelectCardContents.SelectDeckCards;
                        CardSelectPanel.SetCardLibraryPanelEnable(true);
                    });

                    M_SelectCardContents = SelectCardContents.SelectDeckCards;
                    CardSelectPanel.SetCardLibraryPanelEnable(true);
                    CardSelectPanel.SwitchSingleSelect(false);
                    CardSelectPanel.SelectCardsByBuildCards(CardStatTypes.Total);

                    LevelEditorPanel_CardPriority.Initialize(enemy.CardPriority);
                    LevelEditorPanel_CardComboList.Initialize(enemy.CardComboList, Initialize_Right);
                    break;
                }
                case Shop shop:
                {
                    CardSelectPanel.UnselectAllCards();
                    CardSelectPanel.SetCardLibraryPanelEnable(false);
                    Row_ShopItems.Initialize(shop.ShopItems, ClientUtils.UpdateLayout((RectTransform) LevelPropertiesContainer));
                    SetShopItemCardCount(shop.ShopItemCardCount.ToString(), false);
                    SetShopItemOthersCount(shop.ShopItemOthersCount.ToString(), false);
                    Row_ShopItems.SetButtonActions(
                        gotoAction: delegate { }, clearAction: delegate
                        {
                            ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                            cp.Initialize(
                                descText: LanguageManager.Instance.GetText("LevelEditorPanel_ClearConfirm"),
                                leftButtonText: LanguageManager.Instance.GetText("Common_Yes"),
                                rightButtonText: LanguageManager.Instance.GetText("Common_No"),
                                leftButtonClick: delegate
                                {
                                    cp.CloseUIForm();
                                    shop.ShopItems.Clear();
                                    Row_ShopItems.Refresh();
                                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) LevelPropertiesContainer));
                                },
                                rightButtonClick: delegate { cp.CloseUIForm(); });
                        });
                    break;
                }
            }
        }

        return false;
    }

    private SelectCardContents M_SelectCardContents;

    public enum SelectCardContents
    {
        SelectDeckCards,
        SelectBonusCards,
    }

    private void OnStartSelectCard(bool isShow, int refreshCardID, int refreshCardCount, SelectCardContents selectCardContents)
    {
        M_SelectCardContents = selectCardContents;
        CardSelectPanel.SwitchSingleSelect(selectCardContents != SelectCardContents.SelectDeckCards);
        CardSelectPanel.RefreshCard(refreshCardID, refreshCardCount);
        CardSelectPanel.SetCardLibraryPanelEnable(isShow);
    }

    private string LastLevelEnglishName = "";

    private void SaveLevel()
    {
        bool isExistedInLibrary = AllLevels.LevelDict[Cur_Level.LevelType].ContainsKey(Cur_Level.LevelNames["en"]);
        bool isNameChanged = !LastLevelEnglishName.Equals(Cur_Level.LevelNames["en"]);
        bool isInvalidName = Cur_Level.LevelNames["en"].Equals("New" + Cur_Level.LevelType);

        if (isInvalidName)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(string.Format(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_SaveLevelInvalidName"), Cur_Level.LevelNames["en"]), 0f, 1f);
            return;
        }

        if (isExistedInLibrary)
        {
            if (isNameChanged)
            {
                ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                cp.Initialize(String.Format(LanguageManager.Instance.GetText("LevelEditorPanel_OverrideExistedLevel"), Cur_Level.LevelNames["en"]),
                    LanguageManager.Instance.GetText("Common_Yes"),
                    LanguageManager.Instance.GetText("Common_No"),
                    delegate
                    {
                        cp.CloseUIForm();
                        NoticeManager.Instance.ShowInfoPanelCenter("Success", 0, 1f);
                        AllLevels.RenameLevel(Cur_Level.LevelType, LastLevelEnglishName, Cur_Level);
                        LastLevelEnglishName = Cur_Level.LevelNames["en"];
                        AllLevels.ReloadLevelXML();
                    },
                    delegate { cp.CloseUIForm(); });
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter("Success", 0, 1f);
                AllLevels.RefreshLevelXML(Cur_Level);
                LastLevelEnglishName = Cur_Level.LevelNames["en"];
                AllLevels.ReloadLevelXML();
            }
        }
        else
        {
            if (isNameChanged)
            {
                ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                cp.Initialize(String.Format(LanguageManager.Instance.GetText("LevelEditorPanel_DeleteOriginalLevel"), LastLevelEnglishName),
                    LanguageManager.Instance.GetText("Common_Yes"),
                    LanguageManager.Instance.GetText("Common_No"),
                    delegate
                    {
                        cp.CloseUIForm();
                        NoticeManager.Instance.ShowInfoPanelCenter("Success", 0, 1f);
                        AllLevels.RenameLevel(Cur_Level.LevelType, LastLevelEnglishName, Cur_Level);
                        LastLevelEnglishName = Cur_Level.LevelNames["en"];
                        AllLevels.ReloadLevelXML();
                    },
                    delegate
                    {
                        cp.CloseUIForm();
                        NoticeManager.Instance.ShowInfoPanelCenter("Success", 0, 1f);
                        AllLevels.RefreshLevelXML(Cur_Level);
                        LastLevelEnglishName = Cur_Level.LevelNames["en"];
                        AllLevels.ReloadLevelXML();
                    });
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter("Success", 0, 1f);
                AllLevels.RefreshLevelXML(Cur_Level);
                AllLevels.ReloadLevelXML();
            }
        }
    }

    private void ResetLevel()
    {
        NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_ResetSuccess"), 0f, 1f);
        SetLevel(resetLevelBackup);
    }

    #region Center CardLibrary

    [SerializeField] private Button SaveLevelButton;
    [SerializeField] private Button ResetLevelButton;
    [SerializeField] private Text SaveLevelButtonText;
    [SerializeField] private Text ResetLevelButtonText;
    [SerializeField] private CardSelectPanel CardSelectPanel;

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

        if (!PicSelectPanel.IsOpen && UIManager.Instance.GetPeekUIForm() == null)
        {
            CardSelectPanel.CardSelectPanelUpdate();
        }
    }

    private BuildCards Enemy_BuildCards
    {
        get
        {
            if (Cur_Level is Enemy enemy)
            {
                return enemy.BuildInfo.M_BuildCards;
            }

            return null;
        }
    }

    private List<ShopItem> Shop_ShopItems
    {
        get
        {
            if (Cur_Level is Shop shop)
            {
                return shop.ShopItems;
                ;
            }

            return null;
        }
    }

    private void SelectCard(CardBase card)
    {
        switch (M_SelectCardContents)
        {
            case SelectCardContents.SelectDeckCards:
            {
                Enemy_BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectCount++;
                int count = Enemy_BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectCount;
                CardSelectPanel.RefreshCard(card.CardInfo.CardID, count);
                Row_CardSelection.Refresh();
                break;
            }
            case SelectCardContents.SelectBonusCards:
            {
                CardSelectPanel.SetCardLibraryPanelEnable(false);
                Row_BonusGroups.OnCurEditBonusUnlockCardChangeCard(card.CardInfo.CardID);
                Row_BonusGroups.Refresh();
                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) LevelPropertiesContainer));
                break;
            }
        }
    }

    private void UnSelectCard(CardBase card)
    {
        if (Enemy_BuildCards != null)
        {
            if (Enemy_BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectCount == 0) return;
            Enemy_BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectCount--;
            int count = Enemy_BuildCards.CardSelectInfos[card.CardInfo.CardID].CardSelectCount;
            CardSelectPanel.RefreshCard(card.CardInfo.CardID, count);
            Row_CardSelection.Refresh();
        }
    }

    private void AddCardToCardPriorityComboList(CardBase card)
    {
        if (Cur_Level is Enemy enemy)
        {
            Cur_Selected_ILevelEditorPanel_CardComboPriorityCardContainer?.AddCard(card.CardInfo.CardID);
        }
    }

    #endregion

    #region Right AI

    public RectTransform RightPanel;
    [SerializeField] private LevelEditorPanel_CardPriority LevelEditorPanel_CardPriority;
    [SerializeField] private LevelEditorPanel_CardComboList LevelEditorPanel_CardComboList;
    private ILevelEditorPanel_CardComboPriorityCardContainer Cur_Selected_ILevelEditorPanel_CardComboPriorityCardContainer;

    List<ILevelEditorPanel_CardComboPriorityCardContainer> ILevelEditorPanel_CardComboPriorityCardContainers = new List<ILevelEditorPanel_CardComboPriorityCardContainer>();

    public void Initialize_Right()
    {
        ILevelEditorPanel_CardComboPriorityCardContainers.Clear();
        Cur_Selected_ILevelEditorPanel_CardComboPriorityCardContainer = null;

        foreach (LevelEditorPanel_CardCombo lep_cc in LevelEditorPanel_CardComboList.LevelEditorPanel_CardCombos)
        {
            ILevelEditorPanel_CardComboPriorityCardContainers.Add(lep_cc);
        }

        ILevelEditorPanel_CardComboPriorityCardContainers.Add(LevelEditorPanel_CardPriority);

        foreach (ILevelEditorPanel_CardComboPriorityCardContainer c in ILevelEditorPanel_CardComboPriorityCardContainers)
        {
            c.OnSelect = OnSelect_CardComboPriorityCardContainer;
        }

        foreach (ILevelEditorPanel_CardComboPriorityCardContainer c in ILevelEditorPanel_CardComboPriorityCardContainers)
        {
            c.IsSelected = false;
        }
    }

    private void OnSelect_CardComboPriorityCardContainer(ILevelEditorPanel_CardComboPriorityCardContainer c_selected)
    {
        foreach (ILevelEditorPanel_CardComboPriorityCardContainer c in ILevelEditorPanel_CardComboPriorityCardContainers)
        {
            if (c_selected != c)
            {
                c.IsSelected = false;
            }
        }

        Cur_Selected_ILevelEditorPanel_CardComboPriorityCardContainer = c_selected;
        NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("LevelEditorPanel_MouseMiddleButtonToSelect"), 0f, 0.5f);
    }

    #endregion
}