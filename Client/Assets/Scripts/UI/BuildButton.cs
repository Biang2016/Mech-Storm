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
        Text_Count.text = BuildInfo.CardIDs.Length.ToString();
    }
}