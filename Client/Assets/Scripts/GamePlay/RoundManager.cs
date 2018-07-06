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
    internal ClientPlayer CurrentClientPlayer;
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
        CurrentClientPlayer.MyHandManager.GetACardByID(99);
        CurrentClientPlayer.MyHandManager.DrawRetinueCard();
        CurrentClientPlayer.MyHandManager.DrawCards(GamePlaySettings.FirstDrawCard);
        EndRound();
        switchPlayer();
        CurrentClientPlayer.MyHandManager.GetACardByID(99);
        CurrentClientPlayer.MyHandManager.DrawRetinueCard();
        CurrentClientPlayer.MyHandManager.DrawCards(GamePlaySettings.SecondDrawCard);
        EndRound();
        switchPlayer();
        BeginRound();
        DrawCardPhase();
    }
  
    public void SetPlayerTurn(PlayerTurnResponse r)
    {
        CurrentClientPlayer = r.clientId == NetworkManager.NM.SelfClientId ? GameManager.GM.SelfClientPlayer : GameManager.GM.EnemyClientPlayer;
    }


    public void BeginRound()
    {
        CurrentClientPlayer.IncreaseCostMax(GamePlaySettings.CostIncrease);
        CurrentClientPlayer.AddAllCost();
        if (CurrentClientPlayer == GameManager.GM.SelfClientPlayer)
        {
            SelfTurnText.SetActive(true);
            EnemyTurnText.SetActive(false);
        }
        else
        {
            EnemyTurnText.SetActive(true);
            SelfTurnText.SetActive(false);
        }


        CurrentClientPlayer.MyHandManager.BeginRound();
        CurrentClientPlayer.MyBattleGroundManager.BeginRound();
    }

    public void DrawCardPhase() //���ƽ׶�
    {
        CurrentClientPlayer.MyHandManager.DrawCards(GamePlaySettings.DrawCardPerRound);
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
        CurrentClientPlayer.MyHandManager.EndRound();
        CurrentClientPlayer.MyBattleGroundManager.EndRound();
    }

    void switchPlayer()
    {
        CurrentClientPlayer = CurrentClientPlayer == GameManager.GM.SelfClientPlayer ? GameManager.GM.EnemyClientPlayer : GameManager.GM.SelfClientPlayer;
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