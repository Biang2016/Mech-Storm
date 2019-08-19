using System.Collections.Generic;
using System.Xml;

public class Shop : Level
{
    public List<ShopItem> ShopItems = new List<ShopItem>();
    public static int SYSTEM_SHOP_MAX_ITEM = 10;
    public int ShopItemCardCount = 8; // 10 个商品里面几个是卡片
    public int ShopItemOthersCount = 2; // 10 个商品里面几个是其他的

    public Shop(LevelThemeCategory levelThemeCategory, int levelPicId, SortedDictionary<string, string> levelNames, int difficultyLevel, List<ShopItem> shopItems, int shopItemCardCount, int shopItemOthersCount) : base(LevelTypes.Shop, levelThemeCategory, levelPicId, levelNames, difficultyLevel)
    {
        ShopItems = shopItems;
        ShopItemCardCount = shopItemCardCount;
        ShopItemOthersCount = shopItemOthersCount;
    }

    public override Level Clone()
    {
        Shop shop = new Shop(LevelThemeCategory, LevelPicID, CloneVariantUtils.SortedDictionary(LevelNames), DifficultyLevel, CloneVariantUtils.List(ShopItems), ShopItemCardCount, ShopItemOthersCount);
        shop.LevelID = LevelID;
        return shop;
    }

    public override Level Variant()
    {
        return Clone();
    }

    public void AddItemToShop(int cardID, int cardPrice)
    {
    }

    public int AVGPrice
    {
        get
        {
            if (ShopItems.Count == 0) return 0;
            int sum_price = 0;
            foreach (ShopItem si in ShopItems)
            {
                sum_price += si.Price;
            }

            int avg_price = sum_price / ShopItems.Count;
            return avg_price;
        }
    }

    /// <summary>
    /// Can only be executed in StoryEditor/CardEditor/LevelEditor
    /// </summary>
    public override bool DeleteCard(int cardID)
    {
        return false;
    }

    protected override void ChildrenExportToXML(XmlElement level_ele)
    {
        XmlDocument doc = level_ele.OwnerDocument;
        XmlElement shopInfo_ele = doc.CreateElement("ShopInfo");
        level_ele.AppendChild(shopInfo_ele);

        foreach (ShopItem si in ShopItems)
        {
            si.ExportToXML(shopInfo_ele);
        }

        shopInfo_ele.SetAttribute("shopItemCardCount", ShopItemCardCount.ToString());
        shopInfo_ele.SetAttribute("shopItemOthersCount", ShopItemOthersCount.ToString());
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(ShopItems.Count);
        foreach (ShopItem si in ShopItems)
        {
            si.Serialize(writer);
        }

        writer.WriteSInt32(ShopItemCardCount);
        writer.WriteSInt32(ShopItemOthersCount);
    }
}