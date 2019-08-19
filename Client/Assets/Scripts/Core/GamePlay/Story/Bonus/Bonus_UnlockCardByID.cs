using System.Collections.Generic;
using System.Xml;

public class Bonus_UnlockCardByID : Bonus
{
    public int CardID;
    public override int PicID { get; set; } = (int) AllCards.SpecialPicIDs.Empty;

    public Bonus_UnlockCardByID(int cardID) : base(BonusTypes.UnlockCardByID)
    {
        CardID = cardID;
        PicID = AllCards.GetPicIDByCardID(cardID);
    }

    public override string GetDesc()
    {
        Dictionary<BonusTypes, string> dic = BonusDescRaw[LanguageManager_Common.GetCurrentLanguage()];
        return Utils.HighlightStringFormat(dic[BonusType], AllColors.ColorDict[AllColors.ColorType.CardHighLightColor], AllCards.GetCardNameByCardID(CardID));
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

    public override Bonus Clone()
    {
        return new Bonus_UnlockCardByID(CardID);
    }
}