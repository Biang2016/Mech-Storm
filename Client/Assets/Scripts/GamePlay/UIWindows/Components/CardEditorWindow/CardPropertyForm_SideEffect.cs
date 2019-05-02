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
    [SerializeField] private Button DeleteButton;
    [SerializeField] private Dropdown SideEffectTypeDropdown;

    void Awake()
    {
        SideEffectTypeDropdown.options.Clear();
        foreach (string option in AllSideEffects.SideEffectsNameDict.Keys.ToList())
        {
            SideEffectTypeDropdown.options.Add(new Dropdown.OptionData(option));
        }
    }

    public override void PoolRecycle()
    {
        base.PoolRecycle();

        foreach (CardPropertyFormRow cpfr in CardPropertyFormRows)
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

    private List<CardPropertyFormRow> CardPropertyFormRows = new List<CardPropertyFormRow>();
    private List<CardPropertyForm_SideEffectExecute> CardPropertyForm_SideEffectExecuteRows = new List<CardPropertyForm_SideEffectExecute>(); //内嵌BuffSEE

    public void Initialize(SideEffectExecute see, SideEffectBase se, UnityAction onRefreshText, UnityAction onDeleteButtonClick)
    {
        SideEffectTypeDropdown.onValueChanged.RemoveAllListeners();
        SetValue(se.Name);
        SideEffectTypeDropdown.onValueChanged.AddListener(delegate(int index)
        {
            string sideEffectName = SideEffectTypeDropdown.options[index].text;
            SideEffectBase newSE = AllSideEffects.GetSideEffect(sideEffectName);
            see.SideEffectBases.Remove(se);
            see.SideEffectBases.Add(newSE);
            Initialize(see, newSE, onRefreshText, onDeleteButtonClick);
            onRefreshText();
            StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ParamRowContainer));
            StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
        });

        DeleteButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.AddListener(
            delegate
            {
                onDeleteButtonClick();
                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ParamRowContainer));
                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
            });

        foreach (CardPropertyFormRow cpfr in CardPropertyFormRows)
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
                            CardPropertyFormRow row = CardPropertyFormRow.BaseInitialize(
                                CardPropertyFormRow.CardPropertyFormRowType.InputField,
                                ParamRowContainer,
                                sev.Name,
                                delegate(string value_str)
                                {
                                    if (int.TryParse(value_str, out int res)) s.Value = res;
                                    onRefreshText?.Invoke();
                                },
                                out UnityAction<string> setValue);
                            setValue(s.Value.ToString());
                            CardPropertyFormRows.Add(row);
                        }
                    }
                    else
                    {
                        if (s.EnumType == typeof(CardDeck))
                        {
                            CardPropertyFormRow row = CardPropertyFormRow.BaseInitialize(
                                CardPropertyFormRow.CardPropertyFormRowType.InputField,
                                ParamRowContainer,
                                sev.Name,
                                delegate(string value_str)
                                {
                                    if (int.TryParse(value_str, out int res)) s.Value = res;
                                    onRefreshText?.Invoke();
                                },
                                out UnityAction<string> setValue,
                                null,
                                delegate { UIManager.Instance.GetBaseUIForm<CardEditorPanel>().ChangeCard(s.Value); }
                            );
                            setValue(s.Value.ToString());
                            CardPropertyFormRows.Add(row);
                        }
                        else
                        {
                            List<string> enumList = new List<string>();
                            if (se is TargetSideEffect t_se1 && s.EnumType == typeof(TargetSelect))
                            {
                                List<TargetSelect> nameList = t_se1.ValidTargetSelects;
                                if (nameList == null)
                                {
                                    foreach (string enumName in Enum.GetNames(s.EnumType))
                                    {
                                        enumList.Add(enumName);
                                    }
                                }
                                else
                                {
                                    foreach (TargetSelect targetRange in nameList)
                                    {
                                        enumList.Add(targetRange.ToString());
                                    }
                                }

                                CardPropertyFormRow row = CardPropertyFormRow.BaseInitialize(
                                    CardPropertyFormRow.CardPropertyFormRowType.Dropdown,
                                    ParamRowContainer,
                                    sev.Name,
                                    delegate(string value_str)
                                    {
                                        s.Value = (int) Enum.Parse(s.EnumType, value_str);
                                        CardPropertyFormRow triggerRangeRow = null;
                                        foreach (CardPropertyFormRow cardPropertyFormRow in CardPropertyFormRows)
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
                                                CardPropertyFormRow new_TargetRangeRow = CardPropertyFormRow.BaseInitialize(
                                                    CardPropertyFormRow.CardPropertyFormRowType.Dropdown,
                                                    ParamRowContainer,
                                                    targetRangeSEV.Name,
                                                    delegate(string v_str)
                                                    {
                                                        targetRangeSEV.Value = (int) Enum.Parse(targetRangeSEV.EnumType, v_str);
                                                        onRefreshText?.Invoke();
                                                    },
                                                    out UnityAction<string> _setValue,
                                                    enumList_TargetRange);
                                                _setValue(Enum.GetName(targetRangeSEV.EnumType, targetRangeSEV.Value));
                                                CardPropertyFormRows.Add(new_TargetRangeRow);
                                                new_TargetRangeRow.transform.SetSiblingIndex(siblingIndex);
                                            }
                                        }

                                        if (s.Value == (int) TargetSelect.Multiple || s.Value == (int) TargetSelect.MultipleRandom)
                                        {
                                            CardPropertyFormRow choiceCountRow = null;
                                            foreach (CardPropertyFormRow cardPropertyFormRow in CardPropertyFormRows)
                                            {
                                                if (cardPropertyFormRow.LabelStrKey.Equals("ChoiceCount"))
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
                                                    CardPropertyFormRow new_ChoiceCountRow = CardPropertyFormRow.BaseInitialize(
                                                        CardPropertyFormRow.CardPropertyFormRowType.InputField,
                                                        ParamRowContainer,
                                                        choiceCountSEV.Name,
                                                        delegate(string v_str)
                                                        {
                                                            if (int.TryParse(v_str, out int res)) choiceCountSEV.Value = res;
                                                            onRefreshText?.Invoke();
                                                        },
                                                        out UnityAction<string> _setValue);
                                                    _setValue(choiceCountSEV.Value.ToString());
                                                    CardPropertyFormRows.Add(new_ChoiceCountRow);
                                                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ParamRowContainer));
                                                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            CardPropertyFormRow choiceCountRow = null;
                                            foreach (CardPropertyFormRow cardPropertyFormRow in CardPropertyFormRows)
                                            {
                                                if (cardPropertyFormRow.LabelStrKey.Equals("ChoiceCount"))
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
                                    out UnityAction<string> setValue,
                                    enumList);
                                setValue(Enum.GetName(s.EnumType, s.Value));
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

                                CardPropertyFormRow row = CardPropertyFormRow.BaseInitialize(
                                    CardPropertyFormRow.CardPropertyFormRowType.Dropdown,
                                    ParamRowContainer,
                                    sev.Name,
                                    delegate(string value_str)
                                    {
                                        s.Value = (int) Enum.Parse(s.EnumType, value_str);
                                        onRefreshText?.Invoke();
                                    },
                                    out UnityAction<string> setValue,
                                    enumList);
                                setValue(Enum.GetName(s.EnumType, s.Value));
                                CardPropertyFormRows.Add(row);
                            }
                            else
                            {
                                foreach (string enumName in Enum.GetNames(s.EnumType))
                                {
                                    enumList.Add(enumName);
                                }

                                CardPropertyFormRow row = CardPropertyFormRow.BaseInitialize(
                                    CardPropertyFormRow.CardPropertyFormRowType.Dropdown,
                                    ParamRowContainer,
                                    sev.Name,
                                    delegate(string value_str)
                                    {
                                        s.Value = (int) Enum.Parse(s.EnumType, value_str);
                                        onRefreshText?.Invoke();
                                    },
                                    out UnityAction<string> setValue,
                                    enumList);
                                setValue(Enum.GetName(s.EnumType, s.Value));
                                CardPropertyFormRows.Add(row);
                            }
                        }
                    }

                    break;
                }
                case SideEffectValue.ValueTypes.MultipliedInt:
                {
                    SideEffectValue_MultipliedInt s = (SideEffectValue_MultipliedInt) sev;
                    CardPropertyFormRow row = CardPropertyFormRow.BaseInitialize(
                        CardPropertyFormRow.CardPropertyFormRowType.InputField,
                        ParamRowContainer,
                        sev.Name,
                        delegate(string value_str)
                        {
                            if (int.TryParse(value_str, out int res)) s.Value = res;
                            onRefreshText?.Invoke();
                        },
                        out UnityAction<string> setValue);
                    setValue(s.Value.ToString());
                    CardPropertyFormRows.Add(row);
                    break;
                }
                case SideEffectValue.ValueTypes.Bool:
                {
                    SideEffectValue_Bool s = (SideEffectValue_Bool) sev;
                    CardPropertyFormRow row = CardPropertyFormRow.BaseInitialize(
                        CardPropertyFormRow.CardPropertyFormRowType.Toggle,
                        ParamRowContainer,
                        sev.Name,
                        delegate(string value_str)
                        {
                            s.Value = value_str == "True";
                            onRefreshText?.Invoke();
                        },
                        out UnityAction<string> setValue);
                    setValue(s.Value.ToString());
                    CardPropertyFormRows.Add(row);
                    break;
                }
                case SideEffectValue.ValueTypes.String:
                {
                    if (sev.Name.Equals("BuffName"))
                    {
                        SideEffectValue_String s = (SideEffectValue_String) sev;
                        CardPropertyFormRow row = CardPropertyFormRow.BaseInitialize(
                            CardPropertyFormRow.CardPropertyFormRowType.Dropdown,
                            ParamRowContainer,
                            sev.Name,
                            delegate(string value_str)
                            {
                                s.Value = value_str;
                                onRefreshText?.Invoke();
                            },
                            out UnityAction<string> setValue,
                            AllBuffs.BuffDict.Keys.ToList());
                        setValue(s.Value);
                        CardPropertyFormRows.Add(row);

                        SideEffectExecute buff_SEE = ((AddPlayerBuff_Base) se).AttachedBuffSEE;
                        if (buff_SEE != null)
                        {
                            CardPropertyForm_SideEffectExecute newSEERow = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CardPropertyForm_SideEffectExecute].AllocateGameObject<CardPropertyForm_SideEffectExecute>(ParamRowContainer);
                            newSEERow.Initialize(buff_SEE, onRefreshText,
                                delegate
                                {
                                    ((AddPlayerBuff_Base) se).AttachedBuffSEE = null;
                                    Initialize(see, se, onRefreshText, onDeleteButtonClick);
                                    onRefreshText();
                                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ParamRowContainer));
                                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
                                });
                            CardPropertyForm_SideEffectExecuteRows.Add(newSEERow);
                        }
                    }
                    else
                    {
                        SideEffectValue_String s = (SideEffectValue_String) sev;
                        CardPropertyFormRow row = CardPropertyFormRow.BaseInitialize(
                            CardPropertyFormRow.CardPropertyFormRowType.InputField,
                            ParamRowContainer,
                            sev.Name,
                            delegate(string a)
                            {
                                s.Value = a;
                                onRefreshText?.Invoke();
                            },
                            out UnityAction<string> setValue);
                        setValue(s.Value);
                        CardPropertyFormRows.Add(row);
                    }

                    break;
                }
            }
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
            SideEffectTypeDropdown.value = 0;
            SideEffectTypeDropdown.value = setValue;
        }
    }
}