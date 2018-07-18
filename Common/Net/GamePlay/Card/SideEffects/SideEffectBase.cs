using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SideEffectBase
{
    public int SideEffectID;
    public string Name;
    public string Desc;
    public List<Param> Params = new List<Param>();

    public struct Param
    {
        public string Name;
        public object Value;

        public Param(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public Param Clone()
        {
            return new Param(Name, Value);
        }
    }

    public SideEffectBase Clone()
    {
        SideEffectBase newSideEffectBase = new SideEffectBase();
        newSideEffectBase.SideEffectID = SideEffectID;
        newSideEffectBase.Name = Name;
        newSideEffectBase.Desc = Desc;
        foreach (Param param in Params)
        {
            newSideEffectBase.Params.Add(param.Clone());
        }

        return newSideEffectBase;
    }
}