using System.Xml;

public class ShopItem_LifeUpperLimit : ShopItem
{
    public ShopItem_LifeUpperLimit(int price, int lifeUpperLimit, int probability, bool isSingleton) : base(ShopItemTypes.LifeUpperLimit, price, probability, isSingleton)
    {
        LifeUpperLimit = lifeUpperLimit;
    }

    public int LifeUpperLimit;

    public override string Name => LanguageManager_Common.GetText("LevelEditorPanel_LifeUpperLimitLabelValueText");

    public override int PicID => (int) AllCards.SpecialPicIDs.LifeUpperLimit;

    public override ShopItem Clone()
    {
        return new ShopItem_LifeUpperLimit(Price, LifeUpperLimit, Probability, IsSingleton);
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
}