using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

internal class CostMagicManager : MonoBehaviour
{
    [SerializeField] private GameObject CostNumberBlock;
    protected GameObject GoNumberSet_CostNumber;
    protected CardNumberSet NumberSet_CostNumber;

    [SerializeField] private GameObject MagicNumberBlock;
    protected GameObject GoNumberSet_MagicNumber;
    protected CardNumberSet NumberSet_MagicNumber;

    void Start()
    {
        initiateNumbers(ref GoNumberSet_CostNumber, ref NumberSet_CostNumber, NumberSize.Big, CardNumberSet.TextAlign.Center, CostNumberBlock);
        NumberSet_CostNumber.Number = 4;
        initiateNumbers(ref GoNumberSet_MagicNumber, ref NumberSet_MagicNumber, NumberSize.Medium, CardNumberSet.TextAlign.Center, MagicNumberBlock);
        NumberSet_MagicNumber.Number = 4;
    }

    protected void initiateNumbers(ref GameObject Number, ref CardNumberSet cardNumberSet, NumberSize numberType, CardNumberSet.TextAlign textAlign, GameObject block)
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
}