using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    private static RoundManager rm;
    public static RoundManager RM {
        get {
            if (!rm) {
                rm = FindObjectOfType(typeof(RoundManager)) as RoundManager;
            }
            return rm;
        }
    }

    internal int RoundNumber;
    internal Player CurrentPlayer;
    public GameObject SelfTurnText;
    public GameObject EnemyTurnText;

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
        CurrentPlayer = Random.Range(0, 2) == 0 ? GameManager.GM.SelfPlayer : GameManager.GM.EnemyPlayer;
        CurrentPlayer.MyHandManager.GetACardByID(99);
        CurrentPlayer.MyHandManager.DrawRetinueCard();
        CurrentPlayer.MyHandManager.DrawCards(2);
        switchPlayer();
        CurrentPlayer.MyHandManager.GetACardByID(99);
        CurrentPlayer.MyHandManager.DrawRetinueCard();
        CurrentPlayer.MyHandManager.DrawCards(3);
        switchPlayer();
        BeginRound();
    }

    void Update()
    {

    }

    public void BeginRound()
    {
        CurrentPlayer.IncreaseCostMax(1);
        CurrentPlayer.AddAllCost();
        if (CurrentPlayer == GameManager.GM.SelfPlayer) {
            SelfTurnText.transform.position = selfTurnText_DefaultPosition;
            EnemyTurnText.transform.position = farAwayPosition;
        } else {
            EnemyTurnText.transform.position = enemyTurnText_DefaultPosition;
            SelfTurnText.transform.position = farAwayPosition;
        }

        CurrentPlayer.MyHandManager.DrawCard();

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
