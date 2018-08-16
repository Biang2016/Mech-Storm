public struct SlotInfo
{
    public SlotTypes Slot1;
    public SlotTypes Slot2;
    public SlotTypes Slot3;
    public SlotTypes Slot4;

    public SlotInfo(SlotTypes slot1, SlotTypes slot2, SlotTypes slot3, SlotTypes slot4)
    {
        Slot1 = slot1;
        Slot2 = slot2;
        Slot3 = slot3;
        Slot4 = slot4;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32((int) Slot1);
        writer.WriteSInt32((int) Slot2);
        writer.WriteSInt32((int) Slot3);
        writer.WriteSInt32((int) Slot4);
    }

    public static SlotInfo Deserialze(DataStream reader)
    {
        SlotTypes Slot1 = (SlotTypes) reader.ReadSInt32();
        SlotTypes Slot2 = (SlotTypes) reader.ReadSInt32();
        SlotTypes Slot3 = (SlotTypes) reader.ReadSInt32();
        SlotTypes Slot4 = (SlotTypes) reader.ReadSInt32();
        return new SlotInfo(Slot1, Slot2, Slot3, Slot4);
    }
}

public enum SlotTypes
{
    None = 0,
    Weapon = 1,
    Shield = 2,
    Pack = 3,
    MA = 4,
}