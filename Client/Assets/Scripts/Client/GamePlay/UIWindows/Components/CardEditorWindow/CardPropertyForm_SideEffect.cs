using System;
using System.Collections.Generic;
using System.Linq;
using SideEffects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardPropertyForm_SideEffect : PoolObject
{
    [SerializeField] private Transform ParamRowContainer;
    [SerializeField] private Button AddButton;
    [SerializeField] private Button DeleteButton;
    [SerializeField] private Dropdown SideEffectTypeDropdown;

    public override void PoolRecycle()
    {
        base.PoolRecycle();

        foreach (PropertyFormRow cpfr in CardPropertyFormRows)
        {
            cpfr.PoolRecycle();
        }

        CardPropertyFormRows.Clear();
        foreach (CardPropertyForm_SideEffectExecute cpfsee in CardPropertyForm_SideEffectExecuteRows)
        {
            cpfsee.PoolRecycle();
        }

        CardPropertyForm_SideEffectExecuteRows.Clear();
    }

    private List<PropertyFormRow> CardPropertyFormRows = new List<PropertyFormRow>();
    private List<CardPropertyForm_SideEffectExecute> CardPropertyForm_SideEffectExecuteRows = new List<CardPropertyForm_SideEffectExecute>(); //内嵌BuffSEE
    private List<CardPropertyForm_SideEffect> CardPropertyForm_SubSideEffectBaseRows = new List<CardPropertyForm_SideEffect>(); //Buff SE的Sub_SideEffect

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="see">Parent see</param>
    /// <param name="ses">Parent Sub_SideEffectBases of a SideEffectBase</param>
    /// <param name="se">Self info</param>
    /// <param name="onRefreshText"></param>
    /// <param name="onDeleteButtonClick"></param>
    public void Initialize(SideEffectExecute see, List<SideEffectBase> ses, SideEffectBase se, UnityAction onRefreshText, UnityAction onDeleteButtonClick)
    {
        SideEffectTypeDropdown.options.Clear();
        if (se is PlayerBuffSideEffects)
        {
            foreach (string option in AllBuffs.BuffDict.Keys.ToList())
            {
                SideEffectTypeDropdown.options.Add(new Dropdown.OptionData(option));
            }
        }
        else
        {
            foreach (string option in AllSideEffects.SideEffectsNameDict.Keys.ToList())
            {
                SideEffectTypeDropdown.options.Add(new Dropdown.OptionData(option));
            }
        }

        SideEffectTypeDropdown.onValueChanged.RemoveAllListeners();
        SetValue(se.Name);
        SideEffectTypeDropdown.onValueChanged.AddListener(delegate(int index)
        {
            string sideEffectName = SideEffectTypeDropdown.options[index].text;
            SideEffectBase newSE = AllSideEffects.GetSideEffect(sideEffectName);
            if (see != null)
            {
                see.SideEffectBases.Remove(se);
                see.SideEffectBases.Add(newSE);
            }

            if (ses != null)
            {
                ses.Remove(se);
                ses.Add(newSE);
            }

            Initialize(see, ses, newSE, onRefreshText, onDeleteButtonClick);
            onRefreshText();
            StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ParamRowContainer));
            StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
        });

        DeleteButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.AddListener(
            delegate
            {
                if (se is PlayerBuffSideEffects) return;
                onDeleteButtonClick();
                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ParamRowContainer));
                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
            });
        DeleteButton.gameObject.SetActive(!(se is PlayerBuffSideEffects));

        AddButton.gameObject.SetActive(se is PlayerBuffSideEffects);
        if (se is PlayerBuffSideEffects buff_SE)
        {
            AddButton.onClick.RemoveAllListeners();
            AddButton.onClick.AddListener(
                delegate
                {
                    buff_SE.Sub_SideEffect.Add(AllSideEffects.GetSideEffect("Damage"));
                    Initialize(see, ses, se, onRefreshText, onDeleteButtonClick);
                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ParamRowContainer));
                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
                });
        }

        SideEffectTypeDropdown.interactable = !(se is PlayerBuffSideEffects);

        foreach (PropertyFormRow cpfr in CardPropertyFormRows)
        {
            cpfr.PoolRecycle();
        }

        CardPropertyFormRows.Clear();
        foreach (CardPropertyForm_SideEffectExecute cpfsee in CardPropertyForm_SideEffectExecuteRows)
        {
            cpfsee.PoolRecycle();
        }

        CardPropertyForm_SideEffectExecuteRows.Clear();

        foreach (SideEffectValue sev in se.M_SideEffectParam.SideEffectValues)
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
                                parent: ParamRowContainer,
                                labelStrKey: sev_Prefix + sev.Name,
                                onValueChangeAction: delegate(string value_str)
                                {
                                    if (int.TryParse(value_str, out int res)) s.Value = res;
                                    onRefreshText?.Invoke();
                                },
                                setValue: out UnityAction<string> setValue);
                            setValue(s.Value.ToString());
                            CardPropertyFormRows.Add(row);
                        }
                    }
                    else
                    {
                        if (s.EnumType == typeof(CardDeck))
                        {
                            PropertyFormRow row = PropertyFormRow.BaseInitialize(
                                type: PropertyFormRow.CardPropertyFormRowType.InputField,
                                parent: ParamRowContainer,
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
                                setValue: out UnityAction<string> setValue,
                                dropdownOptionList: null,
                                onButtonClick: delegate { UIManager.Instance.GetBaseUIForm<CardEditorPanel>().ChangeCard(s.Value); }
                            );
                            setValue(s.Value.ToString());
                            CardPropertyFormRows.Add(row);
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
                                    parent: ParamRowContainer,
                                    labelStrKey: sev_Prefix + sev.Name,
                                    onValueChangeAction: delegate(string value_str)
                                    {
                                        s.Value = (int) Enum.Parse(s.EnumType, value_str);
                                        PropertyFormRow triggerRangeRow = null;
                                        foreach (PropertyFormRow cardPropertyFormRow in CardPropertyFormRows)
                                        {
                                            if (cardPropertyFormRow.LabelStrKey.Equals("TargetRange"))
                                            {
                                                triggerRangeRow = cardPropertyFormRow;
                                                break;
                                            }
                                        }

                                        if (triggerRangeRow)
                                        {
                                            int siblingIndex = triggerRangeRow.transform.GetSiblingIndex();
                                            triggerRangeRow.PoolRecycle();
                                            CardPropertyFormRows.Remove(triggerRangeRow);

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
                                                    ParamRowContainer,
                                                    sev_Prefix + targetRangeSEV.Name,
                                                    delegate(string v_str)
                                                    {
                                                        targetRangeSEV.Value = (int) Enum.Parse(targetRangeSEV.EnumType, v_str);
                                                        onRefreshText?.Invoke();
                                                    },
                                                    out UnityAction<string> _setValue,
                                                    enumList_TargetRange);
                                                _setValue(Enum.GetName(targetRangeSEV.EnumType, targetRangeSEV.Value));
                                                onRefreshText?.Invoke();
                                                CardPropertyFormRows.Add(new_TargetRangeRow);
                                                new_TargetRangeRow.transform.SetSiblingIndex(siblingIndex);
                                            }
                                        }

                                        if (s.Value == (int) TargetSelect.Multiple || s.Value == (int) TargetSelect.MultipleRandom)
                                        {
                                            PropertyFormRow choiceCountRow = null;
                                            foreach (PropertyFormRow cardPropertyFormRow in CardPropertyFormRows)
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
                                                        ParamRowContainer,
                                                        sev_Prefix + choiceCountSEV.Name,
                                                        delegate(string v_str)
                                                        {
                                                            if (int.TryParse(v_str, out int res)) choiceCountSEV.Value = res;
                                                            onRefreshText?.Invoke();
                                                        },
                                                        out UnityAction<string> _setValue);
                                                    _setValue(choiceCountSEV.Value.ToString());
                                                    onRefreshText?.Invoke();
                                                    CardPropertyFormRows.Add(new_ChoiceCountRow);
                                                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ParamRowContainer));
                                                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            PropertyFormRow choiceCountRow = null;
                                            foreach (PropertyFormRow cardPropertyFormRow in CardPropertyFormRows)
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
                                                CardPropertyFormRows.Remove(choiceCountRow);
                                                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ParamRowContainer));
                                                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
                                            }
                                        }

                                        onRefreshText?.Invoke();
                                    },
                                    setValue: out UnityAction<string> setValue,
                                    dropdownOptionList: enumList);
                                setValue(Enum.GetName(s.EnumType, s.Value));
                                onRefreshText?.Invoke();
                                CardPropertyFormRows.Add(row);
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
                                    parent: ParamRowContainer,
                                    labelStrKey: sev_Prefix + sev.Name,
                                    onValueChangeAction: delegate(string value_str)
                                    {
                                        s.Value = (int) Enum.Parse(s.EnumType, value_str);
                                        onRefreshText?.Invoke();
                                    },
                                    setValue: out UnityAction<string> setValue,
                                    dropdownOptionList: enumList);
                                setValue(Enum.GetName(s.EnumType, s.Value));
                                onRefreshText?.Invoke();
                                CardPropertyFormRows.Add(row);
                            }
                            else
                            {
                                foreach (string enumName in Enum.GetNames(s.EnumType))
                                {
                                    enumList.Add(enumName);
                                }

                                PropertyFormRow row = PropertyFormRow.BaseInitialize(
                                    type: PropertyFormRow.CardPropertyFormRowType.Dropdown,
                                    parent: ParamRowContainer,
                                    labelStrKey: sev_Prefix + sev.Name,
                                    delegate(string value_str)
                                    {
                                        s.Value = (int) Enum.Parse(s.EnumType, value_str);
                                        onRefreshText?.Invoke();
                                    },
                                    setValue: out UnityAction<string> setValue,
                                    dropdownOptionList: enumList);
                                setValue(Enum.GetName(s.EnumType, s.Value));
                                onRefreshText?.Invoke();
                                CardPropertyFormRows.Add(row);
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
                        parent: ParamRowContainer,
                        labelStrKey: sev_Prefix + sev.Name,
                        onValueChangeAction: delegate(string value_str)
                        {
                            if (int.TryParse(value_str, out int res)) s.Value = res;
                            onRefreshText?.Invoke();
                        },
                        setValue: out UnityAction<string> setValue);
                    setValue(s.Value.ToString());
                    CardPropertyFormRows.Add(row);
                    break;
                }
                case SideEffectValue.ValueTypes.Bool:
                {
                    SideEffectValue_Bool s = (SideEffectValue_Bool) sev;
                    PropertyFormRow row = PropertyFormRow.BaseInitialize(
                        type: PropertyFormRow.CardPropertyFormRowType.Toggle,
                        parent: ParamRowContainer,
                        labelStrKey: sev_Prefix + sev.Name,
                        onValueChangeAction: delegate(string value_str)
                        {
                            s.Value = value_str == "True";
                            onRefreshText?.Invoke();
                        },
                        setValue: out UnityAction<string> setValue);
                    setValue(s.Value.ToString());
                    CardPropertyFormRows.Add(row);
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
                            parent: ParamRowContainer,
                            labelStrKey: sev_Prefix + sev.Name,
                            onValueChangeAction: delegate(string value_str)
                            {
                                string buffName = value_str;
                                if (addBuffSE.BuffName == buffName) return;
                                addBuffSE.BuffName = buffName;
                                Initialize(see, ses, addBuffSE, onRefreshText, onDeleteButtonClick);
                                onRefreshText();
                                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ParamRowContainer));
                                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
                            },
                            setValue: out UnityAction<string> setValue,
                            dropdownOptionList: AllBuffs.BuffDict.Keys.ToList());

                        setValue(s.Value);
                        CardPropertyFormRows.Add(row);

                        SideEffectExecute buff_SEE = addBuffSE.AttachedBuffSEE;
                        if (buff_SEE != null)
                        {
                            CardPropertyForm_SideEffectExecute newSEERow = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CardPropertyForm_SideEffectExecute].AllocateGameObject<CardPropertyForm_SideEffectExecute>(ParamRowContainer);
                            newSEERow.Initialize(
                                sideEffectFrom: SideEffectExecute.SideEffectFrom.Buff,
                                see: buff_SEE,
                                onRefreshText: onRefreshText,
                                onDeleteButtonClick: delegate { });
                            CardPropertyForm_SideEffectExecuteRows.Add(newSEERow);
                        }
                    }
                    else
                    {
                        SideEffectValue_String s = (SideEffectValue_String) sev;
                        PropertyFormRow row = PropertyFormRow.BaseInitialize(
                            type: PropertyFormRow.CardPropertyFormRowType.InputField,
                            parent: ParamRowContainer,
                            labelStrKey: sev_Prefix + sev.Name,
                            onValueChangeAction: delegate(string a)
                            {
                                s.Value = a;
                                onRefreshText?.Invoke();
                            },
                            setValue: out UnityAction<string> setValue);
                        setValue(s.Value);
                        CardPropertyFormRows.Add(row);
                    }

                    break;
                }
            }
        }

        foreach (CardPropertyForm_SideEffect cpfse in CardPropertyForm_SubSideEffectBaseRows)
        {
            cpfse.PoolRecycle();
        }

        CardPropertyForm_SubSideEffectBaseRows.Clear();
        foreach (SideEffectBase sub_se in se.Sub_SideEffect)
        {
            CardPropertyForm_SideEffect sub_se_row = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CardPropertyForm_SideEffect].AllocateGameObject<CardPropertyForm_SideEffect>(ParamRowContainer);
            sub_se_row.Initialize(
                see: null,
                ses: se.Sub_SideEffect,
                se: sub_se,
                onRefreshText: onRefreshText,
                onDeleteButtonClick: delegate
                {
                    se.Sub_SideEffect.Remove(sub_se);
                    Initialize(see, ses, se, onRefreshText, onDeleteButtonClick);
                });
            CardPropertyForm_SubSideEffectBaseRows.Add(sub_se_row);
        }
    }

    private void SetValue(string value_str)
    {
        int setValue = -1;
        for (int i = 0; i < SideEffectTypeDropdown.options.Count; i++)
        {
            if (value_str.Equals(SideEffectTypeDropdown.options[i].text))
            {
                setValue = i;
                break;
            }
        }

        if (setValue != -1)
        {
            SideEffectTypeDropdown.value = setValue;
            SideEffectTypeDropdown.RefreshShownValue();
        }
    }
}