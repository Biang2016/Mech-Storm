using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PropertyFormRow_Toggle : PropertyFormRow
{
    [SerializeField] private Toggle Toggle;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
    }

    protected override void Child_Initialize(string labelStrKey, UnityAction<string> onValueChangeAction, List<string> dropdownOptionList = null, UnityAction<string> onButtonClick = null)
    {
        Toggle.onValueChanged.RemoveAllListeners();
        Toggle.onValueChanged.AddListener(delegate { onValueChangeAction(Toggle.isOn.ToString()); });
    }

    protected override void SetValue(string value_str)
    {
        Toggle.isOn = value_str.Equals("True");
    }

    public override void SetReadOnly(bool isReadOnly)
    {
        base.SetReadOnly(isReadOnly);
        Toggle.interactable = !isReadOnly;
    }
}