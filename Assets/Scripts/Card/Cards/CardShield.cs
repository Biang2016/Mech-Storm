using System.Collections.Generic;
using UnityEngine;

internal class CardShield : CardBase {
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
    public string M_ShieldName {
        get {
            return m_ShieldName;
        }

        set {
            m_ShieldName = value;
            ShieldName.text = M_ShieldName;
        }
    }

    private string m_ShieldDesc;
    public string M_ShieldDesc {
        get {
            return m_ShieldDesc;
        }

        set {
            m_ShieldDesc = value;
            ShieldDesc.text = M_ShieldDesc;
        }
    }

    # endregion

    public override void Initiate(CardInfo_Base cardInfo, Player player)
    {
        base.Initiate(cardInfo, player);
        Player = player;
        CardInfo = cardInfo;
        M_ShieldName = CardInfo.CardName;
        M_ShieldDesc = CardInfo.CardDesc;
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors,
        ModuleRetinue moduleRetinue, Vector3 dragLastPosition, Vector3 dragBeginPosition,
        Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slotAnchors, moduleRetinue, dragLastPosition, dragBeginPosition,
            dragBeginQuaternion);

        if (boardAreaType != Player.MyHandArea) //离开手牌区域
            foreach (var sa in slotAnchors)
                if (sa.M_SlotType == SlotType.Shield && sa.Player == Player) {
                    summonShield(dragLastPosition, sa.M_ModuleRetinue);
                    return;
                }

        transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
        Player.MyHandManager.RefreshCardsPlace();
    }

    public override void DragComponent_SetStates(ref bool canDrag, ref bool hasTarget)
    {
        canDrag = true;
        hasTarget = CardInfo.HasTarget;
    }
    public override float DragComponnet_DragDistance()
    {
        return 5f;
    }

    #region 卡牌效果
    //装备武器
    private void summonShield(Vector3 dragLastPosition, ModuleRetinue moduleRetinue)
    {
        if (moduleRetinue == null) {
            Debug.Log("No retinue on Place BUT SLOT HIT");
            return;
        }

        if (!moduleRetinue.M_Shield)
            moduleRetinue.M_Shield = GameObjectPoolManager.GOPM.Pool_ModuleShieldPool
                .AllocateGameObject(moduleRetinue.transform).GetComponent<ModuleShield>();
        moduleRetinue.M_Shield.M_ModuleRetinue = moduleRetinue;
        moduleRetinue.M_Shield.Initiate(CardInfo, Player);
        PoolRecycle();
        Player.MyHandManager.DropCard(this);
    }
    #endregion
}
