using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 代表已选卡片在右侧卡组中的按钮
/// </summary>
public class SelectCard : MonoBehaviour, IGameObjectPool
{
    private GameObjectPool gameObjectPool;

    public void PoolRecycle()
    {
        Count = 0;
        gameObjectPool.RecycleGameObject(gameObject);
    }

    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.Pool_SelectCardPool;
    }

    public Text Text_Cost;
    public Text Text_Count;
    public Button CardButton;
    public Text Text_CardName;

    private int count;

    public int Count
    {
        get { return count; }

        set
        {
            count = value;
            Text_Count.text = "×" + count.ToString();
        }
    }

    private int cost;

    public int Cost
    {
        get { return cost; }

        set
        {
            cost = value;
            Text_Cost.text = cost.ToString();
        }
    }
}