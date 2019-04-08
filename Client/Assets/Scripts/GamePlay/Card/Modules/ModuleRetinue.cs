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
        gameObjectPool = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Retinue];
        DamageNumberPreviewTextMesh.text = "";
        DamageNumberPreviewBGTextMesh.text = "";
    }

    public void SetGoPool(GameObjectPool pool)
    {
        gameObjectPool = pool;
    }

    #region 各模块、自身数值与初始化

    [SerializeField] private Text Text_RetinueName;

    [SerializeField] private Renderer RetinueCanAttackBloom;
    [SerializeField] private Renderer OnHoverBloom;
    [SerializeField] private Renderer SideEffcetBloom;
    [SerializeField] private RawImage WeaponBloom;
    [SerializeField] private RawImage ShieldBloom;
    [SerializeField] private RawImage PackBloom;
    [SerializeField] private RawImage MABloom;

    public Slot Slot1;
    public Slot Slot2;
    public Slot Slot3;
    public Slot Slot4;

    [SerializeField] private Text LifeText;

    [SerializeField] private Text Text_RetinueAttack;
    [SerializeField] private Text Text_RetinueShield;
    [SerializeField] private Text Text_RetinueArmor;

    [SerializeField] private Image Picture;

    [SerializeField] private TextMesh DamageNumberPreviewTextMesh; //受攻击瞄准时的伤害预览
    [SerializeField] private TextMesh DamageNumberPreviewBGTextMesh; //受攻击瞄准时的伤害预览

    [SerializeField] private Animator LifeIconAnim;

    [SerializeField] private Animator ArmorFillAnim;

    [SerializeField] private Image ShieldBar;
    [SerializeField] private Animator ShieldBarAnim;

    [SerializeField] private Image SwordBar;
    [SerializeField] private Animator SwordBarAnim;

    [SerializeField] private Animator RetinueTargetPreviewAnim;
    [SerializeField] private Text DefenceText;
    [SerializeField] private Image SniperTargetImage;
    [SerializeField] private Text SniperTipText;

    [SerializeField] private TextFlyPile LifeChangeNumberFly;
    [SerializeField] private TextFlyPile ArmorChangeNumberFly;
    [SerializeField] private TextFlyPile ShieldChangeNumberFly;
    [SerializeField] private TextFlyPile WeaponAttackChangeNumberFly;
    [SerializeField] private TextFlyPile WeaponEnergyChangeNumberFly;
    [SerializeField] private TextFlyPile ShieldDefenceNumberFly;
    [SerializeField] private TextFlyPile DodgeNumberFly;

    [SerializeField] private Image SideEffectBGCommonIcon;
    [SerializeField] private Image SideEffectCommonIcon;
    [SerializeField] private Image SideEffectDieIcon;
    [SerializeField] private Image SideEffectBGDieIcon;
    [SerializeField] private Animator SideEffectBGCommonAnim;
    [SerializeField] private Animator SideEffectBGDieAnim;

    public Transform[] HitPoints;

    private bool isInitializing = false;

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        transform.localScale = Vector3.one * GameManager.Instance.RetinueDefaultSize;

        isInitializing = true;
        base.Initiate(cardInfo, clientPlayer);
        Text_RetinueArmor.text = "";
        Text_RetinueShield.text = "";
        ArmorFillAnim.gameObject.SetActive(false);
        ShieldBar.fillAmount = 0;
        SwordBar.fillAmount = 0;
        M_RetinueName = CardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()];
        M_RetinueLeftLife = cardInfo.LifeInfo.Life;
        M_RetinueTotalLife = cardInfo.LifeInfo.TotalLife;
        M_RetinueWeaponEnergy = 0;
        M_RetinueWeaponEnergyMax = 0;
        M_RetinueAttack = cardInfo.BattleInfo.BasicAttack;
        M_RetinueArmor = cardInfo.BattleInfo.BasicArmor;
        M_RetinueShield = cardInfo.BattleInfo.BasicShield;

        HideRetinueTypeLooking();

        ClientUtils.ChangeCardPicture(Picture, CardInfo.BaseInfo.PictureID);
        ClientUtils.ChangeColor(WeaponBloom, ClientUtils.GetColorFromColorDict(AllColors.ColorType.Slot1Color));
        ClientUtils.ChangeColor(ShieldBloom, ClientUtils.GetColorFromColorDict(AllColors.ColorType.Slot2Color));
        ClientUtils.ChangeColor(PackBloom, ClientUtils.GetColorFromColorDict(AllColors.ColorType.Slot3Color));
        ClientUtils.ChangeColor(MABloom, ClientUtils.GetColorFromColorDict(AllColors.ColorType.Slot4Color));

        SideEffectBGCommonIcon.gameObject.SetActive(false);
        SideEffectCommonIcon.gameObject.SetActive(false);
        SideEffectDieIcon.gameObject.SetActive(false);
        SideEffectBGDieIcon.gameObject.SetActive(false);

        if (CardInfo.SideEffectBundle_OnBattleGround.GetSideEffectExecutes(SideEffectBundle.TriggerTime.OnRetinueDie, SideEffectBundle.TriggerRange.Self).Count != 0)
        {
            SideEffectDieIcon.gameObject.SetActive(true);
            SideEffectBGDieIcon.gameObject.SetActive(true);
        }

        foreach (SideEffectExecute see in CardInfo.SideEffectBundle_OnBattleGround.SideEffectExecutes)
        {
            if (!(see.TriggerTime == SideEffectBundle.TriggerTime.OnRetinueDie && see.TriggerRange == SideEffectBundle.TriggerRange.Self)
                && !(see.TriggerTime == SideEffectBundle.TriggerTime.OnRetinueSummon && see.TriggerRange == SideEffectBundle.TriggerRange.Self))
            {
                SideEffectCommonIcon.gameObject.SetActive(true);
                SideEffectBGCommonIcon.gameObject.SetActive(true);
                break;
            }
        }

        if (Slot1)
        {
            Slot1.ClientPlayer = ClientPlayer;
            Slot1.M_ModuleRetinue = this;
            Slot1.MSlotTypes = cardInfo.RetinueInfo.Slots[0];
        }

        if (Slot2)
        {
            Slot2.ClientPlayer = ClientPlayer;
            Slot2.M_ModuleRetinue = this;
            Slot2.MSlotTypes = cardInfo.RetinueInfo.Slots[1];
        }

        if (Slot3)
        {
            Slot3.ClientPlayer = ClientPlayer;
            Slot3.M_ModuleRetinue = this;
            Slot3.MSlotTypes = cardInfo.RetinueInfo.Slots[2];
        }

        if (Slot4)
        {
            Slot4.ClientPlayer = ClientPlayer;
            Slot4.M_ModuleRetinue = this;
            Slot4.MSlotTypes = cardInfo.RetinueInfo.Slots[3];
        }

        isInitializing = false;

        M_ClientTempRetinueID = -1;

        CanAttack = false;
        MA_BG.SetActive(false);

        ImmuneIcon.SetActive(false);
        InactivityIcon.SetActive(false);

        RetinueTargetPreviewAnim.SetBool("ShowTarget", false);
        DefenceText.enabled = false;
        SniperTipText.enabled = false;
        SniperTargetImage.enabled = false;

        IsDead = false;
    }

    private void ResetRetinue()
    {
        Text_RetinueArmor.text = "";
        Text_RetinueShield.text = "";
        ArmorFillAnim.gameObject.SetActive(false);
        ShieldBar.fillAmount = 0;
        SwordBar.fillAmount = 0;
        m_RetinueName = "";
        m_RetinueLeftLife = 0;
        m_RetinueTotalLife = 0;
        m_RetinueAttack = 0;
        m_RetinueArmor = 0;
        m_RetinueShield = 0;
        m_RetinueWeaponEnergy = 0;
        m_RetinueWeaponEnergyMax = 0;
    }

    public override void ChangeColor(Color color)
    {
        ClientUtils.ChangeEmissionColor(MainBoardRenderer, color, MainboardEmissionIntensity);
        ClientUtils.ChangeColor(OnHoverBloom, ClientUtils.GetColorFromColorDict(AllColors.ColorType.RetinueOnEnemyHoverBloomColor), 2f);
        ClientUtils.ChangeColor(RetinueCanAttackBloom, ClientUtils.GetColorFromColorDict(AllColors.ColorType.RetinueBloomColor), 2f);
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

    [SerializeField] private GameObject ImmuneIcon;
    [SerializeField] private Text ImmuneCountText;

    private int m_ImmuneLeftRounds = 0;

    public int M_ImmuneLeftRounds
    {
        get { return m_ImmuneLeftRounds; }
        set
        {
            m_ImmuneLeftRounds = value;
            if (m_ImmuneLeftRounds != 0)
            {
                ImmuneIcon.SetActive(true);
                ImmuneCountText.text = m_ImmuneLeftRounds.ToString();
            }
            else
            {
                ImmuneIcon.SetActive(false);
            }
        }
    }

    [SerializeField] private GameObject InactivityIcon;
    [SerializeField] private Text InactivityCountText;

    private int m_InactivityRounds = 0;

    public int M_InactivityRounds
    {
        get { return m_InactivityRounds; }
        set
        {
            m_InactivityRounds = value;
            if (m_InactivityRounds != 0)
            {
                InactivityIcon.SetActive(true);
                InactivityCountText.text = m_InactivityRounds.ToString();
            }
            else
            {
                InactivityIcon.SetActive(false);
            }
        }
    }

    private string m_RetinueName;

    public string M_RetinueName
    {
        get { return m_RetinueName; }
        set
        {
            m_RetinueName = value;
            Text_RetinueName.text = value;
        }
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
            if (m_RetinueAttack != value || isInitializing)
            {
                m_RetinueAttack = value;
                if (M_Weapon)
                {
                    M_Weapon.M_WeaponAttack = value;
                }

                float duration = isInitializing ? 0 : 0.1f;
                BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RetinueAttackChange(M_RetinueAttack, M_RetinueWeaponEnergy, M_RetinueWeaponEnergyMax, M_RetinueWeaponEnergy == 0 ? (value - before) : M_RetinueWeaponFinalAttack - before_att, duration, isInitializing), "Co_RetinueAttackChange");
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
                if (!isInitializing && m_RetinueAttack != 0 && !isAttackChanging) BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RetinueAttackChange(M_RetinueAttack, M_RetinueWeaponEnergy, M_RetinueWeaponEnergyMax, M_RetinueWeaponFinalAttack - before_att, duration, isInitializing), "Co_RetinueAttackChange");
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
                if (!isInitializing) BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RetinueWeaponEnergyMaxChange(M_RetinueWeaponEnergy, M_RetinueWeaponEnergyMax, value - before, duration, isInitializing), "Co_RetinueWeaponEnergyMaxChange");
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
                BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ArmorChange(value, value - before, duration, isInitializing), "Co_ArmorChange");
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
                BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShieldChange(value, value - before, duration, isInitializing), "Co_ShieldChange");
            }
        }
    }

    public void ShieldDefenceDamage(int decreaseValue, int shieldValue)
    {
        float duration = 0;
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShieldChange(shieldValue, 0, duration, isInitializing), "Co_ShieldChange");
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShieldChangeNumberFly(decreaseValue), "Co_ShieldChangeNumberFly");
    }

    IEnumerator Co_ShieldChangeNumberFly(int decreaseValue)
    {
        ShieldDefenceNumberFly.SetText(LanguageManager.Instance.GetText("ModuleRetinue_DecreaseDamageNumberFly") + decreaseValue, "#00FFF2", "#00FFF2", TextFly.FlyDirection.Down);
        yield return new WaitForSeconds(0.1f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    IEnumerator Co_LifeChange(int leftLifeValue, int totalLifeValue, int change, float duration, bool isTotalLifeChanging, bool isInitializing)
    {
        if (!isInitializing)
        {
            string text = LanguageManager.Instance.GetText("ModuleRetinue_LifeNumberFly");
            if (change > 0)
            {
                if (!isTotalLifeChanging && !isInitializing)
                {
                    LifeChangeNumberFly.SetText(text + "+" + change, "#FFFFFF", "#FFFFFF", TextFly.FlyDirection.Up);
                    AudioManager.Instance.SoundPlay("sfx/OnHeal");
                }
            }
            else if (change < 0)
            {
                HitManager.Instance.ShowHit(LifeIconAnim.transform, HitManager.HitType.LineLeftTopToRightButtom, "#FFFFFF", 0.3f);
                LifeChangeNumberFly.SetText(text + change, "#FF0A00", "#FF0A00", TextFly.FlyDirection.Down);
                if (change <= -8)
                {
                    AudioManager.Instance.SoundPlay("sfx/OnDamageBig");
                }
                else
                {
                    AudioManager.Instance.SoundPlay("sfx/OnLifeDamage");
                }
            }
        }

        LifeIconAnim.SetTrigger("Jump");
        LifeTextChange(leftLifeValue, totalLifeValue);
        yield return new WaitForSeconds(duration);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    IEnumerator Co_TotalLifeChange(int leftLifeValue, int totalLifeValue, int change, float duration, bool isInitializing)
    {
        if (!isInitializing)
        {
            if (change > 0)
            {
                if (!isInitializing)
                {
                    LifeChangeNumberFly.SetText("Max +" + change, "#68FF00", "#68FF00", TextFly.FlyDirection.Up);
                    AudioManager.Instance.SoundPlay("sfx/OnAddLife");
                }
            }
            else if (change < 0)
            {
                LifeChangeNumberFly.SetText("Max " + change, "#A000FF", "#A000FF", TextFly.FlyDirection.Down);
            }
        }

        LifeIconAnim.SetTrigger("Jump");
        LifeTextChange(leftLifeValue, totalLifeValue);
        yield return new WaitForSeconds(duration);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private void LifeTextChange(int leftLifeValue, int totalLifeValue)
    {
        if (leftLifeValue < totalLifeValue)
        {
            LifeText.color = ClientUtils.GetColorFromColorDict(AllColors.ColorType.InjuredLifeNumberColor);
        }
        else if (leftLifeValue == totalLifeValue && totalLifeValue > CardInfo.LifeInfo.TotalLife)
        {
            LifeText.color = ClientUtils.GetColorFromColorDict(AllColors.ColorType.OverFlowTotalLifeColor);
        }
        else
        {
            LifeText.color = ClientUtils.GetColorFromColorDict(AllColors.ColorType.DefaultLifeNumberColor);
        }

        LifeText.text = leftLifeValue.ToString();
    }

    IEnumerator Co_RetinueAttackChange(int retinueAttackValue, int retinueEnergy, int retinueEnergyMax, int change, float duration, bool isInitializing)
    {
        if (!isInitializing)
        {
            string text = LanguageManager.Instance.GetText("ModuleRetinue_AttackChangeNumberFly");

            if (change > 0)
            {
                WeaponAttackChangeNumberFly.SetText(text + "+" + change, "#FFF500", "#FFF500", TextFly.FlyDirection.Up);
            }
            else if (change < 0)
            {
                WeaponAttackChangeNumberFly.SetText(text + change, "#FFF500", "#FFF500", TextFly.FlyDirection.Down);
            }
        }

        int finalAttack = retinueEnergy == 0 ? retinueAttackValue : retinueEnergy * retinueAttackValue;
        Text_RetinueAttack.text = finalAttack > 0 ? finalAttack.ToString() : "";
        RefreshSwordBarMask(retinueEnergy, retinueEnergyMax);
        yield return new WaitForSeconds(duration);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    IEnumerator Co_RetinueWeaponEnergyMaxChange(int retinueEnergy, int retinueEnergyMax, int change, float duration, bool isInitializing)
    {
        if (!isInitializing)
        {
            if (change > 0)
            {
                WeaponEnergyChangeNumberFly.SetText("Max +" + change, "#1CFF00", "#1CFF00", TextFly.FlyDirection.Up);
            }
            else if (change < 0)
            {
                WeaponEnergyChangeNumberFly.SetText("Max " + change, "#1CFF00", "#1CFF00", TextFly.FlyDirection.Down);
            }
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

    IEnumerator Co_ArmorChange(int armorValue, int change, float duration, bool isInitializing)
    {
        NormalParticle particle = null;
        if (!isInitializing)
        {
            string text = LanguageManager.Instance.GetText("ModuleRetinue_ArmorChangeNumberFly");
            if (change > 0)
            {
                ArmorChangeNumberFly.SetText(text + "+" + change, "#FFA800", "#FFA800", TextFly.FlyDirection.Up);
            }
            else if (change < 0)
            {
                HitManager.Instance.ShowHit(ArmorFillAnim.transform, HitManager.HitType.LineLeftTopToRightButtom, "#FFD217", 0.3f);
                ArmorChangeNumberFly.SetText(text + change, "#FFA800", "#FFA800", TextFly.FlyDirection.Down);
                if (armorValue != 0) AudioManager.Instance.SoundPlay("sfx/HitArmor", 0.5f);
            }

            if (armorValue == 0)
            {
                AudioManager.Instance.SoundPlay("sfx/BreakArmor", 1f);
                particle = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ParticleSystem].AllocateGameObject<NormalParticle>(ShieldBar.transform);
                particle.ParticleSystem.Play(true);
                particle.ParticleSystem.startColor = ClientUtils.HTMLColorToColor("#FFA800");
            }
        }

        if (armorValue == 0)
        {
            ArmorFillAnim.gameObject.SetActive(false);
            AudioManager.Instance.SoundPlay("sfx/BreakArmor", 1f);
            particle = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ParticleSystem].AllocateGameObject<NormalParticle>(ShieldBar.transform);
            particle.ParticleSystem.Play(true);
            particle.ParticleSystem.startColor = ClientUtils.HTMLColorToColor("#FFA800");
        }
        else
        {
            ArmorFillAnim.gameObject.SetActive(true);
            Text_RetinueArmor.text = armorValue.ToString();
        }

        ArmorFillAnim.SetTrigger("Jump");

        yield return new WaitForSeconds(duration);

        if (particle != null) particle.PoolRecycle();
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    [SerializeField] private int RetinueShieldFull;

    IEnumerator Co_ShieldChange(int shieldValue, int change, float duration, bool isInitializing)
    {
        NormalParticle particle = null;
        if (!isInitializing)
        {
            string text = LanguageManager.Instance.GetText("ModuleRetinue_ShieldChangeNumberFly");
            RetinueShieldFull = Mathf.Max(RetinueShieldFull, shieldValue);
            if (change > 0)
            {
                ShieldChangeNumberFly.SetText(text + "+" + change, "#00FFF2", "#00FFF2", TextFly.FlyDirection.Up);
            }
            else
            {
                if (change < 0) ShieldChangeNumberFly.SetText(text + change, "#00FFF2", "#00FFF2", TextFly.FlyDirection.Down);
                HitManager.Instance.ShowHit(ShieldBar.transform, HitManager.HitType.LineRightTopToLeftButtom, "#2BFFF8", 0.3f);
                if (shieldValue != 0) AudioManager.Instance.SoundPlay("sfx/HitShield", 1f);
            }

            if (shieldValue == 0)
            {
                AudioManager.Instance.SoundPlay("sfx/BreakShield", 1f);
                particle = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ParticleSystem].AllocateGameObject<NormalParticle>(ShieldBar.transform);
                particle.ParticleSystem.Play(true);
                particle.ParticleSystem.startColor = ClientUtils.HTMLColorToColor("#00FFF2");
            }
        }
        else
        {
            RetinueShieldFull = shieldValue;
        }

        if (shieldValue == 0)
        {
            AudioManager.Instance.SoundPlay("sfx/BreakShield", 1f);
            particle = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ParticleSystem].AllocateGameObject<NormalParticle>(ShieldBar.transform);
            particle.ParticleSystem.Play(true);
            particle.ParticleSystem.startColor = ClientUtils.HTMLColorToColor("#00FFF2");

            ShieldBar.fillAmount = 0;
            Text_RetinueShield.text = "";
        }
        else
        {
            ShieldBar.fillAmount = (float) shieldValue / (float) RetinueShieldFull;
            Text_RetinueShield.text = shieldValue.ToString();
        }

        ShieldBarAnim.SetTrigger("Jump");
        yield return new WaitForSeconds(duration);
        if (particle != null) particle.PoolRecycle();
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #region RetinueTypeLooking

    [SerializeField] private GameObject DefenceShow;
    [SerializeField] private GameObject DefenceHoverShow;

    [SerializeField] private GameObject SentryShow;
    [SerializeField] private GameObject SentryHoverShow;

    [SerializeField] private GameObject FrenzyShow;
    [SerializeField] private GameObject FrenzyHoverShow;

    [SerializeField] private GameObject SniperShow;
    [SerializeField] private GameObject SniperHoverShow;

    public bool IsFrenzy
    {
        get
        {
            return CardInfo.RetinueInfo.IsFrenzy ||
                   (M_Weapon != null && M_Weapon.CardInfo.WeaponInfo.IsFrenzy) ||
                   (M_Pack != null && M_Pack.CardInfo.PackInfo.IsFrenzy) ||
                   (M_MA != null && M_MA.CardInfo.MAInfo.IsFrenzy);
        }
    }

    public bool IsSentry
    {
        get { return (M_Weapon != null && M_Weapon.CardInfo.WeaponInfo.IsSentry); }
    }

    public bool IsSniper
    {
        get
        {
            return CardInfo.RetinueInfo.IsSniper ||
                   (M_Pack != null && M_Pack.CardInfo.PackInfo.IsSniper) ||
                   (M_MA != null && M_MA.CardInfo.MAInfo.IsSniper);
        }
    }

    public bool IsDefender
    {
        get
        {
            return CardInfo.RetinueInfo.IsDefence ||
                   (M_Shield != null && M_Shield.CardInfo.ShieldInfo.IsDefence) ||
                   (M_Pack != null && M_Pack.CardInfo.PackInfo.IsDefence) ||
                   (M_MA != null && M_MA.CardInfo.MAInfo.IsDefence);
        }
    }

    public int DodgeProp
    {
        get
        {
            if (M_Pack != null) return M_Pack.CardInfo.PackInfo.DodgeProp;
            return 0;
        }
    }

    void ShowRetinueTypeLooking()
    {
        if (IsFrenzy)
        {
            FrenzyShow.SetActive(!IsFrenzy);
            FrenzyHoverShow.SetActive(IsFrenzy);
        }

        if (IsSentry)
        {
            SentryShow.SetActive(!IsSentry);
            SentryHoverShow.SetActive(IsSentry);
        }

        if (IsSniper)
        {
            SniperShow.SetActive(!IsSniper);
            SniperHoverShow.SetActive(IsSniper);
        }

        if (IsDefender)
        {
            DefenceShow.SetActive(!IsDefender);
            DefenceHoverShow.SetActive(IsDefender);
        }
    }

    void HideRetinueTypeLooking()
    {
        FrenzyShow.SetActive(IsFrenzy);
        FrenzyHoverShow.SetActive(false);

        SentryShow.SetActive(IsSentry);
        SentryHoverShow.SetActive(false);

        SniperShow.SetActive(IsSniper);
        SniperHoverShow.SetActive(false);

        DefenceShow.SetActive(IsDefender);
        DefenceHoverShow.SetActive(false);
    }

    #endregion

    #region 拼装上的模块

    internal bool IsAllEquipExceptMA
    {
        get
        {
            if (CardInfo.RetinueInfo.Slots[0] != SlotTypes.None && M_Weapon == null) return false;
            if (CardInfo.RetinueInfo.Slots[1] != SlotTypes.None && M_Shield == null) return false;
            if (CardInfo.RetinueInfo.Slots[2] != SlotTypes.None && M_Pack == null) return false;
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

    void On_WeaponDown()
    {
        AudioManager.Instance.SoundPlay("sfx/OnEquipDown");
        HideRetinueTypeLooking();
    }

    void On_WeaponEquiped()
    {
        M_Weapon.OnWeaponEquiped();
        HideRetinueTypeLooking();
        AudioManager.Instance.SoundPlay("sfx/OnEquipWeapon");
    }

    void On_WeaponChanged()
    {
        M_Weapon.OnWeaponEquiped();
        HideRetinueTypeLooking();
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

    void On_ShieldDown()
    {
        AudioManager.Instance.SoundPlay("sfx/OnEquipDown");
        HideRetinueTypeLooking();
    }

    void On_ShieldEquiped()
    {
        M_Shield.OnShieldEquiped();
        HideRetinueTypeLooking();
        AudioManager.Instance.SoundPlay("sfx/OnEquipShield");
    }

    void On_ShieldChanged()
    {
        M_Shield.OnShieldEquiped();
        HideRetinueTypeLooking();
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

    void On_PackDown()
    {
        AudioManager.Instance.SoundPlay("sfx/OnEquipDown");
    }

    void On_PackEquiped()
    {
        M_Pack.OnPackEquiped();
        AudioManager.Instance.SoundPlay("sfx/OnEquipPack");
    }

    void On_PackChanged()
    {
        M_Pack.OnPackEquiped();
        AudioManager.Instance.SoundPlay("sfx/OnEquipPack");
    }

    #endregion

    #region MA相关

    [SerializeField] private GameObject MA_BG;

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

    void On_MADown()
    {
        AudioManager.Instance.SoundPlay("sfx/OnEquipDown");
        MA_BG.SetActive(false);
    }

    void On_MAEquiped()
    {
        M_MA.OnMAEquiped();
        MA_BG.SetActive(true);
        AudioManager.Instance.SoundPlay("sfx/OnEquipMA");
    }

    void On_MAChanged()
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
            RetinueCanAttackBloom.gameObject.SetActive(canAttack);
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
            if (M_Weapon == null || M_RetinueWeaponEnergy == 0) return AttackLevel.Sword;
            if (M_Weapon.M_WeaponType == WeaponTypes.Gun) return AttackLevel.Gun;
            if (M_Weapon.M_WeaponType == WeaponTypes.SniperGun) return AttackLevel.SniperGun;
            return AttackLevel.Sword;
        }
    }

    public void Attack(ModuleRetinue targetRetinue, bool isCounterAttack)
    {
        //BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_Attack(targetRetinue, isCounterAttack), "Co_Attack");
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
        if (M_RetinueWeaponEnergy != 0) return M_RetinueAttack * M_RetinueWeaponEnergy;
        else return M_RetinueAttack;
    }

    public int CalculateCounterAttack(ModuleRetinue targetRetinue) //计算对方反击数值
    {
        bool enemyUseGun = targetRetinue.M_Weapon && targetRetinue.M_Weapon.M_WeaponType == WeaponTypes.Gun && targetRetinue.M_RetinueWeaponEnergy != 0;

        int damage = 0;
        if (M_Weapon && M_RetinueWeaponEnergy != 0)
        {
            switch (M_Weapon.M_WeaponType)
            {
                case WeaponTypes.Sword:
                    if (enemyUseGun) damage = 0; //无远程武器不能反击枪械攻击
                    else
                    {
                        if (IsFrenzy)
                        {
                            damage = M_RetinueWeaponFinalAttack * 2;
                        }
                        else
                        {
                            damage = M_RetinueWeaponFinalAttack;
                        }
                    }

                    break;
                case WeaponTypes.Gun: //有远程武器可以反击，只反击一点子弹
                    if (IsFrenzy)
                    {
                        damage = M_RetinueAttack * 2;
                    }
                    else
                    {
                        damage = M_RetinueAttack;
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
                    damage = M_RetinueAttack * 2;
                }
                else
                {
                    damage = M_RetinueAttack;
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
                DefenceText.enabled = IsDefender;
                RetinueTargetPreviewAnim.SetBool("ShowTarget", true);
            }
            else if (!value && targetPreviewArrowShow)
            {
                DefenceText.enabled = false;
                RetinueTargetPreviewAnim.SetBool("ShowTarget", false);
            }

            targetPreviewArrowShow = value;
        }
    }

    public void ShowTargetPreviewArrow(bool beSniperTargeted = false)
    {
        TargetPreviewArrowShow = true;
        SniperTargetImage.enabled = beSniperTargeted && !IsDefender;
    }

    public void HideTargetPreviewArrow()
    {
        TargetPreviewArrowShow = false;
    }

    public void ShowSniperTipText()
    {
        SniperTipText.enabled = true;
    }

    public void HideSniperTipText()
    {
        SniperTipText.enabled = false;
    }

    public override void DragComponent_OnMouseUp(BoardAreaTypes boardAreaType, List<Slot> slots, ModuleRetinue moduleRetinue, Ship ship, Vector3 dragLastPosition, Vector3 dragBeginPosition, Quaternion dragBeginQuaternion)
    {
        base.DragComponent_OnMouseUp(boardAreaType, slots, moduleRetinue, ship, dragLastPosition, dragBeginPosition, dragBeginQuaternion);
        RoundManager.Instance.HideTargetPreviewArrow();
        if (moduleRetinue)
        {
            if (moduleRetinue.CheckModuleRetinueCanAttackMe(this))
            {
                RetinueAttackRetinueRequest request = new RetinueAttackRetinueRequest(ClientPlayer.ClientId, M_RetinueID, RoundManager.Instance.EnemyClientPlayer.ClientId, moduleRetinue.M_RetinueID);
                Client.Instance.Proxy.SendMessage(request);
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("ModuleRetinue_ShouldAttackDefenderFirst"), 0, 0.5f);
            }
        }
        else if (ship)
        {
            if (ship.CheckModuleRetinueCanAttackMe(this) != 0)
            {
                RetinueAttackShipRequest request = new RetinueAttackShipRequest(Client.Instance.Proxy.ClientId, M_RetinueID);
                Client.Instance.Proxy.SendMessage(request);
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("ModuleRetinue_ShouldAttackMechsFirst"), 0, 0.5f);
            }
        }

        DragManager.Instance.DragOutDamage = 0;
    }

    public override void DragComponent_SetStates(ref bool canDrag, ref DragPurpose dragPurpose)
    {
        canDrag = CanAttack && ClientPlayer == RoundManager.Instance.CurrentClientPlayer && ClientPlayer == RoundManager.Instance.SelfClientPlayer && !ClientPlayer.MyBattleGroundManager.RemoveRetinues.Contains(this);
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
        RoundManager.Instance.ShowRetinueAttackPreviewArrow(this);
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
                ClientUtils.ChangeColor(OnHoverBloom, ClientUtils.GetColorFromColorDict(AllColors.ColorType.RetinueOnEnemyHoverBloomColor));
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
            if (mr != null && CheckModuleRetinueCanAttackMe(mr))
            {
                IsBeDraggedHover = true;
                if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
                {
                    ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
                }

                DamageNumberPreviewTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
                DamageNumberPreviewBGTextMesh.text = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;

                ShowRetinueTypeLooking();

                int myCounterAttack = CalculateCounterAttack(mr);
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

    private bool CheckModuleRetinueCanAttackMe(ModuleRetinue attackRetinue)
    {
        if (attackRetinue == this) return false;
        if (attackRetinue.ClientPlayer == ClientPlayer) return false;
        if (RoundManager.Instance.EnemyClientPlayer.MyBattleGroundManager.RemoveRetinues.Contains(this)) return false;
        if (attackRetinue.M_Weapon && attackRetinue.M_Weapon.M_WeaponType == WeaponTypes.SniperGun && attackRetinue.M_RetinueWeaponEnergy != 0) return true; //狙击枪可以越过嘲讽机甲，其他武器只能攻击嘲讽机甲
        if (ClientPlayer.MyBattleGroundManager.HasDefenceRetinue && !IsDefender) return false;
        return true;
    }

    private bool CheckCardSpellCanTarget(CardSpell card)
    {
        if (card.ClientPlayer == ClientPlayer)
        {
            return ((card.CardInfo.TargetInfo.targetRetinueRange & TargetSideEffect.TargetRange.SelfSoldiers) == TargetSideEffect.TargetRange.SelfSoldiers && CardInfo.RetinueInfo.IsSoldier) ||
                   ((card.CardInfo.TargetInfo.targetRetinueRange & TargetSideEffect.TargetRange.SelfHeroes) == TargetSideEffect.TargetRange.SelfHeroes && !CardInfo.RetinueInfo.IsSoldier);
        }
        else
        {
            return ((card.CardInfo.TargetInfo.targetRetinueRange & TargetSideEffect.TargetRange.EnemySoldiers) == TargetSideEffect.TargetRange.EnemySoldiers && CardInfo.RetinueInfo.IsSoldier) ||
                   ((card.CardInfo.TargetInfo.targetRetinueRange & TargetSideEffect.TargetRange.EnemyHeros) == TargetSideEffect.TargetRange.EnemyHeros && !CardInfo.RetinueInfo.IsSoldier);
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

        HideRetinueTypeLooking();

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
                    ClientUtils.ChangeColor(OnHoverBloom, ClientUtils.GetColorFromColorDict(AllColors.ColorType.RetinueOnEnemyHoverBloomColor));
                }
                else
                {
                    ClientUtils.ChangeColor(OnHoverBloom, ClientUtils.GetColorFromColorDict(AllColors.ColorType.RetinueOnSelfHoverBloomColor));
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
                 (targetRange == TargetSideEffect.TargetRange.EnemyMechs ||
                  (targetRange == TargetSideEffect.TargetRange.EnemySoldiers && CardInfo.RetinueInfo.IsSoldier) ||
                  targetRange == TargetSideEffect.TargetRange.EnemyHeros && !CardInfo.RetinueInfo.IsSoldier))
                ||
                ClientPlayer == RoundManager.Instance.SelfClientPlayer && ClientPlayer.MyBattleGroundManager.CurrentSummonPreviewRetinue != this &&
                (targetRange == TargetSideEffect.TargetRange.SelfMechs || (targetRange == TargetSideEffect.TargetRange.SelfSoldiers && CardInfo.RetinueInfo.IsSoldier)))
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
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShowSideEffectBloom(triggerTime, triggerRange, ClientUtils.HTMLColorToColor("#00FFDA"), 0.4f), "ShowSideEffectBloom");
    }

    IEnumerator Co_ShowSideEffectBloom(SideEffectBundle.TriggerTime triggerTime, SideEffectBundle.TriggerRange triggerRange, Color color, float duration)
    {
        if (triggerTime == SideEffectBundle.TriggerTime.OnRetinueDie && triggerRange == SideEffectBundle.TriggerRange.Self)
        {
            SideEffectBGDieAnim.SetTrigger("Jump");
        }
        else
        {
            SideEffectBGCommonAnim.SetTrigger("Jump");
        }

        SideEffcetBloom.gameObject.SetActive(true);
        ClientUtils.ChangeColor(SideEffcetBloom, color, 2);
        AudioManager.Instance.SoundPlay("sfx/OnSE");
        yield return new WaitForSeconds(duration);
        SideEffcetBloom.gameObject.SetActive(false);
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

    public void OnAttack(WeaponTypes weaponType, ModuleRetinue targetRetinue)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_OnAttack(weaponType, targetRetinue), "Co_OnAttack");
    }

    IEnumerator Co_OnAttack(WeaponTypes weaponType, ModuleRetinue targetRetinue)
    {
        switch (weaponType)
        {
            case WeaponTypes.None:
            {
                Vector3 oriPos = transform.position;
                iTween.MoveTo(gameObject, targetRetinue.GetClosestHitPos(transform.position), 0.1f);
                yield return new WaitForSeconds(0.15f);
                AudioManager.Instance.SoundPlay("sfx/AttackSword");
                iTween.MoveTo(gameObject, oriPos, 0.1f);
                break;
            }
            case WeaponTypes.Sword:
            {
                Vector3 oriPos = transform.position;
                iTween.MoveTo(gameObject, targetRetinue.GetClosestHitPos(transform.position), 0.1f);
                yield return new WaitForSeconds(0.15f);
                AudioManager.Instance.SoundPlay("sfx/AttackSword");
                iTween.MoveTo(gameObject, oriPos, 0.1f);
                break;
            }
            case WeaponTypes.Gun:
            {
                Bullet bl = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Bullet].AllocateGameObject<Bullet>(transform);
                bl.Move(transform.position, targetRetinue.transform.position, Color.white, Color.white, 1, 0.03f, 0.1f, 0);
                AudioManager.Instance.SoundPlay("sfx/AttackGun");
                break;
            }
            case WeaponTypes.SniperGun:
            {
                Bullet bl = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.Bullet].AllocateGameObject<Bullet>(transform);
                bl.Move(transform.position, targetRetinue.transform.position, Color.yellow, Color.yellow, 1, 0.03f, 0.15f, 0);
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
                iTween.MoveTo(gameObject, targetShip.GetClosestHitPosition(transform.position), 0.15f);
                yield return new WaitForSeconds(0.17f);
                AudioManager.Instance.SoundPlay("sfx/AttackNone");
                iTween.MoveTo(gameObject, oriPos, 0.15f);
                yield return new WaitForSeconds(0.17f);
                break;
            }
            case WeaponTypes.Sword:
            {
                Vector3 oriPos = transform.position;
                iTween.MoveTo(gameObject, targetShip.GetClosestHitPosition(transform.position), 0.15f);
                yield return new WaitForSeconds(0.17f);
                AudioManager.Instance.SoundPlay("sfx/AttackSword");
                iTween.MoveTo(gameObject, oriPos, 0.15f);
                yield return new WaitForSeconds(0.17f);
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

    public void OnDodge()
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_OnDodge(), "Co_OnDodge");
    }

    IEnumerator Co_OnDodge()
    {
        AudioManager.Instance.SoundPlay("sfx/HitShield");
        DodgeNumberFly.SetText((LanguageManager.Instance.GetText("KeyWords_Dodge")), "#AE70FF", "#AE70FF", TextFly.FlyDirection.Up, showArrow: false);
        yield return new WaitForSeconds(0.1f);
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