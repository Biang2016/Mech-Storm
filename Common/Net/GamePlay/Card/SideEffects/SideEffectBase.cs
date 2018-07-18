using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SideEffectBase
{
    public int SideEffectID;
    public string Name;
    public string Desc;

    public virtual SideEffectBase Clone()
    {
        SideEffectBase newSideEffectBase = new SideEffectBase();
        newSideEffectBase.SideEffectID = SideEffectID;
        newSideEffectBase.Name = Name;
        newSideEffectBase.Desc = Desc;
        return newSideEffectBase;
    }

    public virtual void Excute(object Player)
    {
    }
}