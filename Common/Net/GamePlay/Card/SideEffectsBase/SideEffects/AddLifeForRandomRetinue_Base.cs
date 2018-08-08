using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AddLifeForRandomRetinue_Base : TargetSideEffect
{
    public int Value;

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
        writer.WriteSInt32(Value);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
        Value = reader.ReadSInt32();
    }
}
