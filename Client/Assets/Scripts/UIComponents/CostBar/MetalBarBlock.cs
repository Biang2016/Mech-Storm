using UnityEngine;

public class MetalBarBlock : PoolObject
{
    internal ClientPlayer ClientPlayer;

    private GameObjectPool gameObjectPool;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        transform.localPosition = Vector3.zero;
    }

    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.PoolDict["MetalBarBlock"];
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
            ClientUtils.ChangeColor(My_Renderer, GameManager.Instance.SelfMetalBarColor);
        }
        else
        {
            ClientUtils.ChangeColor(My_Renderer, GameManager.Instance.EnemyMetalBarColor);
        }
    }
}