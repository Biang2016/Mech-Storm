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

    public override string GenerateDesc(bool isEnglish)
    {
        return AttachedBuffSEE.SideEffectBase.GenerateDesc(isEnglish);
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

    protected override void CloneParams(SideEffectBase copy)
    {
        base.CloneParams(copy);
        ((AddPlayerBuff_Base) copy).BuffName = BuffName;
        if (attachedBuffSEE != null)
        {
            ((AddPlayerBuff_Base) copy).AttachedBuffSEE = AttachedBuffSEE?.Clone();
        }
    }
}