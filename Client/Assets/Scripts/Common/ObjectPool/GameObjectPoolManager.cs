using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class GameObjectPoolManager : MonoSingleton<GameObjectPoolManager>
{
    private GameObjectPoolManager()
    {
    }

    public Dictionary<string, int> PoolConfigs = new Dictionary<string, int>
    {
        {"ColliderReplace", 20},
        {"NumberSet", 20},
        {"SmallNumber", 20},
        {"MediumNumber", 20},
        {"BigNumber", 20},
        {"ArrowAiming", 1},
        {"ArrowArrow", 1},
        {"Retinue", 12},
        {"Weapon", 12},
        {"WeaponDetail", 1},
        {"Shield", 12},
        {"ShieldDetail", 1},
        {"Pack", 12},
        {"PackDetail", 1},
        {"MA", 12},
        {"MADetail", 1},
        {"RetinueCard", 5},
        {"EquipCard", 5},
        {"SpellCard", 5},
        {"RetinueCardSelect", 20},
        {"EquipCardSelect", 20},
        {"SpellCardSelect", 20},
        {"SelectCard", 5},
        {"BuildButton", 5},
        {"CardDeckCard", 30},
        {"MetalBarBlock", 20},
        {"TextFly", 10},
        {"Affix", 5},
        {"PlayerBuff", 5},
        {"CoolDownCard", 6},
        {"ParticleSystem", 5},
        {"StoryLevelButton", 5},
        {"StoryLevelCol", 5},
        {"Bullet", 2},
        {"BigBonusItem", 3},
        {"SmallBonusItem", 5},
        {"BonusButton", 3},
        {"StartMenuButton", 10},
    };

    public Dictionary<string, GameObjectPool> PoolDict = new Dictionary<string, GameObjectPool>();

    public GameObjectPool[] Pool_HitPool;
    public PoolObject[] HitPrefab;

    void Awake()
    {
        foreach (KeyValuePair<string, int> kv in PoolConfigs)
        {
            GameObject go = new GameObject("Pool_" + kv.Key);
            GameObjectPool pool = go.AddComponent<GameObjectPool>();
            PoolDict.Add(kv.Key, pool);
            GameObject go_Prefab = PrefabManager.Instance.GetPrefab(kv.Key);
            if (!go_Prefab)
            {
                ClientLog.Instance.PrintError("Prefab not found: " + kv.Key);
                continue;
            }

            PoolObject po = go_Prefab.GetComponent<PoolObject>();
            if (!po)
            {
                ClientLog.Instance.PrintError("Prefab doesn't have PoolObject: " + kv.Key);
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
}