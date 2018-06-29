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

    void Awake()
    {
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
        DrawCardPhase();
    }


    public void BeginRound()
    {
        CurrentPlayer.IncreaseCostMax(1);
        CurrentPlayer.AddAllCost();
        if (CurrentPlayer == GameManager.GM.SelfPlayer)
        {
            SelfTurnText.SetActive(true);
            EnemyTurnText.SetActive(false);
        }
        else
        {
            EnemyTurnText.SetActive(true);
            SelfTurnText.SetActive(false);
        }


        CurrentPlayer.MyHandManager.BeginRound();
        CurrentPlayer.MyBattleGroundManager.BeginRound();
    }

    public void DrawCardPhase() //抽牌阶段
    {
        CurrentPlayer.MyHandManager.DrawCards(GameManager.GM.DrawCardPerRound);
    }

    public void DropCardPhase() //弃牌阶段
    {
    }

    #region 回调函数

    public void OnDrawACard() //每次抽调用
    {

    }

    public void OnPlayACard() //每次出牌调用
    {

    }

    public void OnBeforeAttack() //每次执行攻击前调用
    {

    }

    public void OnArmorDamage() //每次护甲扣血
    {

    }

    public void OnShieldDamage() //每次护盾扣血
    {

    }

    public void OnRetinueDamage() //每次随从扣血
    {

    }

    public void OnLifeDamage() //每次英雄扣血
    {

    }

    public void OnDamage() //每次造成伤害
    {

    }

    public void OnAfterAttack() //每次执行攻击后调用
    {

    }

    #endregion

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
        DropCardPhase();
        EndRound();
        switchPlayer();
        BeginRound();
        DrawCardPhase();
    }
}

public interface IRoundPhaseCallBack
{
    void OnDrawACard(); //每次抽调用
    void OnPlayACard(); //每次出牌调用
    void OnBeforeAttack(); //每次执行攻击前调用
    void OnArmorDamage(); //每次护甲扣血
    void OnShieldDamage(); //每次护盾扣血
    void OnRetinueDamage(); //每次随从扣血
    void OnLifeDamage(); //每次英雄扣血
    void OnDamage(); //每次造成伤害
    void OnAfterAttack(); //每次执行攻击后调用
}