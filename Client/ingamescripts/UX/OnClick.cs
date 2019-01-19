using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnClick : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent LeftClick;
    public UnityEvent LeftDoubleClick;
    public UnityEvent RightClick;
    public UnityEvent RightDoubleClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (eventData.clickCount == 1)
            {
                if (LeftClick != null) LeftClick.Invoke();
            }
            else if (eventData.clickCount == 2)
            {
                if (LeftDoubleClick != null) LeftDoubleClick.Invoke();
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (eventData.clickCount == 1)
            {
                if (RightClick != null) RightClick.Invoke();
            }
            else if (eventData.clickCount == 2)
            {
                if (RightDoubleClick != null) RightDoubleClick.Invoke();
            }
        }
    }
}