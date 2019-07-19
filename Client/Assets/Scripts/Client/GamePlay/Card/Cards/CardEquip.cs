using System.Collections.Generic;
using UnityEngine;

public class CardEquip : CardBase
{
    internal SlotTypes M_EquipType;

    #region 卡牌交互

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleMech moduleMech, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slots, moduleMech, ship, dragLastPosition, dragBeginPosition, dragBeginQuaternion);
        if (DragManager.Instance.IsCanceling)
        {
            DragManager.Instance.IsCanceling = false;
            CancelPlayOut(dragBeginPosition, dragBeginQuaternion);
            return;
        }

        ClientPlayer.BattlePlayer.BattleGroundManager.StopShowSlotBloom();
        if (boardAreaType != ClientPlayer.BattlePlayer.HandArea) //离开手牌区域
        {
            foreach (Slot sa in slots)
            {
                if (CheckMechCanEquipMe(sa, out string info))
                {
                    summonEquipRequest(sa.Mech, dragLastPosition);
                    return;
                }
                else
                {
                    AudioManager.Instance.SoundPlay("sfx/OnSelectMechFalse");
                    NoticeManager.Instance.ShowInfoPanelCenter(info, 0, 1f);
                    CancelPlayOut(dragBeginPosition, dragBeginQuaternion); //收回
                }
            }
        }
        else
        {
            CancelPlayOut(dragBeginPosition, dragBeginQuaternion); //收回
        }
    }

    private void CancelPlayOut(Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion);
        ClientPlayer.BattlePlayer.HandManager.RefreshCardsPlace();
    }

    public bool CheckMechCanEquipMe(Slot sa, out string info)
    {
        if (sa.ClientPlayer == ClientPlayer && sa.MSlotTypes == M_EquipType && !sa.Mech.IsDead)
        {
            if (M_EquipType == SlotTypes.Weapon && CardInfo.WeaponInfo.WeaponType == WeaponTypes.SniperGun)
            {
                if (sa.Mech.CardInfo.MechInfo.IsSniper)
                {
                    info = "";
                    return true; //狙击枪只能装在狙击手上
                }
                else
                {
                    info = LanguageManager.Instance.GetText("Notice_CardEquip_SniperGunOnlySniper");
                    return false;
                }
            }
            else if (M_EquipType == SlotTypes.MA)
            {
                if (sa.Mech.MechEquipSystemComponent.IsAllEquipExceptMA)
                {
                    info = "";
                    return true;
                }
                else
                {
                    info = LanguageManager.Instance.GetText("Notice_CardEquip_MANeedAllEquiped");
                    return false;
                }
            }
            else
            {
                info = "";
                return true;
            }
        }

        info = LanguageManager.Instance.GetText("Notice_CardEquip_SelectCorrectSlot");
        return false;
    }

    public override float DragComponent_DragDistance()
    {
        return 0;
    }

    #region 卡牌效果

    //装备武器
    private void summonEquipRequest(ModuleMech moduleMech, Vector3 dragLastPosition)
    {
        switch (M_EquipType)
        {
            case SlotTypes.Weapon:
            {
                EquipWeaponRequest request = new EquipWeaponRequest(Client.Instance.Proxy.ClientID, M_CardInstanceId, moduleMech.M_MechID);
                Client.Instance.Proxy.SendMessage(request);
                break;
            }
            case SlotTypes.Shield:
            {
                EquipShieldRequest request = new EquipShieldRequest(Client.Instance.Proxy.ClientID, M_CardInstanceId, moduleMech.M_MechID);
                Client.Instance.Proxy.SendMessage(request);
                break;
            }
            case SlotTypes.Pack:
            {
                EquipPackRequest request = new EquipPackRequest(Client.Instance.Proxy.ClientID, M_CardInstanceId, moduleMech.M_MechID);
                Client.Instance.Proxy.SendMessage(request);
                break;
            }
            case SlotTypes.MA:
            {
                EquipMARequest request = new EquipMARequest(Client.Instance.Proxy.ClientID, M_CardInstanceId, moduleMech.M_MechID);
                Client.Instance.Proxy.SendMessage(request);
                break;
            }
        }

        Usable = false;
    }

    #endregion

    #endregion
}