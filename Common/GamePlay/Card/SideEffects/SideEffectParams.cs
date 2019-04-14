using System.Collections.Generic;

public class SideEffectParam
{
    public SideEffectParam(List<SideEffectValue> sevs)
    {
        foreach (SideEffectValue sev in sevs)
        {
            if (!Params.ContainsKey(sev.Name))
            {
                Params.Add(sev.Name, sev);
            }
        }
    }

    public int GetParam(string name)
    {
        if (Params.ContainsKey(name))
        {
            return Params[name].Value;
        }

        return -1;
    }

    Dictionary<string, SideEffectValue> Params = new Dictionary<string, SideEffectValue>();

    public void Serialize(DataStream writer)
    {
    }

    public static void Deserialize(DataStream reader)
    {
    }
}

public class SideEffectValue
{
    public string Name;
    public int Value;
    public bool CanMultipliedByFactor;

    public SideEffectValue(string name, int value, bool canMultipliedByFactor)
    {
        Name = name;
        Value = value;
        CanMultipliedByFactor = canMultipliedByFactor;
    }

    public SideEffectValue Clone()
    {
        return new SideEffectValue(Name, Value, CanMultipliedByFactor);
    }
}