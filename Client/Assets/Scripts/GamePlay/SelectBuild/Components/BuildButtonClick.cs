using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BuildButtonClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public UnityEvent leftClick;
    public UnityEvent middleClick;
    public UnityEvent rightClick;
    public UnityEvent leftDoubleClick;
    public UnityEvent middleDoubleClick;
    public UnityEvent rightDoubleClick;

    void Awake()
    {
    }

    private void Start()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 1)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                leftClick.Invoke();
            else if (eventData.button == PointerEventData.InputButton.Middle)
                middleClick.Invoke();
            else if (eventData.button == PointerEventData.InputButton.Right)
                rightClick.Invoke();
        }
        else if (eventData.clickCount == 2)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                leftDoubleClick.Invoke();
            else if (eventData.button == PointerEventData.InputButton.Middle)
                middleDoubleClick.Invoke();
            else if (eventData.button == PointerEventData.InputButton.Right)
                rightDoubleClick.Invoke();
        }
    }

    public void ResetListeners()
    {
        leftClick.RemoveAllListeners();
        middleClick.RemoveAllListeners();
        rightClick.RemoveAllListeners();
        leftDoubleClick.RemoveAllListeners();
        middleDoubleClick.RemoveAllListeners();
        rightDoubleClick.RemoveAllListeners();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.SoundPlay("sfx/MouseHoverBuildButton", 0.75f);
    }
}