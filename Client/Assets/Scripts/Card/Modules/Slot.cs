using UnityEngine;

internal class Slot : MonoBehaviour, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;
    internal ModuleRetinue M_ModuleRetinue;

    [SerializeField] private Renderer OnHoverShowBloom;
    [SerializeField] private SlotTypes _mSlotTypes = SlotTypes.None;

    public SlotTypes MSlotTypes
    {
        get { return _mSlotTypes; }

        set
        {
            _mSlotTypes = value;
            ChangeSlotColor(value);
            if (value != SlotTypes.None)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void ChangeSlotColor(SlotTypes slotTypes)
    {
        Renderer rd = GetComponent<Renderer>();
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rd.GetPropertyBlock(mpb);
        switch (slotTypes)
        {
            case SlotTypes.Weapon:
                mpb.SetColor("_Color", GameManager.Instance.Slot1Color);
                mpb.SetColor("_EmissionColor", GameManager.Instance.Slot1Color);
                break;
            case SlotTypes.Shield:
                mpb.SetColor("_Color", GameManager.Instance.Slot2Color);
                mpb.SetColor("_EmissionColor", GameManager.Instance.Slot2Color);
                break;
            case SlotTypes.Pack:
                mpb.SetColor("_Color", GameManager.Instance.Slot3Color);
                mpb.SetColor("_EmissionColor", GameManager.Instance.Slot3Color);
                break;
            case SlotTypes.MA:
                mpb.SetColor("_Color", GameManager.Instance.Slot4Color);
                mpb.SetColor("_EmissionColor", GameManager.Instance.Slot4Color);
                break;
            default:
                gameObject.SetActive(false);
                break;
        }

        rd.SetPropertyBlock(mpb);
    }

    public void ShowHoverGO()
    {
        if (OnHoverShowBloom)
        {
            OnHoverShowBloom.enabled = true;
        }
    }

    public void HideHoverShowGO()
    {
        if (OnHoverShowBloom)
        {
            OnHoverShowBloom.enabled = false;
        }
    }

    public void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
        if (DragManager.Instance.CurrentDrag)
        {
            switch (MSlotTypes)
            {
                case SlotTypes.Weapon:
                    CardWeapon cw = DragManager.Instance.CurrentDrag_CardWeapon;
                    if (cw && cw.ClientPlayer == ClientPlayer)
                    {
                        ShowHoverGO();
                    }

                    break;
                case SlotTypes.Shield:
                    CardShield cs = DragManager.Instance.CurrentDrag_CardShield;
                    if (cs && cs.ClientPlayer == ClientPlayer)
                    {
                        ShowHoverGO();
                    }

                    break;
            }
        }
    }

    public void MouseHoverComponent_OnMouseEnterImmediately(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnMouseEnter(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnMouseOver()
    {
    }

    public void MouseHoverComponent_OnMouseLeave()
    {
    }

    public void MouseHoverComponent_OnMouseLeaveImmediately()
    {
        HideHoverShowGO();
    }

    public void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
        HideHoverShowGO();
    }
}