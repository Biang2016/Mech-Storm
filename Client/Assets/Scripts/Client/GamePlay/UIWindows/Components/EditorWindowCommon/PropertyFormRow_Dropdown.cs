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

    public UnityAction<string> OnValueChangeAction;

    protected override void Child_Initialize(string labelStrKey, UnityAction<string> onValueChangeAction, List<string> dropdownOptionList, UnityAction<string> onButtonClick = null)
    {
        OnValueChangeAction = onValueChangeAction;
        Dropdown.options.Clear();
        foreach (string option in dropdownOptionList)
        {
            Dropdown.options.Add(new Dropdown.OptionData(option));
        }

        Dropdown.onValueChanged.RemoveAllListeners();
        Dropdown.value = -1;
        Dropdown.RefreshShownValue();
        Dropdown.onValueChanged.AddListener(delegate { OnValueChangeAction(Dropdown.options[Dropdown.value].text); });
    }

    public void RefreshDropdownOptionList(List<string> dropdownOptionList)
    {
        Dropdown.options.Clear();
        foreach (string option in dropdownOptionList)
        {
            Dropdown.options.Add(new Dropdown.OptionData(option));
        }

        Dropdown.value = 0;
        Dropdown.RefreshShownValue();
    }

    protected override void SetValue(string value_str, bool forceChange = false)
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
            if (Dropdown.value == setValue)
            {
                if (forceChange)
                {
                    OnValueChangeAction(value_str);
                }
            }
            else
            {
                Dropdown.value = setValue;
            }

            Dropdown.RefreshShownValue();
        }
    }

    public override void SetReadOnly(bool isReadOnly)
    {
        base.SetReadOnly(isReadOnly);
        Dropdown.interactable = !isReadOnly;
    }
}