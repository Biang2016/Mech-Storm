/***********************************************************************
        filename:   MaskEffectUpdate.cs
        created:    2013.04.10
        author:     Twj

        purpose:    遮罩效果的更新
*************************************************************************/

using UnityEngine;
using System.Collections;

[System.Serializable]
public class MaskEffectUpdate
{
    // public variables
    
    // 更新遮罩效果
    // @params: mesh - 遮罩网格
    public virtual void UpdateEffect( Mesh mesh )
    {
        deltaTime = Time.time - startTime;  // 效果已播放的时间
    }
    /////////////////////////////////////////////////////////////////////

    // protected variables

    protected float startTime;          // 遮罩效果开始时间
    protected float totalTime;          // 遮罩效果总时间
    protected float deltaTime;          // 效果已播放的时间
    /////////////////////////////////////////////////////////////////////

    // public functions

    // 构造函数
    // @params: startTime - 遮罩效果开始时间
    public MaskEffectUpdate( float startTime, float totalTime )
    {
        this.startTime = startTime;
        this.totalTime = totalTime;
    }
    /////////////////////////////////////////////////////////////////////
}