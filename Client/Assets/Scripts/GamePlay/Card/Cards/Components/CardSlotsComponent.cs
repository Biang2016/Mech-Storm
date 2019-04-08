public class CardSlotsComponent : CardComponentBase
{
    public Slot[] Slots;

    public void ShowSlot(SlotTypes slotType, bool isShow)
    {
        if (Slots[(int) slotType - 1] != null)
        {
            Slots[(int) slotType - 1].gameObject.SetActive(isShow);
        }
    }

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        foreach (Slot slot in Slots)
        {
            if (slot != null)
            {
                slot.SetSortingLayer(cardSortingIndex);
            }
        }
    }
}