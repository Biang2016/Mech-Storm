using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardPropertyFormRow_InputField : CardPropertyFormRow
{
    [SerializeField] private InputField InputField;
    [SerializeField] private Text PlaceHolderText;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        LanguageManager.Instance.UnregisterTextKey(PlaceHolderText);
    }

    protected override void Child_Initialize(string labelStrKey, UnityAction<string> onValueChangeAction, List<string> dropdownOptionList)
    {
        LanguageManager.Instance.RegisterTextKeys(new List<ValueTuple<Text, string>>
        {
            (PlaceHolderText, labelStrKey),
        });

        InputField.onValueChanged.RemoveAllListeners();
        InputField.onValueChanged.AddListener(delegate { onValueChangeAction(InputField.text); });
    }

    protected override void SetValue(string value_str)
    {
        InputField.text = value_str;
    }
}