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

    void Awake()
    {
        SpritePresets[(int) CardStatTypes.HeroMech] = ClientUtils.GetPicByID(0);
        SpritePresets[(int) CardStatTypes.SoldierMech] = ClientUtils.GetPicByID(58);
        SpritePresets[(int) CardStatTypes.Equip] = ClientUtils.GetPicByID(103);
        SpritePresets[(int) CardStatTypes.Spell] = ClientUtils.GetPicByID(612);
        SpritePresets[(int) CardStatTypes.Energy] = ClientUtils.GetPicByID(701);
    }

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