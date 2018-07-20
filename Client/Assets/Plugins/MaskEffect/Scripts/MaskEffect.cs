/***********************************************************************
        filename:   MaskEffect.cs
        created:    2012.04.28
        author:     Twj

        purpose:    ����Ч������
*************************************************************************/

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public abstract class MaskEffect : MonoBehaviour
{
    // public variables

    public delegate void PreStartCallback();
    public delegate void EndCallback();

    // true�������ݽ��ڿ�ʼʱ���£�false��������ÿ֡�������
    public bool     isStatic = true;

    public Color    color = new Color( 0.0f, 0.0f, 0.0f, 0.5f );    // ���ֶ�����ɫ
    /////////////////////////////////////////////////////////////////////

    // protected variables

    protected struct Vertex // �������ݽṹ
    {
        public Vector3  pos;    // λ��
    }

    protected bool              playing;            // �Ƿ����ڲ�������Ч��
    protected float             startTime;          // ����Ч����ʼʱ��
    protected float             totalTime;          // ����Ч����ʱ��
    protected MeshFilter        meshFilter;
    protected MaskEffectUpdate  maskEffectUpdate;   // ����Ч���ĸ���

    protected PreStartCallback  preStartCallback;   // ���ֿ�ʼǰ�Ļص�����
    protected EndCallback       endCallback;        // ���ֽ����Ļص�����
    /////////////////////////////////////////////////////////////////////

    // public functions

    // ��ʼ����Ч��
    // @params: time - ����Ч����ʱ��
    public virtual void StartEffect( float time )
    {
        playing     = true;
        startTime   = Time.time;
        totalTime   = time;

        //
        if ( null != preStartCallback )
        {
            preStartCallback();
        }

        //
        GetComponent<Renderer>().enabled = true;
    }

    // ע�����ֿ�ʼǰ�Ļص�����
    public void RegisterPreStartCallback( PreStartCallback preStartCallback )
    {
        this.preStartCallback = preStartCallback;
    }

    // ע�����ֽ����Ļص�����
    public void RegisterEndCallback( EndCallback endCallback )
    {
        this.endCallback = endCallback;
    }
    /////////////////////////////////////////////////////////////////////

    // protected functions

    protected virtual void Awake()
    {
        InitMesh();

        //
        GetComponent<Renderer>().castShadows = false;
        GetComponent<Renderer>().receiveShadows = false;
    }

    protected virtual void Update()
    {
        if ( !isStatic )
        {
            InitMeshColors( meshFilter.sharedMesh );
        }

        //
        if ( playing )
        {
            float deltaTime = Time.time - startTime;    // Ч���Ѳ��ŵ�ʱ��
            if ( deltaTime > totalTime )
            {
                playing = false;

                //
                if ( null != endCallback )
                {
                    endCallback();
                }

                //
                GetComponent<Renderer>().enabled = false;
            }
            else
            {
                UpdateEffect();
            }
        }
    }

    // ��ʼ��Mesh
    // @return: true��ʼ��Mesh�ɹ���false��ʼ��Meshʧ��
    protected virtual bool InitMesh()
    {
        meshFilter = GetComponent<MeshFilter>();

        meshFilter.sharedMesh = new Mesh();
        InitMeshVertices( meshFilter.sharedMesh );
        InitMeshIndices( meshFilter.sharedMesh );
        InitMeshColors( meshFilter.sharedMesh );

        return true;
    }

    // ��ʼ��Mesh����
    // @return: true��ʼ��Mesh����ɹ���false��ʼ��Mesh����ʧ��
    protected virtual bool InitMeshVertices( Mesh mesh )
    {
        return true;
    }

    // ��ʼ��Mesh����
    // @return: true��ʼ��Mesh�����ɹ���false��ʼ��Mesh����ʧ��
    protected virtual bool InitMeshIndices( Mesh mesh )
    {
        return true;
    }

    // ��ʼ��Mesh��ɫ
    // @return: true��ʼ��Mesh��ɫ�ɹ���false��ʼ��Mesh��ɫʧ��
    protected virtual bool InitMeshColors( Mesh mesh )
    {
        Color[] colors = new Color[mesh.vertices.Length];
        for ( int i = 0; i < colors.Length; ++i )
        {
            colors[i] = color;
        }

        mesh.colors = colors;

        return true;
    }

    // ��������Ч��
    protected virtual void UpdateEffect()
    {
        if ( null != maskEffectUpdate )
        {
            maskEffectUpdate.UpdateEffect( meshFilter.sharedMesh );
        }
    }

    // ��������Ч�����¶���
    protected abstract void GenMaskEffectUpdate();
    /////////////////////////////////////////////////////////////////////
}