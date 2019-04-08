using UnityEngine;

public class CardBasicComponent : CardComponentBase
{
    [SerializeField] private MeshRenderer CardBloom;
    [SerializeField] private MeshRenderer MainBoard;
    [SerializeField] private MeshRenderer CardShadow;
    [SerializeField] private SpriteRenderer Picture;

    public void SetMainBoardColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(MainBoard, color, intensity);
    }

    public void SetCardBloomColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(CardBloom, color, intensity);
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
        CardBloomDefaultSortingOrder = CardBloom.sortingOrder;
        MainBoardDefaultSortingOrder = MainBoard.sortingOrder;
        CardShadowDefaultSortingOrder = CardShadow.sortingOrder;
        PictureDefaultSortingOrder = Picture.sortingOrder;
    }

    private int CardBloomDefaultSortingOrder;
    private int MainBoardDefaultSortingOrder;
    private int CardShadowDefaultSortingOrder;
    private int PictureDefaultSortingOrder;

    protected override void SetSortingIndexOfCard(int cardSortingIndex)
    {
        CardBloom.sortingOrder = cardSortingIndex * 50 + CardBloomDefaultSortingOrder;
        MainBoard.sortingOrder = cardSortingIndex * 50 + MainBoardDefaultSortingOrder;
        CardShadow.sortingOrder = cardSortingIndex * 50 + CardShadowDefaultSortingOrder;
        Picture.sortingOrder = cardSortingIndex * 50 + PictureDefaultSortingOrder;
    }
}