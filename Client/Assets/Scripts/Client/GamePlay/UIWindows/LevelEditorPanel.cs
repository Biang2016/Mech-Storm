using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using UnityEngine.UI;

public class LevelEditorPanel : BaseUIForm
{
    private LevelEditorPanel()
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

        SaveLevelButton.onClick.AddListener(SaveLevel);
        ResetLevelButton.onClick.AddListener(ResetLevel);

        LanguageManager.Instance.RegisterTextKeys(
            new List<(Text, string)>
            {
                (LevelEditorWindowText, "LevelEditorWindow_LevelEditorWindowText"),
                (LanguageLabelText, "SettingMenu_Languages"),
                (SaveLevelButtonText, "LevelEditorWindow_SaveLevelButtonText"),
                (ResetLevelButtonText, "LevelEditorWindow_ResetLevelButtonText"),
            });

        LanguageDropdown.ClearOptions();
        LanguageDropdown.AddOptions(LanguageManager.Instance.LanguageDescs);

        InitializeCardPropertyForm();
//        InitializePreviewCardGrid();
        InitializePicSelectGrid();
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
    }

    private void OnLanguageChange(int _)
    {
    }

    [SerializeField] private Text LevelEditorWindowText;
    [SerializeField] private Text LanguageLabelText;
    [SerializeField] private Dropdown LanguageDropdown;

    #region Left LevelProperties

    public Transform LevelPropertiesContainer;

    public Level Cur_Level;

    private List<PropertyFormRow> MyPropertiesRows = new List<PropertyFormRow>();
    private Dictionary<LevelType, List<PropertyFormRow>> LevelTypePropertiesDict = new Dictionary<LevelType, List<PropertyFormRow>>();
    private Dictionary<EnemyType, List<PropertyFormRow>> EnemyTypePropertiesDict = new Dictionary<EnemyType, List<PropertyFormRow>>();
    private List<PropertyFormRow> LevelPropertiesCommon = new List<PropertyFormRow>();

    private void InitializeCardPropertyForm()
    {
        foreach (PropertyFormRow cpfr in MyPropertiesRows)
        {
            cpfr.PoolRecycle();
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

        PropertyFormRow Row_LevelType = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Dropdown, "LevelEditorWindow_LevelType", OnLevelTypeChange, out SetCardType, levelTypeList);
        PropertyFormRow Row_LevelID = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorWindow_LevelIDLabelText", OnLevelIDChange, out SetLevelID);
        PropertyFormRow Row_LevelPicID = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorWindow_LevelPicIDLabelText", OnLevelPicIDChange, out SetLevelPicID);
        PropertyFormRow Row_LevelName_zh = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorWindow_LevelNameLabelText_zh", OnLevelNameChange_zh, out SetLevelName_zh);
        PropertyFormRow Row_LevelName_en = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorWindow_LevelNameLabelText_en", OnLevelNameChange_en, out SetLevelName_en);

        PropertyFormRow Row_EnemyType = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Dropdown, "LevelEditorWindow_EnemyType", OnEnemyTypeChange, out SetEnemyType, enemyTypeList);
        PropertyFormRow Row_EnemyDrawCardNum = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorWindow_EnemyDrawCardNum", OnEnemyDrawCardNumChange, out SetEnemyDrawCardNum);
        PropertyFormRow Row_EnemyLife = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorWindow_EnemyLife", OnEnemyLifeChange, out SetEnemyLife);
        PropertyFormRow Row_EnemyEnergy = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorWindow_EnemyEnergy", OnEnemyEnergyChange, out SetEnemyEnergy);
        PropertyFormRow Row_EnemyBeginMetal = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "LevelEditorWindow_EnemyBeginMetal", OnEnemyBeginMetalChange, out SetEnemyBeginMetal);

        LevelPropertiesCommon = new List<PropertyFormRow>
        {
            Row_LevelType,
            Row_LevelID,
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
        };

        LevelTypePropertiesDict[LevelType.Shop] = new List<PropertyFormRow>
        {
        };
    }

    private UnityAction<string> SetCardType;

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
    }

    private UnityAction<string> SetLevelID;

    private void OnLevelIDChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            Cur_Level.LevelID = value;
        }
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
            if (Cur_Level is Enemy enemy)
            {
                enemy.BuildInfo.DrawCardNum = value;
            }
        }
    }

    private UnityAction<string> SetEnemyLife;

    private void OnEnemyLifeChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (Cur_Level is Enemy enemy)
            {
                enemy.BuildInfo.Life = value;
            }
        }
    }

    private UnityAction<string> SetEnemyEnergy;

    private void OnEnemyEnergyChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (Cur_Level is Enemy enemy)
            {
                enemy.BuildInfo.Energy = value;
            }
        }
    }

    private UnityAction<string> SetEnemyBeginMetal;

    private void OnEnemyBeginMetalChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (Cur_Level is Enemy enemy)
            {
                enemy.BuildInfo.BeginMetal = value;
            }
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
        Cur_Level = level;
        return false;
    }

    #region Center CardLibrary

    [SerializeField] private Button SaveLevelButton;
    [SerializeField] private Button ResetLevelButton;
    [SerializeField] private Text SaveLevelButtonText;
    [SerializeField] private Text ResetLevelButtonText;

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
    }

    public void SaveLevel()
    {
    }

    public void ResetLevel()
    {
    }

    #endregion

    #region Right Selection

    #endregion

    #region PicSelectPanel

    [SerializeField] private GameObject PicSelectGridPanel;
    [SerializeField] private GridLayoutGroup PicSelectGridContainer;
    [SerializeField] private Button PicSelectGridOpenButton;
    [SerializeField] private Button PicSelectGridCloseButton;
    private List<PicPreviewButton> PicPreviewButtons = new List<PicPreviewButton>();

    private void InitializePicSelectGrid()
    {
        PicSelectGridPanel.SetActive(false);
        PicSelectGridCloseButton.gameObject.SetActive(false);
        foreach (PicPreviewButton ppb in PicPreviewButtons)
        {
            ppb.PoolRecycle();
        }

        PicPreviewButtons.Clear();

        SortedDictionary<int, Sprite> SpriteDict = new SortedDictionary<int, Sprite>();
        for (int i = 0; i <= 10; i++)
        {
            SpriteAtlas sa = AtlasManager.LoadAtlas("CardPics_" + i);
            Sprite[] Sprites = new Sprite[sa.spriteCount];
            sa.GetSprites(Sprites);
            foreach (Sprite sprite in Sprites)
            {
                SpriteDict.Add(int.Parse(sprite.name.Replace("(Clone)", "")), sprite);
            }
        }

        foreach (KeyValuePair<int, Sprite> kv in SpriteDict)
        {
            PicPreviewButton ppb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.PicPreviewButton].AllocateGameObject<PicPreviewButton>(PicSelectGridContainer.transform);
            ppb.Initialize(kv.Value, delegate
            {
                SetLevelPicID(kv.Key.ToString());
                PicSelectGridPanel.SetActive(false);
                PicSelectGridCloseButton.gameObject.SetActive(false);
                PicSelectGridOpenButton.gameObject.SetActive(true);
            });
            PicPreviewButtons.Add(ppb);
        }
    }

    public void OnPicSelectGridOpenButtonClick()
    {
        PicSelectGridPanel.SetActive(true);
        PicSelectGridCloseButton.gameObject.SetActive(true);
        PicSelectGridOpenButton.gameObject.SetActive(false);
    }

    public void OnPicSelectGridCloseButtonClick()
    {
        PicSelectGridPanel.SetActive(false);
        PicSelectGridCloseButton.gameObject.SetActive(false);
        PicSelectGridOpenButton.gameObject.SetActive(true);
    }

    #endregion
}