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

    public GameObject SelfHandArea;
    public HandManager SelfHandManager;
    public GameObject EnemyHandArea;
    public HandManager EnemyHandManager;
    public GameObject SelfBattleGround;
    public BattleGroundManager SelfBattleGroundManager;
    public GameObject EnemyBattleGround;
    public BattleGroundManager EnemyBattleGroundManager;
    public GameObject SelfCardDeck;
    public CardDeckManager SelfCardDeckManager;
    public GameObject EnemyCardDeck;
    public CardDeckManager EnemyCardDeckManager;
    public GameObject CardDetailPreview;

    void Awake()
    {
        SelfHandManager = SelfHandArea.GetComponent<HandManager>();
        EnemyHandManager = EnemyHandArea.GetComponent<HandManager>();
        SelfBattleGroundManager = SelfBattleGround.GetComponent<BattleGroundManager>();
        EnemyBattleGroundManager = EnemyBattleGround.GetComponent<BattleGroundManager>();
        SelfCardDeckManager = SelfCardDeck.GetComponent<CardDeckManager>();
        EnemyCardDeckManager = EnemyCardDeck.GetComponent<CardDeckManager>();
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