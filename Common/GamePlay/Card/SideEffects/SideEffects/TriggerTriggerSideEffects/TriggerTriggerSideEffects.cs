using System;
using System.Collections.Generic;

public abstract class TriggerTriggerSideEffects : SideEffectBase, ITrigger
{
    public SideEffectExecute PeekSEE { get; set; }

    public abstract bool IsTrigger();
}