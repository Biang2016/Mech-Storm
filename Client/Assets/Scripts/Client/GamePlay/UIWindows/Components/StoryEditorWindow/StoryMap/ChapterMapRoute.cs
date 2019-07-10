using UnityEngine;
using UnityEngine.UI;

public class ChapterMapRoute : PoolObject
{
    [SerializeField] private Image Body;
    [SerializeField] private RectTransform BodyRect;
    [SerializeField] private Transform Head;
    [SerializeField] private Image HeadImage;

    [SerializeField] private Sprite[] BodySprites;

    [SerializeField] private Transform ControlDirectionGo;
    private Vector3 defaultPosition_ArrowBody;
    private Quaternion defaultRotation_Control;

    public override void PoolRecycle()
    {
        Refresh(Vector2.zero, Vector2.one, 0, 0, 0);
        base.PoolRecycle();
    }

    void Awake()
    {
        defaultPosition_ArrowBody = Body.transform.localPosition;
        defaultRotation_Control = ControlDirectionGo.localRotation;
    }

    private Vector2 StartPosition;
    private Vector2 EndPosition;
    internal int RouteIndex;
    internal int NodeIndex_0;
    internal int NodeIndex_1;
    private float lineWidth = 4f;

    public void Refresh(Vector2 startPosition, Vector2 endPosition, int routeIndex, int nodeIndex0, int nodeIndex1)
    {
        StartPosition = startPosition;
        EndPosition = endPosition;
        RouteIndex = routeIndex;
        NodeIndex_0 = nodeIndex0;
        NodeIndex_1 = nodeIndex1;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        Body.transform.localPosition = defaultPosition_ArrowBody;
        ControlDirectionGo.localRotation = defaultRotation_Control;

        Vector2 diff = endPosition - startPosition;
        float distance = diff.magnitude;
        Vector3 rotateAxis = Vector3.Cross(Vector3.right, diff);
        float rotateAngle = Vector3.Angle(Vector3.right, diff);
        ControlDirectionGo.Rotate(Vector3.forward * (rotateAxis.z > 0 ? -1 : 1), -180 - rotateAngle);

        BodyRect.sizeDelta = new Vector2(distance, lineWidth);
        Body.transform.localPosition = new Vector3(Body.transform.localPosition.x + distance / 2, Body.transform.localPosition.y, Body.transform.localPosition.z);
        transform.localPosition = EndPosition;
    }

    public void Refresh()
    {
        Refresh(StartPosition, EndPosition, RouteIndex, NodeIndex_0, NodeIndex_1);
    }

    public void SetRouteState(RouteStates routeState)
    {
        if (routeState != RouteStates.None)
        {
            Body.sprite = BodySprites[(int) routeState];
        }

        switch (routeState)
        {
            case RouteStates.None:
            {
                Body.color = ClientUtils.HTMLColorToColor("#00000000");
                break;
            }
            case RouteStates.Dashed:
            {
                Body.color = ClientUtils.HTMLColorToColor("#9F9F9F");
                lineWidth = 8f;
                Refresh();
                break;
            }
            case RouteStates.NextStep:
            {
                Body.color = ClientUtils.HTMLColorToColor("#0099FF");
                lineWidth = 8f;
                Refresh();
                break;
            }
            case RouteStates.Conquered:
            {
                Body.color = ClientUtils.HTMLColorToColor("#00FF0F");
                lineWidth = 12f;
                Refresh();
                break;
            }
        }
    }

    public enum RouteStates
    {
        None,
        Dashed,
        NextStep,
        Conquered
    }
}