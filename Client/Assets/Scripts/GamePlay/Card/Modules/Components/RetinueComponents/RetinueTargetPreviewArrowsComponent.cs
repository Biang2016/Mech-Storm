using TMPro;
using UnityEngine;

public class RetinueTargetPreviewArrowsComponent : RetinueComponentBase
{
    [SerializeField] private SpriteRenderer[] TargetArrows;
    [SerializeField] private Animator TargetArrowsAnim;
    [SerializeField] private SpriteRenderer SniperTargetImage;
    [SerializeField] private TextMeshPro DamagePreviewBG;
    [SerializeField] private TextMeshPro DamagePreview;

    void Awake()
    {
        HideAll();
        TargetArrowsDefaultSortingOrder = TargetArrows[0].sortingOrder;
        SniperTargetImageDefaultSortingOrder = SniperTargetImage.sortingOrder;
        DamagePreviewDefaultSortingOrder = DamagePreviewBG.sortingOrder;
    }

    private int TargetArrowsDefaultSortingOrder;
    private int SniperTargetImageDefaultSortingOrder;
    private int DamagePreviewDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        foreach (SpriteRenderer sp in TargetArrows)
        {
            sp.sortingOrder = cardSortingIndex * 50 + TargetArrowsDefaultSortingOrder;
        }

        SniperTargetImage.sortingOrder = cardSortingIndex * 50 + SniperTargetImageDefaultSortingOrder;
        DamagePreviewBG.sortingOrder = cardSortingIndex * 50 + DamagePreviewDefaultSortingOrder;
        DamagePreview.sortingOrder = cardSortingIndex * 50 + DamagePreviewDefaultSortingOrder;
    }

    public void SetDamagePreviewTest(string text)
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

    public void HideAll()
    {
        SetDamagePreviewTest("");
        TargetArrowAnimEnd();
        ShowSniperTargetImage(false);
    }
}