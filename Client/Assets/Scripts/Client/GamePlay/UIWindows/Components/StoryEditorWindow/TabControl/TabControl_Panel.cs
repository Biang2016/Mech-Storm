using System.Collections.Generic;
using UnityEngine;

public class TabControl_Panel : PoolObject
{
    public Transform Container;

    private List<PoolObject> ContainerChildren = new List<PoolObject>();

    public override void PoolRecycle()
    {
        foreach (PoolObject po in ContainerChildren)
        {
            po.PoolRecycle();
        }

        ContainerChildren.Clear();
        base.PoolRecycle();
    }
}