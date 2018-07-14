using System.Collections.Generic;
using UnityEngine;

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

    protected override void Awake()
    {
        base.Awake();
        gameObjectPool = GameObjectPoolManager.GOPM.Pool_RetinueCardPool;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    #region 卡牌上各模块

    public TextMesh TextMesh_RetinueName;

    public TextMesh TextMesh_RetinueDesc;

    public GameObject Block_RetinueLeftLife;
    GameObject GoNumberSet_RetinueLeftLife;
    CardNumberSet CardNumberSet_RetinueLeftLife;

    public GameObject Block_RetinueTotalLife;
    GameObject GoNumberSet_RetinueTotalLife;
    CardNumberSet CardNumberSet_RetinueTotalLife;

    public GameObject Block_RetinueAttack;
    protected GameObject GoNumberSet_RetinueAttack;
    protected CardNumberSet CardNumberSet_RetinueAttack;

    public GameObject Block_RetinueShield;
    protected GameObject GoNumberSet_RetinueShield;
    protected CardNumberSet CardNumberSet_RetinueShield;

    public GameObject Block_RetinueArmor;
    protected GameObject GoNumberSet_RetinueArmor;
    protected CardNumberSet CardNumberSet_RetinueArmor;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        M_RetinueName = ((CardInfo_Retinue) cardInfo).CardName;
        M_RetinueDesc = ((CardInfo_Retinue) cardInfo).CardDesc;
        M_RetinueLeftLife = ((CardInfo_Retinue) CardInfo).Life;
        M_RetinueTotalLife = ((CardInfo_Retinue) CardInfo).Life;
        M_RetinueAttack = ((CardInfo_Retinue) cardInfo).BasicAttack;
        M_RetinueArmor = ((CardInfo_Retinue) cardInfo).BasicArmor;
        M_RetinueShield = ((CardInfo_Retinue) cardInfo).BasicShield;

        Slot1.ClientPlayer = ClientPlayer;
        Slot1.MSlotTypes = ((CardInfo_Retinue) cardInfo).Slot1;
        Slot2.ClientPlayer = ClientPlayer;
        Slot2.MSlotTypes = ((CardInfo_Retinue) cardInfo).Slot2;
        Slot3.ClientPlayer = ClientPlayer;
        Slot3.MSlotTypes = ((CardInfo_Retinue) cardInfo).Slot3;
        Slot4.ClientPlayer = ClientPlayer;
        Slot4.MSlotTypes = ((CardInfo_Retinue) cardInfo).Slot4;
    }

    private string m_RetinueName;

    public string M_RetinueName
    {
        get { return m_RetinueName; }
        set
        {
            m_RetinueName = value;
            TextMesh_RetinueName.text = value;
        }
    }

    private string m_RetinueDesc;

    public string M_RetinueDesc
    {
        get { return m_RetinueDesc; }
        set
        {
            m_RetinueDesc = value;
            TextMesh_RetinueDesc.text = value;
        }
    }

    private int m_RetinueLeftLife;

    public int M_RetinueLeftLife
    {
        get { return m_RetinueLeftLife; }
        set
        {
            m_RetinueLeftLife = value;
            initiateNumbers(ref GoNumberSet_RetinueLeftLife, ref CardNumberSet_RetinueLeftLife, NumberSize.Big, CardNumberSet.TextAlign.Left, Block_RetinueLeftLife);
            CardNumberSet_RetinueLeftLife.Number = m_RetinueLeftLife;
        }
    }

    private int m_RetinueTotalLife;

    public int M_RetinueTotalLife
    {
        get { return m_RetinueTotalLife; }
        set
        {
            m_RetinueTotalLife = value;
            initiateNumbers(ref GoNumberSet_RetinueTotalLife, ref CardNumberSet_RetinueTotalLife, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueTotalLife, '/');
            CardNumberSet_RetinueTotalLife.Number = m_RetinueTotalLife;
        }
    }

    private int m_RetinueAttack;

    public int M_RetinueAttack
    {
        get { return m_RetinueAttack; }
        set
        {
            m_RetinueAttack = value;
            initiateNumbers(ref GoNumberSet_RetinueAttack, ref CardNumberSet_RetinueAttack, NumberSize.Small, CardNumberSet.TextAlign.Right, Block_RetinueAttack, '+');
            CardNumberSet_RetinueAttack.Number = m_RetinueAttack;
        }
    }

    private int m_RetinueArmor;

    public int M_RetinueArmor
    {
        get { return m_RetinueArmor; }
        set
        {
            m_RetinueArmor = value;
            initiateNumbers(ref GoNumberSet_RetinueArmor, ref CardNumberSet_RetinueArmor, NumberSize.Small, CardNumberSet.TextAlign.Right, Block_RetinueArmor, '+');
            CardNumberSet_RetinueArmor.Number = m_RetinueArmor;
        }
    }

    private int m_RetinueShield;

    public int M_RetinueShield
    {
        get { return m_RetinueShield; }
        set
        {
            m_RetinueShield = value;
            initiateNumbers(ref GoNumberSet_RetinueShield, ref CardNumberSet_RetinueShield, NumberSize.Small, CardNumberSet.TextAlign.Right, Block_RetinueShield, '+');
            CardNumberSet_RetinueShield.Number = m_RetinueShield;
        }
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
    internal GameObject MA; //飞行踏板/机动堡垒
    internal GameObject Energy; //能量槽

    # endregion

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors, ModuleRetinue moduleRetinue, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slotAnchors, moduleRetinue, dragLastPosition, dragBeginPosition, dragBeginQuaternion);

        if (boardAreaType != ClientPlayer.MyHandArea) //脱手即出牌
        {
            if (ClientPlayer.MyBattleGroundManager.BattleGroundIsFull)
            {
                ClientPlayer.MyHandManager.RefreshCardsPlace();
                return;
            }

            summonRetinueRequest(dragLastPosition);
        }
        else
        {
            transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
            ClientPlayer.MyHandManager.RefreshCardsPlace();
        }
    }


    #region 卡牌效果

    //召唤随从
    private void summonRetinueRequest(Vector3 dragLastPosition)
    {
        int handCardIndex = ClientPlayer.MyHandManager.GetCardIndex(this);
        int battleGroundIndex = ClientPlayer.MyBattleGroundManager.ComputePosition(dragLastPosition);
        SummonRetinueRequest request = new SummonRetinueRequest(Client.CS.Proxy.ClientId, (CardInfo_Retinue) CardInfo, handCardIndex, battleGroundIndex);
        Client.CS.Proxy.SendMessage(request);
    }

    #endregion
}