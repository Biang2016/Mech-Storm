using System.Collections.Generic;
using UnityEngine;

internal class CardBack : CardBase
{
    public override void PoolRecycle()
    {
        base.PoolRecycle();
    }

    protected override void Awake()
    {
        base.Awake();
        gameObjectPool = GameObjectPoolManager.GOPM.Pool_CardBackPool;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public override void Initiate(CardInfo_Base cardInfo, ClientPlayer clientPlayer, bool isCardSelect)
    {
        base.Initiate(cardInfo, clientPlayer, isCardSelect);
    }
}