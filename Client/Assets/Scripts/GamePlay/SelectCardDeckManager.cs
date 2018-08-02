using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCardDeckManager : MonoBehaviour
{
    private static SelectCardDeckManager _scdm;

    public static SelectCardDeckManager SCDM
    {
        get
        {
            if (!_scdm) _scdm = FindObjectOfType<SelectCardDeckManager>();
            return _scdm;
        }
    }

    public Transform Content;

    void Start()
    {
        CardInfo_Base cardInfo = AllCards.GetCard(0);
        CardBase newCard = CardBase.InstantiateCardByCardInfo(cardInfo, Content, null, true);
        newCard.transform.localPosition = new Vector3(150, -150,-10);
        newCard.transform.localScale = Vector3.one * 120;
        newCard.transform.rotation = Quaternion.Euler(90, 180, 0);
    }

    void Update()
    {
    }
}