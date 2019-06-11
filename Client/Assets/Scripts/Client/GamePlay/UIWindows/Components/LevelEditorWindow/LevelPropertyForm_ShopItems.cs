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
    private UnityAction<bool, int, int> OnStartSelectCard;
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

    public void SetButtonActions(UnityAction gotoAction, UnityAction clearAction, UnityAction<bool, int, int> onStartSelectCard)
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
                switch (type)
                {
                    case ShopItem.ShopItemTypes.Card:
                    {
                        My_LevelPropertyForm_ShopItemTypeDropdown.Remove(sitd);
                        sitd.PoolRecycle();
                        ShopItem_Card sic = new ShopItem_Card(100, (int) AllCards.EmptyCardTypes.EmptyCard);
                        Cur_ShopItems.Add(sic);
                        Refresh();
                        break;
                    }
                }
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
        OnStartSelectCard?.Invoke(false, (int) AllCards.EmptyCardTypes.NoCard, 0);
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

//        StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ShopItemContainer));
        UIManager.Instance.GetBaseUIForm<StoryEditorPanel>().StartCoroutine(Co_refresh);
    }

    private void GenerateNewShopItem(ShopItem si)
    {
        LevelPropertyForm_ShopItem row_si = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelPropertyForm_ShopItem].AllocateGameObject<LevelPropertyForm_ShopItem>(ShopItemContainer);
        row_si.Initialize(si,
            onEditButtonClick:
            delegate
            {
                CurEdit_ShopItem = row_si;
                if (si is ShopItem_Card sic)
                {
                    OnStartSelectCard(true, sic.CardID, 1);
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