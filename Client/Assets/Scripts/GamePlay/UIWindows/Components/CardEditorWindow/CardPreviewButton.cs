using UnityEngine;
using UnityEngine.UI;

public class CardPreviewButton : PoolObject
{
    [SerializeField] private Button Button;
    [SerializeField] private Image Image;
    [SerializeField] private Text CardIDText;

    public void Initialize(CardInfo_Base ci, OnButtonClickDelegate onClick)
    {
        Button.onClick.RemoveAllListeners();
        CardIDText.text = string.Format("{0:000}", ci.CardID);
        ClientUtils.ChangeCardPicture(Image, ci.BaseInfo.PictureID);
        Button.onClick.AddListener(delegate { onClick(); });
    }

    public delegate void OnButtonClickDelegate();
}