using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardRetinue : CardBase
{
    public override void PoolRecycle()
    {
        base.PoolRecycle();
        Weapon?.PoolRecycle();
        Weapon = null;
        Shield?.PoolRecycle();
        Shield = null;
        Pack?.PoolRecycle();
        Pack = null;
        MA?.PoolRecycle();
        MA = null;
    }

    public override void Initiate(CardInfo_Base cardInfo, CardShowMode cardShowMode, ClientPlayer clientPlayer = null)
    {
        base.Initiate(cardInfo, cardShowMode, clientPlayer);
        M_RetinueTotalLife = CardInfo.LifeInfo.TotalLife;
        M_RetinueAttack = CardInfo.BattleInfo.BasicAttack;
        M_RetinueArmor = CardInfo.BattleInfo.BasicArmor;
        M_RetinueShield = CardInfo.BattleInfo.BasicShield;

        InitSlots();
        if (M_CardShowMode == CardShowMode.CardSelect) ResetSelectCountBlockPosition();
        if (M_CardShowMode == CardShowMode.SelectedCardPreview || M_CardShowMode == CardShowMode.CardUpgradePreview) RefreshSelectCountBlockPosition();
    }

    #region 属性

    private int m_RetinueTotalLife;

    public int M_RetinueTotalLife
    {
        get { return m_RetinueTotalLife; }
        set
        {
            m_RetinueTotalLife = value;
            SetLifeText(value);
        }
    }

    private int m_RetinueAttack;

    public int M_RetinueAttack
    {
        get { return m_RetinueAttack; }
        set { m_RetinueAttack = value; }
    }

    private int m_RetinueArmor;

    public int M_RetinueArmor
    {
        get { return m_RetinueArmor; }
        set { m_RetinueArmor = value; }
    }

    private int m_RetinueShield;

    public int M_RetinueShield
    {
        get { return m_RetinueShield; }
        set { m_RetinueShield = value; }
    }

    #endregion

    #region 拼装上的模块

    internal ModuleWeapon Weapon;
    internal ModuleShield Shield;
    internal ModulePack Pack;
    internal ModuleMA MA;

    # endregion

    public override void DragComponent_OnMousePressed(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Vector3 dragLastPosition)
    {
        base.DragComponent_OnMousePressed(boardAreaType, slots, moduleRetinue, dragLastPosition);

        if (boardAreaType == ClientPlayer.MyBattleGroundArea && !ClientPlayer.MyBattleGroundManager.BattleGroundIsFull) //拖机甲牌到战场区域
        {
            int previewPosition = ClientPlayer.MyBattleGroundManager.ComputePosition(dragLastPosition);
            ClientPlayer.MyBattleGroundManager.AddRetinuePreview(previewPosition);
        }
        else //离开战场区域
        {
            ClientPlayer.MyBattleGroundManager.RemoveRetinuePreview();
        }
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slots, moduleRetinue, ship, dragLastPosition, dragBeginPosition, dragBeginQuaternion);
        if (!CardInfo.TargetInfo.HasTargetRetinue) //无指定目标的副作用
        {
            if (boardAreaType != ClientPlayer.MyHandArea) //脱手即出牌
            {
                summonRetinue(dragLastPosition, Const.TARGET_RETINUE_SELECT_NONE);
            }
            else
            {
                ClientPlayer.MyBattleGroundManager.RemoveRetinuePreview();
                transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
                ClientPlayer.MyHandManager.RefreshCardsPlace();
            }
        }
        else
        {
            if (boardAreaType != ClientPlayer.MyHandArea) //脱手即假召唤，开始展示指定目标箭头
            {
                summonRetinueTarget(dragLastPosition, CardInfo.TargetInfo.targetRetinueRange);
            }
            else
            {
                ClientPlayer.MyBattleGroundManager.RemoveRetinuePreview();
                transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
                ClientPlayer.MyHandManager.RefreshCardsPlace();
            }
        }
    }

    #region 卡牌效果

    //召唤机甲
    private void summonRetinue(Vector3 dragLastPosition, int targetRetinueId)
    {
        if (ClientPlayer.MyBattleGroundManager.BattleGroundIsFull)
        {
            ClientPlayer.MyHandManager.RefreshCardsPlace();
            return;
        }

        int aliveIndex = ClientPlayer.MyBattleGroundManager.ComputePositionInAliveRetinues(dragLastPosition);
        SummonRetinueRequest request = new SummonRetinueRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, aliveIndex, targetRetinueId, false, Const.CLIENT_TEMP_RETINUE_ID_NORMAL);
        Client.Instance.Proxy.SendMessage(request);
        Usable = false;
    }

    //召唤机甲带目标
    private void summonRetinueTarget(Vector3 dragLastPosition, TargetSideEffect.TargetRange targetRange)
    {
        if (ClientPlayer.MyBattleGroundManager.BattleGroundIsFull)
        {
            ClientPlayer.MyHandManager.RefreshCardsPlace();
            return;
        }

        ClientPlayer.MyHandManager.SetCurrentSummonRetinuePreviewCard(this);

        if (ClientPlayer.MyBattleGroundManager.BattleGroundIsEmpty) //场上为空直接召唤
        {
            summonRetinue(dragLastPosition, Const.TARGET_RETINUE_SELECT_NONE);
        }
        else //预览
        {
            int battleGroundIndex = ClientPlayer.MyBattleGroundManager.ComputePosition(dragLastPosition);
            ClientPlayer.MyBattleGroundManager.SummonRetinuePreview(this, battleGroundIndex, targetRange);
        }
    }

    #endregion
}