using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Newtonsoft.Json;

public class ScriptExecuteSettingBase : SideEffectExecute.ExecuteSetting
{
    public ScriptExecuteSettingBase()
    {
        InitSideEffectParam();
    }

    public ScriptExecuteSettingBase(string name, SortedDictionary<string, string> descRaws, SideEffectExecute.TriggerTime triggerTime, SideEffectExecute.TriggerRange triggerRange, int triggerTimes, int triggerDelayTimes, SideEffectExecute.TriggerTime removeTriggerTime, SideEffectExecute.TriggerRange removeTriggerRange, int removeTriggerTimes) : base(triggerTime, triggerRange, triggerTimes, triggerDelayTimes, removeTriggerTime, removeTriggerRange, removeTriggerTimes)
    {
        Name = name;
        DescRaws = descRaws;
        InitSideEffectParam();
    }

    [JsonIgnore] public Player Player;
    public string Name;
    public SortedDictionary<string, string> DescRaws; // key: languageshort, value: desc
    public SideEffectParam M_SideEffectParam = new SideEffectParam(new List<SideEffectValue>(), 1);

    protected virtual void InitSideEffectParam()
    {
    }

    public virtual HashSet<SideEffectExecute.TriggerTime> ValidTriggerTimes
    {
        get { return SideEffectExecute.GetAllTriggerTimeSet(); }
    }

    public virtual HashSet<SideEffectExecute.TriggerTime> ValidRemoveTriggerTimes
    {
        get { return SideEffectExecute.GetAllTriggerTimeSet(); }
    }

    public const int UNLOCKED_EXECUTESETTING_TIMES = -1;

    public virtual int LockedTriggerTimes
    {
        get { return UNLOCKED_EXECUTESETTING_TIMES; }
    }

    public virtual int LockedTriggerDelayTimes
    {
        get { return UNLOCKED_EXECUTESETTING_TIMES; }
    }

    public virtual int LockedRemoveTriggerTimes
    {
        get { return UNLOCKED_EXECUTESETTING_TIMES; }
    }

    public virtual bool IsTrigger(ExecutorInfo executorInfo, ExecutorInfo se_ExecutorInfo)
    {
        return false;
    }

    public override void Serialize(DataStream writer)
    {
        base.Serialize(writer);
        writer.WriteString8(Name);
        writer.WriteSInt32(DescRaws.Count);
        foreach (KeyValuePair<string, string> kv in DescRaws)
        {
            writer.WriteString8(kv.Key);
            writer.WriteString8(kv.Value);
        }

        M_SideEffectParam.Serialize(writer);
    }

    public override void Child_Deserialize(DataStream reader)
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
    }

    private ScriptExecuteSettingBase CloneCore(bool withChange)
    {
        Assembly assembly = AllScriptExecuteSettings.CurrentAssembly; // 获取当前程序集 
        ScriptExecuteSettingBase copy = (ScriptExecuteSettingBase) assembly.CreateInstance("ScriptExecuteSettings." + Name);
        copy.Name = Name;
        copy.DescRaws = CloneVariantUtils.SortedDictionary(DescRaws);
        copy.TriggerTime = TriggerTime;
        copy.TriggerRange = TriggerRange;
        copy.TriggerTimes = TriggerTimes;
        copy.TriggerDelayTimes = TriggerDelayTimes;
        copy.RemoveTriggerTime = RemoveTriggerTime;
        copy.RemoveTriggerRange = RemoveTriggerRange;
        copy.RemoveTriggerTimes = RemoveTriggerTimes;
        if (withChange) copy.M_SideEffectParam = M_SideEffectParam.CloneWithFactor();
        else copy.M_SideEffectParam = M_SideEffectParam.Clone();
        return copy;
    }

    public override SideEffectExecute.ExecuteSetting Clone() //只拷贝基础效果
    {
        return CloneCore(false);
    }

    public virtual SideEffectExecute.ExecuteSetting CloneWithChange() // 将附带的变化，如增益效果等一起拷贝
    {
        return CloneCore(true);
    }

    public virtual string GenerateDesc()
    {
        return "";
    }

    protected static string HighlightStringFormat(string src, params object[] args)
    {
        return Utils.HighlightStringFormat(src, AllColors.ColorDict[AllColors.ColorType.CardHighLightColor], args);
    }

    protected static string HighlightStringFormat(string src, bool[] needTint, params object[] args)
    {
        return Utils.HighlightStringFormat(src, AllColors.ColorDict[AllColors.ColorType.CardHighLightColor], needTint, args);
    }

    public override void ExportToXML(XmlElement ele)
    {
        base.ExportToXML(ele);
        ele.SetAttribute("ScriptName", Name);
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

    public static List<string> HashSetTriggerTimeToListString(HashSet<SideEffectExecute.TriggerTime> src)
    {
        List<string> res = new List<string>();
        foreach (SideEffectExecute.TriggerTime tt in src)
        {
            res.Add(tt.ToString());
        }

        return res;
    }
}