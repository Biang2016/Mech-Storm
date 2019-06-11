using System;
using System.Xml;

public class ShopItem : IClone<ShopItem>
{
    public enum ShopItemTypes
    {
        Card = 0,
        LifeUpperLimit = 1,
        LifeHeal = 2,
        EnergyUpperLimit = 3,
        Budget = 4,
    }

    public ShopItemTypes ShopItemType;

    public int Price;

    public ShopItem(ShopItemTypes shopItemType, int price)
    {
        ShopItemType = shopItemType;
        Price = price;
    }

    public virtual ShopItem Clone()
    {
        return new ShopItem(ShopItemType, Price);
    }

    public virtual string Name => "";
    public virtual int PicID => (int) AllCards.EmptyCardTypes.EmptyCard;

    protected void BaseExportToXML(XmlElement my_ele)
    {
    }

    public void ExportToXML(XmlElement parent_ele)
    {
        XmlDocument doc = parent_ele.OwnerDocument;
        XmlElement si_ele = doc.CreateElement("ShopItem");
        parent_ele.AppendChild(si_ele);

        si_ele.SetAttribute("shopItemType", ShopItemType.ToString());
        si_ele.SetAttribute("price", Price.ToString());
        ChildrenExportToXML(si_ele);
    }

    protected virtual void ChildrenExportToXML(XmlElement my_ele)
    {
    }

    public static ShopItem GenerateShopItemFroXML(XmlNode node_ShopItem, out bool needRefresh)
    {
        needRefresh = false;
        ShopItemTypes type = (ShopItemTypes) Enum.Parse(typeof(ShopItemTypes), node_ShopItem.Attributes["shopItemType"].Value);
        int price = int.Parse(node_ShopItem.Attributes["price"].Value);

        switch (type)
        {
            case ShopItemTypes.Card:
            {
                int cardID = int.Parse(node_ShopItem.Attributes["cardID"].Value);
                if (!AllCards.CardDict.ContainsKey(cardID))
                {
                    needRefresh = true;
                    return null;
                }

                return new ShopItem_Card(price, cardID);
            }
        }

        return null;
    }

    public virtual void Serialize(DataStream writer)
    {
        writer.WriteSInt32((int) ShopItemType);
        writer.WriteSInt32(Price);
    }

    public static ShopItem Deserialize(DataStream reader)
    {
        ShopItemTypes shopItemType = (ShopItemTypes) reader.ReadSInt32();
        int price = reader.ReadSInt32();
        ShopItem si = null;
        switch (shopItemType)
        {
            case ShopItemTypes.Card:
            {
                int cardID = reader.ReadSInt32();
                si = new ShopItem_Card(price, cardID);
                break;
            }
        }

        return si;
    }
}