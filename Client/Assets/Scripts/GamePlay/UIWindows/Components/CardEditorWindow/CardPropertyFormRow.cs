using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class CardPropertyFormRow : PoolObject
{
    [SerializeField] private Text Label;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        LanguageManager.Instance.UnregisterTextKey(Label);
    }

    public enum CardPropertyFormRowType
    {
        InputFiled,
        Dropdown,
        Toggle
    }

    public static CardPropertyFormRow BaseInitialize(CardPropertyFormRowType type, Transform parent, string labelStrKey, UnityAction<string> onValueChangeAction, out UnityAction<string> setValue, List<string> dropdownOptionList = null)
    {
        CardPropertyFormRow res = null;
        switch (type)
        {
            case CardPropertyFormRowType.InputFiled:
            {
                CardPropertyFormRow_InputField row = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CardPropertyFormRow_InputField].AllocateGameObject<CardPropertyFormRow_InputField>(parent);
                row.Initialize(labelStrKey, onValueChangeAction, out setValue);
                res = row;
                break;
            }
            case CardPropertyFormRowType.Dropdown:
            {
                CardPropertyFormRow_Dropdown row = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CardPropertyFormRow_Dropdown].AllocateGameObject<CardPropertyFormRow_Dropdown>(parent);
                row.Initialize(labelStrKey, onValueChangeAction, out setValue, dropdownOptionList);
                res = row;
                break;
            }
            case CardPropertyFormRowType.Toggle:
            {
                CardPropertyFormRow_Toggle row = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CardPropertyFormRow_Toggle].AllocateGameObject<CardPropertyFormRow_Toggle>(parent);
                row.Initialize(labelStrKey, onValueChangeAction, out setValue);
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

    protected void Initialize(string labelStrKey, UnityAction<string> onValueChangeAction, out UnityAction<string> setValue, List<string> dropdownOptionList = null)
    {
        LanguageManager.Instance.RegisterTextKeys(new List<ValueTuple<Text, string>>
        {
            (Label, labelStrKey),
        });

        Child_Initialize(labelStrKey, onValueChangeAction, dropdownOptionList);
        setValue = SetValue;
    }

    protected virtual void Child_Initialize(string labelStrKey, UnityAction<string> onValueChangeAction, List<string> dropdownOptionList = null)
    {
    }

    protected abstract void SetValue(string value_str);
}