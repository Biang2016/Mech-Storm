using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
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

    public HandManager SelfHandManager;
    public HandManager EnemyHandManager;
    public BattleGroundManager SelfBattleGroundManager;
    public BattleGroundManager EnemyBattleGroundManager;
    public CardDeckManager SelfCardDeckManager;
    public CardDeckManager EnemyCardDeckManager;
    public GameObject CardDetailPreview;

    void Awake()
    {
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