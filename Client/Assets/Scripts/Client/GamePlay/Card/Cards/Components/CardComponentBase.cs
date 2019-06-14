using UnityEngine;

[ExecuteInEditMode]
public abstract class CardComponentBase : MonoBehaviour
{
    private int cardOrder = -1;

    public int CardOrder
    {
        get { return cardOrder; }
        set
        {
            if (cardOrder != value)
            {
                SetSortingIndexOfCard(value);
                cardOrder = value;
            }
        }
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    protected abstract void SetSortingIndexOfCard(int cardSortingIndex);
}

public enum CardComponentTypes
{
    Basic,
    Back,
    Desc,
    CostBlock,
    Life,
    CoinBlock,
    Slots,
    SelectCount,
    Notice,
    Stars,
}