using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public struct MechInfo
{
    public bool IsSoldier;
    public bool IsDefense;
    public bool IsSniper;
    public bool IsCharger;
    public bool IsFrenzy;
    public SlotTypes[] Slots;

    public MechInfo(bool isSoldier, bool isDefense, bool isSniper, bool isCharger, bool isFrenzy, SlotTypes slot1, SlotTypes slot2, SlotTypes slot3, SlotTypes slot4)
    {
        IsSoldier = isSoldier;
        IsDefense = isDefense;
        IsSniper = isSniper;
        IsCharger = isCharger;
        IsFrenzy = isFrenzy;
        Slots = new SlotTypes[] {slot1, slot2, slot3, slot4};
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteByte(IsSoldier ? (byte) 0x01 : (byte) 0x00);
        writer.WriteByte(IsDefense ? (byte) 0x01 : (byte) 0x00);
        writer.WriteByte(IsSniper ? (byte) 0x01 : (byte) 0x00);
        writer.WriteByte(IsCharger ? (byte) 0x01 : (byte) 0x00);
        writer.WriteByte(IsFrenzy ? (byte) 0x01 : (byte) 0x00);
        if (Slots == null)
        {
            Slots = new SlotTypes[] {SlotTypes.None, SlotTypes.None, SlotTypes.None, SlotTypes.None};
        }

        for (int i = 0; i < Slots.Length; i++)
        {
            writer.WriteSInt32((int) Slots[i]);
        }
    }

    public static MechInfo Deserialze(DataStream reader)
    {
        bool IsSoldier = reader.ReadByte() == 0x01;
        bool IsDefense = reader.ReadByte() == 0x01;
        bool IsSniper = reader.ReadByte() == 0x01;
        bool IsCharger = reader.ReadByte() == 0x01;
        bool IsFrenzy = reader.ReadByte() == 0x01;
        SlotTypes Slot1 = (SlotTypes) reader.ReadSInt32();
        SlotTypes Slot2 = (SlotTypes) reader.ReadSInt32();
        SlotTypes Slot3 = (SlotTypes) reader.ReadSInt32();
        SlotTypes Slot4 = (SlotTypes) reader.ReadSInt32();
        return new MechInfo(IsSoldier, IsDefense, IsSniper, IsCharger, IsFrenzy, Slot1, Slot2, Slot3, Slot4);
    }

    public bool HasSlotType(SlotTypes slotType)
    {
        foreach (SlotTypes st in Slots)
        {
            if (st == slotType)
                return true;
        }

        return false;
    }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum SlotTypes
{
    None = 0,
    Weapon = 1,
    Shield = 2,
    Pack = 3,
    MA = 4,
}