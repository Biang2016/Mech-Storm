using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoryPropertyForm_GamePlaySettings : PropertyFormRow
{
    [SerializeField] private Transform RowContainer;

    void Awake()
    {
    }

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        foreach (PropertyFormRow pfr in PropertyFormRows)
        {
            pfr.PoolRecycle();
        }

        PropertyFormRows.Clear();
    }

    protected override void SetValue(string value_str)
    {
    }

    private List<PropertyFormRow> PropertyFormRows = new List<PropertyFormRow>();

    private GamePlaySettings Cur_GamePlaySettings;

    public void Initialize()
    {
        foreach (PropertyFormRow pfr in PropertyFormRows)
        {
            pfr.PoolRecycle();
        }

        PropertyFormRows.Clear();

        PropertyFormRow Row_DrawCardPerRound = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorPanel_DrawCardPerRoundLabelText", OnDrawCardPerRoundChange, out SetDrawCardPerRound);
        PropertyFormRow Row_DefaultCoin = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorPanel_DefaultCoinLabelText", OnDefaultCoinChange, out SetDefaultCoin);
        PropertyFormRow Row_DefaultLife = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorPanel_DefaultLifeLabelText", OnDefaultLifeChange, out SetDefaultLife);
        PropertyFormRow Row_DefaultLifeMax = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorPanel_DefaultLifeMaxLabelText", OnDefaultLifeMaxChange, out SetDefaultLifeMax);
        PropertyFormRow Row_DefaultLifeMin = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorPanel_DefaultLifeMinLabelText", OnDefaultLifeMinChange, out SetDefaultLifeMin);
        PropertyFormRow Row_DefaultEnergy = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorPanel_DefaultEnergyLabelText", OnDefaultEnergyChange, out SetDefaultEnergy);
        PropertyFormRow Row_DefaultEnergyMax = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorPanel_DefaultEnergyMaxLabelText", OnDefaultEnergyMaxChange, out SetDefaultEnergyMax);
        PropertyFormRow Row_DefaultDrawCardNum = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorPanel_DefaultDrawCardNumLabelText", OnDefaultDrawCardNumChange, out SetDefaultDrawCardNum);
        PropertyFormRow Row_MinDrawCardNum = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorPanel_MinDrawCardNumLabelText", OnMinDrawCardNumChange, out SetMinDrawCardNum);
        PropertyFormRow Row_MaxDrawCardNum = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorPanel_MaxDrawCardNumLabelText", OnMaxDrawCardNumChange, out SetMaxDrawCardNum);
    }

    public void SetGamePlaySettings(GamePlaySettings gamePlaySettings)
    {
        Cur_GamePlaySettings = gamePlaySettings;

        SetDrawCardPerRound(Cur_GamePlaySettings.DrawCardPerRound.ToString());
        SetDefaultCoin(Cur_GamePlaySettings.DefaultCoin.ToString());
        SetDefaultLife(Cur_GamePlaySettings.DefaultLife.ToString());
        SetDefaultLifeMax(Cur_GamePlaySettings.DefaultLifeMax.ToString());
        SetDefaultLifeMin(Cur_GamePlaySettings.DefaultLifeMin.ToString());
        SetDefaultEnergy(Cur_GamePlaySettings.DefaultEnergy.ToString());
        SetDefaultEnergyMax(Cur_GamePlaySettings.DefaultEnergyMax.ToString());
        SetDefaultDrawCardNum(Cur_GamePlaySettings.DefaultDrawCardNum.ToString());
        SetMinDrawCardNum(Cur_GamePlaySettings.MinDrawCardNum.ToString());
        SetMaxDrawCardNum(Cur_GamePlaySettings.MaxDrawCardNum.ToString());
    }

    private PropertyFormRow GeneralizeRow(PropertyFormRow.CardPropertyFormRowType type, string labelKey, UnityAction<string> onValueChange, out UnityAction<string> setValue, List<string> dropdownOptionList = null, UnityAction<string> onButtonClick = null)
    {
        PropertyFormRow cpfr = BaseInitialize(type, RowContainer, labelKey, onValueChange, out setValue, dropdownOptionList, onButtonClick);
        PropertyFormRows.Add(cpfr);
        return cpfr;
    }

    private UnityAction<string> SetDrawCardPerRound;

    private void OnDrawCardPerRoundChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (Cur_GamePlaySettings != null)
            {
                Cur_GamePlaySettings.DefaultDrawCardNum = value;
            }
        }
    }

    private UnityAction<string> SetDefaultCoin;

    private void OnDefaultCoinChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (Cur_GamePlaySettings != null)
            {
                Cur_GamePlaySettings.DefaultCoin = value;
            }
        }
    }

    private UnityAction<string> SetDefaultLife;

    private void OnDefaultLifeChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (Cur_GamePlaySettings != null)
            {
                Cur_GamePlaySettings.DefaultLife = value;
            }
        }
    }

    private UnityAction<string> SetDefaultLifeMax;

    private void OnDefaultLifeMaxChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (Cur_GamePlaySettings != null)
            {
                Cur_GamePlaySettings.DefaultLifeMax = value;
            }
        }
    }

    private UnityAction<string> SetDefaultLifeMin;

    private void OnDefaultLifeMinChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (Cur_GamePlaySettings != null)
            {
                Cur_GamePlaySettings.DefaultLifeMin = value;
            }
        }
    }

    private UnityAction<string> SetDefaultEnergy;

    private void OnDefaultEnergyChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (Cur_GamePlaySettings != null)
            {
                Cur_GamePlaySettings.DefaultEnergy = value;
            }
        }
    }

    private UnityAction<string> SetDefaultEnergyMax;

    private void OnDefaultEnergyMaxChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (Cur_GamePlaySettings != null)
            {
                Cur_GamePlaySettings.DefaultLifeMax = value;
            }
        }
    }

    private UnityAction<string> SetDefaultDrawCardNum;

    private void OnDefaultDrawCardNumChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (Cur_GamePlaySettings != null)
            {
                Cur_GamePlaySettings.DefaultDrawCardNum = value;
            }
        }
    }

    private UnityAction<string> SetMinDrawCardNum;

    private void OnMinDrawCardNumChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (Cur_GamePlaySettings != null)
            {
                Cur_GamePlaySettings.MinDrawCardNum = value;
            }
        }
    }

    private UnityAction<string> SetMaxDrawCardNum;

    private void OnMaxDrawCardNumChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (Cur_GamePlaySettings != null)
            {
                Cur_GamePlaySettings.MaxDrawCardNum = value;
            }
        }
    }
}