using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelPropertyForm_ShopItemTypeDropdown : PoolObject
{
    [SerializeField] private Button DeleteButton;
    [SerializeField] private Image Pic;
    [SerializeField] private Text TypeLabel;
    [SerializeField] private Dropdown TypeDropdown;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        DeleteButton.onClick.RemoveAllListeners();
    }

    void Awake()
    {
        ClientUtils.ChangeImagePicture(Pic, (int) AllCards.SpecialPicIDs.Shop);
        List<string> shopItemTypes = Enum.GetNames(typeof(ShopItem.ShopItemTypes)).ToList();
        TypeDropdown.ClearOptions();
        TypeDropdown.options.Add(new Dropdown.OptionData(" "));
        TypeDropdown.AddOptions(shopItemTypes);

        LanguageManager.Instance.RegisterTextKey(TypeLabel, "LevelEditorPanel_ShopItemTypeLabel");
    }

    private UnityAction<ShopItem.ShopItemTypes> OnAddShopItem;

    public void Initialize(UnityAction onDeleteButtonClick, UnityAction<ShopItem.ShopItemTypes> onAddShopItem)
    {
        TypeDropdown.onValueChanged.RemoveAllListeners();
        TypeDropdown.value = 0;
        TypeDropdown.onValueChanged.AddListener(delegate(int value) { OnAddShopItem((ShopItem.ShopItemTypes) (value - 1)); });

        OnAddShopItem = onAddShopItem;

        DeleteButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.AddListener(onDeleteButtonClick);
    }
}