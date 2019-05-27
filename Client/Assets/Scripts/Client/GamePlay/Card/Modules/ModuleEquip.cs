using System.Collections;
using TMPro;
using UnityEngine;

public abstract class ModuleEquip : ModuleBase
{
    public int M_EquipID;
    internal ModuleMech M_ModuleMech;
    public TextMeshPro Name;
    public TextMeshPro Name_en;

    public Renderer M_Bloom;
    public Renderer M_BloomSE;
    public Renderer M_BloomSE_Sub;

    public Animator EquipAnim;
    public Animator BloomSEAnim;
    public Animator BloomSE_SubAnim;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        if (EquipAnim) EquipAnim.SetTrigger("Hide");
    }

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer)
    {
        base.Initiate(cardInfo, clientPlayer);
        if (M_Bloom) M_Bloom.gameObject.SetActive(false);
        if (M_BloomSE) M_BloomSE.gameObject.SetActive(false);
        if (M_BloomSE_Sub) M_BloomSE_Sub.gameObject.SetActive(false);
        M_Name = CardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()];
        InitializeComponents();
    }

    public override void SetLanguage(string languageShort) 
    {
        M_Name = CardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()];
    }

    public abstract void OnEquipped();

    #region Preview Details

    public abstract CardInfo_Equip GetCurrentCardInfo();

    public virtual void SetPreview()
    {
        if (M_Bloom) M_Bloom.gameObject.SetActive(true);
    }

    public virtual void SetNoPreview()
    {
    }

    #endregion

    #region 属性

    private string m_Name;

    public string M_Name
    {
        get { return m_Name; }

        set
        {
            m_Name = value;
            Name.text = LanguageManager.Instance.IsEnglish ? "" : Utils.TextToVertical(value);
            Name_en.text = LanguageManager.Instance.IsEnglish ? value : "";
        }
    }

    #endregion

    #region Color

    public MeshRenderer MainBoardRenderer;
    protected float MainBoardEmissionIntensity = 1f;

    protected override void ChangeColor(Color color)
    {
        ClientUtils.ChangeColor(MainBoardRenderer, color, MainBoardEmissionIntensity);
        ClientUtils.ChangeColor(M_Bloom, color, 2);
    }

    protected override void BeDimColor()
    {
        ChangeColor(Color.gray);
    }

    protected override void BeBrightColor()
    {
        ClientUtils.ChangeColor(MainBoardRenderer, ClientUtils.HTMLColorToColor(CardInfo.GetCardColor()));
    }

    #endregion

    #region Blooms

    public override void OnShowEffects(SideEffectExecute.TriggerTime triggerTime, SideEffectExecute.TriggerRange triggerRange)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_ShowSideEffectBloom(ClientUtils.HTMLColorToColor(CardInfo.GetCardColor()), 0.5f), "ShowSideEffectBloom");
    }

    IEnumerator Co_ShowSideEffectBloom(Color color, float duration)
    {
        M_BloomSE.gameObject.SetActive(true);
        M_BloomSE_Sub.gameObject.SetActive(true);
        BloomSEAnim.SetTrigger("OnSE");
        BloomSE_SubAnim.SetTrigger("OnSE");
        ClientUtils.ChangeColor(M_BloomSE, color);
        ClientUtils.ChangeColor(M_BloomSE_Sub, color);
        AudioManager.Instance.SoundPlay("sfx/OnSE");
        yield return new WaitForSeconds(duration);
        BloomSEAnim.SetTrigger("Reset");
        BloomSE_SubAnim.SetTrigger("Reset");
        M_BloomSE.gameObject.SetActive(false);
        M_BloomSE_Sub.gameObject.SetActive(false);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void ShowEquipBloomSE(float seconds = 0.1f)
    {
        StartCoroutine(Co_ShowEquipBloomSE(seconds));
    }

    IEnumerator Co_ShowEquipBloomSE(float seconds)
    {
        ShowEquipBloomAndSE();
        yield return new WaitForSeconds(seconds);
        HideEquipBloomAndSE();
        yield return null;
    }

    private void ShowEquipBloomAndSE()
    {
        M_Bloom.gameObject.SetActive(true);
        M_BloomSE.gameObject.SetActive(true);
        M_BloomSE_Sub.gameObject.SetActive(true);
        BloomSEAnim.SetTrigger("OnSE");
        BloomSE_SubAnim.SetTrigger("OnSE");
    }

    private void HideEquipBloomAndSE()
    {
        M_Bloom.gameObject.SetActive(false);
        BloomSEAnim.SetTrigger("Reset");
        BloomSE_SubAnim.SetTrigger("Reset");
        M_BloomSE.gameObject.SetActive(false);
        M_BloomSE_Sub.gameObject.SetActive(false);
    }

    #endregion

    public override void MouseHoverComponent_OnHover1Begin(Vector3 mousePosition)
    {
        base.MouseHoverComponent_OnHover1Begin(mousePosition);
        if (M_Bloom) M_Bloom.gameObject.SetActive(true);
    }

    public override void MouseHoverComponent_OnHover1End()
    {
        base.MouseHoverComponent_OnHover1End();
        if (M_Bloom) M_Bloom.gameObject.SetActive(false);
    }

    public override void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
        base.MouseHoverComponent_OnMousePressEnterImmediately(mousePosition);
        if (DragManager.Instance.CurrentDrag_CardSpell.CardInfo.TargetInfo.HasTargetEquip)
        {
            if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
            {
                ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = true; //箭头动画
            }

            ShowEquipBloomAndSE();
        }
    }

    public override void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
        base.MouseHoverComponent_OnMousePressLeaveImmediately();
        if (DragManager.Instance.CurrentArrow && DragManager.Instance.CurrentArrow is ArrowAiming)
        {
            ((ArrowAiming) DragManager.Instance.CurrentArrow).IsOnHover = false; //箭头动画
        }

        HideEquipBloomAndSE();
    }

    public override void DragComponent_SetStates(ref bool canDrag, ref DragPurpose dragPurpose)
    {
        canDrag = false;
        dragPurpose = CardInfo.BaseInfo.DragPurpose;
    }
}