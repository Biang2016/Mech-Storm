using UnityEngine;

/// <summary>
/// ������Ԥ������ʱ���ԭ����λ�õ�Collider
/// </summary>
public class ColliderReplace : PoolObject
{
    internal CardBase MyCallerCard;

    internal void Initiate(CardBase callerCard)
    {
        MyCallerCard = callerCard;
        callerCard.MyColliderReplace = this;
        transform.position = callerCard.transform.position;
        transform.rotation = callerCard.transform.rotation;
        transform.localScale = callerCard.transform.localScale;
        GetComponent<BoxCollider>().size = callerCard.GetComponent<BoxCollider>().size;
    }

    public void OnMouseExit()
    {
        MyCallerCard.ClientPlayer.BattlePlayer.HandManager.CardColliderReplaceOnMouseExit(MyCallerCard);
    }
}