using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class SideEffectBase
{
    public Player Player;

    public int SideEffectID;
    public string Name;
    public string Desc;

    public abstract SideEffectBase Clone();

    public abstract void RefreshDesc();

    public abstract void Excute(object Player);
}