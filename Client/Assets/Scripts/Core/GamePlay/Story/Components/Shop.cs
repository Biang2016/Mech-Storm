using System.Collections.Generic;
using System.Text;
using System.Xml;

public class Shop : Level
{
    public SortedDictionary<int, int> ItemPrices = new SortedDictionary<int, int>();

    public Shop(LevelThemeCategory levelThemeCategory, int levelPicId, SortedDictionary<string, string> levelNames, SortedDictionary<int, int> itemPrices) : base(LevelType.Shop, levelThemeCategory, levelPicId, levelNames)
    {
        ItemPrices = itemPrices;
    }

    public override Level Clone()
    {
        return new Shop(LevelThemeCategory, LevelPicID, CloneVariantUtils.SortedDictionary(LevelNames), CloneVariantUtils.SortedDictionary(ItemPrices));
    }

    public override Level Variant()
    {
        return Clone();
    }

    /// <summary>
    /// Can only be executed in StoryEditor/CardEditor/LevelEditor
    /// </summary>
    public override bool DeleteCard(int cardID)
    {
        if (ItemPrices.ContainsKey(cardID))
        {
            ItemPrices.Remove(cardID);
            AllLevels.RefreshLevelXML(this);
            return true;
        }

        return false;
    }

    protected override void ChildrenExportToXML(XmlElement level_ele)
    {
        XmlDocument doc = level_ele.OwnerDocument;
        XmlElement shopInfo_ele = doc.CreateElement("ShopInfo");
        level_ele.AppendChild(shopInfo_ele);
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<int, int> kv in ItemPrices)
        {
            sb.Append(string.Format("({0},{1});", kv.Key, kv.Value));
        }

        string itemPrices_str = sb.ToString().Trim(';');
        shopInfo_ele.SetAttribute("itemPrices", itemPrices_str);
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(ItemPrices.Count);
        foreach (KeyValuePair<int, int> kv in ItemPrices)
        {
            writer.WriteSInt32(kv.Key);
            writer.WriteSInt32(kv.Value);
        }
    }

    public static Shop Deserialize(DataStream reader) // 除Level类外 不可直接调用
    {
        SortedDictionary<int, int> ItemPrices = new SortedDictionary<int, int>();
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            int cardID = reader.ReadSInt32();
            int itemCount = reader.ReadSInt32();
            ItemPrices.Add(cardID, itemCount);
        }

        return new Shop(LevelThemeCategory.Energy, 0, null, ItemPrices);
    }
}