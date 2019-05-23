public class ModulePack : ModuleEquip
{
    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
    }

    #region Preview Details

    public override void SetPreview()
    {
        base.SetPreview();
    }

    public override void SetNoPreview()
    {
        base.SetNoPreview();
    }

    #endregion

    #region 属性

    public override CardInfo_Equip GetCurrentCardInfo()
    {
        CardInfo_Equip currentCI = (CardInfo_Equip) CardInfo.Clone();
        return currentCI;
    }

    #endregion

    public void OnPackEquipped()
    {
        EquipAnim.SetTrigger("PackEquipped");
    }
}