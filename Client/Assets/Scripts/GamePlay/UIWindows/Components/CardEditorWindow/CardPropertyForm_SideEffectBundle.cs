using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardPropertyForm_SideEffectBundle : PoolObject
{
    [SerializeField] private Image BGImage;
    [SerializeField] private Text SideEffectBundleText;
    [SerializeField] private Transform SideEffectRowContainer;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
    }

    private List<CardPropertyFormRow> CardPropertyFormRows = new List<CardPropertyFormRow>();

    private SideEffectBundle cur_SideEffectBundle;
    private UnityAction OnSideEffectRefresh;

    public void Initialize(string labelStrKey, string color, SideEffectBundle sideEffectBundle, UnityAction onSideEffectRefresh)
    {
        BGImage.color = ClientUtils.HTMLColorToColor(color);
        LanguageManager.Instance.UnregisterTextKey(SideEffectBundleText);
        LanguageManager.Instance.RegisterTextKey(SideEffectBundleText, labelStrKey);

        foreach (CardPropertyFormRow cpfr in CardPropertyFormRows)
        {
            cpfr.PoolRecycle();
        }

        CardPropertyFormRows.Clear();

        cur_SideEffectBundle = sideEffectBundle;
        OnSideEffectRefresh = onSideEffectRefresh;

        IEnumerable<SideEffectBundle.TriggerTime> types_TriggerTime = Enum.GetValues(typeof(SideEffectBundle.TriggerTime)) as IEnumerable<SideEffectBundle.TriggerTime>;
        List<string> triggerTimeTypeList = new List<string>();
        foreach (SideEffectBundle.TriggerTime triggerTime in types_TriggerTime)
        {
            triggerTimeTypeList.Add(triggerTime.ToString());
        }

        IEnumerable<SideEffectBundle.TriggerRange> types_TriggerRange = Enum.GetValues(typeof(SideEffectBundle.TriggerRange)) as IEnumerable<SideEffectBundle.TriggerRange>;
        List<string> triggerRangeTypeList = new List<string>();
        foreach (SideEffectBundle.TriggerRange triggerRange in types_TriggerRange)
        {
            triggerRangeTypeList.Add(triggerRange.ToString());
        }

        CardPropertyFormRow cpfr_TriggerTime = CardPropertyFormRow.BaseInitialize(CardPropertyFormRow.CardPropertyFormRowType.Dropdown, SideEffectRowContainer, "CardEditorWindow_TriggerTime", OnTriggerTimeChange, out SetTriggerTime, triggerTimeTypeList, null);
        CardPropertyFormRows.Add(cpfr_TriggerTime);
        CardPropertyFormRow cpfr_TriggerRange = CardPropertyFormRow.BaseInitialize(CardPropertyFormRow.CardPropertyFormRowType.Dropdown, SideEffectRowContainer, "CardEditorWindow_TriggerRange", OnTriggerRangeChange, out SetTriggerRange, triggerRangeTypeList, null);
        CardPropertyFormRows.Add(cpfr_TriggerRange);
        CardPropertyFormRow cpfr_TriggerDelayTimes = CardPropertyFormRow.BaseInitialize(CardPropertyFormRow.CardPropertyFormRowType.InputFiled, SideEffectRowContainer, "CardEditorWindow_TriggerDelayTimes", OnTriggerDelayTimesChange, out SetTriggerDelayTimes, null, null);
        CardPropertyFormRows.Add(cpfr_TriggerDelayTimes);
        CardPropertyFormRow cpfr_TriggerTimes = CardPropertyFormRow.BaseInitialize(CardPropertyFormRow.CardPropertyFormRowType.InputFiled, SideEffectRowContainer, "CardEditorWindow_TriggerTimes", OnTriggerTimesChange, out SetTriggerTimes, null, null);
        CardPropertyFormRows.Add(cpfr_TriggerTimes);
        CardPropertyFormRow cpfr_RemoveTriggerTime = CardPropertyFormRow.BaseInitialize(CardPropertyFormRow.CardPropertyFormRowType.Dropdown, SideEffectRowContainer, "CardEditorWindow_RemoveTriggerTime", OnRemoveTriggerTimeChange, out SetRemoveTriggerTime, triggerTimeTypeList, null);
        CardPropertyFormRows.Add(cpfr_RemoveTriggerTime);
        CardPropertyFormRow cpfr_RemoveTriggerRange = CardPropertyFormRow.BaseInitialize(CardPropertyFormRow.CardPropertyFormRowType.Dropdown, SideEffectRowContainer, "CardEditorWindow_RemoveTriggerRange", OnRemoveTriggerRangeChange, out SetRemoveTriggerRange, triggerRangeTypeList, null);
        CardPropertyFormRows.Add(cpfr_RemoveTriggerRange);
        CardPropertyFormRow cpfr_RemoveTriggerTimes = CardPropertyFormRow.BaseInitialize(CardPropertyFormRow.CardPropertyFormRowType.InputFiled, SideEffectRowContainer, "CardEditorWindow_RemoveTriggerTimes", OnRemoveTriggerTimesChange, out SetRemoveTriggerTimes, null, null);
        CardPropertyFormRows.Add(cpfr_RemoveTriggerTimes);

        if (sideEffectBundle != null)
        {
            if (sideEffectBundle.SideEffectExecutes.Count > 0)
            {
                SideEffectBundle.TriggerTime TriggerTime = sideEffectBundle.SideEffectExecutes[0].TriggerTime; //when to trigger
                SetTriggerTime(TriggerTime.ToString());
                SideEffectBundle.TriggerRange TriggerRange = sideEffectBundle.SideEffectExecutes[0].TriggerRange; //which range of events can trigger this effect
                SetTriggerRange(TriggerRange.ToString());
                int TriggerDelayTimes = sideEffectBundle.SideEffectExecutes[0].TriggerDelayTimes; //how many times we need to trigger it before it can realy trigger
                SetTriggerDelayTimes(TriggerDelayTimes.ToString());
                int TriggerTimes = sideEffectBundle.SideEffectExecutes[0].TriggerTimes; //the max times we can trigger it.
                SetTriggerTimes(TriggerTimes.ToString());
                SideEffectBundle.TriggerTime RemoveTriggerTime = sideEffectBundle.SideEffectExecutes[0].RemoveTriggerTime; //when to remove this effect/decrease the remove time of this effect
                SetRemoveTriggerTime(RemoveTriggerTime.ToString());
                SideEffectBundle.TriggerRange RemoveTriggerRange = sideEffectBundle.SideEffectExecutes[0].RemoveTriggerRange; //which range of events can remove this effect
                SetRemoveTriggerRange(RemoveTriggerRange.ToString());
                int RemoveTriggerTimes = sideEffectBundle.SideEffectExecutes[0].RemoveTriggerTimes; //how many times of remove before we can remove the effect permenantly. (usually used in buffs)
                SetRemoveTriggerTimes(RemoveTriggerTimes.ToString());
            }

            foreach (SideEffectExecute see in sideEffectBundle.SideEffectExecutes)
            {
                SideEffectBase seb = see.SideEffectBase;
            }
        }
    }

    private UnityAction<string> SetTriggerTime;

    private void OnTriggerTimeChange(string value_str)
    {
        SideEffectBundle.TriggerTime value = (SideEffectBundle.TriggerTime) Enum.Parse(typeof(SideEffectBundle.TriggerTime), value_str);
        if (cur_SideEffectBundle != null)
        {
            foreach (SideEffectExecute see in cur_SideEffectBundle.SideEffectExecutes)
            {
                see.TriggerTime = value;
            }

            cur_SideEffectBundle?.RefreshSideEffectExecutesDict();
            OnSideEffectRefresh?.Invoke();
        }
    }

    private UnityAction<string> SetTriggerRange;

    private void OnTriggerRangeChange(string value_str)
    {
        SideEffectBundle.TriggerRange value = (SideEffectBundle.TriggerRange) Enum.Parse(typeof(SideEffectBundle.TriggerRange), value_str);
        if (cur_SideEffectBundle != null)
        {
            foreach (SideEffectExecute see in cur_SideEffectBundle.SideEffectExecutes)
            {
                see.TriggerRange = value;
            }

            cur_SideEffectBundle?.RefreshSideEffectExecutesDict();
            OnSideEffectRefresh?.Invoke();
        }
    }

    private UnityAction<string> SetTriggerDelayTimes;

    private void OnTriggerDelayTimesChange(string value_str)
    {
        int value = -1;
        int.TryParse(value_str, out value);
        if (value != -1)
        {
            if (cur_SideEffectBundle != null)
            {
                foreach (SideEffectExecute see in cur_SideEffectBundle.SideEffectExecutes)
                {
                    see.TriggerDelayTimes = value;
                }

                cur_SideEffectBundle?.RefreshSideEffectExecutesDict();
                OnSideEffectRefresh?.Invoke();
            }
        }
    }

    private UnityAction<string> SetTriggerTimes;

    private void OnTriggerTimesChange(string value_str)
    {
        int value = -1;
        int.TryParse(value_str, out value);
        if (value != -1)
        {
            if (cur_SideEffectBundle != null)
            {
                foreach (SideEffectExecute see in cur_SideEffectBundle.SideEffectExecutes)
                {
                    see.TriggerTimes = value;
                }

                cur_SideEffectBundle?.RefreshSideEffectExecutesDict();
                OnSideEffectRefresh?.Invoke();
            }
        }
    }

    private UnityAction<string> SetRemoveTriggerTime;

    private void OnRemoveTriggerTimeChange(string value_str)
    {
        SideEffectBundle.TriggerTime value = (SideEffectBundle.TriggerTime) Enum.Parse(typeof(SideEffectBundle.TriggerTime), value_str);
        if (cur_SideEffectBundle != null)
        {
            foreach (SideEffectExecute see in cur_SideEffectBundle.SideEffectExecutes)
            {
                see.RemoveTriggerTime = value;
            }

            cur_SideEffectBundle?.RefreshSideEffectExecutesDict();
            OnSideEffectRefresh?.Invoke();
        }
    }

    private UnityAction<string> SetRemoveTriggerRange;

    private void OnRemoveTriggerRangeChange(string value_str)
    {
        SideEffectBundle.TriggerRange value = (SideEffectBundle.TriggerRange) Enum.Parse(typeof(SideEffectBundle.TriggerRange), value_str);
        if (cur_SideEffectBundle != null)
        {
            foreach (SideEffectExecute see in cur_SideEffectBundle.SideEffectExecutes)
            {
                see.RemoveTriggerRange = value;
            }

            cur_SideEffectBundle?.RefreshSideEffectExecutesDict();
            OnSideEffectRefresh?.Invoke();
        }
    }

    private UnityAction<string> SetRemoveTriggerTimes;

    private void OnRemoveTriggerTimesChange(string value_str)
    {
        int value = -1;
        int.TryParse(value_str, out value);
        if (value != -1)
        {
            if (cur_SideEffectBundle != null)
            {
                foreach (SideEffectExecute see in cur_SideEffectBundle.SideEffectExecutes)
                {
                    see.RemoveTriggerTimes = value;
                }

                cur_SideEffectBundle?.RefreshSideEffectExecutesDict();
                OnSideEffectRefresh?.Invoke();
            }
        }
    }
}