using UnityEngine;

[ExecuteInEditMode]
public abstract class RetinueComponentBase : MonoBehaviour
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

public enum RetinueComponentTypes
{
    Basic,
    Back,
    Desc,
    LifeBlock,
    Slots,
    Notice,
    Stars,
}