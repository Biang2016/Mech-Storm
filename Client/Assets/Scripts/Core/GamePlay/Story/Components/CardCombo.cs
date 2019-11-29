using System.Collections.Generic;
using System.Xml;

public class CardCombo : IClone<CardCombo>
{
    public List<int> ComboCardIDList = new List<int>();

    public HashSet<int> ComboCardIDHashSet = new HashSet<int>();

    public CardCombo(List<int> comboCardIdList)
    {
        ComboCardIDList = comboCardIdList;
        foreach (int id in comboCardIdList)
        {
            ComboCardIDHashSet.Add(id);
        }
    }

    public void Clear()
    {
        ComboCardIDList.Clear();
        ComboCardIDHashSet.Clear();
    }

    public CardCombo Clone()
    {
        return new CardCombo(CloneVariantUtils.List(ComboCardIDList));
    }

    public void ExportToXML(XmlElement parent_ele)
    {
        XmlDocument doc = parent_ele.OwnerDocument;
        XmlElement cardCombo_ele = doc.CreateElement("CardCombo");
        parent_ele.AppendChild(cardCombo_ele);
        cardCombo_ele.SetAttribute("ComboCardIDList", string.Join(",", ComboCardIDList));
    }

    public static CardCombo GenerateCardComboFromXML(XmlNode cardComboInfo, out bool needRefresh)
    {
        needRefresh = false;
        string id_str = cardComboInfo.Attributes["ComboCardIDList"].Value;
        string[] ids = id_str.Split(',');
        List<int> comboCardIDList = new List<int>();
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
                    comboCardIDList.Add(cardID);
                }
            }
        }

        CardCombo cc = new CardCombo(comboCardIDList);

        return cc;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(ComboCardIDList.Count);

        foreach (int cardID in ComboCardIDList)
        {
            writer.WriteSInt32(cardID);
        }
    }

    public static CardCombo Deserialize(DataStream reader)
    {
        List<int> cardIDList = new List<int>();
        int count = reader.ReadSInt32();
        for (int i = 0; i < count; i++)
        {
            int cardID = reader.ReadSInt32();
            cardIDList.Add(cardID);
        }

        CardCombo cc = new CardCombo(cardIDList);
        return cc;
    }
}