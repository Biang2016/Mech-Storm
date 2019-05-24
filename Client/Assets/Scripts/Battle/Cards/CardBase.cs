internal abstract class CardBase
{
    internal BattlePlayer ServerPlayer;
    internal CardInfo_Base CardInfo; //卡牌原始数值信息

    public static CardBase InstantiateCardByCardInfo(CardInfo_Base cardInfo, BattlePlayer serverPlayer, int cardInstanceId)
    {
        CardBase newCard;
        switch (cardInfo.BaseInfo.CardType)
        {
            case CardTypes.Mech:
                newCard = new CardMech();
                break;
            case CardTypes.Equip:
                newCard = new CardEquip();
                break;
            case CardTypes.Spell:
                newCard = new CardSpell();
                break;
            case CardTypes.Energy:
                newCard = new CardSpell();
                break;
            default:
                newCard = new CardMech();
                break;
        }

        newCard.Initiate(cardInfo, serverPlayer, cardInstanceId);
        return newCard;
    }

    public virtual void Initiate(CardInfo_Base cardInfo, BattlePlayer serverPlayer, int cardInstanceId)
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
        foreach (SideEffectExecute see in CardInfo.SideEffectBundle.SideEffectExecutes)
        {
            foreach (SideEffectBase se in see.SideEffectBases)
            {
                se.Player = ServerPlayer;
                if (se is HandCardRelatedSideEffect)
                {
                    ((HandCardRelatedSideEffect) se).TargetCardInstanceId = M_CardInstanceId;
                }

                se.M_ExecutorInfo = new ExecutorInfo(clientId: ServerPlayer.ClientId, sideEffectExecutorID: see.ID, cardId: CardInfo.CardID, cardInstanceId: M_CardInstanceId);
            }
        }

        ServerPlayer.GameManager.EventManager.RegisterEvent(CardInfo.SideEffectBundle);
    }

    public void UnRegisterSideEffect()
    {
        ServerPlayer.GameManager.EventManager.UnRegisterEvent(CardInfo.SideEffectBundle);
    }

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
                ServerPlayer.GameManager.Broadcast_AddRequestToOperationResponse(request);
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
                ServerPlayer.GameManager.Broadcast_AddRequestToOperationResponse(request);
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
                    ServerPlayer.GameManager.Broadcast_AddRequestToOperationResponse(request);
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

        set
        {
            if (usable && !value)
            {
                ServerPlayer.HandManager.UsableCards.Remove(M_CardInstanceId);
            }

            if (!usable && value)
            {
                ServerPlayer.HandManager.UsableCards.Add(M_CardInstanceId);
            }

            usable = value;
        }
    }

    protected int stars;

    public int Stars
    {
        get { return stars; }

        set { stars = value; }
    }

    #endregion
}