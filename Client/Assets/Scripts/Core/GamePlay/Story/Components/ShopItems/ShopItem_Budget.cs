using System.Xml;

public class ShopItem_Budget : ShopItem
{
    public ShopItem_Budget(int price, int budget) : base(ShopItemTypes.Budget, price)
    {
        Budget = budget;
    }

    public int Budget;

    public override string Name => LanguageManager_Common.GetText("CardEditorPanel_CardCoinCostLabelText");

    public override int PicID => (int) AllCards.SpecialPicIDs.Budget;

    public override ShopItem Clone()
    {
        return new ShopItem_Budget(Price, Budget);
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
}