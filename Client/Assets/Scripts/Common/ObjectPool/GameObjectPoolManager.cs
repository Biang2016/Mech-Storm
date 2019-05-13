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
        
        TextFly,
        Affix,
        ParticleSystem,
        Bullet,
        
        BigBonusItem,
        SmallBonusItem,
        BonusButton,
        
        StartMenuButton,
        ExitMenuButton,
        
        CardPreviewButton,
        PicPreviewButton,
        CardPropertyFormRow_InputField,
        CardPropertyFormRow_Dropdown,
        CardPropertyFormRow_Toggle,
        CardPropertyFormRow_TwoToggleRow,
        CardPropertyForm_SideEffectBundle,
        CardPropertyForm_SideEffectExecute,
        CardPropertyForm_ExecuteSetting,
        CardPropertyForm_SideEffect,
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
        
        {PrefabNames.TextFly, 10},
        {PrefabNames.Affix, 5},
        {PrefabNames.ParticleSystem, 5},
        {PrefabNames.Bullet, 2},

        {PrefabNames.BigBonusItem, 3},
        {PrefabNames.SmallBonusItem, 5},
        {PrefabNames.BonusButton, 3},
        
        {PrefabNames.StartMenuButton, 10},
        {PrefabNames.ExitMenuButton, 10},
        
        {PrefabNames.CardPreviewButton, 50},
        {PrefabNames.PicPreviewButton, 50},
        {PrefabNames.CardPropertyFormRow_InputField, 10},
        {PrefabNames.CardPropertyFormRow_Dropdown, 5},
        {PrefabNames.CardPropertyFormRow_Toggle, 3},
        {PrefabNames.CardPropertyFormRow_TwoToggleRow, 3},
        {PrefabNames.CardPropertyForm_SideEffectBundle, 2},
        {PrefabNames.CardPropertyForm_SideEffectExecute, 2},
        {PrefabNames.CardPropertyForm_ExecuteSetting, 2},
        {PrefabNames.CardPropertyForm_SideEffect, 2},
    };

    public Dictionary<PrefabNames, int> PoolWarmUpDict = new Dictionary<PrefabNames, int>
    {
        {PrefabNames.MechCard, 30},
        {PrefabNames.EquipCard, 20},
        {PrefabNames.SpellCard, 30},
        { PrefabNames.CardSelectWindowCardContainer, 30},
        {PrefabNames.SelectCard, 50},
    };

    public Dictionary<PrefabNames, GameObjectPool> PoolDict = new Dictionary<PrefabNames, GameObjectPool>();

    public GameObjectPool[] Pool_HitPool;
    public PoolObject[] HitPrefab;

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

        for (int i = 0; i < Pool_HitPool.Length; i++)
        {
            Pool_HitPool[i].Initiate(HitPrefab[i], 3);
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