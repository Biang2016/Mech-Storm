using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PropertyFormRow_InputField : PropertyFormRow
{
    [SerializeField] private InputField InputField;
    [SerializeField] private Text PlaceHolderText;
    [SerializeField] private Button Button;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        LanguageManager.Instance.UnregisterText(PlaceHolderText);
    }

    protected override void Child_Initialize(string labelStrKey, UnityAction<string> onValueChangeAction, List<string> dropdownOptionList, UnityAction<string> onButtonClick)
    {
        LanguageManager.Instance.RegisterTextKeys(new List<ValueTuple<Text, string>>
        {
            (PlaceHolderText, labelStrKey),
        });

        InputField.onValueChanged.RemoveAllListeners();
        InputField.onValueChanged.AddListener(delegate { onValueChangeAction(InputField.text); });

        Button.gameObject.SetActive(onButtonClick != null);

        Button.onClick.RemoveAllListeners();
        if (onButtonClick != null) Button.onClick.AddListener(delegate { onButtonClick(InputField.text); });
    }

    protected override void SetValue(string value_str)
    {
        InputField.text = value_str;
    }

    public override void SetReadOnly(bool isReadOnly)
    {
        base.SetReadOnly(isReadOnly);
        InputField.interactable = !isReadOnly;
    }
}