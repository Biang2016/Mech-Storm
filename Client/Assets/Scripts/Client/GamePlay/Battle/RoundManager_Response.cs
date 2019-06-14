using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class RoundManager
{
    public void OnGameStopByLeave(GameStopByLeaveRequest r)
    {
        if (r.clientId == Client.Instance.Proxy.ClientID)
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
        if (r.winnerClientId == Client.Instance.Proxy.ClientID)
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
            case NetProtocols.SE_BATTLEGROUND_ADD_MECH:
            {
                OnBattleGroundAddMech_PrePass((BattleGroundAddMechRequest) r);
                break;
            }
            case NetProtocols.SE_BATTLEGROUND_REMOVE_MECH:
            {
                OnBattleGroundRemoveMech_PrePass((BattleGroundRemoveMechRequest) r);
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

            case NetProtocols.SE_MECH_ATTRIBUTES_CHANGE:
            {
                OnMechAttributesChange((MechAttributesChangeRequest) r);
                break;
            }

            case NetProtocols.SE_MECH_DIE:
            {
                OnMechDie((MechDieRequest) r);
                break;
            }
            case NetProtocols.SE_BATTLEGROUND_ADD_MECH:
            {
                OnBattleGroundAddMech((BattleGroundAddMechRequest) r);
                break;
            }
            case NetProtocols.SE_BATTLEGROUND_REMOVE_MECH:
            {
                OnBattleGroundRemoveMech((BattleGroundRemoveMechRequest) r);
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

            case NetProtocols.SE_MECH_CARDINFO_SYNC:
            {
                OnMechCardInfoSync((MechCardInfoSyncRequest) r);
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
            case NetProtocols.SE_MECH_ATTACK_MECH_SERVER_REQUEST:
            {
                OnMechAttackMech((MechAttackMechServerRequest) r);
                break;
            }
            case NetProtocols.SE_MECH_ATTACK_SHIP_SERVER_REQUEST:
            {
                OnMechAttackShip((MechAttackShipServerRequest) r);
                break;
            }
            case NetProtocols.SE_MECH_DODGE:
            {
                OnMechDodge((MechDodgeRequest) r);
                break;
            }
            case NetProtocols.SE_MECH_CANATTACK:
            {
                OnMechCanAttackChange((MechCanAttackRequest) r);
                break;
            }
            case NetProtocols.SE_MECH_IMMUNE:
            {
                OnMechImmuneChange((MechImmuneStateRequest) r);
                break;
            }
            case NetProtocols.SE_MECH_INACTIVITY:
            {
                OnMechInactivityChange((MechInactivityStateRequest) r);
                break;
            }
            case NetProtocols.SE_MECH_ONATTACK:
            {
                OnMechOnAttack((MechOnAttackRequest) r);
                break;
            }
            case NetProtocols.SE_MECH_ONATTACKSHIP:
            {
                OnMechOnAttackShip((MechOnAttackShipRequest) r);
                break;
            }
            case NetProtocols.SE_MECH_SHIELD_DEFENSE:
            {
                OnMechShieldDefence((MechShieldDefenseRequest) r);
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
        CurrentClientPlayer = r.clientId == Client.Instance.Proxy.ClientID ? SelfClientPlayer : EnemyClientPlayer;
        IdleClientPlayer = r.clientId == Client.Instance.Proxy.ClientID ? EnemyClientPlayer : SelfClientPlayer;
        if (CurrentClientPlayer == SelfClientPlayer)
        {
            BattleManager.Instance.BattleUIPanel.SetEndRoundButtonState(true);
            ClientLog.Instance.PrintClientStates("MyRound");
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("RoundManager_YourTurn"), 0, 0.8f);
            AudioManager.Instance.SoundPlay("sfx/StoryOpen", 0.5f);
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            BattleManager.Instance.BattleUIPanel.SetEndRoundButtonState(true);
            ClientLog.Instance.PrintClientStates("EnemyRound");
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("RoundManager_EnemyTurn"), 0, 0.8f);
            yield return new WaitForSeconds(0.5f);
        }

        BeginRound();
        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private void OnMechAttributesChange(MechAttributesChangeRequest r)
    {
        ModuleMech mech = GetPlayerByClientId(r.clinetId).BattlePlayer.BattleGroundManager.GetMech(r.mechId);
        mech.MechSwordShieldArmorComponent.IsAttackChanging = r.addAttack != 0;
        mech.M_MechWeaponEnergyMax += r.addWeaponEnergyMax;
        mech.M_MechWeaponEnergy += r.addWeaponEnergy;
        mech.M_MechAttack += r.addAttack;
        mech.MechSwordShieldArmorComponent.IsAttackChanging = false;
        mech.M_MechArmor += r.addArmor;
        mech.M_MechShield += r.addShield;
        mech.MechLifeComponent.IsTotalLifeChanging = r.addMaxLife != 0;
        mech.M_MechTotalLife += r.addMaxLife;
        mech.M_MechLeftLife += r.addLeftLife;
        mech.MechLifeComponent.IsTotalLifeChanging = false;
    }

    private void OnMechDie(MechDieRequest r)
    {
        List<ModuleMech> dieMechs = new List<ModuleMech>();
        foreach (int mechId in r.mechIds)
        {
            ModuleMech mech = SelfClientPlayer.BattlePlayer.BattleGroundManager.GetMech(mechId);
            if (mech != null)
            {
                dieMechs.Add(mech);
            }
            else
            {
                mech = EnemyClientPlayer.BattlePlayer.BattleGroundManager.GetMech(mechId);
                if (mech != null)
                {
                    dieMechs.Add(mech);
                }
            }
        }

        foreach (ModuleMech moduleMech in dieMechs)
        {
            moduleMech.OnDie();
        }

        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_MechDieShock(dieMechs), "Co_MechDieShock");
    }

    IEnumerator Co_MechDieShock(List<ModuleMech> dieMechs) //机甲一起死亡效果
    {
        int shockTimes = 3;
        AudioManager.Instance.SoundPlay("sfx/OnDie");
        for (int i = 0; i < shockTimes; i++)
        {
            foreach (ModuleMech moduleMech in dieMechs)
            {
                moduleMech.transform.Rotate(Vector3.up, 3, Space.Self);
            }

            yield return new WaitForSeconds(0.04f);
            foreach (ModuleMech moduleMech in dieMechs)
            {
                moduleMech.transform.Rotate(Vector3.up, -6, Space.Self);
            }

            yield return new WaitForSeconds(0.04f);
            foreach (ModuleMech moduleMech in dieMechs)
            {
                moduleMech.transform.Rotate(Vector3.up, 3, Space.Self);
            }
        }

        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    private void OnBattleGroundAddMech_PrePass(BattleGroundAddMechRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.BattleGroundManager.AddMech_PrePass(r.cardInfo, r.mechId, r.clientMechTempId);
    }

    private void OnBattleGroundAddMech(BattleGroundAddMechRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        if (cp == SelfClientPlayer && r.clientMechTempId >= 0) return;
        cp.BattlePlayer.BattleGroundManager.AddMech(r.battleGroundIndex);
    }

    private void OnBattleGroundRemoveMech_PrePass(BattleGroundRemoveMechRequest r)
    {
        foreach (int mechId in r.mechIds)
        {
            if (SelfClientPlayer.BattlePlayer.BattleGroundManager.GetMech(mechId) != null)
            {
                SelfClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechTogetherAdd(mechId);
            }
            else if (EnemyClientPlayer.BattlePlayer.BattleGroundManager.GetMech(mechId) != null)
            {
                EnemyClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechTogetherAdd(mechId);
            }
        }
    }

    private void OnBattleGroundRemoveMech(BattleGroundRemoveMechRequest r)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_MechRemoveFromBattleGround_Logic(r.mechIds), "Co_MechRemoveFromBattleGround_Logic");
        SelfClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechTogatherEnd();
        EnemyClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechTogatherEnd();
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

    IEnumerator Co_MechRemoveFromBattleGround_Logic(List<int> mechIds) //机甲一起移除战场(逻辑层)
    {
        SelfClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechTogether(mechIds);
        EnemyClientPlayer.BattlePlayer.BattleGroundManager.RemoveMechTogether(mechIds);

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

    private void OnMechCardInfoSync(MechCardInfoSyncRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ModuleMech mech = cp.BattlePlayer.BattleGroundManager.GetMech(r.instanceId);
        mech.CardInfo = r.cardInfo.Clone();
    }

    private void OnEquipWeapon(EquipWeaponServerRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.BattleGroundManager.EquipWeapon(r.cardInfo, r.mechId, r.equipID);
    }

    private void OnEquipShield(EquipShieldServerRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.BattleGroundManager.EquipShield(r.cardInfo, r.mechId, r.equipID);
    }

    private void OnEquipPack(EquipPackServerRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.BattleGroundManager.EquipPack(r.cardInfo, r.mechId, r.equipID);
    }

    private void OnEquipMA(EquipMAServerRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        cp.BattlePlayer.BattleGroundManager.EquipMA(r.cardInfo, r.mechId, r.equipID);
    }

    private void OnUseSpellCard(UseSpellCardServerRequset r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        CardInfo_Base cb = cp.BattlePlayer.HandManager.GetCardByCardInstanceId(r.handCardInstanceId).CardInfo;
        cp.BattlePlayer.HandManager.UseCard(r.handCardInstanceId, cb);
    }

    public void OnMechAttackMech(MechAttackMechServerRequest r)
    {
        ClientPlayer cp_attack = GetPlayerByClientId(r.AttackMechClientId);
        ClientPlayer cp_beAttack = GetPlayerByClientId(r.BeAttackedMechClientId);
        ModuleMech attackMech = cp_attack.BattlePlayer.BattleGroundManager.GetMech(r.AttackMechId);
        ModuleMech beAttackMech = cp_beAttack.BattlePlayer.BattleGroundManager.GetMech(r.BeAttackedMechId);
        attackMech.Attack(beAttackMech, false);
    }

    private void OnMechAttackShip(MechAttackShipServerRequest r)
    {
        ClientPlayer cp_attack = GetPlayerByClientId(r.AttackMechClientId);
        ClientPlayer cp_beAttack = cp_attack.WhichPlayer == Players.Self ? EnemyClientPlayer : SelfClientPlayer;
        ModuleMech attackMech = cp_attack.BattlePlayer.BattleGroundManager.GetMech(r.AttackMechId);
        attackMech.AttackShip(cp_beAttack);
    }

    private void OnMechDodge(MechDodgeRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ModuleMech mech = cp.BattlePlayer.BattleGroundManager.GetMech(r.mechId);
        mech.MechSwordShieldArmorComponent.OnDodge();
    }

    private void OnMechCanAttackChange(MechCanAttackRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ModuleMech mech = cp.BattlePlayer.BattleGroundManager.GetMech(r.mechId);
        mech.SetCanAttack(r.canAttack);
    }

    private void OnMechImmuneChange(MechImmuneStateRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ModuleMech mech = cp.BattlePlayer.BattleGroundManager.GetMech(r.mechId);
        mech.M_ImmuneLeftRounds = r.immuneRounds;
    }

    private void OnMechInactivityChange(MechInactivityStateRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ModuleMech mech = cp.BattlePlayer.BattleGroundManager.GetMech(r.mechId);
        mech.M_InactivityRounds = r.inactivityRounds;
    }

    private void OnMechOnAttack(MechOnAttackRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ModuleMech mech = FindMech(r.mechId);
        ModuleMech targetMech = FindMech(r.targetMechId);
        mech.OnAttack(r.weaponType, targetMech);
    }

    private void OnMechOnAttackShip(MechOnAttackShipRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ClientPlayer cp_target = GetPlayerByClientId(r.targetClientId);
        ModuleMech mech = FindMech(r.mechId);
        mech.OnAttackShip(r.weaponType, cp_target.BattlePlayer.Ship);
    }

    private void OnMechShieldDefence(MechShieldDefenseRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.clientId);
        ModuleMech mech = cp.BattlePlayer.BattleGroundManager.GetMech(r.mechId);
        mech.MechSwordShieldArmorComponent.ShieldDefenceDamage(r.decreaseValue, r.shieldValue);
    }

    private void OnShowSideEffect(ShowSideEffectTriggeredRequest r)
    {
        ClientPlayer cp = GetPlayerByClientId(r.ExecutorInfo.ClientId);
        if (r.ExecutorInfo.IsPlayerBuff) //PlayerBuff
        {
            ClientLog.Instance.Print("Playerbuff ");
            return;
        }

        if (r.ExecutorInfo.MechId != ExecutorInfo.EXECUTE_INFO_NONE) //随从触发
        {
            if (r.ExecutorInfo.EquipId == ExecutorInfo.EXECUTE_INFO_NONE)
            {
                cp.BattlePlayer.BattleGroundManager.GetMech(r.ExecutorInfo.MechId).OnShowEffects(r.TriggerTime, r.TriggerRange);
            }
            else
            {
                cp.BattlePlayer.BattleGroundManager.GetEquip(r.ExecutorInfo.MechId, r.ExecutorInfo.EquipId).OnShowEffects(r.TriggerTime, r.TriggerRange);
            }
        }
        else if (r.ExecutorInfo.CardInstanceId != ExecutorInfo.EXECUTE_INFO_NONE) //手牌触发
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