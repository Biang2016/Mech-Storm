using System.Security.Policy;

internal abstract class ServerCardBase
{
    internal ServerPlayer ServerPlayer;
    internal CardInfo_Base CardInfo; //卡牌原始数值信息

    #region 属性

    public bool isInitialized = false;

    private int m_Cost;

    public int M_Cost
    {
        get { return m_Cost; }
        set
        {
            int before = m_Magic;
            m_Cost = value;
            if (isInitialized)
            {
                CardAttributeChangeRequest request = new CardAttributeChangeRequest(ServerPlayer.ClientId, M_CardInstanceId, m_Cost - before, 0, m_EffectFactor);
                ServerPlayer.MyGameManager.Broadcast_AddRequestToOperationResponse(request);
            }
        }
    }

    private int m_Magic;

    public int M_Magic
    {
        get { return m_Magic; }
        set
        {
            int before = m_Magic;
            m_Magic = value;
            if (isInitialized)
            {
                CardAttributeChangeRequest request = new CardAttributeChangeRequest(ServerPlayer.ClientId, M_CardInstanceId, 0, m_Magic - before, m_EffectFactor);
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

    public static ServerCardBase InstantiateCardByCardInfo(CardInfo_Base cardInfo, ServerPlayer serverPlayer)
    {
        ServerCardBase newCard;
        switch (cardInfo.BaseInfo.CardType)
        {
            case CardTypes.Retinue:
                newCard = new ServerCardRetinue();
                break;
            case CardTypes.Weapon:
                newCard = new ServerCardWeapon();
                break;
            case CardTypes.Shield:
                newCard = new ServerCardShield();
                break;
            case CardTypes.Spell:
                newCard = new ServerCardSpell();
                break;
            default:
                newCard = new ServerCardRetinue();
                break;
        }

        newCard.Initiate(cardInfo, serverPlayer);
        return newCard;
    }

    public virtual void Initiate(CardInfo_Base cardInfo, ServerPlayer serverPlayer)
    {
        isInitialized = false;
        ServerPlayer = serverPlayer;
        CardInfo = cardInfo.Clone();
        M_Cost = CardInfo.BaseInfo.Cost;
        M_Magic = CardInfo.BaseInfo.Magic;
        M_EffectFactor = CardInfo.BaseInfo.EffectFactor;
        Stars = CardInfo.UpgradeInfo.CardLevel;
        isInitialized = true;
    }

    public virtual void OnBeginRound()
    {
    }

    public virtual void OnEndRound()
    {
        foreach (SideEffectBase se in CardInfo.SideEffects_OnEndRound)
        {
            if (se is CardRelatedSideEffect)
            {
                ((CardRelatedSideEffect) se).TargetCardInstanceId = M_CardInstanceId;
            }

            se.Excute(ServerPlayer);
        }
    }

    public virtual void OnPlayOut(int targetRetinueId)
    {
        foreach (SideEffectBase se in CardInfo.SideEffects_OnPlayOut)
        {
            if (se is CardRelatedSideEffect)
            {
                ((CardRelatedSideEffect) se).TargetCardInstanceId = M_CardInstanceId;
            }
            if (se is TargetSideEffect)
            {
                ((TargetSideEffect) se).TargetRetinueId = targetRetinueId;
            }

            se.Excute(ServerPlayer);
        }
    }
}