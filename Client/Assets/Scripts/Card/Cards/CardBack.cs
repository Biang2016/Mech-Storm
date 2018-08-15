using System.Collections.Generic;
using UnityEngine;

internal class CardBack : CardBase
{
    protected override void Awake()
    {
        base.Awake();
        gameObjectPool = GameObjectPoolManager.GOPM.Pool_CardBackPool;
    }
}