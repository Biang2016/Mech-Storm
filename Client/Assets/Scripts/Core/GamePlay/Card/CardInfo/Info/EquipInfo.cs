public struct EquipInfo
{
    public SlotTypes SlotType;

    public EquipInfo(SlotTypes slotType)
    {
        SlotType = slotType;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32((int) SlotType);
    }

    public static EquipInfo Deserialze(DataStream reader)
    {
        SlotTypes SlotType = (SlotTypes) reader.ReadSInt32();
        return new EquipInfo(SlotType);
    }
}