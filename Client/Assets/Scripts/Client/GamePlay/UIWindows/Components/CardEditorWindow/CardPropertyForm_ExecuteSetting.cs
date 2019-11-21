using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardPropertyForm_ExecuteSetting : PoolObject
{
    [SerializeField] private Transform ExecuteRowContainer;
    private List<string> triggerTimeTypeList = new List<string>();
    [SerializeField] private Dropdown ScriptExecuteSettingTypeDropdown;
    [SerializeField] private PropertyFormRow_Dropdown PFR_ScriptExecuteSettingType;

    void Awake()
    {
        IEnumerable<SideEffectExecute.TriggerTime> types_TriggerTime = Enum.GetValues(typeof(SideEffectExecute.TriggerTime)) as IEnumerable<SideEffectExecute.TriggerTime>;
        foreach (SideEffectExecute.TriggerTime triggerTime in types_TriggerTime)
        {
            triggerTimeTypeList.Add(triggerTime.ToString());
        }
    }

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        foreach (PropertyFormRow cpfr in CardPropertyFormRows)
        {
            cpfr.PoolRecycle();
        }

        CardPropertyFormRows.Clear();
    }

    private List<PropertyFormRow> CardPropertyFormRows = new List<PropertyFormRow>();

    public void Initialize(SideEffectExecute see, UnityAction onRefreshText, bool isReadOnly, bool isExecuteSettingTypeChanged)
    {
        foreach (PropertyFormRow cpfr in CardPropertyFormRows)
        {
            cpfr.PoolRecycle();
        }

        CardPropertyFormRows.Clear();

        bool isScriptExecuteSetting = see.M_ExecuteSetting is ScriptExecuteSettingBase;
        PFR_ScriptExecuteSettingType.gameObject.SetActive(isScriptExecuteSetting);
        ScriptExecuteSettingBase sesb = null;
        if (isScriptExecuteSetting)
        {
            sesb = (ScriptExecuteSettingBase) see.M_ExecuteSetting;
            ScriptExecuteSettingTypeDropdown.options.Clear();
            foreach (string option in AllScriptExecuteSettings.ScriptExecuteSettingsNameDict.Keys.ToList())
            {
                ScriptExecuteSettingTypeDropdown.options.Add(new Dropdown.OptionData(option));
            }

            ScriptExecuteSettingTypeDropdown.onValueChanged.RemoveAllListeners();
            SetScriptExecuteSettingType(sesb.Name);
            ScriptExecuteSettingTypeDropdown.onValueChanged.AddListener(delegate(int index)
            {
                string scriptExecuteSettingName = ScriptExecuteSettingTypeDropdown.options[index].text;
                ScriptExecuteSettingBase newSESB = AllScriptExecuteSettings.GetScriptExecuteSetting(scriptExecuteSettingName);
                if (see != null)
                {
                    see.M_ExecuteSetting = newSESB;
                }

                Initialize(see, onRefreshText, isReadOnly, false);
                onRefreshText();
                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ExecuteRowContainer));
                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
            });

            CardEditorPanel_Params.GenerateParamRows(null, sesb.M_SideEffectParam, onRefreshText, delegate { Initialize(see, onRefreshText, isReadOnly, false); }, ExecuteRowContainer, CardPropertyFormRows, null, delegate
            {
                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ExecuteRowContainer));
                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
            });
        }

        PropertyFormRow cpfr_TriggerRange = null;
        UnityAction<string, bool> _setValueTriggerRange = null;
        PropertyFormRow cpfr_TriggerTime = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.Dropdown, ExecuteRowContainer, "CardEditorPanel_TriggerTime",
            delegate(string value_str)
            {
                SideEffectExecute.TriggerTime value = (SideEffectExecute.TriggerTime) Enum.Parse(typeof(SideEffectExecute.TriggerTime), value_str);
                see.M_ExecuteSetting.TriggerTime = value;
                List<string> trList = SideEffectExecute.GetTriggerRangeListByTriggerTime(see.M_ExecuteSetting.TriggerTime);
                ((PropertyFormRow_Dropdown) cpfr_TriggerRange).RefreshDropdownOptionList(trList);
                if (see.ExecuteSettingType == SideEffectExecute.ExecuteSettingTypes.Scripts || see.ExecuteSettingType == SideEffectExecute.ExecuteSettingTypes.Others) _setValueTriggerRange(trList[0], true);
                onRefreshText?.Invoke();
            },
            out UnityAction<string, bool> _setValueTriggerTime, sesb != null ? ScriptExecuteSettingBase.HashSetTriggerTimeToListString(sesb.ValidTriggerTimes) : triggerTimeTypeList);
        CardPropertyFormRows.Add(cpfr_TriggerTime);
        cpfr_TriggerTime.SetReadOnly(isReadOnly);

        cpfr_TriggerRange = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.Dropdown, ExecuteRowContainer, "CardEditorPanel_TriggerRange",
            delegate(string value_str)
            {
                SideEffectExecute.TriggerRange value = (SideEffectExecute.TriggerRange) Enum.Parse(typeof(SideEffectExecute.TriggerRange), value_str);
                see.M_ExecuteSetting.TriggerRange = value;
                onRefreshText?.Invoke();
            },
            out _setValueTriggerRange, SideEffectExecute.GetTriggerRangeListByTriggerTime(see.M_ExecuteSetting.TriggerTime));
        CardPropertyFormRows.Add(cpfr_TriggerRange);
        cpfr_TriggerRange.SetReadOnly(isReadOnly);
        ((PropertyFormRow_Dropdown) cpfr_TriggerRange).RefreshDropdownOptionList(SideEffectExecute.GetTriggerRangeListByTriggerTime(see.M_ExecuteSetting.TriggerTime));

        PropertyFormRow cpfr_TriggerTimes = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.InputField, ExecuteRowContainer, "CardEditorPanel_TriggerTimes",
            delegate(string value_str)
            {
                if (int.TryParse(value_str, out int value))
                {
                    see.M_ExecuteSetting.TriggerTimes = value;
                    onRefreshText?.Invoke();
                }
            },
            out UnityAction<string, bool> _setValueTriggerTimes);
        CardPropertyFormRows.Add(cpfr_TriggerTimes);
        cpfr_TriggerTimes.SetReadOnly(sesb != null ? sesb.LockedTriggerTimes != ScriptExecuteSettingBase.UNLOCKED_EXECUTESETTING_TIMES : isReadOnly);

        PropertyFormRow cpfr_TriggerDelayTimes = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.InputField, ExecuteRowContainer, "CardEditorPanel_TriggerDelayTimes",
            delegate(string value_str)
            {
                if (int.TryParse(value_str, out int value))
                {
                    see.M_ExecuteSetting.TriggerDelayTimes = value;
                    onRefreshText?.Invoke();
                }
            },
            out UnityAction<string, bool> _setValueTriggerDelayTimes);
        CardPropertyFormRows.Add(cpfr_TriggerDelayTimes);
        cpfr_TriggerDelayTimes.SetReadOnly(sesb != null ? sesb.LockedTriggerDelayTimes != ScriptExecuteSettingBase.UNLOCKED_EXECUTESETTING_TIMES : isReadOnly);

        PropertyFormRow cpfr_RemoveTriggerRange = null;
        UnityAction<string, bool> _setValueRemoveTriggerRange = null;
        PropertyFormRow cpfr_RemoveTriggerTime = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.Dropdown, ExecuteRowContainer, "CardEditorPanel_RemoveTriggerTime",
            delegate(string value_str)
            {
                SideEffectExecute.TriggerTime value = (SideEffectExecute.TriggerTime) Enum.Parse(typeof(SideEffectExecute.TriggerTime), value_str);
                see.M_ExecuteSetting.RemoveTriggerTime = value;
                List<string> trList = SideEffectExecute.GetTriggerRangeListByTriggerTime(see.M_ExecuteSetting.RemoveTriggerTime);
                ((PropertyFormRow_Dropdown) cpfr_RemoveTriggerRange).RefreshDropdownOptionList(trList);
                if (see.ExecuteSettingType == SideEffectExecute.ExecuteSettingTypes.Scripts || see.ExecuteSettingType == SideEffectExecute.ExecuteSettingTypes.Others) _setValueRemoveTriggerRange(trList[0], true);
                onRefreshText?.Invoke();
            },
            out UnityAction<string, bool> _setValueRemoveTriggerTime, sesb != null ? ScriptExecuteSettingBase.HashSetTriggerTimeToListString(sesb.ValidRemoveTriggerTimes) : triggerTimeTypeList);
        CardPropertyFormRows.Add(cpfr_RemoveTriggerTime);
        cpfr_RemoveTriggerTime.SetReadOnly(isReadOnly);

        cpfr_RemoveTriggerRange = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.Dropdown, ExecuteRowContainer, "CardEditorPanel_RemoveTriggerRange",
            delegate(string value_str)
            {
                SideEffectExecute.TriggerRange value = (SideEffectExecute.TriggerRange) Enum.Parse(typeof(SideEffectExecute.TriggerRange), value_str);
                see.M_ExecuteSetting.RemoveTriggerRange = value;
                onRefreshText?.Invoke();
            },
            out _setValueRemoveTriggerRange, SideEffectExecute.GetTriggerRangeListByTriggerTime(see.M_ExecuteSetting.RemoveTriggerTime));
        CardPropertyFormRows.Add(cpfr_RemoveTriggerRange);
        cpfr_RemoveTriggerRange.SetReadOnly(isReadOnly);
        ((PropertyFormRow_Dropdown) cpfr_RemoveTriggerRange).RefreshDropdownOptionList(SideEffectExecute.GetTriggerRangeListByTriggerTime(see.M_ExecuteSetting.RemoveTriggerTime));

        PropertyFormRow cpfr_RemoveTriggerTimes = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.InputField, ExecuteRowContainer, "CardEditorPanel_RemoveTriggerTimes",
            delegate(string value_str)
            {
                if (int.TryParse(value_str, out int value))
                {
                    see.M_ExecuteSetting.RemoveTriggerTimes = value;
                    onRefreshText?.Invoke();
                }
            },
            out UnityAction<string, bool> _setValueRemoveTriggerTimes);
        CardPropertyFormRows.Add(cpfr_RemoveTriggerTimes);
        cpfr_RemoveTriggerTimes.SetReadOnly(sesb != null ? sesb.LockedRemoveTriggerTimes != ScriptExecuteSettingBase.UNLOCKED_EXECUTESETTING_TIMES : isReadOnly);

        PropertyFormRow cpfr_RemoveTriggerDelayTimes = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.InputField, ExecuteRowContainer, "CardEditorPanel_RemoveTriggerDelayTimes",
            delegate(string value_str)
            {
                if (int.TryParse(value_str, out int value))
                {
                    see.M_ExecuteSetting.RemoveTriggerDelayTimes = value;
                    onRefreshText?.Invoke();
                }
            },
            out UnityAction<string, bool> _setValueRemoveTriggerDelayTimes);
        CardPropertyFormRows.Add(cpfr_RemoveTriggerDelayTimes);
        cpfr_RemoveTriggerDelayTimes.SetReadOnly(sesb != null ? sesb.LockedRemoveTriggerDelayTimes != ScriptExecuteSettingBase.UNLOCKED_EXECUTESETTING_TIMES : isReadOnly);

        _setValueTriggerTime(sesb != null ? (sesb.ValidTriggerTimes.Count != 0 ? sesb.ValidTriggerTimes.ToList()[0].ToString() : SideEffectExecute.TriggerTime.None.ToString()) : see.M_ExecuteSetting.TriggerTime.ToString(), isExecuteSettingTypeChanged);
        _setValueTriggerRange(see.M_ExecuteSetting.TriggerRange.ToString(), isExecuteSettingTypeChanged);
        _setValueTriggerTimes(sesb != null ? (sesb.LockedTriggerTimes != ScriptExecuteSettingBase.UNLOCKED_EXECUTESETTING_TIMES ? sesb.LockedTriggerTimes.ToString() : see.M_ExecuteSetting.TriggerTimes.ToString()) : see.M_ExecuteSetting.TriggerTimes.ToString(), isExecuteSettingTypeChanged);
        _setValueTriggerDelayTimes(sesb != null ? (sesb.LockedTriggerDelayTimes != ScriptExecuteSettingBase.UNLOCKED_EXECUTESETTING_TIMES ? sesb.LockedTriggerDelayTimes.ToString() : see.M_ExecuteSetting.TriggerDelayTimes.ToString()) : see.M_ExecuteSetting.TriggerDelayTimes.ToString(), isExecuteSettingTypeChanged);
        _setValueRemoveTriggerTime(sesb != null ? (sesb.ValidRemoveTriggerTimes.Count != 0 ? sesb.ValidRemoveTriggerTimes.ToList()[0].ToString() : SideEffectExecute.TriggerTime.None.ToString()) : see.M_ExecuteSetting.RemoveTriggerTime.ToString(), isExecuteSettingTypeChanged);
        _setValueRemoveTriggerRange(see.M_ExecuteSetting.RemoveTriggerRange.ToString(), isExecuteSettingTypeChanged);
        _setValueRemoveTriggerTimes(sesb != null ? (sesb.LockedRemoveTriggerTimes != ScriptExecuteSettingBase.UNLOCKED_EXECUTESETTING_TIMES ? sesb.LockedRemoveTriggerTimes.ToString() : see.M_ExecuteSetting.RemoveTriggerTimes.ToString()) : see.M_ExecuteSetting.RemoveTriggerTimes.ToString(), isExecuteSettingTypeChanged);
        _setValueRemoveTriggerDelayTimes(sesb != null ? (sesb.LockedRemoveTriggerDelayTimes != ScriptExecuteSettingBase.UNLOCKED_EXECUTESETTING_TIMES ? sesb.LockedRemoveTriggerDelayTimes.ToString() : see.M_ExecuteSetting.RemoveTriggerDelayTimes.ToString()) : see.M_ExecuteSetting.RemoveTriggerDelayTimes.ToString(), isExecuteSettingTypeChanged);
    }

    private void SetScriptExecuteSettingType(string value_str)
    {
        int setValue = -1;
        for (int i = 0; i < ScriptExecuteSettingTypeDropdown.options.Count; i++)
        {
            if (value_str.Equals(ScriptExecuteSettingTypeDropdown.options[i].text))
            {
                setValue = i;
                break;
            }
        }

        if (setValue != -1)
        {
            ScriptExecuteSettingTypeDropdown.value = setValue;
            ScriptExecuteSettingTypeDropdown.RefreshShownValue();
        }
    }
}