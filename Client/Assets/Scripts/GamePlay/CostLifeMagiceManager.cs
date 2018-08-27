using UnityEngine;

internal class CostLifeMagiceManager : MonoBehaviour
{
    public ClientPlayer ClientPlayer;

    [SerializeField] private GameObject CostNumberBlock;
    private GameObject GoNumberSet_CostNumber;
    private CardNumberSet NumberSet_CostNumber;

    [SerializeField] private GameObject LifeNumberBlock;
    private GameObject GoNumberSet_LifeNumber;
    private CardNumberSet NumberSet_LifeNumber;

    [SerializeField] private GameObject MagicNumberBlock;
    private GameObject GoNumberSet_MagicNumber;
    private CardNumberSet NumberSet_MagicNumber;

    void Start()
    {
        initiateNumbers(ref GoNumberSet_CostNumber, ref NumberSet_CostNumber, NumberSize.Big, CardNumberSet.TextAlign.Center, CostNumberBlock);
        //initiateNumbers(ref GoNumberSet_LifeNumber, ref NumberSet_LifeNumber, NumberSize.Big, CardNumberSet.TextAlign.Center, LifeNumberBlock);
        initiateNumbers(ref GoNumberSet_MagicNumber, ref NumberSet_MagicNumber, NumberSize.Medium, CardNumberSet.TextAlign.Center, MagicNumberBlock);
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

    public void SetCost(int value)
    {
        NumberSet_CostNumber.Number = value;
        CostBarMask.transform.localPosition = Vector3.Lerp(CostBarMaskMinPos.localPosition, CostBarMaskMaxPos.localPosition, (float) value / GamePlaySettings.MaxCost);
        CostNumberBlock.transform.localPosition = Vector3.Lerp(CostNumberMinPos.localPosition, CostNumberMaxPos.localPosition, (float) value / GamePlaySettings.MaxCost);
    }

    public void SetLife(int value)
    {
        //NumberSet_LifeNumber.Number = value;
        //LifeBarMask.transform.localPosition = Vector3.Lerp(LifeBarMaskMinPos.localPosition, LifeBarMaskMaxPos.localPosition, (float) value / GamePlaySettings.PlayerDefaultLifeMax);
        //LifeNumberBlock.transform.localPosition = Vector3.Lerp(LifeNumberMinPos.localPosition, LifeNumberMaxPos.localPosition, (float) value / GamePlaySettings.PlayerDefaultLifeMax);
    }

    public void SetMagic(int value)
    {
        NumberSet_MagicNumber.Number = value;
        MagicBarMask.transform.localPosition = Vector3.Lerp(MagicBarMaskMinPos.localPosition, MagicBarMaskMaxPos.localPosition, (float) value / GamePlaySettings.PlayerDefaultMagicMax);
        MagicNumberBlock.transform.localPosition = Vector3.Lerp(MagicNumberMinPos.localPosition, MagicNumberMaxPos.localPosition, (float) value / GamePlaySettings.PlayerDefaultMagicMax);
    }


    [SerializeField] private Transform CostBarMask;
    [SerializeField] private Transform CostBarMaskMinPos;
    [SerializeField] private Transform CostBarMaskMaxPos;
    [SerializeField] private Transform CostNumberMinPos;
    [SerializeField] private Transform CostNumberMaxPos;

    [SerializeField] private Transform LifeBarMask;
    [SerializeField] private Transform LifeBarMaskMinPos;
    [SerializeField] private Transform LifeBarMaskMaxPos;
    [SerializeField] private Transform LifeNumberMinPos;
    [SerializeField] private Transform LifeNumberMaxPos;

    [SerializeField] private Transform MagicBarMask;
    [SerializeField] private Transform MagicBarMaskMinPos;
    [SerializeField] private Transform MagicBarMaskMaxPos;
    [SerializeField] private Transform MagicNumberMinPos;
    [SerializeField] private Transform MagicNumberMaxPos;
}