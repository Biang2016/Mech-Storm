using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolManager : MonoSingleton<GameObjectPoolManager>
{
    private GameObjectPoolManager()
    {
    }

    public enum PrefabNames
    {
        BattlePlayer,
        MetalBarBlock,
        PlayerBuff,
        CoolDownCard,

        ArrowAiming,
        ArrowArrow,

        ModuleMech,
        MechBattleInfoIcon,
        MechTriggerIcon,
        ModuleWeapon,
        ModuleWeaponDetail,
        ModuleShield,
        ModuleShieldDetail,
        ModulePack,
        ModulePackDetail,
        ModuleMA,
        ModuleMADetail,

        MechCard,
        EquipCard,
        SpellCard,
        CardDeckCard,
        ColliderReplace,

        CardSelectWindowCardContainer,
        SelectCard,
        BuildButton,

        FX_Hit0,
        FX_Hit1,
        FX_Blade1,
        FX_Blade2,
        FX_WeaponEnergyUp,
        FX_ShipAddEnergy,
        FX_Particle,
        FX_GunBullet,
        FX_GunBulletHit,
        FX_Shield,
        FX_MechAddLife,

        TextFly,

        Affix,

        BigBonusItem,
        SmallBonusItem,
        BonusButton,

        ShopItemButton_Card,
        ShopItemButton_Small,
        ShopItemSmallContainer,

        StartMenuButton,
        ExitMenuButton,

        PicPreviewButton,
        PropertyFormRow_InputField,
        PropertyFormRow_Dropdown,
        PropertyFormRow_Toggle,
        PropertyFormRow_TwoToggleRow,

        CardEditorPanel_CardPreviewButton,
        CardPropertyForm_SideEffectBundle,
        CardPropertyForm_SideEffectExecute,
        CardPropertyForm_ExecuteSetting,
        CardPropertyForm_SideEffect,

        LevelPropertyForm_CardSelection,
        LevelEditorPanel_HeroCardPicIcon,
        LevelEditorPanel_TypeCardCount,
        LevelEditorPanel_CostStatBar,
        LevelPropertyForm_ShopItem,
        LevelPropertyForm_ShopItems,
        LevelPropertyForm_ShopItemTypeDropdown,
        LevelPropertyForm_BonusGroups,
        LevelPropertyForm_BonusGroup,
        LevelPropertyForm_Bonus,
        LevelPropertyForm_BonusTypeDropdown,

        StoryPropertyForm_GamePlaySettings,
        StoryPropertyForm_Chapters,
        StoryPropertyForm_Chapter,
        StoryEditorPanel_EnemyButton,
        StoryEditorPanel_LevelButtonSliderBar,
        ChapterMap,
        ChapterMapRoute,
        ChapterMapNode,

        TabControl_TabButton,
        TabControl_Panel,
        StoryEditorPanel_ShopButton,
    }

    public Dictionary<PrefabNames, int> PoolConfigs = new Dictionary<PrefabNames, int>
    {
        {PrefabNames.BattlePlayer, 2},
        {PrefabNames.MetalBarBlock, 20},
        {PrefabNames.PlayerBuff, 5},
        {PrefabNames.CoolDownCard, 6},

        {PrefabNames.ArrowAiming, 1},
        {PrefabNames.ArrowArrow, 1},

        {PrefabNames.ModuleMech, 12},
        {PrefabNames.MechBattleInfoIcon, 10},
        {PrefabNames.MechTriggerIcon, 10},
        {PrefabNames.ModuleWeapon, 12},
        {PrefabNames.ModuleWeaponDetail, 1},
        {PrefabNames.ModuleShield, 12},
        {PrefabNames.ModuleShieldDetail, 1},
        {PrefabNames.ModulePack, 12},
        {PrefabNames.ModulePackDetail, 1},
        {PrefabNames.ModuleMA, 12},
        {PrefabNames.ModuleMADetail, 1},

        {PrefabNames.MechCard, 5},
        {PrefabNames.EquipCard, 5},
        {PrefabNames.SpellCard, 5},
        {PrefabNames.CardDeckCard, 30},
        {PrefabNames.ColliderReplace, 20},

        {PrefabNames.CardSelectWindowCardContainer, 30},
        {PrefabNames.SelectCard, 50},
        {PrefabNames.BuildButton, 5},

        {PrefabNames.ShopItemButton_Card, 5},
        {PrefabNames.ShopItemButton_Small, 5},
        {PrefabNames.ShopItemSmallContainer, 5},

        {PrefabNames.FX_Hit0, 3},
        {PrefabNames.FX_Hit1, 3},
        {PrefabNames.FX_Blade1, 2},
        {PrefabNames.FX_Blade2, 2},
        {PrefabNames.FX_WeaponEnergyUp, 3},
        {PrefabNames.FX_ShipAddEnergy, 2},
        {PrefabNames.TextFly, 10},
        {PrefabNames.FX_Particle, 5},
        {PrefabNames.FX_GunBullet, 3},
        {PrefabNames.FX_GunBulletHit, 3},
        {PrefabNames.FX_Shield, 2},
        {PrefabNames.FX_MechAddLife, 2},

        {PrefabNames.Affix, 5},

        {PrefabNames.BigBonusItem, 3},
        {PrefabNames.SmallBonusItem, 5},
        {PrefabNames.BonusButton, 3},

        {PrefabNames.StartMenuButton, 10},
        {PrefabNames.ExitMenuButton, 10},

        {PrefabNames.CardEditorPanel_CardPreviewButton, 50},
        {PrefabNames.PicPreviewButton, 50},
        {PrefabNames.PropertyFormRow_InputField, 10},
        {PrefabNames.PropertyFormRow_Dropdown, 5},
        {PrefabNames.PropertyFormRow_Toggle, 3},
        {PrefabNames.PropertyFormRow_TwoToggleRow, 3},
        {PrefabNames.CardPropertyForm_SideEffectBundle, 2},
        {PrefabNames.CardPropertyForm_SideEffectExecute, 2},
        {PrefabNames.CardPropertyForm_ExecuteSetting, 2},
        {PrefabNames.CardPropertyForm_SideEffect, 2},

        {PrefabNames.LevelPropertyForm_CardSelection, 1},
        {PrefabNames.LevelEditorPanel_HeroCardPicIcon, 4},
        {PrefabNames.LevelEditorPanel_TypeCardCount, 5},
        {PrefabNames.LevelEditorPanel_CostStatBar, 11},
        {PrefabNames.LevelPropertyForm_ShopItem, 5},
        {PrefabNames.LevelPropertyForm_ShopItems, 1},
        {PrefabNames.LevelPropertyForm_ShopItemTypeDropdown, 1},
        {PrefabNames.LevelPropertyForm_BonusGroups, 2},
        {PrefabNames.LevelPropertyForm_BonusGroup, 5},
        {PrefabNames.LevelPropertyForm_Bonus, 5},
        {PrefabNames.LevelPropertyForm_BonusTypeDropdown, 1},

        {PrefabNames.StoryPropertyForm_GamePlaySettings, 1},
        {PrefabNames.StoryPropertyForm_Chapters, 1},
        {PrefabNames.StoryPropertyForm_Chapter, 5},
        {PrefabNames.StoryEditorPanel_EnemyButton, 10},
        {PrefabNames.StoryEditorPanel_LevelButtonSliderBar, 10},

        {PrefabNames.ChapterMap, 2},
        {PrefabNames.ChapterMapRoute, 120},
        {PrefabNames.ChapterMapNode, 73},

        {PrefabNames.TabControl_TabButton, 5},
        {PrefabNames.TabControl_Panel, 5},
        {PrefabNames.StoryEditorPanel_ShopButton, 5},
    };

    public Dictionary<PrefabNames, int> PoolWarmUpDict = new Dictionary<PrefabNames, int>
    {
        {PrefabNames.MechCard, 30},
        {PrefabNames.EquipCard, 20},
        {PrefabNames.SpellCard, 30},
        {PrefabNames.CardSelectWindowCardContainer, 30},
        {PrefabNames.SelectCard, 50},
    };

    public Dictionary<PrefabNames, GameObjectPool> PoolDict = new Dictionary<PrefabNames, GameObjectPool>();

    void Awake()
    {
        foreach (KeyValuePair<PrefabNames, int> kv in PoolConfigs)
        {
            string prefabName = kv.Key.ToString();
            GameObject go = new GameObject("Pool_" + prefabName);
            GameObjectPool pool = go.AddComponent<GameObjectPool>();
            PoolDict.Add(kv.Key, pool);
            GameObject go_Prefab = PrefabManager.Instance.GetPrefab(prefabName);
            if (!go_Prefab)
            {
                ClientLog.Instance.PrintError("Prefab not found: " + prefabName);
                continue;
            }

            PoolObject po = go_Prefab.GetComponent<PoolObject>();
            if (!po)
            {
                ClientLog.Instance.PrintError("Prefab doesn't have PoolObject: " + prefabName);
                continue;
            }

            pool.Initiate(po, kv.Value);
            pool.transform.SetParent(transform);
        }
    }

    public void OptimizeAllGameObjectPools()
    {
        foreach (KeyValuePair<PrefabNames, GameObjectPool> kv in PoolDict)
        {
            kv.Value.OptimizePool();
        }
    }
}