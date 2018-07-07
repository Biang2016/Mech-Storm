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
    internal ClientPlayer SelfClientPlayer;
    internal ClientPlayer EnemyClientPlayer;
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

    #region 响应

    public void InitializePlayers(PlayerResponse r)
    {
        if (r.ClinetId == NetworkManager.NM.SelfClientId)
        {
            SelfClientPlayer = new ClientPlayer(r.CostMax, r.CostLeft, Players.Self);
        }
        else
        {
            EnemyClientPlayer = new ClientPlayer(r.CostMax, r.CostLeft, Players.Enemy);
        }
    }

    public void SetPlayersCost(PlayerCostResponse r)
    {
        if (r.clinetId == NetworkManager.NM.SelfClientId)
        {
            SelfClientPlayer.DoChangeCost(r);
        }
        else
        {
            EnemyClientPlayer.DoChangeCost(r);
        }
    }

    public void InitializeGame()
    {
        RoundNumber = 0;
        CurrentClientPlayer = null;
        SelfCostText.text = "";
        EnemyCostText.text = "";
    }

    public void SetPlayerTurn(PlayerTurnResponse r) //服务器说某玩家回合开始
    {
        if (CurrentClientPlayer != null)
        {
            EndRound();
        }

        CurrentClientPlayer = r.clientId == NetworkManager.NM.SelfClientId ? SelfClientPlayer : EnemyClientPlayer;
        if (CurrentClientPlayer == SelfClientPlayer)
        {
            SelfTurnText.SetActive(true);
            EnemyTurnText.SetActive(false);
        }
        else
        {
            EnemyTurnText.SetActive(true);
            SelfTurnText.SetActive(false);
        }

        BeginRound();
    }

    public void BeginRound()
    {
        CurrentClientPlayer.MyHandManager.BeginRound();
        CurrentClientPlayer.MyBattleGroundManager.BeginRound();
    }

    public void OnPlayerSummonRetinue(SummonRetinueResponse resp)
    {
        if (resp.clientId == NetworkManager.NM.SelfClientId)
        {
            SelfClientPlayer.MyHandManager.SummonRetinue(resp.handCardIndex,resp.battleGroundIndex);
        }
        else
        {
            EnemyClientPlayer.MyHandManager.SummonRetinue(resp.handCardIndex, resp.battleGroundIndex);
        }
    }

    public void OnPlayerDrawCard(DrawCardResponse resp)
    {
        if (resp.isShow)
        {
            CurrentClientPlayer.MyHandManager.GetCard(resp.cardId);
        }
        else
        {
            CurrentClientPlayer.MyHandManager.GetCard(-1); //空白牌，隐藏防止对方知道
        }
    }

    public void EndRound()
    {
        CurrentClientPlayer.MyHandManager.EndRound();
        CurrentClientPlayer.MyBattleGroundManager.EndRound();
    }

    #endregion

    #region 交互

    public void OnEndRoundButtonClick()
    {
        ClientEndRoundRequest request = new ClientEndRoundRequest(NetworkManager.NM.SelfClientId);
        Client.CS.SendMessage(request);
    }

    #endregion
}