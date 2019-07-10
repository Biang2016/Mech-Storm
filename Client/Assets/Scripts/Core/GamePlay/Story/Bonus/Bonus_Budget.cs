using System.Collections.Generic;
using System.Xml;

public class Bonus_Budget : Bonus
{
    public int Budget;
    public override int PicID { get; set; } = (int) AllCards.SpecialPicIDs.Budget;

    public Bonus_Budget(int budget) : base(BonusTypes.Budget)
    {
        Budget = budget;
    }

    public override string GetDesc()
    {
        Dictionary<BonusTypes, string> dic = BonusDescRaw[LanguageManager_Common.GetCurrentLanguage()];
        return Utils.HighlightStringFormat(dic[BonusType], AllColors.ColorDict[AllColors.ColorType.CardHighLightColor], Budget);
    }

    protected override void ChildrenExportToXML(XmlElement my_ele)
    {
        base.ChildrenExportToXML(my_ele);
        my_ele.SetAttribute("budget", Budget.ToString());
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(Budget);
    }

    public override Bonus Clone()
    {
        return new Bonus_Budget(Budget);
    }
}