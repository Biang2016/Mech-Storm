using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

public class SideEffectBase:ICloneable
{
    public Player Player;

    public int SideEffectID;
    public string Name;
    public string Desc;

    public SideEffectBase()
    {
    }

    public SideEffectBase(int sideEffectID, string name, string desc)
    {
        SideEffectID = sideEffectID;
        Name = name;
        Desc = desc;
    }

    object ICloneable.Clone()
    {
        return MemberwiseClone();
    }

    //序列化时无视player，也就是说效果是无关玩家的
    public virtual void Serialze(DataStream writer)
    {
        string type = GetType().ToString();
        writer.WriteString8(type);
        writer.WriteSInt32(SideEffectID);
        writer.WriteString8(Name);
        writer.WriteString8(Desc);
    }

    public static SideEffectBase BaseDeserialze(DataStream reader)
    {
        string type = reader.ReadString8();
        SideEffectBase se = SideEffectManager.GetNewSideEffec(type);
        se.Deserialze(reader);
        return se;
    }

    protected virtual void Deserialze(DataStream reader)
    {
        SideEffectID = reader.ReadSInt32();
        Name = reader.ReadString8();
        Desc = reader.ReadString8();
    }

    public virtual void RefreshDesc()
    {
    }

    public virtual void Excute(object Player)
    {
    }


}