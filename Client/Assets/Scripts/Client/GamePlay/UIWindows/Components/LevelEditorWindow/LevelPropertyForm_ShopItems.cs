using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelPropertyForm_ShopItems : PropertyFormRow
{
    [SerializeField] private Transform ShopItemContainer;

    [SerializeField] private Button GoToButton;
    [SerializeField] private Button AddButton;
    [SerializeField] private Button ClearButton;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        Clear();
        GoToButton.onClick.RemoveAllListeners();
        AddButton.onClick.RemoveAllListeners();
        ClearButton.onClick.RemoveAllListeners();
        OnStartSelectCard = null;
    }

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKey(Label, "LevelEditorPanel_ShopItems");
    }

    protected override void SetValue(string value_str)
    {
    }

    private List<ShopItem> Cur_ShopItems;
    private List<LevelPropertyForm_ShopItem> My_LevelPropertyForm_ShopItem = new List<LevelPropertyForm_ShopItem>();
    private List<LevelPropertyForm_ShopItemTypeDropdown> My_LevelPropertyForm_ShopItemTypeDropdown = new List<LevelPropertyForm_ShopItemTypeDropdown>();
    private UnityAction<bool, int, int, LevelEditorPanel.SelectCardContents> OnStartSelectCard;
    private IEnumerator Co_refresh;

    public void Initialize(List<ShopItem> shopItems, IEnumerator co_refresh)
    {
        Co_refresh = co_refresh;
        Clear();
        GoToButton.onClick.RemoveAllListeners();
        AddButton.onClick.RemoveAllListeners();
        ClearButton.onClick.RemoveAllListeners();
        Cur_ShopItems = shopItems;
        Refresh();
    }

    public void SetButtonActions(UnityAction gotoAction, UnityAction clearAction, UnityAction<bool, int, int, LevelEditorPanel.SelectCardContents> onStartSelectCard)
    {
        GoToButton.onClick.RemoveAllListeners();
        GoToButton.onClick.AddListener(gotoAction);
        AddButton.onClick.RemoveAllListeners();
        AddButton.onClick.AddListener(delegate
        {
            LevelPropertyForm_ShopItemTypeDropdown sitd = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelPropertyForm_ShopItemTypeDropdown].AllocateGameObject<LevelPropertyForm_ShopItemTypeDropdown>(ShopItemContainer);
            sitd.Initialize(onDeleteButtonClick: delegate
            {
                My_LevelPropertyForm_ShopItemTypeDropdown.Remove(sitd);
                sitd.PoolRecycle();
            }, onAddShopItem: delegate(ShopItem.ShopItemTypes type)
            {
                My_LevelPropertyForm_ShopItemTypeDropdown.Remove(sitd);
                sitd.PoolRecycle();
                ShopItem si = null;
                switch (type)
                {
                    case ShopItem.ShopItemTypes.Card:
                    {
                        si = new ShopItem_Card(100, (int) AllCards.EmptyCardTypes.EmptyCard);
                        break;
                    }
                    case ShopItem.ShopItemTypes.Budget:
                    {
                        si = new ShopItem_Budget(100, 50);
                        break;
                    }
                    case ShopItem.ShopItemTypes.LifeUpperLimit:
                    {
                        si = new ShopItem_LifeUpperLimit(100, 2);
                        break;
                    }
                    case ShopItem.ShopItemTypes.EnergyUpperLimit:
                    {
                        si = new ShopItem_EnergyUpperLimit(100, 2);
                        break;
                    }
                }

                Cur_ShopItems.Add(si);
                Refresh();
            });
            My_LevelPropertyForm_ShopItemTypeDropdown.Add(sitd);
        });
        ClearButton.onClick.RemoveAllListeners();
        ClearButton.onClick.AddListener(clearAction);
        OnStartSelectCard = onStartSelectCard;
    }

    private void Clear()
    {
        foreach (LevelPropertyForm_ShopItem si in My_LevelPropertyForm_ShopItem)
        {
            si.PoolRecycle();
        }

        My_LevelPropertyForm_ShopItem.Clear();
        foreach (LevelPropertyForm_ShopItemTypeDropdown si in My_LevelPropertyForm_ShopItemTypeDropdown)
        {
            si.PoolRecycle();
        }

        My_LevelPropertyForm_ShopItemTypeDropdown.Clear();
        OnStartSelectCard?.Invoke(false, (int) AllCards.EmptyCardTypes.NoCard, 0, LevelEditorPanel.SelectCardContents.SelectShopItemCards);
    }

    private LevelPropertyForm_ShopItem CurEdit_ShopItem;

    public void OnCurEditShopItemCardChangeCard(int cardID, int price)
    {
        if (CurEdit_ShopItem.Cur_ShopItem is ShopItem_Card sic)
        {
            sic.CardID = cardID;
            sic.Price = price;
            CurEdit_ShopItem.Refresh();
        }
    }

    public void Refresh()
    {
        Clear();

        if (CurEdit_ShopItem != null)
        {
            CurEdit_ShopItem.IsEdit = false;
            CurEdit_ShopItem = null;
        }

        foreach (ShopItem si in Cur_ShopItems)
        {
            GenerateNewShopItem(si);
        }

        UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().StartCoroutine(Co_refresh);
    }

    private void GenerateNewShopItem(ShopItem si)
    {
        LevelPropertyForm_ShopItem row_si = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelPropertyForm_ShopItem].AllocateGameObject<LevelPropertyForm_ShopItem>(ShopItemContainer);
        row_si.Initialize(si,
            onEditButtonClick:
            delegate
            {
                CurEdit_ShopItem = row_si;

                switch (si)
                {
                    case ShopItem_Card sic:
                    {
                        OnStartSelectCard(true, sic.CardID, 1, LevelEditorPanel.SelectCardContents.SelectShopItemCards);
                        break;
                    }
                    case ShopItem_Budget sib:
                    {
                        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                        cp.Initialize(
                            descText: LanguageManager.Instance.GetText("LevelEditorPanel_SetBudgetPrice"),
                            leftButtonClick: delegate
                            {
                                if (int.TryParse(cp.InputText1, out int budget))
                                {
                                    if (int.TryParse(cp.InputText2, out int price))
                                    {
                                        cp.CloseUIForm();
                                        sib.Budget = budget;
                                        sib.Price = price;
                                        Refresh();
                                        StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ShopItemContainer));
                                    }
                                    else
                                    {
                                        NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_PleaseInputInteger"), 0f, 1f);
                                    }
                                }
                                else
                                {
                                    NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_PleaseInputInteger"), 0f, 1f);
                                }
                            },
                            rightButtonClick: delegate { cp.CloseUIForm(); },
                            leftButtonText: LanguageManager.Instance.GetText("Common_Confirm"),
                            rightButtonText: LanguageManager.Instance.GetText("Common_Cancel"),
                            inputFieldPlaceHolderText1: LanguageManager.Instance.GetText("LevelEditorPanel_BudgetPlaceHolder"),
                            inputFieldPlaceHolderText2: LanguageManager.Instance.GetText("LevelEditorPanel_PricePlaceHolder")
                        );
                        break;
                    }
                    case ShopItem_LifeUpperLimit silu:
                    {
                        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                        cp.Initialize(
                            descText: LanguageManager.Instance.GetText("LevelEditorPanel_SetLifeUpperLimitPrice"),
                            leftButtonClick: delegate
                            {
                                if (int.TryParse(cp.InputText1, out int lifeUpperLimit))
                                {
                                    if (int.TryParse(cp.InputText2, out int price))
                                    {
                                        cp.CloseUIForm();
                                        silu.LifeUpperLimit = lifeUpperLimit;
                                        silu.Price = price;
                                        Refresh();
                                        StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ShopItemContainer));
                                    }
                                    else
                                    {
                                        NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_PleaseInputInteger"), 0f, 1f);
                                    }
                                }
                                else
                                {
                                    NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_PleaseInputInteger"), 0f, 1f);
                                }
                            },
                            rightButtonClick: delegate { cp.CloseUIForm(); },
                            leftButtonText: LanguageManager.Instance.GetText("Common_Confirm"),
                            rightButtonText: LanguageManager.Instance.GetText("Common_Cancel"),
                            inputFieldPlaceHolderText1: LanguageManager.Instance.GetText("LevelEditorPanel_LifeUpperLimitLabelValueText"),
                            inputFieldPlaceHolderText2: LanguageManager.Instance.GetText("LevelEditorPanel_PricePlaceHolder")
                        );
                        break;
                    }
                    case ShopItem_EnergyUpperLimit sieu:
                    {
                        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                        cp.Initialize(
                            descText: LanguageManager.Instance.GetText("LevelEditorPanel_SetEnergyUpperLimitPrice"),
                            leftButtonClick: delegate
                            {
                                if (int.TryParse(cp.InputText1, out int energyUpperLimit))
                                {
                                    if (int.TryParse(cp.InputText2, out int price))
                                    {
                                        cp.CloseUIForm();
                                        sieu.EnergyUpperLimit = energyUpperLimit;
                                        sieu.Price = price;
                                        Refresh();
                                        StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ShopItemContainer));
                                    }
                                    else
                                    {
                                        NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_PleaseInputInteger"), 0f, 1f);
                                    }
                                }
                                else
                                {
                                    NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_PleaseInputInteger"), 0f, 1f);
                                }
                            },
                            rightButtonClick: delegate { cp.CloseUIForm(); },
                            leftButtonText: LanguageManager.Instance.GetText("Common_Confirm"),
                            rightButtonText: LanguageManager.Instance.GetText("Common_Cancel"),
                            inputFieldPlaceHolderText1: LanguageManager.Instance.GetText("LevelEditorPanel_EnergyUpperLimitLabelValueText"),
                            inputFieldPlaceHolderText2: LanguageManager.Instance.GetText("LevelEditorPanel_PricePlaceHolder")
                        );
                        break;
                    }
                }

                foreach (LevelPropertyForm_ShopItem _rsi in My_LevelPropertyForm_ShopItem)
                {
                    _rsi.IsEdit = false;
                }

                CurEdit_ShopItem.IsEdit = true;
            },
            onDeleteButtonClick: delegate
            {
                Cur_ShopItems.Remove(si);
                Refresh();
            });
        My_LevelPropertyForm_ShopItem.Add(row_si);
    }

    public void OnLanguageChange()
    {
        foreach (LevelPropertyForm_ShopItem si in My_LevelPropertyForm_ShopItem)
        {
            si.OnLanguageChange();
        }
    }
}