using UnityEngine;
using UnityEngine.UI;

internal class CostLifeMagiceManager : MonoBehaviour
{
    public ClientPlayer ClientPlayer;

    [SerializeField] private GameObject CostNumberBlock;
    private GameObject GoNumberSet_CostNumber;
    private CardNumberSet NumberSet_CostNumber;

    [SerializeField] private Text LifeNumber;
    [SerializeField] private Text TotalLifeNumber;
    [SerializeField] private Text MagicNumber;
    [SerializeField] private Text TotalMagicNumber;

    void Awake()
    {
        initiateNumbers(ref GoNumberSet_CostNumber, ref NumberSet_CostNumber, NumberSize.Big, CardNumberSet.TextAlign.Center, CostNumberBlock);
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
        LifeNumber.text = value.ToString();
        LifeBarMask.transform.localPosition = Vector3.Lerp(LifeBarMaskMinPos.localPosition, LifeBarMaskMaxPos.localPosition, (float) value / ClientPlayer.LifeMax);
    }

    public void SetTotalLife(int value)
    {
        TotalLifeNumber.text = "/" + value;
    }

    public void SetMagic(int value)
    {
        MagicNumber.text = value.ToString();
        MagicBarMask.transform.localPosition = Vector3.Lerp(MagicBarMaskMinPos.localPosition, MagicBarMaskMaxPos.localPosition, (float) value / ClientPlayer.MagicMax);
    }

    public void SetTotalMagic(int value)
    {
        TotalLifeNumber.text = "/" + value;
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