using System.Collections.Generic;

namespace SideEffects
{
    public class AddPlayerBuff_Base : SideEffectBase
    {
        public AddPlayerBuff_Base()
        {
        }

        public SideEffectExecute AttachedBuffSEE = new SideEffectExecute(
            SideEffectExecute.SideEffectFrom.Buff,
            new List<SideEffectBase>(),
            new SideEffectExecute.ExecuteSetting());

        public string BuffName
        {
            get { return M_SideEffectParam.GetParam_String("BuffName"); }
            set
            {
                AttachedBuffSEE.SideEffectBases.Clear();
                PlayerBuffSideEffects buff = AllBuffs.GetBuff(value);
                buff.MyBuffSEE = AttachedBuffSEE;
                AttachedBuffSEE.SideEffectBases.Add(buff);
                M_SideEffectParam.SetParam_String("BuffName", value);
            }
        }

        protected override void InitSideEffectParam()
        {
            M_SideEffectParam.SetParam_String("BuffName", "");
        }

        public override string GenerateDesc()
        {
            string desc = "";
            foreach (SideEffectBase se in AttachedBuffSEE.SideEffectBases)
            {
                desc += se.GenerateDesc() + ",";
            }

            return desc.TrimEnd(",".ToCharArray());
        }

        public override SideEffectBase Clone()
        {
            AddPlayerBuff_Base copy = (AddPlayerBuff_Base) base.Clone();
            copy.AttachedBuffSEE = AttachedBuffSEE.Clone();
            foreach (SideEffectBase se in copy.AttachedBuffSEE.SideEffectBases)
            {
                ((PlayerBuffSideEffects) se).MyBuffSEE = copy.AttachedBuffSEE;
            }

            return copy;
        }

        public override void Serialize(DataStream writer)
        {
            base.Serialize(writer);
            AttachedBuffSEE.Serialize(writer);
        }

        protected override void Deserialize(DataStream reader)
        {
            base.Deserialize(reader);
            AttachedBuffSEE = SideEffectExecute.Deserialize(reader);
        }
    }
}