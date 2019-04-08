using TMPro;
using UnityEngine;

public class CardSelectCountComponent : CardComponentBase
{
    [SerializeField] private TextMeshPro SelectCountText;
    [SerializeField] private TextMeshPro SelectLimitText;
    [SerializeField] private TextMeshPro SlashText;
    [SerializeField] private Transform Panel;
    [SerializeField] private Transform HigherPivot;
    [SerializeField] private Transform LowerPivot;

    public enum Position
    {
        Higher,
        Lower
    }

    public void SetPosition(Position pos)
    {
        switch (pos)
        {
            case Position.Higher:
                Panel.position = HigherPivot.position;
                break;
            case Position.Lower:
                Panel.position = LowerPivot.position;
                break;
        }
    }

    private bool isForceShow;

    public void SetForceShow(bool value)
    {
        if (isForceShow != value)
        {
            isForceShow = value;
            RefreshShow();
        }
    }

    private void RefreshShow()
    {
        if (!isForceShow)
        {
            if (selectCount == 0)
            {
                SelectCountText.text = "";
            }

            if (selectLimitCount == 0)
            {
                SelectLimitText.text = "";
                SlashText.text = "";
            }
        }
        else
        {
            if (selectCount == 0)
            {
                SelectCountText.text = "0";
            }

            if (selectLimitCount == 0)
            {
                SelectLimitText.text = "0";
            }

            SlashText.text = "/";
        }
    }

    private int selectCount;

    public void SetSelectCount(int value)
    {
        if (selectCount != value)
        {
            SelectCountText.text = value.ToString();
            selectCount = value;
            RefreshShow();
        }
    }

    private int selectLimitCount;

    public void SetSelectLimitCount(int value)
    {
        if (selectLimitCount != value)
        {
            SelectLimitText.text = value.ToString();
            selectLimitCount = value;
            RefreshShow();
        }
    }

    void Awake()
    {
        SelectCountTextDefaultSortingOrder = SelectCountText.sortingOrder;
        SelectLimitTextDefaultSortingOrder = SelectLimitText.sortingOrder;
        SlashTextDefaultSortingOrder = SlashText.sortingOrder;
    }

    private int SelectCountTextDefaultSortingOrder;
    private int SelectLimitTextDefaultSortingOrder;
    private int SlashTextDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        SelectCountText.sortingOrder = cardSortingIndex * 50 + SelectCountTextDefaultSortingOrder;
        SelectLimitText.sortingOrder = cardSortingIndex * 50 + SelectLimitTextDefaultSortingOrder;
        SlashText.sortingOrder = cardSortingIndex * 50 + SlashTextDefaultSortingOrder;
    }
}