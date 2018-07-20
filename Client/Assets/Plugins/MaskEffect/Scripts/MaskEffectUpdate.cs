/***********************************************************************
        filename:   MaskEffectUpdate.cs
        created:    2013.04.10
        author:     Twj

        purpose:    ����Ч���ĸ���
*************************************************************************/

using UnityEngine;
using System.Collections;

[System.Serializable]
public class MaskEffectUpdate
{
    // public variables
    
    // ��������Ч��
    // @params: mesh - ��������
    public virtual void UpdateEffect( Mesh mesh )
    {
        deltaTime = Time.time - startTime;  // Ч���Ѳ��ŵ�ʱ��
    }
    /////////////////////////////////////////////////////////////////////

    // protected variables

    protected float startTime;          // ����Ч����ʼʱ��
    protected float totalTime;          // ����Ч����ʱ��
    protected float deltaTime;          // Ч���Ѳ��ŵ�ʱ��
    /////////////////////////////////////////////////////////////////////

    // public functions

    // ���캯��
    // @params: startTime - ����Ч����ʼʱ��
    public MaskEffectUpdate( float startTime, float totalTime )
    {
        this.startTime = startTime;
        this.totalTime = totalTime;
    }
    /////////////////////////////////////////////////////////////////////
}