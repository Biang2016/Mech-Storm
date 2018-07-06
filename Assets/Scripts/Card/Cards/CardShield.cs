using System.Collections.Generic;
using UnityEngine;

internal class CardShield : CardBase
{
    protected override void Awake()
    {
        base.Awake();
        gameObjectPool = GameObjectPoolManager.GOPM.Pool_ShieldCardPool;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    #region 卡牌上各模块

    public TextMesh ShieldName;

    public TextMesh ShieldDesc;

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

    private string m_ShieldDesc;

    public string M_ShieldDesc
    {
        get { return m_ShieldDesc; }

        set
        {
            m_ShieldDesc = value;
            ShieldDesc.text = M_ShieldDesc;
        }
    }

    # endregion

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        ClientPlayer = clientPlayer;
        CardInfo = cardInfo;
        M_ShieldName = CardInfo.CardName;
        M_ShieldDesc = CardInfo.CardDesc;
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors, ModuleRetinue moduleRetinue, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slotAnchors, moduleRetinue, dragLastPosition, dragBeginPosition, dragBeginQuaternion);

        if (boardAreaType != ClientPlayer.MyHandArea) //离开手牌区域
            foreach (var sa in slotAnchors)
                if (sa.M_Slot.M_SlotType == SlotType.Shield && sa.M_Slot.ClientPlayer == ClientPlayer)
                {
                    summonShield(sa.M_ModuleRetinue);
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
    private void summonShield(ModuleRetinue moduleRetinue)
    {
        ClientPlayer.UseCost(M_Cost);
        if (moduleRetinue == null)
        {
            Debug.Log("No retinue on Place BUT SLOT HIT");
            return;
        }

        if (!moduleRetinue.M_Shield)
        {
            moduleRetinue.M_Shield = GameObjectPoolManager.GOPM.Pool_ModuleShieldPool.AllocateGameObject(moduleRetinue.transform).GetComponent<ModuleShield>();
        }

        moduleRetinue.M_Shield.M_ModuleRetinue = moduleRetinue;
        moduleRetinue.M_Shield.Initiate(CardInfo, ClientPlayer);
        BattleOperationRecord.BOP.Operations.Add(new OperationEquip(ClientPlayer, GameObjectID, new List<int> {moduleRetinue.M_Shield.GameObjectID}, OperationType.Equip, CardInfo.CardID, CardInfo.CardType, moduleRetinue));

        PoolRecycle();
        ClientPlayer.MyHandManager.DropCard(this);
    }

    #endregion
}