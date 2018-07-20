/***********************************************************************
        filename:   RectMaskEffectUpdate.cs
        created:    2012.04.28
        author:     Twj

        purpose:    矩形遮罩效果的更新
*************************************************************************/

using UnityEngine;
using System.Collections;

[System.Serializable]
public class RectMaskEffectUpdate : MaskEffectUpdate
{
    // protected variables

    protected float halfHeight;         // 矩形宽的一半
    protected float halfWidth;          // 矩形高的一半
    /////////////////////////////////////////////////////////////////////

    // public functions

    // 构造函数
    // @params: startTime - 遮罩效果开始时间
    // @params: totalTime - 遮罩效果总时间
    // @params: halfHeight - 矩形宽的一半
    // @params: halfWidth - 矩形高的一半
    public RectMaskEffectUpdate(
            float startTime,
            float totalTime,
            float halfHeight,
            float halfWidth )
    : base( startTime, totalTime )
    {
        this.halfHeight = halfHeight;
        this.halfWidth = halfWidth;
    }
    /////////////////////////////////////////////////////////////////////
}