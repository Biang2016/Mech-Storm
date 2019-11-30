using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Newtonsoft.Json;

/// <summary>
/// 副作用最小单元，包含对战场进行改变的功能和信息
/// </summary>
public abstract class SideEffectBase : IClone<SideEffectBase>
{
    [JsonIgnore] public SideEffectExecute M_SideEffectExecute;

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

    [JsonIgnore] public Player Player;

    public List<SideEffectBase> Sub_SideEffect = new List<SideEffectBase>();

    public SideEffectParam M_SideEffectParam = new SideEffectParam(new List<SideEffectValue>(), 1);

    protected virtual void InitSideEffectParam()
    {
        M_SideEffectParam.SetParam_ConstInt("Chance", 100);
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
        int chance = M_SideEffectParam.GetParam_ConstInt("Chance");
        string changeDesc = "(" + chance + "%) ";
        if (chance == 100)
        {
            changeDesc = "";
        }

        return changeDesc;
    }

    public virtual bool Execute(ExecutorInfo eventTriggerInfo)
    {
        int chance = M_SideEffectParam.GetParam_ConstInt("Chance");
        Random r = new Random();
        int next = r.Next(0, 100);
        return next < chance;
    }

    protected static string HighlightStringFormat(string src, params object[] args)
    {
        return Utils.HighlightStringFormat(src, AllColors.ColorDict[AllColors.ColorType.CardHighLightColor], args);
    }

    protected static string HighlightStringFormat(string src, bool[] needTint, params object[] args)
    {
        return Utils.HighlightStringFormat(src, AllColors.ColorDict[AllColors.ColorType.CardHighLightColor], needTint, args);
    }

    public void ExportToXML(XmlElement ele)
    {
        ele.SetAttribute("name", Name);
        foreach (SideEffectValue sev in M_SideEffectParam.SideEffectValues)
        {
            switch (sev)
            {
                case SideEffectValue_ConstInt sev_ConstInt:
                {
                    if (sev_ConstInt.EnumType == typeof(CardDeck))
                    {
                        ele.SetAttribute(sev.Name, sev_ConstInt.Value.ToString());
                    }
                    else if (sev_ConstInt.EnumType != null)
                    {
                        string enum_name = Enum.ToObject(sev_ConstInt.EnumType, sev_ConstInt.Value).ToString();
                        ele.SetAttribute(sev.Name, enum_name);
                    }
                    else
                    {
                        ele.SetAttribute(sev.Name, sev_ConstInt.Value.ToString());
                    }

                    break;
                }
                case SideEffectValue_MultipliedInt sev_MultipliedInt:
                {
                    ele.SetAttribute(sev.Name, sev_MultipliedInt.Value.ToString());
                    break;
                }
                case SideEffectValue_Bool sev_Bool:
                {
                    ele.SetAttribute(sev.Name, sev_Bool.Value.ToString());
                    break;
                }
                case SideEffectValue_String sev_String:
                {
                    ele.SetAttribute(sev.Name, sev_String.Value);
                    break;
                }
            }
        }
    }
}