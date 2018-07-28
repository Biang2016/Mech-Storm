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
        SwordMaskDefaultPosition = SwordBarMask.transform.localPosition;
        LifeBarMaskDefaultPosition = LifeBarMask.transform.localPosition;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void SetGoPool(GameObjectPool pool)
    {
        gameObjectPool = pool;
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

    public Renderer RetinueBloom;
    public Renderer OnHoverBloom;
    public Renderer SideEffcetBloom;
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

    public GameObject Block_RetinueWeaponEnergy;
    protected GameObject GoNumberSet_RetinueWeaponEnergy;
    protected CardNumberSet CardNumberSet_RetinueWeaponEnergy;

    public GameObject Block_RetinueWeaponEnergyMax;
    protected GameObject GoNumberSet_RetinueWeaponEnergyMax;
    protected CardNumberSet CardNumberSet_RetinueWeaponEnergyMax;

    public GameObject Block_RetinueShield;
    protected GameObject GoNumberSet_RetinueShield;
    protected CardNumberSet CardNumberSet_RetinueShield;

    public GameObject Block_RetinueArmor;
    protected GameObject GoNumberSet_RetinueArmor;
    protected CardNumberSet CardNumberSet_RetinueArmor;

    public Renderer PictureBoxRenderer;

    public GameObject ArmorFill;
    public GameObject ShieldBar;
    public GameObject SwordBar;
    public GameObject SwordBarMask;
    private float SwordMaskFullOffset = 0.451f;
    private Vector3 SwordMaskDefaultPosition;
    public GameObject LifeBarMask;
    private float LifeBarMaskFullOffset = 1f;
    private Vector3 LifeBarMaskDefaultPosition;

    private bool isInitializing = false;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        isInitializing = true;
        base.Initiate(cardInfo, clientPlayer);
        M_RetinueName = cardInfo.BaseInfo.CardName;
        M_RetinueLeftLife = cardInfo.LifeInfo.Life;
        M_RetinueTotalLife = cardInfo.LifeInfo.TotalLife;
        M_RetinueAttack = cardInfo.BattleInfo.BasicAttack;
        M_RetinueArmor = cardInfo.BattleInfo.BasicArmor;
        M_RetinueShield = cardInfo.BattleInfo.BasicShield;
        M_RetinueWeaponEnergy = 0;
        M_RetinueWeaponEnergyMax = 0;
        CardPictureManager.ChangePicture(PictureBoxRenderer, CardInfo.CardID);
        ChangeBloomColor(OnHoverBloom, GameManager.GM.RetinueOnHoverBloomColor);
        ChangeBloomColor(RetinueBloom, GameManager.GM.RetinueBloomColor);

        if (SlotAnchor1)
        {
            SlotAnchor1.M_Slot.ClientPlayer = ClientPlayer;
            SlotAnchor1.M_ModuleRetinue = this;
            SlotAnchor1.M_Slot.MSlotTypes = cardInfo.SlotInfo.Slot1;
        }

        if (SlotAnchor2)
        {
            SlotAnchor2.M_Slot.ClientPlayer = ClientPlayer;
            SlotAnchor2.M_ModuleRetinue = this;
            SlotAnchor2.M_Slot.MSlotTypes = cardInfo.SlotInfo.Slot2;
        }

        if (SlotAnchor3)
        {
            SlotAnchor3.M_Slot.ClientPlayer = ClientPlayer;
            SlotAnchor3.M_ModuleRetinue = this;
            SlotAnchor3.M_Slot.MSlotTypes = cardInfo.SlotInfo.Slot3;
        }

        if (SlotAnchor4)
        {
            SlotAnchor4.M_Slot.ClientPlayer = ClientPlayer;
            SlotAnchor4.M_ModuleRetinue = this;
            SlotAnchor4.M_Slot.MSlotTypes = cardInfo.SlotInfo.Slot4;
        }

        isInitializing = false;
        isFirstRound = true;
    }


    private void ChangeBloomColor(Renderer rd, Color color)
    {
        if (OnHoverBloom)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            rd.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", color);
            mpb.SetColor("_EmissionColor", color);
            rd.SetPropertyBlock(mpb);
        }
    }

    private int m_RetinueID;

    public int M_RetinueID
    {
        get { return m_RetinueID; }
        set { m_RetinueID = value; }
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

    private int m_RetinueLeftLife;

    public int M_RetinueLeftLife
    {
        get { return m_RetinueLeftLife; }
        set
        {
            if (!isInitializing && m_RetinueLeftLife > value) BattleEffectsManager.BEM.Effect_Main.EffectsShow(Co_LifeBeAttacked(), "Co_LifeBeAttacked");
            m_RetinueLeftLife = value;
            initiateNumbers(ref GoNumberSet_RetinueLeftLife, ref CardNumberSet_RetinueLeftLife, NumberSize.Big, CardNumberSet.TextAlign.Left, Block_RetinueLeftLife);
            CardNumberSet_RetinueLeftLife.Number = m_RetinueLeftLife;
            RefreshLifeBarMask();
        }
    }

    IEnumerator Co_LifeBeAttacked()
    {
        CardLifeHit.SetTrigger("BeHit");
        yield return new WaitForSeconds(0.1f);
        BattleEffectsManager.BEM.Effect_Main.EffectEnd();
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
            RefreshLifeBarMask();
        }
    }

    private void RefreshLifeBarMask()
    {
        if (M_RetinueTotalLife != 0)
        {
            LifeBarMask.transform.localPosition = LifeBarMaskDefaultPosition;
            LifeBarMask.transform.Translate(Vector3.left * LifeBarMaskFullOffset * 2 * M_RetinueLeftLife / M_RetinueTotalLife, Space.Self);
        }
        else
        {
            LifeBarMask.transform.localPosition = LifeBarMaskDefaultPosition;
        }
    }

    private int m_RetinueAttack;

    public int M_RetinueAttack
    {
        get { return m_RetinueAttack; }
        set
        {
            m_RetinueAttack = value;
            if (M_Weapon)
            {
                M_Weapon.M_WeaponAttack = value;
            }

            initiateNumbers(ref GoNumberSet_RetinueAttack, ref CardNumberSet_RetinueAttack, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueAttack);
            CardNumberSet_RetinueAttack.Number = m_RetinueAttack;
        }
    }

    private int m_RetinueWeaponEnergy;

    public int M_RetinueWeaponEnergy
    {
        get { return m_RetinueWeaponEnergy; }
        set
        {
            m_RetinueWeaponEnergy = value;
            if (M_Weapon)
            {
                M_Weapon.M_WeaponEnergy = value;
            }

            initiateNumbers(ref GoNumberSet_RetinueWeaponEnergy, ref CardNumberSet_RetinueWeaponEnergy, NumberSize.Medium, CardNumberSet.TextAlign.Left, Block_RetinueWeaponEnergy);
            CardNumberSet_RetinueWeaponEnergy.Number = m_RetinueWeaponEnergy;

            if (SwordBar)
            {
                if (m_RetinueWeaponEnergy == 0)
                {
                    SwordBar.SetActive(false);
                }
                else
                {
                    if (!SwordBar.activeSelf) SwordBar.SetActive(true);
                }
            }

            RefreshSwordBarMask();
        }
    }

    private int m_RetinueWeaponEnergyMax;

    public int M_RetinueWeaponEnergyMax
    {
        get { return m_RetinueWeaponEnergyMax; }
        set
        {
            if (M_Weapon)
            {
                M_Weapon.M_WeaponEnergyMax = value;
            }

            m_RetinueWeaponEnergyMax = value;
            initiateNumbers(ref GoNumberSet_RetinueWeaponEnergyMax, ref CardNumberSet_RetinueWeaponEnergyMax, NumberSize.Medium, CardNumberSet.TextAlign.Right, Block_RetinueWeaponEnergyMax, '/');
            CardNumberSet_RetinueWeaponEnergyMax.Number = m_RetinueWeaponEnergyMax;

            RefreshSwordBarMask();
        }
    }

    private void RefreshSwordBarMask()
    {
        if (M_RetinueWeaponEnergyMax != 0)
        {
            SwordBarMask.transform.localPosition = SwordMaskDefaultPosition;
            SwordBarMask.transform.Translate(Vector3.back * SwordMaskFullOffset * 2 * M_RetinueWeaponEnergy / M_RetinueWeaponEnergyMax, Space.Self);
        }
        else
        {
            SwordBarMask.transform.localPosition = SwordMaskDefaultPosition;
        }
    }

    private int m_RetinueArmor;

    public int M_RetinueArmor
    {
        get { return m_RetinueArmor; }
        set
        {
            if (!isInitializing && m_RetinueArmor > value) BattleEffectsManager.BEM.Effect_Main.EffectsShow(Co_ArmorBeAttacked(), "Co_ArmorBeAttacked");
            m_RetinueArmor = value;
            if (M_Shield)
            {
                M_Shield.M_ShieldArmor = value;
            }

            initiateNumbers(ref GoNumberSet_RetinueArmor, ref CardNumberSet_RetinueArmor, NumberSize.Medium, CardNumberSet.TextAlign.Center, Block_RetinueArmor);
            if (M_Shield)
            {
                CardNumberSet_RetinueArmor.Number = M_Shield.M_ShieldArmor;
            }
            else
            {
                CardNumberSet_RetinueArmor.Number = m_RetinueArmor;
            }

            if (m_RetinueArmor == 0)
            {
                if (ArmorFill) ArmorFill.SetActive(false);
            }
            else
            {
                if (ArmorFill && !ArmorFill.activeSelf) ArmorFill.SetActive(true);
            }
        }
    }

    IEnumerator Co_ArmorBeAttacked()
    {
        ArmorIconHit.SetTrigger("BeHit");
        yield return new WaitForSeconds(0.1f);
        BattleEffectsManager.BEM.Effect_Main.EffectEnd();
    }

    private int m_RetinueShield;

    public int M_RetinueShield
    {
        get { return m_RetinueShield; }
        set
        {
            if (!isInitializing && m_RetinueShield > value) BattleEffectsManager.BEM.Effect_Main.EffectsShow(Co_ShieldBeAttacked(), "Co_ShieldBeAttacked");
            m_RetinueShield = value;
            if (M_Shield)
            {
                M_Shield.M_ShieldShield = value;
            }

            initiateNumbers(ref GoNumberSet_RetinueShield, ref CardNumberSet_RetinueShield, NumberSize.Small, CardNumberSet.TextAlign.Center, Block_RetinueShield);
            if (M_Shield)
            {
                CardNumberSet_RetinueShield.Number = M_Shield.M_ShieldShield;
            }
            else
            {
                CardNumberSet_RetinueShield.Number = m_RetinueShield;
            }


            if (m_RetinueShield == 0)
            {
                if (ShieldBar) ShieldBar.SetActive(false);
            }
            else
            {
                if (ShieldBar && !ShieldBar.activeSelf) ShieldBar.SetActive(true);
            }
        }
    }

    IEnumerator Co_ShieldBeAttacked()
    {
        ShieldIconHit.SetTrigger("BeHit");
        yield return new WaitForSeconds(0.1f);
        BattleEffectsManager.BEM.Effect_Main.EffectEnd();
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
                m_Weapon.PoolRecycle();
                m_Weapon = value;
                On_WeaponDown();
            }
            else if (!m_Weapon && value)
            {
                m_Weapon = value;
                On_WeaponEquiped();
            }
            else if (m_Weapon != value)
            {
                m_Weapon.PoolRecycle();
                m_Weapon = value;
                On_WeaponChanged();
            }

            CheckCanAttack();
        }
    }

    void On_WeaponDown()
    {
    }

    void On_WeaponEquiped()
    {
    }

    void On_WeaponChanged()
    {
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
                m_Shield.PoolRecycle();
                m_Shield = value;
                On_ShieldDown();
            }
            else if (!m_Shield && value)
            {
                m_Shield = value;
                On_ShieldEquiped();
            }
            else if (m_Shield != value)
            {
                m_Shield.PoolRecycle();
                m_Shield = value;
                On_ShieldChanged();
            }

            CheckCanAttack();
        }
    }

    void On_ShieldDown()
    {
    }

    void On_ShieldEquiped()
    {
    }

    void On_ShieldChanged()
    {
    }

    #endregion


    internal ModuleShield M_Pack;
    internal ModuleShield M_MA;

    #endregion

    #region 模块交互

    #region 攻击

    public bool CanAttack_Self;
    public bool CanAttack_Weapon;
    public bool CanAttack_Shield;
    public bool CanAttack_Pack;
    public bool CanAttack_MA;

    public bool isFirstRound = true; //是否是召唤的第一回合

    private bool CheckCanAttack()
    {
        if (isFirstRound) return false;
        bool canAttack = false;
        if (CanAttack_Self && M_Weapon && CanAttack_Weapon && M_Weapon.M_WeaponAttack != 0 && M_RetinueWeaponEnergy > 0)
        {
            canAttack = true;
        }
        else if (M_Shield && CanAttack_Shield)
        {
        }
        else if (M_Pack && CanAttack_Pack)
        {
        }
        else if (M_MA && CanAttack_MA)
        {
        }

        RetinueBloom.gameObject.SetActive(canAttack && (GameManager.GM.CanTestEnemyCards || RoundManager.RM.SelfClientPlayer == RoundManager.RM.CurrentClientPlayer));
        return canAttack;
    }

    public void AllModulesAttack()
    {
        if (M_Weapon)
        {
            M_Weapon.WeaponAttack();
        }
        else
        {
            CanAttack_Self = false;
        }

        CheckCanAttack();
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
            RetinueAttackRetinueRequest request = new RetinueAttackRetinueRequest(Client.CS.Proxy.ClientId, ClientPlayer.ClientId, M_RetinueID, RoundManager.RM.EnemyClientPlayer.ClientId, moduleRetinue.M_RetinueID);
            Client.CS.Proxy.SendMessage(request);
        }
    }

    public override void DragComponent_SetStates(ref bool canDrag, ref DragPurpose dragPurpose)
    {
        canDrag = CheckCanAttack();
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
            OnHoverBloom.gameObject.SetActive(value);
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
                if (DragManager.DM.CurrentArrow && DragManager.DM.CurrentArrow is ArrowAiming)
                {
                    ((ArrowAiming) DragManager.DM.CurrentArrow).IsOnHover = true;
                }
            }
        }
    }

    public override void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
        base.MouseHoverComponent_OnMousePressLeaveImmediately();
        IsBeDraggedHover = false;
        if (DragManager.DM.CurrentArrow && DragManager.DM.CurrentArrow is ArrowAiming)
        {
            ((ArrowAiming) DragManager.DM.CurrentArrow).IsOnHover = false;
        }
    }

    #endregion

    #endregion

    #region 特效

    public void OnSummoned()
    {
        CheckCanAttack();
        foreach (SideEffectBase sideEffectBase in CardInfo.SideEffects_OnSummoned)
        {
            BattleEffectsManager.BEM.Effect_Main.EffectsShow(Co_ShowSideEffectBloom(GameManager.HTMLColorToColor("#64FFDB"), 1f), "ShowSideEffectBloom");
        }
    }

    public void OnDieSideEffects()
    {
        foreach (SideEffectBase sideEffectBase in CardInfo.SideEffects_OnDie)
        {
            BattleEffectsManager.BEM.Effect_Main.EffectsShow(Co_ShowSideEffectBloom(GameManager.HTMLColorToColor("#FFC609"), 1f), "ShowSideEffectBloom");
        }
    }

    IEnumerator Co_ShowSideEffectBloom(Color color, float duration)
    {
        SideEffcetBloom.gameObject.SetActive(true);
        ChangeBloomColor(SideEffcetBloom, color);
        yield return new WaitForSeconds(duration);
        SideEffcetBloom.gameObject.SetActive(false);
        BattleEffectsManager.BEM.Effect_Main.EffectEnd();
    }

    public void OnAttack()
    {
    }

    public void OnMakeDamage(int damage)
    {
    }

    public void OnBeAttacked()
    {
    }

    public void OnBeDamaged(int damage)
    {
    }

    public void OnBeginRound()
    {
        CanAttack_Self = true;
        CanAttack_Weapon = true;
        CanAttack_Shield = true;
        CanAttack_Pack = true;
        CanAttack_MA = true;
        CheckCanAttack();
    }

    public void OnEndRound()
    {
        isFirstRound = false;
    }

    #endregion
}