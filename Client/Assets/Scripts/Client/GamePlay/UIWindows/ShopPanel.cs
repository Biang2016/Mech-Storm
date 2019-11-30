using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : BaseUIForm
{
    private ShopPanel()
    {
    }

    [SerializeField] private RawImage CardRenderRawImage;
    [SerializeField] private Text LeftClickTipText;
    [SerializeField] private Text RightClickTipText;
    [SerializeField] private Text MyDeckText;

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: false,
            isClickElsewhereClose: false,
            uiForms_Type: UIFormTypes.Normal,
            uiForms_ShowMode: UIFormShowModes.Normal,
            uiForm_LucencyType: UIFormLucencyTypes.Penetrable);

        LeaveShopButton.onClick.AddListener(delegate
        {
            ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
            cp.Initialize(LanguageManager.Instance.GetText("ShopPanel_LeaveWarning"), LanguageManager.Instance.GetText("Common_Leave"), LanguageManager.Instance.GetText("Common_Cancel"),
                leftButtonClick: delegate
                {
                    StoryManager.Instance.GetStory().Crystal = CrystalWhenEnter;
                    foreach (ShopItem shopItem in Cur_BoughtShopItem)
                    {
                        BuyShopItemRequest si_request = new BuyShopItemRequest(Client.Instance.Proxy.ClientID, shopItem);
                        Client.Instance.Proxy.SendMessage(si_request);
                    }

                    LeaveShopRequest request = new LeaveShopRequest(Client.Instance.Proxy.ClientID, StoryManager.Instance.GetStory().CurrentFightingChapterID, Cur_Shop.LevelID);
                    Client.Instance.Proxy.SendMessage(request);
                    CloseUIForm();
                    UIManager.Instance.ShowUIForms<StoryPanel>();
                    cp.CloseUIForm();
                },
                rightButtonClick: delegate { cp.CloseUIForm(); }
            );
        });

        LanguageManager.Instance.RegisterTextKeys(new List<(Text, string)>
        {
            (LeftClickTipText, "ShopPanel_LeftClickTipText"),
            (RightClickTipText, "ShopPanel_RightClickTipText"),
            (MyDeckText, "ShopPanel_MyDeckText"),
        });
    }

    public override void Display()
    {
        base.Display();
        AudioManager.Instance.BGMFadeIn("bgm/ShopPanel");
        UIManager.Instance.GetBaseUIForm<StoryPlayerInformationPanel>().Display();
    }

    public override void Hide()
    {
        base.Hide();
        UIManager.Instance.GetBaseUIForm<StoryPlayerInformationPanel>().Hide();
    }

    [SerializeField] private TextMeshProUGUI ShopNameText;
    [SerializeField] private Transform ShopItemContainer;
    [SerializeField] private Button LeaveShopButton;
    public Shop Cur_Shop;
    private SortedDictionary<int, ShopItemButton> ShopItemButtons = new SortedDictionary<int, ShopItemButton>();
    private List<ShopItemSmallContainer> ShopItemSmallContainers = new List<ShopItemSmallContainer>();

    public List<ShopItem> Cur_BoughtShopItem = new List<ShopItem>();

    public const int SHOP_ITEM_OTHERS_GROUP_CAPACITY = 4; // 其他奖励几个分在一个格子里

    public int CrystalWhenEnter = 0;

    public void Initialize(Shop shop)
    {
        Reset();

        CrystalWhenEnter = StoryManager.Instance.GetStory().Crystal;
        Cur_Shop = (Shop) shop.Clone();

        ShopNameText.text = Cur_Shop.LevelNames[LanguageManager.Instance.GetCurrentLanguage()];

        List<ShopItem_Card> si_cards = new List<ShopItem_Card>();
        List<ShopItem> si_others = new List<ShopItem>();
        foreach (ShopItem si in Cur_Shop.ShopItems)
        {
            if (si is ShopItem_Card sic)
            {
                si_cards.Add(sic);
            }
            else
            {
                si_others.Add(si);
            }
        }

        int shopItemCardCount = shop.ShopItemCardCount;
        int shopItemOthersCount = shop.ShopItemOthersCount;

        if (si_cards.Count > 0)
        {
            List<ShopItem_Card> si_cards_random = Utils.GetRandomWithProbabilityFromList(si_cards, shopItemCardCount); // 卡片按概率随机
            HashSet<int> cardIdHashSet = new HashSet<int>();
            foreach (ShopItem_Card sic in si_cards_random)
            {
                CardInfo_Base ci = AllCards.GetRandomCardInfoByLevelNum(sic.CardRareLevel, cardIdHashSet);
                if (ci != null)
                {
                    sic.GenerateCardID = ci.CardID;
                    sic.Price = Mathf.CeilToInt(Random.Range(ci.BaseInfo.ShopPrice * 0.8f, ci.BaseInfo.ShopPrice * 1.2f));
                    cardIdHashSet.Add(sic.GenerateCardID);
                    ShopItemButton btn = ShopItemButton.GenerateShopItemButton(sic, ShopItemSmallContainers, ShopItemContainer);
                    ShopItemButtons.Add(sic.ShopItemID, btn);
                }
                else
                {
                    shopItemCardCount--;
                    shopItemOthersCount++;
                }
            }
        }

        if (si_others.Count > 0)
        {
            List<ShopItem> si_others_random = Utils.GetRandomWithProbabilityFromList(si_others, shopItemOthersCount * SHOP_ITEM_OTHERS_GROUP_CAPACITY); // 其他物品按概率随机

            foreach (ShopItem si in si_others_random)
            {
                if (si is ShopItem_Budget)
                {
                    ShopItemButton btn = ShopItemButton.GenerateShopItemButton(si, ShopItemSmallContainers, ShopItemContainer);
                    ShopItemButtons.Add(si.ShopItemID, btn);
                }
            }

            foreach (ShopItem si in si_others_random)
            {
                if (si is ShopItem_LifeUpperLimit)
                {
                    ShopItemButton btn = ShopItemButton.GenerateShopItemButton(si, ShopItemSmallContainers, ShopItemContainer);
                    ShopItemButtons.Add(si.ShopItemID, btn);
                }
            }

            foreach (ShopItem si in si_others_random)
            {
                if (si is ShopItem_EnergyUpperLimit)
                {
                    ShopItemButton btn = ShopItemButton.GenerateShopItemButton(si, ShopItemSmallContainers, ShopItemContainer);
                    ShopItemButtons.Add(si.ShopItemID, btn);
                }
            }
        }

        RefreshAllShopItemAffordable();
    }

    public void OnLanguageChange()
    {
        ShopNameText.text = Cur_Shop.LevelNames[LanguageManager.Instance.GetCurrentLanguage()];
    }

    public void SetAllButtonLock(bool isLocked)
    {
        foreach (KeyValuePair<int, ShopItemButton> kv in ShopItemButtons)
        {
            kv.Value.SetButtonLocked(isLocked);
        }
    }

    public void SetShopItemSold(int shopItemID)
    {
        ShopItemButtons[shopItemID].IsSold = true;
    }

    public void RefreshAllShopItemAffordable()
    {
        foreach (KeyValuePair<int, ShopItemButton> kv in ShopItemButtons)
        {
            kv.Value.Affordable = kv.Value.Cur_ShopItem.Price <= StoryManager.Instance.GetStory().Crystal;
        }
    }

    private void Reset()
    {
        foreach (KeyValuePair<int, ShopItemButton> kv in ShopItemButtons)
        {
            kv.Value.PoolRecycle();
        }

        ShopItemButtons.Clear();

        foreach (ShopItemSmallContainer c in ShopItemSmallContainers)
        {
            c.PoolRecycle();
        }

        ShopItemSmallContainers.Clear();
        Cur_BoughtShopItem.Clear();
    }

    public void OnDeckButtonClick()
    {
        UIManager.Instance.ShowUIForms<SelectBuildPanel>();
    }
}