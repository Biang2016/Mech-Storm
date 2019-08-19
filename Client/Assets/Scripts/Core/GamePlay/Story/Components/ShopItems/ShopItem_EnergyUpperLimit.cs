using System.Xml;

public class ShopItem_EnergyUpperLimit : ShopItem
{
    public ShopItem_EnergyUpperLimit(int price, int energyUpperLimit, int probability, bool isSingleton) : base(ShopItemTypes.EnergyUpperLimit, price, probability, isSingleton)
    {
        EnergyUpperLimit = energyUpperLimit;
    }

    public int EnergyUpperLimit;

    public override string Name => LanguageManager_Common.GetText("LevelEditorPanel_EnergyUpperLimitLabelValueText");

    public override int PicID => (int) AllCards.SpecialPicIDs.EnergyUpperLimit;

    public override ShopItem Clone()
    {
        return new ShopItem_EnergyUpperLimit(Price, EnergyUpperLimit, Probability, IsSingleton);
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
}