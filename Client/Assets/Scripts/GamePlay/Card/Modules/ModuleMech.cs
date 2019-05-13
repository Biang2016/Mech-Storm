using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class ModuleMech : ModuleBase
{
    private float MECH_DEFAULT_SIZE = 0.17f;

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

        M_ClientTempMechID = -1;
        base.PoolRecycle();
        ResetMech();
    }

    #region 各模块、自身数值与初始化

    private int mechOrder;

    public int MechOrder
    {
        get { return mechOrder; }
        set
        {
            if (mechOrder != value)
            {
                foreach (KeyValuePair<MechComponentTypes, MechComponentBase> kv in MechComponents)
                {
                    if (kv.Value) kv.Value.CardOrder = value;
                }

                foreach (KeyValuePair<CardComponentTypes, CardComponentBase> kv in CardComponents)
                {
                    if (kv.Value) kv.Value.CardOrder = value;
                }

                mechOrder = value;
            }
        }
    }

    public Dictionary<CardComponentTypes, CardComponentBase> CardComponents = new Dictionary<CardComponentTypes, CardComponentBase>();
    public Dictionary<MechComponentTypes, MechComponentBase> MechComponents = new Dictionary<MechComponentTypes, MechComponentBase>();

    void Awake()
    {
        CardComponents.Add(CardComponentTypes.Back, CardBackComponent);
        CardComponents.Add(CardComponentTypes.Basic, CardBasicComponent);
        CardComponents.Add(CardComponentTypes.Desc, CardDescComponent);
        CardComponents.Add(CardComponentTypes.Slots, CardSlotsComponent);

        MechComponents.Add(MechComponentTypes.Life, MechLifeComponent);
        MechComponents.Add(MechComponentTypes.AttrShapes, MechAttrShapesComponent);
        MechComponents.Add(MechComponentTypes.BattleInfo, MechBattleInfoComponent);
        MechComponents.Add(MechComponentTypes.Bloom, MechBloomComponent);
        MechComponents.Add(MechComponentTypes.TargetPreviewArrows, MechTargetPreviewArrowsComponent);
        MechComponents.Add(MechComponentTypes.TriggerIcon, MechTriggerIconComponent);
        MechComponents.Add(MechComponentTypes.SwordShieldArmor, MechSwordShieldArmorComponent);
    }

    public Transform[] HitPoints;

    [SerializeField] private CardBackComponent CardBackComponent;
    [SerializeField] private CardBasicComponent CardBasicComponent;
    [SerializeField] private CardDescComponent CardDescComponent;
    public CardSlotsComponent CardSlotsComponent;
    public MechLifeComponent MechLifeComponent;
    [SerializeField] private MechAttrShapesComponent MechAttrShapesComponent;
    [SerializeField] private MechBattleInfoComponent MechBattleInfoComponent;
    [SerializeField] private MechBloomComponent MechBloomComponent;
    public MechTargetPreviewArrowsComponent MechTargetPreviewArrowsComponent;
    [SerializeField] private MechTriggerIconComponent MechTriggerIconComponent;
    public MechSwordShieldArmorComponent MechSwordShieldArmorComponent;

    public bool IsInitializing = false;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        transform.localScale = Vector3.one * MECH_DEFAULT_SIZE;

        IsInitializing = true;
        base.Initiate(cardInfo, clientPlayer);
        InitializeComponents();

        M_MechLeftLife = cardInfo.LifeInfo.Life;
        M_MechTotalLife = cardInfo.LifeInfo.TotalLife;
        M_MechWeaponEnergy = 0;
        M_MechWeaponEnergyMax = 0;
        M_MechAttack = cardInfo.BattleInfo.BasicAttack;
        M_MechArmor = cardInfo.BattleInfo.BasicArmor;
        M_MechShield = cardInfo.BattleInfo.BasicShield;

        IsInitializing = false;

        M_ClientTempMechID = -1;

        CanAttack = false;

        IsDead = false;
    }

    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        CardDescComponent.SetCardName(CardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()]);
        CardBasicComponent.ChangePicture(CardInfo.BaseInfo.PictureID);
        CardSlotsComponent.SetSlot(ClientPlayer, this, CardInfo.MechInfo);

        MechLifeComponent.Initialize(this);
        MechAttrShapesComponent.Initialize(this);
        MechBattleInfoComponent.Initialize(this);
        MechBloomComponent.Initialize(this);
        MechTargetPreviewArrowsComponent.Initialize(this);
        MechTriggerIconComponent.Initialize(this);
        MechSwordShieldArmorComponent.Initialize(this);
    }

    private void ResetMech()
    {
        m_MechLeftLife = 0;
        m_MechTotalLife = 0;
        m_MechAttack = 0;
        m_MechArmor = 0;
        m_MechShield = 0;
        m_MechWeaponEnergy = 0;
        m_MechWeaponEnergyMax = 0;
        InitializeComponents();
    }

    protected override void ChangeColor(Color color)
    {
        CardBasicComponent.SetMainBoardColor(color, CardInfo.GetCardColorIntensity());
    }

    public int M_MechID { get; set; }

    public enum MechID
    {
        Empty = -1
    }

    public int M_ClientTempMechID { get; set; }

    private int m_ImmuneLeftRounds = 0;

    public int M_ImmuneLeftRounds
    {
        get { return m_ImmuneLeftRounds; }
        set
        {
            m_ImmuneLeftRounds = value;
            MechBattleInfoComponent.SetValue(MechBattleInfoIcon.IconTypes.Immune, value);
        }
    }

    private int m_InactivityRounds = 0;

    public int M_InactivityRounds
    {
        get { return m_InactivityRounds; }
        set
        {
            m_InactivityRounds = value;
            MechBattleInfoComponent.SetValue(MechBattleInfoIcon.IconTypes.Inactivity, value);
        }
    }

    private int m_MechLeftLife;

    public int M_MechLeftLife
    {
        get { return m_MechLeftLife; }
        set
        {
            if (m_MechLeftLife != value || IsInitializing)
            {
                MechLifeComponent.LifeChange(value, m_MechTotalLife, m_MechLeftLife, IsInitializing);
                m_MechLeftLife = value;
            }
        }
    }

    private int m_MechTotalLife;

    public int M_MechTotalLife
    {
        get { return m_MechTotalLife; }
        set
        {
            if (m_MechTotalLife != value || IsInitializing)
            {
                MechLifeComponent.TotalLifeChange(m_MechLeftLife, value, m_MechTotalLife, IsInitializing);
                m_MechTotalLife = value;
            }
        }
    }

    private int m_MechAttack;

    public int M_MechAttack
    {
        get { return m_MechAttack; }
        set
        {
            if (m_MechAttack != value || IsInitializing)
            {
                MechSwordShieldArmorComponent.AttackChange(value, CalculateFinalAttack(value, M_MechWeaponEnergy));
                m_MechAttack = value;
                if (M_Weapon)
                {
                    M_Weapon.M_WeaponAttack = value;
                }
            }
        }
    }

    private int m_MechWeaponEnergy;

    public int M_MechWeaponEnergy
    {
        get { return m_MechWeaponEnergy; }
        set
        {
            if (m_MechWeaponEnergy != value || IsInitializing)
            {
                MechSwordShieldArmorComponent.AttackChange(value, CalculateFinalAttack(m_MechAttack, value));
                m_MechWeaponEnergy = value;
                if (M_Weapon)
                {
                    M_Weapon.M_WeaponEnergy = value;
                }
            }
        }
    }

    private int m_MechWeaponEnergyMax;

    public int M_MechWeaponEnergyMax
    {
        get { return m_MechWeaponEnergyMax; }
        set
        {
            if (m_MechWeaponEnergyMax != value || IsInitializing)
            {
                MechSwordShieldArmorComponent.WeaponEnergyMaxChange(value);
                m_MechWeaponEnergyMax = value;
                if (M_Weapon)
                {
                    M_Weapon.M_WeaponEnergyMax = value;
                }
            }
        }
    }

    public int M_MechWeaponFinalAttack => CalculateFinalAttack(m_MechAttack, m_MechWeaponEnergy);

    public static int CalculateFinalAttack(int attack, int energy)
    {
        if (energy == 0)
        {
            return attack;
        }
        else
        {
            return energy * attack;
        }
    }

    private int m_MechArmor;

    public int M_MechArmor
    {
        get { return m_MechArmor; }
        set
        {
            if (m_MechArmor != value || IsInitializing)
            {
                MechSwordShieldArmorComponent.ArmorChange(value, m_MechArmor, IsInitializing);
                m_MechArmor = value;
                if (M_Shield)
                {
                    M_Shield.M_ShieldArmor = value;
                }
            }
        }
    }

    private int m_MechShield;

    public int M_MechShield
    {
        get { return m_MechShield; }
        set
        {
            if (m_MechShield != value || IsInitializing)
            {
                MechSwordShieldArmorComponent.ShieldChange(value, m_MechShield, IsInitializing);
                m_MechShield = value;
                if (M_Shield)
                {
                    M_Shield.M_ShieldShield = value;
                }
            }
        }
    }

    #endregion

    #region MechAttr

    public bool IsFrenzy => CardInfo.MechInfo.IsFrenzy ||
                            (M_Weapon != null && M_Weapon.CardInfo.WeaponInfo.IsFrenzy) ||
                            (M_Pack != null && M_Pack.CardInfo.PackInfo.IsFrenzy) ||
                            (M_MA != null && M_MA.CardInfo.MAInfo.IsFrenzy);

    public bool IsSentry => (M_Weapon != null && M_Weapon.CardInfo.WeaponInfo.IsSentry);

    public bool IsSniper => CardInfo.MechInfo.IsSniper ||
                            (M_Pack != null && M_Pack.CardInfo.PackInfo.IsSniper) ||
                            (M_MA != null && M_MA.CardInfo.MAInfo.IsSniper);

    public bool IsDefender => CardInfo.MechInfo.IsDefense ||
                              (M_Shield != null && M_Shield.CardInfo.ShieldInfo.IsDefense) ||
                              (M_Pack != null && M_Pack.CardInfo.PackInfo.IsDefense) ||
                              (M_MA != null && M_MA.CardInfo.MAInfo.IsDefense);

    public int DodgeProp => M_Pack ? 0 : M_Pack.CardInfo.PackInfo.DodgeProp;

    #endregion

    #region 拼装上的模块

    internal bool IsAllEquipExceptMA
    {
        get
        {
            if (CardInfo.MechInfo.Slots[0] != SlotTypes.None && M_Weapon == null) return false;
            if (CardInfo.MechInfo.Slots[1] != SlotTypes.None && M_Shield == null) return false;
            if (CardInfo.MechInfo.Slots[2] != SlotTypes.None && M_Pack == null) return false;
            return true;
        }
    }

    #region 武器相关

    public ModuleEquip GetEquipBySlotType(SlotTypes slotType)
    {
        switch (slotType)
        {
            case SlotTypes.Weapon:
                return M_Weapon;
            case SlotTypes.Shield:
                return M_Shield;
            case SlotTypes.Pack:
                return M_Pack;
            case SlotTypes.MA:
                return M_MA;
        }

        return null;
    }

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
        }
    }

    private void On_WeaponDown()
    {
        AudioManager.Instance.SoundPlay("sfx/OnEquipDown");
        MechAttrShapesComponent.OnAttrShapeShow();
    }

    private void On_WeaponEquiped()
    {
        M_Weapon.OnWeaponEquiped();
        MechAttrShapesComponent.OnAttrShapeShow();
        AudioManager.Instance.SoundPlay("sfx/OnEquipWeapon");
    }

    private void On_WeaponChanged()
    {
        M_Weapon.OnWeaponEquiped();
        MechAttrShapesComponent.OnAttrShapeShow();
        AudioManager.Instance.SoundPlay("sfx/OnEquipWeapon");
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
        }
    }

    private void On_ShieldDown()
    {
        AudioManager.Instance.SoundPlay("sfx/OnEquipDown");
        MechAttrShapesComponent.OnAttrShapeShow();
    }

    private void On_ShieldEquiped()
    {
        M_Shield.OnShieldEquiped();
        MechAttrShapesComponent.OnAttrShapeShow();
        AudioManager.Instance.SoundPlay("sfx/OnEquipShield");
    }

    private void On_ShieldChanged()
    {
        M_Shield.OnShieldEquiped();
        MechAttrShapesComponent.OnAttrShapeShow();
        AudioManager.Instance.SoundPlay("sfx/OnEquipShield");
    }

    #endregion

    #region 飞行背包相关

    private ModulePack m_Pack;

    public ModulePack M_Pack
    {
        get { return m_Pack; }
        set
        {
            if (m_Pack && !value)
            {
                m_Pack.PoolRecycle();
                m_Pack = value;
                On_PackDown();
            }
            else if (!m_Pack && value)
            {
                m_Pack = value;
                On_PackEquiped();
            }
            else if (m_Pack != value)
            {
                m_Pack.PoolRecycle();
                m_Pack = value;
                On_PackChanged();
            }
        }
    }

    private void On_PackDown()
    {
        AudioManager.Instance.SoundPlay("sfx/OnEquipDown");
    }

    private void On_PackEquiped()
    {
        M_Pack.OnPackEquiped();
        AudioManager.Instance.SoundPlay("sfx/OnEquipPack");
    }

    private void On_PackChanged()
    {
        M_Pack.OnPackEquiped();
        AudioManager.Instance.SoundPlay("sfx/OnEquipPack");
    }

    #endregion

    #region MA相关

    private ModuleMA m_MA;

    public ModuleMA M_MA
    {
        get { return m_MA; }
        set
        {
            if (m_MA && !value)
            {
                m_MA.PoolRecycle();
                m_MA = value;
                On_MADown();
            }
            else if (!m_MA && value)
            {
                m_MA = value;
                On_MAEquiped();
            }
            else if (m_MA != value)
            {
                m_MA.PoolRecycle();
                m_MA = value;
                On_MAChanged();
            }
        }
    }

    private void On_MADown()
    {
        AudioManager.Instance.SoundPlay("sfx/OnEquipDown");
        MechSwordShieldArmorComponent.MA_BG.SetActive(false);
    }

    private void On_MAEquiped()
    {
        M_MA.OnMAEquiped();
        MechSwordShieldArmorComponent.MA_BG.SetActive(true);
        AudioManager.Instance.SoundPlay("sfx/OnEquipMA");
    }

    private void On_MAChanged()
    {
        M_MA.OnMAEquiped();
        AudioManager.Instance.SoundPlay("sfx/OnEquipMA");
    }

    #endregion

    #endregion

    #region 模块交互

    #region 攻击

    public void SetCanAttack(bool value)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_SetCanAttack(value), "Co_SetCanAttack");
    }

    IEnumerator Co_SetCanAttack(bool value)
    {
        CanAttack = value;
        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private bool canAttack;

    public bool CanAttack
    {
        get { return canAttack; }
        set
        {
            if (!RoundManager.Instance.InRound)
            {
                value = false;
            }

            canAttack = value;
            MechBloomComponent.SetCanAttackBloomShow(canAttack);
        }
    }

    private enum AttackLevel
    {
        Sword = 0,
        Gun = 1,
        SniperGun = 2
    }

    private AttackLevel M_AttackLevel
    {
        get
        {
            if (M_Weapon == null || M_MechWeaponEnergy == 0) return AttackLevel.Sword;
            if (M_Weapon.M_WeaponType == WeaponTypes.Gun) return AttackLevel.Gun;
            if (M_Weapon.M_WeaponType == WeaponTypes.SniperGun) return AttackLevel.SniperGun;
            return AttackLevel.Sword;
        }
    }

    public void Attack(ModuleMech targetMech, bool isCounterAttack)
    {
        //BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_Attack(targetMech, isCounterAttack), "Co_Attack");
    }

    public void AttackShip(ClientPlayer ship)
    {
        //BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_AttackShip(ship), "Co_AttackShip");
    }

    public Vector3 GetClosestHitPos(Vector3 from)
    {
        float min_distance = 999;
        Vector3 closestHitPos = Vector3.zero;
        foreach (Transform hitPoint in HitPoints)
        {
            float dis = Vector3.Magnitude(hitPoint.position - from);
            if (dis < min_distance)
            {
                min_distance = dis;
                closestHitPos = hitPoint.position;
            }
        }

        return closestHitPos;
    }

    public void BeAttacked(int attackNumber)
    {
        OnBeAttacked();
    }

    public int CalculateAttack() //计算拖出攻击数值
    {
        if (M_MechWeaponEnergy != 0) return M_MechAttack * M_MechWeaponEnergy;
        else return M_MechAttack;
    }

    public int CalculateCounterAttack(ModuleMech targetMech) //计算对方反击数值
    {
        bool enemyUseGun = targetMech.M_Weapon && targetMech.M_Weapon.M_WeaponType == WeaponTypes.Gun && targetMech.M_MechWeaponEnergy != 0;

        int damage = 0;
        if (M_Weapon && M_MechWeaponEnergy != 0)
        {
            switch (M_Weapon.M_WeaponType)
            {
                case WeaponTypes.Sword:
                    if (enemyUseGun) damage = 0; //无远程武器不能反击枪械攻击
                    else
                    {
                        if (IsFrenzy)
                        {
                            damage = M_MechWeaponFinalAttack * 2;
                        }
                        else
                        {
                            damage = M_MechWeaponFinalAttack;
                        }
                    }

                    break;
                case WeaponTypes.Gun: //有远程武器可以反击，只反击一点子弹
                    if (IsFrenzy)
                    {
                        damage = M_MechAttack * 2;
                    }
                    else
                    {
                        damage = M_MechAttack;
                    }

                    break;
                case WeaponTypes.SniperGun: //狙击枪无法反击
                    damage = 0;
                    break;
            }
        }
        else //如果没有武器
        {
            if (enemyUseGun) damage = 0;
            else
            {
                if (IsFrenzy)
                {
                    damage = M_MechAttack * 2;
                }
                else
                {
                    damage = M_MechAttack;
                }
            }
        }

        return damage;
    }

    private bool targetPreviewArrowShow;

    public bool TargetPreviewArrowShow
    {
        get { return targetPreviewArrowShow; }

        set
        {
            if (value == targetPreviewArrowShow) return;
            if (value && !targetPreviewArrowShow)
            {
                MechTargetPreviewArrowsComponent.ShowDenfenderText(IsDefender);
                MechTargetPreviewArrowsComponent.TargetArrowAnimStart();
            }
            else if (!value && targetPreviewArrowShow)
            {
                MechTargetPreviewArrowsComponent.ShowDenfenderText(false);
                MechTargetPreviewArrowsComponent.TargetArrowAnimEnd();
            }

            targetPreviewArrowShow = value;
        }
    }

    public void ShowTargetPreviewArrow(bool beSniperTargeted = false)
    {
        TargetPreviewArrowShow = true;
        MechTargetPreviewArrowsComponent.ShowSniperTargetImage(beSniperTargeted && !IsDefender);
    }

    public void HideTargetPreviewArrow()
    {
        TargetPreviewArrowShow = false;
    }

    public void ShowSniperTipText(bool isShow)
    {
        MechTargetPreviewArrowsComponent.ShowSniperTargetImage(isShow);
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleMech moduleMech, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slots, moduleMech, ship, dragLastPosition, dragBeginPosition, dragBeginQuaternion);
        RoundManager.Instance.HideTargetPreviewArrow();
        if (moduleMech)
        {
            if (moduleMech.CheckModuleMechCanAttackMe(this))
            {
                MechAttackMechRequest request = new MechAttackMechRequest(ClientPlayer.ClientId, M_MechID, RoundManager.Instance.EnemyClientPlayer.ClientId, moduleMech.M_MechID);
                Client.Instance.Proxy.SendMessage(request);
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("ModuleMech_ShouldAttackDefenderFirst"), 0, 0.5f);
            }
        }
        else if (ship)
        {
            if (ship.CheckModuleMechCanAttackMe(this) != 0)
            {
                MechAttackShipRequest request = new MechAttackShipRequest(Client.Instance.Proxy.ClientId, M_MechID);
                Client.Instance.Proxy.SendMessage(request);
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("ModuleMech_ShouldAttackMechsFirst"), 0, 0.5f);
            }
        }

        DragManager.Instance.DragOutDamage = 0;
    }

    public override void DragComponent_SetStates(ref bool canDrag, ref DragPurpose dragPurpose)
    {
        canDrag = CanAttack && ClientPlayer == RoundManager.Instance.CurrentClientPlayer && ClientPlayer == RoundManager.Instance.SelfClientPlayer && !ClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechs.Contains(this);
        dragPurpose = DragPurpose.Target;
    }

    public override float DragComponent_DragDistance()
    {
        return 0.2f;
    }

    public override void DragComponent_DragOutEffects()
    {
        base.DragComponent_DragOutEffects();
        DragManager.Instance.DragOutDamage = CalculateAttack();
        RoundManager.Instance.ShowMechAttackPreviewArrow(this);
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
            if (!ClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechs.Contains(this))
            {
                MechBloomComponent.SetOnHoverBloomColor(ClientUtils.GetColorFromColorDict(AllColors.ColorType.MechOnEnemyHoverBloomColor));
                MechBloomComponent.SetOnHoverBloomShow(value);
            }
        }
    }

    public override void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
        base.MouseHoverComponent_OnMousePressEnterImmediately(mousePosition);
        if (DragManager.Instance.CurrentDrag)
        {
            ModuleMech mr = DragManager.Instance.CurrentDrag_ModuleMech;
            CardSpell cs = DragManager.Instance.CurrentDrag_CardSpell;
            if ((mr != null && CheckModuleMechCanAttackMe(mr)) || (cs != null && CheckCardSpellCanTarget(cs)))
            {
                IsBeDraggedHover = true;
                if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                {
                    ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                }
            }
        }

        MechTargetPreviewArrowsComponent.OnMousePressEnterImmediately();
        MechAttrShapesComponent.OnMousePressEnterImmediately();
    }

    public bool CheckModuleMechCanAttackMe(ModuleMech attackMech)
    {
        if (attackMech == this) return false;
        if (attackMech.ClientPlayer == ClientPlayer) return false;
        if (RoundManager.Instance.EnemyClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechs.Contains(this)) return false;
        if (attackMech.M_Weapon && attackMech.M_Weapon.M_WeaponType == WeaponTypes.SniperGun && attackMech.M_MechWeaponEnergy != 0) return true; //狙击枪可以越过嘲讽机甲，其他武器只能攻击嘲讽机甲
        if (ClientPlayer.BattlePlayer.BattleGroundManager.HasDefenceMech && !IsDefender) return false;
        return true;
    }

    public bool CheckCardSpellCanTarget(CardSpell card)
    {
        if (card.ClientPlayer == ClientPlayer)
        {
            return ((card.CardInfo.TargetInfo.targetMechRange & TargetRange.SelfSoldiers) == TargetRange.SelfSoldiers && CardInfo.MechInfo.IsSoldier) ||
                   ((card.CardInfo.TargetInfo.targetMechRange & TargetRange.SelfHeroes) == TargetRange.SelfHeroes && !CardInfo.MechInfo.IsSoldier);
        }
        else
        {
            return ((card.CardInfo.TargetInfo.targetMechRange & TargetRange.EnemySoldiers) == TargetRange.EnemySoldiers && CardInfo.MechInfo.IsSoldier) ||
                   ((card.CardInfo.TargetInfo.targetMechRange & TargetRange.EnemyHeroes) == TargetRange.EnemyHeroes && !CardInfo.MechInfo.IsSoldier);
        }
    }

    public override void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
        base.MouseHoverComponent_OnMousePressLeaveImmediately();
        IsBeDraggedHover = false;

        if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
        {
            ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = false; //箭头动画
        }

        MechAttrShapesComponent.OnAttrShapeShow();
        MechTargetPreviewArrowsComponent.SetDamagePreviewText("");

        if (DragManager.Instance.CurrentDrag)
        {
            ModuleMech mr = DragManager.Instance.CurrentDrag_ModuleMech;
            if (mr != null)
            {
                mr.MechTargetPreviewArrowsComponent.SetDamagePreviewText("");
            }
        }
    }

    #endregion

    #region 其他鼠标Hover效果

    private bool isBeHover = false;

    public bool IsBeHover
    {
        get { return isBeHover; }

        set
        {
            isBeHover = value;
            if (!ClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechs.Contains(this))
            {
                if (ClientPlayer == RoundManager.Instance.EnemyClientPlayer)
                {
                    MechBloomComponent.SetOnHoverBloomColor(ClientUtils.GetColorFromColorDict(AllColors.ColorType.MechOnEnemyHoverBloomColor));
                }
                else
                {
                    MechBloomComponent.SetOnHoverBloomColor(ClientUtils.GetColorFromColorDict(AllColors.ColorType.MechOnSelfHoverBloomColor));
                }

                MechBloomComponent.SetOnHoverBloomShow(value);
            }
        }
    }

    public override void MouseHoverComponent_OnHover1Begin(Vector3 mousePosition)
    {
        base.MouseHoverComponent_OnHover1Begin(mousePosition);
        if (DragManager.Instance.IsSummonPreview)
        {
            TargetRange targetRange = DragManager.Instance.SummonMechTargetRange;
            if ((ClientPlayer == RoundManager.Instance.EnemyClientPlayer &&
                 (targetRange == TargetRange.EnemyMechs ||
                  (targetRange == TargetRange.EnemySoldiers && CardInfo.MechInfo.IsSoldier) ||
                  targetRange == TargetRange.EnemyHeroes && !CardInfo.MechInfo.IsSoldier))
                ||
                ClientPlayer == RoundManager.Instance.SelfClientPlayer && ClientPlayer.BattlePlayer.BattleGroundManager.CurrentSummonPreviewMech != this &&
                (targetRange == TargetRange.SelfMechs || (targetRange == TargetRange.SelfSoldiers && CardInfo.MechInfo.IsSoldier)))
            {
                IsBeHover = true;
                if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                {
                    ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                }
            }
        }
    }

    public override void MouseHoverComponent_OnHover1End()
    {
        base.MouseHoverComponent_OnHover1End();
        IsBeHover = false;
        if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
        {
            ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = false; //箭头动画
        }
    }

    #endregion

    #endregion

    #region 副作用

    public override void OnShowEffects(SideEffectExecute.TriggerTime triggerTime, SideEffectExecute.TriggerRange triggerRange)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShowSideEffectBloom(triggerTime, triggerRange, ClientUtils.HTMLColorToColor("#00FFDA"), 0.4f), "ShowSideEffectBloom");
    }

    IEnumerator Co_ShowSideEffectBloom(SideEffectExecute.TriggerTime triggerTime, SideEffectExecute.TriggerRange triggerRange, Color color, float duration)
    {
        if (triggerTime == SideEffectExecute.TriggerTime.OnMechDie && triggerRange == SideEffectExecute.TriggerRange.Self)
        {
            MechTriggerIconComponent.IconJump(MechTriggerIcon.IconTypes.Die);
        }
        else
        {
            MechTriggerIconComponent.IconJump(MechTriggerIcon.IconTypes.Trigger);
        }

        MechBloomComponent.SetSideEffectTriggerBloomShow(true);
        MechBloomComponent.SetSideEffectTriggerBloomColor(color, 2);
        AudioManager.Instance.SoundPlay("sfx/OnSE");
        yield return new WaitForSeconds(duration);
        MechBloomComponent.SetSideEffectTriggerBloomShow(false);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void OnSummon()
    {
        AudioManager.Instance.SoundPlay("sfx/OnSummonMech");
    }

    public void OnDie()
    {
        if (IsDead) return;
        IsDead = true;
    }

    public void OnAttack(WeaponTypes weaponType, ModuleMech targetMech)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_OnAttack(weaponType, targetMech), "Co_OnAttack");
    }

    IEnumerator Co_OnAttack(WeaponTypes weaponType, ModuleMech targetMech)
    {
        switch (weaponType)
        {
            case WeaponTypes.None:
            {
                Vector3 oriPos = transform.position;
//                iTween.MoveTo(gameObject, targetMech.GetClosestHitPos(transform.position), 0.1f);
//                yield return new WaitForSeconds(0.15f);
//                AudioManager.Instance.SoundPlay("sfx/AttackSword");
//                iTween.MoveTo(gameObject, oriPos, 0.1f);
                break;
            }
            case WeaponTypes.Sword:
            {
                Vector3 oriPos = transform.position;
//                iTween.MoveTo(gameObject, targetMech.GetClosestHitPos(transform.position), 0.1f);
//                yield return new WaitForSeconds(0.15f);
//                AudioManager.Instance.SoundPlay("sfx/AttackSword");
//                iTween.MoveTo(gameObject, oriPos, 0.1f);
                break;
            }
            case WeaponTypes.Gun:
            {
                Bullet bl = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Bullet].AllocateGameObject<Bullet>(transform);
                bl.Move(transform.position, targetMech.transform.position, Color.white, Color.white, 1, 0.03f, 0.1f, 0);
                AudioManager.Instance.SoundPlay("sfx/AttackGun");
                break;
            }
            case WeaponTypes.SniperGun:
            {
                Bullet bl = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Bullet].AllocateGameObject<Bullet>(transform);
                bl.Move(transform.position, targetMech.transform.position, Color.yellow, Color.yellow, 1, 0.03f, 0.15f, 0);
                AudioManager.Instance.SoundPlay("sfx/AttackSniper");
                break;
            }
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void OnAttackShip(WeaponTypes weaponType, Ship targetShip)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_OnAttackShip(weaponType, targetShip), "Co_OnAttack");
    }

    IEnumerator Co_OnAttackShip(WeaponTypes weaponType, Ship targetShip)
    {
        switch (weaponType)
        {
            case WeaponTypes.None:
            {
                Vector3 oriPos = transform.position;
//                iTween.MoveTo(gameObject, targetShip.GetClosestHitPosition(transform.position), 0.15f);
//                yield return new WaitForSeconds(0.17f);
//                AudioManager.Instance.SoundPlay("sfx/AttackNone");
//                iTween.MoveTo(gameObject, oriPos, 0.15f);
//                yield return new WaitForSeconds(0.17f);
                break;
            }
            case WeaponTypes.Sword:
            {
                Vector3 oriPos = transform.position;
//                iTween.MoveTo(gameObject, targetShip.GetClosestHitPosition(transform.position), 0.15f);
//                yield return new WaitForSeconds(0.17f);
//                AudioManager.Instance.SoundPlay("sfx/AttackSword");
//                iTween.MoveTo(gameObject, oriPos, 0.15f);
//                yield return new WaitForSeconds(0.17f);
                break;
            }
            case WeaponTypes.Gun:
            {
                Bullet bl = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Bullet].AllocateGameObject<Bullet>(transform);
                bl.Move(transform.position, targetShip.transform.position, Color.white, Color.white, 1, 0.03f, 0.1f, 0);
                AudioManager.Instance.SoundPlay("sfx/AttackGun");
                break;
            }
            case WeaponTypes.SniperGun:
            {
                Bullet bl = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Bullet].AllocateGameObject<Bullet>(transform);
                bl.Move(transform.position, targetShip.transform.position, Color.yellow, Color.yellow, 1, 0.03f, 0.15f, 0);
                AudioManager.Instance.SoundPlay("sfx/AttackSniper");
                break;
            }
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
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
    }

    public void OnEndRound()
    {
        CanAttack = false;
    }

    #endregion
}