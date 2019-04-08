public class CardSlotsComponent : CardComponentBase
{
    public Slot[] Slots;

    public void SetSlot(ClientPlayer clientPlayer, RetinueInfo retinueInfo)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i] != null)
            {
                Slots[i].ClientPlayer = clientPlayer;
                Slots[i].MSlotTypes = retinueInfo.Slots[i];
            }
        }
    }

    public void ShowSlot(SlotTypes slotType, bool isShow)
    {
        if (Slots[(int) slotType - 1] != null)
        {
            Slots[(int) slotType - 1].gameObject.SetActive(isShow);
        }
    }

    public void ShowAllSlotLights(bool isShow)
    {
        foreach (Slot slot in Slots)
        {
            if (slot != null)
            {
                slot.ShowSlotLight(isShow);
            }
        }
    }

    public void ShowAllSlotBlooms(bool isShow)
    {
        foreach (Slot slot in Slots)
        {
            if (slot != null)
            {
                slot.ShowSlotBloom(isShow);
            }
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