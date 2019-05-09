using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoSingleton<BattleManager>
{
    private BattleManager()
    {
    }
    
    public bool ShowBEMMessages = false;

    internal BattlePlayer SelfBattlePlayer;
    internal BattlePlayer EnemyBattlePlayer;

    public ShowCardDetailInBattleManager ShowCardDetailInBattleManager;
    
    public void ResetAll()
    {
        SelfBattlePlayer?.PoolRecycle();
        EnemyBattlePlayer?.PoolRecycle();
        ShowCardDetailInBattleManager.HideCardDetail();
    }

    public void ShowBattleShips()
    {
        SelfBattlePlayer?.ShowBattleShip();
        EnemyBattlePlayer?.ShowBattleShip();
    }

    public void HideBattleShips()
    {
        SelfBattlePlayer?.HideBattleShip();
        EnemyBattlePlayer?.HideBattleShip();
    }
}

public enum BoardAreaTypes
{
    Others = 0,
    SelfHandArea = 1,
    EnemyHandArea = 2,
    SelfBattleGroundArea = 3,
    EnemyBattleGroundArea = 4,
}