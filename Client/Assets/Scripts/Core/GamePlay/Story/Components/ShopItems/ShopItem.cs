using System;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class ShopItem : IClone<ShopItem>, Probability
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ShopItemTypes
    {
        Card,
        LifeUpperLimit,
        EnergyUpperLimit,
        Budget,
    }

    public ShopItemTypes ShopItemType;

    public int Price;

    public int Probability { get; set; }
    public bool IsSingleton { get; set; }

    private static int ShopItemIDGenerator = 4000;

    public int ShopItemID;

    public static int GenerateShopItemID()
    {
        return ShopItemIDGenerator++;
    }

    public Probability ProbabilityClone()
    {
        return Clone();
    }

    public ShopItem(ShopItemTypes shopItemType, int price, int probability, bool isSingleton)
    {
        ShopItemType = shopItemType;
        Price = price;
        Probability = probability;
        IsSingleton = isSingleton;
        ShopItemID = GenerateShopItemID();
    }

    public virtual ShopItem Clone()
    {
        return new ShopItem(ShopItemType, Price, Probability, IsSingleton);
    }

    public virtual string Name => "";
    public virtual int PicID => (int) AllCards.EmptyCardTypes.EmptyCard;

    public void ExportToXML(XmlElement parent_ele)
    {
        XmlDocument doc = parent_ele.OwnerDocument;
        XmlElement si_ele = doc.CreateElement("ShopItem");
        parent_ele.AppendChild(si_ele);

        si_ele.SetAttribute("shopItemType", ShopItemType.ToString());
        si_ele.SetAttribute("price", Price.ToString());
        si_ele.SetAttribute("probability", Probability.ToString());
        si_ele.SetAttribute("isSingleton", IsSingleton.ToString());
        ChildrenExportToXML(si_ele);
    }

    protected virtual void ChildrenExportToXML(XmlElement my_ele)
    {
    }

    protected virtual void OnEdit()
    {
    }

    public static ShopItem GenerateShopItemFromXML(XmlNode node_ShopItem, out bool needRefresh)
    {
        needRefresh = false;
        ShopItemTypes type = (ShopItemTypes) Enum.Parse(typeof(ShopItemTypes), node_ShopItem.Attributes["shopItemType"].Value);
        int price = int.Parse(node_ShopItem.Attributes["price"].Value);
        int probability = int.Parse(node_ShopItem.Attributes["probability"].Value);
        bool isSingleton = node_ShopItem.Attributes["isSingleton"].Value == "True";

        switch (type)
        {
            case ShopItemTypes.Card:
            {
                int rareLevel = int.Parse(node_ShopItem.Attributes["cardRareLevel"].Value);
                return new ShopItem_Card(price, rareLevel, probability, isSingleton);
            }
            case ShopItemTypes.Budget:
            {
                int budget = int.Parse(node_ShopItem.Attributes["budget"].Value);
                return new ShopItem_Budget(price, budget, probability, isSingleton);
            }
            case ShopItemTypes.LifeUpperLimit:
            {
                int lifeUpperLimit = int.Parse(node_ShopItem.Attributes["lifeUpperLimit"].Value);
                return new ShopItem_LifeUpperLimit(price, lifeUpperLimit, probability, isSingleton);
            }
            case ShopItemTypes.EnergyUpperLimit:
            {
                int energyUpperLimit = int.Parse(node_ShopItem.Attributes["energyUpperLimit"].Value);
                return new ShopItem_EnergyUpperLimit(price, energyUpperLimit, probability, isSingleton);
            }
        }

        return null;
    }

    public virtual void Serialize(DataStream writer)
    {
        writer.WriteSInt32((int) ShopItemType);
        writer.WriteSInt32(Price);
        writer.WriteSInt32(Probability);
        writer.WriteByte((byte) (IsSingleton ? 0x01 : 0x00));
        writer.WriteSInt32(ShopItemID);
    }

    public static ShopItem Deserialize(DataStream reader)
    {
        ShopItemTypes shopItemType = (ShopItemTypes) reader.ReadSInt32();
        int price = reader.ReadSInt32();
        int probability = reader.ReadSInt32();
        bool isSingleton = reader.ReadByte() == 0x01;
        int shopItemID = reader.ReadSInt32();
        ShopItem si = null;
        switch (shopItemType)
        {
            case ShopItemTypes.Card:
            {
                int rareLevel = reader.ReadSInt32();
                int generateCardID = reader.ReadSInt32();
                si = new ShopItem_Card(price, rareLevel, probability, isSingleton);
                ((ShopItem_Card) si).GenerateCardID = generateCardID;
                break;
            }
            case ShopItemTypes.Budget:
            {
                int budget = reader.ReadSInt32();
                si = new ShopItem_Budget(price, budget, probability, isSingleton);
                break;
            }
            case ShopItemTypes.LifeUpperLimit:
            {
                int lifeUpperLimit = reader.ReadSInt32();
                si = new ShopItem_LifeUpperLimit(price, lifeUpperLimit, probability, isSingleton);
                break;
            }
            case ShopItemTypes.EnergyUpperLimit:
            {
                int energyUpperLimit = reader.ReadSInt32();
                si = new ShopItem_EnergyUpperLimit(price, energyUpperLimit, probability, isSingleton);
                break;
            }
        }

        si.ShopItemID = shopItemID;
        return si;
    }
}