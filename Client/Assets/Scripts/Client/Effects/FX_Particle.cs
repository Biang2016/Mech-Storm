using UnityEngine;

public class FX_Particle : FX_Base
{
    public ParticleSystem ParticleSystem;

    public override void Play(Color color, float duration, float scale = 1)
    {
        ParticleSystem.Stop();
        ParticleSystem.startColor = color;
        ParticleSystem.startSize = scale;
        ParticleSystem.Play();
    }

    public override void ForceStop()
    {
        ParticleSystem.Stop();
    }

    void Update()
    {
        if (!IsRecycled && ParticleSystem.isStopped)
        {
            PoolRecycle();
        }
    }
}