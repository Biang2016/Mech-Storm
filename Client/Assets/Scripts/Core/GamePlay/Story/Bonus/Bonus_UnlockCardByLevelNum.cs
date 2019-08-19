using System.Collections.Generic;
using System.Xml;

public class Bonus_UnlockCardByLevelNum : Bonus
{
    public int LevelNum;

    public override int PicID { get; set; } = (int) AllCards.SpecialPicIDs.LevelCards;

    public Bonus_UnlockCardByLevelNum(int levelNum) : base(BonusTypes.UnlockCardByLevelNum)
    {
        LevelNum = levelNum;
    }

    public override string GetDesc()
    {
        Dictionary<BonusTypes, string> dic = BonusDescRaw[LanguageManager_Common.GetCurrentLanguage()];
        return Utils.HighlightStringFormat(dic[BonusType], AllColors.ColorDict[AllColors.ColorType.CardHighLightColor], LevelNum);
    }

    protected override void ChildrenExportToXML(XmlElement my_ele)
    {
        base.ChildrenExportToXML(my_ele);
        my_ele.SetAttribute("levelNum", LevelNum.ToString());
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(LevelNum);
    }

    public override Bonus Clone()
    {
        return new Bonus_UnlockCardByLevelNum(LevelNum);
    }
}