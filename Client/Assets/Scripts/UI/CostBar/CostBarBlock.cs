using UnityEngine;

public class CostBarBlock : MonoBehaviour, IGameObjectPool
{
    internal ClientPlayer ClientPlayer;

    private GameObjectPool gameObjectPool;

    public void PoolRecycle()
    {
        gameObjectPool.RecycleGameObject(gameObject);
        transform.localPosition = Vector3.zero;
    }

    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.Pool_CostBarBlockPool;
    }

    [SerializeField] private Renderer My_Renderer;

    public void Shine()
    {
        ClientUtils.ChangeColor(My_Renderer,ClientUtils.HTMLColorToColor("#FFFFFF"));
    }

    public void ResetColor()
    {
        if (ClientPlayer == RoundManager.Instance.SelfClientPlayer)
        {
            ClientUtils.ChangeColor(My_Renderer, GameManager.Instance.SelfCostBarColor);
        }
        else
        {
            ClientUtils.ChangeColor(My_Renderer, GameManager.Instance.EnemyCostBarColor);
        }
    }
}