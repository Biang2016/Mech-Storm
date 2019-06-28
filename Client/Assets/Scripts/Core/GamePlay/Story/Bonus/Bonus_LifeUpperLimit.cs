using System.Collections.Generic;
using System.Xml;

public class Bonus_LifeUpperLimit : Bonus
{
    public int LifeUpperLimit;
    public override int PicID { get; set; } = (int) AllCards.SpecialPicIDs.LifeUpperLimit;

    public Bonus_LifeUpperLimit(int lifeUpperLimit) : base(BonusTypes.LifeUpperLimit)
    {
        LifeUpperLimit = lifeUpperLimit;
    }

    public override string GetDesc()
    {
        Dictionary<BonusTypes, string> dic = BonusDescRaw[LanguageManager_Common.GetCurrentLanguage()];
        return string.Format(dic[BonusType], LifeUpperLimit);
    }

    protected override void ChildrenExportToXML(XmlElement my_ele)
    {
        base.ChildrenExportToXML(my_ele);
        my_ele.SetAttribute("lifeUpperLimit", LifeUpperLimit.ToString());
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(LifeUpperLimit);
    }

    public override Bonus Clone()
    {
        return new Bonus_LifeUpperLimit(LifeUpperLimit);
    }
}