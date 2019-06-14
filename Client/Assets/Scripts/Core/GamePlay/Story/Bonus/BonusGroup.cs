using System.Collections.Generic;
using System.Xml;

public class BonusGroup : Probability, IClone<BonusGroup>
{
    public bool IsAlways;
    public List<Bonus> Bonuses = new List<Bonus>();

    public int Probability { get; set; }
    public bool Singleton { get; set; }

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
        Singleton = singleton;
    }

    public BonusGroup Clone()
    {
        return new BonusGroup(IsAlways, CloneVariantUtils.List(Bonuses), Probability, Singleton);
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
            bonusGroupInfo_ele.SetAttribute("singleton", Singleton.ToString());
        }

        foreach (Bonus b in Bonuses)
        {
            XmlElement bonus_ele = doc.CreateElement("BonusInfo");
            bonusGroupInfo_ele.AppendChild(bonus_ele);

            bonus_ele.SetAttribute("name", b.M_BonusType.ToString());
            bonus_ele.SetAttribute("value", b.BonusFinalValue.ToString());
        }
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
        writer.WriteByte((byte) (Singleton ? 0x01 : 0x00));
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
        newBonusGroup.Singleton = reader.ReadByte() == 0x01;

        return newBonusGroup;
    }
}

public interface Probability
{
    int Probability { get; set; }
    bool Singleton { get; set; }

    Probability ProbabilityClone();
}