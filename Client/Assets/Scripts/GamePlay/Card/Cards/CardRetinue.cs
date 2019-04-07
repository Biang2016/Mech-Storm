using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardRetinue : CardBase
{
    public override void PoolRecycle()
    {
        base.PoolRecycle();
        HideAllSlotHover();
        MoveCoinBGUpper();
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
            Pack.PoolRecycle();
            Pack = null;
        }

        if (MA)
        {
            MA.PoolRecycle();
            MA = null;
        }

        if (Block_Count_HigherPivot) SetBlockCountPosition(Block_Count_HigherPivot.position);
        if (Block_CountMax_HigherPivot) SetBlockCountMaxPosition(Block_CountMax_HigherPivot.position);
    }

    #region 卡牌上各模块

    [SerializeField] private Text Text_Life;
    [SerializeField] private Transform CoinBGLowerPivot;
    [SerializeField] private Transform CoinBGUpperPivot;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer, bool isCardSelect, int limitNum = -1)
    {
        base.Initiate(cardInfo, clientPlayer, isCardSelect, limitNum);
        M_RetinueTotalLife = CardInfo.LifeInfo.TotalLife;
        M_RetinueAttack = CardInfo.BattleInfo.BasicAttack;
        M_RetinueArmor = CardInfo.BattleInfo.BasicArmor;
        M_RetinueShield = CardInfo.BattleInfo.BasicShield;

        Slot1.ClientPlayer = ClientPlayer;
        Slot1.MSlotTypes = CardInfo.RetinueInfo.Slots[0];
        Slot2.ClientPlayer = ClientPlayer;
        Slot2.MSlotTypes = CardInfo.RetinueInfo.Slots[1];
        Slot3.ClientPlayer = ClientPlayer;
        Slot3.MSlotTypes = CardInfo.RetinueInfo.Slots[2];
        Slot4.ClientPlayer = ClientPlayer;
        Slot4.MSlotTypes = CardInfo.RetinueInfo.Slots[3];

        HideAllSlotHover();

        if (isCardSelect)
        {
            if (cardInfo.RetinueInfo.HasSlotType(SlotTypes.MA))
            {
                SetBlockCountPosition(Block_Count_LowerPivot.localPosition);
                SetBlockCountMaxPosition(Block_CountMax_LowerPivot.localPosition);
            }
            else
            {
                SetBlockCountPosition(Block_Count_HigherPivot.localPosition);
                SetBlockCountMaxPosition(Block_CountMax_HigherPivot.localPosition);
            }
        }
    }

    public void MoveCoinBGLower()
    {
        if (IsCardSelect && !CardInfo.RetinueInfo.IsSoldier) CoinImageBG.transform.position = CoinBGLowerPivot.transform.position;
    }

    public void MoveCoinBGUpper()
    {
        if (IsCardSelect && !CardInfo.RetinueInfo.IsSoldier) CoinImageBG.transform.position = CoinBGUpperPivot.transform.position;
    }

    public void ShowAllSlotHover()
    {
        if (Slot1.MSlotTypes != SlotTypes.None) Slot1.ShowHoverGO();
        if (Slot2.MSlotTypes != SlotTypes.None) Slot2.ShowHoverGO();
        if (Slot3.MSlotTypes != SlotTypes.None) Slot3.ShowHoverGO();
        if (Slot4.MSlotTypes != SlotTypes.None) Slot4.ShowHoverGO();
    }

    public void HideAllSlotHover()
    {
        Slot1.HideHoverShowGO();
        Slot2.HideHoverShowGO();
        Slot3.HideHoverShowGO();
        Slot4.HideHoverShowGO();
    }

    private int m_RetinueTotalLife;

    public int M_RetinueTotalLife
    {
        get { return m_RetinueTotalLife; }
        set
        {
            m_RetinueTotalLife = value;
            Text_Life.text = value.ToString();
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

    [SerializeField] private Slot Slot1;
    [SerializeField] private Slot Slot2;
    [SerializeField] private Slot Slot3;
    [SerializeField] private Slot Slot4;

    [SerializeField] private RawImage WeaponBloom;
    [SerializeField] private RawImage ShieldBloom;
    [SerializeField] private RawImage PackBloom;
    [SerializeField] private RawImage MABloom;

    internal ModuleWeapon Weapon;
    internal ModuleShield Shield;
    internal ModulePack Pack;
    internal ModuleMA MA;

    [SerializeField] private Canvas IconCanvas;

    [SerializeField] private Transform Block_Count_HigherPivot;
    [SerializeField] private Transform Block_Count_LowerPivot;
    [SerializeField] private Transform Block_CountMax_HigherPivot;
    [SerializeField] private Transform Block_CountMax_LowerPivot;

    # endregion

    public override void DragComponent_OnMousePressed(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Vector3 dragLastPosition)
    {
        base.DragComponent_OnMousePressed(boardAreaType, slots, moduleRetinue, dragLastPosition);

        if (boardAreaType == ClientPlayer.MyBattleGroundArea && !ClientPlayer.MyBattleGroundManager.BattleGroundIsFull) //拖机甲牌到战场区域
        {
            int previewPosition = ClientPlayer.MyBattleGroundManager.ComputePosition(dragLastPosition);
            ClientPlayer.MyBattleGroundManager.AddRetinuePreview(previewPosition);
        }
        else //离开战场区域
        {
            ClientPlayer.MyBattleGroundManager.RemoveRetinuePreview();
        }
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slots, moduleRetinue, ship, dragLastPosition, dragBeginPosition, dragBeginQuaternion);
        if (!CardInfo.TargetInfo.HasTargetRetinue) //无指定目标的副作用
        {
            if (boardAreaType != ClientPlayer.MyHandArea) //脱手即出牌
            {
                summonRetinue(dragLastPosition, Const.TARGET_RETINUE_SELECT_NONE);
            }
            else
            {
                ClientPlayer.MyBattleGroundManager.RemoveRetinuePreview();
                transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
                ClientPlayer.MyHandManager.RefreshCardsPlace();
            }
        }
        else
        {
            if (boardAreaType != ClientPlayer.MyHandArea) //脱手即假召唤，开始展示指定目标箭头
            {
                summonRetinueTarget(dragLastPosition, CardInfo.TargetInfo.targetRetinueRange);
            }
            else
            {
                ClientPlayer.MyBattleGroundManager.RemoveRetinuePreview();
                transform.SetPositionAndRotation(dragBeginPosition, dragBeginQuaternion); //如果脱手地方还在手中，则收回
                ClientPlayer.MyHandManager.RefreshCardsPlace();
            }
        }
    }

    #region 卡牌效果

    //召唤机甲
    private void summonRetinue(Vector3 dragLastPosition, int targetRetinueId)
    {
        if (ClientPlayer.MyBattleGroundManager.BattleGroundIsFull)
        {
            ClientPlayer.MyHandManager.RefreshCardsPlace();
            return;
        }

        int aliveIndex = ClientPlayer.MyBattleGroundManager.ComputePositionInAliveRetinues(dragLastPosition);
        SummonRetinueRequest request = new SummonRetinueRequest(Client.Instance.Proxy.ClientId, M_CardInstanceId, aliveIndex, targetRetinueId, false, Const.CLIENT_TEMP_RETINUE_ID_NORMAL);
        Client.Instance.Proxy.SendMessage(request);
        Usable = false;
    }

    //召唤机甲带目标
    private void summonRetinueTarget(Vector3 dragLastPosition, TargetSideEffect.TargetRange targetRange)
    {
        if (ClientPlayer.MyBattleGroundManager.BattleGroundIsFull)
        {
            ClientPlayer.MyHandManager.RefreshCardsPlace();
            return;
        }

        ClientPlayer.MyHandManager.SetCurrentSummonRetinuePreviewCard(this);

        if (ClientPlayer.MyBattleGroundManager.BattleGroundIsEmpty) //场上为空直接召唤
        {
            summonRetinue(dragLastPosition, Const.TARGET_RETINUE_SELECT_NONE);
        }
        else //预览
        {
            int battleGroundIndex = ClientPlayer.MyBattleGroundManager.ComputePosition(dragLastPosition);
            ClientPlayer.MyBattleGroundManager.SummonRetinuePreview(this, battleGroundIndex, targetRange);
        }
    }

    #endregion
}