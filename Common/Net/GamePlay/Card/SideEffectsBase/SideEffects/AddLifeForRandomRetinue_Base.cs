using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AddLifeForRandomRetinue_Base : SideEffectBase
{
    public AddLifeForRandomRetinue_Info Info;

    public override void Serialze(DataStream writer)
    {
        base.Serialze(writer);
        Info.Serialze(writer);
    }

    protected override void Deserialze(DataStream reader)
    {
        base.Deserialze(reader);
        Info.Deserialze(reader);
    }
}

public struct AddLifeForRandomRetinue_Info
{
    public string RetinuePlayer;//限定范围为哪一方
    public int Value;

    public void Serialze(DataStream writer)
    {
        writer.WriteString8(RetinuePlayer);
        writer.WriteSInt32(Value);
    }

    public void Deserialze(DataStream reader)
    {
        RetinuePlayer = reader.ReadString8();
        Value = reader.ReadSInt32();
    }
}