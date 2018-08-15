using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class HandManager : MonoBehaviour
{
    internal ClientPlayer ClientPlayer;

    float[] anglesDict; //每张牌之间的夹角
    float[] horrizonDistanceDict; //每张牌之间的距离
    List<CardBase> cards;
    private CardBase currentShowCard;
    private CardBase lastShowCard;

    int retinueLayer;
    int cardLayer;

    private void Awake()
    {
        cards = new List<CardBase>();
        anglesDict = new float[30] {20f, 20f, 30f, 40f, 45f, 50f, 55f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f, 60f}; //手牌分布角度预设值
        horrizonDistanceDict = new float[30] {1.5f, 1.6f, 1.8f, 2.3f, 2.8f, 3.3f, 4.2f, 4.9f, 5.6f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f, 6.3f}; //手牌分布距离预设值
        retinueLayer = 1 << LayerMask.NameToLayer("Retinues");
        cardLayer = 1 << LayerMask.NameToLayer("Cards");
    }

    void Start()
    {
    }

    void Update()
    {
        CheckMousePosition();
        Update_CheckSlotBloomTipOff();
    }

    #region 响应

    public void Reset()
    {
        foreach (CardBase cardBase in cards)
        {
            cardBase.PoolRecycle();
        }

        cards.Clear();
        if (currentShowCard) currentShowCard.PoolRecycle();
        ClientPlayer = null;
    }

    public void GetCard(int cardId, int cardInstanceId)
    {
        if (ClientPlayer == null) return;
        CardInfo_Base newCardInfoBase = AllCards.GetCard(cardId);
        CardBase newCardBase = CardBase.InstantiateCardByCardInfo(newCardInfoBase, transform, ClientPlayer, false);
        newCardBase.M_CardInstanceId = cardInstanceId;
        cards.Add(newCardBase);
        RefreshCardsPlace();
    }

    public int GetCardIndex(CardBase card)
    {
        int index = -1;
        foreach (CardBase card_b in cards)
        {
            index++;
            if (card == card_b)
            {
                return index;
            }
        }

        return -1;
    }

    public void DropCard(int handCardInstanceId)
    {
        CardBase cardBase = GetCardByCardInstanceId(handCardInstanceId);
        cardBase.PoolRecycle();
        cards.Remove(cardBase);
        RefreshCardsPlace();
    }


    public void UseCard(int handCardInstanceId, CardInfo_Base cardInfo, Vector3 lastDragPosition)
    {
        CardBase cardBase = GetCardByCardInstanceId(handCardInstanceId);

        if (ClientPlayer == RoundManager.Instance.EnemyClientPlayer)
        {
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_UseCardShow(cardBase, cardInfo), "Co_UseCardShow");
        }
        else
        {
            cards.Remove(cardBase);
            cardBase.PoolRecycle();
            RefreshCardsPlace();
        }
    }

    private float lastUseCardShowTime;
    private float useCardShowIntervalMinimum = 1f;

    IEnumerator Co_UseCardShow(CardBase cardBase, CardInfo_Base cardInfo)
    {
        if (Time.time - lastUseCardShowTime < useCardShowIntervalMinimum) yield return new WaitForSeconds(useCardShowIntervalMinimum - (Time.time - lastUseCardShowTime));
        lastUseCardShowTime = Time.time;
        while (current_SubCo_ShowCardForTime != null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        current_SubCo_ShowCardForTime = SubCo_ShowCardForTime(cardBase, cardInfo, GameManager.Instance.ShowCardDuration);
        StartCoroutine(current_SubCo_ShowCardForTime);
        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private IEnumerator current_SubCo_ShowCardForTime;

    IEnumerator SubCo_ShowCardForTime(CardBase cardBase, CardInfo_Base cardInfo, float f)
    {
        cards.Remove(cardBase);
        Vector3 oldPosition = cardBase.transform.position;
        Quaternion oldRotation = cardBase.transform.rotation;
        Vector3 oldScale = cardBase.transform.localScale;
        Vector3 targetPosition = GameManager.Instance.UseCardShowPosition;
        Quaternion targetRotation = Quaternion.Euler(0, 180, 0);
        Vector3 targetScale = Vector3.one * GameManager.Instance.CardShowScale;
        cardBase.PoolRecycle();

        if (currentShowCard) lastShowCard = currentShowCard;
        currentShowCard = CardBase.InstantiateCardByCardInfo(cardInfo, transform, ClientPlayer, false);
        currentShowCard.DragComponent.enabled = false;

        currentShowCard.transform.position = oldPosition;
        currentShowCard.transform.rotation = oldRotation;
        currentShowCard.CanBecomeBigger = false;
        currentShowCard.Usable = false;

        currentShowCard.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#FFFFFF"));
        currentShowCard.CardBloom.SetActive(true);

        float duration = GameManager.Instance.ShowCardFlyDuration;
        float rotateDuration = GameManager.Instance.ShowCardRotateDuration;
        float tick = 0;
        float tickRotate = 0;
        while (true)
        {
            if (tick > duration && tickRotate > rotateDuration) break;

            tick += Time.deltaTime;
            if (tick < duration)
            {
                currentShowCard.transform.position = Vector3.Lerp(oldPosition, targetPosition, tick / duration);
                currentShowCard.transform.localScale = Vector3.Lerp(oldScale, targetScale, tick / duration);
            }

            tickRotate += Time.deltaTime;
            if (tickRotate < rotateDuration)
            {
                currentShowCard.transform.rotation = Quaternion.Slerp(oldRotation, targetRotation, tickRotate / rotateDuration);
            }

            yield return null;
        }

        if (lastShowCard) lastShowCard.PoolRecycle();

        currentShowCard.transform.position = targetPosition;
        currentShowCard.transform.rotation = targetRotation;
        currentShowCard.transform.localScale = targetScale;

        RefreshCardsPlace();
        yield return new WaitForSeconds(f);
        currentShowCard.PoolRecycle();
        current_SubCo_ShowCardForTime = null;
    }

    public void BeginRound()
    {
        foreach (var card in cards) card.OnBeginRound();
        RefreshAllCardUsable();
    }

    public void EndRound()
    {
        foreach (var card in cards) card.OnEndRound();
        SetAllCardUnusable();
    }

    #endregion

    #region 交互

    private void CheckMousePosition() //检查鼠标是否还停留在某张牌上，如果否，取消放大效果
    {
        if (!isBeginDrag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycast;
            Physics.Raycast(ray, out raycast, 10f, cardLayer);
            if (raycast.collider == null)
            {
                if (currentFocusCard)
                {
                    returnToSmaller(currentFocusCard);
                    currentFocusCard = null;
                }
            }
        }
    }

    Quaternion defaultCardRotation;
    bool isSet_defaultCardRotation;
    Vector3 defaultCardPosition;
    bool isSet_defaultCardPosition;

    internal void RefreshCardsPlace() //重置所有手牌位置
    {
        if (ClientPlayer == null) return;
        if (cards.Count == 0) return;
        if (!isSet_defaultCardRotation)
        {
            defaultCardRotation = cards[0].transform.rotation;
            isSet_defaultCardRotation = true;
        }

        if (!isSet_defaultCardPosition)
        {
            defaultCardPosition = cards[0].transform.position;
            isSet_defaultCardPosition = true;
        }

        float angle = anglesDict[cards.Count - 1] * GameManager.Instance.HandCardRotate;
        float horrizonDist = horrizonDistanceDict[cards.Count - 1] * GameManager.Instance.HandCardInterval;
        int count = 0;
        foreach (CardBase card in cards)
        {
            count++;
            card.transform.rotation = defaultCardRotation;
            card.transform.position = defaultCardPosition;
            card.transform.localScale = Vector3.one * GameManager.Instance.HandCardSize;
            float rotateAngle = angle / cards.Count * (((cards.Count - 1) / 2.0f + 1) - count);
            if (ClientPlayer.WhichPlayer == Players.Self)
            {
                //card.transform.Rotate(Vector3.up * 180);
            }
            else
            {
                //card.transform.Rotate(Vector3.forward * 180);
                card.transform.Rotate(Vector3.right * 180);
            }

            card.transform.position = new Vector3(card.transform.position.x, 2f, card.transform.position.z);
            float horrizonDistance = horrizonDist / cards.Count * (((cards.Count - 1) / 2.0f + 1) - count);
            card.transform.Translate(Vector3.right * horrizonDistance * GameManager.Instance.HandCardSize); //向水平向错开，体现手牌展开感
            float distCardsFromCenter = Mathf.Abs(((cards.Count - 1) / 2.0f + 1) - count); //与中心距离几张卡牌
            float factor = (cards.Count - distCardsFromCenter) / cards.Count; //某临时参数
            card.transform.Translate(-Vector3.back * 0.13f * distCardsFromCenter * (1 - factor * factor) * 0.5f * GameManager.Instance.HandCardSize + Vector3.back * cards.Count / 20 * GameManager.Instance.HandCardOffset); //向垂直向错开，体现卡片弧线感
            card.transform.Translate(Vector3.up * 0.1f * (cards.Count - count) * (ClientPlayer == RoundManager.Instance.EnemyClientPlayer ? -1 : 1)); //向上错开，体现卡片前后感
            card.transform.Rotate(Vector3.down, rotateAngle); //卡片微小旋转
            card.ResetColliderAndReplace();
        }

        RefreshAllCardUsable();
    }

    internal void RefreshAllCardUsable() //刷新所有卡牌是否可用
    {
        if (ClientPlayer == null) return;
        foreach (var card in cards)
        {
            if (ClientPlayer == RoundManager.Instance.CurrentClientPlayer)
            {
                card.Usable = (ClientPlayer == RoundManager.Instance.SelfClientPlayer) && (card.M_Cost <= ClientPlayer.CostLeft);
                if (card is CardRetinue) card.Usable &= !ClientPlayer.MyBattleGroundManager.BattleGroundIsFull;
            }
            else
            {
                card.Usable = false;
            }
        }
    }

    internal void SetAllCardUnusable() //禁用所有手牌
    {
        foreach (var card in cards) card.Usable = false;
    }

    CardBase currentFocusCard;
    CardBase currentFocusEquipmentCard;

    internal void CardOnMouseEnter(CardBase focusCard)
    {
        if (currentFocusCard)
        {
            returnToSmaller(currentFocusCard);
        }

        currentFocusCard = focusCard;
        becomeBigger(focusCard);
        if (ClientPlayer == RoundManager.Instance.SelfClientPlayer)
        {
            if (currentFocusCard is CardWeapon)
            {
                ClientPlayer.MyBattleGroundManager.ShowTipSlotBlooms(SlotTypes.Weapon);
                currentFocusEquipmentCard = currentFocusCard;
            }

            if (currentFocusCard is CardShield)
            {
                ClientPlayer.MyBattleGroundManager.ShowTipSlotBlooms(SlotTypes.Shield);
                currentFocusEquipmentCard = currentFocusCard;
            }
        }
    }

    internal void CardColliderReplaceOnMouseExit(CardBase lostFocusCard)
    {
        returnToSmaller(lostFocusCard);
        if (currentFocusEquipmentCard == lostFocusCard)
        {
            currentFocusEquipmentCard = null;
        }

        if (!Input.GetMouseButton(0)) ClientPlayer.MyBattleGroundManager.StopShowSlotBloom();
    }

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

    bool isBeginDrag = false;

    internal void BeginDrag()
    {
        isBeginDrag = true;
    }

    internal void EndDrag()
    {
        isBeginDrag = false;
    }

    //鼠标悬停手牌放大效果
    void becomeBigger(CardBase focusCard)
    {
        if (ClientPlayer == null) return;
        if (!isBeginDrag && ClientPlayer.WhichPlayer == Players.Self)
        {
            //用一个BoxCollider代替原来的位置
            ColliderReplace colliderReplace = GameObjectPoolManager.Instance.Pool_ColliderReplacePool.AllocateGameObject(GameBoardManager.Instance.transform).GetComponent<ColliderReplace>();
            colliderReplace.Initiate(focusCard);
            //本卡牌变大，旋转至正位
            focusCard.transform.localScale = Vector3.one * GameManager.Instance.PullOutCardSize;
            focusCard.transform.rotation = defaultCardRotation;
            if (ClientPlayer.WhichPlayer == Players.Self)
            {
                //focusCard.transform.Rotate(Vector3.up * 180);
                focusCard.transform.position = new Vector3(focusCard.transform.position.x, 2f, focusCard.transform.position.z);
                focusCard.transform.Translate(Vector3.up * 5f);
                focusCard.transform.Translate(Vector3.back * 3f);
                //本卡牌BoxCollider失效
                focusCard.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    void returnToSmaller(CardBase lostFocusCard)
    {
        if (ClientPlayer == null) return;
        if (!isBeginDrag && ClientPlayer.WhichPlayer == Players.Self)
        {
            //一旦替身BoxCollider失焦，恢复原手牌位置
            lostFocusCard.transform.localScale = Vector3.one;
            RefreshCardsPlace();
        }
    }

    #region 预召唤带有指定目标的随从

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