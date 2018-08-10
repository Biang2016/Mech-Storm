using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal class CardSpell : CardBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
    }

    void Update()
    {
    }


    #region 卡牌上各模块

    public Text SpellName;

    public Text SpellDesc;

    private string m_SpellName;

    public string M_SpellName
    {
        get { return m_SpellName; }

        set
        {
            m_SpellName = value;
            SpellName.text = M_SpellName;
        }
    }

    private string m_SpellDesc;

    public string M_SpellDesc
    {
        get { return m_SpellDesc; }

        set
        {
            m_SpellDesc = value;
            SpellDesc.text = M_SpellDesc;
        }
    }

    public bool hasTarget;

    # endregion

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer, bool isCardSelect)
    {
        base.Initiate(cardInfo, clientPlayer, isCardSelect);
        ClientPlayer = clientPlayer;
        CardInfo = cardInfo;
        M_SpellName = CardInfo.BaseInfo.CardName;
        M_SpellDesc = ((CardInfo_Spell) cardInfo).GetCardDescShow();

        hasTarget = false;
        foreach (SideEffectBase sideEffectBase in CardInfo.SideEffects_OnSummoned)
        {
            if (sideEffectBase is TargetSideEffect)
            {
                hasTarget = true;
                break;
            }
        }
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors, ModuleRetinue moduleRetinue, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slotAnchors, moduleRetinue, dragLastPosition, dragBeginPosition, dragBeginQuaternion);

        if (boardAreaType != ClientPlayer.MyHandArea) //离开手牌区域
        {
            if (hasTarget)
            {
                if (moduleRetinue == null)
                {
                    transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //带目标法术卡未指定目标，则收回
                    ClientPlayer.MyHandManager.RefreshCardsPlace();
                }
                else
                {
                    summonSpellRequest(moduleRetinue, dragLastPosition);
                }
            }
            else
            {
                summonSpellRequest(null, dragLastPosition);
            }
        }

        transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
        ClientPlayer.MyHandManager.RefreshCardsPlace();
    }


    public override float DragComponnet_DragDistance()
    {
        return 0f;
    }

    #region 卡牌效果

    //装备武器
    private void summonSpellRequest(ModuleRetinue targetModuleRetinue, Vector3 dragLastPosition)
    {
        if (targetModuleRetinue != null)
        {
            if (targetModuleRetinue.M_ClientTempRetinueID != (int) ModuleRetinue.ClientTempRetinueID.Normal)
            {
                UseSpellCardRequest request = new UseSpellCardRequest(Client.CS.Proxy.ClientId, M_CardInstanceId, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z), targetModuleRetinue.M_RetinueID, true, targetModuleRetinue.M_ClientTempRetinueID);
                Client.CS.Proxy.SendMessage(request);
            }
            else
            {
                UseSpellCardRequest request = new UseSpellCardRequest(Client.CS.Proxy.ClientId, M_CardInstanceId, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z), targetModuleRetinue.M_RetinueID, false, (int) ModuleRetinue.ClientTempRetinueID.Normal);
                Client.CS.Proxy.SendMessage(request);
            }
        }
        else
        {
            UseSpellCardRequest request = new UseSpellCardRequest(Client.CS.Proxy.ClientId, M_CardInstanceId, new MyCardGameCommon.Vector3(dragLastPosition.x, dragLastPosition.y, dragLastPosition.z), (int) DragManager.TargetSelect.None, false, (int) ModuleRetinue.ClientTempRetinueID.Normal);
            Client.CS.Proxy.SendMessage(request);
        }
    }

    #endregion
}