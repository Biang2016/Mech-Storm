using System;
using System.Collections.Generic;
using System.Linq;
using SideEffects;
using UnityEngine;
using UnityEngine.Events;

public class CardEditorPanel_Params
{
    public static void GenerateParamRows(SideEffectBase se, SideEffectParam sep, UnityAction onRefreshText, UnityAction reInitialize, Transform parentContainer, List<PropertyFormRow> pfr_List, List<CardPropertyForm_SideEffectExecute> pfr_List_SEE, UnityAction refreshContainer)
    {
        foreach (SideEffectValue sev in sep.SideEffectValues)
        {
            string sev_Prefix = "SideEffectValueNames_";
            switch (sev.ValueType)
            {
                case SideEffectValue.ValueTypes.ConstInt:
                {
                    SideEffectValue_ConstInt s = (SideEffectValue_ConstInt) sev;
                    if (s.EnumType == null)
                    {
                        bool showInputField = true;
                        if (s.Name == "ChoiceCount")
                        {
                            if (se is TargetSideEffect t_se)
                            {
                                if (t_se.TargetSelect != TargetSelect.Multiple && t_se.TargetSelect != TargetSelect.MultipleRandom)
                                {
                                    showInputField = false;
                                }
                            }
                        }

                        if (showInputField)
                        {
                            PropertyFormRow row = PropertyFormRow.BaseInitialize(
                                type: PropertyFormRow.CardPropertyFormRowType.InputField,
                                parent: parentContainer,
                                labelStrKey: sev_Prefix + sev.Name,
                                onValueChangeAction: delegate(string value_str)
                                {
                                    if (int.TryParse(value_str, out int res)) s.Value = res;
                                    onRefreshText?.Invoke();
                                },
                                setValue: out UnityAction<string, bool> setValue);
                            setValue(s.Value.ToString(), false);
                            pfr_List.Add(row);
                        }
                    }
                    else
                    {
                        if (s.EnumType == typeof(CardDeck))
                        {
                            PropertyFormRow row = PropertyFormRow.BaseInitialize(
                                type: PropertyFormRow.CardPropertyFormRowType.InputField,
                                parent: parentContainer,
                                labelStrKey: sev_Prefix + sev.Name,
                                onValueChangeAction: delegate(string value_str)
                                {
                                    if (int.TryParse(value_str, out int res))
                                    {
                                        bool hasCard = AllCards.CardDict.ContainsKey(res);
                                        if (hasCard)
                                        {
                                            s.Value = res;
                                            onRefreshText?.Invoke();
                                        }
                                    }
                                },
                                setValue: out UnityAction<string, bool> setValue,
                                dropdownOptionList: null,
                                onButtonClick: delegate { UIManager.Instance.GetBaseUIForm<CardEditorPanel>().ChangeCard(s.Value); }
                            );
                            setValue(s.Value.ToString(), false);
                            pfr_List.Add(row);
                        }
                        else
                        {
                            List<string> enumList = new List<string>();
                            if (se is TargetSideEffect t_se1 && s.EnumType == typeof(TargetSelect))
                            {
                                foreach (TargetSelect targetRange in t_se1.ValidTargetSelects)
                                {
                                    enumList.Add(targetRange.ToString());
                                }

                                PropertyFormRow row = PropertyFormRow.BaseInitialize(
                                    type: PropertyFormRow.CardPropertyFormRowType.Dropdown,
                                    parent: parentContainer,
                                    labelStrKey: sev_Prefix + sev.Name,
                                    onValueChangeAction: delegate(string value_str)
                                    {
                                        s.Value = (int) Enum.Parse(s.EnumType, value_str);
                                        PropertyFormRow targetRangeRow = null;
                                        foreach (PropertyFormRow cardPropertyFormRow in pfr_List)
                                        {
                                            if (cardPropertyFormRow.LabelStrKey.Equals(sev_Prefix + "TargetRange"))
                                            {
                                                targetRangeRow = cardPropertyFormRow;
                                                break;
                                            }
                                        }

                                        if (targetRangeRow)
                                        {
                                            int siblingIndex = targetRangeRow.transform.GetSiblingIndex();
                                            targetRangeRow.PoolRecycle();
                                            pfr_List.Remove(targetRangeRow);

                                            Dictionary<TargetSelect, List<TargetRange>> dict = TargetSelector.TargetSelectorPresets[t_se1.TargetSelectorType];
                                            List<string> enumList_TargetRange = new List<string>();
                                            List<TargetRange> nlist = dict[t_se1.TargetSelect];
                                            foreach (TargetRange targetRange in nlist)
                                            {
                                                enumList_TargetRange.Add(targetRange.ToString());
                                            }

                                            SideEffectValue_ConstInt targetRangeSEV = (SideEffectValue_ConstInt) se.M_SideEffectParam.GetParam("TargetRange");
                                            if (targetRangeSEV != null)
                                            {
                                                PropertyFormRow new_TargetRangeRow = PropertyFormRow.BaseInitialize(
                                                    PropertyFormRow.CardPropertyFormRowType.Dropdown,
                                                    parentContainer,
                                                    sev_Prefix + targetRangeSEV.Name,
                                                    delegate(string v_str)
                                                    {
                                                        targetRangeSEV.Value = (int) Enum.Parse(targetRangeSEV.EnumType, v_str);
                                                        onRefreshText?.Invoke();
                                                    },
                                                    out UnityAction<string, bool> _setValue,
                                                    enumList_TargetRange);
                                                targetRangeSEV.SetValue(enumList_TargetRange[0]);
                                                _setValue(Enum.GetName(targetRangeSEV.EnumType, targetRangeSEV.Value), false);
                                                onRefreshText?.Invoke();
                                                pfr_List.Add(new_TargetRangeRow);
                                                new_TargetRangeRow.transform.SetSiblingIndex(siblingIndex);
                                            }
                                        }

                                        if (s.Value == (int) TargetSelect.Multiple || s.Value == (int) TargetSelect.MultipleRandom)
                                        {
                                            PropertyFormRow choiceCountRow = null;
                                            foreach (PropertyFormRow cardPropertyFormRow in pfr_List)
                                            {
                                                if (cardPropertyFormRow.LabelStrKey.Equals(sev_Prefix + "ChoiceCount"))
                                                {
                                                    choiceCountRow = cardPropertyFormRow;
                                                    break;
                                                }
                                            }

                                            if (!choiceCountRow)
                                            {
                                                SideEffectValue_ConstInt choiceCountSEV = (SideEffectValue_ConstInt) se.M_SideEffectParam.GetParam("ChoiceCount");
                                                if (choiceCountSEV != null)
                                                {
                                                    PropertyFormRow new_ChoiceCountRow = PropertyFormRow.BaseInitialize(
                                                        PropertyFormRow.CardPropertyFormRowType.InputField,
                                                        parentContainer,
                                                        sev_Prefix + choiceCountSEV.Name,
                                                        delegate(string v_str)
                                                        {
                                                            if (int.TryParse(v_str, out int res)) choiceCountSEV.Value = res;
                                                            onRefreshText?.Invoke();
                                                        },
                                                        out UnityAction<string, bool> _setValue);
                                                    _setValue(choiceCountSEV.Value.ToString(), false);
                                                    onRefreshText?.Invoke();
                                                    pfr_List.Add(new_ChoiceCountRow);
                                                    refreshContainer();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            PropertyFormRow choiceCountRow = null;
                                            foreach (PropertyFormRow cardPropertyFormRow in pfr_List)
                                            {
                                                if (cardPropertyFormRow.LabelStrKey.Equals(sev_Prefix + "ChoiceCount"))
                                                {
                                                    choiceCountRow = cardPropertyFormRow;
                                                    break;
                                                }
                                            }

                                            if (choiceCountRow)
                                            {
                                                choiceCountRow.PoolRecycle();
                                                pfr_List.Remove(choiceCountRow);
                                                refreshContainer();
                                            }
                                        }

                                        onRefreshText?.Invoke();
                                    },
                                    setValue: out UnityAction<string, bool> setValue,
                                    dropdownOptionList: enumList);
                                setValue(Enum.GetName(s.EnumType, s.Value), false);
                                onRefreshText?.Invoke();
                                pfr_List.Add(row);
                            }
                            else if (se is TargetSideEffect t_se2 && s.EnumType == typeof(TargetRange))
                            {
                                Dictionary<TargetSelect, List<TargetRange>> dict = TargetSelector.TargetSelectorPresets[t_se2.TargetSelectorType];
                                List<TargetRange> nameList = dict[t_se2.TargetSelect];
                                foreach (TargetRange targetRange in nameList)
                                {
                                    enumList.Add(targetRange.ToString());
                                }

                                PropertyFormRow row = PropertyFormRow.BaseInitialize(
                                    type: PropertyFormRow.CardPropertyFormRowType.Dropdown,
                                    parent: parentContainer,
                                    labelStrKey: sev_Prefix + sev.Name,
                                    onValueChangeAction: delegate(string value_str)
                                    {
                                        s.Value = (int) Enum.Parse(s.EnumType, value_str);
                                        onRefreshText?.Invoke();
                                    },
                                    setValue: out UnityAction<string, bool> setValue,
                                    dropdownOptionList: enumList);
                                setValue(Enum.GetName(s.EnumType, s.Value), false);
                                onRefreshText?.Invoke();
                                pfr_List.Add(row);
                            }
                            else
                            {
                                foreach (string enumName in Enum.GetNames(s.EnumType))
                                {
                                    enumList.Add(enumName);
                                }

                                PropertyFormRow row = PropertyFormRow.BaseInitialize(
                                    type: PropertyFormRow.CardPropertyFormRowType.Dropdown,
                                    parent: parentContainer,
                                    labelStrKey: sev_Prefix + sev.Name,
                                    delegate(string value_str)
                                    {
                                        s.Value = (int) Enum.Parse(s.EnumType, value_str);
                                        onRefreshText?.Invoke();
                                    },
                                    setValue: out UnityAction<string, bool> setValue,
                                    dropdownOptionList: enumList);
                                setValue(Enum.GetName(s.EnumType, s.Value), false);
                                onRefreshText?.Invoke();
                                pfr_List.Add(row);
                            }
                        }
                    }

                    break;
                }
                case SideEffectValue.ValueTypes.MultipliedInt:
                {
                    SideEffectValue_MultipliedInt s = (SideEffectValue_MultipliedInt) sev;
                    PropertyFormRow row = PropertyFormRow.BaseInitialize(
                        type: PropertyFormRow.CardPropertyFormRowType.InputField,
                        parent: parentContainer,
                        labelStrKey: sev_Prefix + sev.Name,
                        onValueChangeAction: delegate(string value_str)
                        {
                            if (int.TryParse(value_str, out int res)) s.Value = res;
                            onRefreshText?.Invoke();
                        },
                        setValue: out UnityAction<string, bool> setValue);
                    setValue(s.Value.ToString(), false);
                    pfr_List.Add(row);
                    break;
                }
                case SideEffectValue.ValueTypes.Bool:
                {
                    SideEffectValue_Bool s = (SideEffectValue_Bool) sev;
                    PropertyFormRow row = PropertyFormRow.BaseInitialize(
                        type: PropertyFormRow.CardPropertyFormRowType.Toggle,
                        parent: parentContainer,
                        labelStrKey: sev_Prefix + sev.Name,
                        onValueChangeAction: delegate(string value_str)
                        {
                            s.Value = value_str == "True";
                            onRefreshText?.Invoke();
                        },
                        setValue: out UnityAction<string, bool> setValue);
                    setValue(s.Value.ToString(), false);
                    pfr_List.Add(row);
                    break;
                }
                case SideEffectValue.ValueTypes.String:
                {
                    if (sev.Name.Equals("BuffName"))
                    {
                        AddPlayerBuff_Base addBuffSE = (AddPlayerBuff_Base) se;
                        SideEffectValue_String s = (SideEffectValue_String) sev;
                        PropertyFormRow row = PropertyFormRow.BaseInitialize(
                            type: PropertyFormRow.CardPropertyFormRowType.Dropdown,
                            parent: parentContainer,
                            labelStrKey: sev_Prefix + sev.Name,
                            onValueChangeAction: delegate(string value_str)
                            {
                                string buffName = value_str;
                                if (addBuffSE.BuffName == buffName) return;
                                addBuffSE.BuffName = buffName;
                                reInitialize();
                                onRefreshText();
                                refreshContainer();
                            },
                            setValue: out UnityAction<string, bool> setValue,
                            dropdownOptionList: AllBuffs.BuffDict.Keys.ToList());

                        setValue(s.Value, false);
                        pfr_List.Add(row);

                        SideEffectExecute buff_SEE = addBuffSE.AttachedBuffSEE;
                        if (buff_SEE != null)
                        {
                            CardPropertyForm_SideEffectExecute newSEERow = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CardPropertyForm_SideEffectExecute].AllocateGameObject<CardPropertyForm_SideEffectExecute>(parentContainer);
                            newSEERow.Initialize(
                                sideEffectFrom: SideEffectExecute.SideEffectFrom.Buff,
                                see: buff_SEE,
                                onRefreshText: onRefreshText,
                                onDeleteButtonClick: delegate { });
                            pfr_List_SEE.Add(newSEERow);
                        }
                    }
                    else
                    {
                        SideEffectValue_String s = (SideEffectValue_String) sev;
                        PropertyFormRow row = PropertyFormRow.BaseInitialize(
                            type: PropertyFormRow.CardPropertyFormRowType.InputField,
                            parent: parentContainer,
                            labelStrKey: sev_Prefix + sev.Name,
                            onValueChangeAction: delegate(string a)
                            {
                                s.Value = a;
                                onRefreshText?.Invoke();
                            },
                            setValue: out UnityAction<string, bool> setValue);
                        setValue(s.Value, false);
                        pfr_List.Add(row);
                    }

                    break;
                }
            }
        }
    }
}