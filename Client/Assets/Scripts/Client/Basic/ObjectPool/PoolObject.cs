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
        m_Pool.RecycleGameObject(this);
    }

    public void SoundPlay(string path)
    {
        AudioManager.Instance.SoundPlay(path);
    }
}