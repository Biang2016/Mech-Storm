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

    public void DrawCardPhase() //���ƽ׶�
    {
        CurrentPlayer.MyHandManager.DrawCards(GameManager.GM.DrawCardPerRound);
    }

    public void DropCardPhase() //���ƽ׶�
    {
    }

    #region �ص�����

    public void OnDrawACard() //ÿ�γ����
    {

    }

    public void OnPlayACard() //ÿ�γ��Ƶ���
    {

    }

    public void OnBeforeAttack() //ÿ��ִ�й���ǰ����
    {

    }

    public void OnArmorDamage() //ÿ�λ��׿�Ѫ
    {

    }

    public void OnShieldDamage() //ÿ�λ��ܿ�Ѫ
    {

    }

    public void OnRetinueDamage() //ÿ����ӿ�Ѫ
    {

    }

    public void OnLifeDamage() //ÿ��Ӣ�ۿ�Ѫ
    {

    }

    public void OnDamage() //ÿ������˺�
    {

    }

    public void OnAfterAttack() //ÿ��ִ�й��������
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
    void OnDrawACard(); //ÿ�γ����
    void OnPlayACard(); //ÿ�γ��Ƶ���
    void OnBeforeAttack(); //ÿ��ִ�й���ǰ����
    void OnArmorDamage(); //ÿ�λ��׿�Ѫ
    void OnShieldDamage(); //ÿ�λ��ܿ�Ѫ
    void OnRetinueDamage(); //ÿ����ӿ�Ѫ
    void OnLifeDamage(); //ÿ��Ӣ�ۿ�Ѫ
    void OnDamage(); //ÿ������˺�
    void OnAfterAttack(); //ÿ��ִ�й��������
}