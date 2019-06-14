using System.Xml;

public class ShopItem_LifeUpperLimit : ShopItem
{
    public ShopItem_LifeUpperLimit(int price, int lifeUpperLimit) : base(ShopItemTypes.LifeUpperLimit, price)
    {
        LifeUpperLimit = lifeUpperLimit;
    }

    public int LifeUpperLimit;

    public override string Name => LanguageManager_Common.GetText("LevelEditorPanel_LifeUpperLimitLabelValueText");

    public override int PicID => 1009;

    public override ShopItem Clone()
    {
        return new ShopItem_LifeUpperLimit(Price, LifeUpperLimit);
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