using UnityEngine;

public class PoolObject : MonoBehaviour
{
    private GameObjectPool m_Pool;

    public GameObjectPool Pool
    {
        get { return m_Pool; }
    }

    public void SetObjectPool(GameObjectPool pool)
    {
        m_Pool = pool;
    }

    public virtual void PoolRecycle()
    {
        iTween.Stop(gameObject);
        m_Pool.RecycleGameObject(this);
    }
}