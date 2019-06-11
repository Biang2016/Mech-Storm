using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelPropertyForm_ShopItem : PoolObject
{
    [SerializeField] private Button DeleteButton;
    [SerializeField] private Button EditButton;
    [SerializeField] private Image Pic;
    [SerializeField] private Image EditBG;
    [SerializeField] private Text NameLabel;
    [SerializeField] private Text NameText;
    [SerializeField] private Text PriceLabel;
    [SerializeField] private Text PriceText;
    [SerializeField] private Text TypeLabel;
    [SerializeField] private Text TypeText;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        Cur_ShopItem = null;
        EditButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.RemoveAllListeners();
    }

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKey(NameLabel, "LevelEditorPanel_ShopItemNameLabel");
        LanguageManager.Instance.RegisterTextKey(PriceLabel, "LevelEditorPanel_ShopItemPriceLabel");
        LanguageManager.Instance.RegisterTextKey(TypeLabel, "LevelEditorPanel_ShopItemTypeLabel");
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
        switch (shopItem)
        {
            case ShopItem_Card sic:
            {
                ClientUtils.ChangeImagePicture(Pic, sic.PicID);
                NameText.text = sic.Name;
                PriceText.text = sic.Price.ToString();
                break;
            }
        }
    }

    public void Refresh()
    {
        switch (Cur_ShopItem)
        {
            case ShopItem_Card sic:
            {
                ClientUtils.ChangeImagePicture(Pic, sic.PicID);
                NameText.text = sic.Name;
                PriceText.text = sic.Price.ToString();
                TypeText.text = sic.ShopItemType.ToString();
                break;
            }
        }
    }

    public void OnLanguageChange()
    {
        NameText.text = Cur_ShopItem.Name;
    }
}