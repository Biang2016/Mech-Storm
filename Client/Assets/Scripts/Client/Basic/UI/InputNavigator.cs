using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UGUI Tab键切换InputField
/// </summary>
public class InputNavigator : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private EventSystem system;
    private bool isSelect = false;

    void Start()
    {
        system = EventSystem.current;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && isSelect)
        {
            Selectable next = null;
            var sec = system.currentSelectedGameObject.GetComponent<Selectable>();
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                next = sec.FindSelectableOnUp();
                if (next == null)
                    next = sec;
            }
            else
            {
                next = sec.FindSelectableOnDown();
                if (next == null)
                    next = sec;
            }

            if (next != null)
            {
                var inputField = next.GetComponent<InputField>();
                if (inputField == null) return;
                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelect = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelect = false;
    }
}