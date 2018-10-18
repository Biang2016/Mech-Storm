public class GameObjectPoolManager : MonoSingletion<GameObjectPoolManager>
{
    private GameObjectPoolManager()
    {
    }

    public GameObjectPool Pool_ColliderReplacePool;
    public PoolObject ColliderReplacePrefab;

    public GameObjectPool Pool_CardNumberSetPool;
    public PoolObject CardNumberSetPrefab;

    public GameObjectPool Pool_CardSmallNumberPool;
    public PoolObject CardSmallNumberPrefab;

    public GameObjectPool Pool_CardMediumNumberPool;
    public PoolObject CardMediumNumberPrefab;

    public GameObjectPool Pool_CardBigNumberPool;
    public PoolObject CardBigNumberPrefab;

    public GameObjectPool Pool_ArrowAimingPool;
    public PoolObject ArrowAimingPrefab;

    public GameObjectPool Pool_ArrowArrowPool;
    public PoolObject ArrowArrowPrefab;

    public GameObjectPool Pool_ModuleRetinuePool;
    public PoolObject ModuleRetinuePrefab;

    public GameObjectPool Pool_ModuleWeaponPool;
    public PoolObject ModuleWeaponPrefab;

    public GameObjectPool Pool_ModuleWeaponDetailPool;
    public PoolObject ModuleWeaponDetailPrefab;

    public GameObjectPool Pool_ModuleShieldPool;
    public PoolObject ModuleShieldPrefab;

    public GameObjectPool Pool_ModuleShieldDetailPool;
    public PoolObject ModuleShieldDetailPrefab;

    public GameObjectPool Pool_ModulePackPool;
    public PoolObject ModulePackPrefab;

    public GameObjectPool Pool_ModulePackDetailPool;
    public PoolObject ModulePackDetailPrefab;

    public GameObjectPool Pool_ModuleMAPool;
    public PoolObject ModuleMAPrefab;

    public GameObjectPool Pool_ModuleMADetailPool;
    public PoolObject ModuleMADetailPrefab;

    public GameObjectPool Pool_RetinueCardPool;
    public PoolObject RetinueCardPrefab;

    public GameObjectPool Pool_EquipCardPool;
    public PoolObject EquipCardPrefab;

    public GameObjectPool Pool_SpellCardPool;
    public PoolObject SpellCardPrefab;

    public GameObjectPool Pool_RetinueCardSelectPool;
    public PoolObject RetinueCardSelectPrefab;

    public GameObjectPool Pool_EquipCardSelectPool;
    public PoolObject EquipCardSelectPrefab;

    public GameObjectPool Pool_SpellCardSelectPool;
    public PoolObject SpellCardSelectPrefab;

    public GameObjectPool Pool_SelectCardPool;
    public PoolObject SelectCardPrefab;

    public GameObjectPool Pool_BuildButtonPool;
    public PoolObject BuildButtonPrefab;

    public GameObjectPool Pool_CardDeckCardPool;
    public PoolObject CardDeckCardPrefab;

    public GameObjectPool Pool_MetalBarBlockPool;
    public PoolObject MetalBarBlockPrefab;

    public GameObjectPool Pool_TextFlyPool;
    public PoolObject TextFlyPrefab;

    public GameObjectPool[] Pool_HitPool;
    public PoolObject[] HitPrefab;

    public GameObjectPool Pool_AffixPool;
    public PoolObject AffixPrefab;

    public GameObjectPool Pool_ConfirmWindowPool;
    public PoolObject ConfirmWindowPrefab;

    public GameObjectPool Pool_PlayerBuffPool;
    public PoolObject PlayerBuffPrefab;


    void Awake()
    {
        Pool_ColliderReplacePool.Initiate(ColliderReplacePrefab, 20);

        Pool_CardNumberSetPool.Initiate(CardNumberSetPrefab, 60);

        Pool_CardSmallNumberPool.Initiate(CardSmallNumberPrefab, 180);

        Pool_CardMediumNumberPool.Initiate(CardMediumNumberPrefab, 180);

        Pool_CardBigNumberPool.Initiate(CardBigNumberPrefab, 180);

        Pool_ArrowArrowPool.Initiate(ArrowArrowPrefab, 2);

        Pool_ArrowAimingPool.Initiate(ArrowAimingPrefab, 2);

        Pool_ModuleRetinuePool.Initiate(ModuleRetinuePrefab, 14);

        Pool_ModuleWeaponPool.Initiate(ModuleWeaponPrefab, 14);

        Pool_ModuleWeaponDetailPool.Initiate(ModuleWeaponDetailPrefab, 1);

        Pool_ModuleShieldPool.Initiate(ModuleShieldPrefab, 14);

        Pool_ModuleShieldDetailPool.Initiate(ModuleShieldDetailPrefab, 1);

        Pool_ModulePackPool.Initiate(ModulePackPrefab, 5);

        Pool_ModulePackDetailPool.Initiate(ModulePackDetailPrefab, 1);

        Pool_ModuleMAPool.Initiate(ModuleMAPrefab, 5);

        Pool_ModuleMADetailPool.Initiate(ModuleMADetailPrefab, 1);

        Pool_RetinueCardPool.Initiate(RetinueCardPrefab, 20);

        Pool_EquipCardPool.Initiate(EquipCardPrefab, 20);

        Pool_SpellCardPool.Initiate(SpellCardPrefab, 20);

        Pool_RetinueCardSelectPool.Initiate(RetinueCardSelectPrefab, 20);

        Pool_EquipCardSelectPool.Initiate(EquipCardSelectPrefab, 20);

        Pool_SpellCardSelectPool.Initiate(SpellCardSelectPrefab, 20);

        Pool_SelectCardPool.Initiate(SelectCardPrefab, 20);

        Pool_BuildButtonPool.Initiate(BuildButtonPrefab, 10);

        Pool_CardDeckCardPool.Initiate(CardDeckCardPrefab, 20);

        Pool_MetalBarBlockPool.Initiate(MetalBarBlockPrefab, 20);

        Pool_TextFlyPool.Initiate(TextFlyPrefab, 10);

        for (int i = 0; i < Pool_HitPool.Length; i++)
        {
            Pool_HitPool[i].Initiate(HitPrefab[i], 3);
        }

        Pool_AffixPool.Initiate(AffixPrefab, 10);

        Pool_ConfirmWindowPool.Initiate(ConfirmWindowPrefab, 3);

        Pool_PlayerBuffPool.Initiate(PlayerBuffPrefab, 5);
    }
}