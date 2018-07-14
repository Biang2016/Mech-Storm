using System.Collections;
using System;
using System.Collections.Generic;

internal abstract class ServerModuleBase
{
    internal ServerPlayer ServerPlayer;
    internal CardInfo_Base CardInfo; //卡牌原始数值信息
    protected bool isInitialized = false;

    public virtual void Initiate(CardInfo_Base cardInfo, ServerPlayer serverPlayer)
    {
        ServerPlayer = serverPlayer;
        CardInfo = cardInfo;
        Stars = cardInfo.CardLevel;
        isInitialized = true;
    }

    public abstract CardInfo_Base GetCurrentCardInfo();

    #region 属性

    protected int stars;

    public virtual int Stars
    {
        get { return stars; }

        set { stars = value; }
    }

    private int m_RetinuePlaceIndex;

    public int M_RetinuePlaceIndex
    {
        get { return m_RetinuePlaceIndex; }
        set { m_RetinuePlaceIndex = value; }
    }

    #endregion

    #region 各模块

    #endregion
}