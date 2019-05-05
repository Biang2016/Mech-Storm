using UnityEngine;
using UnityEngine.UI;

public class CardPreviewButton : PoolObject
{
    [SerializeField] private Button Button;
    [SerializeField] private Image Image;
    [SerializeField] private Text CardIDText;
    [SerializeField] private Image HideImage;
    [SerializeField] private Text HideText;
    [SerializeField] private Image TempImage;
    [SerializeField] private Text TempText;
    [SerializeField] private Image CurEditBorder;

    [SerializeField] private StarsGroup CardPreviewButtonStarsGroup;

    public void Awake()
    {
        LanguageManager.Instance.RegisterTextKey(HideText, "CardEditorWindow_CardPreviewButtonHideText");
        LanguageManager.Instance.RegisterTextKey(TempText, "CardEditorWindow_CardPreviewButtonTempText");
        IsEdit = false;
    }

    private bool isEdit;

    public bool IsEdit
    {
        get { return isEdit; }
        set
        {
            CurEditBorder.enabled = value;
            isEdit = value;
        }
    }

    public void Initialize(CardInfo_Base ci, OnButtonClickDelegate onClick)
    {
        IsEdit = false;
        Button.onClick.RemoveAllListeners();
        CardIDText.text = string.Format("{0:000}", ci.CardID);
        ClientUtils.ChangeCardPicture(Image, ci.BaseInfo.PictureID);
        Button.onClick.AddListener(delegate { onClick(); });
        HideImage.gameObject.SetActive(ci.BaseInfo.IsHide);
        TempImage.gameObject.SetActive(ci.BaseInfo.IsTemp);
        CardPreviewButtonStarsGroup.SetStarNumber(ci.UpgradeInfo.CardLevel, ci.UpgradeInfo.CardLevelMax);
    }

    public delegate void OnButtonClickDelegate();
}