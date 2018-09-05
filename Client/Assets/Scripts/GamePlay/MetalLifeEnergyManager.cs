using UnityEngine;
using UnityEngine.UI;

internal class MetalLifeEnergyManager : MonoBehaviour
{
    internal ClientPlayer ClientPlayer;

    [SerializeField] private GameObject MetalNumberBlock;
    private GameObject GoNumberSet_MetalNumber;
    private CardNumberSet NumberSet_MetalNumber;

    [SerializeField] private Text LifeNumber;
    [SerializeField] private Text TotalLifeNumber;
    [SerializeField] private Text EnergyNumber;
    [SerializeField] private Text TotalEnergyNumber;


    void Awake()
    {
        initiateNumbers(ref GoNumberSet_MetalNumber, ref NumberSet_MetalNumber, NumberSize.Big, CardNumberSet.TextAlign.Center, MetalNumberBlock);
    }

    private void initiateNumbers(ref GameObject Number, ref CardNumberSet cardNumberSet, NumberSize numberType, CardNumberSet.TextAlign textAlign, GameObject block)
    {
        if (!Number)
        {
            Number = GameObjectPoolManager.Instance.Pool_CardNumberSetPool.AllocateGameObject(block.transform);
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(0, numberType, textAlign, false);
        }
        else
        {
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(0, numberType, textAlign, false);
        }
    }

    private void initiateNumbers(ref GameObject Number, ref CardNumberSet cardNumberSet, NumberSize numberType, CardNumberSet.TextAlign textAlign, GameObject block, char firstSign)
    {
        if (!Number)
        {
            Number = GameObjectPoolManager.Instance.Pool_CardNumberSetPool.AllocateGameObject(block.transform);
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(firstSign, 0, numberType, textAlign, false);
        }
        else
        {
            cardNumberSet = Number.GetComponent<CardNumberSet>();
            cardNumberSet.initiate(firstSign, 0, numberType, textAlign, false);
        }
    }

    public void SetMetal(int value)
    {
        MetalBarManager.ClientPlayer = ClientPlayer;
        MetalBarManager.SetMetalNumber(value);
        NumberSet_MetalNumber.Number = value;
        MetalNumberBlock.transform.localPosition = Vector3.Lerp(MetalNumberMinPos.localPosition, MetalNumberMaxPos.localPosition, (float) value / GamePlaySettings.MaxMetal);
    }

    public void SetLife(int value)
    {
        LifeNumber.text = value.ToString();
        LifeBarMask.transform.localPosition = Vector3.Lerp(LifeBarMaskMinPos.localPosition, LifeBarMaskMaxPos.localPosition, (float) value / ClientPlayer.LifeMax);
    }

    public void SetTotalLife(int value)
    {
        TotalLifeNumber.text = "/" + value;
    }

    public void SetEnergy(int value)
    {
        EnergyNumber.text = value.ToString();
        EnergyBarMask.transform.localPosition = Vector3.Lerp(EnergyBarMaskMinPos.localPosition, EnergyBarMaskMaxPos.localPosition, (float) value / ClientPlayer.EnergyMax);
    }

    public void SetTotalEnergy(int value)
    {
        TotalEnergyNumber.text = "/" + value;
    }

    [SerializeField] private Transform MetalNumberMinPos;
    [SerializeField] private Transform MetalNumberMaxPos;

    [SerializeField] private Transform LifeBarMask;
    [SerializeField] private Transform LifeBarMaskMinPos;
    [SerializeField] private Transform LifeBarMaskMaxPos;

    [SerializeField] private Transform EnergyBarMask;
    [SerializeField] private Transform EnergyBarMaskMinPos;
    [SerializeField] private Transform EnergyBarMaskMaxPos;

    public MetalBarManager MetalBarManager;
}