using System;
using System.Collections.Generic;

public partial class SideEffectBase : ICloneable
{
    public int SideEffectID;
    public string Name;
    public string DescRaw;

    public string HightlightColor;

    public ExecuterInfo M_ExecuterInfo;

    public Player Player;

    public SideEffectBase()
    {
    }

    public SideEffectBase(int sideEffectID, string name, string desc)
    {
        SideEffectID = sideEffectID;
        Name = name;
        DescRaw = desc;
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
        writer.WriteString8(DescRaw);
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
        DescRaw = reader.ReadString8();
    }

    public virtual string GenerateDesc()
    {
        return "";
    }

    public virtual void Excute(ExecuterInfo eventTriggerInfo)
    {
    }

    public static string HightlightStringFormat(string hightlightColor, string src, params object[] args)
    {
        string[] colorStrings = new string[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            colorStrings[i] = "<color=\"" + hightlightColor + "\">" + args[i].ToString() + "</color>";
        }

        return String.Format(src, colorStrings);
    }

    public class ExecuterInfo
    {
        public int ClientId;
        public int RetinueId;
        public int TargetRetinueId;
        public int CardId;
        public int CardInstanceId;

        public ExecuterInfo(int clientId, int retinueId = -999, int targetRetinueId = -999, int cardId = -999, int cardInstanceId = -999)
        {
            ClientId = clientId;
            RetinueId = retinueId;
            TargetRetinueId = targetRetinueId;
            CardId = cardId;
            CardInstanceId = cardInstanceId;
        }
    }
}