using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class GameBoardManager : MonoBehaviour
{
    private static GameBoardManager gbm;

    public static GameBoardManager GBM
    {
        get
        {
            if (!gbm)
            {
                gbm = FindObjectOfType(typeof(GameBoardManager)) as GameBoardManager;
            }

            return gbm;
        }
    }

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

    void Start()
    {
    }

    void Update()
    {
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