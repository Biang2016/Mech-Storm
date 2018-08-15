﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 卡堆
/// </summary>
public class CardDeckManager : MonoBehaviour
{
    private static CardDeckManager _cdm;

    public static CardDeckManager CDM
    {
        get
        {
            if (!_cdm) _cdm = FindObjectOfType<CardDeckManager>();
            return _cdm;
        }
    }

    [SerializeField] private Transform SelfCardDeckArea;
    [SerializeField] private Transform EnemyCardDeckArea;

    [SerializeField] private Text SelfCardLeftNumText;
    [SerializeField] private Text EnemyCardLeftNumText;

    [SerializeField] private Text SelfCardLeftNumText_BG;
    [SerializeField] private Text EnemyCardLeftNumText_BG;

    private CardDeckCard[] self_CardDeckCards;
    private CardDeckCard[] enemy_CardDeckCards;

    private int[] cardDeckShowCardNumMap; //剩余卡牌数量和卡堆模型中显示的卡牌数量映射关系

    void Start()
    {
        InitializeCardDeckShowCardMap();
        InitializeCardDeckCard();
        HideAll();
    }

    private void InitializeCardDeckShowCardMap()
    {
        cardDeckShowCardNumMap = new int[] {0, 1, 2, 3, 3, 3, 4, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 9, 9, 9, 9, 9, 10};
    }

    private void InitializeCardDeckCard()
    {
        self_CardDeckCards = new CardDeckCard[GameManager.GM.CardDeckCardNum];
        enemy_CardDeckCards = new CardDeckCard[GameManager.GM.CardDeckCardNum];

        for (int i = 0; i < GameManager.GM.CardDeckCardNum; i++)
        {
            self_CardDeckCards[i] = GameObjectPoolManager.GOPM.Pool_CardDeckCardPool.AllocateGameObject(SelfCardDeckArea).GetComponent<CardDeckCard>();
            enemy_CardDeckCards[i] = GameObjectPoolManager.GOPM.Pool_CardDeckCardPool.AllocateGameObject(EnemyCardDeckArea).GetComponent<CardDeckCard>();
            self_CardDeckCards[i].ChangeColor(ClientUtils.HTMLColorToColor("#919191"));
            enemy_CardDeckCards[i].ChangeColor(ClientUtils.HTMLColorToColor("#919191"));
            self_CardDeckCards[i].ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#007AFF"));
            enemy_CardDeckCards[i].ChangeCardBloomColor(ClientUtils.HTMLColorToColor("#FF3645"));
            self_CardDeckCards[i].transform.Translate(GameManager.GM.Self_CardDeckCardInterval * i);
            enemy_CardDeckCards[i].transform.Translate(GameManager.GM.Enemy_CardDeckCardInterval * i);
        }

        SetSelfCardDeckNumber(0);
        SetEnemyCardDeckNumber(0);
    }


    public void HideAll()
    {
        SelfCardDeckArea.gameObject.SetActive(false);
        EnemyCardDeckArea.gameObject.SetActive(false);
        SelfCardLeftNumText.gameObject.SetActive(false);
        EnemyCardLeftNumText.gameObject.SetActive(false);
        SelfCardLeftNumText_BG.gameObject.SetActive(false);
        EnemyCardLeftNumText_BG.gameObject.SetActive(false);
    }

    public void ShowAll()
    {
        SelfCardDeckArea.gameObject.SetActive(true);
        EnemyCardDeckArea.gameObject.SetActive(true);
        SelfCardLeftNumText.gameObject.SetActive(true);
        EnemyCardLeftNumText.gameObject.SetActive(true);
        SelfCardLeftNumText_BG.gameObject.SetActive(true);
        EnemyCardLeftNumText_BG.gameObject.SetActive(true);
    }

    public void SetSelfCardDeckNumber(int value)
    {
        SelfCardLeftNumText.text = value.ToString();
        SelfCardLeftNumText_BG.text = value.ToString();
        SetCardDeckShowCardNum(self_CardDeckCards, value);
    }

    public void SetEnemyCardDeckNumber(int value)
    {
        EnemyCardLeftNumText.text = value.ToString();
        EnemyCardLeftNumText_BG.text = value.ToString();
        SetCardDeckShowCardNum(enemy_CardDeckCards, value);
    }

    public void SetCardDeckShowCardNum(CardDeckCard[] targetCardDeckShowCards, int number)
    {
        int showCardNumber = 0;
        if (number <= 0) showCardNumber = 0;
        else if (number > cardDeckShowCardNumMap.Length)
        {
            showCardNumber = cardDeckShowCardNumMap[cardDeckShowCardNumMap.Length - 1];
        }
        else
        {
            showCardNumber = cardDeckShowCardNumMap[number];
        }

        for (int i = targetCardDeckShowCards.Length - 1; i >= showCardNumber; i--)
        {
            if (targetCardDeckShowCards[i].gameObject.activeSelf) targetCardDeckShowCards[i].gameObject.SetActive(false);
        }

        for (int i = showCardNumber - 1; i >= 0; i--)
        {
            if (!targetCardDeckShowCards[i].gameObject.activeSelf) targetCardDeckShowCards[i].gameObject.SetActive(true);
        }
    }
}