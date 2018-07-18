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
    }
}