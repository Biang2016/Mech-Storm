using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelPropertyForm_CardSelection : PropertyFormRow
{
    private List<LevelEditorPanel_HeroCardPicIcon> HeroCardPicIcons = new List<LevelEditorPanel_HeroCardPicIcon>();
    [SerializeField] private Transform HeroCardPicIconContainer;

    private List<LevelEditorPanel_TypeCardCount> TypeCardCounts = new List<LevelEditorPanel_TypeCardCount>();
    [SerializeField] private Transform TypeCardCountContainer;

    private List<LevelEditorPanel_CostStatBar> CostStatBars_Metal = new List<LevelEditorPanel_CostStatBar>();
    [SerializeField] private Transform CostStatBarContainer_Metal;
    private List<LevelEditorPanel_CostStatBar> CostStatBars_Energy = new List<LevelEditorPanel_CostStatBar>();
    [SerializeField] private Transform CostStatBarContainer_Energy;

    [SerializeField] private Button GoToButton;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        Clear();
        BuildCards = null;
        GoToButton.onClick.RemoveAllListeners();
    }

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKey(Label, "LevelEditorPanel_CardSelection");
    }

    protected override void SetValue(string value_str)
    {
    }

    private BuildInfo.BuildCards BuildCards;

    public void Initialize(BuildInfo.BuildCards buildCards, UnityAction gotoAction)
    {
        BuildCards = buildCards;
        Refresh();
        GoToButton.onClick.RemoveAllListeners();
        GoToButton.onClick.AddListener(gotoAction);
    }

    private void Clear()
    {
        foreach (LevelEditorPanel_HeroCardPicIcon hcpi in HeroCardPicIcons)
        {
            hcpi.PoolRecycle();
        }

        HeroCardPicIcons.Clear();

        foreach (LevelEditorPanel_TypeCardCount tcc in TypeCardCounts)
        {
            tcc.PoolRecycle();
        }

        TypeCardCounts.Clear();

        foreach (LevelEditorPanel_CostStatBar csb in CostStatBars_Metal)
        {
            csb.PoolRecycle();
        }

        CostStatBars_Metal.Clear();

        foreach (LevelEditorPanel_CostStatBar csb in CostStatBars_Energy)
        {
            csb.PoolRecycle();
        }

        CostStatBars_Energy.Clear();
    }

    public void Refresh()
    {
        Clear();

        foreach (int heroCardID in BuildCards.GetHeroCardIDs())
        {
            int pid = AllCards.GetCard(heroCardID).BaseInfo.PictureID;
            LevelEditorPanel_HeroCardPicIcon heroCardPicIcon = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelEditorPanel_HeroCardPicIcon].AllocateGameObject<LevelEditorPanel_HeroCardPicIcon>(HeroCardPicIconContainer);
            heroCardPicIcon.Initialize(pid, heroCardID);
            HeroCardPicIcons.Add(heroCardPicIcon);
        }

        foreach (KeyValuePair<CardStatTypes, int> kv in BuildCards.GetTypeCardCountDict())
        {
            LevelEditorPanel_TypeCardCount typeCardCount = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelEditorPanel_TypeCardCount].AllocateGameObject<LevelEditorPanel_TypeCardCount>(TypeCardCountContainer);
            typeCardCount.Initialize(
                kv.Key,
                kv.Value,
                delegate
                {
                    foreach (LevelEditorPanel_TypeCardCount tcc in TypeCardCounts)
                    {
                        tcc.IsSelected = false;
                    }

                    typeCardCount.IsSelected = true;
                    RefreshBars(typeCardCount.CardStatType);
                });

            typeCardCount.IsSelected = typeCardCount.CardStatType == CardStatTypes.Total;
            TypeCardCounts.Add(typeCardCount);
        }

        RefreshBars(CardStatTypes.Total);
    }

    private void RefreshBars(CardStatTypes cardStatType)
    {
        foreach (LevelEditorPanel_CostStatBar csb in CostStatBars_Metal)
        {
            csb.PoolRecycle();
        }

        CostStatBars_Metal.Clear();

        foreach (LevelEditorPanel_CostStatBar csb in CostStatBars_Energy)
        {
            csb.PoolRecycle();
        }

        CostStatBars_Energy.Clear();
        SortedDictionary<int, int> costDict_Metal = BuildCards.GetCostDictByMetal(cardStatType);
        int maxCount_Metal = 0;
        foreach (KeyValuePair<int, int> kv in costDict_Metal)
        {
            if (kv.Key == 0) continue;
            maxCount_Metal = Mathf.Max(maxCount_Metal, kv.Value);
        }

        foreach (KeyValuePair<int, int> kv in BuildCards.GetCostDictByMetal(cardStatType))
        {
            LevelEditorPanel_CostStatBar csb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelEditorPanel_CostStatBar].AllocateGameObject<LevelEditorPanel_CostStatBar>(CostStatBarContainer_Metal);
            csb.Initialize(kv.Key, kv.Value, kv.Key == 0 ? kv.Value : maxCount_Metal, LevelEditorPanel_CostStatBar.ColorTypes.Metal);
            CostStatBars_Metal.Add(csb);
        }

        SortedDictionary<int, int> costDict_Energy = BuildCards.GetCostDictByEnergy(cardStatType);
        int maxCount_Energy = 0;
        foreach (KeyValuePair<int, int> kv in costDict_Energy)
        {
            if (kv.Key == 0) continue;
            maxCount_Energy = Mathf.Max(maxCount_Energy, kv.Value);
        }

        foreach (KeyValuePair<int, int> kv in BuildCards.GetCostDictByEnergy(cardStatType))
        {
            LevelEditorPanel_CostStatBar csb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelEditorPanel_CostStatBar].AllocateGameObject<LevelEditorPanel_CostStatBar>(CostStatBarContainer_Energy);
            csb.Initialize(kv.Key, kv.Value, kv.Key == 0 ? kv.Value : maxCount_Energy, LevelEditorPanel_CostStatBar.ColorTypes.Energy);
            CostStatBars_Energy.Add(csb);
        }
    }
}