using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AddLifeForSomeRetinue_Base : SideEffectBase
{
    public AddLifeForSomeRetinue_Info Info;

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

public struct AddLifeForSomeRetinue_Info
{
    public string RetinuePlayer;
    public string Select;
    public int Value;

    public void Serialze(DataStream writer)
    {
        writer.WriteString8(RetinuePlayer);
        writer.WriteString8(Select);
        writer.WriteSInt32(Value);
    }

    public void Deserialze(DataStream reader)
    {
        RetinuePlayer = reader.ReadString8();
        Select = reader.ReadString8();
        Value = reader.ReadSInt32();
    }
}