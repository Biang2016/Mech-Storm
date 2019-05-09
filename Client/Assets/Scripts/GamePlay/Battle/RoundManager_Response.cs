using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class RoundManager
{
    public void OnGameStopByLeave(GameStopByLeaveRequest r)
    {
        if (r.clientId == Client.Instance.Proxy.ClientId)
        {
            ClientLog.Instance.PrintClientStates("你 " + r.clientId + " 退出了比赛");
        }
        else
        {
            ClientLog.Instance.PrintReceive("你的对手 " + r.clientId + " 退出了比赛");
        }

        OnGameStop();
        Client.Instance.Proxy.ClientState = ProxyBase.ClientStates.Login;
    }

    public void OnGameStopByWin(GameStopByWinRequest r)
    {
        BattleResultPanel brp = UIManager.Instance.ShowUIForms<BattleResultPanel>();
        if (r.winnerClientId == Client.Instance.Proxy.ClientId)
        {
            ClientLog.Instance.PrintClientStates("你赢了");
            brp.WinGame();
        }
        else
        {
            ClientLog.Instance.PrintReceive("你输了");
            brp.LostGame();
        }
    }

    public void OnGameStopByServerError(GameStopByServerErrorRequest r)
    {
        OnGameStop();
        NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("RoundManager_EndedByServerError"), 0, 2f);
        Client.Instance.Proxy.ClientState = ProxyBase.ClientStates.Login;
    }

    public void OnRandomNumberSeed(RandomNumberSeedRequest r)
    {
        RandomNumberGenerator = new RandomNumberGenerator(r.randomNumberSeed);
    }

    public void ResponseToSideEffects_PrePass(ServerRequestBase r) //第一轮
    {
        switch (r.GetProtocol())
        {
            case NetProtocols.SE_SET_PLAYER:
            {
                Client.Instance.Proxy.ClientState = ProxyBase.ClientStates.Playing;
                OnSetPlayer_PrePass(r);
                break;
            }
            case NetProtocols.SE_BATTLEGROUND_ADD_RETINUE:
            {
                OnBattleGroundAddRetinue_PrePass((BattleGroundAddRetinueRequest) r);
                break;
            }
            case NetProtocols.SE_BATTLEGROUND_REMOVE_RETINUE:
            {
                OnBattleGroundRemoveRetinue_PrePass((BattleGroundRemoveRetinueRequest) r);
                break;
            }
        }
    }

    public void ResponseToSideEffects(ServerRequestBase r)
    {
        switch (r.GetProtocol())
        {
            case NetProtocols.SE_PLAYER_TURN:
            {
                OnSetPlayerTurn((PlayerTurnRequest) r);
                break;
            }
            case NetProtocols.SE_PLAYER_METAL_CHANGE:
            {
                OnSetPlayersMetal((PlayerMetalChangeRequest) r);
                break;
            }
            case NetProtocols.SE_PLAYER_LIFE_CHANGE:
            {
                OnSetPlayersLife((PlayerLifeChangeRequest) r);
                break;
            }
            case NetProtocols.SE_PLAYER_ENERGY_CHANGE:
            {
                OnSetPlayersEnergy((PlayerEnergyChangeRequest) r);
                break;
            }

            case NetProtocols.SE_RETINUE_ATTRIBUTES_CHANGE:
            {
                OnRetinueAttributesChange((RetinueAttributesChangeRequest) r);
                break;
            }

            case NetProtocols.SE_RETINUE_DIE:
            {
                OnRetinueDie((RetinueDieRequest) r);
                break;
            }
            case NetProtocols.SE_BATTLEGROUND_ADD_RETINUE:
            {
                OnBattleGroundAddRetinue((BattleGroundAddRetinueRequest) r);
                break;
            }
            case NetProtocols.SE_BATTLEGROUND_REMOVE_RETINUE:
            {
                OnBattleGroundRemoveRetinue((BattleGroundRemoveRetinueRequest) r);
                break;
            }
            case NetProtocols.SE_PLAYER_BUFF_UPDATE_REQUEST:
            {
                OnUpdatePlayerBuff((PlayerBuffUpdateRequest) r);
                break;
            }
            case NetProtocols.SE_PLAYER_BUFF_REMOVE_REQUEST:
            {
                OnRemovePlayerBuff((PlayerBuffRemoveRequest) r);
                break;
            }
            case NetProtocols.SE_PLAYER_COOLDOWNCARD_UPDATE_REQUEST:
            {
                OnUpdatePlayerCoolDownCard((PlayerCoolDownCardUpdateRequest) r);
                break;
            }
            case NetProtocols.SE_PLAYER_COOLDOWNCARD_REMOVE_REQUEST:
            {
                OnRemovePlayerCoolDownCard((PlayerCoolDownCardRemoveRequest) r);
                break;
            }

            case NetProtocols.SE_CARDDECT_LEFT_CHANGE:
            {
                OnCardDeckLeftChange((CardDeckLeftChangeRequest) r);
                break;
            }
            case NetProtocols.SE_DRAW_CARD:
            {
                OnPlayerDrawCard((DrawCardRequest) r);
                break;
            }
            case NetProtocols.SE_DROP_CARD:
            {
                OnPlayerDropCard((DropCardRequest) r);
                break;
            }
            case NetProtocols.SE_USE_CARD:
            {
                OnPlayerUseCard((UseCardRequest) r);
                break;
            }

            case NetProtocols.SE_RETINUE_CARDINFO_SYNC:
            {
                OnRetinueCardInfoSync((RetinueCardInfoSyncRequest) r);
                break;
            }

            case NetProtocols.SE_EQUIP_WEAPON_SERVER_REQUEST:
            {
                OnEquipWeapon((EquipWeaponServerRequest) r);
                break;
            }
            case NetProtocols.SE_EQUIP_SHIELD_SERVER_REQUEST:
            {
                OnEquipShield((EquipShieldServerRequest) r);
                break;
            }
            case NetProtocols.SE_EQUIP_PACK_SERVER_REQUEST:
            {
                OnEquipPack((EquipPackServerRequest) r);
                break;
            }
            case NetProtocols.SE_EQUIP_MA_SERVER_REQUEST:
            {
                OnEquipMA((EquipMAServerRequest) r);
                break;
            }
            case NetProtocols.SE_USE_SPELLCARD_SERVER_REQUEST:
            {
                OnUseSpellCard((UseSpellCardServerRequset) r);
                break;
            }
            case NetProtocols.SE_RETINUE_ATTACK_RETINUE_SERVER_REQUEST:
            {
                OnRetinueAttackRetinue((RetinueAttackRetinueServerRequest) r);
                break;
            }
            case NetProtocols.SE_RETINUE_ATTACK_SHIP_SERVER_REQUEST:
            {
                OnRetinueAttackShip((RetinueAttackShipServerRequest) r);
                break;
            }
            case NetProtocols.SE_RETINUE_DODGE:
            {
                OnRetinueDodge((RetinueDodgeRequest) r);
                break;
            }
            case NetProtocols.SE_RETINUE_CANATTACK:
            {
                OnRetinueCanAttackChange((RetinueCanAttackRequest) r);
                break;
            }
            case NetProtocols.SE_RETINUE_IMMUNE:
            {
                OnRetinueImmuneChange((RetinueImmuneStateRequest) r);
                break;
            }
            case NetProtocols.SE_RETINUE_INACTIVITY:
            {
                OnRetinueInactivityChange((RetinueInactivityStateRequest) r);
                break;
            }
            case NetProtocols.SE_RETINUE_ONATTACK:
            {
                OnRetinueOnAttack((RetinueOnAttackRequest) r);
                break;
            }
            case NetProtocols.SE_RETINUE_ONATTACKSHIP:
            {
                OnRetinueOnAttackShip((RetinueOnAttackShipRequest) r);
                break;
            }
            case NetProtocols.SE_RETINUE_SHIELD_DEFENSE:
            {
                OnRetinueShieldDefence((RetinueShieldDefenseRequest) r);
                break;
            }
            case NetProtocols.SE_SHOW_SIDEEFFECT_TRIGGERED_EFFECT:
            {
                OnShowSideEffect((ShowSideEffectTriggeredRequest) r);
                break;
            }
            case NetProtocols.SE_CARD_ATTR_CHANGE:
            {
                OnCardAttributeChange((CardAttributeChangeRequest) r);
                break;
            }
            case NetProtocols.GAME_STOP_BY_WIN_REQUEST:
            {
                OnGameStopByWin((GameStopByWinRequest) r);
                break;
            }
        }
    }

    private void OnSetPlayer_PrePass(ServerRequestBase r)
    {
        NetworkManager.Instance.SuccessMatched();
        InitializePlayers((SetPlayerRequest) r);
    }

    private void OnSetPlayersMetal(PlayerMetalChangeRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clinetId);
        cp.DoChangeMetal(r);
    }

    private void OnSetPlayersLife(PlayerLifeChangeRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clinetId);
        cp.DoChangeLife(r);
    }

    private void OnSetPlayersEnergy(PlayerEnergyChangeRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clinetId);
        cp.DoChangeEnergy(r);
    }

    private void OnSetPlayerTurn(PlayerTurnRequest r) //服务器说某玩家回合开始
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_SetPlayerTurn(r), "Co_SetPlayerTurn");
    }

    IEnumerator Co_SetPlayerTurn(PlayerTurnRequest r)
    {
        CurrentClientPlayer = r.clientId == Client.Instance.Proxy.ClientId ? SelfClientPlayer : EnemyClientPlayer;
        IdleClientPlayer = r.clientId == Client.Instance.Proxy.ClientId ? EnemyClientPlayer : SelfClientPlayer;
        if (CurrentClientPlayer == SelfClientPlayer)
        {
            UIManager.Instance.GetBaseUIForm<BattleUIPanel>().SetEndRoundButtonState(true);
            ClientLog.Instance.PrintClientStates("MyRound");
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("RoundManager_YourTurn"), 0, 0.8f);
            AudioManager.Instance.SoundPlay("sfx/StoryOpen", 0.5f);
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            UIManager.Instance.GetBaseUIForm<BattleUIPanel>().SetEndRoundButtonState(true);
            ClientLog.Instance.PrintClientStates("EnemyRound");
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("RoundManager_EnemyTurn"), 0, 0.8f);
            yield return new WaitForSeconds(0.5f);
        }

        BeginRound();
        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private void OnRetinueAttributesChange(RetinueAttributesChangeRequest r)
    {
        ModuleRetinue retinue = GetPlayerByClientId(r.clinetId).BattlePlayer.BattleGroundManager.GetRetinue(r.retinueId);
        retinue.isAttackChanging = r.addAttack != 0;
        retinue.M_RetinueWeaponEnergyMax += r.addWeaponEnergyMax;
        retinue.M_RetinueWeaponEnergy += r.addWeaponEnergy;
        retinue.M_RetinueAttack += r.addAttack;
        retinue.isAttackChanging = false;
        retinue.M_RetinueArmor += r.addArmor;
        retinue.M_RetinueShield += r.addShield;
        retinue.isTotalLifeChanging = r.addMaxLife != 0;
        retinue.M_RetinueTotalLife += r.addMaxLife;
        retinue.M_RetinueLeftLife += r.addLeftLife;
        retinue.isTotalLifeChanging = false;
    }

    private void OnRetinueDie(RetinueDieRequest r)
    {
        List<ModuleRetinue> dieRetinues = new List<ModuleRetinue>();
        foreach (int retinueId in r.retinueIds)
        {
            ModuleRetinue retinue = SelfClientPlayer.BattlePlayer.BattleGroundManager.GetRetinue(retinueId);
            if (retinue != null)
            {
                dieRetinues.Add(retinue);
            }
            else
            {
                retinue = EnemyClientPlayer.BattlePlayer.BattleGroundManager.GetRetinue(retinueId);
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

        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RetinueDieShock(dieRetinues), "Co_RetinueDieShock");
    }

    IEnumerator Co_RetinueDieShock(List<ModuleRetinue> dieRetinues) //机甲一起死亡效果
    {
        int shockTimes = 3;
        AudioManager.Instance.SoundPlay("sfx/OnDie");
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

        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private void OnBattleGroundAddRetinue_PrePass(BattleGroundAddRetinueRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.BattleGroundManager.AddRetinue_PrePass(r.cardInfo, r.retinueId, r.clientRetinueTempId);
    }

    private void OnBattleGroundAddRetinue(BattleGroundAddRetinueRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        if (cp == SelfClientPlayer && r.clientRetinueTempId >= 0) return;
        cp.BattlePlayer.BattleGroundManager.AddRetinue(r.battleGroundIndex);
    }

    private void OnBattleGroundRemoveRetinue_PrePass(BattleGroundRemoveRetinueRequest r)
    {
        foreach (int retinueId in r.retinueIds)
        {
            if (SelfClientPlayer.BattlePlayer.BattleGroundManager.GetRetinue(retinueId) != null)
            {
                SelfClientPlayer.BattlePlayer.BattleGroundManager.RemoveRetinueTogetherAdd(retinueId);
            }
            else if (EnemyClientPlayer.BattlePlayer.BattleGroundManager.GetRetinue(retinueId) != null)
            {
                EnemyClientPlayer.BattlePlayer.BattleGroundManager.RemoveRetinueTogetherAdd(retinueId);
            }
        }
    }

    private void OnBattleGroundRemoveRetinue(BattleGroundRemoveRetinueRequest r)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RetinueRemoveFromBattleGround_Logic(r.retinueIds), "Co_RetinueRemoveFromBattleGround_Logic");
        SelfClientPlayer.BattlePlayer.BattleGroundManager.RemoveRetinueTogatherEnd();
        EnemyClientPlayer.BattlePlayer.BattleGroundManager.RemoveRetinueTogatherEnd();
    }

    private void OnUpdatePlayerBuff(PlayerBuffUpdateRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.PlayerBuffManager.UpdatePlayerBuff(r.buffSEE, r.buffId);
    }

    private void OnRemovePlayerBuff(PlayerBuffRemoveRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.PlayerBuffManager.RemovePlayerBuff(r.buffId);
    }

    private void OnUpdatePlayerCoolDownCard(PlayerCoolDownCardUpdateRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.PlayerCoolDownCardManager.UpdateCoolDownCard(r.coolingDownCard);
    }

    private void OnRemovePlayerCoolDownCard(PlayerCoolDownCardRemoveRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.PlayerCoolDownCardManager.RemoveCoolDownCard(r.coolingDownCard.CardInstanceID);
    }

    IEnumerator Co_RetinueRemoveFromBattleGround_Logic(List<int> retinueIds) //机甲一起移除战场(逻辑层)
    {
        SelfClientPlayer.BattlePlayer.BattleGroundManager.RemoveRetinueTogether(retinueIds);
        EnemyClientPlayer.BattlePlayer.BattleGroundManager.RemoveRetinueTogether(retinueIds);

        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private void OnCardDeckLeftChange(CardDeckLeftChangeRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_OnCardDeckLeftChange(cp, r.left), "Co_OnCardDeckLeftChange");
    }

    IEnumerator Co_OnCardDeckLeftChange(ClientPlayer cp, int left)
    {
        cp.BattlePlayer.CardDeckManager.SetCardDeckNumber(left);
        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private void OnPlayerDrawCard(DrawCardRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.HandManager.GetCards(r.cardInfos);
    }

    private void OnPlayerDropCard(DropCardRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.HandManager.DropCard(r.handCardInstanceId);
    }

    private void OnPlayerUseCard(UseCardRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.HandManager.UseCard(r.handCardInstanceId, r.cardInfo);
    }

    private void OnRetinueCardInfoSync(RetinueCardInfoSyncRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ModuleRetinue retinue = cp.BattlePlayer.BattleGroundManager.GetRetinue(r.instanceId);
        retinue.CardInfo = r.cardInfo.Clone();
    }

    private void OnEquipWeapon(EquipWeaponServerRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.BattleGroundManager.EquipWeapon(r.cardInfo, r.retinueId, r.equipID);
    }

    private void OnEquipShield(EquipShieldServerRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.BattleGroundManager.EquipShield(r.cardInfo, r.retinueId, r.equipID);
    }

    private void OnEquipPack(EquipPackServerRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.BattleGroundManager.EquipPack(r.cardInfo, r.retinueId, r.equipID);
    }

    private void OnEquipMA(EquipMAServerRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.BattleGroundManager.EquipMA(r.cardInfo, r.retinueId, r.equipID);
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
        ModuleRetinue attackRetinue = cp_attack.BattlePlayer.BattleGroundManager.GetRetinue(r.AttackRetinueId);
        ModuleRetinue beAttackRetinue = cp_beAttack.BattlePlayer.BattleGroundManager.GetRetinue(r.BeAttackedRetinueId);
        attackRetinue.Attack(beAttackRetinue, false);
    }

    private void OnRetinueAttackShip(RetinueAttackShipServerRequest r)
    {
        ClientPlayer cp_attack = GetPlayerByClientId(r.AttackRetinueClientId);
        ClientPlayer cp_beAttack = cp_attack.WhichPlayer == Players.Self ? EnemyClientPlayer : SelfClientPlayer;
        ModuleRetinue attackRetinue = cp_attack.BattlePlayer.BattleGroundManager.GetRetinue(r.AttackRetinueId);
        attackRetinue.AttackShip(cp_beAttack);
    }

    private void OnRetinueDodge(RetinueDodgeRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ModuleRetinue retinue = cp.BattlePlayer.BattleGroundManager.GetRetinue(r.retinueId);
        retinue.OnDodge();
    }

    private void OnRetinueCanAttackChange(RetinueCanAttackRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ModuleRetinue retinue = cp.BattlePlayer.BattleGroundManager.GetRetinue(r.retinueId);
        retinue.SetCanAttack(r.canAttack);
    }

    private void OnRetinueImmuneChange(RetinueImmuneStateRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ModuleRetinue retinue = cp.BattlePlayer.BattleGroundManager.GetRetinue(r.retinueId);
        retinue.M_ImmuneLeftRounds = r.immuneRounds;
    }

    private void OnRetinueInactivityChange(RetinueInactivityStateRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ModuleRetinue retinue = cp.BattlePlayer.BattleGroundManager.GetRetinue(r.retinueId);
        retinue.M_InactivityRounds = r.inactivityRounds;
    }

    private void OnRetinueOnAttack(RetinueOnAttackRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ModuleRetinue retinue = FindRetinue(r.retinueId);
        ModuleRetinue targetRetinue = FindRetinue(r.targetRetinueId);
        retinue.OnAttack(r.weaponType, targetRetinue);
    }

    private void OnRetinueOnAttackShip(RetinueOnAttackShipRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ClientPlayer cp_target = GetPlayerByClientId(r.targetClientId);
        ModuleRetinue retinue = FindRetinue(r.retinueId);
        retinue.OnAttackShip(r.weaponType, cp_target.BattlePlayer.Ship);
    }

    private void OnRetinueShieldDefence(RetinueShieldDefenseRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ModuleRetinue retinue = cp.BattlePlayer.BattleGroundManager.GetRetinue(r.retinueId);
        retinue.ShieldDefenceDamage(r.decreaseValue, r.shieldValue);
    }

    private void OnShowSideEffect(ShowSideEffectTriggeredRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.ExecutorInfo.ClientId);
        if (r.ExecutorInfo.IsPlayerBuff) //PlayerBuff
        {
            ClientLog.Instance.Print("Playerbuff ");
            return;
        }

        if (r.ExecutorInfo.RetinueId != -99999) //随从触发
        {
            if (r.ExecutorInfo.EquipId == -99999)
            {
                cp.BattlePlayer.BattleGroundManager.GetRetinue(r.ExecutorInfo.RetinueId).OnShowEffects(r.TriggerTime, r.TriggerRange);
            }
            else
            {
                cp.BattlePlayer.BattleGroundManager.GetEquip(r.ExecutorInfo.RetinueId, r.ExecutorInfo.EquipId).OnShowEffects(r.TriggerTime, r.TriggerRange);
            }
        }
        else if (r.ExecutorInfo.CardInstanceId != -99999) //手牌触发
        {
            //Todo 手牌SE效果
        }
    }

    private void OnCardAttributeChange(CardAttributeChangeRequest r)
    {
        CardBase cb = GetPlayerByClientId(r.clientId).BattlePlayer.HandManager.GetCardByCardInstanceId(r.cardInstanceId);
        cb.M_Energy += r.energyChange;
        cb.M_Metal += r.metalChange;
        cb.M_EffectFactor = r.effectFactor;
        cb.M_Desc = cb.CardInfo.GetCardDescShow();
    }
}