using System.Collections.Generic;
using System.Xml;

public class Shop : Level
{
    public List<ShopItem> ShopItems = new List<ShopItem>();

    public Shop(LevelThemeCategory levelThemeCategory, int levelPicId, SortedDictionary<string, string> levelNames, List<ShopItem> shopItems) : base(LevelType.Shop, levelThemeCategory, levelPicId, levelNames)
    {
        ShopItems = shopItems;
    }

    public override Level Clone()
    {
        return new Shop(LevelThemeCategory, LevelPicID, CloneVariantUtils.SortedDictionary(LevelNames), CloneVariantUtils.List(ShopItems));
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
        List<ShopItem> remove_SI = new List<ShopItem>();
        foreach (ShopItem si in ShopItems)
        {
            if (si is ShopItem_Card si_card)
            {
                if (si_card.CardID == cardID)
                {
                    remove_SI.Add(si);
                }
            }
        }

        foreach (ShopItem si in remove_SI)
        {
            ShopItems.Remove(si);
        }

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
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(ShopItems.Count);
        foreach (ShopItem si in ShopItems)
        {
            si.Serialize(writer);
        }
    }
}