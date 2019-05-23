using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class PropertyFormRow : PoolObject
{
    [SerializeField] private Text Label;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        LanguageManager.Instance.UnregisterText(Label);
    }

    public enum CardPropertyFormRowType
    {
        InputField,
        Dropdown,
        Toggle,
        TwoToggle
    }

    public static PropertyFormRow BaseInitialize(CardPropertyFormRowType type, Transform parent, string labelStrKey, UnityAction<string> onValueChangeAction, out UnityAction<string> setValue, List<string> dropdownOptionList = null, UnityAction<string> onButtonClick = null)
    {
        PropertyFormRow res = null;
        switch (type)
        {
            case CardPropertyFormRowType.InputField:
            {
                PropertyFormRow_InputField row = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.PropertyFormRow_InputField].AllocateGameObject<PropertyFormRow_InputField>(parent);
                row.Initialize(labelStrKey, onValueChangeAction: onValueChangeAction, setValue: out setValue, onButtonClick: onButtonClick);
                res = row;
                break;
            }
            case CardPropertyFormRowType.Dropdown:
            {
                PropertyFormRow_Dropdown row = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.PropertyFormRow_Dropdown].AllocateGameObject<PropertyFormRow_Dropdown>(parent);
                row.Initialize(labelStrKey, onValueChangeAction: onValueChangeAction, setValue: out setValue, dropdownOptionList: dropdownOptionList, onButtonClick: onButtonClick);
                res = row;
                break;
            }
            case CardPropertyFormRowType.Toggle:
            {
                PropertyFormRow_Toggle row = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.PropertyFormRow_Toggle].AllocateGameObject<PropertyFormRow_Toggle>(parent);
                row.Initialize(labelStrKey, onValueChangeAction: onValueChangeAction, setValue: out setValue, onButtonClick: onButtonClick);
                res = row;
                break;
            }
            case CardPropertyFormRowType.TwoToggle:
            {
                PropertyFormRow_TwoToggleRow row = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.PropertyFormRow_TwoToggleRow].AllocateGameObject<PropertyFormRow_TwoToggleRow>(parent);
                row.Initialize(labelStrKey, null, out setValue);
                res = row;
                break;
            }
            default:
            {
                setValue = null;
                break;
            }
        }

        return res;
    }

    internal string LabelStrKey = "";

    protected void Initialize(string labelStrKey, UnityAction<string> onValueChangeAction, out UnityAction<string> setValue, List<string> dropdownOptionList = null, UnityAction<string> onButtonClick = null)
    {
        LabelStrKey = labelStrKey;
        if (LanguageManager.Instance.GetText(labelStrKey) != null)
        {
            LanguageManager.Instance.RegisterTextKeys(new List<ValueTuple<Text, string>>
            {
                (Label, labelStrKey),
            });
        }
        else
        {
            if (Label)
            {
                Label.text = labelStrKey;
            }
        }

        Child_Initialize(labelStrKey, onValueChangeAction, dropdownOptionList, onButtonClick);
        setValue = SetValue;
    }

    public virtual void SetReadOnly(bool isReadOnly)
    {
    }

    protected virtual void Child_Initialize(string labelStrKey, UnityAction<string> onValueChangeAction, List<string> dropdownOptionList = null, UnityAction<string> onButtonClick = null)
    {
    }

    protected abstract void SetValue(string value_str);
}