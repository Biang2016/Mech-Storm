using System.Collections.Generic;
using UnityEngine;

public class CardSpell : CardBase
{
    public override void Initiate(CardInfo_Base cardInfo, CardShowMode cardShowMode, ClientPlayer clientPlayer = null)
    {
        base.Initiate(cardInfo, cardShowMode, clientPlayer);
        CardInfo.TargetInfo.HasTargetRetinue = false;
        CardInfo.TargetInfo.HasTargetEquip = false;
        CardInfo.TargetInfo.HasTargetShip = false;

        foreach (SideEffectExecute see in CardInfo.SideEffectBundle.GetSideEffectExecutes(SideEffectExecute.TriggerTime.OnPlayCard, SideEffectExecute.TriggerRange.Self))
        {
            foreach (SideEffectBase se in see.SideEffectBases)
            {
                if (se is TargetSideEffect && ((TargetSideEffect) se).IsNeedChoice)
                {
                    if (se is TargetSideEffectEquip && ((TargetSideEffectEquip) se).IsNeedChoice)
                    {
                        CardInfo.TargetInfo.HasTargetEquip = true;
                        CardInfo.TargetInfo.targetEquipRange = ((TargetSideEffectEquip) se).TargetRange;
                        break;
                    }
                    else
                    {
                        TargetSideEffect.TargetRange temp = ((TargetSideEffect) se).TargetRange;
                        if (temp != TargetSideEffect.TargetRange.Ships && temp != TargetSideEffect.TargetRange.SelfShip && temp != TargetSideEffect.TargetRange.EnemyShip && temp != TargetSideEffect.TargetRange.AllLife)
                        {
                            CardInfo.TargetInfo.HasTargetRetinue = true;
                            CardInfo.TargetInfo.targetRetinueRange = ((TargetSideEffect) se).TargetRange;
                            break;
                        }
                        else
                        {
                            CardInfo.TargetInfo.HasTargetShip = true;
                            CardInfo.TargetInfo.targetShipRange = ((TargetSideEffect) se).TargetRange;
                            break;
                        }
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
            if (!CardInfo.TargetInfo.HasTargetRetinue && !CardInfo.TargetInfo.HasTargetEquip && !CardInfo.TargetInfo.HasTargetShip)
            {
                summonSpellRequest(dragLastPosition);
                return;
            }
            else if (CardInfo.TargetInfo.HasTargetRetinue)
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
                    switch (CardInfo.TargetInfo.targetRetinueRange)
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
                        case TargetSideEffect.TargetRange.Heroes:
                            if (!moduleRetinue.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                            break;
                        case TargetSideEffect.TargetRange.SelfHeroes:
                            if (moduleRetinue.ClientPlayer == RoundManager.Instance.SelfClientPlayer && !moduleRetinue.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                            break;
                        case TargetSideEffect.TargetRange.EnemyHeroes:
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
                        NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardSpell_SelectCorrectMech"), 0, 1f);
                    }
                }
            }
            else if (CardInfo.TargetInfo.HasTargetEquip)
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
                        switch (CardInfo.TargetInfo.targetEquipRange)
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
                            case TargetSideEffect.TargetRange.Heroes:
                                if (!equip.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                                break;
                            case TargetSideEffect.TargetRange.SelfHeroes:
                                if (equip.ClientPlayer == RoundManager.Instance.SelfClientPlayer && !equip.CardInfo.RetinueInfo.IsSoldier) validTarget = true;
                                break;
                            case TargetSideEffect.TargetRange.EnemyHeroes:
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
                            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardSpell_SelectCorrectEquip"), 0, 1f);
                        }
                    }
                }
            }
            else if (CardInfo.TargetInfo.HasTargetShip)
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
                    switch (CardInfo.TargetInfo.targetShipRange)
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
                        NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardSpell_SelectCorrectShip"), 0, 1f);
                    }
                }
            }

            DragManager.Instance.DragOutDamage = 0;
        }

        transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
        ClientPlayer.MyHandManager.RefreshCardsPlace();
    }

    public override void DragComponent_DragOutEffects()
    {
        base.DragComponent_DragOutEffects();
        DragManager.Instance.DragOutDamage = CalculateAttack();
        RoundManager.Instance.ShowTargetPreviewArrow(CardInfo.TargetInfo.targetRetinueRange);
    }

    private int CalculateAttack()
    {
        int damage = 0;
        foreach (SideEffectExecute see in CardInfo.SideEffectBundle.GetSideEffectExecutes(SideEffectExecute.TriggerTime.OnPlayCard, SideEffectExecute.TriggerRange.Self))
        {
            foreach (SideEffectBase se in see.SideEffectBases)
            {
                if (se is TargetSideEffect && ((TargetSideEffect) se).IsNeedChoice)
                {
                    if (se is IDamage damageSE) damage += damageSE.CalculateDamage();
                }
            }
        }

        return damage;
    }

    public override float DragComponent_DragDistance()
    {
        return GameManager.Instance.PullOutCardDistanceThreshold;
    }

    #region 卡牌效果

    private void summonSpellRequest(Vector3 dragLastPosition)
    {
        UseSpellCardRequest request = new UseSpellCardRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId);
        Client.Instance.Proxy.SendMessage(request);
        Usable = false;
    }

    private void summonSpellRequestToRetinue(ModuleRetinue targetModuleRetinue, Vector3 dragLastPosition)
    {
        if (targetModuleRetinue.M_ClientTempRetinueID != Const.CLIENT_TEMP_RETINUE_ID_NORMAL)
        {
            UseSpellCardToRetinueRequest request = new UseSpellCardToRetinueRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, targetModuleRetinue.M_RetinueID, true, targetModuleRetinue.M_ClientTempRetinueID);
            Client.Instance.Proxy.SendMessage(request);
        }
        else
        {
            UseSpellCardToRetinueRequest request = new UseSpellCardToRetinueRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, targetModuleRetinue.M_RetinueID, false, Const.CLIENT_TEMP_RETINUE_ID_NORMAL);
            Client.Instance.Proxy.SendMessage(request);
        }

        Usable = false;
    }

    private void summonSpellRequestToEquip(ModuleEquip targetEquip, Vector3 dragLastPosition)
    {
        UseSpellCardToEquipRequest request = new UseSpellCardToEquipRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, targetEquip.M_EquipID);
        Client.Instance.Proxy.SendMessage(request);

        Usable = false;
    }

    private void summonSpellRequestToShip(Ship targetShip, Vector3 dragLastPosition)
    {
        UseSpellCardToShipRequest request = new UseSpellCardToShipRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, targetShip.ClientPlayer.ClientId);
        Client.Instance.Proxy.SendMessage(request);
        Usable = false;
    }

    #endregion
}