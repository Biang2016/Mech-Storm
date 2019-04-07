using UnityEngine;

public class CardBasicComponent : MonoBehaviour
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
}