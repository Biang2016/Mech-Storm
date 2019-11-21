using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardPropertyForm_SideEffectBundle : PoolObject
{
    [SerializeField] private Text SideEffectBundleText;
    [SerializeField] private Transform SideEffectExecuteRowContainer;
    [SerializeField] private Button RefreshSideEffectBundleTransformButton;
    [SerializeField] private Button AddSideEffectExecuteButton;

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
        LanguageManager.Instance.RegisterTextKey(SideEffectBundleText, "CardEditorPanel_SideEffectBundleText");
    }

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
            if (seb == cardInfo.SideEffectBundle)
            {
                LanguageManager.Instance.RegisterTextKey(SideEffectBundleText, "CardEditorPanel_SideEffectBundleText");
            }
            else
            {
                LanguageManager.Instance.RegisterTextKey(SideEffectBundleText, "CardEditorPanel_SideEffectBundleAuraText");
            }

            AddSideEffectExecuteButton.onClick.RemoveAllListeners();
            AddSideEffectExecuteButton.onClick.AddListener(delegate
            {
                SideEffectExecute.SideEffectFrom sef = SideEffectExecute.SideEffectFrom.Unknown;
                SideEffectExecute.ExecuteSetting executeSetting = SideEffectExecute.ExecuteSetting_Presets[SideEffectExecute.ExecuteSettingTypes.PlayOutEffect].Clone();

                switch (cardInfo.BaseInfo.CardType)
                {
                    case CardTypes.Mech:
                        sef = SideEffectExecute.SideEffectFrom.MechSideEffect;
                        executeSetting = SideEffectExecute.ExecuteSetting_Presets[SideEffectExecute.ExecuteSettingTypes.BattleCry];
                        break;
                    case CardTypes.Equip:
                        sef = SideEffectExecute.SideEffectFrom.EquipSideEffect;
                        executeSetting = SideEffectExecute.ExecuteSetting_Presets[SideEffectExecute.ExecuteSettingTypes.EquipBattleCry];
                        break;
                    case CardTypes.Spell:
                        sef = SideEffectExecute.SideEffectFrom.SpellCard;
                        executeSetting = SideEffectExecute.ExecuteSetting_Presets[SideEffectExecute.ExecuteSettingTypes.PlayOutEffect];
                        break;
                    case CardTypes.Energy:
                        sef = SideEffectExecute.SideEffectFrom.EnergyCard;
                        executeSetting = SideEffectExecute.ExecuteSetting_Presets[SideEffectExecute.ExecuteSettingTypes.PlayOutEffect];
                        break;
                    //todo from buff
                }

                SideEffectExecute newSEE = new SideEffectExecute(sef, new List<SideEffectBase> {AllSideEffects.GetSideEffect("Damage").Clone()}, executeSetting);

                seb.AddSideEffectExecute(newSEE);
                seb.RefreshSideEffectExecutesDict();
                Initialize(cardInfo, seb, onRefreshText);
                onRefreshText();
                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer));
            });

            RefreshSideEffectBundleTransformButton.onClick.RemoveAllListeners();
            RefreshSideEffectBundleTransformButton.onClick.AddListener(delegate { StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<CardEditorPanel>().CardPropertiesContainer)); }
            );

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