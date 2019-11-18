using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelPropertyForm_BonusGroup : PropertyFormRow
{
    [SerializeField] private Transform BonusContainer;
    [SerializeField] private Button DeleteButton;
    [SerializeField] private Button ClearButton;
    [SerializeField] private Button AddButton;

    private BonusGroup Cur_BonusGroup;
    private List<PropertyFormRow> My_BonusGroupPropertyForm = new List<PropertyFormRow>();
    private List<LevelPropertyForm_Bonus> My_BonusGroupPropertyForm_Bonus = new List<LevelPropertyForm_Bonus>();
    private List<LevelPropertyForm_BonusTypeDropdown> My_BonusGroupPropertyForm_BonusTypeDropdown = new List<LevelPropertyForm_BonusTypeDropdown>();
    private UnityAction<bool, int, int, LevelEditorPanel.SelectCardContents> OnStartSelectCard;
    private IEnumerator Co_refresh;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        Clear();
        DeleteButton.onClick.RemoveAllListeners();
        ClearButton.onClick.RemoveAllListeners();
        AddButton.onClick.RemoveAllListeners();
        OnStartSelectCard = null;
    }

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKey(Label, "LevelEditorPanel_BonusGroup");
        PropertyFormRow Row_BonusGroupIsAways = GeneralizeRow(CardPropertyFormRowType.Toggle, "LevelEditorPanel_BonusGroupIsAlways", OnBonusGroupIsAlwaysChange, out SetBonusGroupIsAlways);
        Row_BonusGroupProbability = GeneralizeRow(CardPropertyFormRowType.InputField, "LevelEditorPanel_BonusGroupProbability", OnBonusGroupProbabilityChange, out SetBonusGroupProbability);
        Row_BonusGroupIsSingleton = GeneralizeRow(CardPropertyFormRowType.Toggle, "LevelEditorPanel_BonusGroupIsSingleton", OnBonusGroupIsSingletonChange, out SetBonusGroupIsSingleton);
    }

    private PropertyFormRow Row_BonusGroupProbability;
    private PropertyFormRow Row_BonusGroupIsSingleton;

    protected override void SetValue(string value_str, bool forceChange = false)
    {
    }

    public void Initialize(BonusGroup bonusGroup, IEnumerator co_refresh, UnityAction deleteAction, UnityAction clearAction, UnityAction onEditAction, UnityAction<bool, int, int, LevelEditorPanel.SelectCardContents> onStartSelectCard)
    {
        Clear();
        Co_refresh = co_refresh;
        DeleteButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.AddListener(deleteAction);
        ClearButton.onClick.RemoveAllListeners();
        ClearButton.onClick.AddListener(clearAction);
        OnEditAction = onEditAction;
        OnStartSelectCard = onStartSelectCard;
        AddButton.onClick.RemoveAllListeners();
        AddButton.onClick.AddListener(delegate
        {
            LevelPropertyForm_BonusTypeDropdown btd = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelPropertyForm_BonusTypeDropdown].AllocateGameObject<LevelPropertyForm_BonusTypeDropdown>(BonusContainer);
            btd.Initialize(onDeleteButtonClick: delegate
            {
                My_BonusGroupPropertyForm_BonusTypeDropdown.Remove(btd);
                btd.PoolRecycle();
            }, onAddBonusType: delegate(Bonus.BonusTypes type)
            {
                My_BonusGroupPropertyForm_BonusTypeDropdown.Remove(btd);
                btd.PoolRecycle();
                Bonus bonus = null;
                switch (type)
                {
                    case Bonus.BonusTypes.UnlockCardByID:
                    {
                        bonus = new Bonus_UnlockCardByID((int) AllCards.EmptyCardTypes.EmptyCard);
                        break;
                    }
                    case Bonus.BonusTypes.UnlockCardByLevelNum:
                    {
                        bonus = new Bonus_UnlockCardByLevelNum(1);
                        break;
                    }
                    case Bonus.BonusTypes.Budget:
                    {
                        bonus = new Bonus_Budget(25);
                        break;
                    }
                    case Bonus.BonusTypes.BudgetLifeEnergyMixed:
                    {
                        bonus = new Bonus_BudgetLifeEnergyMixed(150);
                        break;
                    }
                    case Bonus.BonusTypes.LifeUpperLimit:
                    {
                        bonus = new Bonus_LifeUpperLimit(2);
                        break;
                    }
                    case Bonus.BonusTypes.EnergyUpperLimit:
                    {
                        bonus = new Bonus_EnergyUpperLimit(2);
                        break;
                    }
                }

                Cur_BonusGroup.Bonuses.Add(bonus);
                Refresh();
            });
            My_BonusGroupPropertyForm_BonusTypeDropdown.Add(btd);
        });

        SetBonusGroup(bonusGroup);
    }

    public UnityAction<string, bool> SetBonusGroupIsAlways;

    private void OnBonusGroupIsAlwaysChange(string value_str)
    {
        bool isAlways = value_str.Equals("True");
        Cur_BonusGroup.IsAlways = isAlways;
        Row_BonusGroupProbability.gameObject.SetActive(!isAlways);
        Row_BonusGroupIsSingleton.gameObject.SetActive(!isAlways);
        UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().StartCoroutine(Co_refresh);
    }

    public UnityAction<string, bool> SetBonusGroupProbability;

    private void OnBonusGroupProbabilityChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < 0)
            {
                SetBonusGroupProbability(0.ToString(), false);
            }
            else
            {
                Cur_BonusGroup.Probability = value;
            }
        }
        else
        {
            SetBonusGroupProbability(0.ToString(), false);
        }
    }

    public UnityAction<string, bool> SetBonusGroupIsSingleton;
    public UnityAction OnEditAction;

    private void OnBonusGroupIsSingletonChange(string value_str)
    {
        bool isSingleton = value_str.Equals("True");
        Cur_BonusGroup.IsSingleton = isSingleton;
    }

    private PropertyFormRow GeneralizeRow(CardPropertyFormRowType type, string labelKey, UnityAction<string> onValueChange, out UnityAction<string, bool> setValue, List<string> dropdownOptionList = null, UnityAction<string> onButtonClick = null)
    {
        PropertyFormRow cpfr = BaseInitialize(type, BonusContainer, labelKey, onValueChange, out setValue, dropdownOptionList, onButtonClick);
        My_BonusGroupPropertyForm.Add(cpfr);
        return cpfr;
    }

    public void OnCurEditBonusUnlockCardChangeCard(int cardID)
    {
        if (CurEdit_Bonus.Cur_Bonus is Bonus_UnlockCardByID b_cid)
        {
            b_cid.CardID = cardID;
            CurEdit_Bonus.Refresh();
        }
    }

    private void Clear()
    {
        foreach (LevelPropertyForm_Bonus b in My_BonusGroupPropertyForm_Bonus)
        {
            b.PoolRecycle();
        }

        My_BonusGroupPropertyForm_Bonus.Clear();
        OnStartSelectCard?.Invoke(false, (int) AllCards.EmptyCardTypes.NoCard, 0, LevelEditorPanel.SelectCardContents.SelectBonusCards);
    }

    public void Refresh()
    {
        Clear();
        SetBonusGroup(Cur_BonusGroup);
    }

    private void SetBonusGroup(BonusGroup bonusGroup)
    {
        if (CurEdit_Bonus != null)
        {
            CurEdit_Bonus.IsEdit = false;
            CurEdit_Bonus = null;
        }

        Cur_BonusGroup = bonusGroup;
        SetBonusGroupIsAlways(Cur_BonusGroup.IsAlways.ToString(), false);
        SetBonusGroupProbability(Cur_BonusGroup.Probability.ToString(), false);
        SetBonusGroupIsSingleton(Cur_BonusGroup.IsSingleton.ToString(), false);

        foreach (Bonus bonus in Cur_BonusGroup.Bonuses)
        {
            GenerateNewBonus(bonus);
        }

        UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().StartCoroutine(Co_refresh);
    }

    private LevelPropertyForm_Bonus CurEdit_Bonus;

    private void GenerateNewBonus(Bonus bonus)
    {
        LevelPropertyForm_Bonus row_bonus = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelPropertyForm_Bonus].AllocateGameObject<LevelPropertyForm_Bonus>(BonusContainer);
        row_bonus.Initialize(bonus,
            onEditButtonClick:
            delegate
            {
                OnEditAction?.Invoke();
                CurEdit_Bonus = row_bonus;

                switch (bonus)
                {
                    case Bonus_UnlockCardByID b_cid:
                    {
                        OnStartSelectCard(true, b_cid.CardID, 1, LevelEditorPanel.SelectCardContents.SelectBonusCards);
                        break;
                    }
                    case Bonus_UnlockCardByLevelNum b_level:
                    {
                        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                        cp.Initialize(
                            descText: LanguageManager.Instance.GetText("LevelEditorPanel_SetUnlockCardLevelNum"),
                            leftButtonClick: delegate
                            {
                                if (int.TryParse(cp.InputText1, out int levelNum))
                                {
                                    cp.CloseUIForm();
                                    b_level.LevelNum = levelNum;
                                    Refresh();
                                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) BonusContainer));
                                }
                                else
                                {
                                    NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_PleaseInputInteger"), 0f, 1f);
                                }
                            },
                            rightButtonClick: delegate { cp.CloseUIForm(); },
                            leftButtonText: LanguageManager.Instance.GetText("Common_Confirm"),
                            rightButtonText: LanguageManager.Instance.GetText("Common_Cancel"),
                            inputFieldPlaceHolderText1: LanguageManager.Instance.GetText("LevelEditorPanel_CardLevelNum")
                        );
                        break;
                    }
                    case Bonus_Budget b_budget:
                    {
                        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                        cp.Initialize(
                            descText: LanguageManager.Instance.GetText("LevelEditorPanel_SetBudget"),
                            leftButtonClick: delegate
                            {
                                if (int.TryParse(cp.InputText1, out int budget))
                                {
                                    cp.CloseUIForm();
                                    b_budget.Budget = budget;
                                    Refresh();
                                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) BonusContainer));
                                }
                                else
                                {
                                    NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_PleaseInputInteger"), 0f, 1f);
                                }
                            },
                            rightButtonClick: delegate { cp.CloseUIForm(); },
                            leftButtonText: LanguageManager.Instance.GetText("Common_Confirm"),
                            rightButtonText: LanguageManager.Instance.GetText("Common_Cancel"),
                            inputFieldPlaceHolderText1: LanguageManager.Instance.GetText("LevelEditorPanel_Budget")
                        );
                        break;
                    }
                    case Bonus_BudgetLifeEnergyMixed b_blem:
                    {
                        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                        cp.Initialize(
                            descText: LanguageManager.Instance.GetText("LevelEditorPanel_SetMixedValuePrice"),
                            leftButtonClick: delegate
                            {
                                if (int.TryParse(cp.InputText1, out int value))
                                {
                                    cp.CloseUIForm();
                                    b_blem.TotalValue = value;
                                    Refresh();
                                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) BonusContainer));
                                }
                                else
                                {
                                    NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_PleaseInputInteger"), 0f, 1f);
                                }
                            },
                            rightButtonClick: delegate { cp.CloseUIForm(); },
                            leftButtonText: LanguageManager.Instance.GetText("Common_Confirm"),
                            rightButtonText: LanguageManager.Instance.GetText("Common_Cancel"),
                            inputFieldPlaceHolderText1: LanguageManager.Instance.GetText("LevelEditorPanel_ValuePlaceHolder")
                        );
                        break;
                    }
                    case Bonus_LifeUpperLimit b_life:
                    {
                        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                        cp.Initialize(
                            descText: LanguageManager.Instance.GetText("LevelEditorPanel_SetLifeUpperLimit"),
                            leftButtonClick: delegate
                            {
                                if (int.TryParse(cp.InputText1, out int lifeUpperLimit))
                                {
                                    cp.CloseUIForm();
                                    b_life.LifeUpperLimit = lifeUpperLimit;
                                    Refresh();
                                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) BonusContainer));
                                }
                                else
                                {
                                    NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_PleaseInputInteger"), 0f, 1f);
                                }
                            },
                            rightButtonClick: delegate { cp.CloseUIForm(); },
                            leftButtonText: LanguageManager.Instance.GetText("Common_Confirm"),
                            rightButtonText: LanguageManager.Instance.GetText("Common_Cancel"),
                            inputFieldPlaceHolderText1: LanguageManager.Instance.GetText("LevelEditorPanel_LifeUpperLimit")
                        );
                        break;
                    }
                    case Bonus_EnergyUpperLimit b_energy:
                    {
                        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                        cp.Initialize(
                            descText: LanguageManager.Instance.GetText("LevelEditorPanel_SetEnergyUpperLimit"),
                            leftButtonClick: delegate
                            {
                                if (int.TryParse(cp.InputText1, out int energyUpperLimit))
                                {
                                    cp.CloseUIForm();
                                    b_energy.EnergyUpperLimit = energyUpperLimit;
                                    Refresh();
                                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) BonusContainer));
                                }
                                else
                                {
                                    NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_PleaseInputInteger"), 0f, 1f);
                                }
                            },
                            rightButtonClick: delegate { cp.CloseUIForm(); },
                            leftButtonText: LanguageManager.Instance.GetText("Common_Confirm"),
                            rightButtonText: LanguageManager.Instance.GetText("Common_Cancel"),
                            inputFieldPlaceHolderText1: LanguageManager.Instance.GetText("LevelEditorPanel_EnergyUpperLimit")
                        );
                        break;
                    }
                }

                foreach (LevelPropertyForm_Bonus _bonus_row in My_BonusGroupPropertyForm_Bonus)
                {
                    _bonus_row.IsEdit = false;
                }

                CurEdit_Bonus.IsEdit = true;
            },
            onDeleteButtonClick: delegate
            {
                Cur_BonusGroup.Bonuses.Remove(bonus);
                Refresh();
            });
        My_BonusGroupPropertyForm_Bonus.Add(row_bonus);
    }

    public void OnLanguageChange()
    {
        foreach (LevelPropertyForm_Bonus b in My_BonusGroupPropertyForm_Bonus)
        {
            b.OnLanguageChange();
        }
    }
}