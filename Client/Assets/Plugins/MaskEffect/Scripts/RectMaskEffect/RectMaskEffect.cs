/***********************************************************************
        filename:   RectMaskEffect.cs
        created:    2012.04.28
        author:     Twj

        purpose:    矩形遮罩效果
                    代码中将使用10个顶点来渲染遮罩，索引如下：
                    8----1/9----2
                    |     |     |
                    7-----0-----3
                    |     |     |
                    6-----5-----4
*************************************************************************/

using UnityEngine;
using System.Collections;

public class RectMaskEffect : MaskEffect
{
    // public variables
    public enum TYPE    // 类型
    {
        NONE,       // 无
        CIRCLE,     // 圆周
        L_TO_R,     // 从左到右
        R_TO_L,     // 从右到左
        T_TO_B,     // 从上到下
        B_TO_T,     // 从下到上
        LT_TO_RB,   // 从左上到右下
        RB_TO_LT,   // 从右下到左上
        LB_TO_RT,   // 从左下到右上
        RT_TO_LB,   // 从右上到左下
    }
    public TYPE type = TYPE.NONE;

    public float                halfHeight = 0.5f;      // 矩形宽的一半
    public float                halfWidth = 0.5f;       // 矩形高的一半
    /////////////////////////////////////////////////////////////////////

    // private variables

    private const int   VERTEX_COUNT = 10;  // 渲染遮罩的顶点数量
    private Vertex[]    arrVertices = new Vertex[VERTEX_COUNT];
    /////////////////////////////////////////////////////////////////////

    // public functions

    public override void StartEffect( float time )
    {
        base.StartEffect( time );

        //
        SetMeshVertices( meshFilter.sharedMesh );
        GenMaskEffectUpdate();
    }
    /////////////////////////////////////////////////////////////////////

    // protected functions

    protected override void Awake()
    {
        base.Awake();

        StartEffect( 5.0f );
    }

    // 初始化Mesh顶点
    // @return: true初始化Mesh顶点成功，false初始化Mesh顶点失败
    protected override bool InitMeshVertices( Mesh mesh )
    {
        ResetVertices();

        //
        SetMeshVertices( mesh );

        return true;
    }

    // 初始化Mesh索引
    // @return: true初始化Mesh索引成功，false初始化Mesh索引失败
    protected override bool InitMeshIndices( Mesh mesh )
    {
        int[] indices = new int[]
                        {
                            0, 1, 2,
                            0, 2, 3,
                            0, 3, 4,
                            0, 4, 5,
                            0, 5, 6,
                            0, 6, 7,
                            0, 7, 8,
                            0, 8, 9,
                        };
    
        mesh.triangles = indices;

        return true;
    }

    // 生成遮罩效果更新对象
    protected override void GenMaskEffectUpdate()
    {
        switch ( type )
        {
        case TYPE.CIRCLE:
            {
                maskEffectUpdate = new RectMaskEffectUpdateCircle(
                                            startTime,
                                            totalTime,
                                            halfHeight,
                                            halfWidth );
            }
            break;

        case TYPE.L_TO_R:
            {
                maskEffectUpdate = new RectMaskEffectUpdateLToR(
                                            startTime,
                                            totalTime,
                                            halfHeight,
                                            halfWidth );
            }
            break;

        case TYPE.R_TO_L:
            {
                maskEffectUpdate = new RectMaskEffectUpdateRToL(
                                            startTime,
                                            totalTime,
                                            halfHeight,
                                            halfWidth );
            }
            break;

        case TYPE.T_TO_B:
            {
                maskEffectUpdate = new RectMaskEffectUpdateTToB(
                                            startTime,
                                            totalTime,
                                            halfHeight,
                                            halfWidth );
            }
            break;

        case TYPE.B_TO_T:
            {
                maskEffectUpdate = new RectMaskEffectUpdateBToT(
                                            startTime,
                                            totalTime,
                                            halfHeight,
                                            halfWidth );
            }
            break;

        case TYPE.LT_TO_RB:
            {
                maskEffectUpdate = new RectMaskEffectUpdateLTToRB(
                                            startTime,
                                            totalTime,
                                            halfHeight,
                                            halfWidth,
                                            meshFilter.sharedMesh );
            }
            break;

        case TYPE.RB_TO_LT:
            {
                maskEffectUpdate = new RectMaskEffectUpdateRBToLT(
                                            startTime,
                                            totalTime,
                                            halfHeight,
                                            halfWidth,
                                            meshFilter.sharedMesh );
            }
            break;

        case TYPE.LB_TO_RT:
            {
                maskEffectUpdate = new RectMaskEffectUpdateLBToRT(
                                            startTime,
                                            totalTime,
                                            halfHeight,
                                            halfWidth,
                                            meshFilter.sharedMesh );
            }
            break;

        case TYPE.RT_TO_LB:
            {
                maskEffectUpdate = new RectMaskEffectUpdateRTToLB(
                                            startTime,
                                            totalTime,
                                            halfHeight,
                                            halfWidth,
                                            meshFilter.sharedMesh );
            }
            break;

        default:
            maskEffectUpdate = null;
            break;
        }
    }
    /////////////////////////////////////////////////////////////////////

    // private functions

    // 重置顶点数据
    private void ResetVertices()
    {
        arrVertices[0].pos = new Vector3( 0.0f, 0.0f, 0.0f );
        arrVertices[1].pos = new Vector3( 0.0f, halfHeight, 0.0f );
        arrVertices[2].pos = new Vector3( halfWidth, halfHeight, 0.0f );
        arrVertices[3].pos = new Vector3( halfWidth, 0.0f, 0.0f );
        arrVertices[4].pos = new Vector3( halfWidth, -halfHeight, 0.0f );
        arrVertices[5].pos = new Vector3( 0.0f, -halfHeight, 0.0f );
        arrVertices[6].pos = new Vector3( -halfWidth, -halfHeight, 0.0f );
        arrVertices[7].pos = new Vector3( -halfWidth, 0.0f, 0.0f );
        arrVertices[8].pos = new Vector3( -halfWidth, halfHeight, 0.0f );
        arrVertices[9].pos = new Vector3( 0.0f, halfHeight, 0.0f );
    }

    // 设置Mesh顶点数据
    private void SetMeshVertices( Mesh mesh )
    {
        Vector3[] vertices = new Vector3[arrVertices.Length];
        for ( int i = 0; i < arrVertices.Length; ++i )
        {
            vertices[i] = arrVertices[i].pos;
        }

        mesh.vertices = vertices;
    }
    /////////////////////////////////////////////////////////////////////
}