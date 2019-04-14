public class AddPlayerBuff_Base : SideEffectBase
{
    public string BuffName
    {
        get { return M_SideEffectParam.GetParam_String("BuffName"); }
    }

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
        M_SideEffectParam.SetParam_String("BuffName", "");
    }

    public override string GenerateDesc()
    {
        return AttachedBuffSEE.SideEffectBase.GenerateDesc();
    }
}