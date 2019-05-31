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

    private static bool clickTabInFrame = false;

    void Update()
    {
        if (clickTabInFrame) return;
        if (Input.GetKeyDown(KeyCode.Tab) && isSelect)
        {
            clickTabInFrame = true;
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

                if (next == null) //Recycle
                {
                    Selectable temp = sec;
                    Selectable temp_notNull = temp;
                    while (temp != null)
                    {
                        temp_notNull = temp;
                        temp = temp.FindSelectableOnUp();
                    }

                    next = temp_notNull;
                }
            }

            InputField inputField = next.GetComponent<InputField>();
            if (inputField == null) return;
            system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
        }
    }

    void LateUpdate()
    {
        clickTabInFrame = false;
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