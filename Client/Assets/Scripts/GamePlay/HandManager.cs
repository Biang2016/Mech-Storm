using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    internal ClientPlayer ClientPlayer;

    float[] anglesDict; //每张牌之间的夹角
    float[] horrizonDistanceDict; //每张牌之间的距离
    public List<CardBase> cards;
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
        UpdateCurrentFocusCardTicker();
    }

    #region 响应

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
        lastUseCardShowTime = 0;
        summonRetinuePreviewCardIndex = 0;
    }

    public void GetCards(List<DrawCardRequest.CardIdAndInstanceId> cardIdAndInstanceIds)
    {
        if (ClientPlayer == null) return;
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_GetCards(cardIdAndInstanceIds), "Co_GetCard");
    }

    [SerializeField] private Transform DrawCardPivot;

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

    IEnumerator SubCo_GetCard(int indexNumber, int totalCardNumber, DrawCardRequest.CardIdAndInstanceId cardIdAndInstanceId, float duration) //单卡片抽取动画
    {
        Transform srcPos = DrawCardPivot;
        Transform tarTran = GetCardPlace(indexNumber, totalCardNumber);
        Vector3 tarPos = tarTran.position;
        Quaternion tarRot = tarTran.rotation;

        Hashtable arg = new Hashtable();
        arg.Add("position", tarPos);
        arg.Add("time", duration);
        arg.Add("rotation", tarRot.eulerAngles);

        CardInfo_Base newCardInfoBase = AllCards.GetCard(cardIdAndInstanceId.CardId);
        CardBase newCardBase;

        newCardBase = CardBase.InstantiateCardByCardInfo(newCardInfoBase, transform, ClientPlayer, false);
        newCardBase.IsFlying = true;
        cards.Add(newCardBase);
        newCardBase.myCollider.enabled = false;
        newCardBase.transform.position = srcPos.position;
        newCardBase.transform.rotation = srcPos.rotation;
        newCardBase.transform.localScale = Vector3.one * GameManager.Instance.HandCardSize;
        AudioManager.Instance.SoundPlay("sfx/DrawCard0");
        iTween.MoveTo(newCardBase.gameObject, arg);
        iTween.RotateTo(newCardBase.gameObject, arg);
        yield return new WaitForSeconds(duration);

        newCardBase.M_CardInstanceId = cardIdAndInstanceId.CardInstanceId;
        newCardBase.myCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        newCardBase.IsFlying = false;
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
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_UseCard(cardBase, cardInfo), "Co_UseCardShow");
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

    IEnumerator Co_UseCard(CardBase cardBase, CardInfo_Base cardInfo)
    {
        if (Time.time - lastUseCardShowTime < useCardShowIntervalMinimum) yield return new WaitForSeconds(useCardShowIntervalMinimum - (Time.time - lastUseCardShowTime));
        lastUseCardShowTime = Time.time;
        while (current_SubCo_ShowCardForTime != null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        cardBase.OnPlayOut();

        current_SubCo_ShowCardForTime = SubCo_ShowCardForTime(cardBase, cardInfo, GameManager.Instance.ShowCardDuration);
        StartCoroutine(current_SubCo_ShowCardForTime);
        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private IEnumerator current_SubCo_ShowCardForTime;

    IEnumerator SubCo_ShowCardForTime(CardBase cardBase, CardInfo_Base cardInfo, float showCardDuration)
    {
        cards.Remove(cardBase);
        Vector3 oldPosition = cardBase.transform.position;
        Quaternion oldRotation = cardBase.transform.rotation;
        Vector3 oldScale = cardBase.transform.localScale;
        Vector3 targetPosition;
        if (lastShowCard) targetPosition = GameManager.Instance.UseCardShowOverlayPosition;
        else targetPosition = GameManager.Instance.UseCardShowPosition;
        Quaternion targetRotation = Quaternion.Euler(0, 180, 0);
        Vector3 targetScale = Vector3.one * GameManager.Instance.CardShowScale;
        cardBase.PoolRecycle();

        if (currentShowCard) lastShowCard = currentShowCard;
        currentShowCard = CardBase.InstantiateCardByCardInfo(cardInfo, transform, ClientPlayer, false);
        currentShowCard.DragComponent.enabled = false;
        currentShowCard.CanBecomeBigger = false;
        currentShowCard.Usable = false;
        currentShowCard.ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#FFFFFF"));
        currentShowCard.CardBloom.SetActive(true);
        currentShowCard.BeBrightColor();

        float duration = GameManager.Instance.ShowCardFlyDuration;
        float rotateDuration = GameManager.Instance.ShowCardRotateDuration;

        yield return ClientUtils.MoveGameObject(currentShowCard.transform, oldPosition, oldRotation, oldScale, targetPosition, targetRotation, targetScale, duration, rotateDuration);

        RefreshCardsPlace();
        yield return new WaitForSeconds(showCardDuration);
        currentShowCard.PoolRecycle();
        current_SubCo_ShowCardForTime = null;
    }

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
                if (CurrentFocusCard)
                {
                    returnToSmaller(CurrentFocusCard);
                    CurrentFocusCard = null;
                }
            }
        }
    }

    [SerializeField] private Transform DefaultCardPivot;

    internal void RefreshCardsPlace() //重置所有手牌位置
    {
        RefreshCardsPlace(cards.Count, 0.1f);
    }

    internal void RefreshCardsPlace(float duration) //重置所有手牌位置
    {
        RefreshCardsPlace(cards.Count, duration);
    }

    internal void RefreshCardsPlace(int cardCount, float duration) //按虚拟的手牌总数重置所有手牌位置
    {
        if (CurrentFocusCard)
        {
            returnToSmaller(CurrentFocusCard);
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
    }

    internal void RefreshCardsPlaceImmediately() //瞬间重置
    {
        RefreshCardsPlaceImmediately(cards.Count);
    }

    internal void RefreshCardsPlaceImmediately(int cardCount) //瞬间重置
    {
        if (CurrentFocusCard)
        {
            returnToSmaller(CurrentFocusCard);
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
            Quaternion rotation = result.rotation;
            Vector3 scale = result.localScale;

            card.transform.position = position;
            card.transform.rotation = rotation;
            card.transform.localScale = scale;
        }

        RefreshAllCardUsable();
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

        float angle = anglesDict[toatalCardNumber - 1] * GameManager.Instance.HandCardRotate;
        float horrizonDist = horrizonDistanceDict[toatalCardNumber - 1] * GameManager.Instance.HandCardInterval;
        float rotateAngle = angle / toatalCardNumber * (((toatalCardNumber - 1) / 2.0f + 1) - cardIndex);
        if (ClientPlayer.WhichPlayer == Players.Self)
        {
        }
        else
        {
            GetCardPlacePivot.transform.Rotate(-Vector3.right * 180);
        }

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

    internal void SetAllCardUnusable() //禁用所有手牌
    {
        foreach (CardBase card in cards) card.Usable = false;
    }

    public CardBase CurrentFocusCard;
    CardBase currentFocusEquipmentCard;

    internal void CardOnMouseEnter(CardBase focusCard)
    {
        if (isBeginDrag) return;
        if (CurrentFocusCard == focusCard) return;
        if (CurrentFocusCard)
        {
            returnToSmaller(CurrentFocusCard);
            RefreshCardsPlaceImmediately();
            ClientPlayer.MyMetalLifeEnergyManager.MetalBarManager.ResetHightlightTopBlocks();
        }

        CurrentFocusCard = focusCard;
        becomeBigger(focusCard);
        if (ClientPlayer == RoundManager.Instance.SelfClientPlayer)
        {
            if (CurrentFocusCard is CardEquip)
            {
                ClientPlayer.MyBattleGroundManager.ShowTipSlotBlooms((CardEquip) CurrentFocusCard);
                currentFocusEquipmentCard = CurrentFocusCard;
            }

            ClientPlayer.MyMetalLifeEnergyManager.MetalBarManager.HightlightTopBlocks(focusCard.CardInfo.BaseInfo.Metal);
        }

        if (focusCard is CardSpell && ((CardSpell) focusCard).HasTargetEquip)
        {
            RoundManager.Instance.SelfClientPlayer.MyBattleGroundManager.ShowTipModuleBloomSE(0.3f);
            RoundManager.Instance.EnemyClientPlayer.MyBattleGroundManager.ShowTipModuleBloomSE(0.3f);
        }

        currentFocusCardTickerBegin = true;
    }

    internal void CardOnMouseLeave(CardBase focusCard)
    {
        if (isBeginDrag) return;
        RefreshCardsPlaceImmediately();
        ClientPlayer.MyMetalLifeEnergyManager.MetalBarManager.ResetHightlightTopBlocks();
    }

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
                if (CurrentFocusCard) AffixManager.Instance.ShowAffixTips(new List<CardInfo_Base> {CurrentFocusCard.CardInfo});
                currentFocusCardTickerBegin = false;
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

        ClientPlayer.MyMetalLifeEnergyManager.MetalBarManager.ResetHightlightTopBlocks();
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
            ColliderReplace colliderReplace = GameObjectPoolManager.Instance.Pool_ColliderReplacePool.AllocateGameObject<ColliderReplace>(GameBoardManager.Instance.transform);
            colliderReplace.Initiate(focusCard);
            //本卡牌变大，旋转至正位
            focusCard.transform.localScale = Vector3.one * GameManager.Instance.PullOutCardSize;
            focusCard.transform.rotation = DefaultCardPivot.rotation;
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
            lostFocusCard.transform.localScale = Vector3.one * GameManager.Instance.HandCardSize;
            if (lostFocusCard.myColliderReplace)
            {
                lostFocusCard.transform.position = lostFocusCard.myColliderReplace.transform.position;
                lostFocusCard.transform.rotation = lostFocusCard.myColliderReplace.transform.rotation;
            }

            lostFocusCard.ResetColliderAndReplace();
            AffixManager.Instance.HideAffixPanel();
            currentFocusCardTickerBegin = false;
        }
    }

    #region 预召唤带有指定目标的机甲

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