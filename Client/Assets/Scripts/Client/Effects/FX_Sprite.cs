using System.Collections;
using UnityEngine;

public class FX_Sprite : FX_Base
{
    [SerializeField] private Animator[] Anims;

    public override void PoolRecycle()
    {
        gameObject.SetActive(true);
        foreach (Animator anim in Anims)
        {
            if (anim)
            {
                anim.speed = 1;
            }
        }

        base.PoolRecycle();
    }

    public override void ForceStop()
    {
    }

    public override void Play(Color color, float duration, float scale = 1)
    {
        StartCoroutine(Co_Play(duration, scale));
    }

    IEnumerator Co_Play(float duration, float scale)
    {
        transform.localScale = Vector3.one * scale;
        foreach (Animator anim in Anims)
        {
            anim.SetTrigger("Show");
            float duration_ori = ClientUtils.GetClipLength(anim, "Show");
            anim.speed = anim.speed * duration_ori / duration;
        }

        yield return new WaitForSeconds(duration);
        PoolRecycle();
    }
}