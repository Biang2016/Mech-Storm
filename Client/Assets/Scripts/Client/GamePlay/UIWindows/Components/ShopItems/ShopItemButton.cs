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
            else
            {
                PriceText.color = Color.white;
            }
        }
    }

    public virtual void Initialize(ShopItem shopItem)
    {
        Cur_ShopItem = shopItem.Clone();
        if (shopItem is ShopItem_Card si_card)
        {
            ((ShopItem_Card) Cur_ShopItem).GenerateCardID = si_card.GenerateCardID;
        }

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
        return shopItemButton;
    }

    public void OnHover()
    {
        AudioManager.Instance.SoundPlay("sfx/BonusHover", 0.5f);
    }

    public void ShopItemButtonLeftClick()
    {
        if (Affordable)
        {
            AudioManager.Instance.SoundPlay("sfx/OnBuyShopItem");
            ShopPanel sp = UIManager.Instance.GetBaseUIForm<ShopPanel>();
            sp.SetAllButtonLock(true);
            sp.Cur_BoughtShopItem.Add(Cur_ShopItem);
            StoryManager.Instance.GetStory().Crystal -= Cur_ShopItem.Price;
            IsSold = true;
            SetButtonLocked(true);
            UIManager.Instance.GetBaseUIForm<StoryPlayerInformationPanel>().SetCrystal(StoryManager.Instance.GetStory().Crystal);
            sp.RefreshAllShopItemAffordable();
        }
        else
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("ShopPanel_CannotAffordShopItem"), 0f, 1f);
        }
    }

    public void ShopItemButtonRightClick()
    {
        ShopPanel sp = UIManager.Instance.GetBaseUIForm<ShopPanel>();
        if (sp.Cur_BoughtShopItem.Contains(Cur_ShopItem))
        {
            sp.Cur_BoughtShopItem.Remove(Cur_ShopItem);
            StoryManager.Instance.GetStory().Crystal += Cur_ShopItem.Price;
            UIManager.Instance.GetBaseUIForm<StoryPlayerInformationPanel>().SetCrystal(StoryManager.Instance.GetStory().Crystal);
            IsSold = false;
            SetButtonLocked(false);
            sp.RefreshAllShopItemAffordable();
        }
    }
}