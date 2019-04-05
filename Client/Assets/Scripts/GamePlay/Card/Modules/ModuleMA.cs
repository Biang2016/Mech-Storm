public class ModuleMA : ModuleEquip
{
    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.PoolDict["ModuleMA"];
    }

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        M_MAName = cardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()];
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

    private string m_MAName;

    public string M_MAName
    {
        get { return m_MAName; }

        set
        {
            m_MAName = value;
            Name.text = LanguageManager.Instance.IsEnglish ? "" : value;
            Name_en.text = LanguageManager.Instance.IsEnglish ? value : "";
        }
    }

    public void OnMAEquiped()
    {
        EquipAnim.SetTrigger("MAEquiped");
    }
}