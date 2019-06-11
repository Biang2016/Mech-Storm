using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardEditorPanel : BaseUIForm
{
    private CardEditorPanel()
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

        SaveCardButton.onClick.AddListener(SaveCard);
        ResetCardButton.onClick.AddListener(ResetCard);
        DeleteCardButton.onClick.AddListener(DeleteCard);
        ReturnToGameButton.onClick.AddListener(ReturnToGame);

        LanguageManager.Instance.RegisterTextKeys(
            new List<(Text, string)>
            {
                (CardEditorWindowText, "CardEditorPanel_CardEditorWindowText"),
                (LanguageLabelText, "SettingMenu_Languages"),
                (ReturnToGameButtonText, "SettingMenu_ReturnToGameText"),
                (SaveCardButtonText, "CardEditorPanel_SaveCardButtonText"),
                (ResetCardButtonText, "CardEditorPanel_ResetCardButtonText"),
                (DeleteCardButtonText, "CardEditorPanel_DeleteCardButtonText"),
                (CardTotalCountText, "CardEditorPanel_CardTotalCountText"),
            });

        LanguageDropdown.ClearOptions();
        LanguageDropdown.AddOptions(LanguageManager.Instance.LanguageDescs);
        CardTotalCountNumberText.text = AllCards.CardDict.Count.ToString();

        InitializeCardPropertyForm();
        InitializePreviewCardGrid();

        PicSelectPanel.OnClickPicAction = SetCardPicID;
        PicSelectPanel.InitializePicSelectGrid("CardEditorPanel_PicSelectGridLabel");
    }

    void Start()
    {
        ChangeCard(0);
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
        if (cur_PreviewCard)
        {
            ChangeCard(cur_PreviewCard.CardInfo);
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
            rightButtonClick: delegate
            {
                cp.CloseUIForm();
            });
    }

    [SerializeField] private Text CardEditorWindowText;
    [SerializeField] private Text CardTotalCountText;
    [SerializeField] private Text CardTotalCountNumberText;
    [SerializeField] private Text LanguageLabelText;
    [SerializeField] private Dropdown LanguageDropdown;
    [SerializeField] private Button ReturnToGameButton;
    [SerializeField] private Text ReturnToGameButtonText;

    #region Left CardProperties

    public Transform CardPropertiesContainer;

    private List<PropertyFormRow> MyPropertiesRows = new List<PropertyFormRow>();
    private List<PropertyFormRow> CardPropertiesCommon = new List<PropertyFormRow>();
    private List<PropertyFormRow> SlotPropertiesRows = new List<PropertyFormRow>();
    private List<PropertyFormRow> WeaponPropertiesRows = new List<PropertyFormRow>();
    private List<PropertyFormRow> ShieldPropertiesRows = new List<PropertyFormRow>();
    private Dictionary<CardTypes, List<PropertyFormRow>> CardTypePropertiesDict = new Dictionary<CardTypes, List<PropertyFormRow>>();
    private Dictionary<SlotTypes, List<PropertyFormRow>> SlotTypePropertiesDict = new Dictionary<SlotTypes, List<PropertyFormRow>>();
    private Dictionary<WeaponTypes, List<PropertyFormRow>> WeaponTypePropertiesDict = new Dictionary<WeaponTypes, List<PropertyFormRow>>();
    private Dictionary<ShieldTypes, List<PropertyFormRow>> ShieldTypePropertiesDict = new Dictionary<ShieldTypes, List<PropertyFormRow>>();

    private CardPropertyForm_SideEffectBundle Row_SideEffectBundle = null;

    private void InitializeCardPropertyForm()
    {
        foreach (PropertyFormRow cpfr in MyPropertiesRows)
        {
            cpfr.PoolRecycle();
        }

        MyPropertiesRows.Clear();
        CardTypePropertiesDict.Clear();

        IEnumerable<CardTypes> types_card = Enum.GetValues(typeof(CardTypes)) as IEnumerable<CardTypes>;
        List<string> cardTypeList = new List<string>();
        foreach (CardTypes cardType in types_card)
        {
            cardTypeList.Add(cardType.ToString());
            CardTypePropertiesDict.Add(cardType, new List<PropertyFormRow>());
        }

        IEnumerable<SlotTypes> types_slot = Enum.GetValues(typeof(SlotTypes)) as IEnumerable<SlotTypes>;
        List<string> slotTypeList = new List<string>();
        foreach (SlotTypes slotType in types_slot)
        {
            slotTypeList.Add(slotType.ToString());
            SlotTypePropertiesDict.Add(slotType, new List<PropertyFormRow>());
        }

        IEnumerable<WeaponTypes> types_weapon = Enum.GetValues(typeof(WeaponTypes)) as IEnumerable<WeaponTypes>;
        List<string> weaponTypeList = new List<string>();
        foreach (WeaponTypes weaponType in types_weapon)
        {
            weaponTypeList.Add(weaponType.ToString());
            WeaponTypePropertiesDict.Add(weaponType, new List<PropertyFormRow>());
        }

        IEnumerable<ShieldTypes> types_shield = Enum.GetValues(typeof(ShieldTypes)) as IEnumerable<ShieldTypes>;
        List<string> shieldTypeList = new List<string>();
        foreach (ShieldTypes shieldType in types_shield)
        {
            shieldTypeList.Add(shieldType.ToString());
            ShieldTypePropertiesDict.Add(shieldType, new List<PropertyFormRow>());
        }

        PropertyFormRow Row_CardType = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Dropdown, "CardEditorPanel_CardType", OnCardTypeChange, out SetCardType, cardTypeList);
        PropertyFormRow Row_CardID = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_CardIDLabelText", OnCardIDChange, out SetCardID);
        PropertyFormRow Row_CardPicID = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_CardPicIDLabelText", OnCardPicIDChange, out SetCardPicID);
        Row_CardPicID.SetReadOnly(true);
        PropertyFormRow Row_CardDegradeID = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_CardDegradeIDLabelText", OnCardDegradeIDChange, out SetCardDegradeID, null, OnDegradeIDButtonClick);
        PropertyFormRow Row_CardUpgradeID = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_CardUpgradeIDLabelText", OnCardUpgradeIDChange, out SetCardUpgradeID, null, OnUpgradeIDButtonClick);
        PropertyFormRow Row_CardName_zh = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_CardNameLabelText_zh", OnCardNameChange_zh, out SetCardName_zh);
        PropertyFormRow Row_CardName_en = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_CardNameLabelText_en", OnCardNameChange_en, out SetCardName_en);
        PropertyFormRow Row_CardCoinCost = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_CardCoinCostLabelText", OnCardCoinCostChange, out SetCardCoinCost);
        PropertyFormRow Row_CardMetalCost = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_CardMetalCostLabelText", OnCardMetalCostChange, out SetCardMetalCost);
        PropertyFormRow Row_CardEnergyCost = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_CardEnergyCostLabelText", OnCardEnergyCostChange, out SetCardEnergyCost);
        PropertyFormRow Row_CardSelectLimit = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_CardSelectLimitLabelText", OnCardSelectLimitChange, out SetCardSelectLimit);
        PropertyFormRow Row_CardIsTemp = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Toggle, "CardEditorPanel_CardIsTempLabelText", OnCardIsTempChange, out SetCardIsTemp);
        PropertyFormRow Row_CardIsHide = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Toggle, "CardEditorPanel_CardIsHideLabelText", OnCardIsHideChange, out SetCardIsHide);

        PropertyFormRow Row_MechLife = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_MechLifeLabelText", OnMechLifeChange, out SetMechLife);
        PropertyFormRow Row_MechAttack = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_MechAttackLabelText", OnMechAttackChange, out SetMechAttack);
        PropertyFormRow Row_MechArmor = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_MechArmorLabelText", OnMechArmorChange, out SetMechArmor);
        PropertyFormRow Row_MechShield = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_MechShieldLabelText", OnMechShieldChange, out SetMechShield);
        PropertyFormRow Row_MechWeaponSlot = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Toggle, "CardEditorPanel_MechWeaponSlotLabelText", OnMechWeaponSlotChange, out SetMechWeaponSlot);
        PropertyFormRow Row_MechShieldSlot = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Toggle, "CardEditorPanel_MechShieldSlotLabelText", OnMechShieldSlotChange, out SetMechShieldSlot);
        PropertyFormRow Row_MechPackSlot = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Toggle, "CardEditorPanel_MechPackSlotLabelText", OnMechPackSlotChange, out SetMechPackSlot);
        PropertyFormRow Row_MechMASlot = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Toggle, "CardEditorPanel_MechMASlotLabelText", OnMechMASlotChange, out SetMechMASlot);
        PropertyFormRow Row_MechIsDefense = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Toggle, "CardEditorPanel_MechIsDefenseLabelText", OnMechIsDefenseChange, out SetMechIsDefense);
        PropertyFormRow Row_MechIsSniper = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Toggle, "CardEditorPanel_MechIsSniperLabelText", OnMechIsSniperChange, out SetMechIsSniper);
        PropertyFormRow Row_MechIsCharger = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Toggle, "CardEditorPanel_MechIsChargerLabelText", OnMechIsChargerChange, out SetMechIsCharger);
        PropertyFormRow Row_MechIsFrenzy = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Toggle, "CardEditorPanel_MechIsFrenzyLabelText", OnMechIsFrenzyChange, out SetMechIsFrenzy);
        PropertyFormRow Row_MechIsSoldier = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Toggle, "CardEditorPanel_MechIsSoldierLabelText", OnMechIsSoldierChange, out SetMechIsSoldier);

        PropertyFormRow Row_SlotType = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Dropdown, "CardEditorPanel_SlotType", OnSlotTypeChange, out SetSlotType, slotTypeList);

        PropertyFormRow Row_WeaponType = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Dropdown, "CardEditorPanel_WeaponTypeLabelText", OnWeaponTypeChange, out SetWeaponType, weaponTypeList);
        PropertyFormRow Row_WeaponSwordAttack = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_WeaponSwordAttackLabelText", OnWeaponSwordAttackChange, out SetWeaponSwordAttack);
        PropertyFormRow Row_WeaponSwordEnergy = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_WeaponSwordEnergyLabelText", OnWeaponSwordEnergyChange, out SetWeaponSwordEnergy);
        PropertyFormRow Row_WeaponSwordMaxEnergy = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_WeaponSwordMaxEnergyLabelText", OnWeaponSwordMaxEnergyChange, out SetWeaponSwordMaxEnergy);
        PropertyFormRow Row_WeaponGunAttack = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_WeaponGunAttackLabelText", OnWeaponGunAttackChange, out SetWeaponGunAttack);
        PropertyFormRow Row_WeaponGunBullet = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_WeaponGunBulletLabelText", OnWeaponGunBulletChange, out SetWeaponGunBullet);
        PropertyFormRow Row_WeaponGunMaxBullet = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_WeaponGunMaxBulletLabelText", OnWeaponGunMaxBulletChange, out SetWeaponGunMaxBullet);

        PropertyFormRow Row_ShieldType = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.Dropdown, "CardEditorPanel_ShieldTypeLabelText", OnShieldTypeChange, out SetShieldType, shieldTypeList);
        PropertyFormRow Row_ShieldBasicArmor = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_ShieldBasicArmorLabelText", OnShieldBasicArmorChange, out SetShieldBasicArmor);
        PropertyFormRow Row_ShieldBasicShield = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "CardEditorPanel_ShieldBasicShieldLabelText", OnShieldBasicShieldChange, out SetShieldBasicShield);

        Row_SideEffectBundle?.PoolRecycle();
        Row_SideEffectBundle = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CardPropertyForm_SideEffectBundle].AllocateGameObject<CardPropertyForm_SideEffectBundle>(CardPropertiesContainer);

        Row_SideEffectBundle.Initialize(null, null, null);

        CardPropertiesCommon = new List<PropertyFormRow>
        {
            Row_CardType,
            Row_CardID,
            Row_CardPicID,
            Row_CardUpgradeID,
            Row_CardDegradeID,
            Row_CardName_zh,
            Row_CardName_en,
            Row_CardCoinCost,
            Row_CardMetalCost,
            Row_CardEnergyCost,
            Row_CardSelectLimit,
            Row_CardIsTemp,
            Row_CardIsHide,
        };
        CardTypePropertiesDict[CardTypes.Mech] = new List<PropertyFormRow>
        {
            Row_MechLife,
            Row_MechAttack,
            Row_MechArmor,
            Row_MechShield,
            Row_MechWeaponSlot,
            Row_MechShieldSlot,
            Row_MechPackSlot,
            Row_MechMASlot,
            Row_MechIsSoldier,
            Row_MechIsDefense,
            Row_MechIsSniper,
            Row_MechIsCharger,
            Row_MechIsFrenzy,
        };
        CardTypePropertiesDict[CardTypes.Energy] = new List<PropertyFormRow>
        {
        };
        CardTypePropertiesDict[CardTypes.Spell] = new List<PropertyFormRow>
        {
        };
        CardTypePropertiesDict[CardTypes.Equip] = new List<PropertyFormRow>
        {
            Row_SlotType,
        };

        SlotPropertiesRows = new List<PropertyFormRow>
        {
            Row_WeaponType,
            Row_WeaponSwordAttack,
            Row_WeaponSwordEnergy,
            Row_WeaponSwordMaxEnergy,
            Row_WeaponGunAttack,
            Row_WeaponGunBullet,
            Row_WeaponGunMaxBullet,
            Row_ShieldType,
            Row_ShieldBasicArmor,
            Row_ShieldBasicShield
        };
        SlotTypePropertiesDict[SlotTypes.Weapon] = new List<PropertyFormRow>
        {
            Row_WeaponType,
        };
        SlotTypePropertiesDict[SlotTypes.Shield] = new List<PropertyFormRow>
        {
            Row_ShieldType,
        };

        WeaponPropertiesRows = new List<PropertyFormRow>
        {
            Row_WeaponSwordAttack,
            Row_WeaponSwordEnergy,
            Row_WeaponSwordMaxEnergy,
            Row_WeaponGunAttack,
            Row_WeaponGunBullet,
            Row_WeaponGunMaxBullet
        };
        WeaponTypePropertiesDict[WeaponTypes.None] = new List<PropertyFormRow>
        {
        };
        WeaponTypePropertiesDict[WeaponTypes.Sword] = new List<PropertyFormRow>
        {
            Row_WeaponSwordAttack,
            Row_WeaponSwordEnergy,
            Row_WeaponSwordMaxEnergy,
        };
        WeaponTypePropertiesDict[WeaponTypes.Gun] = new List<PropertyFormRow>
        {
            Row_WeaponGunAttack,
            Row_WeaponGunBullet,
            Row_WeaponGunMaxBullet
        };
        WeaponTypePropertiesDict[WeaponTypes.SniperGun] = new List<PropertyFormRow>
        {
            Row_WeaponGunAttack,
            Row_WeaponGunBullet,
            Row_WeaponGunMaxBullet
        };

        ShieldPropertiesRows.AddRange(new[]
        {
            Row_ShieldBasicArmor,
            Row_ShieldBasicShield
        });
        ShieldTypePropertiesDict[ShieldTypes.Mixed] = new List<PropertyFormRow>
        {
            Row_ShieldBasicArmor,
            Row_ShieldBasicShield
        };
        ShieldTypePropertiesDict[ShieldTypes.Armor] = new List<PropertyFormRow>
        {
            Row_ShieldBasicArmor
        };
        ShieldTypePropertiesDict[ShieldTypes.Shield] = new List<PropertyFormRow>
        {
            Row_ShieldBasicShield
        };

        SetCardType("Spell");
        SetCardType("Mech");

        FormatTwoToggleIntoOneRow();
    }

    private PropertyFormRow GeneralizeRow(PropertyFormRow.CardPropertyFormRowType type, string labelKey, UnityAction<string> onValueChange, out UnityAction<string> setValue, List<string> dropdownOptionList = null, UnityAction<string> onButtonClick = null)
    {
        PropertyFormRow cpfr = PropertyFormRow.BaseInitialize(type, CardPropertiesContainer, labelKey, onValueChange, out setValue, dropdownOptionList, onButtonClick);
        MyPropertiesRows.Add(cpfr);
        return cpfr;
    }

    List<PropertyFormRow> twoToggleRows = new List<PropertyFormRow>();

    private void FormatTwoToggleIntoOneRow()
    {
        foreach (PropertyFormRow row in twoToggleRows)
        {
            PropertyFormRow_TwoToggleRow twoRow = (PropertyFormRow_TwoToggleRow) row;
            int index = row.transform.GetSiblingIndex();
            twoRow.ToggleLeft?.transform.SetParent(CardPropertiesContainer);
            twoRow.ToggleRight?.transform.SetParent(CardPropertiesContainer);
            twoRow.ToggleRight?.transform.SetSiblingIndex(index);
            twoRow.ToggleLeft?.transform.SetSiblingIndex(index);
            twoRow.PoolRecycle();
        }

        int count = 0;
        PropertyFormRow_Toggle lastToggle = null;
        UnityAction<string> temp = new UnityAction<string>(delegate(string arg0) { });
        foreach (PropertyFormRow row in MyPropertiesRows)
        {
            if (row is PropertyFormRow_Toggle toggleRow)
            {
                if (toggleRow.gameObject.activeInHierarchy)
                {
                    count++;
                    if (count == 2)
                    {
                        PropertyFormRow cpfr = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.TwoToggle, CardPropertiesContainer, "Nulll", null, out temp, null, null);
                        int index = toggleRow.transform.GetSiblingIndex();
                        cpfr.transform.SetSiblingIndex(index);
                        ((PropertyFormRow_TwoToggleRow) cpfr).ToggleLeft = lastToggle;
                        ((PropertyFormRow_TwoToggleRow) cpfr).ToggleRight = toggleRow;
                        lastToggle.transform.SetParent(cpfr.transform);
                        toggleRow.transform.SetParent(cpfr.transform);
                        count = 0;
                        twoToggleRows.Add(cpfr);
                    }

                    lastToggle = toggleRow;
                }
            }
            else
            {
                count = 0;
            }
        }
    }

    private bool OnChangeCardTypeByEdit = false;
    private UnityAction<string> SetCardType;

    private void OnCardTypeChange(string value_str)
    {
        CardTypes type = (CardTypes) Enum.Parse(typeof(CardTypes), value_str);
        foreach (PropertyFormRow cpfr in CardPropertiesCommon)
        {
            cpfr.gameObject.SetActive(true);
        }

        List<PropertyFormRow> targets = CardTypePropertiesDict[type];
        foreach (PropertyFormRow cpfr in MyPropertiesRows)
        {
            if (!CardPropertiesCommon.Contains(cpfr))
            {
                cpfr.gameObject.SetActive(targets.Contains(cpfr));
            }
        }

        if (!isPreviewExistingCards && type == CardTypes.Equip)
        {
            SetSlotType("None");
        }

        if (cur_PreviewCard)
        {
            if (!OnChangeCardTypeByEdit)
            {
                OnChangeCardTypeByEdit = true;
                CardInfo_Base newCardInfoBase = CardInfo_Base.ConvertCardInfo(cur_PreviewCard.CardInfo, type);
                newCardInfoBase.SideEffectBundle = new SideEffectBundle();
                ChangeCard(newCardInfoBase);
                OnChangeCardTypeByEdit = false;
            }
        }

        FormatTwoToggleIntoOneRow();
    }

    private UnityAction<string> SetCardID;

    private void OnCardIDChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.CardID = value;
            }

            foreach (KeyValuePair<int, CardEditorPanel_CardPreviewButton> kv in CardPreviewButtons)
            {
                kv.Value.IsEdit = false;
            }

            if (CardPreviewButtons.ContainsKey(value))
            {
                CardPreviewButtons[value].IsEdit = true;
                if (-CardPreviewButtons[value].transform.localPosition.y - ExistingCardGridContainer.transform.localPosition.y > ((RectTransform) ExistingCardGridContainer.transform.parent).rect.height * 0.8f
                    || -CardPreviewButtons[value].transform.localPosition.y - ExistingCardGridContainer.transform.localPosition.y < ((RectTransform) ExistingCardGridContainer.transform.parent).rect.height * 0.2f
                )
                {
                    ExistingCardGridContainer.transform.localPosition = new Vector3(ExistingCardGridContainer.transform.localPosition.x, -CardPreviewButtons[value].transform.localPosition.y - 260f, ExistingCardGridContainer.transform.localPosition.z);
                }
            }
        }
    }

    private UnityAction<string> SetCardPicID;

    private void OnCardPicIDChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                bool picIDValid = ClientUtils.IsCardPictureValid(value);
                if (picIDValid)
                {
                    cur_PreviewCard.CardInfo.BaseInfo.PictureID = value;
                    cur_PreviewCard.ChangeCardPicture(value);
                }
                else
                {
                    cur_PreviewCard.CardInfo.BaseInfo.PictureID = 999;
                    cur_PreviewCard.ChangeCardPicture(999);
                }
            }
        }
    }

    private UnityAction<string> SetCardUpgradeID;

    private void OnCardUpgradeIDChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.UpgradeInfo.UpgradeCardID = value;
            }
        }
    }

    private void OnUpgradeIDButtonClick(string value_str)
    {
        if (value_str == "-1" || value_str == "")
        {
            return;
        }

        if (int.TryParse(value_str, out int value))
        {
            bool suc = ChangeCard(value);
            if (suc)
            {
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardEditorPanel_NoSuchCard"), 0, 0.5f);
            }
        }
    }

    private UnityAction<string> SetCardDegradeID;

    private void OnCardDegradeIDChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.UpgradeInfo.DegradeCardID = value;
            }
        }
    }

    private void OnDegradeIDButtonClick(string value_str)
    {
        if (value_str == "-1" || value_str == "")
        {
            return;
        }

        if (int.TryParse(value_str, out int value))
        {
            bool suc = ChangeCard(value);
            if (suc)
            {
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardEditorPanel_NoSuchCard"), 0, 0.5f);
            }
        }
    }

    private UnityAction<string> SetCardName_zh;

    private void OnCardNameChange_zh(string value_str)
    {
        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.BaseInfo.CardNames["zh"] = value_str;
            cur_PreviewCard.M_Name = cur_PreviewCard.CardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()];
        }
    }

    private UnityAction<string> SetCardName_en;

    private void OnCardNameChange_en(string value_str)
    {
        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.BaseInfo.CardNames["en"] = value_str;
            cur_PreviewCard.M_Name = cur_PreviewCard.CardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()];
        }
    }

    private UnityAction<string> SetCardCoinCost;

    private void OnCardCoinCostChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.BaseInfo.Coin = value;
                cur_PreviewCard.M_Coin = value;
            }
        }
    }

    private UnityAction<string> SetCardMetalCost;

    private void OnCardMetalCostChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.BaseInfo.Metal = value;
                cur_PreviewCard.M_Metal = value;
            }
        }
    }

    private UnityAction<string> SetCardEnergyCost;

    private void OnCardEnergyCostChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.BaseInfo.Energy = value;
                cur_PreviewCard.M_Energy = value;
            }
        }
    }

    private UnityAction<string> SetCardSelectLimit;

    private void OnCardSelectLimitChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.BaseInfo.LimitNum = value;
                cur_PreviewCard.ChangeCardSelectLimit(value, true);
            }
        }
    }

    private UnityAction<string> SetCardIsTemp;

    private void OnCardIsTempChange(string value_str)
    {
        bool value = value_str.Equals("True");
        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.BaseInfo.IsTemp = value;
            cur_PreviewCard.RefreshCardTextLanguage();
        }
    }

    private UnityAction<string> SetCardIsHide;

    private void OnCardIsHideChange(string value_str)
    {
        bool value = value_str.Equals("True");
        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.BaseInfo.IsHide = value;
            cur_PreviewCard.RefreshCardTextLanguage();
        }
    }

    private UnityAction<string> SetMechLife;

    private void OnMechLifeChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.LifeInfo.TotalLife = value;
                cur_PreviewCard.CardInfo.LifeInfo.Life = value;
                if (cur_PreviewCard as CardMech)
                {
                    ((CardMech) cur_PreviewCard).M_MechTotalLife = value;
                }
            }
        }
    }

    private UnityAction<string> SetMechAttack;

    private void OnMechAttackChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.BattleInfo.BasicAttack = value;
                if (cur_PreviewCard as CardMech)
                {
                    ((CardMech) cur_PreviewCard).M_MechAttack = value;
                    cur_PreviewCard.RefreshCardTextLanguage();
                    cur_PreviewCard.RefreshCardAllColors();
                }
            }
        }
    }

    private UnityAction<string> SetMechArmor;

    private void OnMechArmorChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.BattleInfo.BasicArmor = value;
                if (cur_PreviewCard as CardMech)
                {
                    ((CardMech) cur_PreviewCard).M_MechArmor = value;
                    cur_PreviewCard.RefreshCardTextLanguage();
                    cur_PreviewCard.RefreshCardAllColors();
                }
            }
        }
    }

    private UnityAction<string> SetMechShield;

    private void OnMechShieldChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.BattleInfo.BasicShield = value;
                if (cur_PreviewCard as CardMech)
                {
                    ((CardMech) cur_PreviewCard).M_MechShield = value;
                    cur_PreviewCard.RefreshCardTextLanguage();
                    cur_PreviewCard.RefreshCardAllColors();
                }
            }
        }
    }

    private UnityAction<string> SetMechWeaponSlot;

    private void OnMechWeaponSlotChange(string value_str)
    {
        bool value = value_str.Equals("True");
        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.MechInfo.Slots[0] = value ? SlotTypes.Weapon : SlotTypes.None;
            if (cur_PreviewCard as CardMech)
            {
                ((CardMech) cur_PreviewCard).InitSlots();
            }
        }
    }

    private UnityAction<string> SetMechShieldSlot;

    private void OnMechShieldSlotChange(string value_str)
    {
        bool value = value_str.Equals("True");
        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.MechInfo.Slots[1] = value ? SlotTypes.Shield : SlotTypes.None;
            if (cur_PreviewCard as CardMech)
            {
                ((CardMech) cur_PreviewCard).InitSlots();
            }
        }
    }

    private UnityAction<string> SetMechPackSlot;

    private void OnMechPackSlotChange(string value_str)
    {
        bool value = value_str.Equals("True");
        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.MechInfo.Slots[2] = value ? SlotTypes.Pack : SlotTypes.None;
            if (cur_PreviewCard as CardMech)
            {
                ((CardMech) cur_PreviewCard).InitSlots();
            }
        }
    }

    private UnityAction<string> SetMechMASlot;

    private void OnMechMASlotChange(string value_str)
    {
        bool value = value_str.Equals("True");
        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.MechInfo.Slots[3] = value ? SlotTypes.MA : SlotTypes.None;
            if (cur_PreviewCard as CardMech)
            {
                ((CardMech) cur_PreviewCard).InitSlots();
            }
        }
    }

    private UnityAction<string> SetMechIsDefense;

    private void OnMechIsDefenseChange(string value_str)
    {
        bool value = value_str.Equals("True");
        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.MechInfo.IsDefense = value;
            cur_PreviewCard.RefreshCardAllColors();
            cur_PreviewCard.RefreshCardTextLanguage();
        }
    }

    private UnityAction<string> SetMechIsSniper;

    private void OnMechIsSniperChange(string value_str)
    {
        bool value = value_str.Equals("True");
        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.MechInfo.IsSniper = value;
            cur_PreviewCard.RefreshCardAllColors();
            cur_PreviewCard.RefreshCardTextLanguage();
        }
    }

    private UnityAction<string> SetMechIsCharger;

    private void OnMechIsChargerChange(string value_str)
    {
        bool value = value_str.Equals("True");
        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.MechInfo.IsCharger = value;
            cur_PreviewCard.RefreshCardAllColors();
            cur_PreviewCard.RefreshCardTextLanguage();
        }
    }

    private UnityAction<string> SetMechIsFrenzy;

    private void OnMechIsFrenzyChange(string value_str)
    {
        bool value = value_str.Equals("True");
        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.MechInfo.IsFrenzy = value;
            cur_PreviewCard.RefreshCardAllColors();
            cur_PreviewCard.RefreshCardTextLanguage();
        }
    }

    private UnityAction<string> SetMechIsSoldier;

    private void OnMechIsSoldierChange(string value_str)
    {
        bool value = value_str.Equals("True");
        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.MechInfo.IsSoldier = value;
            cur_PreviewCard.RefreshCardAllColors();
            cur_PreviewCard.RefreshCardTextLanguage();
        }
    }

    private UnityAction<string> SetSlotType;

    private void OnSlotTypeChange(string value_str)
    {
        SlotTypes type = (SlotTypes) Enum.Parse(typeof(SlotTypes), value_str);
        foreach (PropertyFormRow cpfr in SlotPropertiesRows)
        {
            cpfr.gameObject.SetActive(false);
        }

        List<PropertyFormRow> targets_slot = SlotTypePropertiesDict[type];
        foreach (PropertyFormRow cpfr in targets_slot)
        {
            cpfr.gameObject.SetActive(targets_slot.Contains(cpfr));
        }

        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.EquipInfo.SlotType = type;
            cur_PreviewCard.RefreshCardTextLanguage();
            cur_PreviewCard.RefreshCardAllColors();
        }

        if (!isPreviewExistingCards)
        {
            SetWeaponType("None");
            SetShieldType("None");
        }

        FormatTwoToggleIntoOneRow();
    }

    private UnityAction<string> SetWeaponType;

    private void OnWeaponTypeChange(string value_str)
    {
        WeaponTypes type = (WeaponTypes) Enum.Parse(typeof(WeaponTypes), value_str);
        List<PropertyFormRow> targets_weapon = WeaponTypePropertiesDict[type];
        foreach (PropertyFormRow cpfr in WeaponPropertiesRows)
        {
            cpfr.gameObject.SetActive(targets_weapon.Contains(cpfr));
        }

        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.WeaponInfo.WeaponType = type;
            cur_PreviewCard.RefreshCardTextLanguage();
            cur_PreviewCard.RefreshCardAllColors();
        }

        FormatTwoToggleIntoOneRow();
    }

    private UnityAction<string> SetWeaponSwordAttack;

    private void OnWeaponSwordAttackChange(string value_str)
    {
        int value = -1;
        int.TryParse(value_str, out value);
        if (value != -1)
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.WeaponInfo.Attack = value;
                cur_PreviewCard.RefreshCardTextLanguage();
                cur_PreviewCard.RefreshCardAllColors();
            }
        }
    }

    private UnityAction<string> SetWeaponSwordEnergy;

    private void OnWeaponSwordEnergyChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.WeaponInfo.Energy = value;
                cur_PreviewCard.RefreshCardTextLanguage();
                cur_PreviewCard.RefreshCardAllColors();
            }
        }
    }

    private UnityAction<string> SetWeaponSwordMaxEnergy;

    private void OnWeaponSwordMaxEnergyChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.WeaponInfo.EnergyMax = value;
                cur_PreviewCard.RefreshCardTextLanguage();
                cur_PreviewCard.RefreshCardAllColors();
            }
        }
    }

    private UnityAction<string> SetWeaponGunAttack;

    private void OnWeaponGunAttackChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.WeaponInfo.Attack = value;
                cur_PreviewCard.RefreshCardTextLanguage();
                cur_PreviewCard.RefreshCardAllColors();
            }
        }
    }

    private UnityAction<string> SetWeaponGunBullet;

    private void OnWeaponGunBulletChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.WeaponInfo.Energy = value;
                cur_PreviewCard.RefreshCardTextLanguage();
                cur_PreviewCard.RefreshCardAllColors();
            }
        }
    }

    private UnityAction<string> SetWeaponGunMaxBullet;

    private void OnWeaponGunMaxBulletChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.WeaponInfo.EnergyMax = value;
                cur_PreviewCard.RefreshCardTextLanguage();
                cur_PreviewCard.RefreshCardAllColors();
            }
        }
    }

    private UnityAction<string> SetShieldType;

    private void OnShieldTypeChange(string value_str)
    {
        ShieldTypes type = (ShieldTypes) Enum.Parse(typeof(ShieldTypes), value_str);
        List<PropertyFormRow> targets_shield = ShieldTypePropertiesDict[type];
        foreach (PropertyFormRow cpfr in ShieldPropertiesRows)
        {
            cpfr.gameObject.SetActive(targets_shield.Contains(cpfr));
        }

        if (cur_PreviewCard)
        {
            cur_PreviewCard.CardInfo.ShieldInfo.ShieldType = type;
            cur_PreviewCard.RefreshCardTextLanguage();
            cur_PreviewCard.RefreshCardAllColors();
        }

        FormatTwoToggleIntoOneRow();
    }

    private UnityAction<string> SetShieldBasicArmor;

    private void OnShieldBasicArmorChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.ShieldInfo.Armor = value;
                cur_PreviewCard.RefreshCardTextLanguage();
                cur_PreviewCard.RefreshCardAllColors();
            }
        }
    }

    private UnityAction<string> SetShieldBasicShield;

    private void OnShieldBasicShieldChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (cur_PreviewCard)
            {
                cur_PreviewCard.CardInfo.ShieldInfo.Shield = value;
                cur_PreviewCard.RefreshCardTextLanguage();
                cur_PreviewCard.RefreshCardAllColors();
            }
        }
    }

    #region SideEffect

    #endregion

    #endregion

    #region Center CardPreview

    [SerializeField] private ScrollRect CardPreviewButtonScrollRect;
    [SerializeField] private Transform CardPreviewContainer;
    [SerializeField] private RawImage CardPreviewRawImage;
    private CardBase cur_PreviewCard;
    private CardBase cur_PreviewCard_Up;
    private CardBase cur_PreviewCard_De;
    private bool isPreviewExistingCards = false;

    private void ChangeCard(CardInfo_Base ci)
    {
        isPreviewExistingCards = true;

        cur_PreviewCard?.PoolRecycle();
        cur_PreviewCard = CardBase.InstantiateCardByCardInfo(ci, CardPreviewContainer, CardBase.CardShowMode.CardSelect);
        cur_PreviewCard.transform.localScale = Vector3.one * 35;
        cur_PreviewCard.transform.localPosition = new Vector3(-25, 0, 0);
        cur_PreviewCard.ShowCardBloom(true);

        cur_PreviewCard_Up?.PoolRecycle();
        cur_PreviewCard_Up = null;
        if (AllCards.CardDict.ContainsKey(ci.UpgradeInfo.UpgradeCardID))
        {
            CardInfo_Base cardInfoBase_Up = AllCards.GetCard(ci.UpgradeInfo.UpgradeCardID);
            cur_PreviewCard_Up = CardBase.InstantiateCardByCardInfo(cardInfoBase_Up, CardPreviewContainer, CardBase.CardShowMode.CardSelect);
            cur_PreviewCard_Up.transform.localScale = Vector3.one * 25;
            cur_PreviewCard_Up.transform.localPosition = new Vector3(475, 0, 0);
            cur_PreviewCard_Up.ShowCardBloom(true);
        }

        cur_PreviewCard_De?.PoolRecycle();
        cur_PreviewCard_De = null;
        if (AllCards.CardDict.ContainsKey(ci.UpgradeInfo.DegradeCardID))
        {
            CardInfo_Base cardInfoBase_De = AllCards.GetCard(ci.UpgradeInfo.DegradeCardID);
            cur_PreviewCard_De = CardBase.InstantiateCardByCardInfo(cardInfoBase_De, CardPreviewContainer, CardBase.CardShowMode.CardSelect);
            cur_PreviewCard_De.transform.localScale = Vector3.one * 25;
            cur_PreviewCard_De.transform.localPosition = new Vector3(-500, 0, 0);
            cur_PreviewCard_De.ShowCardBloom(true);
        }

        SetCardType(cur_PreviewCard.CardInfo.BaseInfo.CardType.ToString());
        SetCardID(string.Format("{0:000}", ci.CardID));
        SetCardPicID(string.Format("{0:000}", ci.BaseInfo.PictureID));
        SetCardUpgradeID(ci.UpgradeInfo.UpgradeCardID.ToString());
        SetCardDegradeID(ci.UpgradeInfo.DegradeCardID.ToString());
        SetCardName_zh(cur_PreviewCard.CardInfo.BaseInfo.CardNames["zh"]);
        SetCardName_en(cur_PreviewCard.CardInfo.BaseInfo.CardNames["en"]);
        SetCardCoinCost(cur_PreviewCard.CardInfo.BaseInfo.Coin.ToString());
        SetCardMetalCost(cur_PreviewCard.CardInfo.BaseInfo.Metal.ToString());
        SetCardEnergyCost(cur_PreviewCard.CardInfo.BaseInfo.Energy.ToString());
        SetCardSelectLimit(cur_PreviewCard.CardInfo.BaseInfo.LimitNum.ToString());
        SetCardIsTemp(cur_PreviewCard.CardInfo.BaseInfo.IsTemp.ToString());
        SetCardIsHide(cur_PreviewCard.CardInfo.BaseInfo.IsHide.ToString());

        switch (ci.BaseInfo.CardType)
        {
            case CardTypes.Mech:
            {
                SetMechLife(ci.LifeInfo.Life.ToString());
                SetMechAttack(ci.BattleInfo.BasicAttack.ToString());
                SetMechArmor(ci.BattleInfo.BasicArmor.ToString());
                SetMechShield(ci.BattleInfo.BasicShield.ToString());
                SetMechIsSoldier((ci.MechInfo.IsSoldier).ToString());
                SetMechIsDefense((ci.MechInfo.IsDefense).ToString());
                SetMechIsSniper((ci.MechInfo.IsSniper).ToString());
                SetMechIsCharger((ci.MechInfo.IsCharger).ToString());
                SetMechIsFrenzy((ci.MechInfo.IsFrenzy).ToString());
                SetMechWeaponSlot((ci.MechInfo.Slots[0] == SlotTypes.Weapon).ToString());
                SetMechShieldSlot((ci.MechInfo.Slots[1] == SlotTypes.Shield).ToString());
                SetMechPackSlot((ci.MechInfo.Slots[2] == SlotTypes.Pack).ToString());
                SetMechMASlot((ci.MechInfo.Slots[3] == SlotTypes.MA).ToString());
                break;
            }
            case CardTypes.Equip:
            {
                SetSlotType(ci.EquipInfo.SlotType.ToString());
                switch (ci.EquipInfo.SlotType)
                {
                    case SlotTypes.Weapon:
                    {
                        SetWeaponType(ci.WeaponInfo.WeaponType.ToString());
                        switch (ci.WeaponInfo.WeaponType)
                        {
                            case WeaponTypes.Sword:
                            {
                                SetWeaponSwordAttack(ci.WeaponInfo.Attack.ToString());
                                SetWeaponSwordEnergy(ci.WeaponInfo.Energy.ToString());
                                SetWeaponSwordMaxEnergy(ci.WeaponInfo.EnergyMax.ToString());
                                break;
                            }
                            case WeaponTypes.Gun:
                            {
                                SetWeaponGunAttack(ci.WeaponInfo.Attack.ToString());
                                SetWeaponGunBullet(ci.WeaponInfo.Energy.ToString());
                                SetWeaponGunMaxBullet(ci.WeaponInfo.EnergyMax.ToString());
                                break;
                            }
                            case WeaponTypes.SniperGun:
                            {
                                SetWeaponGunAttack(ci.WeaponInfo.Attack.ToString());
                                SetWeaponGunBullet(ci.WeaponInfo.Energy.ToString());
                                SetWeaponGunMaxBullet(ci.WeaponInfo.EnergyMax.ToString());
                                break;
                            }
                        }

                        break;
                    }

                    case SlotTypes.Shield:
                    {
                        SetShieldType(ci.ShieldInfo.ShieldType.ToString());
                        SetShieldBasicArmor(ci.ShieldInfo.Armor.ToString());
                        SetShieldBasicShield(ci.ShieldInfo.Shield.ToString());
                        break;
                    }
                }

                break;
            }
        }

        Row_SideEffectBundle.Initialize(cur_PreviewCard.CardInfo, cur_PreviewCard.CardInfo.SideEffectBundle, cur_PreviewCard.RefreshCardTextLanguage);

        cur_PreviewCard.RefreshCardTextLanguage();
        cur_PreviewCard.RefreshCardAllColors();
        FormatTwoToggleIntoOneRow();

        StartCoroutine(ClientUtils.UpdateLayout((RectTransform) CardPropertiesContainer));

        isPreviewExistingCards = false;
    }

    public bool ChangeCard(int cardID)
    {
        if (AllCards.CardDict.ContainsKey(cardID))
        {
            ChangeCard(AllCards.GetCard(cardID));
            return true;
        }

        return false;
    }

    [SerializeField] private Button SaveCardButton;
    [SerializeField] private Button ResetCardButton;
    [SerializeField] private Button DeleteCardButton;
    [SerializeField] private Text SaveCardButtonText;
    [SerializeField] private Text ResetCardButtonText;
    [SerializeField] private Text DeleteCardButtonText;

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
                SaveCard();
            }
        }

        int curCardID = -1;
        if (cur_PreviewCard)
        {
            curCardID = cur_PreviewCard.CardInfo.CardID;
        }

        if (leftShiftPress && Input.GetKeyUp(KeyCode.RightArrow))
        {
            foreach (KeyValuePair<int, CardInfo_Base> kv in AllCards.CardDict)
            {
                if (kv.Key > curCardID)
                {
                    ChangeCard(kv.Key);
                    break;
                }
            }
        }

        if (leftShiftPress && Input.GetKeyUp(KeyCode.LeftArrow))
        {
            int changeCardID = 0;
            foreach (KeyValuePair<int, CardInfo_Base> kv in AllCards.CardDict)
            {
                if (kv.Key >= curCardID)
                {
                    ChangeCard(changeCardID);
                    break;
                }

                changeCardID = kv.Key;
            }
        }

        int gridColumns = Mathf.RoundToInt(((RectTransform) ExistingCardGridContainer.transform).rect.width - ExistingCardGridContainer.padding.left) / Mathf.RoundToInt(ExistingCardGridContainer.cellSize.x + ExistingCardGridContainer.spacing.x);

        if (leftShiftPress && Input.GetKeyUp(KeyCode.DownArrow))
        {
            int count = 0;
            foreach (KeyValuePair<int, CardInfo_Base> kv in AllCards.CardDict)
            {
                if (kv.Key > curCardID)
                {
                    count++;
                    if (count >= gridColumns)
                    {
                        ChangeCard(kv.Key);
                        break;
                    }
                }
            }
        }

        if (leftShiftPress && Input.GetKeyUp(KeyCode.UpArrow))
        {
            int[] before = new int[gridColumns];
            for (int i = 0; i < before.Length; i++)
            {
                before[i] = -1;
            }

            foreach (KeyValuePair<int, CardInfo_Base> kv in AllCards.CardDict)
            {
                if (kv.Key >= curCardID)
                {
                    if (before[0] != -1)
                    {
                        ChangeCard(before[0]);
                        break;
                    }
                }

                for (int i = 0; i < before.Length - 1; i++)
                {
                    before[i] = before[i + 1];
                }

                before[before.Length - 1] = kv.Key;
            }
        }

        if (Input.GetKeyUp(KeyCode.Delete))
        {
            if (cur_PreviewCard)
            {
                DeleteCard();
            }
        }

        float size = Mathf.Min(1800f, ((RectTransform) CardPreviewContainer.transform).rect.width);
        ((RectTransform) CardPreviewRawImage.transform).sizeDelta = new Vector2(size, size * 4 / 3);
    }

    public void SaveCard()
    {
        if (!cur_PreviewCard)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardEditorPanel_EmptyCardID"), 0, 1f);
            return;
        }

        if (cur_PreviewCard.CardInfo.UpgradeInfo.UpgradeCardID != -1 && !AllCards.CardDict.ContainsKey(cur_PreviewCard.CardInfo.UpgradeInfo.UpgradeCardID))
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardEditorPanel_SaveCardUpgradeIDNotExists"), 0, 2.5f);
            return;
        }
        else if (cur_PreviewCard.CardInfo.UpgradeInfo.DegradeCardID != -1 && !AllCards.CardDict.ContainsKey(cur_PreviewCard.CardInfo.UpgradeInfo.DegradeCardID))
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardEditorPanel_SaveCardDegradeIDNotExists"), 0, 2.5f);
            return;
        }

        //升降级ID不能与自身卡牌ID相同
        bool upgradeCardIDValid = cur_PreviewCard.CardInfo.CardID != cur_PreviewCard.CardInfo.UpgradeInfo.UpgradeCardID;
        bool degradeCardIDValid = cur_PreviewCard.CardInfo.CardID != cur_PreviewCard.CardInfo.UpgradeInfo.DegradeCardID;
        if (!upgradeCardIDValid)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardEditorPanel_SaveCardUpgradeIDIdenticalInvalid"), 0, 2.5f);
            return;
        }
        else if (!degradeCardIDValid)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardEditorPanel_SaveCardDegradeIDIdenticalInvalid"), 0, 2.5f);
            return;
        }

        //自身ID、升级ID、降级ID是否构成自身升级链循环
        List<int> cardSeries = AllCards.GetCardSeries(cur_PreviewCard.CardInfo.CardID);
        List<int> idCircle_up = new List<int>();
        List<int> idCircle_de = new List<int>();
        if (cardSeries.Count != 0)
        {
            int thisCardIDIndex = cardSeries.IndexOf(cur_PreviewCard.CardInfo.CardID);

            for (int i = 0; i < cardSeries.Count; i++)
            {
                int id = cardSeries[i];
                if (i > thisCardIDIndex)
                {
                    if (degradeCardIDValid)
                    {
                        idCircle_de.Add(id);
                    }

                    if (id == cur_PreviewCard.CardInfo.UpgradeInfo.DegradeCardID)
                    {
                        idCircle_de.Add(cur_PreviewCard.CardInfo.CardID);
                        degradeCardIDValid = false;
                    }
                }
                else if (i < thisCardIDIndex)
                {
                    if (!upgradeCardIDValid)
                    {
                        idCircle_up.Add(id);
                    }

                    if (id == cur_PreviewCard.CardInfo.UpgradeInfo.UpgradeCardID)
                    {
                        idCircle_up.Add(cur_PreviewCard.CardInfo.CardID);
                        idCircle_up.Add(cur_PreviewCard.CardInfo.UpgradeInfo.UpgradeCardID);
                        upgradeCardIDValid = false;
                    }
                }
            }

            idCircle_up.Add(cur_PreviewCard.CardInfo.CardID);
            idCircle_de.Add(cur_PreviewCard.CardInfo.CardID);
        }

        //升级ID、降级ID是否致使其他卡片升级链循环
        if (upgradeCardIDValid && degradeCardIDValid)
        {
            idCircle_up = new List<int>();
            if (cur_PreviewCard.CardInfo.UpgradeInfo.UpgradeCardID != -1 && cur_PreviewCard.CardInfo.UpgradeInfo.DegradeCardID != -1)
            {
                cardSeries = AllCards.GetCardSeries(cur_PreviewCard.CardInfo.UpgradeInfo.UpgradeCardID);
                for (int i = cardSeries.IndexOf(cur_PreviewCard.CardInfo.UpgradeInfo.UpgradeCardID); i < cardSeries.Count; i++)
                {
                    idCircle_up.Add(cardSeries[i]);
                    if (cardSeries[i] == cur_PreviewCard.CardInfo.UpgradeInfo.DegradeCardID)
                    {
                        upgradeCardIDValid = false;
                        idCircle_up.Add(cur_PreviewCard.CardInfo.CardID);
                        idCircle_up.Add(cur_PreviewCard.CardInfo.UpgradeInfo.UpgradeCardID);
                        break;
                    }
                }
            }
        }

        if (!upgradeCardIDValid)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardEditorPanel_SaveCardUpgradeIDInvalid") + " ID cycle: " + string.Join(" -> ", idCircle_up), 0, 2.5f);
            return;
        }
        else if (!degradeCardIDValid)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardEditorPanel_SaveCardDegradeIDInvalid") + " ID cycle: " + string.Join(" -> ", idCircle_de), 0, 2.5f);
            return;
        }

        string info = "";
        if (AllCards.CardDict.ContainsKey(cur_PreviewCard.CardInfo.CardID))
        {
            info = string.Format(LanguageManager.Instance.GetText("CardEditorPanel_ConfirmSaveCard"), cur_PreviewCard.CardInfo.CardID);
        }
        else
        {
            info = string.Format(LanguageManager.Instance.GetText("CardEditorPanel_ConfirmCreateNewCard"), cur_PreviewCard.CardInfo.CardID);
        }

        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
        cp.Initialize(
            info,
            LanguageManager.Instance.GetText("Common_Yes"),
            LanguageManager.Instance.GetText("Common_No"),
            delegate
            {
                if (AllCards.CardDict.ContainsKey(cur_PreviewCard.CardInfo.CardID))
                {
                    CardInfo_Base oriCurCardInfo = AllCards.GetCard(cur_PreviewCard.CardInfo.CardID);
                    bool removeUpgradeCardDegradeCardID = oriCurCardInfo.UpgradeInfo.UpgradeCardID != -1 && cur_PreviewCard.CardInfo.UpgradeInfo.UpgradeCardID == -1;
                    bool removeDegradeCardUpgradeCardID = oriCurCardInfo.UpgradeInfo.DegradeCardID != -1 && cur_PreviewCard.CardInfo.UpgradeInfo.DegradeCardID == -1;

                    if (removeUpgradeCardDegradeCardID)
                    {
                        CardInfo_Base ori_up_ci = AllCards.GetCard(oriCurCardInfo.UpgradeInfo.UpgradeCardID);
                        ori_up_ci.UpgradeInfo.DegradeCardID = -1;
                        AllCards.RefreshCardXML(ori_up_ci);
                    }

                    if (removeDegradeCardUpgradeCardID)
                    {
                        CardInfo_Base ori_de_ci = AllCards.GetCard(oriCurCardInfo.UpgradeInfo.DegradeCardID);
                        ori_de_ci.UpgradeInfo.UpgradeCardID = -1;
                        AllCards.RefreshCardXML(ori_de_ci);
                    }
                }

                AllCards.RefreshCardXML(cur_PreviewCard.CardInfo);

                if (AllCards.CardDict.ContainsKey(cur_PreviewCard.CardInfo.UpgradeInfo.UpgradeCardID))
                {
                    CardInfo_Base up_ci = AllCards.GetCard(cur_PreviewCard.CardInfo.UpgradeInfo.UpgradeCardID);
                    CardInfo_Base up_ci_de_ori = AllCards.GetCard(up_ci.UpgradeInfo.DegradeCardID);
                    if (up_ci_de_ori != null && up_ci_de_ori.CardID != cur_PreviewCard.CardInfo.CardID)
                    {
                        up_ci_de_ori.UpgradeInfo.UpgradeCardID = -1;
                        AllCards.RefreshCardXML(up_ci_de_ori);
                    }

                    up_ci.UpgradeInfo.DegradeCardID = cur_PreviewCard.CardInfo.CardID;
                    AllCards.RefreshCardXML(up_ci);
                }

                if (AllCards.CardDict.ContainsKey(cur_PreviewCard.CardInfo.UpgradeInfo.DegradeCardID))
                {
                    CardInfo_Base de_ci = AllCards.GetCard(cur_PreviewCard.CardInfo.UpgradeInfo.DegradeCardID);
                    CardInfo_Base de_ci_up_ori = AllCards.GetCard(de_ci.UpgradeInfo.UpgradeCardID);
                    if (de_ci_up_ori != null && de_ci_up_ori.CardID != cur_PreviewCard.CardInfo.CardID)
                    {
                        de_ci_up_ori.UpgradeInfo.DegradeCardID = -1;
                        AllCards.RefreshCardXML(de_ci_up_ori);
                    }

                    de_ci.UpgradeInfo.UpgradeCardID = cur_PreviewCard.CardInfo.CardID;
                    AllCards.RefreshCardXML(de_ci);
                }

                AllCards.ReloadCardXML();
                InitializePreviewCardGrid();
                cp.CloseUIForm();
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardEditorPanel_SaveCardSuccess"), 0, 1f);
                CardTotalCountNumberText.text = AllCards.CardDict.Count.ToString();
                ChangeCard(cur_PreviewCard.CardInfo.CardID);
            },
            cp.CloseUIForm);
    }

    public void ResetCard()
    {
        if (cur_PreviewCard)
        {
            ChangeCard(cur_PreviewCard.CardInfo.CardID);
            NoticeManager.Instance.ShowInfoPanelCenter(string.Format(LanguageManager.Instance.GetText("CardEditorPanel_ResetCardNotice"), cur_PreviewCard.CardInfo.CardID), 0, 1f);
        }
    }

    public void DeleteCard()
    {
        if (cur_PreviewCard)
        {
            ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
            cp.Initialize(
                string.Format(LanguageManager.Instance.GetText("CardEditorPanel_ConfirmDeleteCard"), cur_PreviewCard.CardInfo.CardID),
                LanguageManager.Instance.GetText("Common_Yes"),
                LanguageManager.Instance.GetText("Common_No"),
                delegate
                {
                    AllCards.DeleteCard(cur_PreviewCard.CardInfo.CardID);
                    InitializePreviewCardGrid();
                    cp.CloseUIForm();
                    NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardEditorPanel_DeleteCardSuccess"), 0, 1f);
                    CardTotalCountNumberText.text = AllCards.CardDict.Count.ToString();
                },
                cp.CloseUIForm);
        }
        else
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardEditorPanel_EmptyCardID"), 0, 1f);
        }
    }

    #endregion

    #region Right CardPics

    [SerializeField] private GridLayoutGroup ExistingCardGridContainer;
    private SortedDictionary<int, CardEditorPanel_CardPreviewButton> CardPreviewButtons = new SortedDictionary<int, CardEditorPanel_CardPreviewButton>();

    private void InitializePreviewCardGrid()
    {
        foreach (KeyValuePair<int, CardEditorPanel_CardPreviewButton> kv in CardPreviewButtons)
        {
            kv.Value.PoolRecycle();
        }

        CardPreviewButtons.Clear();
        foreach (KeyValuePair<int, CardInfo_Base> kv in AllCards.CardDict)
        {
            if (kv.Key != (int) AllCards.EmptyCardTypes.EmptyCard)
            {
                CardEditorPanel_CardPreviewButton cpb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CardEditorPanel_CardPreviewButton].AllocateGameObject<CardEditorPanel_CardPreviewButton>(ExistingCardGridContainer.transform);
                cpb.Initialize(kv.Value, delegate { ChangeCard(kv.Key); });
                CardPreviewButtons.Add(kv.Key, cpb);
            }
        }
    }

    #endregion
}