public class ModulePack : ModuleEquip
{
    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.Pool_ModulePackPool;
    }

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        M_PackName = GameManager.Instance.isEnglish ? cardInfo.BaseInfo.CardName_en : cardInfo.BaseInfo.CardName;
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
            Name.text = GameManager.Instance.isEnglish ? "" : Utils.TextToVertical(value);
            Name_en.text = GameManager.Instance.isEnglish ? value : "";
        }
    }


    public void OnPackEquiped()
    {
        EquipAnim.SetTrigger("PackEquiped");
    }
}