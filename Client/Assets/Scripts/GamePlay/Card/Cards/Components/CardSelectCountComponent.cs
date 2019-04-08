using TMPro;
using UnityEngine;

public class CardSelectCountComponent : CardComponentBase
{
    [SerializeField] private TextMeshPro SelectCountText;
    [SerializeField] private TextMeshPro SelectLimitText;
    [SerializeField] private TextMeshPro SlashText;

    private bool isForceShow;

    public bool IsForceShow
    {
        get { return isForceShow; }
        set
        {
            if (isForceShow != value)
            {
                isForceShow = value;
                RefreshShow();
            }
        }
    }

    private void RefreshShow()
    {
        if (!isForceShow)
        {
            if (SelectCount == 0)
            {
                SelectCountText.text = "";
            }

            if (SelectLimitCount == 0)
            {
                SelectLimitText.text = "";
                SlashText.text = "";
            }
        }
        else
        {
            if (SelectCount == 0)
            {
                SelectCountText.text = "0";
            }

            if (SelectLimitCount == 0)
            {
                SelectLimitText.text = "0";
            }

            SlashText.text = "/";
        }
    }

    private int selectCount;

    public int SelectCount
    {
        get { return selectCount; }
        set
        {
            if (selectCount != value)
            {
                SelectCountText.text = value.ToString();
                selectCount = value;
                RefreshShow();
            }
        }
    }

    private int selectLimitCount;

    public int SelectLimitCount
    {
        get { return selectLimitCount; }
        set
        {
            if (selectLimitCount != value)
            {
                SelectLimitText.text = value.ToString();
                selectLimitCount = value;
                RefreshShow();
            }
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