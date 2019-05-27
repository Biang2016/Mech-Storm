using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    //Angles between hand cards
    private static readonly float[] ANGLES_DICT = new float[30] {20f, 20f, 30f, 40f, 45f, 50f, 55f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f};

    //distances between hand cards
    private static readonly float[] HORRIZON_DISTANCE_DICT = new float[30] {1.5f, 1.6f, 1.8f, 2.3f, 2.8f, 3.3f, 4.2f, 4.9f, 5.6f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f};

    private readonly float HAND_CARD_SIZE = 0.3f;
    private readonly float HAND_CARD_INTERVAL = 5f;
    private readonly float HAND_CARD_ROTATE = 1.0f;
    private readonly float HAND_CARD_OFFSET = 1.2f;

    private readonly float PULL_OUT_CARD_SIZE = 0.5f;
    private readonly float PULL_OUT_CARD_OFFSET = 3.5f;

    private readonly float DRAW_CARD_FLY_TIME = 0.4f;
    private readonly float DRAW_CARD_INTERVAL_TIME = 0.15f;

    private readonly float USE_CARD_SHOW_SIZE = 0.5f;
    private readonly float USE_CARD_SHOW_DURATION = 0.5f;
    private readonly float USE_CARD_SHOW_FLY_DURATION = 0.15f;
    private static readonly Vector3 USE_CARD_SHOW_POSITION = new Vector3(10, 3, 0);
    private static readonly Vector3 USE_CARD_SHOW_POSITION_OVERLAY = new Vector3(10, 3, 0.2f);

    internal ClientPlayer ClientPlayer;
    public List<CardBase> cards = new List<CardBase>();
    private CardBase currentShowCard;
    private CardBase lastShowCard;

    [SerializeField] private Text HandCardCountText;
    [SerializeField] private Animator HandCardCountPanelAnim;

    public void Initialize(ClientPlayer clientPlayer)
    {
        ResetAll();
        ClientPlayer = clientPlayer;
        DrawCardPivots = ClientPlayer.WhichPlayer == Players.Self ? SelfDrawCardPivots : EnemyDrawCardPivots;
    }

    void Update()
    {
        CheckMousePosition();
        Update_CheckSlotBloomTipOff();
        UpdateCurrentFocusCardTicker();
        UpdateHandCardCountTicker();
    }

    public void SetLanguage(string languageShort)
    {
        foreach (CardBase card in cards)
        {
            card.RefreshCardTextLanguage();
        }
    }

    #region Game Process

    public void BeginRound()
    {
        RoundManager.Instance.IdleClientPlayer.BattlePlayer.HandManager.SetAllCardUnusable();
        foreach (CardBase card in cards) card.OnBeginRound();
        RefreshAllCardUsable();
    }

    public void EndRound()
    {
        foreach (CardBase card in cards) card.OnEndRound();
        RefreshAllCardUsable();
    }

    public void ResetAll()
    {
        foreach (CardBase cardBase in cards)
        {
            cardBase.PoolRecycle();
        }

        cards.Clear();
        if (currentShowCard)
        {
            currentShowCard.PoolRecycle();
            currentShowCard = null;
        }

        if (lastShowCard)
        {
            lastShowCard.PoolRecycle();
            lastShowCard = null;
        }

        if (CurrentFocusCard)
        {
            CurrentFocusCard.PoolRecycle();
            CurrentFocusCard = null;
        }

        if (currentFocusEquipmentCard)
        {
            currentFocusEquipmentCard.PoolRecycle();
            currentFocusEquipmentCard = null;
        }

        if (currentSummonMechPreviewCard)
        {
            currentSummonMechPreviewCard.PoolRecycle();
            currentSummonMechPreviewCard = null;
        }

        ClientPlayer = null;
        summonMechPreviewCardIndex = 0;
    }

    #endregion

    #region RefreshHandCards

    private void CheckMousePosition() //check mouse hover on cards，if false then shrink card
    {
        if (!IsBeginDrag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 10f, GameManager.Instance.Layer_Cards);
            if (raycast.collider == null)
            {
                if (CurrentFocusCard)
                {
                    HandCardShrink(CurrentFocusCard);
                    CurrentFocusCard = null;
                    RefreshCardsPlace();
                }
            }
        }
    }

    [SerializeField] private Transform DefaultCardPivot;

    internal void RefreshCardsPlace(float duration = 0.1f) //replace all handcards
    {
        RefreshCardsPlace(cards.Count, duration);
    }

    private void RefreshCardsPlace(int cardCount, float duration) //reset all handcards by preset places
    {
        if (CurrentFocusCard)
        {
            HandCardShrink(CurrentFocusCard);
            CurrentFocusCard = null;
        }

        if (ClientPlayer == null) return;
        if (cards.Count == 0) return;

        int count = 0;
        foreach (CardBase card in cards)
        {
            count++;
            Transform result = GetCardPlace(count, cardCount);
            Debug.Log("Replace: " + count + "/" + cardCount);
            Vector3 position = result.position;
            Vector3 rotation = result.rotation.eulerAngles;
            Vector3 scale = result.localScale;

            card.transform.DOMove(position, duration).SetEase(Ease.InOutQuart);
            card.transform.DORotate(rotation, duration).SetEase(Ease.InOutQuart);
            card.transform.DOScale(scale, duration).SetEase(Ease.InOutQuart);
        }

        RefreshAllCardUsable();
        RefreshCardsOrderInLayer();
    }

    private GameObject GetCardPlacePivot;

    enum HandCardOrientation
    {
        LeftToRight,
        RightToLeft,
    }

    private HandCardOrientation m_HandCardOrientation = HandCardOrientation.LeftToRight;

    private Transform GetCardPlace(int cardIndex, int totalCardNumber)
    {
        int rev = m_HandCardOrientation == HandCardOrientation.LeftToRight ? 1 : -1;
        int rev_player = ClientPlayer.WhichPlayer == Players.Self ? 1 : -1;
        int rev_player_180 = ClientPlayer.WhichPlayer == Players.Self ? 0 : 1;

        if (totalCardNumber == 0) return null;
        if (GetCardPlacePivot == null) GetCardPlacePivot = new GameObject("GetCardPlacePivot");
        GetCardPlacePivot.transform.SetParent(transform);

        GetCardPlacePivot.transform.position = DefaultCardPivot.position;
        GetCardPlacePivot.transform.rotation = DefaultCardPivot.rotation;
        GetCardPlacePivot.transform.localScale = Vector3.one * HAND_CARD_SIZE;

        float angle = ANGLES_DICT[totalCardNumber - 1] * HAND_CARD_ROTATE;
        float horizonDist = HORRIZON_DISTANCE_DICT[totalCardNumber - 1] * HAND_CARD_INTERVAL;
        float rotateAngle = angle / totalCardNumber * (((totalCardNumber - 1) / 2.0f + 1) - cardIndex);
        if (ClientPlayer.WhichPlayer == Players.Enemy) GetCardPlacePivot.transform.Rotate(-Vector3.right * 180);
        GetCardPlacePivot.transform.position = new Vector3(GetCardPlacePivot.transform.position.x, 4f, GetCardPlacePivot.transform.position.z);
        float horizonDistance = horizonDist / totalCardNumber * (((totalCardNumber - 1) / 2.0f + 1) - cardIndex);
        GetCardPlacePivot.transform.Translate(rev * (-Vector3.right * horizonDistance * HAND_CARD_SIZE)); //horizontal offset, fro
        float distCardsFromCenter = Mathf.Abs(((totalCardNumber - 1) / 2.0f + 1) - cardIndex); //the number of cards between this card and center card
        float factor = (totalCardNumber - distCardsFromCenter) / totalCardNumber; //temp param
        GetCardPlacePivot.transform.Translate(rev_player * -1 * (-Vector3.down * 1f * distCardsFromCenter * (1 - factor * factor) * 0.5f * HAND_CARD_SIZE + Vector3.down * totalCardNumber / 30 * HAND_CARD_OFFSET)); //arc offset
        GetCardPlacePivot.transform.Rotate(rev_player * Vector3.forward, rev * rotateAngle + rev_player_180 * 180); //tiny rotate of cards
        GetCardPlacePivot.transform.Translate(rev_player * -1 * Vector3.back * 0.01f * cardIndex); //vertical offset
        return GetCardPlacePivot.transform;
    }

    internal void RefreshAllCardUsable()
    {
        if (ClientPlayer == null) return;
        foreach (CardBase card in cards)
        {
            if (ClientPlayer == RoundManager.Instance.CurrentClientPlayer)
            {
                card.Usable = (ClientPlayer == RoundManager.Instance.SelfClientPlayer) && (card.M_Metal <= ClientPlayer.MetalLeft && card.M_Energy <= ClientPlayer.EnergyLeft);
                if (card is CardMech) card.Usable &= !ClientPlayer.BattlePlayer.BattleGroundManager.BattleGroundIsFull;
            }
            else
            {
                card.Usable = false;
            }
        }
    }

    internal void SetAllCardUnusable()
    {
        foreach (CardBase card in cards) card.Usable = false;
    }

    #endregion

    #region Draw cards

    public void GetCards(List<DrawCardRequest.CardIdAndInstanceId> cardIdAndInstanceIds)
    {
        if (ClientPlayer == null) return;
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_GetCards(cardIdAndInstanceIds), "Co_GetCard");
    }

    IEnumerator Co_GetCards(List<DrawCardRequest.CardIdAndInstanceId> cardIdAndInstanceIds) //animation of drawing multiple cards
    {
        RefreshCardsPlace(cards.Count + cardIdAndInstanceIds.Count, 0.3f);
        yield return new WaitForSeconds(0.3f);

        int count = 0;
        int currentCount = cards.Count;
        foreach (DrawCardRequest.CardIdAndInstanceId cardIdAndInstanceId in cardIdAndInstanceIds)
        {
            count++;
            StartCoroutine(SubCo_GetCard(currentCount + count, currentCount + cardIdAndInstanceIds.Count, cardIdAndInstanceId, DRAW_CARD_FLY_TIME));
            yield return new WaitForSeconds(DRAW_CARD_INTERVAL_TIME);
        }

        yield return new WaitForSeconds(DRAW_CARD_FLY_TIME - DRAW_CARD_INTERVAL_TIME);

        RefreshAllCardUsable();
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
        yield return null;
    }

    [SerializeField] private Transform[] SelfDrawCardPivots;
    [SerializeField] private Transform[] EnemyDrawCardPivots;
    private Transform[] DrawCardPivots;

    IEnumerator SubCo_GetCard(int indexNumber, int totalCardNumber, DrawCardRequest.CardIdAndInstanceId cardIdAndInstanceId, float duration) //animation of draw single card
    {
        CardInfo_Base newCardInfoBase = AllCards.GetCard(cardIdAndInstanceId.CardId);
        CardBase newCardBase = CardBase.InstantiateCardByCardInfo(newCardInfoBase, transform, CardBase.CardShowMode.HandCard, ClientPlayer);
        newCardBase.M_CardInstanceId = cardIdAndInstanceId.CardInstanceId;
        cards.Add(newCardBase);

        RefreshCardsOrderInLayer();
        RefreshAllCardUsable();

        Transform deckCardTran = ClientPlayer.BattlePlayer.CardDeckManager.GetFirstCardDeckCardPos();
        Transform srcTran = DrawCardPivots[0];
        Transform tarTran = GetCardPlace(indexNumber, totalCardNumber);
        Vector3 tarPos = tarTran.position;
        Quaternion tarRotation = tarTran.rotation;

        newCardBase.transform.position = deckCardTran.position;
        newCardBase.transform.rotation = srcTran.rotation;
        newCardBase.transform.localScale = Vector3.one * HAND_CARD_SIZE;

        newCardBase.BoxCollider.enabled = false;

        Vector3[] path = new Vector3[DrawCardPivots.Length];
        for (int i = 1; i < DrawCardPivots.Length; i++)
        {
            path[i - 1] = DrawCardPivots[i].position;
        }

        path[path.Length - 1] = tarPos;
        newCardBase.transform.DOPath(path, duration, PathType.CatmullRom);

        AudioManager.Instance.SoundPlay("sfx/DrawCard0", 0.4f);

        Sequence seq = DOTween.Sequence();

        for (int i = 1; i < DrawCardPivots.Length - 1; i++)
        {
            seq.Append(newCardBase.transform.DORotateQuaternion(DrawCardPivots[i].rotation, duration / DrawCardPivots.Length).SetEase(Ease.InOutQuart));
        }

        seq.Append(newCardBase.transform.DORotateQuaternion(tarRotation, duration / DrawCardPivots.Length).SetEase(Ease.InOutQuart));
        seq.Append(newCardBase.transform.DORotateQuaternion(tarRotation, duration / DrawCardPivots.Length).SetEase(Ease.Linear));
        seq.Play();
        yield return new WaitForSeconds(duration);
        newCardBase.BoxCollider.enabled = true;
        yield return null;
    }

    private void RefreshCardsOrderInLayer()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].CardOrder = i;
        }

        if (CurrentFocusCard) CurrentFocusCard.CardOrder = 80;
    }

    public void DropCard(int handCardInstanceId)
    {
        CardBase cardBase = GetCardByCardInstanceId(handCardInstanceId);
        cardBase.PoolRecycle();
        cards.Remove(cardBase);
        RefreshCardsPlace();
    }

    #endregion

    #region Play cards

    public void UseCard(int handCardInstanceId, CardInfo_Base cardInfo)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_UseCard(handCardInstanceId, cardInfo), "Co_UseCardShow");
    }

    IEnumerator Co_UseCard(int handCardInstanceId, CardInfo_Base cardInfo)
    {
        CardBase cardBase = GetCardByCardInstanceId(handCardInstanceId);

        if (ClientPlayer == RoundManager.Instance.EnemyClientPlayer)
        {
            if (cardBase)
            {
                cardBase.OnPlayOut();
                cards.Remove(cardBase);

                cardBase.transform.DOPause();

                if (currentShowCard)
                {
                    lastShowCard = currentShowCard;
                    lastShowCard.transform.DOPause();

                    lastShowCard.transform.DOMove(USE_CARD_SHOW_POSITION, USE_CARD_SHOW_FLY_DURATION).SetEase(Ease.Linear);
                    lastShowCard.transform.DORotate(new Vector3(0, 180, 0), USE_CARD_SHOW_FLY_DURATION).SetEase(Ease.Linear);
                    lastShowCard.transform.DOScale(Vector3.one * USE_CARD_SHOW_SIZE, USE_CARD_SHOW_FLY_DURATION).SetEase(Ease.Linear);
                }

                currentShowCard = CardBase.InstantiateCardByCardInfo(cardInfo, transform, CardBase.CardShowMode.ShowCard, ClientPlayer);
                currentShowCard.transform.position = cardBase.transform.position;
                currentShowCard.transform.rotation = cardBase.transform.rotation;
                currentShowCard.transform.localScale = cardBase.transform.localScale;

                cardBase.PoolRecycle();

                currentShowCard.DragComponent.enabled = false;
                currentShowCard.Usable = false;
                currentShowCard.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#FFFFFF"),1);
                currentShowCard.ShowCardBloom(true);
                currentShowCard.BeBrightColor();
                currentShowCard.CardOrder = 200;

                currentShowCard.transform.DOMove(USE_CARD_SHOW_POSITION_OVERLAY, USE_CARD_SHOW_FLY_DURATION).SetEase(Ease.Linear);
                currentShowCard.transform.DORotate(new Vector3(-90, 180, 0), USE_CARD_SHOW_FLY_DURATION).SetEase(Ease.Linear);
                currentShowCard.transform.DOScale(Vector3.one * USE_CARD_SHOW_SIZE, USE_CARD_SHOW_FLY_DURATION).SetEase(Ease.Linear);

                RefreshCardsPlace();
                yield return new WaitForSeconds(USE_CARD_SHOW_FLY_DURATION);

                if (lastShowCard)
                {
                    lastShowCard.PoolRecycle();
                    lastShowCard = null;
                }

                yield return new WaitForSeconds(USE_CARD_SHOW_DURATION - USE_CARD_SHOW_FLY_DURATION);

                currentShowCard.PoolRecycle();
                currentShowCard = null;
            }
        }
        else
        {
            cardBase.OnPlayOut();
            cards.Remove(cardBase);
            cardBase.PoolRecycle();
            RefreshCardsPlace();
        }

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #region UX

    public CardBase CurrentFocusCard;

    CardBase currentFocusEquipmentCard;

    internal void CardOnMouseEnter(CardBase focusCard)
    {
        if (ClientPlayer.WhichPlayer == Players.Enemy) return;
        if (IsBeginDrag && DragManager.Instance.CurrentDrag.gameObject != focusCard.gameObject)
        {
            return;
        }

        if (CurrentFocusCard == focusCard)
        {
            if (!isEnlarge)
            {
                HandCardEnlarge(focusCard);
            }

            return;
        }

        if (CurrentFocusCard && ClientPlayer == RoundManager.Instance.SelfClientPlayer)
        {
            HandCardShrink(CurrentFocusCard);
            RefreshCardsPlace();
            ClientPlayer.BattlePlayer.MetalLifeEnergyManager.MetalBarManager.ResetHighlightTopBlocks();
        }

        CurrentFocusCard = focusCard;
        HandCardEnlarge(focusCard);

        if (ClientPlayer == RoundManager.Instance.SelfClientPlayer)
        {
            if (CurrentFocusCard is CardEquip)
            {
                ClientPlayer.BattlePlayer.BattleGroundManager.ShowTipSlotBlooms((CardEquip) CurrentFocusCard);
                currentFocusEquipmentCard = CurrentFocusCard;
            }

            ClientPlayer.BattlePlayer.MetalLifeEnergyManager.MetalBarManager.HighlightTopBlocks(focusCard.CardInfo.BaseInfo.Metal);
        }

        if (focusCard is CardSpell && ((CardSpell) focusCard).CardInfo.TargetInfo.HasTargetEquip)
        {
            RoundManager.Instance.SelfClientPlayer.BattlePlayer.BattleGroundManager.ShowTipModuleBloomSE(0.3f);
            RoundManager.Instance.EnemyClientPlayer.BattlePlayer.BattleGroundManager.ShowTipModuleBloomSE(0.3f);
        }

        currentFocusCardTickerBegin = true;
    }

    internal void CardOnMouseLeave(CardBase focusCard)
    {
        if (ClientPlayer.WhichPlayer == Players.Enemy) return;
        if (IsBeginDrag) return;
        RefreshCardsPlace();
        ClientPlayer.BattlePlayer.MetalLifeEnergyManager.MetalBarManager.ResetHighlightTopBlocks();
    }

    internal void CardColliderReplaceOnMouseExit(CardBase lostFocusCard)
    {
        if (ClientPlayer.WhichPlayer == Players.Enemy) return;
        if (IsBeginDrag) return;
        HandCardShrink(lostFocusCard);
        RefreshCardsPlace();
        if (currentFocusEquipmentCard == lostFocusCard) currentFocusEquipmentCard = null;
        ClientPlayer.BattlePlayer.MetalLifeEnergyManager.MetalBarManager.ResetHighlightTopBlocks();
        if (!Input.GetMouseButton(0)) ClientPlayer.BattlePlayer.BattleGroundManager.StopShowSlotBloom();
    }

    #region Hang cards enlarge and shrink.

    private bool isBeginDrag;

    internal bool IsBeginDrag
    {
        get { return isBeginDrag; }
        set { isBeginDrag = value; }
    }

    private bool isEnlarge = false;

    void HandCardEnlarge(CardBase focusCard)
    {
        if (ClientPlayer == null) return;
        if (ClientPlayer.WhichPlayer == Players.Enemy) return;
        if (IsBeginDrag && DragManager.Instance.CurrentDrag.gameObject != focusCard.gameObject)
        {
            return;
        }

        focusCard.transform.DOPause();

        //Replace the card by a boxcollider
        ColliderReplace colliderReplace = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ColliderReplace].AllocateGameObject<ColliderReplace>(transform);
        colliderReplace.Initiate(focusCard);
        //Enlarge the card and put it in upright position
        focusCard.transform.localScale = Vector3.one * PULL_OUT_CARD_SIZE;
        focusCard.transform.rotation = DefaultCardPivot.rotation;
        focusCard.transform.position = new Vector3(focusCard.transform.position.x, focusCard.transform.position.y, focusCard.transform.position.z + PULL_OUT_CARD_OFFSET);
        //Disenable the card's boxcollider
        focusCard.BoxCollider.enabled = false;
        focusCard.CardOrder = 200;
        isEnlarge = true;
    }

    void HandCardShrink(CardBase lostFocusCard)
    {
        if (ClientPlayer == null) return;
        if (ClientPlayer.WhichPlayer == Players.Enemy) return;
        if (IsBeginDrag) return;

        lostFocusCard.transform.DOPause();

        lostFocusCard.transform.localScale = Vector3.one * HAND_CARD_SIZE;
        if (lostFocusCard.MyColliderReplace)
        {
            lostFocusCard.transform.position = lostFocusCard.MyColliderReplace.transform.position;
            lostFocusCard.transform.rotation = lostFocusCard.MyColliderReplace.transform.rotation;
        }

        lostFocusCard.ResetColliderAndReplace();
        currentFocusCardTickerBegin = false;
        UIManager.Instance.CloseUIForm<AffixPanel>();
        isEnlarge = false;
    }

    #endregion

    #region ShowHandCardAffixTips

    private bool currentFocusCardTickerBegin = false;
    private float currentFocusCardTicker = 0;
    private float currentFocusCardShowAffixTimeThreshold = 0.8f;

    private void UpdateCurrentFocusCardTicker()
    {
        if (currentFocusCardTickerBegin)
        {
            currentFocusCardTicker += Time.deltaTime;
            if (currentFocusCardTicker > currentFocusCardShowAffixTimeThreshold)
            {
                currentFocusCardTicker = 0;
                if (CurrentFocusCard)
                {
                    UIManager.Instance.ShowUIForms<AffixPanel>().ShowAffixTips(new List<CardInfo_Base> {CurrentFocusCard.CardInfo}, null);
                }

                currentFocusCardTickerBegin = false;
            }
        }
    }

    #endregion

    #region ShowHandCardCountTip

    private bool handCardCountTickerBegin = false;
    private float handCardCountTicker = 0;
    private float handCardCountTimeThreshold = 2f;
    private float handCardCountTimeThreshold_Enemy = 0.1f;

    private void UpdateHandCardCountTicker()
    {
        if (Client.Instance.IsPlaying())
        {
            if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                if (ClientPlayer == null) return;
                if (DragComponent.CheckAreas() == ClientPlayer.BattlePlayer.HandArea)
                {
                    if (!handCardCountTickerBegin)
                    {
                        handCardCountTicker += Time.deltaTime;
                        if (handCardCountTicker > (ClientPlayer == RoundManager.Instance.SelfClientPlayer ? handCardCountTimeThreshold : handCardCountTimeThreshold_Enemy))
                        {
                            handCardCountTickerBegin = true;
                            handCardCountTicker = 0;
                            HandCardCountPanelAnim.SetTrigger("Jump");

                            HandCardCountText.text = string.Format(LanguageManager.Instance.GetText("HandManager_TotalCards"), cards.Count);
                            if (ClientPlayer == RoundManager.Instance.EnemyClientPlayer)
                            {
                                foreach (CardBase cb in cards)
                                {
                                    cb.ShowCardBackBloom(true);
                                }
                            }
                        }
                    }
                }
                else
                {
                    handCardCountTicker = 0;
                    handCardCountTickerBegin = false;
                    HandCardCountPanelAnim.SetTrigger("Reset");
                    if (ClientPlayer == RoundManager.Instance.EnemyClientPlayer)
                    {
                        foreach (CardBase cb in cards)
                        {
                            cb.ShowCardBackBloom(false);
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region ShowAndHideEquipSlotBloomTip

    internal void Update_CheckSlotBloomTipOff()
    {
        if (!Input.GetMouseButton(0) && currentFocusEquipmentCard)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 10f, GameManager.Instance.Layer_Cards);
            if (raycast.collider != null)
            {
                ColliderReplace collider = raycast.collider.gameObject.GetComponent<ColliderReplace>();
                if (!collider)
                {
                    ClientPlayer.BattlePlayer.BattleGroundManager.StopShowSlotBloom();
                    currentFocusEquipmentCard = null;
                }
            }
            else
            {
                ClientPlayer.BattlePlayer.BattleGroundManager.StopShowSlotBloom();
                currentFocusEquipmentCard = null;
            }
        }
    }

    #endregion

    #region Pre_summon mechs that have target SideEffects.

    private CardMech currentSummonMechPreviewCard;
    private int summonMechPreviewCardIndex;

    public void SetCurrentSummonMechPreviewCard(CardMech mechCard)
    {
        currentSummonMechPreviewCard = mechCard;
        summonMechPreviewCardIndex = cards.IndexOf(currentSummonMechPreviewCard);
        currentSummonMechPreviewCard.gameObject.SetActive(false);
    }

    public void CancelSummonMechPreview()
    {
        currentSummonMechPreviewCard.gameObject.SetActive(true);
    }

    #endregion

    #endregion

    #region Utils

    public CardBase GetCardByCardInstanceId(int cardInstanceId)
    {
        foreach (CardBase cardBase in cards)
        {
            if (cardBase.M_CardInstanceId == cardInstanceId)
            {
                return cardBase;
            }
        }

        return null;
    }

    #endregion
}