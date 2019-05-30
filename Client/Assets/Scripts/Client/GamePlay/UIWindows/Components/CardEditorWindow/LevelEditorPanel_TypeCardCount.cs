using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelEditorPanel_TypeCardCount : PoolObject
{
    private LevelEditorPanel_TypeCardCount()
    {
    }

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        Button.onClick.RemoveAllListeners();
        LanguageManager.Instance.UnregisterText(CardTypeText);
    }

    [SerializeField] private Button Button;
    [SerializeField] private Image SelectedImage;
    [SerializeField] private Image PicImage;
    [SerializeField] private Text CardTypeText;
    [SerializeField] private Text CountText;

    [SerializeField] private Sprite[] SpritePresets;

    internal CardStatTypes CardStatType;

    public void Initialize(CardStatTypes cardStatType, int count, UnityAction onClick)
    {
        CardStatType = cardStatType;
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(onClick);
        PicImage.sprite = SpritePresets[(int) cardStatType];
        LanguageManager.Instance.RegisterTextKey(CardTypeText, "CardType_" + cardStatType);
        CountText.text = "x" + count;
    }

    private bool isSelected;

    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            SelectedImage.enabled = value;
        }
    }
}