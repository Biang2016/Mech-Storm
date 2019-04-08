﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardStarsComponent : CardComponentBase
{
    [SerializeField] private SpriteRenderer[] Stars;

    void Awake()
    {
        if (Stars.Length > 0)
        {
            StarDefaultSortingOrder = Stars[0].sortingOrder;
        }
    }

    public void SetStarNumber(int number)
    {
        for (int i = 0; i < Stars.Length; i++)
        {
            if (Stars[i] != null)
            {
                Stars[i].color = i < number ? Color.white : Color.black;
            }
        }
    }

    private int StarDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        foreach (SpriteRenderer star in Stars)
        {
            star.sortingOrder = cardSortingIndex * 50 +  StarDefaultSortingOrder;
        }
    }
}