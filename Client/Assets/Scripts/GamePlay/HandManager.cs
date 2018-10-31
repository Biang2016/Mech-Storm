using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    //每张牌之间的夹角
    static float[] ANGLES_DICT = new float[30] {20f, 20f, 30f, 40f, 45f, 50f, 55f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f};

    //每张牌之间的距离
    static float[] HORRIZON_DISTANCE_DICT = new float[30] {1.5f, 1.6f, 1.8f, 2.3f, 2.8f, 3.3f, 4.2f, 4.9f, 5.6f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f};

    internal ClientPlayer ClientPlayer;
    public List<CardBase> cards;
    private CardBase currentShowCard;
    private CardBase lastShowCard;

    [SerializeField] private Text HandCardCountText;
    [SerializeField] private Animator HandCardCountPanelAnim;

    int cardLayer;

    private void Awake()
    {
        cards = new List<CardBase>();
        cardLayer = 1 << LayerMask.NameToLayer("Cards");
    }

    void Update()
    {
        CheckMousePosition();
        Update_CheckSlotBloomTipOff();
        UpdateCurrentFocusCardTicker();
        UpdateHandCardCountTicker();
    }

    #region Game Process

    public void BeginRound()
    {
        RoundManager.Instance.IdleClientPlayer.MyHandManager.SetAllCardUnusable();
        foreach (CardBase card in cards) card.OnBeginRound();
        RefreshAllCardUsable();
    }

    public void EndRound()
    {
        foreach (CardBase card in cards) card.OnEndRound();
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

        if (currentSummonRetinuePreviewCard)
        {
            currentSummonRetinuePreviewCard.PoolRecycle();
            currentSummonRetinuePreviewCard = null;
        }

        ClientPlayer = null;
        summonRetinuePreviewCardIndex = 0;
    }

    #endregion

    #region RefreshHandCards

    private void CheckMousePosition() //检查鼠标是否还停留在某张牌上，如果否，取消放大效果
    {
        if (!IsBeginDrag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 10f, cardLayer);
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

    internal void RefreshCardsPlace(float duration = 0.1f) //重置所有手牌位置
    {
        RefreshCardsPlace(cards.Count, duration);
    }

    private void RefreshCardsPlace(int cardCount, float duration) //按虚拟的手牌总数重置所有手牌位置
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
            Vector3 position = result.position;
            Vector3 rotation = result.rotation.eulerAngles;
            Vector3 scale = result.localScale;

            Hashtable args = new Hashtable();
            args.Add("position", position);
            args.Add("time", duration);
            args.Add("easeType", iTween.EaseType.linear);
            iTween.MoveTo(card.gameObject, args);
            args.Add("rotation", rotation);
            iTween.RotateTo(card.gameObject, args);
            args.Add("scale", scale);
            iTween.ScaleTo(card.gameObject, args);
        }

        RefreshAllCardUsable();
        RefreshCardsOrderInLayer();
    }

    private GameObject GetCardPlacePivot;

    private Transform GetCardPlace(int cardIndex, int toatalCardNumber)
    {
        int rev = ClientPlayer == RoundManager.Instance.EnemyClientPlayer ? -1 : 1;

        if (toatalCardNumber == 0) return null;
        if (GetCardPlacePivot == null) GetCardPlacePivot = new GameObject("GetCardPlacePivot");
        GetCardPlacePivot.transform.SetParent(transform);

        GetCardPlacePivot.transform.position = DefaultCardPivot.position;
        GetCardPlacePivot.transform.rotation = DefaultCardPivot.rotation;
        GetCardPlacePivot.transform.localScale = Vector3.one * GameManager.Instance.HandCardSize;

        float angle = ANGLES_DICT[toatalCardNumber - 1] * GameManager.Instance.HandCardRotate;
        float horrizonDist = HORRIZON_DISTANCE_DICT[toatalCardNumber - 1] * GameManager.Instance.HandCardInterval;
        float rotateAngle = angle / toatalCardNumber * (((toatalCardNumber - 1) / 2.0f + 1) - cardIndex);
        if (ClientPlayer.WhichPlayer == Players.Enemy) GetCardPlacePivot.transform.Rotate(-Vector3.right * 180);
        GetCardPlacePivot.transform.position = new Vector3(GetCardPlacePivot.transform.position.x, 4f, GetCardPlacePivot.transform.position.z);
        float horrizonDistance = horrizonDist / toatalCardNumber * (((toatalCardNumber - 1) / 2.0f + 1) - cardIndex);
        GetCardPlacePivot.transform.Translate(-Vector3.right * horrizonDistance * GameManager.Instance.HandCardSize); //向水平向错开，体现手牌展开感
        float distCardsFromCenter = Mathf.Abs(((toatalCardNumber - 1) / 2.0f + 1) - cardIndex); //与中心距离几张卡牌
        float factor = (toatalCardNumber - distCardsFromCenter) / toatalCardNumber; //某临时参数
        GetCardPlacePivot.transform.Translate(-Vector3.back * 0.13f * distCardsFromCenter * (1 - factor * factor) * 0.5f * GameManager.Instance.HandCardSize + Vector3.back * toatalCardNumber / 20 * GameManager.Instance.HandCardOffset); //向垂直向错开，体现卡片弧线感
        GetCardPlacePivot.transform.Translate(Vector3.down * 0.1f * (toatalCardNumber - cardIndex) * rev); //向上错开，体现卡片前后感
        GetCardPlacePivot.transform.Rotate(-Vector3.down, rotateAngle); //卡片微小旋转

        return GetCardPlacePivot.transform;
    }

    internal void RefreshAllCardUsable() //刷新所有卡牌是否可用
    {
        if (ClientPlayer == null) return;
        foreach (CardBase card in cards)
        {
            if (ClientPlayer == RoundManager.Instance.CurrentClientPlayer)
            {
                card.Usable = (ClientPlayer == RoundManager.Instance.SelfClientPlayer) && (card.M_Metal <= ClientPlayer.MetalLeft && card.M_Energy <= ClientPlayer.EnergyLeft);
                if (card is CardRetinue) card.Usable &= !ClientPlayer.MyBattleGroundManager.BattleGroundIsFull;
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

    IEnumerator Co_GetCards(List<DrawCardRequest.CardIdAndInstanceId> cardIdAndInstanceIds) //多卡片抽取动画
    {
        float cardFlyTime = 1f;
        float intervalTime = 0.3f;

        RefreshCardsPlace(cards.Count + cardIdAndInstanceIds.Count, 0.1f);
        yield return new WaitForSeconds(0.2f);

        int count = 0;
        int currentCount = cards.Count;
        foreach (DrawCardRequest.CardIdAndInstanceId cardIdAndInstanceId in cardIdAndInstanceIds)
        {
            count++;
            StartCoroutine(SubCo_GetCard(currentCount + count, currentCount + cardIdAndInstanceIds.Count, cardIdAndInstanceId, cardFlyTime));
            yield return new WaitForSeconds(intervalTime);
        }

        RefreshAllCardUsable();
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
        yield return null;
    }

    [SerializeField] private Transform DrawCardPivot;

    IEnumerator SubCo_GetCard(int indexNumber, int totalCardNumber, DrawCardRequest.CardIdAndInstanceId cardIdAndInstanceId, float duration) //单卡片抽取动画
    {
        CardInfo_Base newCardInfoBase = AllCards.GetCard(cardIdAndInstanceId.CardId);
        CardBase newCardBase = CardBase.InstantiateCardByCardInfo(newCardInfoBase, transform, ClientPlayer, false);
        newCardBase.M_CardInstanceId = cardIdAndInstanceId.CardInstanceId;

        cards.Add(newCardBase);

        RefreshCardsOrderInLayer();

        RefreshAllCardUsable();

        Transform srcPos = DrawCardPivot;
        Transform tarTran = GetCardPlace(indexNumber, totalCardNumber);
        Vector3 tarPos = tarTran.position;
        Quaternion tarRot = tarTran.rotation;

        newCardBase.transform.position = srcPos.position;
        newCardBase.transform.rotation = srcPos.rotation;
        newCardBase.transform.localScale = Vector3.one * GameManager.Instance.HandCardSize;

        Hashtable arg = new Hashtable();
        arg.Add("position", tarPos);
        arg.Add("time", duration);
        arg.Add("rotation", tarRot.eulerAngles);

        iTween.MoveTo(newCardBase.gameObject, arg);
        iTween.RotateTo(newCardBase.gameObject, arg);

        newCardBase.IsFlying = true;
        AudioManager.Instance.SoundPlay("sfx/DrawCard0");

        yield return new WaitForSeconds(duration);
        newCardBase.IsFlying = false;

        yield return new WaitForSeconds(0.1f);
    }

    private void RefreshCardsOrderInLayer()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].SetOrderInLayer(i);
        }

        if (CurrentFocusCard) CurrentFocusCard.SetOrderInLayer(100);
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
                iTween.Stop(cardBase.gameObject);

                if (currentShowCard)
                {
                    lastShowCard = currentShowCard;
                    iTween.Stop(lastShowCard.gameObject);

                    Hashtable lastShowCardMoveBeneath = new Hashtable();
                    lastShowCardMoveBeneath.Add("time", GameManager.Instance.ShowCardFlyDuration);
                    lastShowCardMoveBeneath.Add("position", GameManager.Instance.UseCardShowPosition);
                    lastShowCardMoveBeneath.Add("rotation", new Vector3(0, 180, 0));
                    lastShowCardMoveBeneath.Add("scale", Vector3.one * GameManager.Instance.CardShowScale);

                    iTween.MoveTo(lastShowCard.gameObject, lastShowCardMoveBeneath);
                    iTween.RotateTo(lastShowCard.gameObject, lastShowCardMoveBeneath);
                    iTween.ScaleTo(lastShowCard.gameObject, lastShowCardMoveBeneath);
                }

                currentShowCard = CardBase.InstantiateCardByCardInfo(cardInfo, transform, ClientPlayer, false);
                currentShowCard.transform.position = cardBase.transform.position;
                currentShowCard.transform.rotation = cardBase.transform.rotation;
                currentShowCard.transform.localScale = cardBase.transform.localScale;

                cardBase.PoolRecycle();

                currentShowCard.DragComponent.enabled = false;
                currentShowCard.Usable = false;
                currentShowCard.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#FFFFFF"));
                currentShowCard.CardBloom.SetActive(true);
                currentShowCard.BeBrightColor();
                currentShowCard.CardCanvas.enabled = true;

                Hashtable currentCardMove = new Hashtable();
                currentCardMove.Add("time", GameManager.Instance.ShowCardFlyDuration);
                currentCardMove.Add("position", GameManager.Instance.UseCardShowPosition_Overlay);
                currentCardMove.Add("rotation", new Vector3(0, 180, 0));
                currentCardMove.Add("scale", Vector3.one * GameManager.Instance.CardShowScale);

                iTween.MoveTo(currentShowCard.gameObject, currentCardMove);
                iTween.RotateTo(currentShowCard.gameObject, currentCardMove);
                iTween.ScaleTo(currentShowCard.gameObject, currentCardMove);

                RefreshCardsPlace();
                yield return new WaitForSeconds(GameManager.Instance.ShowCardFlyDuration);

                if (lastShowCard)
                {
                    lastShowCard.PoolRecycle();
                    lastShowCard = null;
                }

                yield return new WaitForSeconds(GameManager.Instance.ShowCardDuration - GameManager.Instance.ShowCardFlyDuration);

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
            ClientPlayer.MyMetalLifeEnergyManager.MetalBarManager.ResetHightlightTopBlocks();
        }

        CurrentFocusCard = focusCard;
        HandCardEnlarge(focusCard);

        if (ClientPlayer == RoundManager.Instance.SelfClientPlayer)
        {
            if (CurrentFocusCard is CardEquip)
            {
                ClientPlayer.MyBattleGroundManager.ShowTipSlotBlooms((CardEquip) CurrentFocusCard);
                currentFocusEquipmentCard = CurrentFocusCard;
            }

            ClientPlayer.MyMetalLifeEnergyManager.MetalBarManager.HightlightTopBlocks(focusCard.CardInfo.BaseInfo.Metal);
        }

        if (focusCard is CardSpell && ((CardSpell) focusCard).CardInfo.TargetInfo.HasTargetEquip)
        {
            RoundManager.Instance.SelfClientPlayer.MyBattleGroundManager.ShowTipModuleBloomSE(0.3f);
            RoundManager.Instance.EnemyClientPlayer.MyBattleGroundManager.ShowTipModuleBloomSE(0.3f);
        }

        currentFocusCardTickerBegin = true;
    }

    internal void CardOnMouseLeave(CardBase focusCard) //鼠标离开卡牌
    {
        if (ClientPlayer.WhichPlayer == Players.Enemy) return;
        if (IsBeginDrag) return;
        RefreshCardsPlace();
        ClientPlayer.MyMetalLifeEnergyManager.MetalBarManager.ResetHightlightTopBlocks();
    }

    internal void CardColliderReplaceOnMouseExit(CardBase lostFocusCard) //鼠标离开代替卡牌的碰撞区
    {
        if (ClientPlayer.WhichPlayer == Players.Enemy) return;
        if (IsBeginDrag) return;
        HandCardShrink(lostFocusCard);
        RefreshCardsPlace();
        if (currentFocusEquipmentCard == lostFocusCard) currentFocusEquipmentCard = null;
        ClientPlayer.MyMetalLifeEnergyManager.MetalBarManager.ResetHightlightTopBlocks();
        if (!Input.GetMouseButton(0)) ClientPlayer.MyBattleGroundManager.StopShowSlotBloom();
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

        iTween.Stop(focusCard.gameObject);

        //Replace the card by a boxcollider
        ColliderReplace colliderReplace = GameObjectPoolManager.Instance.Pool_ColliderReplacePool.AllocateGameObject<ColliderReplace>(GameBoardManager.Instance.transform);
        colliderReplace.Initiate(focusCard);
        //Enlarge the card and put it in upright position
        focusCard.transform.localScale = Vector3.one * GameManager.Instance.PullOutCardSize;
        focusCard.transform.rotation = DefaultCardPivot.rotation;
        focusCard.transform.position = new Vector3(focusCard.transform.position.x, 2f, focusCard.transform.position.z);
        focusCard.transform.Translate(Vector3.up * 5f);
        focusCard.transform.Translate(Vector3.back * 3f);
        //Disenable the card's boxcollider
        focusCard.GetComponent<BoxCollider>().enabled = false;
        isEnlarge = true;
    }

    void HandCardShrink(CardBase lostFocusCard)
    {
        if (ClientPlayer == null) return;
        if (ClientPlayer.WhichPlayer == Players.Enemy) return;
        if (IsBeginDrag) return;

        iTween.Stop(lostFocusCard.gameObject);

        lostFocusCard.transform.localScale = Vector3.one * GameManager.Instance.HandCardSize;
        if (lostFocusCard.myColliderReplace)
        {
            lostFocusCard.transform.position = lostFocusCard.myColliderReplace.transform.position;
            lostFocusCard.transform.rotation = lostFocusCard.myColliderReplace.transform.rotation;
        }

        lostFocusCard.ResetColliderAndReplace();
        currentFocusCardTickerBegin = false;
        AffixManager.Instance.HideAffixPanel();
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
                    AffixManager.Instance.ShowAffixTips(new List<CardInfo_Base> {CurrentFocusCard.CardInfo});
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
                if (DragComponent.CheckAreas() == ClientPlayer.MyHandArea)
                {
                    if (!handCardCountTickerBegin)
                    {
                        handCardCountTicker += Time.deltaTime;
                        if (handCardCountTicker > (ClientPlayer == RoundManager.Instance.SelfClientPlayer ? handCardCountTimeThreshold : handCardCountTimeThreshold_Enemy))
                        {
                            handCardCountTickerBegin = true;
                            handCardCountTicker = 0;
                            HandCardCountPanelAnim.SetTrigger("Jump");
                            if (ClientPlayer == RoundManager.Instance.SelfClientPlayer)
                            {
                                HandCardCountText.text = GameManager.Instance.IsEnglish ? "Your have " + cards.Count + " cards." : "你有" + cards.Count + "张手牌";
                            }
                            else
                            {
                                HandCardCountText.text = GameManager.Instance.IsEnglish ? "Your component has " + cards.Count + " cards." : "你的对手有" + cards.Count + "张手牌";
                                foreach (CardBase cb in cards)
                                {
                                    cb.CardBackBloom.SetActive(true);
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
                    foreach (CardBase cb in cards)
                    {
                        cb.CardBackBloom.SetActive(false);
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
            Physics.Raycast(ray, out raycast, 10f, cardLayer);
            if (raycast.collider != null)
            {
                ColliderReplace collider = raycast.collider.gameObject.GetComponent<ColliderReplace>();
                if (!collider)
                {
                    ClientPlayer.MyBattleGroundManager.StopShowSlotBloom();
                    currentFocusEquipmentCard = null;
                }
            }
            else
            {
                ClientPlayer.MyBattleGroundManager.StopShowSlotBloom();
                currentFocusEquipmentCard = null;
            }
        }
    }

    #endregion


    #region Pre_summon mechs that have target SideEffects.

    private CardRetinue currentSummonRetinuePreviewCard;
    private int summonRetinuePreviewCardIndex;

    public void SetCurrentSummonRetinuePreviewCard(CardRetinue retinueCard)
    {
        currentSummonRetinuePreviewCard = retinueCard;
        summonRetinuePreviewCardIndex = cards.IndexOf(currentSummonRetinuePreviewCard);
        currentSummonRetinuePreviewCard.gameObject.SetActive(false);
    }

    public void CancelSummonRetinuePreview()
    {
        currentSummonRetinuePreviewCard.gameObject.SetActive(true);
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