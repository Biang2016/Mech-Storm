using System.Collections.Generic;
using System.Xml;

public class BonusGroup : Probability, IClone<BonusGroup>
{
    public bool IsAlways;
    public List<Bonus> Bonuses = new List<Bonus>();

    public int Probability { get; set; }
    public bool IsSingleton { get; set; }

    public Probability ProbabilityClone()
    {
        return Clone();
    }

    private BonusGroup()
    {
    }

    public BonusGroup(bool isAlways, List<Bonus> bonuses, int probability, bool singleton)
    {
        IsAlways = isAlways;
        Bonuses = bonuses;
        Probability = probability;
        IsSingleton = singleton;
    }

    public BonusGroup Clone()
    {
        return new BonusGroup(IsAlways, CloneVariantUtils.List(Bonuses), Probability, IsSingleton);
    }

    public void ExportToXML(XmlElement parent_ele)
    {
        XmlDocument doc = parent_ele.OwnerDocument;
        XmlElement bonusGroupInfo_ele = doc.CreateElement("BonusGroupInfo");
        parent_ele.AppendChild(bonusGroupInfo_ele);

        bonusGroupInfo_ele.SetAttribute("isAlways", IsAlways.ToString());
        if (!IsAlways)
        {
            bonusGroupInfo_ele.SetAttribute("probability", Probability.ToString());
            bonusGroupInfo_ele.SetAttribute("isSingleton", IsSingleton.ToString());
        }

        foreach (Bonus b in Bonuses)
        {
            b.ExportToXML(bonusGroupInfo_ele);
        }
    }

    public static BonusGroup GenerateBonusGroupFromXML(XmlNode bonusGroupInfo, out bool needRefresh)
    {
        needRefresh = false;
        bool isAlways = bonusGroupInfo.Attributes["isAlways"].Value == "True";
        List<Bonus> bonuses = new List<Bonus>();
        int probability = 0;
        bool singleton = false;
        if (isAlways)
        {
            probability = 0;
            singleton = true;
        }
        else
        {
            probability = int.Parse(bonusGroupInfo.Attributes["probability"].Value);
            singleton = bonusGroupInfo.Attributes["isSingleton"].Value == "True";
        }

        BonusGroup bg = new BonusGroup(isAlways, bonuses, probability, singleton);
        for (int i = 0; i < bonusGroupInfo.ChildNodes.Count; i++)
        {
            XmlNode bonusInfo = bonusGroupInfo.ChildNodes.Item(i);
            Bonus bonus = Bonus.GenerateBonusFromXML(bonusInfo, out bool _needRefresh);
            needRefresh |= _needRefresh;
            bg.Bonuses.Add(bonus);
        }

        return bg;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteByte((byte) (IsAlways ? 0x01 : 0x00));

        writer.WriteSInt32(Bonuses.Count);
        foreach (Bonus bonus in Bonuses)
        {
            bonus.Serialize(writer);
        }

        writer.WriteSInt32(Probability);
        writer.WriteByte((byte) (IsSingleton ? 0x01 : 0x00));
    }

    public static BonusGroup Deserialize(DataStream reader)
    {
        BonusGroup newBonusGroup = new BonusGroup();
        newBonusGroup.IsAlways = reader.ReadByte() == 0x01;

        int BonusesCount = reader.ReadSInt32();
        newBonusGroup.Bonuses = new List<Bonus>();
        for (int i = 0; i < BonusesCount; i++)
        {
            newBonusGroup.Bonuses.Add(Bonus.Deserialize(reader));
        }

        newBonusGroup.Probability = reader.ReadSInt32();
        newBonusGroup.IsSingleton = reader.ReadByte() == 0x01;

        return newBonusGroup;
    }
}

public interface Probability
{
    int Probability { get; set; }
    bool IsSingleton { get; set; }

    Probability ProbabilityClone();
}