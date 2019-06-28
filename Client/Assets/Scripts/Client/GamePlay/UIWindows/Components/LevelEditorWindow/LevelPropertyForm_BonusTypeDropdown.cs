using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelPropertyForm_BonusTypeDropdown : PoolObject
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
        List<string> bonusTypes = Enum.GetNames(typeof(Bonus.BonusTypes)).ToList();
        TypeDropdown.ClearOptions();
        TypeDropdown.options.Add(new Dropdown.OptionData(" "));
        TypeDropdown.AddOptions(bonusTypes);

        LanguageManager.Instance.RegisterTextKey(TypeLabel, "LevelEditorPanel_BonusTypeLabel");
    }

    private UnityAction<Bonus.BonusTypes> OnAddBonusType;

    public void Initialize(UnityAction onDeleteButtonClick, UnityAction<Bonus.BonusTypes> onAddBonusType)
    {
        TypeDropdown.onValueChanged.RemoveAllListeners();
        TypeDropdown.value = 0;
        TypeDropdown.onValueChanged.AddListener(delegate(int value) { OnAddBonusType((Bonus.BonusTypes) (value - 1)); });

        OnAddBonusType = onAddBonusType;

        DeleteButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.AddListener(onDeleteButtonClick);
    }
}