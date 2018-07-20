/***********************************************************************
        filename:   RectMaskEffectUpdate.cs
        created:    2012.04.28
        author:     Twj

        purpose:    ��������Ч���ĸ���
*************************************************************************/

using UnityEngine;
using System.Collections;

[System.Serializable]
public class RectMaskEffectUpdate : MaskEffectUpdate
{
    // protected variables

    protected float halfHeight;         // ���ο��һ��
    protected float halfWidth;          // ���θߵ�һ��
    /////////////////////////////////////////////////////////////////////

    // public functions

    // ���캯��
    // @params: startTime - ����Ч����ʼʱ��
    // @params: totalTime - ����Ч����ʱ��
    // @params: halfHeight - ���ο��һ��
    // @params: halfWidth - ���θߵ�һ��
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