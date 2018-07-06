using UnityEngine;
using System.Collections;
using System;

public abstract class Arrow : MonoBehaviour, IGameObjectPool
{
    public virtual void PoolRecycle()
    {
        GameObjectPoolManager.GOPM.Pool_ArrowArrowPool.RecycleGameObject(gameObject);
    }

    public GameObject ArrowBody;

    Animator anim;

    void Awake()
    {
    }

    public ArrowType M_ArrowType;


    public abstract void Render(Vector3 StartPosition, Vector3 EndPosition);
}

public enum ArrowType
{
    Arrow = 0,
    Aiming = 1,
}