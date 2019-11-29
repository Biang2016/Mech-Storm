using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelEditorPanel_CardRow : PropertyFormRow
{
    [SerializeField] private Image CardBGImage;

    [SerializeField] private Button MoveUpButton;
    [SerializeField] private Button MoveDownButton;
    [SerializeField] private Button RemoveButton;

    [SerializeField] private Text CardName;
    [SerializeField] private Image CardImage;
    [SerializeField] private StarsGroup StarsGroup;

    public CardInfo_Base CardInfo;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        MoveUpButton.onClick.RemoveAllListeners();
        MoveDownButton.onClick.RemoveAllListeners();
        RemoveButton.onClick.RemoveAllListeners();
    }

    public void Initialize(CardInfo_Base cardInfo, UnityAction<int> onMoveUp, UnityAction<int> onMoveDown, UnityAction<int> onRemove)
    {
        CardInfo = cardInfo;
        CardName.text = CardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()];
        Color cardColor = ClientUtils.HTMLColorToColor(CardInfo.GetCardColor());
        CardBGImage.color = cardColor;
        ClientUtils.ChangeImagePicture(CardImage, CardInfo.BaseInfo.PictureID);
        StarsGroup.SetStarNumber(CardInfo.UpgradeInfo.CardLevel, CardInfo.UpgradeInfo.CardLevelMax);

        MoveUpButton.onClick.RemoveAllListeners();
        MoveDownButton.onClick.RemoveAllListeners();
        RemoveButton.onClick.RemoveAllListeners();

        MoveUpButton.onClick.AddListener(delegate { onMoveUp(CardInfo.CardID); });
        MoveDownButton.onClick.AddListener(delegate { onMoveDown(CardInfo.CardID); });
        RemoveButton.onClick.AddListener(delegate { onRemove(CardInfo.CardID); });
    }

    protected override void SetValue(string value_str, bool forceChange = false)
    {
    }

    public void OnLanguageChange()
    {
        CardName.text = CardInfo.BaseInfo.CardNames[LanguageManager.Instance.GetCurrentLanguage()];
    }
}