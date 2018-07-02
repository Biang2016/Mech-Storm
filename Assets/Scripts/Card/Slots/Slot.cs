using UnityEngine;
using System.Collections;

public class Slot : MonoBehaviour
{
    public Player Player;

    [SerializeField]
    private SlotType m_SlotType = SlotType.None;

    public SlotType M_SlotType
    {
        get { return m_SlotType; }

        set
        {
            m_SlotType = value;
            ChangeSlotColor(value);
            if (value != SlotType.None)
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

    private void ChangeSlotColor(SlotType slotType)
    {
        Renderer rd = GetComponent<Renderer>();
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rd.GetPropertyBlock(mpb);
        switch (slotType)
        {
            case SlotType.Weapon:
                mpb.SetColor("_Color", GameManager.GM.Slot1Color);
                mpb.SetColor("_EmissionColor", GameManager.GM.Slot1Color);
                break;
            case SlotType.Shield:
                mpb.SetColor("_Color", GameManager.GM.Slot2Color);
                mpb.SetColor("_EmissionColor", GameManager.GM.Slot2Color);
                break;
            case SlotType.Pack:
                mpb.SetColor("_Color", GameManager.GM.Slot3Color);
                mpb.SetColor("_EmissionColor", GameManager.GM.Slot3Color);
                break;
            case SlotType.MA:
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

public enum SlotType
{
    None = 0,
    Weapon = 1,
    Shield = 2,
    Pack = 3,
    MA = 4,
}