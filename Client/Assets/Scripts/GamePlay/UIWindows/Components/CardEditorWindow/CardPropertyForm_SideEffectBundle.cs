using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardPropertyForm_SideEffectBundle : PoolObject
{
    [SerializeField] private Text SideEffectBundleText;
    [SerializeField] private Transform SideEffectExecuteRowContainer;
    [SerializeField] private Button AddSideEffectButton;

    public override void PoolRecycle()
    {
        base.PoolRecycle();

        foreach (CardPropertyForm_SideEffectExecute cpfsee in CardPropertyForm_SideEffectExecuteRows)
        {
            cpfsee.PoolRecycle();
        }

        CardPropertyForm_SideEffectExecuteRows.Clear();
    }

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKey(SideEffectBundleText, "CardEditorWindow_SideEffectBundleText");
    }

    private SideEffectExecute.ExecuteSetting Cur_ExecuteSetting = SideEffectExecute.ExecuteSetting_Presets[SideEffectExecute.ExecuteSettingTypes.PlayOutEffect];

    private List<CardPropertyForm_SideEffectExecute> CardPropertyForm_SideEffectExecuteRows = new List<CardPropertyForm_SideEffectExecute>();

    public void Initialize(CardInfo_Base cardInfo, SideEffectBundle seb, UnityAction onRefreshText)
    {
        foreach (CardPropertyForm_SideEffectExecute cpfsee in CardPropertyForm_SideEffectExecuteRows)
        {
            cpfsee.PoolRecycle();
        }

        CardPropertyForm_SideEffectExecuteRows.Clear();

        if (cardInfo != null)
        {
            AddSideEffectButton.onClick.RemoveAllListeners();
            AddSideEffectButton.onClick.AddListener(delegate
            {
                SideEffectExecute.SideEffectFrom sef = SideEffectExecute.SideEffectFrom.Unknown;
                switch (cardInfo.BaseInfo.CardType)
                {
                    case CardTypes.Retinue:
                        sef = SideEffectExecute.SideEffectFrom.RetinueSideEffect;
                        break;
                    case CardTypes.Equip:
                        sef = SideEffectExecute.SideEffectFrom.EquipSideEffect;
                        break;
                    case CardTypes.Spell:
                        sef = SideEffectExecute.SideEffectFrom.SpellCard;
                        break;
                    case CardTypes.Energy:
                        sef = SideEffectExecute.SideEffectFrom.EnergyCard;
                        break;
                    //todo from buff
                }

                SideEffectExecute newSEE = new SideEffectExecute(sef, new List<SideEffectBase> {AllSideEffects.GetSideEffect("Damage").Clone()}, Cur_ExecuteSetting);

                seb.AddSideEffectExecute(newSEE);
                seb.RefreshSideEffectExecutesDict();
                Initialize(cardInfo, seb, onRefreshText);
                onRefreshText();
                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
            });

            foreach (SideEffectExecute see in seb.SideEffectExecutes)
            {
                CardPropertyForm_SideEffectExecute cpfsee = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CardPropertyForm_SideEffectExecute].AllocateGameObject<CardPropertyForm_SideEffectExecute>(SideEffectExecuteRowContainer);
                cpfsee.Initialize(SideEffectExecute.GetSideEffectFromByCardType(cardInfo.BaseInfo.CardType), see, onRefreshText, delegate
                {
                    seb.RemoveSideEffectExecute(see);
                    Initialize(cardInfo, seb, onRefreshText);
                    onRefreshText();
                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) SideEffectExecuteRowContainer));
                });
                CardPropertyForm_SideEffectExecuteRows.Add(cpfsee);
            }
        }
    }
}