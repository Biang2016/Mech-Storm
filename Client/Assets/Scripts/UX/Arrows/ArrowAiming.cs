using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAiming : Arrow
{
    public override void PoolRecycle()
    {
        isOnHover = false;
        base.PoolRecycle();
    }


    Animator animator;

    void Awake()
    {
        defaultRotation = transform.rotation;
        defaultPosition_ArrowBody = ArrowBody.transform.localPosition;
        defaultRotation_Control = ControlDirectionGo.localRotation;
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("IsOnValidTarget", true);
    }

    Quaternion defaultRotation;
    Vector3 defaultPosition_ArrowBody;
    public Transform ControlDirectionGo;
    Quaternion defaultRotation_Control;

    public override void Render(Vector3 StartPosition, Vector3 EndPosition)
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

    private bool isOnHover;

    public bool IsOnHover
    {
        get { return isOnHover; }

        set
        {
            animator.SetBool("IsOnValidTarget", value);
            isOnHover = value;
        }
    }
}