using System;
using System.Reflection;

public partial class SideEffectBase
{
    public int SideEffectID;
    public string Name;
    public string DescRaw;
    public string DescRaw_en;

    public ExecuterInfo M_ExecuterInfo;

    public Player Player;

    public SideEffectBase()
    {
    }

    public SideEffectBase(int sideEffectID, string name, string desc, string desc_en)
    {
        SideEffectID = sideEffectID;
        Name = name;
        DescRaw = desc;
        DescRaw_en = desc_en;
    }

    //序列化时无视player，也就是说效果是无关玩家的
    public virtual void Serialze(DataStream writer)
    {
        string type = GetType().ToString();
        writer.WriteString8(type);
        writer.WriteSInt32(SideEffectID);
        writer.WriteString8(Name);
        writer.WriteString8(DescRaw);
        writer.WriteString8(DescRaw_en);
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
        DescRaw_en = reader.ReadString8();
    }

    public SideEffectBase Clone()
    {
        Assembly assembly = AllSideEffects.CurrentAssembly; // 获取当前程序集 
        SideEffectBase copy = (SideEffectBase) assembly.CreateInstance("SideEffects." + Name);
        copy.SideEffectID = SideEffectID;
        copy.Name = Name;
        copy.DescRaw = DescRaw;
        copy.DescRaw_en = DescRaw_en;
        if (M_ExecuterInfo != null)
        {
            copy.M_ExecuterInfo = M_ExecuterInfo.Clone();
        }

        CloneParams(copy);
        return copy;
    }

    protected virtual void CloneParams(SideEffectBase copy)
    {
    }

    public virtual string GenerateDesc(bool isEnglish)
    {
        return "";
    }

    public virtual void Excute(ExecuterInfo eventTriggerInfo)
    {
    }

    public static string HightlightStringFormat(string src, params object[] args)
    {
        string[] colorStrings = new string[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            colorStrings[i] = "<color=\"" + GamePlaySettings.CardHightLightColor + "\">" + args[i].ToString() + "</color>";
        }

        return String.Format(src, colorStrings);
    }

    public class ExecuterInfo
    {
        public int ClientId;
        public int TargetClientId;
        public int RetinueId;
        public int TargetRetinueId;
        public int CardId;
        public int CardInstanceId;
        public int EquipId;
        public int TargetEquipId;

        public ExecuterInfo(int clientId, int targetClientId = -1, int retinueId = -999, int targetRetinueId = -999, int cardId = -999, int cardInstanceId = -999, int equipId = -999, int targetEquipId = -999)
        {
            ClientId = clientId;
            TargetClientId = targetClientId;
            RetinueId = retinueId;
            TargetRetinueId = targetRetinueId;
            CardId = cardId;
            CardInstanceId = cardInstanceId;
            EquipId = equipId;
            TargetEquipId = targetEquipId;
        }

        public ExecuterInfo Clone()
        {
            return new ExecuterInfo(ClientId, TargetClientId, RetinueId, TargetRetinueId, CardId, CardInstanceId, EquipId, TargetEquipId);
        }

        public void Serialize(DataStream writer)
        {
            writer.WriteSInt32(ClientId);
            writer.WriteSInt32(TargetClientId);
            writer.WriteSInt32(RetinueId);
            writer.WriteSInt32(TargetRetinueId);
            writer.WriteSInt32(CardId);
            writer.WriteSInt32(CardInstanceId);
            writer.WriteSInt32(EquipId);
            writer.WriteSInt32(TargetEquipId);
        }

        public static ExecuterInfo Deserialize(DataStream reader)
        {
            int ClientId = reader.ReadSInt32();
            int TargetClientId = reader.ReadSInt32();
            int RetinueId = reader.ReadSInt32();
            int TargetRetinueId = reader.ReadSInt32();
            int CardId = reader.ReadSInt32();
            int CardInstanceId = reader.ReadSInt32();
            int EquipId = reader.ReadSInt32();
            int TargetEquipId = reader.ReadSInt32();
            return new ExecuterInfo(ClientId, TargetClientId, RetinueId, TargetRetinueId, CardId, CardInstanceId, EquipId, TargetEquipId);
        }

        public string DeserializeLog()
        {
            string log = "";
            log += " [ClientId]=" + ClientId;
            log += " [TargetClientId]=" + TargetClientId;
            log += " [RetinueId]=" + RetinueId;
            log += " [TargetRetinueId]=" + TargetRetinueId;
            log += " [CardId]=" + CardId;
            log += " [CardInstanceId]=" + CardInstanceId;
            log += " [EquipId]=" + EquipId;
            log += " [TargetEquipId]=" + TargetEquipId;
            return log;
        }
    }
}