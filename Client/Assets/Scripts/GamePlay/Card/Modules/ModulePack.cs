public class ModulePack : ModuleEquip
{
    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        M_PackName = cardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()];
    }

    public override void SetPreview()
    {
        base.SetPreview();
    }

    public override void SetNoPreview()
    {
        base.SetNoPreview();
    }

    public CardInfo_Equip GetCurrentCardInfo()
    {
        CardInfo_Equip currentCI = (CardInfo_Equip) CardInfo.Clone();
        return currentCI;
    }

    private string m_PackName;

    public string M_PackName
    {
        get { return m_PackName; }

        set
        {
            m_PackName = value;
            Name.text = LanguageManager.Instance.IsEnglish ? "" : Utils.TextToVertical(value);
            Name_en.text = LanguageManager.Instance.IsEnglish ? value : "";
        }
    }

    public void OnPackEquiped()
    {
        EquipAnim.SetTrigger("PackEquiped");
    }
}