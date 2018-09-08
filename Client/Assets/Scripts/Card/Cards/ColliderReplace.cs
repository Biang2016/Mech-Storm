using UnityEngine;

/// <summary>
/// 手牌中预览卡牌时替代原卡牌位置的Collider
/// </summary>
public class ColliderReplace : MonoBehaviour
{
    internal CardBase MyCallerCard;

    internal void Initiate(CardBase callerCard)
    {
        MyCallerCard = callerCard;
        callerCard.myColliderReplace = this;
        transform.position = callerCard.transform.position;
        transform.rotation = callerCard.transform.rotation;
        transform.localScale = callerCard.transform.localScale;
        GetComponent<BoxCollider>().size = callerCard.GetComponent<BoxCollider>().size;
    }

    private void OnMouseExit()
    {
        MyCallerCard.ClientPlayer.MyHandManager.CardColliderReplaceOnMouseExit(MyCallerCard);
    }
}
