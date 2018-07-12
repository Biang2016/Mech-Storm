using System.Collections;
using System;
using System.Collections.Generic;

internal abstract class ServerModuleBase
{
    internal ServerPlayer ServerPlayer;
    internal CardInfo_Base CardInfo; //卡牌原始数值信息

    public virtual void Initiate(CardInfo_Base cardInfo, ServerPlayer serverPlayer)
    {
        ServerPlayer = serverPlayer;
        CardInfo = cardInfo;
        Stars = cardInfo.CardLevel;
    }

    public abstract CardInfo_Base GetCurrentCardInfo();

    #region 各模块

    protected int stars;

    public virtual int Stars
    {
        get { return stars; }

        set
        {
            stars = value;
        }
    }

    #endregion
}