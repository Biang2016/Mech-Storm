using UnityEngine;
using UnityEngine.UI;

public class ChapterMapRoute : PoolObject
{
    [SerializeField] private Image Body;
    [SerializeField] private RectTransform BodyRect;
    [SerializeField] private Transform Head;

    [SerializeField] private Transform ControlDirectionGo;
    private Vector3 defaultPosition_ArrowBody;
    private Quaternion defaultRotation_Control;

    public override void PoolRecycle()
    {
        Refresh(Vector2.zero, Vector2.one, 0f);
        base.PoolRecycle();
    }

    void Awake()
    {
        defaultPosition_ArrowBody = Body.transform.localPosition;
        defaultRotation_Control = ControlDirectionGo.localRotation;
    }

    public void Refresh(Vector2 StartPosition, Vector2 EndPosition, float width)
    {
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        Body.transform.localPosition = defaultPosition_ArrowBody;
        ControlDirectionGo.localRotation = defaultRotation_Control;

        Vector2 diff = EndPosition - StartPosition;
        float distance = diff.magnitude;
        Vector3 rotateAxis = Vector3.Cross(Vector3.right, diff);
        float rotateAngle = Vector3.Angle(Vector3.right, diff);
        ControlDirectionGo.Rotate(Vector3.forward * (rotateAxis.z > 0 ? -1 : 1), -180 - rotateAngle);

        BodyRect.sizeDelta = new Vector2(distance, width);
        Body.transform.localPosition = new Vector3(Body.transform.localPosition.x + distance / 2, Body.transform.localPosition.y, Body.transform.localPosition.z);
        transform.localPosition = EndPosition;
    }
}