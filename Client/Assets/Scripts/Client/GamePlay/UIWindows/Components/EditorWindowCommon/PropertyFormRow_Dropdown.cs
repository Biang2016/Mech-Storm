using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PropertyFormRow_Dropdown : PropertyFormRow
{
    [SerializeField] private Dropdown Dropdown;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
    }

    protected override void Child_Initialize(string labelStrKey, UnityAction<string> onValueChangeAction, List<string> dropdownOptionList, UnityAction<string> onButtonClick = null)
    {
        Dropdown.options.Clear();
        foreach (string option in dropdownOptionList)
        {
            Dropdown.options.Add(new Dropdown.OptionData(option));
        }

        Dropdown.onValueChanged.RemoveAllListeners();
        Dropdown.value = -1;
        Dropdown.RefreshShownValue();
        Dropdown.onValueChanged.AddListener(delegate { onValueChangeAction(Dropdown.options[Dropdown.value].text); });
    }

    protected override void SetValue(string value_str)
    {
        int setValue = -1;
        for (int i = 0; i < Dropdown.options.Count; i++)
        {
            if (value_str.Equals(Dropdown.options[i].text))
            {
                setValue = i;
                break;
            }
        }

        if (setValue != -1)
        {
            Dropdown.value = setValue;
            Dropdown.RefreshShownValue();
        }
    }

    public override void SetReadOnly(bool isReadOnly)
    {
        base.SetReadOnly(isReadOnly);
        Dropdown.interactable = !isReadOnly;
    }
}