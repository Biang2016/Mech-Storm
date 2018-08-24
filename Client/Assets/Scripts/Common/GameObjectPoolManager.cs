using UnityEngine;

internal class GameObjectPoolManager : MonoSingletion<GameObjectPoolManager>
{
    private GameObjectPoolManager()
    {
    }

    public GameObject ColliderReplacePool;
    internal GameObjectPool Pool_ColliderReplacePool;
    public GameObject ColliderReplacePrefab;

    public GameObject CardNumberSetPool;
    internal GameObjectPool Pool_CardNumberSetPool;
    public GameObject CardNumberSetPrefab;

    public GameObject CardSmallNumberPool;
    internal GameObjectPool Pool_CardSmallNumberPool;
    public GameObject CardSmallNumberPrefab;

    public GameObject CardMediumNumberPool;
    internal GameObjectPool Pool_CardMediumNumberPool;
    public GameObject CardMediumNumberPrefab;

    public GameObject CardBigNumberPool;
    internal GameObjectPool Pool_CardBigNumberPool;
    public GameObject CardBigNumberPrefab;

    public GameObject ArrowAimingPool;
    internal GameObjectPool Pool_ArrowAimingPool;
    public GameObject ArrowAimingPrefab;

    public GameObject ArrowArrowPool;
    internal GameObjectPool Pool_ArrowArrowPool;
    public GameObject ArrowArrowPrefab;

    public GameObject ModuleRetinuePool;
    internal GameObjectPool Pool_ModuleRetinuePool;
    public GameObject ModuleRetinuePrefab;

    public GameObject ModuleEnergyPool;
    internal GameObjectPool Pool_ModuleEnergyPool;
    public GameObject ModuleEnergyPrefab;

    public GameObject ModuleMAPool;
    internal GameObjectPool Pool_ModuleMAPool;
    public GameObject ModuleMAPrefab;

    public GameObject ModulePackPool;
    internal GameObjectPool Pool_ModulePackPool;
    public GameObject ModulePackPrefab;

    public GameObject ModuleWeaponPool;
    internal GameObjectPool Pool_ModuleWeaponPool;
    public GameObject ModuleWeaponPrefab;

    public GameObject ModuleWeaponDetailPool;
    internal GameObjectPool Pool_ModuleWeaponDetailPool;
    public GameObject ModuleWeaponDetailPrefab;

    public GameObject ModuleShieldPool;
    internal GameObjectPool Pool_ModuleShieldPool;
    public GameObject ModuleShieldPrefab;

    public GameObject ModuleShieldDetailPool;
    internal GameObjectPool Pool_ModuleShieldDetailPool;
    public GameObject ModuleShieldDetailPrefab;

    public GameObject RetinueCardPool;
    internal GameObjectPool Pool_RetinueCardPool;
    public GameObject RetinueCardPrefab;

    public GameObject WeaponCardPool;
    internal GameObjectPool Pool_WeaponCardPool;
    public GameObject WeaponCardPrefab;

    public GameObject ShieldCardPool;
    internal GameObjectPool Pool_ShieldCardPool;
    public GameObject ShieldCardPrefab;

    public GameObject SpellCardPool;
    internal GameObjectPool Pool_SpellCardPool;
    public GameObject SpellCardPrefab;

    public GameObject PackCardPool;
    internal GameObjectPool Pool_PackCardPool;
    public GameObject PackCardPrefab;

    public GameObject MACardPool;
    internal GameObjectPool Pool_MACardPool;
    public GameObject MACardPrefab;

    public GameObject RetinueSelectCardPool;
    internal GameObjectPool Pool_RetinueSelectCardPool;
    public GameObject RetinueSelectCardPrefab;

    public GameObject WeaponSelectCardPool;
    internal GameObjectPool Pool_WeaponSelectCardPool;
    public GameObject WeaponSelectCardPrefab;

    public GameObject ShieldSelectCardPool;
    internal GameObjectPool Pool_ShieldSelectCardPool;
    public GameObject ShieldSelectCardPrefab;

    public GameObject SpellSelectCardPool;
    internal GameObjectPool Pool_SpellSelectCardPool;
    public GameObject SpellSelectCardPrefab;

    public GameObject SelectCardPool;
    internal GameObjectPool Pool_SelectCardPool;
    public GameObject SelectCardPrefab;

    public GameObject BuildButtonPool;
    internal GameObjectPool Pool_BuildButtonPool;
    public GameObject BuildButtonPrefab;

    public GameObject CardDeckCardPool;
    internal GameObjectPool Pool_CardDeckCardPool;
    public GameObject CardDeckCardPrefab;

    void Awake()
    {
        Pool_ColliderReplacePool = ColliderReplacePool.GetComponent<GameObjectPool>();
        Pool_ColliderReplacePool.Initiate(ColliderReplacePrefab, 20);

        Pool_CardNumberSetPool = CardNumberSetPool.GetComponent<GameObjectPool>();
        Pool_CardNumberSetPool.Initiate(CardNumberSetPrefab, 60);

        Pool_CardSmallNumberPool = CardSmallNumberPool.GetComponent<GameObjectPool>();
        Pool_CardSmallNumberPool.Initiate(CardSmallNumberPrefab, 180);

        Pool_CardMediumNumberPool = CardMediumNumberPool.GetComponent<GameObjectPool>();
        Pool_CardMediumNumberPool.Initiate(CardMediumNumberPrefab, 180);

        Pool_CardBigNumberPool = CardBigNumberPool.GetComponent<GameObjectPool>();
        Pool_CardBigNumberPool.Initiate(CardBigNumberPrefab, 180);

        Pool_ArrowArrowPool = ArrowArrowPool.GetComponent<GameObjectPool>();
        Pool_ArrowArrowPool.Initiate(ArrowArrowPrefab, 2);

        Pool_ArrowAimingPool = ArrowAimingPool.GetComponent<GameObjectPool>();
        Pool_ArrowAimingPool.Initiate(ArrowAimingPrefab, 2);

        Pool_ModuleRetinuePool = ModuleRetinuePool.GetComponent<GameObjectPool>();
        Pool_ModuleRetinuePool.Initiate(ModuleRetinuePrefab, 14);

        Pool_ModuleEnergyPool = ModuleEnergyPool.GetComponent<GameObjectPool>();
        Pool_ModuleEnergyPool.Initiate(ModuleEnergyPrefab, 30);

        Pool_ModuleMAPool = ModuleMAPool.GetComponent<GameObjectPool>();
        Pool_ModuleMAPool.Initiate(ModuleMAPrefab, 30);

        Pool_ModulePackPool = ModulePackPool.GetComponent<GameObjectPool>();
        Pool_ModulePackPool.Initiate(ModulePackPrefab, 30);

        Pool_ModuleWeaponPool = ModuleWeaponPool.GetComponent<GameObjectPool>();
        Pool_ModuleWeaponPool.Initiate(ModuleWeaponPrefab, 30);

        Pool_ModuleWeaponDetailPool = ModuleWeaponDetailPool.GetComponent<GameObjectPool>();
        Pool_ModuleWeaponDetailPool.Initiate(ModuleWeaponDetailPrefab, 3);

        Pool_ModuleShieldPool = ModuleShieldPool.GetComponent<GameObjectPool>();
        Pool_ModuleShieldPool.Initiate(ModuleShieldPrefab, 30);

        Pool_ModuleShieldDetailPool = ModuleShieldDetailPool.GetComponent<GameObjectPool>();
        Pool_ModuleShieldDetailPool.Initiate(ModuleShieldDetailPrefab, 3);

        Pool_RetinueCardPool = RetinueCardPool.GetComponent<GameObjectPool>();
        Pool_RetinueCardPool.Initiate(RetinueCardPrefab, 30);

        Pool_WeaponCardPool = WeaponCardPool.GetComponent<GameObjectPool>();
        Pool_WeaponCardPool.Initiate(WeaponCardPrefab, 30);

        Pool_ShieldCardPool = ShieldCardPool.GetComponent<GameObjectPool>();
        Pool_ShieldCardPool.Initiate(ShieldCardPrefab, 30);

        Pool_SpellCardPool = SpellCardPool.GetComponent<GameObjectPool>();
        Pool_SpellCardPool.Initiate(SpellCardPrefab, 30);

        Pool_PackCardPool = PackCardPool.GetComponent<GameObjectPool>();
        Pool_PackCardPool.Initiate(PackCardPrefab, 30);

        Pool_MACardPool = MACardPool.GetComponent<GameObjectPool>();
        Pool_MACardPool.Initiate(MACardPrefab, 30);

        Pool_RetinueSelectCardPool = RetinueSelectCardPool.GetComponent<GameObjectPool>();
        Pool_RetinueSelectCardPool.Initiate(RetinueSelectCardPrefab, 30);

        Pool_WeaponSelectCardPool = WeaponSelectCardPool.GetComponent<GameObjectPool>();
        Pool_WeaponSelectCardPool.Initiate(WeaponSelectCardPrefab, 30);

        Pool_ShieldSelectCardPool = ShieldSelectCardPool.GetComponent<GameObjectPool>();
        Pool_ShieldSelectCardPool.Initiate(ShieldSelectCardPrefab, 30);

        Pool_SpellSelectCardPool = SpellSelectCardPool.GetComponent<GameObjectPool>();
        Pool_SpellSelectCardPool.Initiate(SpellSelectCardPrefab, 30);

        Pool_SelectCardPool = SelectCardPool.GetComponent<GameObjectPool>();
        Pool_SelectCardPool.Initiate(SelectCardPrefab, 20);

        Pool_BuildButtonPool = BuildButtonPool.GetComponent<GameObjectPool>();
        Pool_BuildButtonPool.Initiate(BuildButtonPrefab, 10);

        Pool_CardDeckCardPool = CardDeckCardPool.GetComponent<GameObjectPool>();
        Pool_CardDeckCardPool.Initiate(CardDeckCardPrefab, 20);
    }
}

public interface IGameObjectPool
{
    void PoolRecycle();
}