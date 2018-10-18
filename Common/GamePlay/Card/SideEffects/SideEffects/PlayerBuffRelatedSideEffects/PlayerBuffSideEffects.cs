using System;
using System.Collections.Generic;

public abstract class PlayerBuffSideEffects : SideEffectBase
{
    public string BuffName;
    public int BuffPicId;
    public string BuffColor;
    public bool HasNumberShow;
    public bool CanPiled;
    public bool Singleton;

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteString8(BuffName);
        writer.WriteSInt32(BuffPicId);
        writer.WriteString8(BuffColor);
        writer.WriteByte((byte) (HasNumberShow ? 0x01 : 0x00));
        writer.WriteByte((byte) (CanPiled ? 0x01 : 0x00));
        writer.WriteByte((byte) (Singleton ? 0x01 : 0x00));
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        BuffName = reader.ReadString8();
        BuffPicId = reader.ReadSInt32();
        BuffColor = reader.ReadString8();
        HasNumberShow = reader.ReadByte() == 0x01;
        CanPiled = reader.ReadByte() == 0x01;
        Singleton = reader.ReadByte() == 0x01;
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((PlayerBuffSideEffects) copy).BuffName = BuffName;
        ((PlayerBuffSideEffects) copy).BuffPicId = BuffPicId;
        ((PlayerBuffSideEffects) copy).BuffColor = BuffColor;
        ((PlayerBuffSideEffects) copy).HasNumberShow = HasNumberShow;
        ((PlayerBuffSideEffects) copy).CanPiled = CanPiled;
        ((PlayerBuffSideEffects) copy).Singleton = Singleton;
    }
}