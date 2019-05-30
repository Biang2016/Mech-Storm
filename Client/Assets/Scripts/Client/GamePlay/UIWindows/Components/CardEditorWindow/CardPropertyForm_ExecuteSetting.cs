using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardPropertyForm_ExecuteSetting : PoolObject
{
    [SerializeField] private Transform ExecuteRowContainer;
    private List<string> triggerTimeTypeList = new List<string>();
    private List<string> triggerRangeTypeList = new List<string>();

    void Awake()
    {
        IEnumerable<SideEffectExecute.TriggerTime> types_TriggerTime = Enum.GetValues(typeof(SideEffectExecute.TriggerTime)) as IEnumerable<SideEffectExecute.TriggerTime>;
        foreach (SideEffectExecute.TriggerTime triggerTime in types_TriggerTime)
        {
            triggerTimeTypeList.Add(triggerTime.ToString());
        }

        IEnumerable<SideEffectExecute.TriggerRange> types_TriggerRange = Enum.GetValues(typeof(SideEffectExecute.TriggerRange)) as IEnumerable<SideEffectExecute.TriggerRange>;
        foreach (SideEffectExecute.TriggerRange triggerRange in types_TriggerRange)
        {
            triggerRangeTypeList.Add(triggerRange.ToString());
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

    public void Initialize(SideEffectExecute see, UnityAction onRefreshText, bool isReadOnly)
    {
        foreach (PropertyFormRow cpfr in CardPropertyFormRows)
        {
            cpfr.PoolRecycle();
        }

        CardPropertyFormRows.Clear();

        PropertyFormRow cpfr_TriggerTime = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.Dropdown, ExecuteRowContainer, "CardEditorPanel_TriggerTime",
            delegate(string value_str)
            {
                SideEffectExecute.TriggerTime value = (SideEffectExecute.TriggerTime) Enum.Parse(typeof(SideEffectExecute.TriggerTime), value_str);
                see.M_ExecuteSetting.TriggerTime = value;
                onRefreshText?.Invoke();
            },
            out UnityAction<string> _setValueTriggerTime, triggerTimeTypeList);
        _setValueTriggerTime(see.M_ExecuteSetting.TriggerTime.ToString());
        CardPropertyFormRows.Add(cpfr_TriggerTime);
        cpfr_TriggerTime.SetReadOnly(isReadOnly);

        PropertyFormRow cpfr_TriggerRange = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.Dropdown, ExecuteRowContainer, "CardEditorPanel_TriggerRange",
            delegate(string value_str)
            {
                SideEffectExecute.TriggerRange value = (SideEffectExecute.TriggerRange) Enum.Parse(typeof(SideEffectExecute.TriggerRange), value_str);
                see.M_ExecuteSetting.TriggerRange = value;
                onRefreshText?.Invoke();
            },
            out UnityAction<string> _setValueTriggerRange, triggerRangeTypeList);
        _setValueTriggerRange(see.M_ExecuteSetting.TriggerRange.ToString());
        CardPropertyFormRows.Add(cpfr_TriggerRange);
        cpfr_TriggerRange.SetReadOnly(isReadOnly);

        PropertyFormRow cpfr_TriggerDelayTimes = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.InputField, ExecuteRowContainer, "CardEditorPanel_TriggerDelayTimes",
            delegate(string value_str)
            {
                if (int.TryParse(value_str, out int value))
                {
                    onRefreshText?.Invoke();
                }
            },
            out UnityAction<string> _setValueTriggerDelayTimes);
        _setValueTriggerDelayTimes(see.M_ExecuteSetting.TriggerDelayTimes.ToString());
        CardPropertyFormRows.Add(cpfr_TriggerDelayTimes);
        cpfr_TriggerDelayTimes.SetReadOnly(isReadOnly);

        PropertyFormRow cpfr_TriggerTimes = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.InputField, ExecuteRowContainer, "CardEditorPanel_TriggerTimes",
            delegate(string value_str)
            {
                if (int.TryParse(value_str, out int value))
                {
                    see.M_ExecuteSetting.TriggerTimes = value;
                    onRefreshText?.Invoke();
                }
            },
            out UnityAction<string> _setValueTriggerTimes);
        _setValueTriggerTimes(see.M_ExecuteSetting.TriggerTimes.ToString());
        CardPropertyFormRows.Add(cpfr_TriggerTimes);
        cpfr_TriggerTimes.SetReadOnly(isReadOnly);

        PropertyFormRow cpfr_RemoveTriggerTime = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.Dropdown, ExecuteRowContainer, "CardEditorPanel_RemoveTriggerTime",
            delegate(string value_str)
            {
                SideEffectExecute.TriggerTime value = (SideEffectExecute.TriggerTime) Enum.Parse(typeof(SideEffectExecute.TriggerTime), value_str);
                see.M_ExecuteSetting.RemoveTriggerTime = value;
                onRefreshText?.Invoke();
            },
            out UnityAction<string> _setValueRemoveTriggerTime, triggerTimeTypeList);
        _setValueRemoveTriggerTime(see.M_ExecuteSetting.RemoveTriggerTime.ToString());
        CardPropertyFormRows.Add(cpfr_RemoveTriggerTime);
        cpfr_RemoveTriggerTime.SetReadOnly(isReadOnly);

        PropertyFormRow cpfr_RemoveTriggerRange = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.Dropdown, ExecuteRowContainer, "CardEditorPanel_RemoveTriggerRange",
            delegate(string value_str)
            {
                SideEffectExecute.TriggerRange value = (SideEffectExecute.TriggerRange) Enum.Parse(typeof(SideEffectExecute.TriggerRange), value_str);
                see.M_ExecuteSetting.RemoveTriggerRange = value;
                onRefreshText?.Invoke();
            },
            out UnityAction<string> _setValueRemoveTriggerRange, triggerRangeTypeList);
        _setValueRemoveTriggerRange(see.M_ExecuteSetting.RemoveTriggerRange.ToString());
        CardPropertyFormRows.Add(cpfr_RemoveTriggerRange);
        cpfr_RemoveTriggerRange.SetReadOnly(isReadOnly);

        PropertyFormRow cpfr_RemoveTriggerTimes = PropertyFormRow.BaseInitialize(PropertyFormRow.CardPropertyFormRowType.InputField, ExecuteRowContainer, "CardEditorPanel_RemoveTriggerTimes",
            delegate(string value_str)
            {
                if (int.TryParse(value_str, out int value))
                {
                    see.M_ExecuteSetting.RemoveTriggerTimes = value;
                    onRefreshText?.Invoke();
                }
            },
            out UnityAction<string> _setValueRemoveTriggerTimes);
        _setValueRemoveTriggerTimes(see.M_ExecuteSetting.RemoveTriggerTimes.ToString());
        CardPropertyFormRows.Add(cpfr_RemoveTriggerTimes);
        cpfr_RemoveTriggerTimes.SetReadOnly(isReadOnly);
    }
}