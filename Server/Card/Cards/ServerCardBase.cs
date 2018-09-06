using System.Security.Policy;

internal abstract class ServerCardBase
{
    internal ServerPlayer ServerPlayer;
    internal CardInfo_Base CardInfo; //卡牌原始数值信息

    #region 属性

    public bool isInitialized = false;

    private int m_Metal;

    public int M_Metal
    {
        get { return m_Metal; }
        set
        {
            int before = m_Energy;
            m_Metal = value;
            if (isInitialized)
            {
                CardAttributeChangeRequest request = new CardAttributeChangeRequest(ServerPlayer.ClientId, M_CardInstanceId, m_Metal - before, 0, m_EffectFactor);
                ServerPlayer.MyGameManager.Broadcast_AddRequestToOperationResponse(request);
            }
        }
    }

    private int m_Energy;

    public int M_Energy
    {
        get { return m_Energy; }
        set
        {
            int before = m_Energy;
            m_Energy = value;
            if (isInitialized)
            {
                CardAttributeChangeRequest request = new CardAttributeChangeRequest(ServerPlayer.ClientId, M_CardInstanceId, 0, m_Energy - before, m_EffectFactor);
                ServerPlayer.MyGameManager.Broadcast_AddRequestToOperationResponse(request);
            }
        }
    }

    private int m_EffectFactor;

    public int M_EffectFactor
    {
        get { return m_EffectFactor; }
        set
        {
            if (value != m_EffectFactor)
            {
                m_EffectFactor = value;
                if (isInitialized)
                {
                    CardAttributeChangeRequest request = new CardAttributeChangeRequest(ServerPlayer.ClientId, M_CardInstanceId, 0, 0, m_EffectFactor);
                    ServerPlayer.MyGameManager.Broadcast_AddRequestToOperationResponse(request);
                }
            }
        }
    }

    private int m_CardInstanceId;

    public int M_CardInstanceId
    {
        get { return m_CardInstanceId; }
        set { m_CardInstanceId = value; }
    }

    private bool usable;

    internal bool Usable
    {
        get { return usable; }

        set { usable = value; }
    }

    protected int stars;

    public int Stars
    {
        get { return stars; }

        set { stars = value; }
    }

    #endregion

    public static ServerCardBase InstantiateCardByCardInfo(CardInfo_Base cardInfo, ServerPlayer serverPlayer, int cardInstanceId)
    {
        ServerCardBase newCard;
        switch (cardInfo.BaseInfo.CardType)
        {
            case CardTypes.Retinue:
                newCard = new ServerCardRetinue();
                break;
            case CardTypes.Equip:
                newCard = new ServerCardEquip();
                break;
            case CardTypes.Spell:
                newCard = new ServerCardSpell();
                break;
            default:
                newCard = new ServerCardRetinue();
                break;
        }

        newCard.Initiate(cardInfo, serverPlayer, cardInstanceId);
        return newCard;
    }

    public virtual void Initiate(CardInfo_Base cardInfo, ServerPlayer serverPlayer, int cardInstanceId)
    {
        isInitialized = false;
        ServerPlayer = serverPlayer;
        CardInfo = cardInfo.Clone();
        M_Metal = CardInfo.BaseInfo.Metal;
        M_Energy = CardInfo.BaseInfo.Energy;
        M_EffectFactor = CardInfo.BaseInfo.EffectFactor;
        M_CardInstanceId = cardInstanceId;
        Stars = CardInfo.UpgradeInfo.CardLevel;
        isInitialized = true;
        foreach (SideEffectBundle.SideEffectExecute see in CardInfo.SideEffects.GetSideEffects())
        {
            see.SideEffectBase.Player = ServerPlayer;
            if (see.SideEffectBase is CardRelatedSideEffect)
            {
                ((CardRelatedSideEffect) see.SideEffectBase).TargetCardInstanceId = M_CardInstanceId;
            }
        }

        EventManager.Instance.RegisterEvent(CardInfo.SideEffects);
    }
}