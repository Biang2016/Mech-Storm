using UnityEngine;

public abstract class Arrow : PoolObject
{
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