using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCardDeckManager : MonoBehaviour
{
    private static SelectCardDeckManager _scdm;

    public static SelectCardDeckManager SCDM
    {
        get
        {
            if (!_scdm) _scdm = FindObjectOfType<SelectCardDeckManager>();
            return _scdm;
        }
    }

    private int cardSelectLayer;

    void Awake()
    {
        cardSelectLayer = 1 << LayerMask.NameToLayer("CardSelect");
    }

    void Start()
    {
        HideWindow();
        AddAllCards();
    }

    void Update()
    {
        if (!Canvas.enabled)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ShowWindow();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
            {
                HideWindow();
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (CurrentPreviewCard)
                    {
                        HidePreviewCard();
                    }
                    else
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit raycast;
                        Physics.Raycast(ray, out raycast, 10f, cardSelectLayer);
                        if (raycast.collider != null)
                        {
                            CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                            if (card)
                            {
                                ShowPreviewCard(card);
                            }
                            else
                            {
                                HidePreviewCard();
                            }
                        }
                        else
                        {
                            HidePreviewCard();
                        }
                    }
                }
            }
        }
    }

    private void ShowPreviewCard(CardBase card)
    {
        HidePreviewCard();
        CurrentPreviewCard = card;
        PreviewCard = CardBase.InstantiateCardByCardInfo(card.CardInfo, Content, null, true);
        PreviewCard.transform.localScale = Vector3.one * 4;
    }

    private void HidePreviewCard()
    {
        if (PreviewCard)
        {
            PreviewCard.PoolRecycle();
            PreviewCard = null;
            CurrentPreviewCard = null;
        }
    }

    private CardBase CurrentPreviewCard;
    private CardBase PreviewCard;

    [SerializeField]
    private Transform Content;

    [SerializeField]
    private Canvas Canvas;

    [SerializeField]
    private Button ConfirmButton;

    [SerializeField]
    private Button CloseButton;

    private List<CardBase> allCards = new List<CardBase>();
    private List<CardBase> SelectedCards = new List<CardBase>();

    public void ShowWindow()
    {
        Canvas.enabled = true;
        bool state = Client.CS.Proxy.ClientState == ProxyBase.ClientStates.GetId || Client.CS.Proxy.ClientState == ProxyBase.ClientStates.SubmitCardDeck;
        ConfirmButton.gameObject.SetActive(state);
        CloseButton.gameObject.SetActive(!state);
    }

    public void HideWindow()
    {
        Canvas.enabled = false;
    }

    public void AddAllCards()
    {
        foreach (CardInfo_Base cardInfo in AllCards.CardDict.Values)
        {
            AddCardIntoCardSelectWindow(cardInfo);
        }
    }

    public void AddCardIntoCardSelectWindow(CardInfo_Base cardInfo)
    {
        CardBase newCard = CardBase.InstantiateCardByCardInfo(cardInfo, Content, null, true);
        newCard.transform.localScale = Vector3.one * 120;
        newCard.transform.rotation = Quaternion.Euler(90, 180, 0);
    }

    public void ConfirmSubmitCardDeck()
    {
        Client.CS.Proxy.OnSendCardDeck(new CardDeckInfo(new int[] {0, 0, 1, 0, 100, 101, 300, 301, 350, 351, 200, 201, 202, 350, 102}));
        NetworkManager.NM.ShowInfoPanel("更新卡组成功", 0, 1f);
    }
}