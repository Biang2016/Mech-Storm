using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 卡堆
/// </summary>
public class CardDeckManager : MonoBehaviour
{
    internal ClientPlayer ClientPlayer;

    [SerializeField] private Transform CardDeckContainer;
    [SerializeField] private Text CardLeftNumText;
    [SerializeField] private Text CardLeftNumText_BG;

    private CardBase[] CardDeckCards = new CardBase[CARD_DECK_CARD_NUM];

    private static readonly int[] CARD_DECK_SHOW_CARD_NUM_MAP = new int[] {0, 1, 2, 3, 3, 3, 4, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 9, 9, 9, 9, 9, 10}; //剩余卡牌数量和卡堆模型中显示的卡牌数量映射关系
    private const int CARD_DECK_CARD_NUM = 10;
    private const float CARD_DECK_CARD_SIZE = 0.2f;
    private static readonly Vector3 SELF_CARD_DECK_CARD_INTERVAL = new Vector3(0.05f, 0.01f, 0.1f);

    public void Initialize(ClientPlayer clientPlayer)
    {
        ResetAll();
        ClientPlayer = clientPlayer;

        for (int i = 0; i < CARD_DECK_CARD_NUM; i++)
        {
            CardDeckCards[i] = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CardDeckCard].AllocateGameObject<CardDeckCard>(CardDeckContainer);
            CardDeckCards[i].SetCardBackColor();
            CardDeckCards[i].transform.Translate(-SELF_CARD_DECK_CARD_INTERVAL * i);
            CardDeckCards[i].transform.localScale = Vector3.one * CARD_DECK_CARD_SIZE;
        }

        ResetCardDeckNumberText();
    }

    public void ResetCardDeckNumberText()
    {
        SetCardDeckNumber(0, true);
    }

    public void ResetAll()
    {
        ClientPlayer = null;
        foreach (CardBase cb in CardDeckCards)
        {
            cb?.PoolRecycle();
        }

        CardDeckCards = new CardBase[CARD_DECK_CARD_NUM];
        ResetCardDeckNumberText();
    }

    private int cardDeckNumber;

    public void SetCardDeckNumber(int value, bool forceUpdate = false)
    {
        if (cardDeckNumber != value || forceUpdate)
        {
            if (value == 0)
            {
                CardLeftNumText.text = "";
                CardLeftNumText_BG.text = "";
            }
            else
            {
                CardLeftNumText.text = value.ToString();
                CardLeftNumText_BG.text = value.ToString();
            }

            SetCardDeckShowCardNum(CardDeckCards, value);
            cardDeckNumber = value;
        }
    }

    private void SetCardDeckShowCardNum(CardBase[] targetCardDeckShowCards, int number)
    {
        int showCardNumber = 0;
        if (number <= 0) showCardNumber = 0;
        else if (number >= CARD_DECK_SHOW_CARD_NUM_MAP.Length)
        {
            showCardNumber = CARD_DECK_SHOW_CARD_NUM_MAP[CARD_DECK_SHOW_CARD_NUM_MAP.Length - 1];
        }
        else
        {
            showCardNumber = CARD_DECK_SHOW_CARD_NUM_MAP[number];
        }

        for (int i = targetCardDeckShowCards.Length - 1; i >= showCardNumber; i--)
        {
            if (targetCardDeckShowCards[i] != null)
            {
                if (targetCardDeckShowCards[i].gameObject.activeSelf) targetCardDeckShowCards[i].gameObject.SetActive(false);
            }
        }

        for (int i = showCardNumber - 1; i >= 0; i--)
        {
            if (targetCardDeckShowCards[i] != null)
            {
                if (!targetCardDeckShowCards[i].gameObject.activeSelf) targetCardDeckShowCards[i].gameObject.SetActive(true);
            }
        }
    }
}