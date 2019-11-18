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

        CardEditorPanel_Params.GenerateParamRows(se,se.M_SideEffectParam, onRefreshText, delegate { Initialize(see, ses, se, onRefreshText, onDeleteButtonClick); }, ParamRowContainer, CardPropertyFormRows, CardPropertyForm_SideEffectExecuteRows, delegate
        {
            StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ParamRowContainer));
            StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
        });

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