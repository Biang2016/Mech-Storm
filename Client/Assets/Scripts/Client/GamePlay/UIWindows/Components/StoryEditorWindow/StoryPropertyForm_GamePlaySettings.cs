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

    private UnityAction<string> SetDefaultCoin;

    private void OnDefaultCoinChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value <= 0)
            {
                SetDefaultCoin(1500.ToString());
            }
            else
            {
                Cur_GamePlaySettings.DefaultCoin = value;
            }
        }
        else
        {
            SetDefaultCoin(1500.ToString());
        }
    }

    private UnityAction<string> SetDefaultLife;

    private void OnDefaultLifeChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < Cur_GamePlaySettings.DefaultLifeMin)
            {
                SetDefaultLife(Cur_GamePlaySettings.DefaultLifeMin.ToString());
            }
            else if (value > Cur_GamePlaySettings.DefaultLifeMax)
            {
                SetDefaultLife(Cur_GamePlaySettings.DefaultLifeMax.ToString());
            }
            else
            {
                Cur_GamePlaySettings.DefaultLife = value;
            }
        }
        else
        {
            SetDefaultLife(Cur_GamePlaySettings.DefaultLifeMin.ToString());
        }
    }

    private UnityAction<string> SetDefaultLifeMax;

    private void OnDefaultLifeMaxChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < Cur_GamePlaySettings.DefaultLifeMin)
            {
                SetDefaultLifeMax(Cur_GamePlaySettings.DefaultLifeMin.ToString());
            }
            else if (value <= GamePlaySettings.SystemMaxLife)
            {
                Cur_GamePlaySettings.DefaultLifeMax = value;
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(string.Format(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_EnemyLifeMax"), GamePlaySettings.SystemMaxLife), 0, 0.5f);
                SetDefaultLifeMax(GamePlaySettings.SystemMaxLife.ToString());
            }
        }
        else
        {
            SetDefaultLifeMax(Cur_GamePlaySettings.DefaultLifeMin.ToString());
        }
    }

    private UnityAction<string> SetDefaultLifeMin;

    private void OnDefaultLifeMinChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < GamePlaySettings.SystemMinLife)
            {
                SetDefaultLifeMin(GamePlaySettings.SystemMinLife.ToString());
            }
            else if (value > Cur_GamePlaySettings.DefaultLifeMax)
            {
                SetDefaultLifeMin(Cur_GamePlaySettings.DefaultLifeMax.ToString());
            }
            else
            {
                Cur_GamePlaySettings.DefaultLifeMin = value;
            }
        }
        else
        {
            SetDefaultLifeMin(GamePlaySettings.SystemMinLife.ToString());
        }
    }

    private UnityAction<string> SetDefaultEnergy;

    private void OnDefaultEnergyChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < 0)
            {
                SetDefaultEnergy(0.ToString());
            }
            else if (value <= GamePlaySettings.SystemMaxEnergy)
            {
                if (value > Cur_GamePlaySettings.DefaultEnergyMax)
                {
                    SetDefaultEnergy(Cur_GamePlaySettings.DefaultEnergyMax.ToString());
                }
                else
                {
                    Cur_GamePlaySettings.DefaultEnergy = value;
                }
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(string.Format(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_EnemyEnergyMax"), GamePlaySettings.SystemMaxEnergy), 0, 0.5f);
                SetDefaultEnergy(GamePlaySettings.SystemMaxEnergy.ToString());
            }
        }
        else
        {
            SetDefaultEnergy(0.ToString());
        }
    }

    private UnityAction<string> SetDefaultEnergyMax;

    private void OnDefaultEnergyMaxChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < 0)
            {
                SetDefaultEnergyMax(0.ToString());
            }
            else if (value <= GamePlaySettings.SystemMaxEnergy)
            {
                Cur_GamePlaySettings.DefaultEnergyMax = value;
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(string.Format(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_EnemyEnergyMax"), GamePlaySettings.SystemMaxEnergy), 0, 0.5f);
                SetDefaultEnergyMax(GamePlaySettings.SystemMaxEnergy.ToString());
            }
        }
        else
        {
            SetDefaultEnergyMax(GamePlaySettings.SystemMaxEnergy.ToString());
        }
    }

    private UnityAction<string> SetDefaultDrawCardNum;

    private void OnDefaultDrawCardNumChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < Cur_GamePlaySettings.MinDrawCardNum)
            {
                SetDefaultDrawCardNum(Cur_GamePlaySettings.MinDrawCardNum.ToString());
            }
            else if (value > Cur_GamePlaySettings.MaxDrawCardNum)
            {
                SetDefaultDrawCardNum(Cur_GamePlaySettings.MaxDrawCardNum.ToString());
            }
            else
            {
                Cur_GamePlaySettings.DefaultDrawCardNum = value;
            }
        }
        else
        {
            SetDefaultDrawCardNum(Cur_GamePlaySettings.MinDrawCardNum.ToString());
        }
    }

    private UnityAction<string> SetMinDrawCardNum;

    private void OnMinDrawCardNumChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < GamePlaySettings.SystemMinDrawCardNum)
            {
                SetMinDrawCardNum(GamePlaySettings.SystemMinDrawCardNum.ToString());
            }
            else if (value > Cur_GamePlaySettings.MaxDrawCardNum)
            {
                SetMinDrawCardNum(Cur_GamePlaySettings.MaxDrawCardNum.ToString());
            }
            else
            {
                Cur_GamePlaySettings.MinDrawCardNum = value;
            }
        }
        else
        {
            SetMinDrawCardNum(GamePlaySettings.SystemMinDrawCardNum.ToString());
        }
    }

    private UnityAction<string> SetMaxDrawCardNum;

    private void OnMaxDrawCardNumChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < Cur_GamePlaySettings.MinDrawCardNum)
            {
                SetMaxDrawCardNum(Cur_GamePlaySettings.MinDrawCardNum.ToString());
            }
            else if (value > GamePlaySettings.SystemMaxDrawCardNum)
            {
                SetMaxDrawCardNum(GamePlaySettings.SystemMaxDrawCardNum.ToString());
            }
            else
            {
                Cur_GamePlaySettings.MaxDrawCardNum = value;
            }
        }
        else
        {
            SetMaxDrawCardNum(Cur_GamePlaySettings.MinDrawCardNum.ToString());
        }
    }
}