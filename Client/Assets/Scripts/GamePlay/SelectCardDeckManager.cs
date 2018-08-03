using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
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
            bool isMouseDown = ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && EventSystem.current.IsPointerOverGameObject());
            bool isMouseUp = ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && EventSystem.current.IsPointerOverGameObject());
            if (CurrentPreviewCard)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || isMouseDown) HidePreviewCard();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
                {
                    HideWindow();
                }
                else if (isMouseDown)
                {
                    mouseDownPosition = Input.mousePosition;
                    Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit raycast;
                    Physics.Raycast(ray, out raycast, 10f, cardSelectLayer);
                    if (raycast.collider != null)
                    {
                        CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                        if (Input.GetMouseButtonDown(1))
                        {
                            if (card)
                            {
                                mouseRightDownCard = card;
                            }
                            else
                            {
                                mouseRightDownCard = null;
                            }
                        }
                        else if (Input.GetMouseButtonDown(0) && card)
                        {
                            mouseLeftDownCard = card;
                        }
                    }
                    else
                    {
                        mouseRightDownCard = null;
                        mouseLeftDownCard = null;
                        HidePreviewCard();
                    }
                }
                else if (isMouseUp)
                {
                    Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit raycast;
                    Physics.Raycast(ray, out raycast, 10f, cardSelectLayer);
                    if (raycast.collider != null)
                    {
                        CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                        if (Input.GetMouseButtonUp(1))
                        {
                            if (card && mouseRightDownCard == card)
                            {
                                ShowPreviewCard(card);
                            }
                            else
                            {
                                HidePreviewCard();
                            }
                        }
                        else if (Input.GetMouseButtonUp(0) && card)
                        {
                            if (isSelecting)
                            {
                                if ((Input.mousePosition - mouseDownPosition).magnitude < 50)
                                {
                                    if (mouseLeftDownCard == card)
                                    {
                                        SelectCard(card);
                                    }
                                }
                            }
                            else
                            {
                                if (card && mouseLeftDownCard == card)
                                {
                                    ShowPreviewCard(card);
                                }
                                else
                                {
                                    HidePreviewCard();
                                }
                            }
                        }
                    }
                    else
                    {
                        HidePreviewCard();
                    }

                    mouseLeftDownCard = null;
                    mouseRightDownCard = null;
                }
            }
        }
    }

    private void SelectCard(CardBase card)
    {
        if (SelectedCards.ContainsKey(card.CardInfo.CardID))
        {
            int count = ++SelectedCards[card.CardInfo.CardID].Count;
            card.SetBlockCountValue(count);
        }
        else
        {
            SelectCard newSC = GameObjectPoolManager.GOPM.Pool_SelectCardPool.AllocateGameObject(SelectionContent).GetComponent<SelectCard>();

            newSC.Count = 1;
            newSC.Cost = card.CardInfo.BaseInfo.Cost;
            newSC.Text_CardName.text = card.CardInfo.BaseInfo.CardName;
            newSC.CardButton.onClick.RemoveAllListeners();
            newSC.CardButton.onClick.AddListener(delegate { UnSelectCard(card); });
            Color cardColor = GameManager.HTMLColorToColor(card.CardInfo.BaseInfo.CardColor);
            newSC.CardButton.image.color = new Color(cardColor.r, cardColor.g, cardColor.b, 0.5f);

            SelectedCards.Add(card.CardInfo.CardID, newSC);
            card.SetBlockCountValue(1);
            card.BeBrightColor();
            card.CardBloom.SetActive(true);

            List<SelectCard> SCs = SelectionContent.GetComponentsInChildren<SelectCard>(true).ToList();
            SCs.Sort((a, b) => a.Cost.CompareTo(b.Cost));
            SelectionContent.DetachChildren();
            foreach (SelectCard selectCard in SCs)
            {
                selectCard.transform.parent = SelectionContent;
            }
        }
    }

    private void UnSelectCard(CardBase card)
    {
        if (!isSelecting) return;
        int count = --SelectedCards[card.CardInfo.CardID].Count;
        card.SetBlockCountValue(count);

        if (SelectedCards[card.CardInfo.CardID].Count == 0)
        {
            SelectedCards[card.CardInfo.CardID].PoolRecycle();
            SelectedCards.Remove(card.CardInfo.CardID);
            card.BeDimColor();

            card.CardBloom.SetActive(false);
        }
    }

    private CardBase mouseLeftDownCard;
    private CardBase mouseRightDownCard;
    private Vector3 mouseDownPosition;

    private CardBase CurrentPreviewCard;
    private CardBase PreviewCard;

    [SerializeField] private Transform Content;
    [SerializeField] private Transform SelectionContent;

    [SerializeField] private Canvas Canvas;
    [SerializeField] private Canvas Canvas_BG;
    [SerializeField] private Transform PreviewContent;

    [SerializeField] private Button ConfirmButton;

    [SerializeField] private Button CloseButton;

    [SerializeField] private Camera Camera;

    private List<CardBase> allCards = new List<CardBase>();
    private Dictionary<int, SelectCard> SelectedCards = new Dictionary<int, SelectCard>();

    [SerializeField] private Transform SelectCardPrefab;

    private void ShowPreviewCard(CardBase card)
    {
        HidePreviewCard();
        CurrentPreviewCard = card;
        PreviewCard = CardBase.InstantiateCardByCardInfo(card.CardInfo, PreviewContent, null, true);
        PreviewCard.transform.localScale = Vector3.one * 200;
        PreviewCard.transform.rotation = Quaternion.Euler(90, 180, 0);
        PreviewCard.transform.localPosition = new Vector3(0, 0, -300);
        PreviewCard.CardBloom.SetActive(true);
        PreviewCard.ChangeCardBloomColor(GameManager.HTMLColorToColor("#FFDD8C"));
    }

    private void HidePreviewCard()
    {
        if (PreviewCard)
        {
            PreviewCard.CardBloom.SetActive(true);
            PreviewCard.PoolRecycle();
            PreviewCard = null;
            CurrentPreviewCard = null;
        }
    }

    public bool IsShowing()
    {
        return Canvas.enabled;
    }

    private bool isSelecting;

    public void ShowWindow()
    {
        Canvas.enabled = true;
        Canvas_BG.enabled = true;
        isSelecting = Client.CS.Proxy.ClientState == ProxyBase.ClientStates.GetId || Client.CS.Proxy.ClientState == ProxyBase.ClientStates.SubmitCardDeck;
        ConfirmButton.gameObject.SetActive(isSelecting);
        CloseButton.gameObject.SetActive(!isSelecting);
    }

    public void HideWindow()
    {
        Canvas.enabled = false;
        Canvas_BG.enabled = false;
    }

    public void AddAllCards()
    {
        foreach (CardInfo_Base cardInfo in AllCards.CardDict.Values)
        {
            if (cardInfo.CardID == 999) continue;
            AddCardIntoCardSelectWindow(cardInfo);
        }
    }

    public void AddCardIntoCardSelectWindow(CardInfo_Base cardInfo)
    {
        CardBase newCard = CardBase.InstantiateCardByCardInfo(cardInfo, Content, null, true);
        newCard.transform.localScale = Vector3.one * 120;
        newCard.transform.rotation = Quaternion.Euler(90, 180, 0);
        newCard.BeDimColor();
        allCards.Add(newCard);
    }

    public void ConfirmSubmitCardDeck()
    {
        List<int> cardIds = new List<int>();
        foreach (KeyValuePair<int, SelectCard> kv in SelectedCards)
        {
            for (int i = 0; i < kv.Value.Count; i++)
            {
                cardIds.Add(kv.Key);
            }
        }

        CardDeckInfo cdi = new CardDeckInfo(cardIds.ToArray());
        Client.CS.Proxy.OnSendCardDeck(cdi);
        NetworkManager.NM.ShowInfoPanel("更新卡组成功", 0, 1f);
        HideWindow();
    }
}