using System.Collections.Generic;
using UnityEngine;

public class CardSpell : CardBase
{
    #region 卡牌上各模块

    internal bool HasTargetRetinue;
    internal bool HasTargetEquip;
    internal bool HasTargetShip;
    internal TargetSideEffect.TargetRange targetRetinueRange = TargetSideEffect.TargetRange.None;
    internal TargetSideEffect.TargetRange targetEquipRange = TargetSideEffect.TargetRange.None;
    internal TargetSideEffect.TargetRange targetShipRange = TargetSideEffect.TargetRange.None;

    # endregion

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer, bool isCardSelect)
    {
        base.Initiate(cardInfo, clientPlayer, isCardSelect);
        HasTargetRetinue = false;
        HasTargetEquip = false;
        HasTargetShip = false;

        foreach (SideEffectExecute see in CardInfo.SideEffectBundle.GetSideEffectExecutes(SideEffectBundle.TriggerTime.OnPlayCard, SideEffectBundle.TriggerRange.Self))
        {
            SideEffectBase se = see.SideEffectBase;
            if (se is TargetSideEffect && ((TargetSideEffect) se).IsNeedChoise)
            {
                if (se is TargetSideEffectEquip && ((TargetSideEffectEquip) se).IsNeedChoise)
                {
                    HasTargetEquip = true;
                    targetEquipRange = ((TargetSideEffectEquip) se).M_TargetRange;
                    break;
                }
                else
                {
                    TargetSideEffect.TargetRange temp = ((TargetSideEffect) se).M_TargetRange;
                    if (temp != TargetSideEffect.TargetRange.Ships && temp != TargetSideEffect.TargetRange.SelfShip && temp != TargetSideEffect.TargetRange.EnemyShip && temp != TargetSideEffect.TargetRange.AllLife)
                    {
                        HasTargetRetinue = true;
                        targetRetinueRange = ((TargetSideEffect) se).M_TargetRange;
                        break;
                    }
                    else
                    {
                        HasTargetShip = true;
                        targetShipRange = ((TargetSideEffect) se).M_TargetRange;
                        break;
                    }
                }
            }
        }
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slots, moduleRetinue, ship, dragLastPosition, dragBeginPosition, dragBeginQuaternion);
        RoundManager.Instance.HideTargetPreviewArrow();
        if (boardAreaType != ClientPlayer.MyHandArea) //离开手牌区域
        {
            if (!HasTargetRetinue && !HasTargetEquip && !HasTargetShip)
            {
                summonSpellRequest(dragLastPosition);
                return;
            }
            else if (HasTargetRetinue)
            {
                //To Retinue
                if (moduleRetinue == null || moduleRetinue.IsDead)
                {
                    transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //带目标法术卡未指定目标，则收回
                    ClientPlayer.MyHandManager.RefreshCardsPlace();
                }
                else
                {
                    bool validTarget = false;
                    switch (targetRetinueRange)
                    {
                        case TargetSideEffect.TargetRange.None:
                            validTarget = false;
                            break;
                        case TargetSideEffect.TargetRange.Mechs:
                            validTarget = true;
                            break;
                        case TargetSideEffect.TargetRange.SelfMechs:
                            if (moduleRetinue.ClientPlayer == RoundManager.Instance.SelfClientPlayer) validTarget = true;
                            break;
                        case TargetSideEffect.TargetRange.EnemyMechs:
                            if (moduleRetinue.ClientPlayer == RoundManager.Instance.EnemyClientPlayer) validTarget = true;
                            break;
                        case TargetSideEffect.TargetRange.Heros:
                            if (!moduleRetinue.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                            break;
                        case TargetSideEffect.TargetRange.SelfHeros:
                            if (moduleRetinue.ClientPlayer == RoundManager.Instance.SelfClientPlayer && !moduleRetinue.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                            break;
                        case TargetSideEffect.TargetRange.EnemyHeros:
                            if (moduleRetinue.ClientPlayer == RoundManager.Instance.EnemyClientPlayer && !moduleRetinue.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                            break;
                        case TargetSideEffect.TargetRange.Soldiers:
                            if (moduleRetinue.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                            break;
                        case TargetSideEffect.TargetRange.SelfSoldiers:
                            if (moduleRetinue.ClientPlayer == RoundManager.Instance.SelfClientPlayer && moduleRetinue.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                            break;
                        case TargetSideEffect.TargetRange.EnemySoldiers:
                            if (moduleRetinue.ClientPlayer == RoundManager.Instance.EnemyClientPlayer && moduleRetinue.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                            break;
                        case TargetSideEffect.TargetRange.AllLife:
                            validTarget = true;
                            break;
                    }

                    if (validTarget)
                    {
                        summonSpellRequestToRetinue(moduleRetinue, dragLastPosition);
                        return;
                    }
                    else
                    {
                        AudioManager.Instance.SoundPlay("sfx/OnSelectRetinueFalse");
                        NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "You should select a " + targetRetinueRange : "请选择正确的机甲", 0, 1f);
                    }
                }
            }
            else if (HasTargetEquip)
            {
                //To Equip
                if (slots.Count == 0)
                {
                    transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //带目标法术卡未指定目标，则收回
                    ClientPlayer.MyHandManager.RefreshCardsPlace();
                }
                else
                {
                    ModuleEquip equip = slots[0].M_ModuleRetinue.GetEquipBySlotType(slots[0].MSlotTypes);
                    if (equip == null || equip.IsDead)
                    {
                        transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //带目标法术卡未指定目标，则收回
                        ClientPlayer.MyHandManager.RefreshCardsPlace();
                    }
                    else
                    {
                        bool validTarget = false;
                        switch (targetEquipRange)
                        {
                            case TargetSideEffect.TargetRange.None:
                                validTarget = false;
                                break;
                            case TargetSideEffect.TargetRange.Mechs:
                                validTarget = true;
                                break;
                            case TargetSideEffect.TargetRange.SelfMechs:
                                if (equip.ClientPlayer == RoundManager.Instance.SelfClientPlayer) validTarget = true;
                                break;
                            case TargetSideEffect.TargetRange.EnemyMechs:
                                if (equip.ClientPlayer == RoundManager.Instance.EnemyClientPlayer) validTarget = true;
                                break;
                            case TargetSideEffect.TargetRange.Heros:
                                if (!equip.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                                break;
                            case TargetSideEffect.TargetRange.SelfHeros:
                                if (equip.ClientPlayer == RoundManager.Instance.SelfClientPlayer && !equip.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                                break;
                            case TargetSideEffect.TargetRange.EnemyHeros:
                                if (equip.ClientPlayer == RoundManager.Instance.EnemyClientPlayer && !equip.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                                break;
                            case TargetSideEffect.TargetRange.Soldiers:
                                if (equip.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                                break;
                            case TargetSideEffect.TargetRange.SelfSoldiers:
                                if (equip.ClientPlayer == RoundManager.Instance.SelfClientPlayer && equip.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                                break;
                            case TargetSideEffect.TargetRange.EnemySoldiers:
                                if (equip.ClientPlayer == RoundManager.Instance.EnemyClientPlayer && equip.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                                break;
                            case TargetSideEffect.TargetRange.AllLife:
                                validTarget = true;
                                break;
                        }

                        if (validTarget)
                        {
                            summonSpellRequestToEquip(equip, dragLastPosition);
                            return;
                        }
                        else
                        {
                            AudioManager.Instance.SoundPlay("sfx/OnSelectRetinueFalse");
                            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "You should select a right equip." : "请选择正确的装备", 0, 1f);
                        }
                    }
                }
            }
            else if (HasTargetShip)
            {
                // ToShip
                if (!ship)
                {
                    transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //带目标法术卡未指定目标，则收回
                    ClientPlayer.MyHandManager.RefreshCardsPlace();
                }
                else
                {
                    bool validTarget = false;
                    switch (targetShipRange)
                    {
                        case TargetSideEffect.TargetRange.Ships:
                            validTarget = true;
                            break;
                        case TargetSideEffect.TargetRange.SelfShip:
                            if (ship.ClientPlayer == RoundManager.Instance.SelfClientPlayer) validTarget = true;
                            break;
                        case TargetSideEffect.TargetRange.EnemyShip:
                            if (ship.ClientPlayer == RoundManager.Instance.EnemyClientPlayer) validTarget = true;
                            break;
                        case TargetSideEffect.TargetRange.AllLife:
                            validTarget = true;
                            break;
                    }

                    if (validTarget)
                    {
                        summonSpellRequestToShip(ship, dragLastPosition);
                        return;
                    }
                    else
                    {
                        AudioManager.Instance.SoundPlay("sfx/OnSelectRetinueFalse");
                        NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "You should select a right ship." : "请选择正确的战舰", 0, 1f);
                    }
                }
            }

            DragManager.Instance.DragOutDamage = 0;
        }

        transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
        ClientPlayer.MyHandManager.RefreshCardsPlace();
    }

    public override void DragComponnet_DragOutEffects()
    {
        base.DragComponnet_DragOutEffects();
        DragManager.Instance.DragOutDamage = CalculateAttack();
        RoundManager.Instance.ShowTargetPreviewArrow(targetRetinueRange);
    }

    private int CalculateAttack()
    {
        foreach (SideEffectExecute see in CardInfo.SideEffectBundle.GetSideEffectExecutes(SideEffectBundle.TriggerTime.OnPlayCard, SideEffectBundle.TriggerRange.Self))
        {
            SideEffectBase se = see.SideEffectBase;
            if (se is TargetSideEffect && ((TargetSideEffect) se).IsNeedChoise)
            {
                if (se is IDamage) return ((IDamage) se).CalculateDamage();
            }
        }

        return 0;
    }

    public override float DragComponnet_DragDistance()
    {
        return GameManager.Instance.PullOutCardDistanceThreshold;
    }

    #region 卡牌效果

    private void summonSpellRequest(Vector3 dragLastPosition)
    {
        UseSpellCardRequest request = new UseSpellCardRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z));
        Client.Instance.Proxy.SendMessage(request);
        Usable = false;
    }

    private void summonSpellRequestToRetinue(ModuleRetinue targetModuleRetinue, Vector3 dragLastPosition)
    {
        if (targetModuleRetinue.M_ClientTempRetinueID != Const.CLIENT_TEMP_RETINUE_ID_NORMAL)
        {
            UseSpellCardToRetinueRequest request = new UseSpellCardToRetinueRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z), targetModuleRetinue.M_RetinueID, true, targetModuleRetinue.M_ClientTempRetinueID);
            Client.Instance.Proxy.SendMessage(request);
        }
        else
        {
            UseSpellCardToRetinueRequest request = new UseSpellCardToRetinueRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z), targetModuleRetinue.M_RetinueID, false, Const.CLIENT_TEMP_RETINUE_ID_NORMAL);
            Client.Instance.Proxy.SendMessage(request);
        }

        Usable = false;
    }

    private void summonSpellRequestToEquip(ModuleEquip targetEquip, Vector3 dragLastPosition)
    {
        UseSpellCardToEquipRequest request = new UseSpellCardToEquipRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z), targetEquip.M_EquipID);
        Client.Instance.Proxy.SendMessage(request);

        Usable = false;
    }

    private void summonSpellRequestToShip(Ship targetShip, Vector3 dragLastPosition)
    {
        UseSpellCardToShipRequest request = new UseSpellCardToShipRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z), targetShip.ClientPlayer.ClientId);
        Client.Instance.Proxy.SendMessage(request);
        Usable = false;
    }

    #endregion
}