using UnityEngine;

public abstract class FX_Base : PoolObject
{
    public override void PoolRecycle()
    {
        base.PoolRecycle();
        ForceStop();
    }

    public abstract void Play(Color color, float duration, float scale = 1);
    public abstract void ForceStop();
}