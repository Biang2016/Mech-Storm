using UnityEngine;
using UnityEngine.UI;

public class CardDeckCard : PoolObject
{
    void Awake()
    {
    }

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        transform.localScale = Vector3.one * 1.4f;
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    [SerializeField] private RawImage MainBoardRenderer;
    [SerializeField] private RawImage CardBloomRenderer;

    public void ResetColor(bool isSelf)
    {
        if (isSelf)
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