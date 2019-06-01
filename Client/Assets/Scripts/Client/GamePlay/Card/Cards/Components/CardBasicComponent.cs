using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CardBasicComponent : CardComponentBase
{
    [SerializeField] private MeshRenderer CardBloom;
    [SerializeField] private MeshRenderer MainBoard;
    [SerializeField] private MeshRenderer CardShadow;
    [SerializeField] private SortingGroup CardBloomSG;
    [SerializeField] private SortingGroup MainBoardSG;
    [SerializeField] private SortingGroup CardShadowSG;
    [SerializeField] private SpriteRenderer Picture;

    public Color MainBoardColor { get; set; }

    public float MainBoardColorIntensity { get; set; }

    public void SetMainBoardColor(Color color, float intensity)
    {
        ClientUtils.ChangeEmissionColor(MainBoard, color, intensity);
        MainBoardColor = color;
        MainBoardColorIntensity = intensity;
    }

    public Color CardBloomColor { get; set; }

    public float CardBloomColorIntensity { get; set; }

    public void SetCardBloomColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(CardBloom, color, intensity);
        CardBloomColor = color;
        CardBloomColorIntensity = intensity;
    }

    public void SetBloomShow(bool isShow)
    {
        CardBloom.gameObject.SetActive(isShow);
    }

    public Color PictureColor { get; set; }

    public float PictureColorIntensity { get; set; }

    public void SetPictureColor(Color color, float intensity)
    {
        ClientUtils.ChangeColor(Picture, color, intensity);
        PictureColor = color;
        PictureColorIntensity = intensity;
    }

    public void ChangePicture(int picID)
    {
        ClientUtils.ChangeImagePicture(Picture, picID);
    }

    void Awake()
    {
        MainBoardColorIntensity = 1.5f;
        CardBloomColorIntensity = 1.5f;
        MainBoardColor = MainBoard.sharedMaterial.GetColor("_EmissionColor");
        CardBloomColor = CardBloom.sharedMaterial.GetColor("_EmissionColor");
        PictureColor = Picture.color;
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