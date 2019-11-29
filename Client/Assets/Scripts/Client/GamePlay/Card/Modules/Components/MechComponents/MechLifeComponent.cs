using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class MechLifeComponent : MechComponentBase
{
    [SerializeField] private TextMeshPro LifeText;
    [SerializeField] private MeshRenderer LifeIcon;
    [SerializeField] private Animator LifeIconAnim;
    [SerializeField] private SortingGroup LifeIconSG;
    [SerializeField] private TextFlyPile LifeChangeNumberFly;
    [SerializeField] private TextFlyPile TotalLifeChangeNumberFly;

    public void ChangeLifeIconColor(Color color)
    {
        ClientUtils.ChangeColor(LifeIcon, color);
    }

    void Awake()
    {
        Reset();
        LifeTextDefaultSortingOrder = LifeText.sortingOrder;
        LifeIconDefaultSortingOrder = LifeIconSG.sortingOrder;
    }

    private int LifeTextDefaultSortingOrder;
    private int LifeIconDefaultSortingOrder;

    protected override void Child_Initialize()
    {
    }

    internal bool IsTotalLifeChanging = false;

    public void LifeChange(int leftLife, int totalLife, int before_leftLife, bool cur_IsInitializing)
    {
        float duration = Mech.IsInitializing ? 0 : 0.1f;
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_LifeChange(leftLife, totalLife, leftLife - before_leftLife, duration * BattleEffectsManager.AnimationSpeed, IsTotalLifeChanging, cur_IsInitializing), "Co_LifeChange");
    }

    private IEnumerator Co_LifeChange(int leftLifeValue, int totalLifeValue, int change, float duration, bool isTotalLifeChanging, bool cur_IsInitializing)
    {
        if (!cur_IsInitializing)
        {
            string text = LanguageManager.Instance.GetText("ModuleMech_LifeNumberFly");
            if (change > 0)
            {
                if (!isTotalLifeChanging)
                {
                    LifeChangeNumberFly.SetText(text + "+" + change, "#FFFFFF", "#FFFFFF", TextFly.FlyDirection.Up);
                    AudioManager.Instance.SoundPlay("sfx/OnHeal");
                }
            }
            else if (change < 0)
            {
                FXManager.Instance.PlayFX(LifeIcon.transform, FXManager.FXType.FX_Hit0, "#FFFFFF", 0.3f);
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

        LifeTextChange(leftLifeValue, totalLifeValue);
        yield return new WaitForSeconds(duration);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private void LifeTextChange(int leftLifeValue, int totalLifeValue)
    {
        LifeIconAnim.SetTrigger("Jump");
        if (leftLifeValue < totalLifeValue)
        {
            LifeText.color = ClientUtils.GetColorFromColorDict(AllColors.ColorType.InjuredLifeNumberColor);
        }
        else if (leftLifeValue == totalLifeValue && Mech != null && totalLifeValue > Mech.CardInfo.LifeInfo.TotalLife)
        {
            LifeText.color = ClientUtils.GetColorFromColorDict(AllColors.ColorType.OverFlowTotalLifeColor);
        }
        else
        {
            LifeText.color = ClientUtils.GetColorFromColorDict(AllColors.ColorType.DefaultLifeNumberColor);
        }

        LifeText.text = leftLifeValue.ToString();
    }

    public void TotalLifeChange(int leftLife, int totalLife, int before_totalLife, bool cur_IsInitializing)
    {
        float duration = Mech.IsInitializing ? 0 : 0.1f;
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_TotalLifeChange(leftLife, totalLife, totalLife - before_totalLife, duration * BattleEffectsManager.AnimationSpeed, cur_IsInitializing), "Co_TotalLifeChange");
    }

    private IEnumerator Co_TotalLifeChange(int leftLifeValue, int totalLifeValue, int change, float duration, bool cur_IsInitializing)
    {
        if (!cur_IsInitializing)
        {
            if (change > 0)
            {
                TotalLifeChangeNumberFly.SetText("Max +" + change, "#68FF00", "#68FF00", TextFly.FlyDirection.Up);
                FXManager.Instance.PlayFX(transform.parent, FXManager.FXType.FX_MechAddLife, "#FFFFFF", 0.3f, 3f);
                AudioManager.Instance.SoundPlay("sfx/OnAddLife");
            }
            else if (change < 0)
            {
                TotalLifeChangeNumberFly.SetText("Max " + change, "#A000FF", "#A000FF", TextFly.FlyDirection.Down);
            }
        }

        LifeTextChange(leftLifeValue, totalLifeValue);
        yield return new WaitForSeconds(duration * BattleEffectsManager.AnimationSpeed);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    protected override void Reset()
    {
        LifeTextChange(0, 0);
        IsTotalLifeChanging = false;
    }

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        LifeText.sortingOrder = cardSortingIndex * 50 + LifeTextDefaultSortingOrder;
        LifeIconSG.sortingOrder = cardSortingIndex * 50 + LifeIconDefaultSortingOrder;
    }
}