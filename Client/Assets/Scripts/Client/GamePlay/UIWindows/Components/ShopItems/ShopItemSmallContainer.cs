using UnityEngine;
using System.Collections;

public class ShopItemSmallContainer : PoolObject
{
    public Transform Container;

    internal int ItemCount = 0;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        ItemCount = 0;
    }

    public const int Capacity = 4;
}