using System.Collections.Generic;
using UnityEngine;

public class CardMech : CardBase
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
        M_MechTotalLife = CardInfo.LifeInfo.TotalLife;
        M_MechAttack = CardInfo.BattleInfo.BasicAttack;
        M_MechArmor = CardInfo.BattleInfo.BasicArmor;
        M_MechShield = CardInfo.BattleInfo.BasicShield;

        InitSlots();
        if (M_CardShowMode == CardShowMode.CardSelect) ResetSelectCountBlockPosition();
        if (M_CardShowMode == CardShowMode.SelectedCardPreview || M_CardShowMode == CardShowMode.CardUpgradePreview) RefreshSelectCountBlockPosition();
    }

    #region 属性

    private int m_MechTotalLife;

    public int M_MechTotalLife
    {
        get { return m_MechTotalLife; }
        set
        {
            m_MechTotalLife = value;
            SetLifeText(value);
        }
    }

    private int m_MechAttack;

    public int M_MechAttack
    {
        get { return m_MechAttack; }
        set { m_MechAttack = value; }
    }

    private int m_MechArmor;

    public int M_MechArmor
    {
        get { return m_MechArmor; }
        set { m_MechArmor = value; }
    }

    private int m_MechShield;

    public int M_MechShield
    {
        get { return m_MechShield; }
        set { m_MechShield = value; }
    }

    #endregion

    #region 拼装上的模块

    internal ModuleWeapon Weapon;
    internal ModuleShield Shield;
    internal ModulePack Pack;
    internal ModuleMA MA;

    [SerializeField] private Transform[] EquipPivots;

    public void SetEquipInPlace()
    {
        if (Weapon) Weapon.transform.position = EquipPivots[0].position;
        if (Shield) Shield.transform.position = EquipPivots[1].position;
        if (Pack) Pack.transform.position = EquipPivots[2].position;
        if (MA) MA.transform.position = EquipPivots[3].position;
    }

    public void ShowEquipCardBloom()
    {
        CardSlotsComponent.ShowSlot(SlotTypes.Weapon, !Weapon);
        CardSlotsComponent.ShowSlot(SlotTypes.Shield, !Shield);
        CardSlotsComponent.ShowSlot(SlotTypes.Pack, !Pack);
        CardSlotsComponent.ShowSlot(SlotTypes.MA, !MA);
    }

    # endregion

    public override void DragComponent_OnMousePressed(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleMech moduleMech, Vector3 dragLastPosition)
    {
        base.DragComponent_OnMousePressed(boardAreaType, slots, moduleMech, dragLastPosition);

        if (boardAreaType == ClientPlayer.BattlePlayer.BattleGroundArea && !ClientPlayer.BattlePlayer.BattleGroundManager.BattleGroundIsFull) //拖机甲牌到战场区域
        {
            int previewPosition = ClientPlayer.BattlePlayer.BattleGroundManager.ComputePosition(dragLastPosition);
            ClientPlayer.BattlePlayer.BattleGroundManager.AddMechPreview(previewPosition);
        }
        else //离开战场区域
        {
            ClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechPreview();
        }
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleMech moduleMech, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slots, moduleMech, ship, dragLastPosition, dragBeginPosition, dragBeginQuaternion);
        if (!CardInfo.TargetInfo.HasTargetMech) //无指定目标的副作用
        {
            if (boardAreaType != ClientPlayer.BattlePlayer.HandArea) //脱手即出牌
            {
                summonMech(dragLastPosition, Const.TARGET_MECH_SELECT_NONE);
            }
            else
            {
                ClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechPreview();
                transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
                ClientPlayer.BattlePlayer.HandManager.RefreshCardsPlace();
            }
        }
        else
        {
            if (boardAreaType != ClientPlayer.BattlePlayer.HandArea) //脱手即假召唤，开始展示指定目标箭头
            {
                summonMechTarget(dragLastPosition, CardInfo.TargetInfo.targetMechRange);
            }
            else
            {
                ClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechPreview();
                transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
                ClientPlayer.BattlePlayer.HandManager.RefreshCardsPlace();
            }
        }
    }

    #region 卡牌效果

    //召唤机甲
    private void summonMech(Vector3 dragLastPosition, int targetMechId)
    {
        if (ClientPlayer.BattlePlayer.BattleGroundManager.BattleGroundIsFull)
        {
            ClientPlayer.BattlePlayer.HandManager.RefreshCardsPlace();
            return;
        }

        int aliveIndex = ClientPlayer.BattlePlayer.BattleGroundManager.ComputePositionInAliveMechs(dragLastPosition);
        SummonMechRequest request = new SummonMechRequest(Client.Instance.Proxy.ClientID, M_CardInstanceId, aliveIndex, targetMechId, false, Const.CLIENT_TEMP_MECH_ID_NORMAL);
        Client.Instance.Proxy.SendMessage(request);
        Usable = false;
    }

    //召唤机甲带目标
    private void summonMechTarget(Vector3 dragLastPosition, TargetRange targetRange)
    {
        if (ClientPlayer.BattlePlayer.BattleGroundManager.BattleGroundIsFull)
        {
            ClientPlayer.BattlePlayer.HandManager.RefreshCardsPlace();
            return;
        }

        ClientPlayer.BattlePlayer.HandManager.SetCurrentSummonMechPreviewCard(this);

        if (ClientPlayer.BattlePlayer.BattleGroundManager.BattleGroundIsEmpty) //场上为空直接召唤
        {
            summonMech(dragLastPosition, Const.TARGET_MECH_SELECT_NONE);
        }
        else //预览
        {
            int battleGroundIndex = ClientPlayer.BattlePlayer.BattleGroundManager.ComputePosition(dragLastPosition);
            ClientPlayer.BattlePlayer.BattleGroundManager.SummonMechPreview(this, battleGroundIndex, targetRange);
        }
    }

    #endregion
}