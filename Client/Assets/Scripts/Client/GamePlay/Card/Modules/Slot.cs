using UnityEngine;
using UnityEngine.Rendering;

public class Slot : MonoBehaviour, IMouseHoverComponent
{
    internal ClientPlayer ClientPlayer;
    internal ModuleMech Mech;

    [SerializeField] private MeshRenderer SlotLight;
    [SerializeField] private MeshRenderer SlotBloom;
    [SerializeField] private SortingGroup SlotLightSG;
    [SerializeField] private SortingGroup SlotBloomSG;
    [SerializeField] private BoxCollider SlotBoxCollider;

    void Awake()
    {
        SlotLightDefaultSortingOrder = SlotLightSG.sortingOrder;
        SlotBloomDefaultSortingOrder = SlotBloomSG.sortingOrder;
    }

    public void Initialize(ClientPlayer clientPlayer, ModuleMech mech, SlotTypes slotType)
    {
        ClientPlayer = clientPlayer;
        Mech = mech;
        MSlotTypes = slotType;
        SlotBoxCollider.enabled = Mech != null;
    }

    private SlotTypes _mSlotTypes = SlotTypes.None;

    public SlotTypes MSlotTypes
    {
        get { return _mSlotTypes; }

        set
        {
            _mSlotTypes = value;
            //ClientUtils.ChangeSlotColor(SlotLight, value);
            //ClientUtils.ChangeSlotColor(SlotBloom, value);
            SlotLight.enabled = value != SlotTypes.None;
            SlotBloom.enabled = value != SlotTypes.None;
        }
    }

    public void ShowSlotBloom(bool isShow, bool isSniper)
    {
        SlotBloom.gameObject.SetActive(isShow);

        if (isSniper && Mech)
        {
            if (MSlotTypes == SlotTypes.Weapon && Mech.CardInfo.MechInfo.IsSniper)
            {
                Mech?.ShowSniperTipText(isShow);
            }
        }
    }

    public void ShowSlotLight(bool isShow)
    {
        SlotLight.gameObject.SetActive(isShow);
    }

    private int SlotLightDefaultSortingOrder;
    private int SlotBloomDefaultSortingOrder;

    public void SetSortingLayer(int index)
    {
        SlotLightSG.sortingOrder = index * 50 + SlotLightDefaultSortingOrder;
        SlotBloomSG.sortingOrder = index * 50 + SlotBloomDefaultSortingOrder;
    }

    public void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
        if (DragManager.Instance.CurrentDrag)
        {
            CardEquip cw = DragManager.Instance.CurrentDrag_CardEquip;
            if (cw && cw.ClientPlayer == ClientPlayer)
            {
                if (_mSlotTypes != SlotTypes.None) ShowSlotBloom(true, false);
            }
        }
    }

    public void MouseHoverComponent_OnHover1Begin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnHover1End()
    {
        ShowSlotBloom(false, false);
    }

    public void MouseHoverComponent_OnHover2Begin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnHover2End()
    {
    }

    public void MouseHoverComponent_OnFocusBegin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnFocusEnd()
    {
    }

    public void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
        ShowSlotBloom(false, false);
    }
}