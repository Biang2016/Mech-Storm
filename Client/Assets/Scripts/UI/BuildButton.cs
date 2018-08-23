using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 代表已选卡片在右侧卡组中的按钮
/// </summary>
public class BuildButton : MonoBehaviour, IGameObjectPool
{
    private GameObjectPool gameObjectPool;

    public void PoolRecycle()
    {
        gameObjectPool.RecycleGameObject(gameObject);
    }

    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.Pool_BuildButtonPool;
    }

    public Button Button;
    public Text Text_CardDeckName;
    public Text Text_Count;

    public BuildInfo BuildInfo;

    public void Initialize(BuildInfo buildInfo)
    {
        BuildInfo = buildInfo;
        Text_CardDeckName.text = BuildInfo.BuildName;
        Text_Count.text = BuildInfo.CardIDs.Count.ToString();
    }

    public void AddHeroCard(int cardId)
    {
        BuildInfo.BeginRetinueIDs.Add(cardId);
        Text_Count.text = BuildInfo.CardCount().ToString();
    }

    public void RemoveHeroCard(int cardId)
    {
        BuildInfo.BeginRetinueIDs.Remove(cardId);
        Text_Count.text = BuildInfo.CardCount().ToString();
    }

    public void AddCard(int cardId)
    {
        BuildInfo.CardIDs.Add(cardId);
        Text_Count.text = BuildInfo.CardCount().ToString();
    }

    public void RemoveCard(int cardId)
    {
        BuildInfo.CardIDs.Remove(cardId);
        Text_Count.text = BuildInfo.CardCount().ToString();
    }
}