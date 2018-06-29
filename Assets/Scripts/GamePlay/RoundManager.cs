using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    private static RoundManager rm;

    public static RoundManager RM
    {
        get
        {
            if (!rm)
            {
                rm = FindObjectOfType(typeof(RoundManager)) as RoundManager;
            }

            return rm;
        }
    }

    internal int RoundNumber;
    internal Player CurrentPlayer;
    public GameObject SelfTurnText;
    public GameObject EnemyTurnText;
    public Text SelfCostText;
    public Text EnemyCostText;

    Vector3 selfTurnText_DefaultPosition;
    Vector3 enemyTurnText_DefaultPosition;
    Vector3 farAwayPosition;

    void Awake()
    {
        selfTurnText_DefaultPosition = SelfTurnText.transform.position;
        enemyTurnText_DefaultPosition = EnemyTurnText.transform.position;
        farAwayPosition = new Vector3(-100, -100, -100);
    }

    private void Start()
    {
    }

    void Update()
    {
    }

    public void GameStart()
    {
        CurrentPlayer = Random.Range(0, 2) == 0 ? GameManager.GM.SelfPlayer : GameManager.GM.EnemyPlayer;
        CurrentPlayer.MyHandManager.GetACardByID(99);
        CurrentPlayer.MyHandManager.DrawRetinueCard();
        CurrentPlayer.MyHandManager.DrawCards(2);
        EndRound();
        switchPlayer();
        CurrentPlayer.MyHandManager.GetACardByID(99);
        CurrentPlayer.MyHandManager.DrawRetinueCard();
        CurrentPlayer.MyHandManager.DrawCards(3);
        EndRound();
        switchPlayer();
        BeginRound();
    }


    public void BeginRound()
    {
        CurrentPlayer.IncreaseCostMax(1);
        CurrentPlayer.AddAllCost();
        if (CurrentPlayer == GameManager.GM.SelfPlayer)
        {
            SelfTurnText.transform.position = selfTurnText_DefaultPosition;
            EnemyTurnText.transform.position = farAwayPosition;
        }
        else
        {
            EnemyTurnText.transform.position = enemyTurnText_DefaultPosition;
            SelfTurnText.transform.position = farAwayPosition;
        }

        CurrentPlayer.MyHandManager.DrawCards(GameManager.GM.DrawCardPerRound);

        CurrentPlayer.MyHandManager.BeginRound();
        CurrentPlayer.MyBattleGroundManager.BeginRound();
    }

    public void EndRound()
    {
        CurrentPlayer.MyHandManager.EndRound();
        CurrentPlayer.MyBattleGroundManager.EndRound();
    }

    void switchPlayer()
    {
        CurrentPlayer = CurrentPlayer == GameManager.GM.SelfPlayer ? GameManager.GM.EnemyPlayer : GameManager.GM.SelfPlayer;
    }

    public void OnEndRoundButtonClick()
    {
        EndRound();
        switchPlayer();
        BeginRound();
    }
}