using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal abstract class ServerCardBase
{
    internal ServerPlayer ServerPlayer;
    internal CardInfo_Base CardInfo; //卡牌原始数值信息

    #region 属性

    private int m_Cost;

    public int M_Cost
    {
        get { return m_Cost; }
        set { m_Cost = value; }
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
        ServerPlayer = serverPlayer;
        CardInfo = cardInfo;
        M_Cost = CardInfo.BaseInfo.Cost;
        Stars = cardInfo.UpgradeInfo.CardLevel;
    }

    public virtual void OnBeginRound()
    {
    }

    public virtual void OnEndRound()
    {
    }
}