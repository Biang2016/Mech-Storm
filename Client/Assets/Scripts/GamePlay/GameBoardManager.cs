using UnityEngine;

internal class GameBoardManager : MonoSingletion<GameBoardManager>
{
    private GameBoardManager()
    {
    }

    internal HandManager SelfHandManager;
    internal HandManager EnemyHandManager;
    internal BattleGroundManager SelfBattleGroundManager;
    internal BattleGroundManager EnemyBattleGroundManager;
    internal GameObject CardDetailPreview;

    void Awake()
    {
        SelfHandManager = transform.Find("SelfHandArea").GetComponent<HandManager>();
        EnemyHandManager = transform.Find("EnemyHandArea").GetComponent<HandManager>();
        SelfBattleGroundManager = transform.Find("SelfBattleGroundArea").GetComponent<BattleGroundManager>();
        EnemyBattleGroundManager = transform.Find("EnemyBattleGroundArea").GetComponent<BattleGroundManager>();
        CardDetailPreview = transform.Find("CardDetailPreview").gameObject;
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