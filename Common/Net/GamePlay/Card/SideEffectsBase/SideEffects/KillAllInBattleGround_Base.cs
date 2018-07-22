using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class KillAllInBattleGround_Base : SideEffectBase
{
    public KillAllInBattleGround_Info Info;

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

public struct KillAllInBattleGround_Info
{
    public string WhoseBattleGround;

    public void Serialze(DataStream writer)
    {
        writer.WriteString8(WhoseBattleGround);
    }

    public void Deserialze(DataStream reader)
    {
        WhoseBattleGround = reader.ReadString8();
    }
}