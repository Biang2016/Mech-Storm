using System.Collections;
using UnityEngine;

internal class Hit : PoolObject
{
    [SerializeField] private Animator Anim;
    [SerializeField] private SpriteRenderer SR;
    [SerializeField] private HitManager.HitType HitType;

    private Hit()
    {
    }

    void Awake()
    {
        SetObjectPool(GameObjectPoolManager.Instance.Pool_HitPool[(int) HitType]);
    }

    public override void PoolRecycle()
    {
        gameObject.SetActive(true);
        Anim.speed = 1;
        base.PoolRecycle();
    }

    public void ShowHit(Color color, float duration, float scale = 1)
    {
        StartCoroutine(Co_ShowHit(color, duration, scale));
    }

    IEnumerator Co_ShowHit(Color color, float duration, float scale)
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