using UnityEngine;
using System.Collections;
using System;

public class Arrow : MonoBehaviour, IGameObjectPool
{

    //Todo ”–ø’’˚¿Ì
    public void PoolRecycle()
    {
        GameObjectPoolManager.GOPM.Pool_ArrowArrowPool.RecycleGameObject(gameObject);
    }

    public GameObject ArrowBody;

    Animator anim;

    void Awake()
    {
        defaultRotation = transform.rotation;
        defaultPosition_ArrowBody = ArrowBody.transform.localPosition;
        defaultRotation_ArrowBody = ArrowBody.transform.localRotation;
        if (M_ArrowType == ArrowType.Arrow)
        {
        }
        else if (M_ArrowType == ArrowType.Aiming)
        {
            defaultRotation_Control = ControlDirectionGo.localRotation;
            anim = GetComponent<Animator>();
            anim.SetBool("IsOnValidTarget", true);
        }
    }

    public ArrowType M_ArrowType;
    Quaternion defaultRotation;
    Vector3 defaultPosition_ArrowBody;
    Quaternion defaultRotation_ArrowBody;
    Quaternion defaultRotation_Control;

    public Transform ControlDirectionGo;


    public void Initiate(Vector3 StartPosition, Vector3 EndPosition)
    {
        if (M_ArrowType == ArrowType.Arrow)
        {
            Initiate_Arrow(StartPosition, EndPosition);
        }
        else if (M_ArrowType == ArrowType.Aiming)
        {
            Initiate_Aiming(StartPosition, EndPosition);
        }
    }


    public void Initiate_Arrow(Vector3 StartPosition, Vector3 EndPosition)
    {
        transform.rotation = defaultRotation;
        ArrowBody.transform.localPosition = defaultPosition_ArrowBody;
        ArrowBody.transform.localRotation = defaultRotation_ArrowBody;

        StartPosition = new Vector3(StartPosition.x, 5, StartPosition.z);
        EndPosition = new Vector3(EndPosition.x, 5, EndPosition.z);
        Vector3 diff = EndPosition - StartPosition;
        float distance = diff.magnitude;
        Vector3 rotateAxis = Vector3.Cross(Vector3.forward, diff);
        float rotateAngle = Vector3.Angle(Vector3.forward, diff);

        transform.Rotate(rotateAxis, rotateAngle);
        ArrowBody.transform.localScale = new Vector3(0.3f, 0.3f, distance);
        ArrowBody.transform.localPosition = new Vector3(ArrowBody.transform.localPosition.x, ArrowBody.transform.localPosition.y, ArrowBody.transform.localPosition.z - distance / 2);
        transform.position = EndPosition;
    }

    public void Initiate_Aiming(Vector3 StartPosition, Vector3 EndPosition)
    {
        transform.rotation = defaultRotation;
        ArrowBody.transform.localPosition = defaultPosition_ArrowBody;
        ControlDirectionGo.localRotation = defaultRotation_Control;

        StartPosition = new Vector3(StartPosition.x, 5, StartPosition.z);
        EndPosition = new Vector3(EndPosition.x, 5, EndPosition.z);
        Vector3 diff = EndPosition - StartPosition;
        float distance = diff.magnitude;
        Vector3 rotateAxis = Vector3.Cross(Vector3.forward, diff);
        float rotateAngle = Vector3.Angle(Vector3.forward, diff);
        ControlDirectionGo.Rotate(Vector3.up * (rotateAxis.y > 0 ? 1 : -1), rotateAngle);
        ArrowBody.transform.localScale = new Vector3(0.3f, 0.3f, distance);
        ArrowBody.transform.localPosition = new Vector3(ArrowBody.transform.localPosition.x, ArrowBody.transform.localPosition.y, ArrowBody.transform.localPosition.z - distance / 2);
        transform.position = EndPosition;
    }
}

public enum ArrowType
{
    Arrow = 0,
    Aiming = 1,
}