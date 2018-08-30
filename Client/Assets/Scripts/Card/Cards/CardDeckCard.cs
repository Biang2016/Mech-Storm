using UnityEngine;

public class CardDeckCard : MonoBehaviour, IGameObjectPool
{
    internal ClientPlayer ClientPlayer;

    GameObjectPool gameObjectPool;

    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.Pool_CardDeckCardPool;
    }

    public virtual void PoolRecycle()
    {
        gameObjectPool.RecycleGameObject(gameObject);
        transform.localScale = Vector3.one * 1.4f;
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    [SerializeField] private Renderer MainBoardRenderer;
    [SerializeField] private Renderer CardBloomRenderer;

    public void ResetColor()
    {
        if (ClientPlayer == RoundManager.Instance.SelfClientPlayer)
        {
            ClientUtils.ChangeColor(MainBoardRenderer, GameManager.Instance.SelfCardDeckCardColor);
            ClientUtils.ChangeColor(CardBloomRenderer, GameManager.Instance.SelfCardDeckCardColor);
        }
        else
        {
            ClientUtils.ChangeColor(MainBoardRenderer, GameManager.Instance.EnemyCardDeckCardColor);
            ClientUtils.ChangeColor(CardBloomRenderer, GameManager.Instance.EnemyCardDeckCardColor);
        }
    }
}