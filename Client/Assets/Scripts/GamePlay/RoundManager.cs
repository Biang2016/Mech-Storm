using UnityEngine;
using System.Collections;
using UnityEngine.UI;

internal class RoundManager : MonoBehaviour
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
    internal ClientPlayer IdleClientPlayer;

    public GameObject SelfTurnText;
    public GameObject EnemyTurnText;
    public GameObject EndRoundButton;
    public Text SelfCostText;
    public Text EnemyCostText;

    #region 响应

    public void Initialize()
    {
        RoundNumber = 0;
        CurrentClientPlayer = null;
        IdleClientPlayer = null;

        SelfTurnText.SetActive(false);
        EnemyTurnText.SetActive(false);
        EndRoundButton.SetActive(false);
        SelfCostText.text = "";
        EnemyCostText.text = "";
    }

    public void OnGameStop()
    {
        CardBase[] cardPreviews = GameBoardManager.GBM.CardDetailPreview.transform.GetComponentsInChildren<CardBase>();
        foreach (CardBase cardPreview in cardPreviews)
        {
            cardPreview.PoolRecycle();
        }

        ModuleBase[] modulePreviews = GameBoardManager.GBM.CardDetailPreview.transform.GetComponentsInChildren<ModuleBase>();
        foreach (ModuleBase modulePreview in modulePreviews)
        {
            modulePreview.PoolRecycle();
        }

        GameBoardManager.GBM.CardDetailPreview.transform.DetachChildren();

        GameBoardManager.GBM.SelfBattleGroundManager.Reset();
        GameBoardManager.GBM.EnemyBattleGroundManager.Reset();
        GameBoardManager.GBM.SelfHandManager.Reset();
        GameBoardManager.GBM.EnemyHandManager.Reset();
        SelfClientPlayer = null;
        EnemyClientPlayer = null;
        CurrentClientPlayer = null;
        IdleClientPlayer = null;
        SelfCostText.text = "";
        EnemyCostText.text = "";
        RoundNumber = 0;
    }

    public void InitializePlayers(PlayerRequest r)
    {
        if (r.clientId == Client.CS.Proxy.ClientId)
        {
            SelfClientPlayer = new ClientPlayer(r.costMax, r.costLeft, Players.Self);
            SelfClientPlayer.ClientId = r.clientId;
        }
        else
        {
            EnemyClientPlayer = new ClientPlayer(r.costMax, r.costLeft, Players.Enemy);
            EnemyClientPlayer.ClientId = r.clientId;
        }
    }

    public void SetPlayersCost(PlayerCostRequest r)
    {
        if (r.clinetId == Client.CS.Proxy.ClientId)
        {
            SelfClientPlayer.DoChangeCost(r);
            SelfClientPlayer.MyHandManager.RefreshAllCardUsable();
        }
        else
        {
            EnemyClientPlayer.DoChangeCost(r);
            EnemyClientPlayer.MyHandManager.RefreshAllCardUsable();
        }
    }

    public void SetPlayerTurn(PlayerTurnRequest r) //服务器说某玩家回合开始
    {
        if (CurrentClientPlayer != null)
        {
            EndRound();
        }

        CurrentClientPlayer = r.clientId == Client.CS.Proxy.ClientId ? SelfClientPlayer : EnemyClientPlayer;
        IdleClientPlayer = r.clientId == Client.CS.Proxy.ClientId ? EnemyClientPlayer : SelfClientPlayer;
        if (CurrentClientPlayer == SelfClientPlayer)
        {
            ClientLog.CL.PrintClientStates("MyRound");
            SelfTurnText.SetActive(true);
            EndRoundButton.SetActive(true);
            EnemyTurnText.SetActive(false);
        }
        else
        {
            ClientLog.CL.PrintClientStates("EnemyRound");
            SelfTurnText.SetActive(false);
            EndRoundButton.SetActive(false);
            EnemyTurnText.SetActive(true);
        }

        BeginRound();
    }

    public void BeginRound()
    {
        CurrentClientPlayer.MyHandManager.BeginRound();
        CurrentClientPlayer.MyBattleGroundManager.BeginRound();
    }

    public void OnPlayerSummonRetinue(SummonRetinueRequest_Response r)
    {
        GetPlayerByClientId(r.clientId).MyHandManager.UseCard(r.handCardIndex);
        GetPlayerByClientId(r.clientId).MyBattleGroundManager.AddRetinue(r);
    }

    public void OnPlayerEquipWeapon(EquipWeaponRequest_Response r)
    {
        GetPlayerByClientId(r.clientId).MyHandManager.UseCard(r.handCardIndex);
        GetPlayerByClientId(r.clientId).MyBattleGroundManager.EquipWeapon(r);
    }

    public void OnPlayerEquipShield(EquipShieldRequest_Response r)
    {
        GetPlayerByClientId(r.clientId).MyHandManager.UseCard(r.handCardIndex);
        GetPlayerByClientId(r.clientId).MyBattleGroundManager.EquipShield(r);
    }

    public void OnPlayerDrawCard(DrawCardRequest resp)
    {
        if (resp.clientId == Client.CS.Proxy.ClientId)
        {
            foreach (int respCardId in resp.cardIds)
            {
                SelfClientPlayer.MyHandManager.GetCard(respCardId);
            }
        }
        else
        {
            for (int i = 0; i < resp.cardCount; i++)
            {
                EnemyClientPlayer.MyHandManager.GetCard(999); //空白牌，隐藏防止对方知道
            }
        }
    }

    #region 战斗核心

    public void OnRetinueAttackRetinue(RetinueAttackRetinueRequest_Response r)
    {
        GetPlayerByClientId(r.AttackRetinueClientId).MyBattleGroundManager.GetRetinue(r.AttackRetinuePlaceIndex).AllModulesAttack();
    }

    public void OnWeaponAttributesChange(WeaponAttributesRequest r)
    {
        ModuleWeapon weapon = GetPlayerByClientId(r.clinetId).MyBattleGroundManager.GetRetinue(r.retinuePlaceIndex).M_Weapon;
        weapon.M_WeaponAttack += r.addAttack;
        weapon.M_WeaponEnergy += r.addEnergy;
    }

    public void OnRetinueAttributesChange(RetinueAttributesRequest r)
    {
        ModuleRetinue retinue = GetPlayerByClientId(r.clinetId).MyBattleGroundManager.GetRetinue(r.retinuePlaceIndex);
        retinue.M_RetinueAttack += r.addAttack;
        retinue.M_RetinueArmor += r.addArmor;
        retinue.M_RetinueShield += r.addShield;
        retinue.M_RetinueTotalLife += r.addMaxLife;
        retinue.M_RetinueLeftLife += r.addLeftLife;
    }

    public void OnShieldAttributesChange(ShieldAttributesRequest r)
    {
        ModuleShield shield = GetPlayerByClientId(r.clinetId).MyBattleGroundManager.GetRetinue(r.retinuePlaceIndex).M_Shield;
        shield.M_ShieldArmor += r.addArmor;
        shield.M_ShieldArmorMax += r.addArmorMax;
        shield.M_ShieldShield += r.addShield;
        shield.M_ShieldShieldMax += r.addShieldMax;
    }

    #endregion


    public void EndRound()
    {
        CurrentClientPlayer.MyHandManager.EndRound();
        CurrentClientPlayer.MyBattleGroundManager.EndRound();
    }

    #endregion

    #region 交互

    public void OnEndRoundButtonClick()
    {
        if (CurrentClientPlayer == SelfClientPlayer)
        {
            EndRoundRequest request = new EndRoundRequest(Client.CS.Proxy.ClientId);
            Client.CS.Proxy.SendMessage(request);
        }
        else
        {
            ClientLog.CL.PrintWarning("不是你的回合");
        }
    }

    public ClientPlayer GetPlayerByClientId(int clientId)
    {
        if (Client.CS.Proxy.ClientId == clientId)
        {
            return SelfClientPlayer;
        }
        else
        {
            return EnemyClientPlayer;
        }
    }

    #endregion
}