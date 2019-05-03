using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 副作用最小单元，包含对战场进行改变的功能和信息
/// </summary>
public abstract class SideEffectBase : IClone<SideEffectBase>
{
    public SideEffectBase()
    {
        InitSideEffectParam();
    }

    public SideEffectBase(string name, SortedDictionary<string, string> descRaws)
    {
        Name = name;
        DescRaws = descRaws;
        InitSideEffectParam();
    }

    public string Name;
    public SortedDictionary<string, string> DescRaws; // key: languageshort, value: desc

    public ExecutorInfo M_ExecutorInfo; //SE携带者信息，触发时和事件执行者信息进行比对，判定是否触发
    public Player Player;

    public List<SideEffectBase> Sub_SideEffect = new List<SideEffectBase>();

    public SideEffectParam M_SideEffectParam = new SideEffectParam(new List<SideEffectValue>(), 1);

    protected abstract void InitSideEffectParam();

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

        M_SideEffectParam.Serialize(writer);

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

        M_SideEffectParam = SideEffectParam.Deserialize(reader);

        int sub_SE_Count = reader.ReadSInt32();
        for (int i = 0; i < sub_SE_Count; i++)
        {
            SideEffectBase se = BaseDeserialize(reader);
            Sub_SideEffect.Add(se);
        }
    }

    private SideEffectBase CloneCore(bool withChange)
    {
        Assembly assembly = AllSideEffects.CurrentAssembly; // 获取当前程序集 
        SideEffectBase copy = (SideEffectBase) assembly.CreateInstance("SideEffects." + Name);
        copy.Name = Name;
        copy.DescRaws = CloneVariantUtils.SortedDictionary(DescRaws);
        if (withChange) copy.M_SideEffectParam = M_SideEffectParam.CloneWithFactor();
        else copy.M_SideEffectParam = M_SideEffectParam.Clone();
        copy.Sub_SideEffect = CloneVariantUtils.List(Sub_SideEffect);
        return copy;
    }

    public virtual SideEffectBase Clone() //只拷贝基础效果
    {
        return CloneCore(false);
    }

    public virtual SideEffectBase CloneWithChange() // 将附带的变化，如增益效果等一起拷贝
    {
        return CloneCore(true);
    }

    public virtual string GenerateDesc()
    {
        return "";
    }

    public virtual void Execute(ExecutorInfo eventTriggerInfo)
    {
    }

    protected static string HighlightStringFormat(string src, params object[] args)
    {
        string[] colorStrings = new string[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            colorStrings[i] = "<" + AllColors.ColorDict[AllColors.ColorType.CardHighLightColor] + ">" + args[i] + "</color>";
        }

        return string.Format(src, colorStrings);
    }
}