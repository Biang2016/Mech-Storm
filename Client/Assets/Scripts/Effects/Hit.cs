using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Hit : MonoBehaviour, IGameObjectPool
{
    private GameObjectPool gameObjectPool;
    [SerializeField] private Animator Anim;
    [SerializeField] private SpriteRenderer SR;
    [SerializeField] private HitManager.HitType HitType;

    private Hit()
    {
    }

    void Awake()
    {
        switch (HitType)
        {
            case HitManager.HitType.LineLeftTopToRightButtom:
                gameObjectPool = GameObjectPoolManager.Instance.Pool_Hit0Pool;
                break;
            case HitManager.HitType.LineRightTopToLeftButtom:
                gameObjectPool = GameObjectPoolManager.Instance.Pool_Hit1Pool;
                break;
            case HitManager.HitType.Blade:
                gameObjectPool = GameObjectPoolManager.Instance.Pool_Hit2Pool;
                break;
        }
    }

    public void PoolRecycle()
    {
        gameObject.SetActive(true);
        Anim.speed = 1;
        gameObjectPool.RecycleGameObject(gameObject);
    }

    public void ShowHit(Color color, float duration, float scale=1)
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