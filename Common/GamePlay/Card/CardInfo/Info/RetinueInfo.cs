public struct RetinueInfo
{
    public bool IsSoldier;
    public bool IsDefence;
    public bool IsSniper;
    public bool IsCharger;
    public bool IsFrenzy;
    public SlotTypes Slot1;
    public SlotTypes Slot2;
    public SlotTypes Slot3;
    public SlotTypes Slot4;

    public RetinueInfo(bool isSoldier, bool isDefence, bool isSniper, bool isCharger, bool isFrenzy, SlotTypes slot1, SlotTypes slot2, SlotTypes slot3, SlotTypes slot4)
    {
        IsSoldier = isSoldier;
        IsDefence = isDefence;
        IsSniper = isSniper;
        IsCharger = isCharger;
        IsFrenzy = isFrenzy;
        Slot1 = slot1;
        Slot2 = slot2;
        Slot3 = slot3;
        Slot4 = slot4;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteByte(IsSoldier ? (byte) 0x01 : (byte) 0x00);
        writer.WriteByte(IsDefence ? (byte) 0x01 : (byte) 0x00);
        writer.WriteByte(IsSniper ? (byte) 0x01 : (byte) 0x00);
        writer.WriteByte(IsCharger ? (byte) 0x01 : (byte) 0x00);
        writer.WriteByte(IsFrenzy ? (byte) 0x01 : (byte) 0x00);
        writer.WriteSInt32((int) Slot1);
        writer.WriteSInt32((int) Slot2);
        writer.WriteSInt32((int) Slot3);
        writer.WriteSInt32((int) Slot4);
    }

    public static RetinueInfo Deserialze(DataStream reader)
    {
        bool IsSoldier = reader.ReadByte() == 0x01;
        bool IsDefence = reader.ReadByte() == 0x01;
        bool IsSniper = reader.ReadByte() == 0x01;
        bool IsCharger = reader.ReadByte() == 0x01;
        bool IsFrenzy = reader.ReadByte() == 0x01;
        SlotTypes Slot1 = (SlotTypes) reader.ReadSInt32();
        SlotTypes Slot2 = (SlotTypes) reader.ReadSInt32();
        SlotTypes Slot3 = (SlotTypes) reader.ReadSInt32();
        SlotTypes Slot4 = (SlotTypes) reader.ReadSInt32();
        return new RetinueInfo(IsSoldier, IsDefence, IsSniper, IsCharger, IsFrenzy, Slot1, Slot2, Slot3, Slot4);
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