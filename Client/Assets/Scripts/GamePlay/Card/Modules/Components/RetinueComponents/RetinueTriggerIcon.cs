using UnityEngine;

public class RetinueTriggerIcon : PoolObject
{
    [SerializeField] private Sprite[] IconSprites;

    public enum IconTypes
    {
        Trigger = 0,
        Die = 1
    }

    [SerializeField] private Animator Anim;
    [SerializeField] private SpriteRenderer Icon;
    [SerializeField] private SpriteRenderer IconTrough;

    void Awake()
    {
        ShowIcon(false);
        IconDefaultSortingOrder = Icon.sortingOrder;
        IconTroughDefaultSortingOrder = IconTrough.sortingOrder;
    }

    public void Initialize(IconTypes iconType)
    {
        ShowIcon(false);
        Icon.sprite = IconSprites[(int) iconType];
    }

    internal bool IsShow = false;

    public void ShowIcon(bool isShow)
    {
        Anim.enabled = isShow;
        Anim.gameObject.SetActive(isShow);
        IsShow = isShow;
    }

    public void IconJump()
    {
        Anim.SetTrigger("Jump");
    }

    private int IconDefaultSortingOrder;
    private int IconTroughDefaultSortingOrder;

    public void SetSortingIndexOfCard(int cardSortingIndex)
    {
        Icon.sortingOrder = cardSortingIndex * 50 + IconDefaultSortingOrder;
        IconTrough.sortingOrder = cardSortingIndex * 50 + IconTroughDefaultSortingOrder;
    }
}