using System;
using System.Collections.Generic;
using System.Xml;

public class SideEffectParam : IClone<SideEffectParam>
{
    public SideEffectParam(List<SideEffectValue> sevs, int factor = 1)
    {
        foreach (SideEffectValue.ValueTypes valueType in Enum.GetValues(typeof(SideEffectValue.ValueTypes)))
        {
            ParamsDict.Add(valueType, new Dictionary<string, SideEffectValue>());
        }

        foreach (SideEffectValue sev in sevs)
        {
            ParamsDict[sev.ValueType].Add(sev.Name, sev);
        }

        Factor = factor;
    }

    public int Factor = 1;
    Dictionary<SideEffectValue.ValueTypes, Dictionary<string, SideEffectValue>> ParamsDict = new Dictionary<SideEffectValue.ValueTypes, Dictionary<string, SideEffectValue>>(); // 按类型分

    public SideEffectValue GetParam(string name)
    {
        foreach (KeyValuePair<SideEffectValue.ValueTypes, Dictionary<string, SideEffectValue>> kv in ParamsDict)
        {
            kv.Value.TryGetValue(name, out SideEffectValue sev);
            if (sev != null) return sev;
        }

        return null;
    }

    public void SetParam_ConstInt(string name, int value, Type enumType = null)
    {
        if (!ParamsDict[SideEffectValue.ValueTypes.ConstInt].ContainsKey(name))
        {
            ParamsDict[SideEffectValue.ValueTypes.ConstInt].Add(name, new SideEffectValue_ContInt(name, value, enumType));
        }
        else
        {
            ParamsDict[SideEffectValue.ValueTypes.ConstInt][name] = new SideEffectValue_ContInt(name, value, enumType);
        }
    }

    public void SetParam_MultipliedInt(string name, int value)
    {
        if (!ParamsDict[SideEffectValue.ValueTypes.MultipliedInt].ContainsKey(name))
        {
            ParamsDict[SideEffectValue.ValueTypes.MultipliedInt].Add(name, new SideEffectValue_MultipliedInt(name, value));
        }
        else
        {
            ParamsDict[SideEffectValue.ValueTypes.MultipliedInt][name] = new SideEffectValue_MultipliedInt(name, value);
        }
    }

    public void SetParam_Bool(string name, bool value)
    {
        if (!ParamsDict[SideEffectValue.ValueTypes.Bool].ContainsKey(name))
        {
            ParamsDict[SideEffectValue.ValueTypes.Bool].Add(name, new SideEffectValue_Bool(name, value));
        }
        else
        {
            ParamsDict[SideEffectValue.ValueTypes.Bool][name] = new SideEffectValue_Bool(name, value);
        }
    }

    public void SetParam_String(string name, String value)
    {
        if (!ParamsDict[SideEffectValue.ValueTypes.String].ContainsKey(name))
        {
            ParamsDict[SideEffectValue.ValueTypes.String].Add(name, new SideEffectValue_String(name, value));
        }
        else
        {
            ParamsDict[SideEffectValue.ValueTypes.String][name] = new SideEffectValue_String(name, value);
        }
    }

    public int GetParam_ConstInt(string name)
    {
        ParamsDict[SideEffectValue.ValueTypes.ConstInt].TryGetValue(name, out SideEffectValue sev);
        if (sev != null)
        {
            return ((SideEffectValue_ContInt) sev).Value;
        }
        else
        {
            return -1;
        }
    }

    public int GetParam_MultipliedInt(string name)
    {
        ParamsDict[SideEffectValue.ValueTypes.MultipliedInt].TryGetValue(name, out SideEffectValue sev);
        if (sev != null)
        {
            return ((SideEffectValue_MultipliedInt) sev).Value * Factor;
        }
        else
        {
            return -1;
        }
    }

    public bool GetParam_Bool(string name)
    {
        ParamsDict[SideEffectValue.ValueTypes.Bool].TryGetValue(name, out SideEffectValue sev);
        if (sev != null)
        {
            return ((SideEffectValue_Bool) sev).Value;
        }
        else
        {
            return false;
        }
    }

    public string GetParam_String(string name)
    {
        ParamsDict[SideEffectValue.ValueTypes.String].TryGetValue(name, out SideEffectValue sev);
        return ((SideEffectValue_String) sev)?.Value;
    }

    public bool HasParamCanBeMultiplied()
    {
        return ParamsDict[SideEffectValue.ValueTypes.MultipliedInt].Count == 1;
    }

    public void Plus(SideEffectParam target) //如果是buff则令factor = 1
    {
        Factor = 1;
        foreach (KeyValuePair<string, SideEffectValue> kv in ParamsDict[SideEffectValue.ValueTypes.MultipliedInt])
        {
            int srcValue = GetParam_MultipliedInt(kv.Key);
            if (target.GetParam_MultipliedInt(kv.Key) != -1)
            {
                ((SideEffectValue_MultipliedInt) kv.Value).Value = srcValue + target.GetParam_MultipliedInt(kv.Key);
            }
        }
    }

    private SideEffectParam CloneCore(bool withFactor)
    {
        List<SideEffectValue> newSideEffectValues = new List<SideEffectValue>();
        foreach (KeyValuePair<SideEffectValue.ValueTypes, Dictionary<string, SideEffectValue>> kv in ParamsDict)
        {
            foreach (KeyValuePair<string, SideEffectValue> sevs in kv.Value)
            {
                newSideEffectValues.Add(sevs.Value.Clone());
            }
        }

        return new SideEffectParam(newSideEffectValues, withFactor ? Factor : 1);
    }

    public SideEffectParam Clone()
    {
        return CloneCore(false);
    }

    public SideEffectParam CloneWithFactor()
    {
        return CloneCore(true);
    }

    public List<XmlAttribute> GetParamsFromXMLNode(XmlNode node)
    {
        List<XmlAttribute> notMatchAttrs = new List<XmlAttribute>();
        for (int i = 0; i < node.Attributes.Count; i++)
        {
            XmlAttribute attr = node.Attributes[i];
            SideEffectValue sev = GetParam(attr.Name);
            if (sev != null)
            {
                sev.SetValue(attr.Value);
            }
            else
            {
                notMatchAttrs.Add(attr);
            }
        }

        return notMatchAttrs;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(Factor);
        writer.WriteSInt32(ParamsDict.Count);
        foreach (KeyValuePair<SideEffectValue.ValueTypes, Dictionary<string, SideEffectValue>> kv in ParamsDict)
        {
            foreach (KeyValuePair<string, SideEffectValue> kvv in kv.Value)
            {
                kvv.Value.Serialize(writer);
            }
        }
    }

    public static SideEffectParam Deserialize(DataStream reader)
    {
        int factor = reader.ReadSInt32();
        int count = reader.ReadSInt32();
        List<SideEffectValue> sevs = new List<SideEffectValue>();
        for (int i = 0; i < count; i++)
        {
            SideEffectValue sev = SideEffectValue.BaseDeserialize(reader);
            sevs.Add(sev);
        }

        SideEffectParam sep = new SideEffectParam(sevs, factor);
        return sep;
    }
}