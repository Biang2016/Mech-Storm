using UnityEngine;
using System.Collections;

internal class Slot : MonoBehaviour
{
    public ClientPlayer ClientPlayer;

    [SerializeField]
    private SlotTypes _mSlotTypes = SlotTypes.None;

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

    void Start()
    {
    }

    void Update()
    {
    }

    private void ChangeSlotColor(SlotTypes slotTypes)
    {
        Renderer rd = GetComponent<Renderer>();
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rd.GetPropertyBlock(mpb);
        switch (slotTypes)
        {
            case SlotTypes.Weapon:
                mpb.SetColor("_Color", GameManager.GM.Slot1Color);
                mpb.SetColor("_EmissionColor", GameManager.GM.Slot1Color);
                break;
            case SlotTypes.Shield:
                mpb.SetColor("_Color", GameManager.GM.Slot2Color);
                mpb.SetColor("_EmissionColor", GameManager.GM.Slot2Color);
                break;
            case SlotTypes.Pack:
                mpb.SetColor("_Color", GameManager.GM.Slot3Color);
                mpb.SetColor("_EmissionColor", GameManager.GM.Slot3Color);
                break;
            case SlotTypes.MA:
                mpb.SetColor("_Color", GameManager.GM.Slot4Color);
                mpb.SetColor("_EmissionColor", GameManager.GM.Slot4Color);
                break;
            default:
                gameObject.SetActive(false);
                break;
        }

        rd.SetPropertyBlock(mpb);
    }
}

