using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelPropertyForm_ShopItem : PoolObject
{
    [SerializeField] private Button DeleteButton;
    [SerializeField] private Button EditButton;
    [SerializeField] private Image Pic;
    [SerializeField] private Image EditBG;
    [SerializeField] private Text TypeLabel;
    [SerializeField] private Text TypeText;
    [SerializeField] private Text NameLabel;
    [SerializeField] private Text NameText;
    [SerializeField] private Text PriceLabel;
    [SerializeField] private Text PriceText;
    [SerializeField] private Text ProbabilityLabel;
    [SerializeField] private Text ProbabilityText;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        Cur_ShopItem = null;
        EditButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.RemoveAllListeners();
    }

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKey(TypeLabel, "LevelEditorPanel_ShopItemTypeLabel");
        LanguageManager.Instance.RegisterTextKey(NameLabel, "LevelEditorPanel_ShopItemNameLabel");
        LanguageManager.Instance.RegisterTextKey(PriceLabel, "LevelEditorPanel_ShopItemPriceLabel");
        LanguageManager.Instance.RegisterTextKey(ProbabilityLabel, "LevelEditorPanel_ShopItemProbabilityLabel");
    }

    public ShopItem Cur_ShopItem;

    private bool isEdit;

    public bool IsEdit
    {
        get { return isEdit; }
        set
        {
            isEdit = value;
            EditBG.enabled = isEdit;
        }
    }

    public void Initialize(ShopItem shopItem, UnityAction onEditButtonClick, UnityAction onDeleteButtonClick)
    {
        Cur_ShopItem = shopItem;
        EditButton.onClick.RemoveAllListeners();
        EditButton.onClick.AddListener(onEditButtonClick);
        DeleteButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.AddListener(onDeleteButtonClick);

        ClientUtils.ChangeImagePicture(Pic, shopItem.PicID);
        PriceText.text = shopItem.Price.ToString();
        ProbabilityText.text = shopItem.Probability.ToString();
        TypeText.text = shopItem.ShopItemType.ToString();
        switch (shopItem)
        {
            case ShopItem_Card sic:
            {
                NameText.text = sic.Name;
                break;
            }
            case ShopItem_Budget sib:
            {
                NameText.text = string.Format(LanguageManager.Instance.GetText("LevelEditorPanel_BudgetValueText"), sib.Budget);
                break;
            }
            case ShopItem_LifeUpperLimit silu:
            {
                NameText.text = string.Format(LanguageManager.Instance.GetText("LevelEditorPanel_LifeUpperLimitValueText"), silu.LifeUpperLimit);
                break;
            }
            case ShopItem_EnergyUpperLimit sieu:
            {
                NameText.text = string.Format(LanguageManager.Instance.GetText("LevelEditorPanel_EnergyUpperLimitValueText"), sieu.EnergyUpperLimit);
                break;
            }
        }
    }

    public void Refresh()
    {
        PriceText.text = Cur_ShopItem.Price.ToString();
        ProbabilityText.text = Cur_ShopItem.Probability.ToString();
        TypeText.text = Cur_ShopItem.ShopItemType.ToString();

        switch (Cur_ShopItem)
        {
            case ShopItem_Card sic:
            {
                ClientUtils.ChangeImagePicture(Pic, sic.PicID);
                NameText.text = sic.Name;
                break;
            }
            case ShopItem_Budget sib:
            {
                NameText.text = string.Format(LanguageManager.Instance.GetText("LevelEditorPanel_BudgetValueText"), sib.Budget);
                break;
            }
            case ShopItem_LifeUpperLimit silu:
            {
                NameText.text = string.Format(LanguageManager.Instance.GetText("LevelEditorPanel_LifeUpperLimitValueText"), silu.LifeUpperLimit);
                break;
            }
            case ShopItem_EnergyUpperLimit sieu:
            {
                NameText.text = string.Format(LanguageManager.Instance.GetText("LevelEditorPanel_EnergyUpperLimitValueText"), sieu.EnergyUpperLimit);
                break;
            }
        }
    }

    public void OnLanguageChange()
    {
        Refresh();
    }
}