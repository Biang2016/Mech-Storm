using System;
using System.Collections.Generic;

public abstract class PlayerBuffRelatedSideEffects : SideEffectBase
{
    public int BuffPicId;
    public string BuffColor;
    public bool HasNumberShow;
    public bool CanPiled;
    public bool Singleton;

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteSInt32(BuffPicId);
        writer.WriteString8(BuffColor);
        writer.WriteByte((byte) (HasNumberShow ? 0x01 : 0x00));
        writer.WriteByte((byte) (CanPiled ? 0x01 : 0x00));
        writer.WriteByte((byte) (Singleton ? 0x01 : 0x00));
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        BuffPicId = reader.ReadSInt32();
        BuffColor = reader.ReadString8();
        HasNumberShow = reader.ReadByte() == 0x01;
        CanPiled = reader.ReadByte() == 0x01;
        Singleton = reader.ReadByte() == 0x01;
    }

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((PlayerBuffRelatedSideEffects) copy).BuffPicId = BuffPicId;
        ((PlayerBuffRelatedSideEffects) copy).BuffColor = BuffColor;
        ((PlayerBuffRelatedSideEffects) copy).HasNumberShow = HasNumberShow;
        ((PlayerBuffRelatedSideEffects) copy).CanPiled = CanPiled;
        ((PlayerBuffRelatedSideEffects) copy).Singleton = Singleton;
    }
}