using System.Collections.Generic;
using System.Xml;

public class ShopItem_Card : ShopItem
{
    public ShopItem_Card(int price, int cardID) : base(ShopItemTypes.Card, price)
    {
        CardID = cardID;
    }

    public int CardID;

    public override string Name
    {
        get
        {
            if (AllCards.CardDict.ContainsKey(CardID))
            {
                SortedDictionary<string, string> names = AllCards.CardDict[CardID].BaseInfo.CardNames;
                return names[LanguageManager_Common.GetCurrentLanguage()];
            }

            return "";
        }
    }

    public override int PicID
    {
        get
        {
            if (AllCards.CardDict.ContainsKey(CardID))
            {
                int picID = AllCards.CardDict[CardID].BaseInfo.PictureID;
                return picID;
            }

            return (int) AllCards.EmptyCardTypes.EmptyCard;
        }
    }

    public override void OnEdit()
    {
        base.OnEdit();
    }

    public override ShopItem Clone()
    {
        return new ShopItem_Card(Price, CardID);
    }

    protected override void ChildrenExportToXML(XmlElement my_ele)
    {
        base.ChildrenExportToXML(my_ele);
        my_ele.SetAttribute("cardID", CardID.ToString());
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(CardID);
    }
}