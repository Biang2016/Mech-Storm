using UnityEngine;

public class BattlePlayer : PoolObject
{
    public override void PoolRecycle()
    {
        ResetAll();
        base.PoolRecycle();
    }

    internal ClientPlayer ClientPlayer;

    public HandManager HandManager; //卡牌所属的手部区管理器
    public BattleGroundManager BattleGroundManager; //卡牌所属方的战场区域管理器
    public MetalLifeEnergyManager MetalLifeEnergyManager; //Metal、Energy、Life条的管理器
    public PlayerBuffManager PlayerBuffManager; //Buff的管理器
    public CoolDownCardManager PlayerCoolDownCardManager; //冷却卡片的管理器
    public CardDeckManager CardDeckManager; //卡组管理器
    public Ship Ship;

    internal BoardAreaTypes BattleGroundArea; //卡牌所属方的战场区
    internal BoardAreaTypes HandArea; //卡牌所属的手部区

    void Awake()
    {
        RefreshBattlePlayerSize();
    }

    public void Initialize(ClientPlayer clientPlayer)
    {
        ResetAll();
        RefreshBattlePlayerSize();
        ClientPlayer = clientPlayer;
        transform.rotation = ClientPlayer.WhichPlayer == Players.Self ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        HandManager.Initialize(ClientPlayer);
        BattleGroundManager.Initialize(ClientPlayer);
        MetalLifeEnergyManager.Initialize(ClientPlayer);
        PlayerBuffManager.Initialize(ClientPlayer);
        PlayerCoolDownCardManager.Initialize(ClientPlayer);
        CardDeckManager.Initialize(ClientPlayer);
        Ship.Initialize(ClientPlayer);

        HandArea = clientPlayer.WhichPlayer == Players.Self ? BoardAreaTypes.SelfHandArea : BoardAreaTypes.EnemyHandArea;
        BattleGroundArea = clientPlayer.WhichPlayer == Players.Self ? BoardAreaTypes.SelfBattleGroundArea : BoardAreaTypes.EnemyBattleGroundArea;
    }

    public void RefreshBattlePlayerSize()
    {
        float screenScale = ((float) Screen.width / Screen.height) / (16.0f / 9.0f);
        transform.localScale = Vector3.one * screenScale;
    }

    public void ResetAll()
    {
        HandManager.StopAllCoroutines();
        BattleGroundManager.StopAllCoroutines();
        MetalLifeEnergyManager.StopAllCoroutines();
        PlayerBuffManager.StopAllCoroutines();
        CardDeckManager.StopAllCoroutines();
        Ship.StopAllCoroutines();
        ClientPlayer = null;
        HandManager.ResetAll();
        BattleGroundManager.ResetAll();
        MetalLifeEnergyManager.ResetAll();
        PlayerBuffManager.ResetAll();
        PlayerCoolDownCardManager.ResetAll();
        CardDeckManager.ResetAll();
        Ship.ResetAll();
    }

    public void ShowBattleShip()
    {
        gameObject.SetActive(true);
    }

    public void HideBattleShip()
    {
        gameObject.SetActive(false);
    }
}