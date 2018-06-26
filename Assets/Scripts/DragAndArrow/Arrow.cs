using UnityEngine;
using System.Collections;
using System;

public class Arrow : MonoBehaviour,IGameObjectPool
{
    public void PoolRecycle()
    {
        GameObjectPoolManager.GOPM.Pool_ArrowPool.RecycleGameObject(gameObject);
    }

    public GameObject ArrowBody;
    void Awake()
    {
        defaultRotation = transform.rotation;
        defaultPosition_ArrowBody = ArrowBody.transform.localPosition;
        defaultRotation_ArrowBody = ArrowBody.transform.localRotation;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    Quaternion defaultRotation;
    Vector3 defaultPosition_ArrowBody;
    Quaternion defaultRotation_ArrowBody;
    public void Initiate(Vector3 StartPosition, Vector3 EndPosition)
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


}
