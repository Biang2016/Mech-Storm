using UnityEngine;

public class CardStarsComponent : CardComponentBase
{
    [SerializeField] private SpriteRenderer[] Stars;
    [SerializeField] private Sprite StarSprite;
    [SerializeField] private Sprite StarEmptySprite;

    void Awake()
    {
        if (Stars.Length > 0)
        {
            StarDefaultSortingOrder = Stars[0].sortingOrder;
        }
    }

    public void SetStarNumber(int number, int maxNumber)
    {
        if (maxNumber == 1)
        {
            foreach (SpriteRenderer star in Stars)
            {
                star?.gameObject.SetActive(false);
            }

            return;
        }

        for (int i = 0; i < Stars.Length; i++)
        {
            if (Stars[i] != null)
            {
                Stars[i].sprite = i < number ? StarSprite : StarEmptySprite;
                Stars[i].gameObject.SetActive(i < maxNumber);
            }
        }
    }

    private int StarDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        foreach (SpriteRenderer star in Stars)
        {
            star.sortingOrder = cardSortingIndex * 50 + StarDefaultSortingOrder;
        }
    }
}