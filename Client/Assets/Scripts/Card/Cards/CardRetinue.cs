using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal class CardRetinue : CardBase
{
    public override void PoolRecycle()
    {
        base.PoolRecycle();
        if (Weapon)
        {
            Weapon.PoolRecycle();
            Weapon = null;
        }

        if (Shield)
        {
            Shield.PoolRecycle();
            Shield = null;
        }

        if (Pack)
        {
            //Pack.PoolRecycle();
            Pack = null;
        }

        if (MA)
        {
            //MA.PoolRecycle();
            MA = null;
        }

        if (Pack) Pack = null;
        if (MA) MA = null;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    #region 卡牌上各模块

    public Text Text_RetinueName;

    public Text Text_RetinueDesc;

    public GameObject Block_RetinueTotalLife;
    GameObject GoNumberSet_RetinueTotalLife;
    CardNumberSet CardNumberSet_RetinueTotalLife;


    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer, bool isCardSelect)
    {
        base.Initiate(cardInfo, clientPlayer,isCardSelect);
        M_RetinueName = cardInfo.BaseInfo.CardName;
        M_RetinueDesc = ((CardInfo_Retinue) cardInfo).GetCardDescShow();
        M_RetinueLeftLife = cardInfo.LifeInfo.Life;
        M_RetinueTotalLife = cardInfo.LifeInfo.TotalLife;
        M_RetinueAttack = cardInfo.BattleInfo.BasicAttack;
        M_RetinueArmor = cardInfo.BattleInfo.BasicArmor;
        M_RetinueShield = cardInfo.BattleInfo.BasicShield;

        Slot1.ClientPlayer = ClientPlayer;
        Slot1.MSlotTypes = cardInfo.SlotInfo.Slot1;
        Slot2.ClientPlayer = ClientPlayer;
        Slot2.MSlotTypes = cardInfo.SlotInfo.Slot2;
        Slot3.ClientPlayer = ClientPlayer;
        Slot3.MSlotTypes = cardInfo.SlotInfo.Slot3;
        Slot4.ClientPlayer = ClientPlayer;
        Slot4.MSlotTypes = cardInfo.SlotInfo.Slot4;
    }

    private string m_RetinueName;

    public string M_RetinueName
    {
        get { return m_RetinueName; }
        set
        {
            m_RetinueName = value;
            Text_RetinueName.text = value;
        }
    }

    private string m_RetinueDesc;

    public string M_RetinueDesc
    {
        get { return m_RetinueDesc; }
        set
        {
            m_RetinueDesc = value;
            Text_RetinueDesc.text = value;
        }
    }

    private int m_RetinueLeftLife;

    public int M_RetinueLeftLife
    {
        get { return m_RetinueLeftLife; }
        set { m_RetinueLeftLife = value; }
    }

    private int m_RetinueTotalLife;

    public int M_RetinueTotalLife
    {
        get { return m_RetinueTotalLife; }
        set
        {
            m_RetinueTotalLife = value;
            initiateNumbers(ref GoNumberSet_RetinueTotalLife, ref CardNumberSet_RetinueTotalLife, NumberSize.Big, CardNumberSet.TextAlign.Right, Block_RetinueTotalLife);
            CardNumberSet_RetinueTotalLife.Number = m_RetinueTotalLife;
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

    public Slot Slot1;
    public Slot Slot2;
    public Slot Slot3;
    public Slot Slot4;
    internal GameObject Pack;
    internal ModuleWeapon Weapon;
    internal ModuleShield Shield;
    internal GameObject MA;

    # endregion

    public override void DragComponent_OnMousePressed(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors, ModuleRetinue moduleRetinue, Vector3 dragLastPosition)
    {
        base.DragComponent_OnMousePressed(boardAreaType, slotAnchors, moduleRetinue, dragLastPosition);

        if (boardAreaType == ClientPlayer.MyBattleGroundArea && !ClientPlayer.MyBattleGroundManager.BattleGroundIsFull) //拖随从牌到战场区域
        {
            int previewPosition = ClientPlayer.MyBattleGroundManager.ComputePosition(dragLastPosition);
            ClientPlayer.MyBattleGroundManager.AddRetinuePreview(previewPosition);
        }
        else //离开战场区域
        {
            ClientPlayer.MyBattleGroundManager.RemoveRetinuePreview();
        }
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors, ModuleRetinue moduleRetinue, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slotAnchors, moduleRetinue, dragLastPosition, dragBeginPosition, dragBeginQuaternion);

        if (boardAreaType != ClientPlayer.MyHandArea) //脱手即出牌
        {
            summonRetinueRequest(dragLastPosition);
        }
        else
        {
            ClientPlayer.MyBattleGroundManager.RemoveRetinuePreview();
            transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
            ClientPlayer.MyHandManager.RefreshCardsPlace();
        }
    }


    #region 卡牌效果

    //召唤随从
    private void summonRetinueRequest(Vector3 dragLastPosition)
    {
        if (ClientPlayer.MyBattleGroundManager.BattleGroundIsFull)
        {
            ClientPlayer.MyHandManager.RefreshCardsPlace();
            return;
        }

        int battleGroundIndex = ClientPlayer.MyBattleGroundManager.ComputePosition(dragLastPosition);
        SummonRetinueRequest request = new SummonRetinueRequest(Client.CS.Proxy.ClientId, M_CardInstanceId, battleGroundIndex, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z));
        Client.CS.Proxy.SendMessage(request);
    }

    #endregion
}