using System.Collections.Generic;
using UnityEngine;

internal class CardWeapon : CardBase
{
    protected override void Awake()
    {
        base.Awake();
        gameObjectPool = GameObjectPoolManager.GOPM.Pool_WeaponCardPool;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    #region 卡牌上各模块

    public TextMesh WeaponName;

    public TextMesh WeaponDesc;

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

    # endregion

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        ClientPlayer = clientPlayer;
        CardInfo = cardInfo;
        M_WeaponName = CardInfo.CardName;
        M_WeaponDesc = CardInfo.CardDesc;
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors, ModuleRetinue moduleRetinue, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slotAnchors, moduleRetinue, dragLastPosition, dragBeginPosition, dragBeginQuaternion);

        if (boardAreaType != ClientPlayer.MyHandArea) //离开手牌区域
            foreach (SlotAnchor sa in slotAnchors)
                if (sa.M_Slot.MSlotTypes == SlotTypes.Weapon && sa.M_Slot.ClientPlayer == ClientPlayer)
                {
                    summonWeaponRequest(sa.M_ModuleRetinue);
                    return;
                }

        transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
        ClientPlayer.MyHandManager.RefreshCardsPlace();
    }


    public override float DragComponnet_DragDistance()
    {
        return 5f;
    }

    #region 卡牌效果

    //装备武器
    private void summonWeaponRequest(ModuleRetinue moduleRetinue)
    {
        int handCardIndex = ClientPlayer.MyHandManager.GetCardIndex(this);
        int battleGroundIndex = ClientPlayer.MyBattleGroundManager.GetRetinuePlaceIndex(moduleRetinue);
        EquipWeaponRequest request = new EquipWeaponRequest(Client.CS.Proxy.ClientId, (CardInfo_Weapon) CardInfo, handCardIndex, battleGroundIndex, 0);
        Client.CS.Proxy.SendMessage(request);
    }

    #endregion
}