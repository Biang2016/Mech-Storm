using System;
using System.Collections.Generic;
using System.Linq;
using SideEffects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoryPropertyForm_Bonus : PoolObject
{
    [SerializeField] private Text Label;
    [SerializeField] private Transform BonusRowContainer;
    [SerializeField] private Button AddButton;

    public override void PoolRecycle()
    {
        base.PoolRecycle();

        foreach (PropertyFormRow cpfr in BonusPropertyFormRows)
        {
            cpfr.PoolRecycle();
        }

        BonusPropertyFormRows.Clear();
        LanguageManager.Instance.UnregisterText(Label);
    }

    private List<PropertyFormRow> BonusPropertyFormRows = new List<PropertyFormRow>();

    public void Initialize(Chapter chapter)
    {
        LanguageManager.Instance.RegisterTextKey(Label, "StoryEditorPanel_AllBonusLabel");
        AddButton.onClick.RemoveAllListeners();
        AddButton.onClick.AddListener(
            delegate { StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<StoryEditorPanel>().StoryPropertiesContainer)); });

        foreach (PropertyFormRow cpfr in BonusPropertyFormRows)
        {
            cpfr.PoolRecycle();
        }

        BonusPropertyFormRows.Clear();
    }

    protected void SetValue(string value_str)
    {
    }
}