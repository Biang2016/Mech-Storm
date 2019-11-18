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

    protected override void SetValue(string value_str, bool forceChange = false)
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

        SetDefaultCoin(Cur_GamePlaySettings.DefaultCoin.ToString(), false);
        SetDefaultLife(Cur_GamePlaySettings.DefaultLife.ToString(), false);
        SetDefaultLifeMax(Cur_GamePlaySettings.DefaultLifeMax.ToString(), false);
        SetDefaultLifeMin(Cur_GamePlaySettings.DefaultLifeMin.ToString(), false);
        SetDefaultEnergy(Cur_GamePlaySettings.DefaultEnergy.ToString(), false);
        SetDefaultEnergyMax(Cur_GamePlaySettings.DefaultEnergyMax.ToString(), false);
        SetDefaultDrawCardNum(Cur_GamePlaySettings.DefaultDrawCardNum.ToString(), false);
        SetMinDrawCardNum(Cur_GamePlaySettings.MinDrawCardNum.ToString(), false);
        SetMaxDrawCardNum(Cur_GamePlaySettings.MaxDrawCardNum.ToString(), false);
    }

    private PropertyFormRow GeneralizeRow(PropertyFormRow.CardPropertyFormRowType type, string labelKey, UnityAction<string> onValueChange, out UnityAction<string, bool> setValue, List<string> dropdownOptionList = null, UnityAction<string> onButtonClick = null)
    {
        PropertyFormRow cpfr = BaseInitialize(type, RowContainer, labelKey, onValueChange, out setValue, dropdownOptionList, onButtonClick);
        PropertyFormRows.Add(cpfr);
        return cpfr;
    }

    private UnityAction<string, bool> SetDefaultCoin;

    private void OnDefaultCoinChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value <= 0)
            {
                SetDefaultCoin(1500.ToString(), false);
            }
            else
            {
                Cur_GamePlaySettings.DefaultCoin = value;
            }
        }
        else
        {
            SetDefaultCoin(1500.ToString(), false);
        }
    }

    private UnityAction<string, bool> SetDefaultLife;

    private void OnDefaultLifeChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < Cur_GamePlaySettings.DefaultLifeMin)
            {
                SetDefaultLife(Cur_GamePlaySettings.DefaultLifeMin.ToString(), false);
            }
            else if (value > Cur_GamePlaySettings.DefaultLifeMax)
            {
                SetDefaultLife(Cur_GamePlaySettings.DefaultLifeMax.ToString(), false);
            }
            else
            {
                Cur_GamePlaySettings.DefaultLife = value;
            }
        }
        else
        {
            SetDefaultLife(Cur_GamePlaySettings.DefaultLifeMin.ToString(), false);
        }
    }

    private UnityAction<string, bool> SetDefaultLifeMax;

    private void OnDefaultLifeMaxChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < Cur_GamePlaySettings.DefaultLifeMin)
            {
                SetDefaultLifeMax(Cur_GamePlaySettings.DefaultLifeMin.ToString(), false);
            }
            else if (value <= GamePlaySettings.SystemMaxLife)
            {
                Cur_GamePlaySettings.DefaultLifeMax = value;
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(string.Format(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_EnemyLifeMax"), GamePlaySettings.SystemMaxLife), 0, 0.5f);
                SetDefaultLifeMax(GamePlaySettings.SystemMaxLife.ToString(), false);
            }
        }
        else
        {
            SetDefaultLifeMax(Cur_GamePlaySettings.DefaultLifeMin.ToString(), false);
        }
    }

    private UnityAction<string, bool> SetDefaultLifeMin;

    private void OnDefaultLifeMinChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < GamePlaySettings.SystemMinLife)
            {
                SetDefaultLifeMin(GamePlaySettings.SystemMinLife.ToString(), false);
            }
            else if (value > Cur_GamePlaySettings.DefaultLifeMax)
            {
                SetDefaultLifeMin(Cur_GamePlaySettings.DefaultLifeMax.ToString(), false);
            }
            else
            {
                Cur_GamePlaySettings.DefaultLifeMin = value;
            }
        }
        else
        {
            SetDefaultLifeMin(GamePlaySettings.SystemMinLife.ToString(), false);
        }
    }

    private UnityAction<string, bool> SetDefaultEnergy;

    private void OnDefaultEnergyChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < 0)
            {
                SetDefaultEnergy(0.ToString(), false);
            }
            else if (value <= GamePlaySettings.SystemMaxEnergy)
            {
                if (value > Cur_GamePlaySettings.DefaultEnergyMax)
                {
                    SetDefaultEnergy(Cur_GamePlaySettings.DefaultEnergyMax.ToString(), false);
                }
                else
                {
                    Cur_GamePlaySettings.DefaultEnergy = value;
                }
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(string.Format(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_EnemyEnergyMax"), GamePlaySettings.SystemMaxEnergy), 0, 0.5f);
                SetDefaultEnergy(GamePlaySettings.SystemMaxEnergy.ToString(), false);
            }
        }
        else
        {
            SetDefaultEnergy(0.ToString(), false);
        }
    }

    private UnityAction<string, bool> SetDefaultEnergyMax;

    private void OnDefaultEnergyMaxChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < 0)
            {
                SetDefaultEnergyMax(0.ToString(), false);
            }
            else if (value <= GamePlaySettings.SystemMaxEnergy)
            {
                Cur_GamePlaySettings.DefaultEnergyMax = value;
            }
            else
            {
                NoticeManager.Instance.ShowInfoPanelCenter(string.Format(LanguageManager.Instance.GetText("Notice_LevelEditorPanel_EnemyEnergyMax"), GamePlaySettings.SystemMaxEnergy), 0, 0.5f);
                SetDefaultEnergyMax(GamePlaySettings.SystemMaxEnergy.ToString(), false);
            }
        }
        else
        {
            SetDefaultEnergyMax(GamePlaySettings.SystemMaxEnergy.ToString(), false);
        }
    }

    private UnityAction<string, bool> SetDefaultDrawCardNum;

    private void OnDefaultDrawCardNumChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < Cur_GamePlaySettings.MinDrawCardNum)
            {
                SetDefaultDrawCardNum(Cur_GamePlaySettings.MinDrawCardNum.ToString(), false);
            }
            else if (value > Cur_GamePlaySettings.MaxDrawCardNum)
            {
                SetDefaultDrawCardNum(Cur_GamePlaySettings.MaxDrawCardNum.ToString(), false);
            }
            else
            {
                Cur_GamePlaySettings.DefaultDrawCardNum = value;
            }
        }
        else
        {
            SetDefaultDrawCardNum(Cur_GamePlaySettings.MinDrawCardNum.ToString(), false);
        }
    }

    private UnityAction<string, bool> SetMinDrawCardNum;

    private void OnMinDrawCardNumChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < GamePlaySettings.SystemMinDrawCardNum)
            {
                SetMinDrawCardNum(GamePlaySettings.SystemMinDrawCardNum.ToString(), false);
            }
            else if (value > Cur_GamePlaySettings.MaxDrawCardNum)
            {
                SetMinDrawCardNum(Cur_GamePlaySettings.MaxDrawCardNum.ToString(), false);
            }
            else
            {
                Cur_GamePlaySettings.MinDrawCardNum = value;
            }
        }
        else
        {
            SetMinDrawCardNum(GamePlaySettings.SystemMinDrawCardNum.ToString(), false);
        }
    }

    private UnityAction<string, bool> SetMaxDrawCardNum;

    private void OnMaxDrawCardNumChange(string value_str)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (value < Cur_GamePlaySettings.MinDrawCardNum)
            {
                SetMaxDrawCardNum(Cur_GamePlaySettings.MinDrawCardNum.ToString(), false);
            }
            else if (value > GamePlaySettings.SystemMaxDrawCardNum)
            {
                SetMaxDrawCardNum(GamePlaySettings.SystemMaxDrawCardNum.ToString(), false);
            }
            else
            {
                Cur_GamePlaySettings.MaxDrawCardNum = value;
            }
        }
        else
        {
            SetMaxDrawCardNum(Cur_GamePlaySettings.MinDrawCardNum.ToString(), false);
        }
    }
}