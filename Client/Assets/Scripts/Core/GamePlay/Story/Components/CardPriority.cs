using System;
using System.Collections.Generic;
using System.Xml;

public class CardPriority : IClone<CardPriority>
{
    public List<int> CardIDListByPriority = new List<int>();// sorted by card priority

    public CardPriority(List<int> cardIDListByPriority)
    {
        CardIDListByPriority = cardIDListByPriority;
    }

    public void Clear()
    {
        CardIDListByPriority.Clear();
    }

    public CardPriority Clone()
    {
        return new CardPriority(CloneVariantUtils.List(CardIDListByPriority));
    }

    public void ExportToXML(XmlElement parent_ele)
    {
        XmlDocument doc = parent_ele.OwnerDocument;
        XmlElement cardPriority_ele = doc.CreateElement("CardPriority");
        parent_ele.AppendChild(cardPriority_ele);
        cardPriority_ele.SetAttribute("PriorityCardIDList", string.Join(",", CardIDListByPriority));
    }

    public static CardPriority GenerateCardPriorityFromXML(XmlNode cardPriorityInfo, out bool needRefresh)
    {
        needRefresh = false;
        string id_str = cardPriorityInfo.Attributes["PriorityCardIDList"].Value;
        string[] ids = id_str.Split(',');
        List<int> cardIDList = new List<int>();
        foreach (string s in ids)
        {
            if (string.IsNullOrEmpty(s)) continue;
            if (int.TryParse(s, out int cardID))
            {
                if (!AllCards.CardDict.ContainsKey(cardID))
                {
                    needRefresh = true;
                }
                else
                {
                    cardIDList.Add(cardID);
                }
            }
        }

        CardPriority cp = new CardPriority(cardIDList);
        return cp;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(CardIDListByPriority.Count);

        foreach (int cardID in CardIDListByPriority)
        {
            writer.WriteSInt32(cardID);
        }
    }

    public static CardPriority Deserialize(DataStream reader)
    {
        List<int> cardIDList = new List<int>();
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            int cardID = reader.ReadSInt32();
            cardIDList.Add(cardID);
        }

        CardPriority cp = new CardPriority(cardIDList);
        return cp;
    }
}