using System.Collections.Generic;
using UnityEngine;

public class CardSpell : CardBase
{
    public override void Initiate(CardInfo_Base cardInfo, CardShowMode cardShowMode, ClientPlayer clientPlayer = null)
    {
        base.Initiate(cardInfo, cardShowMode, clientPlayer);
        CardInfo.TargetInfo.HasTargetMech = false;
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
                        TargetRange temp = ((TargetSideEffect) se).TargetRange;
                        if (temp != TargetRange.Ships && temp != TargetRange.SelfShip && temp != TargetRange.EnemyShip && temp != TargetRange.AllLife)
                        {
                            CardInfo.TargetInfo.HasTargetMech = true;
                            CardInfo.TargetInfo.targetMechRange = ((TargetSideEffect) se).TargetRange;
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

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleMech moduleMech, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slots, moduleMech, ship, dragLastPosition, dragBeginPosition, dragBeginQuaternion);
        RoundManager.Instance.HideTargetPreviewArrow();
        if (boardAreaType != ClientPlayer.BattlePlayer.HandArea) //离开手牌区域
        {
            if (!CardInfo.TargetInfo.HasTargetMech && !CardInfo.TargetInfo.HasTargetEquip && !CardInfo.TargetInfo.HasTargetShip)
            {
                summonSpellRequest(dragLastPosition);
                return;
            }
            else if (CardInfo.TargetInfo.HasTargetMech)
            {
                //To Mech
                if (moduleMech == null || moduleMech.IsDead)
                {
                    transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //带目标法术卡未指定目标，则收回
                    ClientPlayer.BattlePlayer.HandManager.RefreshCardsPlace();
                }
                else
                {
                    bool validTarget = false;
                    switch (CardInfo.TargetInfo.targetMechRange)
                    {
                        case TargetRange.None:
                            validTarget = false;
                            break;
                        case TargetRange.Mechs:
                            validTarget = true;
                            break;
                        case TargetRange.SelfMechs:
                            if (moduleMech.ClientPlayer == RoundManager.Instance.SelfClientPlayer) validTarget = true;
                            break;
                        case TargetRange.EnemyMechs:
                            if (moduleMech.ClientPlayer == RoundManager.Instance.EnemyClientPlayer) validTarget = true;
                            break;
                        case TargetRange.Heroes:
                            if (!moduleMech.CardInfo.MechInfo.IsSoldier) validTarget = true;
                            break;
                        case TargetRange.SelfHeroes:
                            if (moduleMech.ClientPlayer == RoundManager.Instance.SelfClientPlayer && !moduleMech.CardInfo.MechInfo.IsSoldier) validTarget = true;
                            break;
                        case TargetRange.EnemyHeroes:
                            if (moduleMech.ClientPlayer == RoundManager.Instance.EnemyClientPlayer && !moduleMech.CardInfo.MechInfo.IsSoldier) validTarget = true;
                            break;
                        case TargetRange.Soldiers:
                            if (moduleMech.CardInfo.MechInfo.IsSoldier) validTarget = true;
                            break;
                        case TargetRange.SelfSoldiers:
                            if (moduleMech.ClientPlayer == RoundManager.Instance.SelfClientPlayer && moduleMech.CardInfo.MechInfo.IsSoldier) validTarget = true;
                            break;
                        case TargetRange.EnemySoldiers:
                            if (moduleMech.ClientPlayer == RoundManager.Instance.EnemyClientPlayer && moduleMech.CardInfo.MechInfo.IsSoldier) validTarget = true;
                            break;
                        case TargetRange.AllLife:
                            validTarget = true;
                            break;
                    }

                    if (validTarget)
                    {
                        summonSpellRequestToMech(moduleMech, dragLastPosition);
                        return;
                    }
                    else
                    {
                        AudioManager.Instance.SoundPlay("sfx/OnSelectMechFalse");
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
                    ClientPlayer.BattlePlayer.HandManager.RefreshCardsPlace();
                }
                else
                {
                    ModuleEquip equip = slots[0].Mech.GetEquipBySlotType(slots[0].MSlotTypes);
                    if (equip == null || equip.IsDead)
                    {
                        transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //带目标法术卡未指定目标，则收回
                        ClientPlayer.BattlePlayer.HandManager.RefreshCardsPlace();
                    }
                    else
                    {
                        bool validTarget = false;
                        switch (CardInfo.TargetInfo.targetEquipRange)
                        {
                            case TargetRange.None:
                                validTarget = false;
                                break;
                            case TargetRange.Mechs:
                                validTarget = true;
                                break;
                            case TargetRange.SelfMechs:
                                if (equip.ClientPlayer == RoundManager.Instance.SelfClientPlayer) validTarget = true;
                                break;
                            case TargetRange.EnemyMechs:
                                if (equip.ClientPlayer == RoundManager.Instance.EnemyClientPlayer) validTarget = true;
                                break;
                            case TargetRange.Heroes:
                                if (!equip.CardInfo.MechInfo.IsSoldier) validTarget = true;
                                break;
                            case TargetRange.SelfHeroes:
                                if (equip.ClientPlayer == RoundManager.Instance.SelfClientPlayer && !equip.CardInfo.MechInfo.IsSoldier) validTarget = true;
                                break;
                            case TargetRange.EnemyHeroes:
                                if (equip.ClientPlayer == RoundManager.Instance.EnemyClientPlayer && !equip.CardInfo.MechInfo.IsSoldier) validTarget = true;
                                break;
                            case TargetRange.Soldiers:
                                if (equip.CardInfo.MechInfo.IsSoldier) validTarget = true;
                                break;
                            case TargetRange.SelfSoldiers:
                                if (equip.ClientPlayer == RoundManager.Instance.SelfClientPlayer && equip.CardInfo.MechInfo.IsSoldier) validTarget = true;
                                break;
                            case TargetRange.EnemySoldiers:
                                if (equip.ClientPlayer == RoundManager.Instance.EnemyClientPlayer && equip.CardInfo.MechInfo.IsSoldier) validTarget = true;
                                break;
                            case TargetRange.AllLife:
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
                            AudioManager.Instance.SoundPlay("sfx/OnSelectMechFalse");
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
                    ClientPlayer.BattlePlayer.HandManager.RefreshCardsPlace();
                }
                else
                {
                    bool validTarget = false;
                    switch (CardInfo.TargetInfo.targetShipRange)
                    {
                        case TargetRange.Ships:
                            validTarget = true;
                            break;
                        case TargetRange.SelfShip:
                            if (ship.ClientPlayer == RoundManager.Instance.SelfClientPlayer) validTarget = true;
                            break;
                        case TargetRange.EnemyShip:
                            if (ship.ClientPlayer == RoundManager.Instance.EnemyClientPlayer) validTarget = true;
                            break;
                        case TargetRange.AllLife:
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
                        AudioManager.Instance.SoundPlay("sfx/OnSelectMechFalse");
                        NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("CardSpell_SelectCorrectShip"), 0, 1f);
                    }
                }
            }

            DragManager.Instance.DragOutDamage = 0;
        }

        transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
        ClientPlayer.BattlePlayer.HandManager.RefreshCardsPlace();
    }

    public override void DragComponent_DragOutEffects()
    {
        base.DragComponent_DragOutEffects();
        DragManager.Instance.DragOutDamage = CalculateAttack();
        RoundManager.Instance.ShowTargetPreviewArrow(CardInfo.TargetInfo.targetMechRange);
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
        return 0;
    }

    #region 卡牌效果

    private void summonSpellRequest(Vector3 dragLastPosition)
    {
        UseSpellCardRequest request = new UseSpellCardRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId);
        Client.Instance.Proxy.SendMessage(request);
        Usable = false;
    }

    private void summonSpellRequestToMech(ModuleMech targetModuleMech, Vector3 dragLastPosition)
    {
        if (targetModuleMech.M_ClientTempMechID != Const.CLIENT_TEMP_MECH_ID_NORMAL)
        {
            UseSpellCardToMechRequest request = new UseSpellCardToMechRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, targetModuleMech.M_MechID, true, targetModuleMech.M_ClientTempMechID);
            Client.Instance.Proxy.SendMessage(request);
        }
        else
        {
            UseSpellCardToMechRequest request = new UseSpellCardToMechRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, targetModuleMech.M_MechID, false, Const.CLIENT_TEMP_MECH_ID_NORMAL);
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