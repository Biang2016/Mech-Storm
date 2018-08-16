using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal class CardWeapon : CardBase
{
    #region 卡牌上各模块

    [SerializeField] private Text WeaponName;
    [SerializeField] private Text WeaponDesc;

    private string m_WeaponName;

    public string M_WeaponName
    {
        get { return m_WeaponName; }

        set
        {
            m_WeaponName = value;
            WeaponName.text = M_WeaponName;
        }
    }

    private string m_WeaponDesc;

    public string M_WeaponDesc
    {
        get { return m_WeaponDesc; }

        set
        {
            m_WeaponDesc = value;
            WeaponDesc.text = M_WeaponDesc;
        }
    }

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer,bool isCardSelect)
    {
        base.Initiate(cardInfo, clientPlayer, isCardSelect);
        ClientPlayer = clientPlayer;
        CardInfo = cardInfo;
        M_WeaponName = CardInfo.BaseInfo.CardName;
        M_WeaponDesc = ((CardInfo_Weapon) cardInfo).GetCardDescShow();
    }

    #endregion

    #region 卡牌交互

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slots, moduleRetinue, dragLastPosition, dragBeginPosition, dragBeginQuaternion);

        if (boardAreaType != ClientPlayer.MyHandArea) //离开手牌区域
            foreach (Slot sa in slots)
                if (sa.MSlotTypes == SlotTypes.Weapon && sa.ClientPlayer == ClientPlayer)
                {
                    summonWeaponRequest(sa.M_ModuleRetinue, dragLastPosition);
                    ClientPlayer.MyBattleGroundManager.StopShowSlotBloom();
                    return;
                }

        transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
        ClientPlayer.MyBattleGroundManager.StopShowSlotBloom();
        ClientPlayer.MyHandManager.RefreshCardsPlace();
    }


    public override float DragComponnet_DragDistance()
    {
        return GameManager.Instance.PullOutCardDistanceThreshold;
    }

    #region 卡牌效果

    //装备武器
    private void summonWeaponRequest(ModuleRetinue moduleRetinue, Vector3 dragLastPosition)
    {
        EquipWeaponRequest request = new EquipWeaponRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, moduleRetinue.M_RetinueID, 0, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z));
        Client.Instance.Proxy.SendMessage(request);
        Usable = false;
    }

    #endregion

    #endregion
}