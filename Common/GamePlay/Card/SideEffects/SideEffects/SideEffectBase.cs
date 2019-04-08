using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 副作用最小单元，包含对战场进行改变的功能和信息
/// </summary>
public partial class SideEffectBase
{
    public string Name;
    public SortedDictionary<string, string> DescRaws;

    public ExecutorInfo M_ExecutorInfo; //SE携带者信息，触发时和事件执行者信息进行比对，判定是否触发
    public Player Player;
    public List<SideEffectBase> Sub_SideEffect = new List<SideEffectBase>();

    public SideEffectBase()
    {
    }

    public SideEffectBase(string name, SortedDictionary<string, string> descRaws)
    {
        Name = name;
        DescRaws = descRaws;
    }

    //序列化时无视player，和M_ExecutorInfo，也就是说效果是无关玩家和执行者的
    public virtual void Serialize(DataStream writer)
    {
        string type = GetType().ToString();
        writer.WriteString8(type);
        writer.WriteString8(Name);
        writer.WriteSInt32(DescRaws.Count);
        foreach (KeyValuePair<string, string> kv in DescRaws)
        {
            writer.WriteString8(kv.Key);
            writer.WriteString8(kv.Value);
        }

        writer.WriteSInt32(Sub_SideEffect.Count);
        foreach (SideEffectBase sub_SE in Sub_SideEffect)
        {
            sub_SE.Serialize(writer);
        }
    }

    public static SideEffectBase BaseDeserialize(DataStream reader)
    {
        string type = reader.ReadString8();
        SideEffectBase se = SideEffectManager.GetNewSideEffect(type);
        se.Deserialize(reader);
        return se;
    }

    protected virtual void Deserialize(DataStream reader)
    {
        Name = reader.ReadString8();
        DescRaws = new SortedDictionary<string, string>();
        int descRawCount = reader.ReadSInt32();
        for (int i = 0; i < descRawCount; i++)
        {
            string ls = reader.ReadString8();
            string value = reader.ReadString8();
            DescRaws[ls] = value;
        }

        int sub_SE_Count = reader.ReadSInt32();
        for (int i = 0; i < sub_SE_Count; i++)
        {
            SideEffectBase se = BaseDeserialize(reader);
            Sub_SideEffect.Add(se);
        }
    }

    public SideEffectBase Clone()
    {
        Assembly assembly = AllSideEffects.CurrentAssembly; // 获取当前程序集 
        SideEffectBase copy = (SideEffectBase) assembly.CreateInstance("SideEffects." + Name);
        copy.Name = Name;
        copy.DescRaws = CloneVariantUtils.SortedDictionary(DescRaws);
        foreach (SideEffectBase sub_SE in Sub_SideEffect)
        {
            copy.Sub_SideEffect.Add(sub_SE.Clone());
        }

        CloneParams(copy);
        return copy;
    }

    protected virtual void CloneParams(SideEffectBase copy)
    {
    }

    public virtual string GenerateDesc()
    {
        return "";
    }

    public virtual void Execute(ExecutorInfo eventTriggerInfo)
    {
    }

    public static string HighlightStringFormat(string src, params object[] args)
    {
        string[] colorStrings = new string[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            colorStrings[i] = "<" + AllColors.ColorDict[AllColors.ColorType.CardHighLightColor] + ">" + args[i] + "</color>";
        }

        return string.Format(src, colorStrings);
    }

    public class ExecutorInfo
    {
        public const int EXECUTE_INFO_NONE = -999;
        public const int EXECUTOR_ID_EVENTS = -1;

        public int SideEffectExecutorID;
        public int ClientId;
        public int TargetClientId;
        public int RetinueId;
        public int TargetRetinueId;
        public int CardId;
        public int CardInstanceId;
        public int EquipId;
        public int TargetEquipId;
        public int Value;
        public bool IsPlayerBuff;

        public ExecutorInfo(int clientId, int targetClientId = EXECUTE_INFO_NONE, int sideEffectExecutorID = EXECUTOR_ID_EVENTS, int retinueId = EXECUTE_INFO_NONE, int targetRetinueId = EXECUTE_INFO_NONE, int cardId = EXECUTE_INFO_NONE, int cardInstanceId = EXECUTE_INFO_NONE, int equipId = EXECUTE_INFO_NONE, int targetEquipId = EXECUTE_INFO_NONE, int value = 0, bool isPlayerBuff = false)
        {
            ClientId = clientId;
            TargetClientId = targetClientId;
            SideEffectExecutorID = sideEffectExecutorID;
            RetinueId = retinueId;
            TargetRetinueId = targetRetinueId;
            CardId = cardId;
            CardInstanceId = cardInstanceId;
            EquipId = equipId;
            TargetEquipId = targetEquipId;
            Value = value;
            IsPlayerBuff = isPlayerBuff;
        }

        public ExecutorInfo Clone()
        {
            return new ExecutorInfo(ClientId, TargetClientId, SideEffectExecutorID, RetinueId, TargetRetinueId, CardId, CardInstanceId, EquipId, TargetEquipId, Value);
        }

        public void Serialize(DataStream writer)
        {
            writer.WriteSInt32(ClientId);
            writer.WriteSInt32(TargetClientId);
            writer.WriteSInt32(SideEffectExecutorID);
            writer.WriteSInt32(RetinueId);
            writer.WriteSInt32(TargetRetinueId);
            writer.WriteSInt32(CardId);
            writer.WriteSInt32(CardInstanceId);
            writer.WriteSInt32(EquipId);
            writer.WriteSInt32(TargetEquipId);
            writer.WriteSInt32(Value);
        }

        public static ExecutorInfo Deserialize(DataStream reader)
        {
            int ClientId = reader.ReadSInt32();
            int TargetClientId = reader.ReadSInt32();
            int SideEffectExecutorID = reader.ReadSInt32();
            int RetinueId = reader.ReadSInt32();
            int TargetRetinueId = reader.ReadSInt32();
            int CardId = reader.ReadSInt32();
            int CardInstanceId = reader.ReadSInt32();
            int EquipId = reader.ReadSInt32();
            int TargetEquipId = reader.ReadSInt32();
            int Value = reader.ReadSInt32();
            return new ExecutorInfo(ClientId, TargetClientId, SideEffectExecutorID, RetinueId, TargetRetinueId, CardId, CardInstanceId, EquipId, TargetEquipId, Value);
        }
    }
}