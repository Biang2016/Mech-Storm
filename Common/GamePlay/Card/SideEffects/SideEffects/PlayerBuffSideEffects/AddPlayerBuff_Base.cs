public class AddPlayerBuff_Base : SideEffectBase
{
    public string BuffName;
    private SideEffectExecute attachedBuffSEE;

    public SideEffectExecute AttachedBuffSEE
    {
        get
        {
            if (attachedBuffSEE == null)
            {
                attachedBuffSEE = AllBuffs.GetBuff(BuffName).Clone();
            }

            return attachedBuffSEE;
        }
        set => attachedBuffSEE = value;
    }

    protected override void InitSideEffectParam()
    {
    }

    public override string GenerateDesc()
    {
        return AttachedBuffSEE.SideEffectBase.GenerateDesc();
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteString8(BuffName);
        //AttachedBuffSEE.Serialize(writer);
    }

    protected override void Deserialize(DataStream reader)
    {
        base.Deserialize(reader);
        BuffName = reader.ReadString8();
        //AttachedBuffSEE = SideEffectExecute.Deserialze(reader);
    }
}