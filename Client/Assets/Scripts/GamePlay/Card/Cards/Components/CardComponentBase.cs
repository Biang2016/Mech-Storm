using UnityEngine;

public abstract class CardComponentBase : MonoBehaviour
{
    private int cardOrder;

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
