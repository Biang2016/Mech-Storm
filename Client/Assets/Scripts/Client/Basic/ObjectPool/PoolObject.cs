using DG.Tweening;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    private GameObjectPool Pool { get; set; }

    internal bool IsRecycled = false;

    public void SetObjectPool(GameObjectPool pool)
    {
        Pool = pool;
    }

    public virtual void PoolRecycle()
    {
        transform.DOPause();
        Pool.RecycleGameObject(this);
        IsRecycled = true;
    }

    public void SoundPlay(string path)
    {
        AudioManager.Instance.SoundPlay(path);
    }
}