using UnityEngine;

[ExecuteInEditMode]
public abstract class MechComponentBase : MonoBehaviour
{
    protected ModuleMech Mech;

    public void Initialize(ModuleMech mech)
    {
        Mech = mech;
        Reset();
        Child_Initialize();
    }

    protected abstract void Child_Initialize();

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

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    protected abstract void Reset();

    protected abstract void SetSortingIndexOfCard(int cardSortingIndex);
}

public enum MechComponentTypes
{
    Life,
    AttrShapes,
    BattleInfo,
    Bloom,
    TargetPreviewArrows,
    TriggerIcon,
    SwordShieldArmor,
}