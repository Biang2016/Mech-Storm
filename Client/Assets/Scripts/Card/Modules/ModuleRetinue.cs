﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        ArmorFill.gameObject.SetActive(false);

        M_ClientTempRetinueID = -1;
        DamageNumberPreviewTextMesh.text = "";
        DamageNumberPreviewBGTextMesh.text = "";
        base.PoolRecycle();
    }

    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.Pool_ModuleRetinuePool;
        SwordMaskDefaultPosition = SwordBarMask.transform.localPosition;
        DamageNumberPreviewTextMesh.text = "";
        DamageNumberPreviewBGTextMesh.text = "";
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

    [SerializeField] private TextMesh TextMesh_RetinueName;

    [SerializeField] private Renderer RetinueCanAttackBloom;
    [SerializeField] private Renderer OnHoverBloom;
    [SerializeField] private Renderer SideEffcetBloom;
    [SerializeField] private Animator ShieldIconHit;
    [SerializeField] private Animator ArmorIconHit;
    [SerializeField] private Animator CardLifeHit;

    public Slot Slot1;
    public Slot Slot2;
    public Slot Slot3;
    public Slot Slot4;

    [SerializeField] private Text LifeText;

    [SerializeField] private Text Text_RetinueAttack;
    [SerializeField] private Text Text_RetinueShield;
    [SerializeField] private Text Text_RetinueArmor;

    [SerializeField] private Renderer PictureBoxRenderer;

    [SerializeField] private TextMesh DamageNumberPreviewTextMesh; //受攻击瞄准时的伤害预览
    [SerializeField] private TextMesh DamageNumberPreviewBGTextMesh; //受攻击瞄准时的伤害预览

    [SerializeField] private Animator ArmorFill;

    [SerializeField] private Animator ShieldBar;
    [SerializeField] private Image ShieldBarImage;

    [SerializeField] private GameObject SwordBar;
    [SerializeField] private GameObject SwordBarMask;
    private float SwordMaskFullOffset = 0.451f;
    private Vector3 SwordMaskDefaultPosition;

    [SerializeField] private Animator LifeIncreaseArrow;

    [SerializeField] private Animator RetinueTargetPreviewAnim;

    [SerializeField] private TextFlyPile LifeChangeNumberFly;
    [SerializeField] private TextFlyPile ArmorChangeNumberFly;
    [SerializeField] private TextFlyPile ShieldChangeNumberFly;
    [SerializeField] private TextFlyPile WeaponAttackChangeNumberFly;
    [SerializeField] private TextFlyPile WeaponEnergyChangeNumberFly;

    private bool isInitializing = false;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        isInitializing = true;
        base.Initiate(cardInfo, clientPlayer);
        Text_RetinueArmor.text = "";
        Text_RetinueShield.text = "";
        ArmorFill.gameObject.SetActive(false);
        ShieldBarImage.fillAmount = 0;
        M_RetinueName = cardInfo.BaseInfo.CardName;
        M_RetinueLeftLife = cardInfo.LifeInfo.Life;
        M_RetinueTotalLife = cardInfo.LifeInfo.TotalLife;
        M_RetinueAttack = cardInfo.BattleInfo.BasicAttack;
        M_RetinueArmor = cardInfo.BattleInfo.BasicArmor;
        M_RetinueShield = cardInfo.BattleInfo.BasicShield;
        M_RetinueWeaponEnergy = 0;
        M_RetinueWeaponEnergyMax = 0;
        ClientUtils.ChangePicture(PictureBoxRenderer, CardInfo.BaseInfo.PictureID);
        ClientUtils.ChangeColor(OnHoverBloom, GameManager.Instance.RetinueOnEnemyHoverBloomColor);
        ClientUtils.ChangeColor(RetinueCanAttackBloom, GameManager.Instance.RetinueBloomColor);

        if (Slot1)
        {
            Slot1.ClientPlayer = ClientPlayer;
            Slot1.M_ModuleRetinue = this;
            Slot1.MSlotTypes = cardInfo.SlotInfo.Slot1;
        }

        if (Slot2)
        {
            Slot2.ClientPlayer = ClientPlayer;
            Slot2.M_ModuleRetinue = this;
            Slot2.MSlotTypes = cardInfo.SlotInfo.Slot2;
        }

        if (Slot3)
        {
            Slot3.ClientPlayer = ClientPlayer;
            Slot3.M_ModuleRetinue = this;
            Slot3.MSlotTypes = cardInfo.SlotInfo.Slot3;
        }

        if (Slot4)
        {
            Slot4.ClientPlayer = ClientPlayer;
            Slot4.M_ModuleRetinue = this;
            Slot4.MSlotTypes = cardInfo.SlotInfo.Slot4;
        }

        isInitializing = false;

        isFirstRound = true;
        CannotAttackBecauseDie = false;
        CanAttackThisRound = false;
        M_ClientTempRetinueID = -1;

        IsDead = false;
    }

    private int m_RetinueID;

    public int M_RetinueID
    {
        get { return m_RetinueID; }
        set { m_RetinueID = value; }
    }

    public enum RetinueID
    {
        Empty = -1
    }

    private int m_ClientTempRetinueID;

    public int M_ClientTempRetinueID
    {
        get { return m_ClientTempRetinueID; }
        set { m_ClientTempRetinueID = value; }
    }

    public const int CLIENT_TEMP_RETINUE_ID_NORMAL = -1; //默认值，普通召唤的随从
    public const int CLIENT_TEMP_RETINUE_ID_SUMMON_PREVIEW_NOT_CONFIRM = -2; //预召唤随从上场但未选择目标

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

    private bool isDead;

    public bool IsDead
    {
        get { return isDead; }
        set { isDead = value; }
    }

    private int m_RetinueLeftLife;

    public int M_RetinueLeftLife
    {
        get { return m_RetinueLeftLife; }
        set
        {
            if (!isInitializing)
            {
                if (m_RetinueLeftLife > value) BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_LifeBeAttacked(value, m_RetinueTotalLife, m_RetinueLeftLife - value), "Co_LifeBeAttacked");
                else if (m_RetinueLeftLife < value) BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_LifeAdded(value, m_RetinueTotalLife, value - m_RetinueLeftLife), "Co_LifeAdded");
            }

            m_RetinueLeftLife = value;
        }
    }

    IEnumerator Co_LifeBeAttacked(int leftLifeValue, int totalLifeValue, int damage)
    {
        CardLifeHit.SetTrigger("BeHit");
        LifeChangeNumberFly.SetText("-" + damage, ClientUtils.HTMLColorToColor("#FF0A00"));
        retinueLifeChange(leftLifeValue, totalLifeValue);
        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    IEnumerator Co_LifeAdded(int leftLifeValue, int totalLifeValue, int addAmount)
    {
        LifeIncreaseArrow.SetTrigger("LifeAdd");

        LifeChangeNumberFly.SetText("+" + addAmount, ClientUtils.HTMLColorToColor("#92FF00"));
        retinueLifeChange(leftLifeValue, totalLifeValue);
        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private int m_RetinueTotalLife;

    public int M_RetinueTotalLife
    {
        get { return m_RetinueTotalLife; }
        set
        {
            m_RetinueTotalLife = value;
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_TotalLifeAdded(m_RetinueLeftLife, value), "Co_TotalLifeAdded");
        }
    }

    IEnumerator Co_TotalLifeAdded(int leftLifeValue, int totalLifeValue)
    {
        retinueLifeChange(leftLifeValue, totalLifeValue);
        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private void retinueLifeChange(int leftLifeValue, int totalLifeValue)
    {
        if (leftLifeValue < totalLifeValue)
        {
            LifeText.color = GameManager.Instance.InjuredLifeNumberColor;
        }
        else if (leftLifeValue == totalLifeValue && totalLifeValue > CardInfo.LifeInfo.TotalLife)
        {
            LifeText.color = GameManager.Instance.OverFlowTotalLifeColor;
        }
        else
        {
            LifeText.color = GameManager.Instance.DefaultLifeNumberColor;
        }

        LifeText.text = leftLifeValue.ToString();
    }

    private int m_RetinueAttack;

    public int M_RetinueAttack
    {
        get { return m_RetinueAttack; }
        set
        {
            int before = m_RetinueAttack;
            if (m_RetinueAttack != value)
            {
                m_RetinueAttack = value;
                if (M_Weapon)
                {
                    M_Weapon.M_WeaponAttack = value;
                }

                CheckCanAttack();
                BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RetinueAttackChange(m_RetinueAttack, value - before), "Co_RetinueAttackChange");
            }
        }
    }

    IEnumerator Co_RetinueAttackChange(int retinueAttackValue, int change)
    {
        Text_RetinueAttack.text = retinueAttackValue > 0 ? retinueAttackValue.ToString() : "";
        if (change > 0)
        {
            WeaponAttackChangeNumberFly.SetText("+" + change, ClientUtils.HTMLColorToColor("#92FF00"));
        }
        else
        {
            WeaponAttackChangeNumberFly.SetText(change.ToString(), ClientUtils.HTMLColorToColor("#FF0000"));
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private int m_RetinueWeaponEnergy;

    public int M_RetinueWeaponEnergy
    {
        get { return m_RetinueWeaponEnergy; }
        set
        {
            int before = m_RetinueWeaponEnergy;
            if (m_RetinueWeaponEnergy != value)
            {
                m_RetinueWeaponEnergy = value;
                if (M_Weapon)
                {
                    M_Weapon.M_WeaponEnergy = value;
                }

                BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RetinueWeaponEnergyChange(m_RetinueWeaponEnergy, m_RetinueWeaponEnergyMax, value - before), "Co_RetinueWeaponEnergyChange");
            }
        }
    }

    private int m_RetinueWeaponEnergyMax;

    public int M_RetinueWeaponEnergyMax
    {
        get { return m_RetinueWeaponEnergyMax; }
        set
        {
            int before = m_RetinueWeaponEnergyMax;
            if (m_RetinueWeaponEnergyMax != value)
            {
                m_RetinueWeaponEnergyMax = value;
                if (M_Weapon)
                {
                    M_Weapon.M_WeaponEnergyMax = value;
                }

                BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RetinueWeaponEnergyChange(m_RetinueWeaponEnergy, m_RetinueWeaponEnergyMax, value - before), "Co_RetinueWeaponEnergyChange");
            }
        }
    }

    IEnumerator Co_RetinueWeaponEnergyChange(int retinueWeaponEnergyValue, int retinueWeaponEnergyMaxValue, int change)
    {
        RefreshSwordBarMask(retinueWeaponEnergyValue, retinueWeaponEnergyMaxValue);
        if (change > 0)
        {
            WeaponEnergyChangeNumberFly.SetText("+" + change, ClientUtils.HTMLColorToColor("#92FF00"));
        }
        else
        {
            WeaponEnergyChangeNumberFly.SetText(change.ToString(), ClientUtils.HTMLColorToColor("#FF0000"));
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private void RefreshSwordBarMask(int retinueWeaponEnergyValue, int retinueWeaponEnergyMaxValue)
    {
        if (retinueWeaponEnergyMaxValue != 0)
        {
            SwordBarMask.transform.localPosition = SwordMaskDefaultPosition;
            SwordBarMask.transform.Translate(Vector3.back * SwordMaskFullOffset * 2 * retinueWeaponEnergyValue / retinueWeaponEnergyMaxValue, Space.Self);
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
            if (!isInitializing && m_RetinueArmor > value) BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ArmorBeAttacked(value, m_RetinueArmor - value), "Co_ArmorBeAttacked");
            else if (m_RetinueArmor < value) BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ArmorAdded(value, value - m_RetinueArmor), "Co_ArmorAdded");

            m_RetinueArmor = value;
            if (M_Shield)
            {
                M_Shield.M_ShieldArmor = value;
            }
        }
    }

    IEnumerator Co_ArmorBeAttacked(int armorValue, int change)
    {
        ArmorIconHit.SetTrigger("BeHit");
        ArmorFill.SetTrigger("ArmorAdd");

        ArmorChangeNumberFly.SetText("-" + change, ClientUtils.HTMLColorToColor("#FF1300"));

        if (armorValue == 0)
        {
            if (ArmorFill) ArmorFill.gameObject.SetActive(false);
            Text_RetinueArmor.text = "";
        }
        else
        {
            if (ArmorFill && !ArmorFill.gameObject.activeSelf) ArmorFill.gameObject.SetActive(true);
            Text_RetinueArmor.text = armorValue.ToString();
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    IEnumerator Co_ArmorAdded(int armorValue, int change)
    {
        ArmorFill.SetTrigger("ArmorAdd");

        ArmorChangeNumberFly.SetText("+" + change, ClientUtils.HTMLColorToColor("#FFDD00"));

        if (armorValue == 0)
        {
            if (ArmorFill) ArmorFill.gameObject.SetActive(false);
            Text_RetinueArmor.text = "";
        }
        else
        {
            if (ArmorFill && !ArmorFill.gameObject.activeSelf) ArmorFill.gameObject.SetActive(true);
            Text_RetinueArmor.text = armorValue.ToString();
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    [SerializeField] private int RetinueShieldFull;

    private int m_RetinueShield;

    public int M_RetinueShield
    {
        get { return m_RetinueShield; }
        set
        {
            if (!isInitializing && m_RetinueShield > value) BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShieldBeAttacked(value, m_RetinueShield - value), "Co_ShieldBeAttacked");
            else if (m_RetinueShield < value)
            {
                BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShieldAdded(value, value - m_RetinueShield), "Co_ShieldAdded");
            }

            m_RetinueShield = value;
            if (M_Shield)
            {
                M_Shield.M_ShieldShield = value;
            }
        }
    }

    IEnumerator Co_ShieldBeAttacked(int shieldValue, int change)
    {
        ShieldIconHit.SetTrigger("BeHit");
        ShieldBar.SetTrigger("ShieldAdd");

        ShieldChangeNumberFly.SetText("-" + change, ClientUtils.HTMLColorToColor("#FF1300"));

        if (shieldValue == 0)
        {
            if (ShieldBar) ShieldBarImage.fillAmount = 0;
            Text_RetinueShield.text = "";
        }
        else
        {
            if (ShieldBar) ShieldBarImage.fillAmount = (float) shieldValue / (float) RetinueShieldFull;
            Text_RetinueShield.text = shieldValue.ToString();
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    IEnumerator Co_ShieldAdded(int shieldValue, int change)
    {
        RetinueShieldFull = Mathf.Max(RetinueShieldFull, shieldValue);
        ShieldBar.SetTrigger("ShieldAdd");

        ShieldChangeNumberFly.SetText("+" + change, ClientUtils.HTMLColorToColor("#00FFF2"));

        if (shieldValue == 0)
        {
            if (ShieldBar) ShieldBarImage.fillAmount = 0;
            Text_RetinueShield.text = "";
        }
        else
        {
            if (ShieldBar) ShieldBarImage.fillAmount = 1;
            Text_RetinueShield.text = shieldValue.ToString();
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
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
        }
    }

    void On_WeaponDown()
    {
    }

    void On_WeaponEquiped()
    {
        M_Weapon.OnWeaponEquiped();
        CheckCanAttack();
    }

    void On_WeaponChanged()
    {
        M_Weapon.OnWeaponEquiped();
        CheckCanAttack();
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
        M_Shield.OnShieldEquiped();
    }

    void On_ShieldChanged()
    {
        M_Shield.OnShieldEquiped();
    }

    #endregion


    internal ModuleShield M_Pack;
    internal ModuleShield M_MA;

    #endregion

    #region 模块交互

    #region 攻击

    public bool isFirstRound = true; //是否是召唤的第一回合
    public bool CannotAttackBecauseDie = false; //是否已预先判定死亡
    public bool CanAttackThisRound; //本回合攻击
    public bool CanCharge = false; //冲锋
    public bool EndRound = false; //回合结束后

    private bool CheckCanAttack()
    {
        bool canAttack = true;
        canAttack &= RoundManager.Instance.CurrentClientPlayer == ClientPlayer;
        canAttack &= ClientPlayer == RoundManager.Instance.SelfClientPlayer;
        canAttack &= !isFirstRound || (isFirstRound && CanCharge);
        canAttack &= (!CannotAttackBecauseDie);
        canAttack &= (CanAttackThisRound);
        canAttack &= (M_RetinueAttack > 0);
        canAttack &= !EndRound;

        RetinueCanAttackBloom.gameObject.SetActive(canAttack);
        return canAttack;
    }

    public void Attack(ModuleRetinue targetRetinue, bool beHarm)
    {
        OnAttack(); //随从特效
        if (M_Weapon) M_Weapon.OnAttack(); //武器特效
        int damage;
        if (M_Weapon && M_RetinueWeaponEnergy != 0) //有武器避免反击
        {
            switch (M_Weapon.M_WeaponType)
            {
                case WeaponTypes.Sword:
                    damage = M_RetinueAttack * M_RetinueWeaponEnergy;
                    targetRetinue.BeAttacked(damage);
                    OnMakeDamage(damage);
                    if (M_RetinueWeaponEnergy < M_RetinueWeaponEnergyMax) M_RetinueWeaponEnergy++;
                    break;
                case WeaponTypes.Gun:
                    int tmp = M_RetinueWeaponEnergy;
                    for (int i = 0; i < tmp; i++)
                    {
                        targetRetinue.BeAttacked(M_RetinueAttack);
                        OnMakeDamage(M_RetinueAttack);
                        M_RetinueWeaponEnergy--;
                    }

                    break;
            }
        }
        else //如果没有武器，则受到反击
        {
            damage = M_RetinueAttack;
            targetRetinue.BeAttacked(damage);
            OnMakeDamage(damage);
            if (beHarm) targetRetinue.Attack(this, false); //攻击对方且接受对方反击
        }

        CanAttackThisRound = false;
        CheckCanAttack();
    }

    public void AttackShip(ClientPlayer ship)
    {
        OnAttack(); //随从特效
        if (M_Weapon) M_Weapon.OnAttack(); //武器特效
        int damage;
        if (M_Weapon && M_RetinueWeaponEnergy != 0) //有武器避免反击
        {
            switch (M_Weapon.M_WeaponType)
            {
                case WeaponTypes.Sword:
                    damage = M_RetinueAttack * M_RetinueWeaponEnergy;
                    OnMakeDamage(damage);
                    if (M_RetinueWeaponEnergy < M_RetinueWeaponEnergyMax) M_RetinueWeaponEnergy++;
                    break;
                case WeaponTypes.Gun:
                    int tmp = M_RetinueWeaponEnergy;
                    for (int i = 0; i < tmp; i++)
                    {
                        OnMakeDamage(M_RetinueAttack);
                        M_RetinueWeaponEnergy--;
                    }

                    break;
            }
        }
        else //如果没有武器
        {
            damage = M_RetinueAttack;
            OnMakeDamage(damage);
        }

        CanAttackThisRound = false;
        CheckCanAttack();
    }

    public void BeAttacked(int attackNumber) //攻击和被攻击仅发送伤害数值给客户端，具体计算分别处理(这里是被攻击，指的是攻击动作，不是掉血事件)
    {
        OnBeAttacked();
        int remainAttackNumber = attackNumber;

        if (M_RetinueShield > 0)
        {
            if (M_RetinueShield >= remainAttackNumber)
            {
                M_RetinueShield = M_RetinueShield - remainAttackNumber;
                remainAttackNumber = 0;
                return;
            }
            else
            {
                remainAttackNumber -= M_RetinueShield;
                M_RetinueShield = 0;
            }
        }

        if (M_RetinueArmor > 0)
        {
            if (M_RetinueArmor >= remainAttackNumber)
            {
                M_RetinueArmor = M_RetinueArmor - remainAttackNumber;
                remainAttackNumber = 0;
                return;
            }
            else
            {
                remainAttackNumber -= M_RetinueArmor;
                M_RetinueArmor = 0;
            }
        }

        if (M_RetinueLeftLife <= remainAttackNumber)
        {
            M_RetinueLeftLife -= M_RetinueLeftLife;
            remainAttackNumber -= M_RetinueLeftLife;
        }
        else
        {
            M_RetinueLeftLife -= remainAttackNumber;
            remainAttackNumber = 0;
            return;
        }
    }

    public int CalculateAttack()
    {
        if (M_RetinueWeaponEnergy != 0) return M_RetinueAttack * M_RetinueWeaponEnergy;
        else return M_RetinueAttack;
    }

    public void ShowTargetPreviewArrow()
    {
        RetinueTargetPreviewAnim.SetTrigger("BeginTarget");
    }

    public void HideTargetPreviewArrow()
    {
        RetinueTargetPreviewAnim.SetTrigger("EndTarget");
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slots, moduleRetinue, ship, dragLastPosition, dragBeginPosition, dragBeginQuaternion);
        if (moduleRetinue && moduleRetinue.ClientPlayer != ClientPlayer && !RoundManager.Instance.EnemyClientPlayer.MyBattleGroundManager.RemoveRetinues.Contains(moduleRetinue))
        {
            RetinueAttackRetinueRequest request = new RetinueAttackRetinueRequest(ClientPlayer.ClientId, M_RetinueID, RoundManager.Instance.EnemyClientPlayer.ClientId, moduleRetinue.M_RetinueID);
            Client.Instance.Proxy.SendMessage(request);
        }
        else if (ship && ship.ClientPlayer != ClientPlayer)
        {
            RetinueAttackShipRequest request = new RetinueAttackShipRequest(Client.Instance.Proxy.ClientId, M_RetinueID);
            Client.Instance.Proxy.SendMessage(request);
        }

        DragManager.Instance.DragOutDamage = 0;
    }

    public override void DragComponent_SetStates(ref bool canDrag, ref DragPurpose dragPurpose)
    {
        canDrag = CheckCanAttack();
        dragPurpose = DragPurpose.Target;
    }

    public override float DragComponnet_DragDistance()
    {
        return 0.2f;
    }

    public override void DragComponnet_DragOutEffects()
    {
        base.DragComponnet_DragOutEffects();
        DragManager.Instance.DragOutDamage = CalculateAttack();
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
            if (!ClientPlayer.MyBattleGroundManager.RemoveRetinues.Contains(this))
            {
                ClientUtils.ChangeColor(OnHoverBloom, GameManager.Instance.RetinueOnEnemyHoverBloomColor);
                OnHoverBloom.gameObject.SetActive(value);
            }
        }
    }

    public override void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
        base.MouseHoverComponent_OnMousePressEnterImmediately(mousePosition);
        if (DragManager.Instance.CurrentDrag)
        {
            ModuleRetinue mr = DragManager.Instance.CurrentDrag_ModuleRetinue;
            CardSpell cs = DragManager.Instance.CurrentDrag_CardSpell;
            if (mr != null)
            {
                if (mr.ClientPlayer != ClientPlayer && mr != this)
                {
                    IsBeDraggedHover = true;
                    if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                    {
                        ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                    }

                    DamageNumberPreviewTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
                    DamageNumberPreviewBGTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
                }
            }
            else if (cs != null)
            {
                IsBeDraggedHover = true;
                if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                {
                    ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                }

                DamageNumberPreviewTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
                DamageNumberPreviewBGTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
            }
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

        DamageNumberPreviewTextMesh.text = "";
        DamageNumberPreviewBGTextMesh.text = "";
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
            if (!ClientPlayer.MyBattleGroundManager.RemoveRetinues.Contains(this))
            {
                if (ClientPlayer == RoundManager.Instance.EnemyClientPlayer)
                {
                    ClientUtils.ChangeColor(OnHoverBloom, GameManager.Instance.RetinueOnEnemyHoverBloomColor);
                }
                else
                {
                    ClientUtils.ChangeColor(OnHoverBloom, GameManager.Instance.RetinueOnSelfHoverBloomColor);
                }

                OnHoverBloom.gameObject.SetActive(value);
            }
        }
    }

    public override void MouseHoverComponent_OnMouseEnterImmediately(Vector3 mousePosition)
    {
        base.MouseHoverComponent_OnMouseEnterImmediately(mousePosition);
        if (DragManager.Instance.IsSummonPreview)
        {
            TargetSideEffect.TargetRange targetRange = DragManager.Instance.SummonRetinueTargetRange;
            if ((ClientPlayer == RoundManager.Instance.EnemyClientPlayer &&
                 (targetRange == TargetSideEffect.TargetRange.EnemyBattleGround ||
                  (targetRange == TargetSideEffect.TargetRange.EnemySoldiers && CardInfo.BattleInfo.IsSoldier) ||
                  targetRange == TargetSideEffect.TargetRange.EnemyHeros && !CardInfo.BattleInfo.IsSoldier))
                ||
                ClientPlayer == RoundManager.Instance.SelfClientPlayer && ClientPlayer.MyBattleGroundManager.CurrentSummonPreviewRetinue != this &&
                (targetRange == TargetSideEffect.TargetRange.SelfBattleGround || (targetRange == TargetSideEffect.TargetRange.SelfSoldiers && CardInfo.BattleInfo.IsSoldier)))
            {
                IsBeHover = true;
                if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                {
                    ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                }
            }
        }
    }

    public override void MouseHoverComponent_OnMouseLeaveImmediately()
    {
        base.MouseHoverComponent_OnMouseLeaveImmediately();
        IsBeHover = false;
        if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
        {
            ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = false; //箭头动画
        }
    }

    #endregion

    #endregion

    #region 副作用

    public void OnSummon()
    {
        CheckCanAttack();
    }

    public void OnSummonShowEffects()
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShowSideEffectBloom(ClientUtils.HTMLColorToColor("#00FFDA"), 0.5f), "ShowSideEffectBloom");
    }

    public void OnDie()
    {
        if (IsDead) return;
        IsDead = true;
    }

    public void OnDieShowEffects()
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShowSideEffectBloom(ClientUtils.HTMLColorToColor("#000000"), 0.5f), "ShowSideEffectBloom");
    }

    IEnumerator Co_ShowSideEffectBloom(Color color, float duration)
    {
        SideEffcetBloom.gameObject.SetActive(true);
        ClientUtils.ChangeColor(SideEffcetBloom, color);
        yield return new WaitForSeconds(duration);
        SideEffcetBloom.gameObject.SetActive(false);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
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
        EndRound = false;
        CanAttackThisRound = true;
        CheckCanAttack();
    }

    public void OnEndRound()
    {
        EndRound = true;
        CheckCanAttack();
        isFirstRound = false;
    }

    #endregion
}