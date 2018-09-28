using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardEquip : CardBase
{
    #region 卡牌上各模块

    internal SlotTypes M_EquipType;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer, bool isCardSelect)
    {
        base.Initiate(cardInfo, clientPlayer, isCardSelect);
    }

    #endregion

    #region 卡牌交互

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slots, moduleRetinue, ship, dragLastPosition, dragBeginPosition, dragBeginQuaternion);

        ClientPlayer.MyBattleGroundManager.StopShowSlotBloom();
        if (boardAreaType != ClientPlayer.MyHandArea) //离开手牌区域
            foreach (Slot sa in slots)
                if (CheckRetinueCanEquipMe(sa))
                {
                    summonEquipRequest(sa.M_ModuleRetinue, dragLastPosition);
                    return;
                }
                else
                {
                    AudioManager.Instance.SoundPlay("sfx/OnSelectRetinueFalse");
                    NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.isEnglish ? "You should select a right Slot to Equip." : "请选择正确的插槽装备", 0, 1f);
                }

        transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
        ClientPlayer.MyHandManager.RefreshCardsPlaceImmediately();
    }

    public bool CheckRetinueCanEquipMe(Slot sa)
    {
        if (sa.ClientPlayer == ClientPlayer && sa.MSlotTypes == M_EquipType && !sa.M_ModuleRetinue.IsDead)
        {
            if (M_EquipType == SlotTypes.Weapon && CardInfo.WeaponInfo.WeaponType == WeaponTypes.SniperGun)
            {
                if (sa.M_ModuleRetinue.CardInfo.RetinueInfo.IsSniper) return true; //狙击枪只能装在狙击手上
                else return false;
            }
            else return true;
        }

        return false;
    }


    public override float DragComponnet_DragDistance()
    {
        return GameManager.Instance.PullOutCardDistanceThreshold;
    }

    #region 卡牌效果

    //装备武器
    private void summonEquipRequest(ModuleRetinue moduleRetinue, Vector3 dragLastPosition)
    {
        switch (M_EquipType)
        {
            case SlotTypes.Weapon:
            {
                EquipWeaponRequest request = new EquipWeaponRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, moduleRetinue.M_RetinueID, 0, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z));
                Client.Instance.Proxy.SendMessage(request);
                break;
            }
            case SlotTypes.Shield:
            {
                EquipShieldRequest request = new EquipShieldRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, moduleRetinue.M_RetinueID, 0, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z));
                Client.Instance.Proxy.SendMessage(request);
                break;
            }
            case SlotTypes.Pack:
            {
                EquipPackRequest request = new EquipPackRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, moduleRetinue.M_RetinueID, 0, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z));
                Client.Instance.Proxy.SendMessage(request);
                break;
            }
            case SlotTypes.MA:
            {
                EquipMARequest request = new EquipMARequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, moduleRetinue.M_RetinueID, 0, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z));
                Client.Instance.Proxy.SendMessage(request);
                break;
            }
        }

        Usable = false;
    }

    #endregion

    #endregion
}