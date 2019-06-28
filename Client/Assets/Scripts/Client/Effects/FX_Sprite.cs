using System.Collections;
using UnityEngine;

public class FX_Sprite : FX_Base
{
    private Animator Anim;
    private SpriteRenderer SR;

    public override void PoolRecycle()
    {
        gameObject.SetActive(true);
        Anim.speed = 1;
        base.PoolRecycle();
    }

    void Awake()
    {
        Anim = GetComponent<Animator>();
        SR = GetComponent<SpriteRenderer>();
    }

    public override void ForceStop()
    {
    }

    public override void Play(Color color, float duration, float scale = 1)
    {
        StartCoroutine(Co_Play(color, duration, scale));
    }

    IEnumerator Co_Play(Color color, float duration, float scale)
    {
        SR.color = color;
        transform.localScale = Vector3.one * scale;
        Anim.SetTrigger("BeHit");
        float duration_ori = ClientUtils.GetClipLength(Anim, "Hit");
        Anim.speed = Anim.speed * duration_ori / duration;
        yield return new WaitForSeconds(duration);
        PoolRecycle();
    }
}