using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    internal Player Player;

    float[] anglesDict;//每张牌之间的夹角
    float[] horrizonDistanceDict;//每张牌之间的距离
    public int cardNumber = 0;//手牌数
    List<CardBase> cards;

    int retinueLayer;
    int cardLayer;

    public float HandCardSize = 1.0f;
    private void Awake()
    {
        cards = new List<CardBase>();
        anglesDict = new float[10] { 20f, 20f, 30f, 40f, 45f, 50f, 55f, 60f, 60f, 60f };//手牌分布角度预设值
        horrizonDistanceDict = new float[10] { 1.5f, 1.6f, 1.8f, 2.3f, 2.8f, 3.3f, 4.2f, 4.9f, 5.6f, 6.3f };//手牌分布距离预设值
        retinueLayer = 1 << LayerMask.NameToLayer("Retinues");
        cardLayer = 1 << LayerMask.NameToLayer("Cards");
    }

    void Start()
    {

    }

    void Update()
    {
        CheckMousePosition();
    }

    private void CheckMousePosition()//检查鼠标是否还停留在某张牌上，如果否，取消放大效果
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
                }
            }
        }
    }

    internal void DrawCards(int number)
    {
        for (int i = 0; i < number; i++)
        {
            DrawCard();
        }
    }

    internal void DrawCard()
    {
        if (cardNumber >= 10)
        {
            //无法抽牌
        }
        else
        {
            CardInfo_Base newCardInfo = Player.MyCardDeckManager.DrawTop();
            if (newCardInfo == null) {
                Debug.Log("No Card");
                return;
            }
            CardBase newCard = CardBase.InstantiateCardByCardInfo(newCardInfo, transform, Player);
            cards.Add(newCard);
            cardNumber++;
            RefreshCardsPlace();
        }
    }

    internal void GetACardByID(int cardID)
    {
        CardInfo_Base cardInfo = GameManager.GM.AllCard.GetCard(cardID);
        CardBase newCard = CardBase.InstantiateCardByCardInfo(cardInfo, transform, Player);
        cards.Add(newCard);
        cardNumber++;
        RefreshCardsPlace();
    }

    internal void DrawRetinueCard()
    {
        CardInfo_Base newCardInfo = Player.MyCardDeckManager.DrawRetinueCard();
        if (newCardInfo == null) {
            Debug.Log("No Card");
            return;
        }
        CardBase newCard = CardBase.InstantiateCardByCardInfo(newCardInfo, transform, Player);
        cards.Add(newCard);
        cardNumber++;
        RefreshCardsPlace();
    }

    internal void DropFirstCard()
    {
        DropCard(cards[0]);
    }

    internal void DropCard(CardBase dropCard)
    {
        cards.Remove(dropCard);
        cardNumber--;
        RefreshCardsPlace();
    }



    Quaternion defaultCardRotation;
    bool isSet_defaultCardRotation;
    Vector3 defaultCardPosition;
    bool isSet_defaultCardPosition;

    internal void RefreshCardsPlace()//重置所有手牌位置
    {
        if (cardNumber == 0) return;
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

        float angle = anglesDict[cardNumber - 1];
        float horrizonDist = horrizonDistanceDict[cardNumber - 1];
        int count = 0;
        foreach (CardBase card in cards)
        {
            count++;
            card.transform.rotation = defaultCardRotation;
            card.transform.position = defaultCardPosition;
            card.transform.localScale = Vector3.one * HandCardSize;
            float rotateAngle = angle / cardNumber * (((cardNumber - 1) / 2.0f + 1) - count);
            if (Player.WhichPlayer == Players.Self)
            {
                //card.transform.Rotate(Vector3.up * 180);
                card.transform.position = new Vector3(card.transform.position.x, 2f, card.transform.position.z);
                float horrizonDistance = horrizonDist / cardNumber * (((cardNumber - 1) / 2.0f + 1) - count);
                card.transform.Translate(Vector3.right * horrizonDistance* HandCardSize);//向水平向错开，体现手牌展开感
                card.transform.Translate(-Vector3.back * 0.13f * Mathf.Abs(((cardNumber - 1) / 2.0f + 1) - count) * HandCardSize);//向垂直向错开，体现卡片弧线感
                card.transform.Translate(Vector3.up * 0.1f * (cardNumber-count));//向上错开，体现卡片前后感
                card.transform.Rotate(Vector3.down, rotateAngle);//卡片微小旋转
            }
            else
            {
                if (GameManager.GM.CanTestEnemyCards)
                {
                    card.transform.Rotate(Vector3.up * 180);
                    card.transform.position = new Vector3(card.transform.position.x, 2f, card.transform.position.z);
                    float horrizonDistance = horrizonDist / cardNumber * (((cardNumber - 1) / 2.0f + 1) - count);
                    card.transform.Translate(Vector3.right * horrizonDistance * HandCardSize);//向水平向错开，体现手牌展开感
                    card.transform.Translate(-Vector3.back * 0.13f * Mathf.Abs(((cardNumber - 1) / 2.0f + 1) - count) * HandCardSize);//向垂直向错开，体现卡片弧线感
                    card.transform.Translate(Vector3.up * 0.1f * (cardNumber - count));//向上错开，体现卡片前后感
                    card.transform.Rotate(Vector3.down, rotateAngle);//卡片微小旋转
                }
                else
                {
                    //card.transform.Rotate(Vector3.up * 180);
                    card.transform.Rotate(Vector3.forward * 180);
                    card.transform.position = new Vector3(card.transform.position.x, 2f, card.transform.position.z);
                    float horrizonDistance = horrizonDist / cardNumber * (((cardNumber - 1) / 2.0f + 1) - count);
                    card.transform.Translate(-Vector3.right * horrizonDistance * HandCardSize);//向水平向错开，体现手牌展开感
                    card.transform.Translate(-Vector3.back * 0.13f * Mathf.Abs(((cardNumber - 1) / 2.0f + 1) - count) * HandCardSize);//向垂直向错开，体现卡片弧线感
                    card.transform.Translate(-Vector3.up * 0.1f * (cardNumber - count));//向上错开，体现卡片前后感
                    card.transform.Rotate(-Vector3.down, rotateAngle);//卡片微小旋转
                }
            }
            card.ResetColliderAndReplace();
        }
    }

    internal void RefreshAllCardUsable() //刷新所有卡牌是否可用
    {
        foreach (var card in cards) card.Usable = card.M_Cost <= Player.CostLeft;
    }

    CardBase currentFocusCard;
    internal void CardOnMouseEnter(CardBase focusCard)
    {
        if (currentFocusCard)
        {
            returnToSmaller(currentFocusCard);
        }
        currentFocusCard = focusCard;
        becomeBigger(focusCard);
    }

    internal void CardColliderReplaceOnMouseExit(CardBase lostFocusCard)
    {
        returnToSmaller(lostFocusCard);
        if (currentFocusCard == lostFocusCard)
        {
            currentFocusCard = null;
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

    public float PullOutCardSize = 3.0f;
    //鼠标悬停手牌放大效果
    void becomeBigger(CardBase focusCard)
    {
        if (!isBeginDrag && (GameManager.GM.CanTestEnemyCards || Player.WhichPlayer == Players.Self))
        {//用一个BoxCollider代替原来的位置
            ColliderReplace colliderReplace = GameObjectPoolManager.GOPM.Pool_ColliderReplacePool.AllocateGameObject(GameBoardManager.GBM.SelfHandArea.transform).GetComponent<ColliderReplace>();
            colliderReplace.Initiate(focusCard);
            //本卡牌变大，旋转至正位
            focusCard.transform.localScale = Vector3.one * PullOutCardSize;
            focusCard.transform.rotation = defaultCardRotation;
            if (Player.WhichPlayer == Players.Self)
            {
                //focusCard.transform.Rotate(Vector3.up * 180);
                focusCard.transform.position = new Vector3(focusCard.transform.position.x, 2f, focusCard.transform.position.z);
                focusCard.transform.Translate(Vector3.up * 5f);
                focusCard.transform.Translate(Vector3.back * 3f);
                //本卡牌BoxCollider失效
                focusCard.GetComponent<BoxCollider>().enabled = false;
            }
            else
            {
                if (GameManager.GM.CanTestEnemyCards)
                {
                    //focusCard.transform.Rotate(Vector3.up * 180);
                    focusCard.transform.position = new Vector3(focusCard.transform.position.x, 2f, focusCard.transform.position.z);
                    focusCard.transform.Translate(Vector3.up * 5f);
                    focusCard.transform.Translate(Vector3.forward * 3f);
                    //本卡牌BoxCollider失效
                    focusCard.GetComponent<BoxCollider>().enabled = false;
                }
                else
                {
                    
                }
            }
            
        }
    }

    void returnToSmaller(CardBase lostFocusCard)
    {
        if (!isBeginDrag && (GameManager.GM.CanTestEnemyCards || Player.WhichPlayer == Players.Self))
        {
            //一旦替身BoxCollider失焦，恢复原手牌位置
            lostFocusCard.transform.localScale = Vector3.one;
            RefreshCardsPlace();
        }
    }

    public void BeginRound()
    {
        foreach (var card in cards) card.OnBeginRound();
    }

    public void EndRound()
    {
        foreach (var card in cards) card.OnEndRound();
    }

}
