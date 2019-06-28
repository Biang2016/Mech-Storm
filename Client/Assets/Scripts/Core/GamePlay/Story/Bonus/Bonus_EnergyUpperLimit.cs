using System.Collections.Generic;
using System.Xml;

public class Bonus_EnergyUpperLimit : Bonus
{
    public int EnergyUpperLimit;
    public override int PicID { get; set; } = (int) AllCards.SpecialPicIDs.EnergyUpperLimit;

    public Bonus_EnergyUpperLimit(int energyUpperLimit) : base(BonusTypes.EnergyUpperLimit)
    {
        EnergyUpperLimit = energyUpperLimit;
    }

    public override string GetDesc()
    {
        Dictionary<BonusTypes, string> dic = BonusDescRaw[LanguageManager_Common.GetCurrentLanguage()];
        return string.Format(dic[BonusType], EnergyUpperLimit);
    }

    protected override void ChildrenExportToXML(XmlElement my_ele)
    {
        base.ChildrenExportToXML(my_ele);
        my_ele.SetAttribute("energyUpperLimit", EnergyUpperLimit.ToString());
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(EnergyUpperLimit);
    }

    public override Bonus Clone()
    {
        return new Bonus_EnergyUpperLimit(EnergyUpperLimit);
    }
}