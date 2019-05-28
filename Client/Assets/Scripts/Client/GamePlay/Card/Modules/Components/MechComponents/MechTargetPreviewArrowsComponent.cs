using TMPro;
using UnityEngine;

public class MechTargetPreviewArrowsComponent : MechComponentBase
{
    [SerializeField] private SpriteRenderer[] TargetArrows;
    [SerializeField] private Animator TargetArrowsAnim;
    [SerializeField] private SpriteRenderer SniperTargetImage;
    [SerializeField] private TextMeshPro DamagePreviewBG;
    [SerializeField] private TextMeshPro DamagePreview; //受攻击瞄准时的伤害预览
    [SerializeField] private TextMeshPro DefenderText;
    [SerializeField] private TextMeshPro SniperText;

    void Awake()
    {
        Reset();
        TargetArrowsDefaultSortingOrder = TargetArrows[0].sortingOrder;
        SniperTargetImageDefaultSortingOrder = SniperTargetImage.sortingOrder;
        DamagePreviewDefaultSortingOrder = DamagePreviewBG.sortingOrder;
        TextDefaultSortingOrder = DefenderText.sortingOrder;
    }

    private int TargetArrowsDefaultSortingOrder;
    private int SniperTargetImageDefaultSortingOrder;
    private int DamagePreviewDefaultSortingOrder;
    private int TextDefaultSortingOrder;

    protected override void Child_Initialize()
    {
    }

    protected override void Reset()
    {
        SetDamagePreviewText("");
        TargetArrowAnimEnd();
        ShowSniperTargetImage(false);
        ShowDefenderText(false);
        ShowSniperText(false);
    }

    public void OnMousePressEnterImmediately()
    {
        if (DragManager.Instance.CurrentDrag)
        {
            ModuleMech mr = DragManager.Instance.CurrentDrag_ModuleMech;
            CardSpell cs = DragManager.Instance.CurrentDrag_CardSpell;
            string dragOutString = DragManager.Instance.DragOutDamage == 0 ? "" : "-" + DragManager.Instance.DragOutDamage;
            if (mr != null && Mech.CheckModuleMechCanAttackMe(mr))
            {
                SetDamagePreviewText(dragOutString);
                int myCounterAttack = Mech.CalculateCounterAttack(mr);
                mr.MechTargetPreviewArrowsComponent.SetDamagePreviewText(myCounterAttack == 0 ? "" : "-" + myCounterAttack);
            }
            else if (cs != null && Mech.CheckCardSpellCanTarget(cs))
            {
                SetDamagePreviewText(dragOutString);
            }
        }
    }

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        foreach (SpriteRenderer sp in TargetArrows)
        {
            sp.sortingOrder = cardSortingIndex * 50 + TargetArrowsDefaultSortingOrder;
        }

        SniperTargetImage.sortingOrder = cardSortingIndex * 50 + SniperTargetImageDefaultSortingOrder;
        DamagePreviewBG.sortingOrder = cardSortingIndex * 50 + DamagePreviewDefaultSortingOrder;
        DamagePreview.sortingOrder = cardSortingIndex * 50 + DamagePreviewDefaultSortingOrder;
        DefenderText.sortingOrder = cardSortingIndex * 50 + TextDefaultSortingOrder;
        SniperText.sortingOrder = cardSortingIndex * 50 + TextDefaultSortingOrder;
    }

    public void SetDamagePreviewText(string text)
    {
        DamagePreview.text = text;
        DamagePreviewBG.text = text;
    }

    public void TargetArrowAnimStart()
    {
        TargetArrowsAnim.gameObject.SetActive(true);
        TargetArrowsAnim.enabled = true;
        TargetArrowsAnim.SetTrigger("ShowTarget");
    }

    public void TargetArrowAnimEnd()
    {
        TargetArrowsAnim.enabled = false;
        TargetArrowsAnim.gameObject.SetActive(false);
    }

    public void ShowSniperTargetImage(bool isShow)
    {
        SniperTargetImage.gameObject.SetActive(isShow);
    }

    public void ShowDefenderText(bool isShow)
    {
        DefenderText.gameObject.SetActive(isShow);
    }

    public void ShowSniperText(bool isShow)
    {
        SniperText.gameObject.SetActive(isShow);
    }
}