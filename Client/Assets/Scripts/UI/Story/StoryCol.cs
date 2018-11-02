using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryCol : PoolObject
{
    public override void PoolRecycle()
    {
        base.PoolRecycle();
    }

    void Awake()
    {
    }

    internal List<StoryLevelButton> StoryLevelButtons = new List<StoryLevelButton>();

    public void Initialize()
    {
        StoryLevelButton slb = GameObjectPoolManager.Instance.Pool_StoryLevelButtonPool.AllocateGameObject<StoryLevelButton>(transform);
        slb.Initialize(ClientUtils.GetColorFromColorDict(AllColors.ColorType.StoryLevelColor5), 0);
        StoryLevelButton slb1 = GameObjectPoolManager.Instance.Pool_StoryLevelButtonPool.AllocateGameObject<StoryLevelButton>(transform);
        slb1.Initialize(ClientUtils.GetColorFromColorDict(AllColors.ColorType.StoryLevelColor6), 1);
        StoryLevelButton slb2 = GameObjectPoolManager.Instance.Pool_StoryLevelButtonPool.AllocateGameObject<StoryLevelButton>(transform);
        slb2.Initialize(ClientUtils.GetColorFromColorDict(AllColors.ColorType.StoryLevelColor7), 2);
    }
}