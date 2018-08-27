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
    [SerializeField] private GameObject TotalLifeNumberBlock;
    private GameObject GoNumberSet_TotalLifeNumber;
    private CardNumberSet NumberSet_TotalLifeNumber;

    [SerializeField] private GameObject MagicNumberBlock;
    private GameObject GoNumberSet_MagicNumber;
    private CardNumberSet NumberSet_MagicNumber;
    [SerializeField] private GameObject TotalMagicNumberBlock;
    private GameObject GoNumberSet_TotalMagicNumber;
    private CardNumberSet NumberSet_TotalMagicNumber;

    void Start()
    {
        initiateNumbers(ref GoNumberSet_CostNumber, ref NumberSet_CostNumber, NumberSize.Big, CardNumberSet.TextAlign.Center, CostNumberBlock);
        initiateNumbers(ref GoNumberSet_LifeNumber, ref NumberSet_LifeNumber, NumberSize.Big, CardNumberSet.TextAlign.Left, LifeNumberBlock);
        initiateNumbers(ref GoNumberSet_TotalLifeNumber, ref NumberSet_TotalLifeNumber, NumberSize.Big, CardNumberSet.TextAlign.Right, TotalLifeNumberBlock, '/');
        initiateNumbers(ref GoNumberSet_MagicNumber, ref NumberSet_MagicNumber, NumberSize.Big, CardNumberSet.TextAlign.Left, MagicNumberBlock);
        initiateNumbers(ref GoNumberSet_TotalMagicNumber, ref NumberSet_TotalMagicNumber, NumberSize.Big, CardNumberSet.TextAlign.Right, TotalMagicNumberBlock, '/');

        SetLife(50);
        SetTotalLife(200);
        SetMagic(10);
        SetTotalMagic(50);
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

    public void SetCost(int value)
    {
        NumberSet_CostNumber.Number = value;
        CostBarMask.transform.localPosition = Vector3.Lerp(CostBarMaskMinPos.localPosition, CostBarMaskMaxPos.localPosition, (float) value / GamePlaySettings.MaxCost);
        CostNumberBlock.transform.localPosition = Vector3.Lerp(CostNumberMinPos.localPosition, CostNumberMaxPos.localPosition, (float) value / GamePlaySettings.MaxCost);
    }

    public void SetLife(int value)
    {
        NumberSet_LifeNumber.Number = value;
        LifeBarMask.transform.localPosition = Vector3.Lerp(LifeBarMaskMinPos.localPosition, LifeBarMaskMaxPos.localPosition, (float) value / GamePlaySettings.PlayerDefaultLifeMax);
    }

    public void SetTotalLife(int value)
    {
        NumberSet_TotalLifeNumber.Number = value;
    }

    public void SetMagic(int value)
    {
        NumberSet_MagicNumber.Number = value;
        MagicBarMask.transform.localPosition = Vector3.Lerp(MagicBarMaskMinPos.localPosition, MagicBarMaskMaxPos.localPosition, (float) value / GamePlaySettings.PlayerDefaultMagicMax);
    }

    public void SetTotalMagic(int value)
    {
        NumberSet_TotalMagicNumber.Number = value;
    }

    [SerializeField] private Transform CostBarMask;
    [SerializeField] private Transform CostBarMaskMinPos;
    [SerializeField] private Transform CostBarMaskMaxPos;
    [SerializeField] private Transform CostNumberMinPos;
    [SerializeField] private Transform CostNumberMaxPos;

    [SerializeField] private Transform LifeBarMask;
    [SerializeField] private Transform LifeBarMaskMinPos;
    [SerializeField] private Transform LifeBarMaskMaxPos;

    [SerializeField] private Transform MagicBarMask;
    [SerializeField] private Transform MagicBarMaskMinPos;
    [SerializeField] private Transform MagicBarMaskMaxPos;
}