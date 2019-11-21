using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : PoolObject
{
    public Button Button;
    public Image SoldImage;
    [SerializeField] private Text PriceText;

    public ShopItem Cur_ShopItem;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        Cur_ShopItem = null;
        IsSold = false;
        Button.onClick.RemoveAllListeners();
        PriceText.text = "";
        PriceText.color = Color.white;
    }

    public void SetButtonLocked(bool isLocked)
    {
        if (isSold)
        {
            Button.enabled = false;
            return;
        }

        Button.enabled = !isLocked;
    }

    private bool isSold;

    public bool IsSold
    {
        get { return isSold; }
        set
        {
            isSold = value;
            SoldImage.gameObject.SetActive(value);
            Button.enabled = !isSold;
        }
    }

    private bool affordable;

    public bool Affordable
    {
        get { return affordable; }
        set
        {
            affordable = value;
            if (!affordable)
            {
                PriceText.color = Color.red;
            }
        }
    }

    public virtual void Initialize(ShopItem shopItem)
    {
        Cur_ShopItem = shopItem.Clone();
        PriceText.text = shopItem.Price.ToString();
        IsSold = false;
    }

    public static ShopItemButton GenerateShopItemButton(ShopItem shopItem, List<ShopItemSmallContainer> smallItemContainers, Transform parent)
    {
        ShopItemButton shopItemButton;
        switch (shopItem)
        {
            case ShopItem_Card si_card:
            {
                shopItemButton = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ShopItemButton_Card].AllocateGameObject<ShopItemButton_Card>(parent);
                break;
            }
            default:
            {
                ShopItemSmallContainer container = null;

                foreach (ShopItemSmallContainer c in smallItemContainers)
                {
                    if (c.ItemCount < ShopItemSmallContainer.Capacity)
                    {
                        container = c;
                        break;
                    }
                }

                if (!container)
                {
                    container = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ShopItemSmallContainer].AllocateGameObject<ShopItemSmallContainer>(parent);
                    smallItemContainers.Add(container);
                }

                shopItemButton = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ShopItemButton_Small].AllocateGameObject<ShopItemButton_Small>(container.Container);
                container.ItemCount++;
                break;
            }
        }

        shopItemButton.Initialize(shopItem);
        shopItemButton.Button.onClick.AddListener(delegate
        {
            if (shopItemButton.Affordable)
            {
                AudioManager.Instance.SoundPlay("sfx/OnBuyShopItem");
                UIManager.Instance.GetBaseUIForm<ShopPanel>().SetAllButtonLock(true);
                BuyShopItemRequest request = new BuyShopItemRequest(Client.Instance.Proxy.ClientID, shopItem);
                Client.Instance.Proxy.SendMessage(request);
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("ShopPanel_CannotAffordShopItem"), 0f, 1f);
            }
        });
        return shopItemButton;
    }

    public void OnHover()
    {
        AudioManager.Instance.SoundPlay("sfx/BonusHover", 0.5f);
    }
}