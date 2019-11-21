using System.Xml;

public class ShopItem_Card : ShopItem
{
    public ShopItem_Card(int price, int cardRareLevel, int probability, bool isSingleton) : base(ShopItemTypes.Card, price, probability, isSingleton)
    {
        CardRareLevel = cardRareLevel;
    }

    public int CardRareLevel;
    public int GenerateCardID; // Temp used by client.

    public override string Name
    {
        get { return string.Format(LanguageManager_Common.GetText("ShopPanel_CardRareLevelDesc"), CardRareLevel); }
    }

    public override int PicID
    {
        get { return (int) AllCards.SpecialPicIDs.LevelCards; }
    }

    protected override void OnEdit()
    {
        base.OnEdit();
    }

    public override ShopItem Clone()
    {
        return new ShopItem_Card(Price, CardRareLevel, Probability, IsSingleton);
    }

    protected override void ChildrenExportToXML(XmlElement my_ele)
    {
        base.ChildrenExportToXML(my_ele);
        my_ele.SetAttribute("cardRareLevel", CardRareLevel.ToString());
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(CardRareLevel);
        writer.WriteSInt32(GenerateCardID);
    }
}