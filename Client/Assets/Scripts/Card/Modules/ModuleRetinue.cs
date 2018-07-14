using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class ModuleRetinue : ModuleBase
{
    public override void PoolRecycle()
    {
        if (M_Weapon)
        {
            M_Weapon.PoolRecycle();
            M_Weapon = null;
        }

        if (M_Shield)
        {
            M_Shield.PoolRecycle();
            M_Shield = null;
        }

        if (M_Pack)
        {
            M_Pack.PoolRecycle();
            M_Pack = null;
        }

        if (M_MA)
        {
            M_MA.PoolRecycle();
            M_MA = null;
        }

        base.PoolRecycle();
    }

    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.GOPM.Pool_ModuleRetinuePool;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    #region 各模块、自身数值与初始化

    public GameObject Star4;

    public override int Stars
    {
        get { return stars; }

        set
        {
            stars = value;
            switch (value)
            {
                case 0:
                    if (Star1) Star1.SetActive(false);
                    if (Star2) Star2.SetActive(false);
                    if (Star3) Star3.SetActive(false);
                    if (Star4) Star4.SetActive(false);
                    break;
                case 1:
                    if (Star1) Star1.SetActive(true);
                    if (Star2) Star2.SetActive(false);
                    if (Star3) Star3.SetActive(false);
                    if (Star4) Star4.SetActive(false);
                    break;
                case 2:
                    if (Star1) Star1.SetActive(false);
                    if (Star2) Star2.SetActive(true);
                    if (Star3) Star3.SetActive(false);
                    if (Star4) Star4.SetActive(false);
                    break;
                case 3:
                    if (Star1) Star1.SetActive(false);
                    if (Star2) Star2.SetActive(false);
                    if (Star3) Star3.SetActive(true);
                    if (Star4) Star4.SetActive(false);
                    break;
                case 4:
                    if (Star1) Star1.SetActive(false);
                    if (Star2) Star2.SetActive(false);
                    if (Star3) Star3.SetActive(false);
                    if (Star4) Star4.SetActive(true);
                    break;
                default: break;
            }
        }
    }

    public TextMesh TextMesh_RetinueName;
    public TextMesh TextMesh_RetinueDesc;

    public GameObject CardBloom;
    public GameObject CardDraggedHoverBloom;
    public Animator ShieldIconHit;
    public Animator ArmorIconHit;
    public Animator CardLifeHit;

    public SlotAnchor SlotAnchor1;
    public SlotAnchor SlotAnchor2;
    public SlotAnchor SlotAnchor3;
    public SlotAnchor SlotAnchor4;

    public GameObject Block_RetinueLeftLife;
    protected GameObject GoNumberSet_RetinueLeftLife;
    protected CardNumberSet CardNumberSet_RetinueLeftLife;

    public GameObject Block_RetinueTotalLife;
    protected GameObject GoNumberSet_RetinueTotalLife;
    protected CardNumberSet CardNumberSet_RetinueTotalLife;

    public GameObject Block_RetinueAttack;
    protected GameObject GoNumberSet_RetinueAttack;
    protected CardNumberSet CardNumberSet_RetinueAttack;

    public GameObject Block_RetinueShield;
    protected GameObject GoNumberSet_RetinueShield;
    protected CardNumberSet CardNumberSet_RetinueShield;

    public GameObject Block_RetinueArmor;
    protected GameObject GoNumberSet_RetinueArmor;
    protected CardNumberSet CardNumberSet_RetinueArmor;

    public Renderer PictureBoxRenderer;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        M_RetinueName = ((CardInfo_Retinue) cardInfo).CardName;
        M_RetinueDesc = ((CardInfo_Retinue) cardInfo).CardDesc;
        M_RetinueLeftLife = ((CardInfo_Retinue) cardInfo).Life;
        M_RetinueTotalLife = ((CardInfo_Retinue) cardInfo).Life;
        M_RetinueAttack = ((CardInfo_Retinue) cardInfo).BasicAttack;
        M_RetinueArmor = ((CardInfo_Retinue) cardInfo).BasicArmor;
        M_RetinueShield = ((CardInfo_Retinue) cardInfo).BasicShield;
        CardPictureManager.ChangePicture(PictureBoxRenderer, CardInfo.CardID);
        ChangeCardDragBloomColor(GameManager.GM.CardDragBloomColor);

        if (SlotAnchor1)
        {
            SlotAnchor1.M_Slot.ClientPlayer = ClientPlayer;
            SlotAnchor1.M_ModuleRetinue = this;
            SlotAnchor1.M_Slot.MSlotTypes = ((CardInfo_Retinue) cardInfo).Slot1;
        }

        if (SlotAnchor2)
        {
            SlotAnchor2.M_Slot.ClientPlayer = ClientPlayer;
            SlotAnchor2.M_ModuleRetinue = this;
            SlotAnchor2.M_Slot.MSlotTypes = ((CardInfo_Retinue) cardInfo).Slot2;
        }

        if (SlotAnchor3)
        {
            SlotAnchor3.M_Slot.ClientPlayer = ClientPlayer;
            SlotAnchor3.M_ModuleRetinue = this;
            SlotAnchor3.M_Slot.MSlotTypes = ((CardInfo_Retinue) cardInfo).Slot3;
        }

        if (SlotAnchor4)
        {
            SlotAnchor4.M_Slot.ClientPlayer = ClientPlayer;
            SlotAnchor4.M_ModuleRetinue = this;
            SlotAnchor4.M_Slot.MSlotTypes = ((CardInfo_Retinue) cardInfo).Slot4;
        }
    }


    private void ChangeCardDragBloomColor(Color color)
    {
        if (CardDraggedHoverBloom)
        {
            Renderer rd = CardDraggedHoverBloom.GetComponent<Renderer>();
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            rd.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", color);
            mpb.SetColor("_EmissionColor", color);
            rd.SetPropertyBlock(mpb);
        }
    }

    public override CardInfo_Base GetCurrentCardInfo()
    {
        return new CardInfo_Retinue(CardInfo.CardID, CardInfo.CardName, CardInfo.CardDesc, CardInfo.Cost, CardInfo.DragPurpose, CardInfo.CardType, CardInfo.CardColor, CardInfo.UpgradeID, CardInfo.CardLevel, M_RetinueLeftLife, M_RetinueTotalLife, M_RetinueAttack, M_RetinueShield, M_RetinueArmor, ((CardInfo_Retinue) CardInfo).Slot1, ((CardInfo_Retinue) CardInfo).Slot2, ((CardInfo_Retinue) CardInfo).Slot3, ((CardInfo_Retinue) CardInfo).Slot4);
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
            if (m_RetinueLeftLife > value) CardLifeHit.SetTrigger("BeHit");
            m_RetinueLeftLife = value;
            initiateNumbers(ref GoNumberSet_RetinueLeftLife, ref CardNumberSet_RetinueLeftLife, NumberSize.Big, CardNumberSet.TextAlign.Left, Block_RetinueLeftLife);
            CardNumberSet_RetinueLeftLife.Number = m_RetinueLeftLife;

            if (m_RetinueLeftLife <= 0)
            {
                ClientPlayer.MyBattleGroundManager.RemoveRetinue(this);
                ClientPlayer.MyBattleGroundManager.RefreshBattleGround();
                StartCoroutine(DelayPoolRecycle());
            }
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
            if (!M_Weapon)
            {
                initiateNumbers(ref GoNumberSet_RetinueAttack, ref CardNumberSet_RetinueAttack, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueAttack);
            }
            else
            {
                initiateNumbers(ref GoNumberSet_RetinueAttack, ref CardNumberSet_RetinueAttack, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueAttack, '+');
            }

            CardNumberSet_RetinueAttack.Number = m_RetinueAttack;
        }
    }

    private int m_RetinueArmor;

    public int M_RetinueArmor
    {
        get { return m_RetinueArmor; }
        set
        {
            if (m_RetinueArmor > value) ArmorIconHit.SetTrigger("BeHit");
            m_RetinueArmor = value;
            if (!M_Shield)
            {
                initiateNumbers(ref GoNumberSet_RetinueArmor, ref CardNumberSet_RetinueArmor, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueArmor);
            }
            else
            {
                initiateNumbers(ref GoNumberSet_RetinueArmor, ref CardNumberSet_RetinueArmor, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueArmor, '+');
            }

            CardNumberSet_RetinueArmor.Number = m_RetinueArmor;
        }
    }

    private int m_RetinueShield;

    public int M_RetinueShield
    {
        get { return m_RetinueShield; }
        set
        {
            if (m_RetinueShield > value) ShieldIconHit.SetTrigger("BeHit");

            m_RetinueShield = value;
            if (!M_Shield)
            {
                initiateNumbers(ref GoNumberSet_RetinueShield, ref CardNumberSet_RetinueShield, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueShield);
            }
            else
            {
                initiateNumbers(ref GoNumberSet_RetinueShield, ref CardNumberSet_RetinueShield, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueShield, '+');
            }

            CardNumberSet_RetinueShield.Number = m_RetinueShield;
        }
    }

    #endregion

    #region 拼装上的模块

    #region 武器相关

    private ModuleWeapon m_Weapon;

    public ModuleWeapon M_Weapon
    {
        get { return m_Weapon; }
        set
        {
            if (m_Weapon && !value)
            {
                On_WeaponDown();
            }
            else if (!m_Weapon && value)
            {
                On_WeaponEquiped();
            }
            else if (m_Weapon != value)
            {
                On_WeaponChanged(value);
                return;
            }

            m_Weapon = value;
        }
    }

    void On_WeaponDown()
    {
        initiateNumbers(ref GoNumberSet_RetinueAttack, ref CardNumberSet_RetinueAttack, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueAttack);
        CardNumberSet_RetinueAttack.Number = M_RetinueAttack;
    }

    void On_WeaponEquiped()
    {
        initiateNumbers(ref GoNumberSet_RetinueAttack, ref CardNumberSet_RetinueAttack, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueAttack, '+');
        CardNumberSet_RetinueAttack.Number = M_RetinueAttack;
    }

    void On_WeaponChanged(ModuleWeapon newWeapon)
    {
        initiateNumbers(ref GoNumberSet_RetinueAttack, ref CardNumberSet_RetinueAttack, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueAttack, '+');
        CardNumberSet_RetinueAttack.Number = ((CardInfo_Retinue) CardInfo).BasicAttack; //更换武器时机体基础攻击力恢复
        M_Weapon.ChangeWeapon(newWeapon, ref m_Weapon);
    }

    #endregion


    #region 防具相关

    private ModuleShield m_Shield;

    public ModuleShield M_Shield
    {
        get { return m_Shield; }
        set
        {
            if (m_Shield && !value)
            {
                On_ShieldDown();
            }
            else if (!m_Shield && value)
            {
                On_ShieldEquiped();
            }
            else if (m_Shield != value)
            {
                On_ShieldChanged(value);
                return;
            }

            m_Shield = value;
        }
    }

    void On_ShieldDown()
    {
        initiateNumbers(ref GoNumberSet_RetinueArmor, ref CardNumberSet_RetinueArmor, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueArmor);
        CardNumberSet_RetinueArmor.Number = M_RetinueArmor;
        initiateNumbers(ref GoNumberSet_RetinueShield, ref CardNumberSet_RetinueShield, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueShield);
        CardNumberSet_RetinueShield.Number = M_RetinueShield;
    }

    void On_ShieldEquiped()
    {
        initiateNumbers(ref GoNumberSet_RetinueArmor, ref CardNumberSet_RetinueArmor, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueArmor, '+');
        CardNumberSet_RetinueArmor.Number = M_RetinueArmor;
        initiateNumbers(ref GoNumberSet_RetinueShield, ref CardNumberSet_RetinueShield, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueShield, '+');
        CardNumberSet_RetinueShield.Number = M_RetinueShield;
    }

    void On_ShieldChanged(ModuleShield newShield) //更换防具时机体基础护甲护盾恢复
    {
        initiateNumbers(ref GoNumberSet_RetinueArmor, ref CardNumberSet_RetinueArmor, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueArmor);
        CardNumberSet_RetinueArmor.Number = ((CardInfo_Retinue) CardInfo).BasicArmor;
        initiateNumbers(ref GoNumberSet_RetinueShield, ref CardNumberSet_RetinueShield, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueShield);
        CardNumberSet_RetinueShield.Number = ((CardInfo_Retinue) CardInfo).BasicShield;
        M_Shield.ChangeShield(newShield, ref m_Shield);
    }

    #endregion


    internal ModuleShield M_Pack;
    internal ModuleShield M_MA;

    #endregion

    #region 模块交互

    #region 攻击

    private bool canAttack = false;

    internal bool CanAttack
    {
        get { return canAttack; }

        set
        {
            canAttack = value;
            CardBloom.SetActive(value && (GameManager.GM.CanTestEnemyCards || RoundManager.RM.SelfClientPlayer == RoundManager.RM.CurrentClientPlayer));
        }
    }


    public void OnBeginRound()
    {
        if (!M_Weapon && M_RetinueAttack != 0)
        {
            CanAttack = true;
        }

        if (M_Weapon && M_Weapon.M_WeaponAttack + M_RetinueAttack != 0)
        {
            CanAttack = true;
            M_Weapon.CanAttack = true;
        }
    }

    public void OnEndRound()
    {
        CanAttack = false;
        if (M_Weapon) M_Weapon.CanAttack = false;
    }

    public void AllModulesAttack()
    {
        if (M_Weapon)
        {
            M_Weapon.WeaponAttack();
        }

        CanAttack = false;
    }

    IEnumerator DelayPoolRecycle()
    {
        yield return new WaitForSeconds(0.5F);
        PoolRecycle();
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<SlotAnchor> slotAnchors, ModuleRetinue moduleRetinue, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slotAnchors, moduleRetinue, dragLastPosition, dragBeginPosition, dragBeginQuaternion);
        if (moduleRetinue && moduleRetinue.ClientPlayer != ClientPlayer)
        {
            RetinueAttackRetinueRequest request = new RetinueAttackRetinueRequest(Client.CS.Proxy.ClientId, ClientPlayer.ClientId, M_RetinuePlaceIndex, RoundManager.RM.EnemyClientPlayer.ClientId, moduleRetinue.M_RetinuePlaceIndex);
            Client.CS.Proxy.SendMessage(request);
        }
    }

    public override void DragComponent_SetStates(ref bool canDrag, ref DragPurpose dragPurpose)
    {
        canDrag = CanAttack;
        dragPurpose = DragPurpose.Attack;
    }

    public override float DragComponnet_DragDistance()
    {
        return 0.2f;
    }

    #endregion


    #region 被敌方拖动鼠标Hover

    private bool isBeDraggedHover = false;

    public bool IsBeDraggedHover
    {
        get { return isBeDraggedHover; }

        set
        {
            isBeDraggedHover = value;
            CardDraggedHoverBloom.SetActive(value);
        }
    }

    public override void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
        base.MouseHoverComponent_OnMousePressEnterImmediately(mousePosition);
        if (DragManager.DM.CurrentDrag)
        {
            ModuleRetinue mr = DragManager.DM.CurrentDrag.GetComponent<ModuleRetinue>();
            if (mr.ClientPlayer != ClientPlayer && mr != this)
            {
                IsBeDraggedHover = true;
                ((ArrowAiming) DragManager.DM.CurrentArrow).IsOnHover = true;
            }
        }
    }

    public override void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
        base.MouseHoverComponent_OnMousePressLeaveImmediately();
        IsBeDraggedHover = false;
        ((ArrowAiming) DragManager.DM.CurrentArrow).IsOnHover = false;
    }

    #endregion

    #endregion
}