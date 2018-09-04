using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal class CardShield : CardBase
{
    #region 卡牌上各模块

    [SerializeField] private Text ShieldName;
    [SerializeField] private Text ShieldDesc;

    private string m_ShieldName;

    public string M_ShieldName
    {
        get { return m_ShieldName; }

        set
        {
            m_ShieldName = value;
            ShieldName.text = M_ShieldName;
        }
    }

    public override string M_Desc
    {
        get { return m_Desc; }

        set
        {
            m_Desc = value;
            ShieldDesc.text = value;
        }
    }

    # endregion

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer, bool isCardSelect)
    {
        base.Initiate(cardInfo, clientPlayer, isCardSelect);
        M_ShieldName = CardInfo.BaseInfo.CardName;
        M_Desc = ((CardInfo_Shield) CardInfo).GetCardDescShow();
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slots, moduleRetinue, ship, dragLastPosition, dragBeginPosition, dragBeginQuaternion);

        if (boardAreaType != ClientPlayer.MyHandArea) //离开手牌区域
            foreach (Slot sa in slots)
                if (sa.MSlotTypes == SlotTypes.Shield && sa.ClientPlayer == ClientPlayer)
                {
                    summonShieldRequest(sa.M_ModuleRetinue, dragLastPosition);
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
    private void summonShieldRequest(ModuleRetinue moduleRetinue, Vector3 dragLastPosition)
    {
        EquipShieldRequest request = new EquipShieldRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, moduleRetinue.M_RetinueID, 0, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z));
        Client.Instance.Proxy.SendMessage(request);
        Usable = false;
    }

    #endregion
}