using TMPro;
using UnityEngine;

public class RetinueBattleInfoIcon : PoolObject
{
    [SerializeField] private Sprite[] IconSprites;

    public enum IconTypes
    {
        Immune = 0,
        Inactivity = 1
    }

    [SerializeField] private Animator Anim;
    [SerializeField] private SpriteRenderer Icon;
    [SerializeField] private TextMeshPro ValueText;

    void Awake()
    {
        Set_Value(0);
        IconDefaultSortingOrder = Icon.sortingOrder;
        TextDefaultSortingOrder = ValueText.sortingOrder;
    }

    public void Initialize(IconTypes iconType)
    {
        Icon.sprite = IconSprites[(int) iconType];
    }

    internal int Value = 0;

    public void Set_Value(int value)
    {
        if (value == 0)
        {
            Anim.enabled = false;
            Anim.gameObject.SetActive(false);
        }
        else
        {
            if (!Anim.isActiveAndEnabled)
            {
                Anim.enabled = true;
                Anim.gameObject.SetActive(true);
            }

            if (value != Value)
            {
                Anim.SetTrigger("Jump");
                Value = value;
            }

            ValueText.text = value.ToString();
        }
    }

    private int IconDefaultSortingOrder;
    private int TextDefaultSortingOrder;

    public void SetSortingIndexOfCard(int cardSortingIndex)
    {
        Icon.sortingOrder = cardSortingIndex * 50 + IconDefaultSortingOrder;
        ValueText.sortingOrder = cardSortingIndex * 50 + TextDefaultSortingOrder;
    }
}