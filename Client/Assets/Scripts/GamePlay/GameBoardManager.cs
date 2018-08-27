using UnityEngine;

internal class GameBoardManager : MonoSingletion<GameBoardManager>
{
    private GameBoardManager()
    {
    }

    public HandManager SelfHandManager;
    public HandManager EnemyHandManager;
    public BattleGroundManager SelfBattleGroundManager;
    public BattleGroundManager EnemyBattleGroundManager;
    public CostLifeMagiceManager SelfCostLifeMagiceManager;
    public CostLifeMagiceManager EnemyCostLifeMagiceManager;
    public GameObject CardDetailPreview;

}

public enum BoardAreaTypes
{
    Others = 0,
    SelfHandArea = 1,
    EnemyHandArea = 2,
    SelfBattleGroundArea = 3,
    EnemyBattleGroundArea = 4,
}