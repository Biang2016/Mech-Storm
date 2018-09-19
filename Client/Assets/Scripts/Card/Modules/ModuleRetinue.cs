using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleRetinue : ModuleBase
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

        ArmorFillAnim.gameObject.SetActive(false);

        M_ClientTempRetinueID = -1;
        DamageNumberPreviewTextMesh.text = "";
        DamageNumberPreviewBGTextMesh.text = "";
        base.PoolRecycle();
        ResetRetinue();
    }

    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.Pool_ModuleRetinuePool;
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
    [SerializeField] private Renderer WeaponBloom;
    [SerializeField] private Renderer ShieldBloom;

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

    [SerializeField] private Animator LifeIconAnim;

    [SerializeField] private Animator ArmorFillAnim;

    [SerializeField] private Image ShieldBar;
    [SerializeField] private Animator ShieldBarAnim;

    [SerializeField] private Image SwordBar;
    [SerializeField] private Animator SwordBarAnim;

    [SerializeField] private Animator RetinueTargetPreviewAnim;

    [SerializeField] private TextFlyPile LifeChangeNumberFly;
    [SerializeField] private TextFlyPile ArmorChangeNumberFly;
    [SerializeField] private TextFlyPile ShieldChangeNumberFly;
    [SerializeField] private TextFlyPile WeaponAttackChangeNumberFly;
    [SerializeField] private TextFlyPile WeaponEnergyChangeNumberFly;
    [SerializeField] private TextFlyPile ShieldDefenceNumberFly;

    private bool isInitializing = false;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        isInitializing = true;
        base.Initiate(cardInfo, clientPlayer);
        Text_RetinueArmor.text = "";
        Text_RetinueShield.text = "";
        ArmorFillAnim.gameObject.SetActive(false);
        ShieldBar.fillAmount = 0;
        SwordBar.fillAmount = 0;
        M_RetinueName = cardInfo.BaseInfo.CardName;
        M_RetinueLeftLife = cardInfo.LifeInfo.Life;
        M_RetinueTotalLife = cardInfo.LifeInfo.TotalLife;
        M_RetinueAttack = cardInfo.BattleInfo.BasicAttack;
        M_RetinueArmor = cardInfo.BattleInfo.BasicArmor;
        M_RetinueShield = cardInfo.BattleInfo.BasicShield;
        M_RetinueWeaponEnergy = 0;
        M_RetinueWeaponEnergyMax = 0;
        ClientUtils.ChangePicture(PictureBoxRenderer, CardInfo.BaseInfo.PictureID);
        ClientUtils.ChangeColor(WeaponBloom, ClientUtils.HTMLColorToColor("#FF0022"));
        ClientUtils.ChangeColor(ShieldBloom, ClientUtils.HTMLColorToColor("#FFA600"));

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

    private void ResetRetinue()
    {
        Text_RetinueArmor.text = "";
        Text_RetinueShield.text = "";
        ArmorFillAnim.gameObject.SetActive(false);
        ShieldBar.fillAmount = 0;
        SwordBar.fillAmount = 0;
        M_RetinueName = "";
        M_RetinueLeftLife = 0;
        M_RetinueTotalLife = 0;
        M_RetinueAttack = 0;
        M_RetinueArmor = 0;
        M_RetinueShield = 0;
        M_RetinueWeaponEnergy = 0;
        M_RetinueWeaponEnergyMax = 0;
    }

    public override void ChangeColor(Color color)
    {
        base.ChangeColor(color);
        ClientUtils.ChangeColor(OnHoverBloom, GameManager.Instance.RetinueOnEnemyHoverBloomColor);
        ClientUtils.ChangeColor(RetinueCanAttackBloom, GameManager.Instance.RetinueBloomColor);
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

    public bool isTotalLifeChanging = false;

    private int m_RetinueLeftLife;

    public int M_RetinueLeftLife
    {
        get { return m_RetinueLeftLife; }
        set
        {
            int before = m_RetinueLeftLife;
            if (m_RetinueLeftLife != value)
            {
                m_RetinueLeftLife = value;
                float duration = isInitializing ? 0 : 0.1f;
                BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_LifeChange(value, m_RetinueTotalLife, value - before, duration, isTotalLifeChanging, isInitializing), "Co_LifeChange");
            }
        }
    }

    private int m_RetinueTotalLife;

    public int M_RetinueTotalLife
    {
        get { return m_RetinueTotalLife; }
        set
        {
            int before = m_RetinueTotalLife;

            if (m_RetinueTotalLife != value)
            {
                m_RetinueTotalLife = value;
                float duration = isInitializing ? 0 : 0.1f;
                BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_TotalLifeChange(m_RetinueLeftLife, value, value - before, duration, isInitializing), "Co_TotalLifeChange");
            }
        }
    }

    public bool isAttackChanging = false;
    private int m_RetinueAttack;

    public int M_RetinueAttack
    {
        get { return m_RetinueAttack; }
        set
        {
            int before = m_RetinueAttack;
            int before_att = M_RetinueWeaponFinalAttack;
            if (m_RetinueAttack != value)
            {
                m_RetinueAttack = value;
                if (M_Weapon)
                {
                    M_Weapon.M_WeaponAttack = value;
                }

                CheckCanAttack();
                float duration = isInitializing ? 0 : 0.1f;
                BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RetinueAttackChange(M_RetinueAttack, M_RetinueWeaponEnergy, M_RetinueWeaponEnergyMax, M_RetinueWeaponEnergy == 0 ? (value - before) : M_RetinueWeaponFinalAttack - before_att, duration), "Co_RetinueAttackChange");
            }
        }
    }

    private int m_RetinueWeaponEnergy;

    public int M_RetinueWeaponEnergy
    {
        get { return m_RetinueWeaponEnergy; }
        set
        {
            int before_att = M_RetinueWeaponFinalAttack;
            if (m_RetinueWeaponEnergy != value)
            {
                m_RetinueWeaponEnergy = value;
                if (M_Weapon)
                {
                    M_Weapon.M_WeaponEnergy = value;
                }

                float duration = isInitializing ? 0 : 0.1f;
                if (!isInitializing && m_RetinueAttack != 0 && !isAttackChanging) BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RetinueAttackChange(M_RetinueAttack, M_RetinueWeaponEnergy, M_RetinueWeaponEnergyMax, M_RetinueWeaponFinalAttack - before_att, duration), "Co_RetinueAttackChange");
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

                float duration = isInitializing ? 0 : 0.1f;
                if (!isInitializing) BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RetinueWeaponEnergyMaxChange(M_RetinueWeaponEnergy, M_RetinueWeaponEnergyMax, value - before, duration), "Co_RetinueWeaponEnergyMaxChange");
            }
        }
    }

    public int M_RetinueWeaponFinalAttack
    {
        get
        {
            if (M_RetinueWeaponEnergy == 0)
            {
                return M_RetinueAttack;
            }
            else
            {
                return M_RetinueWeaponEnergy * M_RetinueAttack;
            }
        }
    }


    private int m_RetinueArmor;

    public int M_RetinueArmor
    {
        get { return m_RetinueArmor; }
        set
        {
            int before = m_RetinueArmor;
            if (m_RetinueArmor != value)
            {
                m_RetinueArmor = value;
                if (M_Shield)
                {
                    M_Shield.M_ShieldArmor = value;
                }

                float duration = 0;
                BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ArmorChange(value, value - before, duration), "Co_ArmorChange");
            }
        }
    }

    private int m_RetinueShield;

    public int M_RetinueShield
    {
        get { return m_RetinueShield; }
        set
        {
            int before = m_RetinueShield;
            if (m_RetinueShield != value)
            {
                m_RetinueShield = value;
                if (M_Shield)
                {
                    M_Shield.M_ShieldShield = value;
                }

                float duration = 0;
                BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShieldChange(value, value - before, duration), "Co_ShieldChange");
            }
        }
    }

    private void ShieldDefenceDamage(int decreaseValue, int shieldValue)
    {
        float duration = 0;
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShieldChange(shieldValue, 0, duration), "Co_ShieldChange");
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShieldChangeNumberFly(decreaseValue), "Co_ShieldChangeNumberFly");
    }

    IEnumerator Co_ShieldChangeNumberFly(int decreaseValue)
    {
        ShieldDefenceNumberFly.SetText((GameManager.Instance.isEnglish ? "Dec -" : "阻挡 -") + decreaseValue, "#00FFF2", "#00FFF2", TextFly.FlyDirection.Down, fontSize: 40);
        yield return new WaitForSeconds(0.1f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    IEnumerator Co_LifeChange(int leftLifeValue, int totalLifeValue, int change, float duration, bool isTotalLifeChanging, bool isInitializing)
    {
        LifeIconAnim.SetTrigger("Jump");
        if (change > 0)
        {
            LifeChangeNumberFly.SetText("+" + change, "#92FF00", "#92FF00", TextFly.FlyDirection.Up);
            if (!isTotalLifeChanging && !isInitializing) AudioManager.Instance.SoundPlay("sfx/OnHeal");
        }
        else if (change < 0)
        {
            HitManager.Instance.ShowHit(LifeIconAnim.transform, HitManager.HitType.LineLeftTopToRightButtom, ClientUtils.HTMLColorToColor("#FFFFFF"), 0.3f);
            LifeChangeNumberFly.SetText("-" + (-change), "#FF0A00", "#FF0A00", TextFly.FlyDirection.Down);
            if (change <= -8)
            {
                AudioManager.Instance.SoundPlay("sfx/OnDamageBig");
            }
            else
            {
                AudioManager.Instance.SoundPlay("sfx/OnLifeDamage");
            }
        }

        LifeTextChange(leftLifeValue, totalLifeValue);
        yield return new WaitForSeconds(duration);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    IEnumerator Co_TotalLifeChange(int leftLifeValue, int totalLifeValue, int change, float duration, bool isInitializing)
    {
        LifeIconAnim.SetTrigger("Jump");
        if (change > 0)
        {
            LifeChangeNumberFly.SetText("+" + change, "#FFFFFF", "#FFFFFF", TextFly.FlyDirection.Up);
            if (!isInitializing) AudioManager.Instance.SoundPlay("sfx/OnAddLife");
        }
        else
        {
            LifeChangeNumberFly.SetText("-" + (-change), "#00A8FF", "#00A8FF", TextFly.FlyDirection.Down);
        }

        LifeTextChange(leftLifeValue, totalLifeValue);
        yield return new WaitForSeconds(duration);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private void LifeTextChange(int leftLifeValue, int totalLifeValue)
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


    IEnumerator Co_RetinueAttackChange(int retinueAttackValue, int retinueEnergy, int retinueEnergyMax, int change, float duration)
    {
        int finalAttack = retinueEnergy == 0 ? retinueAttackValue : retinueEnergy * retinueAttackValue;

        Text_RetinueAttack.text = finalAttack > 0 ? finalAttack.ToString() : "";
        if (change > 0)
        {
            WeaponAttackChangeNumberFly.SetText("+" + change, "#92FF00", "#92FF00", TextFly.FlyDirection.Up);
        }
        else if (change < 0)
        {
            WeaponAttackChangeNumberFly.SetText(change.ToString(), "#530000", "#530000", TextFly.FlyDirection.Down);
        }

        RefreshSwordBarMask(retinueEnergy, retinueEnergyMax);
        yield return new WaitForSeconds(duration);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    IEnumerator Co_RetinueWeaponEnergyMaxChange(int retinueEnergy, int retinueEnergyMax, int change, float duration)
    {
        if (change > 0)
        {
            WeaponEnergyChangeNumberFly.SetText("Max +" + change, "#1CFF00", "#1CFF00", TextFly.FlyDirection.Up);
        }
        else if (change < 0)
        {
            WeaponEnergyChangeNumberFly.SetText("Max " + change, "#530000", "#530000", TextFly.FlyDirection.Down);
        }

        RefreshSwordBarMask(retinueEnergy, retinueEnergyMax);
        yield return new WaitForSeconds(duration);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private void RefreshSwordBarMask(int retinueWeaponEnergyValue, int retinueWeaponEnergyMaxValue)
    {
        if (retinueWeaponEnergyMaxValue != 0)
        {
            SwordBar.fillAmount = (float) retinueWeaponEnergyValue / retinueWeaponEnergyMaxValue;
            SwordBarAnim.SetTrigger("Jump");
        }
        else
        {
            SwordBar.fillAmount = 0;
        }
    }

    IEnumerator Co_ArmorChange(int armorValue, int change, float duration)
    {
        ArmorFillAnim.SetTrigger("Jump");
        if (change > 0)
        {
            ArmorChangeNumberFly.SetText("+" + change, "#92FF00", "#92FF00", TextFly.FlyDirection.Up);
        }
        else
        {
            HitManager.Instance.ShowHit(ArmorFillAnim.transform, HitManager.HitType.LineLeftTopToRightButtom, ClientUtils.HTMLColorToColor("#FFD217"), 0.3f);
            ArmorChangeNumberFly.SetText("-" + change, "#FF0000", "#FF0000", TextFly.FlyDirection.Down);
            if (armorValue != 0) AudioManager.Instance.SoundPlay("sfx/ArmorHit", 0.5f);
        }

        if (armorValue == 0)
        {
            ArmorFillAnim.gameObject.SetActive(false);
            Text_RetinueArmor.text = "";
            AudioManager.Instance.SoundPlay("sfx/ArmorBroke", 1f);
        }
        else
        {
            ArmorFillAnim.gameObject.SetActive(true);
            Text_RetinueArmor.text = armorValue.ToString();
        }

        yield return new WaitForSeconds(duration);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }


    [SerializeField] private int RetinueShieldFull;


    IEnumerator Co_ShieldChange(int shieldValue, int change, float duration)
    {
        RetinueShieldFull = Mathf.Max(RetinueShieldFull, shieldValue);
        ShieldBarAnim.SetTrigger("Jump");
        if (change > 0)
        {
            ShieldChangeNumberFly.SetText("+" + change, "#00FFF2", "#00FFF2", TextFly.FlyDirection.Up);
        }
        else
        {
            HitManager.Instance.ShowHit(ShieldBar.transform, HitManager.HitType.LineRightTopToLeftButtom, ClientUtils.HTMLColorToColor("#2BFFF8"), 0.3f);
            ShieldChangeNumberFly.SetText("-" + change, "#FF0000", "#FF0000", TextFly.FlyDirection.Down);
            if (shieldValue != 0) AudioManager.Instance.SoundPlay("sfx/ShieldHit", 1f);
        }


        if (shieldValue == 0)
        {
            AudioManager.Instance.SoundPlay("sfx/ShieldBroke", 1f);
            ShieldBar.fillAmount = 0;
            Text_RetinueShield.text = "";
        }
        else
        {
            ShieldBar.fillAmount = (float) shieldValue / (float) RetinueShieldFull;
            Text_RetinueShield.text = shieldValue.ToString();
        }

        yield return new WaitForSeconds(duration);
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
        AudioManager.Instance.SoundPlay("sfx/WeaponChange");
    }

    void On_WeaponChanged()
    {
        M_Weapon.OnWeaponEquiped();
        CheckCanAttack();
        AudioManager.Instance.SoundPlay("sfx/WeaponChange");
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
        AudioManager.Instance.SoundPlay("sfx/ShieldChange");
    }

    void On_ShieldChanged()
    {
        M_Shield.OnShieldEquiped();
        AudioManager.Instance.SoundPlay("sfx/ShieldChange");
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
                        if (targetRetinue.IsDead) break;
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

    public void BeAttacked(int attackNumber) //攻击和被攻击仅发送伤害数值给客户端，具体计算分别处理
    {
        OnBeAttacked();
        int remainAttackNumber = attackNumber;

        //小于等于护盾的伤害的全部免除，护盾无任何损失，大于护盾的伤害，每超过一点，护盾受到一点伤害，如果扣为0，则护盾破坏
        if (M_RetinueShield > 0)
        {
            if (M_RetinueShield >= remainAttackNumber)
            {
                ShieldDefenceDamage(remainAttackNumber, M_RetinueShield);
                remainAttackNumber = 0;
                return;
            }
            else
            {
                int shieldDecrease = remainAttackNumber - M_RetinueShield;
                remainAttackNumber -= M_RetinueShield;
                M_RetinueShield -= Mathf.Min(m_RetinueShield, shieldDecrease);
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
        RetinueTargetPreviewAnim.ResetTrigger("BeginTarget");
        RetinueTargetPreviewAnim.ResetTrigger("EndTarget");
        RetinueTargetPreviewAnim.SetTrigger("BeginTarget");
    }

    public void HideTargetPreviewArrow()
    {
        RetinueTargetPreviewAnim.ResetTrigger("BeginTarget");
        RetinueTargetPreviewAnim.ResetTrigger("EndTarget");
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
            if (mr != null && CheckModuleRetinueCanAttack(mr))
            {
                IsBeDraggedHover = true;
                if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                {
                    ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                }

                DamageNumberPreviewTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
                DamageNumberPreviewBGTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;

                int myCounterAttack = CalculateAttack();
                mr.DamageNumberPreviewTextMesh.text = myCounterAttack == 0 ? "" : "-" + myCounterAttack;
                mr.DamageNumberPreviewBGTextMesh.text = myCounterAttack == 0 ? "" : "-" + myCounterAttack;
            }
            else if (cs != null && CheckCardSpellCanTarget(cs))
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

    private bool CheckModuleRetinueCanAttack(ModuleRetinue retinue)
    {
        if (retinue == this) return false;
        //Todo 嘲讽类随从等逻辑
        if (retinue.ClientPlayer == ClientPlayer)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool CheckCardSpellCanTarget(CardSpell card)
    {
        if (card.ClientPlayer == ClientPlayer)
        {
            return card.targetRange == TargetSideEffect.TargetRange.All ||
                   card.targetRange == TargetSideEffect.TargetRange.BattleGrounds ||
                   card.targetRange == TargetSideEffect.TargetRange.SelfBattleGround ||
                   card.targetRange == TargetSideEffect.TargetRange.Heros ||
                   card.targetRange == TargetSideEffect.TargetRange.Soldiers ||
                   card.targetRange == TargetSideEffect.TargetRange.SelfHeros ||
                   card.targetRange == TargetSideEffect.TargetRange.SelfSoldiers;
        }
        else
        {
            return card.targetRange == TargetSideEffect.TargetRange.All ||
                   card.targetRange == TargetSideEffect.TargetRange.BattleGrounds ||
                   card.targetRange == TargetSideEffect.TargetRange.EnemyBattleGround ||
                   card.targetRange == TargetSideEffect.TargetRange.Heros ||
                   card.targetRange == TargetSideEffect.TargetRange.Soldiers ||
                   card.targetRange == TargetSideEffect.TargetRange.EnemyHeros ||
                   card.targetRange == TargetSideEffect.TargetRange.EnemySoldiers;
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

        if (DragManager.Instance.CurrentDrag)
        {
            ModuleRetinue mr = DragManager.Instance.CurrentDrag_ModuleRetinue;
            CardSpell cs = DragManager.Instance.CurrentDrag_CardSpell;
            if (mr != null)
            {
                mr.DamageNumberPreviewTextMesh.text = "";
                mr.DamageNumberPreviewBGTextMesh.text = "";
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

    public override void MouseHoverComponent_OnHover1Begin(Vector3 mousePosition)
    {
        base.MouseHoverComponent_OnHover1Begin(mousePosition);
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

    public override void OnShowEffects(SideEffectBundle.TriggerTime triggerTime, SideEffectBundle.TriggerRange triggerRange)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShowSideEffectBloom(ClientUtils.HTMLColorToColor("#00FFDA"), 0.5f), "ShowSideEffectBloom");
    }

    IEnumerator Co_ShowSideEffectBloom(Color color, float duration)
    {
        SideEffcetBloom.gameObject.SetActive(true);
        ClientUtils.ChangeColor(SideEffcetBloom, color);
        AudioManager.Instance.SoundPlay("sfx/OnSE");
        yield return new WaitForSeconds(duration);
        SideEffcetBloom.gameObject.SetActive(false);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void OnSummon()
    {
        CheckCanAttack();
    }

    public void OnDie()
    {
        if (IsDead) return;
        IsDead = true;
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