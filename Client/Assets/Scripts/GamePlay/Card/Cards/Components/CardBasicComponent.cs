using UnityEngine;
using UnityEngine.Rendering;

public class CardBasicComponent : CardComponentBase
{
    [SerializeField] private MeshRenderer CardBloom;
    [SerializeField] private MeshRenderer MainBoard;
    [SerializeField] private MeshRenderer CardShadow;
    [SerializeField] private SortingGroup CardBloomSG;
    [SerializeField] private SortingGroup MainBoardSG;
    [SerializeField] private SortingGroup CardShadowSG;
    [SerializeField] private SpriteRenderer Picture;

    public void SetMainBoardColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(MainBoard, color, intensity);
    }

    public void SetCardBloomColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(CardBloom, color, intensity);
    }

    public void SetPictureColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(Picture, color, intensity);
    }

    public void SetBloomShow(bool isShow)
    {
        CardBloom.gameObject.SetActive(isShow);
    }

    public void ChangePicture(int picID)
    {
        ClientUtils.ChangeCardPicture(Picture, picID);
    }

    void Awake()
    {
        CardBloomDefaultSortingOrder = CardBloomSG.sortingOrder;
        MainBoardDefaultSortingOrder = MainBoardSG.sortingOrder;
        CardShadowDefaultSortingOrder = CardShadowSG.sortingOrder;
        PictureDefaultSortingOrder = Picture.sortingOrder;
    }

    private int CardBloomDefaultSortingOrder;
    private int MainBoardDefaultSortingOrder;
    private int CardShadowDefaultSortingOrder;
    private int PictureDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        CardBloomSG.sortingOrder = cardSortingIndex * 50 + CardBloomDefaultSortingOrder;
        MainBoardSG.sortingOrder = cardSortingIndex * 50 + MainBoardDefaultSortingOrder;
        CardShadowSG.sortingOrder = cardSortingIndex * 50 + CardShadowDefaultSortingOrder;
        Picture.sortingOrder = cardSortingIndex * 50 + PictureDefaultSortingOrder;
    }
}