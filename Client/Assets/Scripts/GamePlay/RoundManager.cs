using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    internal RandomNumberGenerator RandomNumberGenerator;
    internal ClientPlayer SelfClientPlayer;
    internal ClientPlayer EnemyClientPlayer;
    internal ClientPlayer CurrentClientPlayer;
    internal ClientPlayer IdleClientPlayer;

    [SerializeField] private Canvas BattleCanvas;
    [SerializeField] private GameObject SelfTurnText;
    [SerializeField] private GameObject EnemyTurnText;
    [SerializeField] private GameObject EndRoundButton;
    [SerializeField] public Text SelfCostText;
    [SerializeField] public Text EnemyCostText;

    void Awake()
    {
        rm = FindObjectOfType(typeof(RoundManager)) as RoundManager;
        BattleCanvas.gameObject.SetActive(false);
    }

    void Start()
    {
    }

    private void Update()
    {
        if (isStop)
        {
            OnGameStop();
            isStop = false;
        }
    }

    private void BeginRound()
    {
        CurrentClientPlayer.MyHandManager.BeginRound();
        CurrentClientPlayer.MyBattleGroundManager.BeginRound();
    }

    private void EndRound()
    {
        CurrentClientPlayer.MyHandManager.EndRound();
        CurrentClientPlayer.MyBattleGroundManager.EndRound();
    }

    bool isStop = false;

    public void StopGame()
    {
        isStop = true; //标记为，待Update的时候正式处理OnGameStop
    }

    private void OnGameStop()
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
        SelfTurnText.SetActive(false);
        EnemyTurnText.SetActive(false);
        EndRoundButton.SetActive(false);
        SelfCostText.gameObject.SetActive(false);
        EnemyCostText.gameObject.SetActive(false);
        BattleCanvas.gameObject.SetActive(false);

        CardDeckManager.CDM.HideAll();
        RandomNumberGenerator = null;

        BattleEffectsManager.BEM.Effect_Main.AllEffectsEnd();
        BattleEffectsManager.BEM.Effect_RefreshBattleGroundOnAddRetinue.AllEffectsEnd();
    }

    #region PureRequests

    public void OnGameStopByLeave(GameStopByLeaveRequest r)
    {
        if (r.clientId == Client.CS.Proxy.ClientId)
        {
            ClientLog.CL.PrintClientStates("你 " + r.clientId + " 退出了比赛");
        }
        else
        {
            ClientLog.CL.PrintReceive("你的对手 " + r.clientId + " 退出了比赛");
        }

        OnGameStop();
        Client.CS.Proxy.ClientState = ProxyBase.ClientStates.SubmitCardDeck;
    }

    public void OnRandomNumberSeed(RandomNumberSeedRequest se)
    {
        RandomNumberGenerator = new RandomNumberGenerator(se.randomNumberSeed);
    }

    #endregion


    #region SideEffects

    public void ResponseToSideEffects_PrePass(ServerRequestBase se) //第一轮
    {
        switch (se.GetProtocol())
        {
            case NetProtocols.SE_SET_PLAYER:
            {
                Client.CS.Proxy.ClientState = ProxyBase.ClientStates.Playing;
                NetworkManager.NM.SuccessMatched();
                Initialize();
                InitializePlayers((SetPlayerRequest) se);
                break;
            }
            case NetProtocols.SE_BATTLEGROUND_ADD_RETINUE:
            {
                OnBattleGroundAddRetinue_PrePass((BattleGroundAddRetinueRequest) se);
                break;
            }
            case NetProtocols.SE_BATTLEGROUND_REMOVE_RETINUE:
            {
                OnBattleGroundRemoveRetinue_PrePass((BattleGroundRemoveRetinueRequest) se);
                break;
            }


            //case NetProtocols.SE_DRAW_CARD:
            //{
            //    OnPlayerDrawCard((DrawCardRequest) se);
            //    break;
            //}
            //case NetProtocols.SE_DROP_CARD:
            //{
            //    OnPlayerDropCard((DropCardRequest) se);
            //    break;
            //}

            //case NetProtocols.SE_EQUIP_WEAPON_SERVER_REQUEST:
            //{
            //    OnEquipWeapon((EquipWeaponServerRequest) se);
            //    break;
            //}
            //case NetProtocols.SE_EQUIP_SHIELD_SERVER_REQUEST:
            //{
            //    OnEquipShield((EquipShieldServerRequest) se);
            //    break;
            //}
            //case NetProtocols.SE_RETINUE_ATTACK_RETINUE_SERVER_REQUEST:
            //{
            //    OnRetinueAttackRetinue((RetinueAttackRetinueServerRequest) se);
            //    break;
            //}
        }
    }

    public void ResponseToSideEffects(ServerRequestBase se)
    {
        switch (se.GetProtocol())
        {
            case NetProtocols.SE_PLAYER_TURN:
            {
                SetPlayerTurn((PlayerTurnRequest) se);
                break;
            }
            case NetProtocols.SE_PLAYER_COST_CHANGE:
            {
                SetPlayersCost((PlayerCostChangeRequest) se);
                break;
            }

            case NetProtocols.SE_RETINUE_ATTRIBUTES_CHANGE:
            {
                OnRetinueAttributesChange((RetinueAttributesChangeRequest) se);
                break;
            }

            case NetProtocols.SE_RETINUE_DIE:
            {
                OnRetinueDie((RetinueDieRequest) se);
                break;
            }
            case NetProtocols.SE_BATTLEGROUND_ADD_RETINUE:
            {
                OnBattleGroundAddRetinue((BattleGroundAddRetinueRequest) se);
                break;
            }
            case NetProtocols.SE_BATTLEGROUND_REMOVE_RETINUE:
            {
                OnBattleGroundRemoveRetinue((BattleGroundRemoveRetinueRequest) se);
                break;
            }

            case NetProtocols.SE_CARDDECT_LEFT_CHANGE:
            {
                OnCardDeckLeftChange((CardDeckLeftChangeRequest) se);
                break;
            }
            case NetProtocols.SE_DRAW_CARD:
            {
                OnPlayerDrawCard((DrawCardRequest) se);
                break;
            }
            case NetProtocols.SE_DROP_CARD:
            {
                OnPlayerDropCard((DropCardRequest) se);
                break;
            }
            case NetProtocols.SE_USE_CARD:
            {
                OnPlayerUseCard((UseCardRequest) se);
                break;
            }

            case NetProtocols.SE_EQUIP_WEAPON_SERVER_REQUEST:
            {
                OnEquipWeapon((EquipWeaponServerRequest) se);
                break;
            }
            case NetProtocols.SE_EQUIP_SHIELD_SERVER_REQUEST:
            {
                OnEquipShield((EquipShieldServerRequest) se);
                break;
            }
            case NetProtocols.SE_USE_SPELLCARD_SERVER_REQUEST:
            {
                OnUseSpellCard((UseSpellCardServerRequset) se);
                break;
            }
            case NetProtocols.SE_RETINUE_ATTACK_RETINUE_SERVER_REQUEST:
            {
                OnRetinueAttackRetinue((RetinueAttackRetinueServerRequest) se);
                break;
            }
            case NetProtocols.SE_DAMAGE_SOME_RETINUE_REQUEST:
            {
                OnDamageSomeRetinue((DamageSomeRetinueRequest) se);
                break;
            }
            case NetProtocols.SE_RETINUE_EFFECT:
            {
                OnRetinueEffect((RetinueEffectRequest) se);
                break;
            }
        }
    }

    private void Initialize()
    {
        RoundNumber = 0;
        CurrentClientPlayer = null;
        IdleClientPlayer = null;

        BattleCanvas.gameObject.SetActive(true);
        SelfTurnText.SetActive(false);
        EnemyTurnText.SetActive(false);
        EndRoundButton.SetActive(false);
        SelfCostText.gameObject.SetActive(true);
        EnemyCostText.gameObject.SetActive(true);
        SelfCostText.text = "";
        EnemyCostText.text = "";

        CardDeckManager.CDM.ShowAll();
    }

    private void InitializePlayers(SetPlayerRequest r)
    {
        if (r.clientId == Client.CS.Proxy.ClientId)
        {
            SelfClientPlayer = new ClientPlayer(r.costLeft, r.costMax, Players.Self);
            SelfClientPlayer.ClientId = r.clientId;
        }
        else
        {
            EnemyClientPlayer = new ClientPlayer(r.costLeft, r.costMax, Players.Enemy);
            EnemyClientPlayer.ClientId = r.clientId;
        }
    }

    private void SetPlayersCost(PlayerCostChangeRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clinetId);
        cp.DoChangeCost(r);
        cp.MyHandManager.RefreshAllCardUsable();
    }

    private void SetPlayerTurn(PlayerTurnRequest r) //服务器说某玩家回合开始
    {
        BattleEffectsManager.BEM.Effect_Main.EffectsShow(Co_SetPlayerTurn(r), "Co_SetPlayerTurn");
    }

    IEnumerator Co_SetPlayerTurn(PlayerTurnRequest r)
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
        yield return null;
        BattleEffectsManager.BEM.Effect_Main.EffectEnd();
    }

    private void OnRetinueAttributesChange(RetinueAttributesChangeRequest r)
    {
        ModuleRetinue retinue = GetPlayerByClientId(r.clinetId).MyBattleGroundManager.GetRetinue(r.retinueId);
        retinue.M_RetinueAttack += r.addAttack;
        retinue.M_RetinueWeaponEnergy += r.addWeaponEnergy;
        retinue.M_RetinueWeaponEnergyMax += r.addWeaponEnergyMax;
        retinue.M_RetinueArmor += r.addArmor;
        retinue.M_RetinueShield += r.addShield;
        retinue.M_RetinueTotalLife += r.addMaxLife;
        retinue.M_RetinueLeftLife += r.addLeftLife;
    }

    private void OnRetinueDie(RetinueDieRequest r)
    {
        List<ModuleRetinue> dieRetinues = new List<ModuleRetinue>();
        foreach (int retinueId in r.retinueIds)
        {
            ModuleRetinue retinue = SelfClientPlayer.MyBattleGroundManager.GetRetinue(retinueId);
            if (retinue != null)
            {
                dieRetinues.Add(retinue);
            }
            else
            {
                retinue = EnemyClientPlayer.MyBattleGroundManager.GetRetinue(retinueId);
                if (retinue != null)
                {
                    dieRetinues.Add(retinue);
                }
            }
        }

        foreach (ModuleRetinue moduleRetinue in dieRetinues)
        {
            moduleRetinue.OnDie();
        }

        BattleEffectsManager.BEM.Effect_Main.EffectsShow(Co_RetinueDieShock(dieRetinues), "Co_RetinueDieShock");
    }

    IEnumerator Co_RetinueDieShock(List<ModuleRetinue> dieRetinues) //随从一起死亡效果
    {
        int shockTimes = 3;
        for (int i = 0; i < shockTimes; i++)
        {
            foreach (ModuleRetinue moduleRetinue in dieRetinues)
            {
                moduleRetinue.transform.Rotate(Vector3.up, 3, Space.Self);
            }

            yield return new WaitForSeconds(0.04f);
            foreach (ModuleRetinue moduleRetinue in dieRetinues)
            {
                moduleRetinue.transform.Rotate(Vector3.up, -6, Space.Self);
            }

            yield return new WaitForSeconds(0.04f);
            foreach (ModuleRetinue moduleRetinue in dieRetinues)
            {
                moduleRetinue.transform.Rotate(Vector3.up, 3, Space.Self);
            }
        }

        BattleEffectsManager.BEM.Effect_Main.EffectEnd();
    }

    private void OnBattleGroundAddRetinue_PrePass(BattleGroundAddRetinueRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.MyBattleGroundManager.AddRetinue_PrePass(r.cardInfo, r.retinueId, r.clientRetinueTempId);
    }

    private void OnBattleGroundAddRetinue(BattleGroundAddRetinueRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        if (cp == SelfClientPlayer && r.clientRetinueTempId >= 0) return;
        else
        {
            cp.MyBattleGroundManager.AddRetinue(r.battleGroundIndex);
        }
    }

    private void OnBattleGroundRemoveRetinue_PrePass(BattleGroundRemoveRetinueRequest r)
    {
        foreach (int retinueId in r.retinueIds)
        {
            if (SelfClientPlayer.MyBattleGroundManager.GetRetinue(retinueId) != null)
            {
                SelfClientPlayer.MyBattleGroundManager.RemoveRetinueTogatherAdd(retinueId);
            }
            else if (EnemyClientPlayer.MyBattleGroundManager.GetRetinue(retinueId) != null)
            {
                EnemyClientPlayer.MyBattleGroundManager.RemoveRetinueTogatherAdd(retinueId);
            }
        }
    }

    private void OnBattleGroundRemoveRetinue(BattleGroundRemoveRetinueRequest r)
    {
        BattleEffectsManager.BEM.Effect_Main.EffectsShow(Co_RetinueRemoveFromBattleGround(), "Co_RetinueRemoveFromBattleGround");
    }

    IEnumerator Co_RetinueRemoveFromBattleGround() //随从一起移除战场
    {
        SelfClientPlayer.MyBattleGroundManager.RemoveRetinueTogather();
        EnemyClientPlayer.MyBattleGroundManager.RemoveRetinueTogather();

        SelfClientPlayer.MyBattleGroundManager.RemoveRetinueTogatherEnd();
        EnemyClientPlayer.MyBattleGroundManager.RemoveRetinueTogatherEnd();

        yield return null;
        BattleEffectsManager.BEM.Effect_Main.EffectEnd();
    }

    private void OnCardDeckLeftChange(CardDeckLeftChangeRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        BattleEffectsManager.BEM.Effect_Main.EffectsShow(Co_OnCardDeckLeftChange(cp, r.left), "Co_OnCardDeckLeftChange");
    }

    IEnumerator Co_OnCardDeckLeftChange(ClientPlayer cp, int left)
    {
        if (cp == SelfClientPlayer) CardDeckManager.CDM.SetSelfCardDeckNumber(left);
        else CardDeckManager.CDM.SetEnemyCardDeckNumber(left);
        yield return null;
        BattleEffectsManager.BEM.Effect_Main.EffectEnd();
    }

    private void OnPlayerDrawCard(DrawCardRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        foreach (DrawCardRequest.CardIdAndInstanceId respCardId in r.cardInfos)
        {
            cp.MyHandManager.GetCard(respCardId.CardId, respCardId.CardInstanceId);
        }
    }

    private void OnPlayerDropCard(DropCardRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.MyHandManager.DropCard(r.handCardInstanceId);
    }

    private void OnPlayerUseCard(UseCardRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.MyHandManager.UseCard(r.handCardInstanceId, r.cardInfo, new Vector3(r.lastDragPosition.x, r.lastDragPosition.y, r.lastDragPosition.z));
    }

    private void OnEquipWeapon(EquipWeaponServerRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.MyBattleGroundManager.EquipWeapon(r.cardInfo, r.retinueId);
    }

    private void OnEquipShield(EquipShieldServerRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.MyBattleGroundManager.EquipShield(r.cardInfo, r.retinueId);
    }

    private void OnUseSpellCard(UseSpellCardServerRequset r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        //Todo
    }

    public void OnRetinueAttackRetinue(RetinueAttackRetinueServerRequest r)
    {
        ClientPlayer cp_attack = GetPlayerByClientId(r.AttackRetinueClientId);
        ClientPlayer cp_beAttack = GetPlayerByClientId(r.BeAttackedRetinueClientId);
        ModuleRetinue attackRetinue = cp_attack.MyBattleGroundManager.GetRetinue(r.AttackRetinueId);
        ModuleRetinue beAttackRetinue = cp_beAttack.MyBattleGroundManager.GetRetinue(r.BeAttackedRetinueId);
        attackRetinue.Attack(beAttackRetinue, true);
    }

    public void OnDamageSomeRetinue(DamageSomeRetinueRequest r)
    {
        ClientPlayer cp_beAttack = GetPlayerByClientId(r.beDamagedRetinueClientId);
        cp_beAttack.MyBattleGroundManager.DamageSomeRetinue(r.beDamagedRetinueId, r.value);
    }

    private void OnRetinueEffect(RetinueEffectRequest r)
    {
        ModuleRetinue retinue = GetPlayerByClientId(r.clientId).MyBattleGroundManager.GetRetinue(r.retinueId);
        switch (r.effectType)
        {
            case RetinueEffectRequest.EffectType.OnSummon:
                retinue.OnSummonShowEffects();
                break;
            case RetinueEffectRequest.EffectType.OnDie:
                retinue.OnDieShowEffects();
                break;
        }
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

    #endregion

    #region Utils

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