using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MechSwordShieldArmorComponent : MechComponentBase
{
    [SerializeField] private Animator SwordIconAnim;
    [SerializeField] private Image SwordBar;
    [SerializeField] private SpriteRenderer SwordTrough;
    [SerializeField] private TextMeshPro SwordText;
    [SerializeField] private TextFlyPile WeaponAttackChangeNumberFly;
    [SerializeField] private TextFlyPile WeaponEnergyChangeNumberFly;

    [SerializeField] private Animator ShieldIconAnim;
    [SerializeField] private Image ShieldBar;
    [SerializeField] private SpriteRenderer ShieldTrough;
    [SerializeField] private TextMeshPro ShieldText;
    [SerializeField] private TextFlyPile ShieldChangeNumberFly;
    [SerializeField] private TextFlyPile ShieldDefenceNumberFly;

    [SerializeField] private Animator ArmorIconAnim;
    [SerializeField] private SpriteRenderer ArmorIcon;
    [SerializeField] private SpriteRenderer ArmorTrough;
    [SerializeField] private TextMeshPro ArmorText;
    [SerializeField] private TextFlyPile ArmorChangeNumberFly;

    [SerializeField] private TextFlyPile DodgeNumberFly;

    public GameObject MA_BG;

    private void Awake()
    {
        TextDefaultSortingOrder = SwordText.sortingOrder;
        BarDefaultSortingOrder = SwordBar.canvas.sortingOrder;
        TroughDefaultSortingOrder = SwordTrough.sortingOrder;
    }

    private int TextDefaultSortingOrder;
    private int BarDefaultSortingOrder;
    private int TroughDefaultSortingOrder;

    protected override void Child_Initialize()
    {
    }

    protected override void Reset()
    {
        MechShieldFull = 0;
        IsAttackChanging = false;
        MA_BG.SetActive(false);
    }

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        SwordText.sortingOrder = cardSortingIndex * 50 + TextDefaultSortingOrder;
        ShieldText.sortingOrder = cardSortingIndex * 50 + TextDefaultSortingOrder;
        ArmorText.sortingOrder = cardSortingIndex * 50 + TextDefaultSortingOrder;

        SwordBar.canvas.sortingOrder = cardSortingIndex * 50 + BarDefaultSortingOrder;
        ShieldBar.canvas.sortingOrder = cardSortingIndex * 50 + BarDefaultSortingOrder;
        ArmorIcon.sortingOrder = cardSortingIndex * 50 + BarDefaultSortingOrder;

        SwordTrough.sortingOrder = cardSortingIndex * 50 + TroughDefaultSortingOrder;
        ShieldTrough.sortingOrder = cardSortingIndex * 50 + TroughDefaultSortingOrder;
        ArmorTrough.sortingOrder = cardSortingIndex * 50 + TroughDefaultSortingOrder;
    }

    #region Attack

    public void AttackChange(int value, int finalAttack_result)
    {
        float duration = Mech.IsInitializing ? 0 : 0.1f;
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(
            Co_MechAttackChange(value, Mech.M_MechWeaponEnergy, Mech.M_MechWeaponEnergyMax, finalAttack_result - Mech.M_MechWeaponFinalAttack, duration, Mech.IsInitializing), "Co_MechAttackChange");
    }

    internal bool IsAttackChanging = false;

    public void WeaponEnergyChange(int value, int finalAttack_result)
    {
        float duration = Mech.IsInitializing ? 0 : 0.1f;
        if (!Mech.IsInitializing && Mech.M_MechAttack != 0 && !IsAttackChanging)
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(
                Co_MechAttackChange(Mech.M_MechAttack, value, Mech.M_MechWeaponEnergyMax, finalAttack_result - Mech.M_MechWeaponFinalAttack, duration, Mech.IsInitializing), "Co_MechAttackChange");
    }

    public void WeaponEnergyMaxChange(int value)
    {
        float duration = Mech.IsInitializing ? 0 : 0.1f;
        if (!Mech.IsInitializing)
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(
                Co_MechWeaponEnergyMaxChange(Mech.M_MechWeaponEnergy, value, value - Mech.M_MechWeaponEnergyMax, duration, Mech.IsInitializing), "Co_MechWeaponEnergyMaxChange");
    }

    IEnumerator Co_MechAttackChange(int mechAttackValue, int mechEnergy, int mechEnergyMax, int change, float duration, bool cur_IsInitializing)
    {
        if (!cur_IsInitializing)
        {
            string text = LanguageManager.Instance.GetText("ModuleMech_AttackChangeNumberFly");

            if (change > 0)
            {
                WeaponAttackChangeNumberFly.SetText(text + "+" + change, "#FFF500", "#FFF500", TextFly.FlyDirection.Up);
            }
            else if (change < 0)
            {
                WeaponAttackChangeNumberFly.SetText(text + change, "#FFF500", "#FFF500", TextFly.FlyDirection.Down);
            }
        }

        int finalAttack = ModuleMech.CalculateFinalAttack(mechAttackValue, mechEnergy);
        SwordText.text = finalAttack > 0 ? finalAttack.ToString() : "";
        SwordTrough.color = finalAttack > 0 ? ClientUtils.HTMLColorToColor("#ffffff") : ClientUtils.HTMLColorToColor("#333333");
        RefreshSwordBarMask(mechEnergy, mechEnergyMax);
        yield return new WaitForSeconds(duration);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    IEnumerator Co_MechWeaponEnergyMaxChange(int mechEnergy, int mechEnergyMax, int change, float duration, bool cur_IsInitializing)
    {
        if (!cur_IsInitializing)
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

        RefreshSwordBarMask(mechEnergy, mechEnergyMax);
        yield return new WaitForSeconds(duration);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private void RefreshSwordBarMask(int mechWeaponEnergyValue, int mechWeaponEnergyMaxValue)
    {
        if (mechWeaponEnergyMaxValue != 0)
        {
            SwordBar.fillAmount = (float) mechWeaponEnergyValue / mechWeaponEnergyMaxValue;
            SwordIconAnim.SetTrigger("Jump");
        }
        else
        {
            SwordBar.fillAmount = 0;
        }
    }

    #endregion

    #region Armor

    public void ArmorChange(int mechArmor, int before_mechArmor, bool cur_IsInitializing)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ArmorChange(mechArmor, mechArmor - before_mechArmor, 0, cur_IsInitializing), "Co_ArmorChange");
    }

    IEnumerator Co_ArmorChange(int armorValue, int change, float duration, bool cur_IsInitializing)
    {
        NormalParticle particle = null;
        if (!cur_IsInitializing)
        {
            string text = LanguageManager.Instance.GetText("ModuleMech_ArmorChangeNumberFly");
            if (change > 0)
            {
                ArmorChangeNumberFly.SetText(text + "+" + change, "#FFA800", "#FFA800", TextFly.FlyDirection.Up);
            }
            else if (change < 0)
            {
                HitManager.Instance.ShowHit(ArmorIconAnim.transform, HitManager.HitType.LineLeftTopToRightButtom, "#FFD217", 0.3f);
                ArmorChangeNumberFly.SetText(text + change, "#FFA800", "#FFA800", TextFly.FlyDirection.Down);
                if (armorValue != 0) AudioManager.Instance.SoundPlay("sfx/HitArmor", 0.5f);
            }

            if (armorValue == 0)
            {
                AudioManager.Instance.SoundPlay("sfx/BreakArmor", 1f);
//                particle = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ParticleSystem].AllocateGameObject<NormalParticle>(ArmorIconAnim.transform);
//                particle.ParticleSystem.Play(true);
//                particle.ParticleSystem.startColor = ClientUtils.HTMLColorToColor("#FFA800");
            }
        }

        if (armorValue == 0)
        {
            ArmorIconAnim.gameObject.SetActive(false);
            ArmorTrough.color = ClientUtils.HTMLColorToColor("#333333");
//            particle = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ParticleSystem].AllocateGameObject<NormalParticle>(ArmorIconAnim.transform);
//            particle.ParticleSystem.Play(true);
//            particle.ParticleSystem.startColor = ClientUtils.HTMLColorToColor("#FFA800");
        }
        else
        {
            ArmorIconAnim.gameObject.SetActive(true);
            ArmorTrough.color = ClientUtils.HTMLColorToColor("#ffffff");
            ArmorText.text = armorValue.ToString();
        }

        ArmorIconAnim.SetTrigger("Jump");

        yield return new WaitForSeconds(duration);

        if (particle != null) particle.PoolRecycle();
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #region Shield

    public void ShieldChange(int mechShield, int before_mechShield, bool cur_IsInitializing)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShieldChange(mechShield, mechShield - before_mechShield, 0, cur_IsInitializing), "Co_ShieldChange");
    }

    private int MechShieldFull;

    IEnumerator Co_ShieldChange(int shieldValue, int change, float duration, bool cur_IsInitializing)
    {
        NormalParticle particle = null;
        if (!cur_IsInitializing)
        {
            string text = LanguageManager.Instance.GetText("ModuleMech_ShieldChangeNumberFly");
            MechShieldFull = Mathf.Max(MechShieldFull, shieldValue);
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

//                particle = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ParticleSystem].AllocateGameObject<NormalParticle>(ShieldIconAnim.transform);
//                particle.ParticleSystem.Play(true);
//                particle.ParticleSystem.startColor = ClientUtils.HTMLColorToColor("#00FFF2");
            }
        }
        else
        {
            MechShieldFull = shieldValue;
        }

        if (shieldValue == 0)
        {
//            particle = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ParticleSystem].AllocateGameObject<NormalParticle>(ShieldIconAnim.transform);
//            particle.ParticleSystem.Play(true);
//            particle.ParticleSystem.startColor = ClientUtils.HTMLColorToColor("#00FFF2");
            ShieldTrough.color = ClientUtils.HTMLColorToColor("#333333");
            ShieldBar.fillAmount = 0;
            ShieldText.text = "";
        }
        else
        {
            ShieldTrough.color = ClientUtils.HTMLColorToColor("#ffffff");
            ShieldBar.fillAmount = (float) shieldValue / MechShieldFull;
            ShieldText.text = shieldValue.ToString();
        }

        ShieldIconAnim.SetTrigger("Jump");
        yield return new WaitForSeconds(duration);
        if (particle != null) particle.PoolRecycle();
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void ShieldDefenceDamage(int decreaseValue, int shieldValue)
    {
        float duration = 0;
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShieldChange(shieldValue, 0, duration, Mech.IsInitializing), "Co_ShieldChange");
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShieldChangeNumberFly(decreaseValue), "Co_ShieldChangeNumberFly");
    }

    IEnumerator Co_ShieldChangeNumberFly(int decreaseValue)
    {
        ShieldDefenceNumberFly.SetText(LanguageManager.Instance.GetText("ModuleMech_DecreaseDamageNumberFly") + decreaseValue, "#00FFF2", "#00FFF2", TextFly.FlyDirection.Down);
        yield return new WaitForSeconds(0.1f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #region Dodge

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

    #endregion
}