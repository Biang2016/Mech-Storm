using System;
using System.Collections.Generic;
using SideEffects;

/// <summary>
/// This AI class inherit from BattleProxy for online compatibility
/// This AI class has no need of any Socket functions
/// If vs AI, PlayerA always is player, and PlayerB always is AI
/// </summary>
public class BattleProxyAI : BattleProxy
{
    public Enemy Enemy;
    public bool IsCustomizeBattle = false;

    public BattleProxyAI(int clientId, string userName, Enemy enemy, bool isCustomizeBattle) : base(clientId, userName, null, null)
    {
        Enemy = enemy;
        SendMessage = sendMessage;
        IsCustomizeBattle = isCustomizeBattle;
    }

    private void sendMessage(ServerRequestBase request)
    {
        if (request is ResponseBundleBase r)
        {
            foreach (ServerRequestBase req in r.AttachedRequests)
            {
                if (req is PlayerTurnRequest ptr) // Only listen to round start
                {
                    if (ptr.clientId == ClientID)
                    {
                        AIOperation();
                    }
                }
            }
        }
    }

    public void ReceiveMessage(ClientRequestBase request)
    {
    }

    protected void Response()
    {
    }

    #region AIOperation

    HashSet<int> TriedCards = new HashSet<int>();
    HashSet<int> TriedMechs = new HashSet<int>();

    public void ResetTriedCards()
    {
        TriedCards.Clear();
    }

    public void AIOperation()
    {
        while (true)
        {
            bool lastOperation = false;
            bool failedAgain = false;
            while (true)
            {
                // Try preset combo first
                CardCombo cc = FindCardComboInHand();
                if (cc != null)
                {
                    foreach (int cardID in cc.ComboCardIDList)
                    {
                        CardBase cb = MyPlayer.HandManager.SelectHandCardByCardID(cardID);
                        if (cb != null)
                        {
                            TryUseCard(cb);
                        }
                    }
                }

                // Try cards by card priority

                CardBase card = FindCardUsable();
                ModuleMech mech = FindMechMovable();
                if (card == null && mech == null)
                {
                    if (!lastOperation)
                    {
                        failedAgain = true; //try twice and both failed, then stop
                    }

                    break;
                }
                else
                {
                    if (card != null)
                    {
                        if (TryUseCard(card)) //succeed
                        {
                            lastOperation = true;
                        }
                        else
                        {
                            TriedCards.Add(card.M_CardInstanceId); //don't try the cards already tried
                        }
                    }

                    if (mech != null)
                    {
                        if (TryAttack(mech)) //succeed
                        {
                            lastOperation = true;
                        }
                        else
                        {
                            TriedMechs.Add(mech.M_MechID); //don't try the mechs already tried
                        }
                    }
                }
            }

            TriedCards.Clear();
            TriedMechs.Clear();

            if (failedAgain) break;
        }

        BattleGameManager.OnEndRoundRequest(new EndRoundRequest(ClientID));
    }

    /// <summary>
    /// Try use card. If card need target and there is no available target, return false
    /// 尝试使用卡牌，如果卡牌需要指定目标，但没有合适目标，则使用失败，返回false
    /// </summary>
    /// <returns></returns>
    private bool TryUseCard(CardBase card)
    {
        TargetInfo ti = card.CardInfo.TargetInfo;
        HashSet<Type> sefs = GetSideEffectFunctionsByCardInfo(card.CardInfo);

        if (card.CardInfo.BaseInfo.CardType == CardTypes.Spell || card.CardInfo.BaseInfo.CardType == CardTypes.Energy)
        {
            if (!ti.HasTarget)
            {
                BattleGameManager.OnClientUseSpellCardRequest(new UseSpellCardRequest(ClientID, card.M_CardInstanceId));
                return true;
            }
            else
            {
                if (ti.HasTargetMech)
                {
                    ModuleMech mech = GetTargetMechByTargetInfo(sefs, ti);
                    if (mech != null)
                    {
                        DebugLog.PrintError("SpelltoMech: " + card.CardInfo.BaseInfo.CardNames["zh"] + "  " + mech.CardInfo.BaseInfo.CardNames["zh"]);
                        BattleGameManager.OnClientUseSpellCardToMechRequest(new UseSpellCardToMechRequest(ClientID, card.M_CardInstanceId, new List<(int, bool)> {(mech.M_MechID, false)}));
                        return true;
                    }
                }

                if (ti.HasTargetShip)
                {
                    switch (ti.targetShipRange)
                    {
                        case TargetRange.Ships:
                        {
                            if (card.CardInfo.GetCardUseBias() < 0)
                            {
                                BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {BattleGameManager.PlayerA.ClientId}));
                            }
                            else if (card.CardInfo.GetCardUseBias() > 0)
                            {
                                BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {ClientID}));
                            }
                            else
                            {
                                Random rd = new Random();
                                int ran = rd.Next(0, 2);
                                if (ran == 0)
                                {
                                    BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {BattleGameManager.PlayerA.ClientId}));
                                }
                                else
                                {
                                    BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {ClientID}));
                                }
                            }

                            return true;
                        }
                        case TargetRange.SelfShip:
                        {
                            BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {ClientID}));
                            return true;
                        }
                        case TargetRange.EnemyShip:
                        {
                            BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {BattleGameManager.PlayerA.ClientId}));
                            return true;
                        }

                        case TargetRange.AllLife:
                        {
                            if (card.CardInfo.GetCardUseBias() < 0)
                            {
                                BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {BattleGameManager.PlayerA.ClientId}));
                            }
                            else if (card.CardInfo.GetCardUseBias() > 0)
                            {
                                BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {ClientID}));
                            }
                            else
                            {
                                Random rd = new Random();
                                int ran = rd.Next(0, 2);
                                if (ran == 0)
                                {
                                    BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {BattleGameManager.PlayerA.ClientId}));
                                }
                                else
                                {
                                    BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {ClientID}));
                                }
                            }

                            return true;
                        }
                        case TargetRange.SelfLife:
                        {
                            BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {ClientID}));
                            return true;
                        }
                        case TargetRange.EnemyLife:
                        {
                            BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {BattleGameManager.PlayerA.ClientId}));
                            return true;
                        }

                        case TargetRange.Decks:
                        {
                            if (card.CardInfo.GetCardUseBias() < 0)
                            {
                                BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {BattleGameManager.PlayerA.ClientId}));
                            }
                            else if (card.CardInfo.GetCardUseBias() > 0)
                            {
                                BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {ClientID}));
                            }
                            else
                            {
                                Random rd = new Random();
                                int ran = rd.Next(0, 2);
                                if (ran == 0)
                                {
                                    BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {BattleGameManager.PlayerA.ClientId}));
                                }
                                else
                                {
                                    BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {ClientID}));
                                }
                            }

                            return true;
                        }
                        case TargetRange.SelfDeck:
                        {
                            BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {ClientID}));
                            return true;
                        }
                        case TargetRange.EnemyDeck:
                        {
                            BattleGameManager.OnClientUseSpellCardToShipRequest(new UseSpellCardToShipRequest(ClientID, card.M_CardInstanceId, new List<int> {BattleGameManager.PlayerA.ClientId}));
                            return true;
                        }
                    }
                }

                if (ti.HasTargetEquip) //Todo
                {
                    switch (ti.targetMechRange)
                    {
                        case TargetRange.EnemyMechs:
                        {
                            break;
                        }
                        case TargetRange.EnemyHeroes:
                        {
                            break;
                        }
                        case TargetRange.EnemySoldiers:
                        {
                            break;
                        }
                        case TargetRange.SelfMechs:
                        {
                            break;
                        }
                        case TargetRange.SelfHeroes:
                        {
                            break;
                        }
                        case TargetRange.SelfSoldiers:
                        {
                            break;
                        }
                    }
                }
            }
        }
        else if (card.CardInfo.BaseInfo.CardType == CardTypes.Mech)
        {
            if (MyPlayer.BattleGroundManager.BattleGroundIsFull) return false;

            bool canSummonDirectly = false;
            canSummonDirectly |= !ti.HasTarget;
            canSummonDirectly |= (ti.targetMechRange == TargetRange.SelfMechs && MyPlayer.BattleGroundManager.MechCount == 0);
            canSummonDirectly |= (ti.targetMechRange == TargetRange.SelfHeroes && MyPlayer.BattleGroundManager.HeroCount == 0);
            canSummonDirectly |= (ti.targetMechRange == TargetRange.SelfSoldiers && MyPlayer.BattleGroundManager.SoldierCount == 0);
            canSummonDirectly |= (ti.targetMechRange == TargetRange.EnemyMechs && EnemyPlayer.BattleGroundManager.MechCount == 0);
            canSummonDirectly |= (ti.targetMechRange == TargetRange.EnemyHeroes && EnemyPlayer.BattleGroundManager.HeroCount == 0);
            canSummonDirectly |= (ti.targetMechRange == TargetRange.EnemySoldiers && EnemyPlayer.BattleGroundManager.SoldierCount == 0);
            //Todo 针对装备等还没处理

            if (canSummonDirectly)
            {
                BattleGameManager.OnClientSummonMechRequest(new SummonMechRequest(ClientID, card.M_CardInstanceId, MyPlayer.BattleGroundManager.MechCount, (int) Const.SpecialMechID.ClientTempMechIDNormal, null, null));
                return true;
            }

            if (ti.HasTargetMech)
            {
                ModuleMech mech = GetTargetMechByTargetInfo(sefs, ti);
                if (mech != null)
                {
                    BattleGameManager.OnClientSummonMechRequest(new SummonMechRequest(ClientID, card.M_CardInstanceId, MyPlayer.BattleGroundManager.MechCount, (int) Const.SpecialMechID.ClientTempMechIDNormal, new List<int> {mech.M_MechID}, new List<bool> {false}));
                    return true;
                }
            }
            else if (ti.HasTargetShip)
            {
                //Todo 针对战舰等还没处理
            }
            else if (ti.HasTargetEquip)
            {
                //Todo 针对装备等还没处理
            }
        }
        else if (card.CardInfo.BaseInfo.CardType == CardTypes.Equip)
        {
            // Select suitable mech to equip
            ModuleMech mech = SelectMechToEquip(MyPlayer.BattleGroundManager.Heroes, card.CardInfo.EquipInfo.SlotType, (CardInfo_Equip) card.CardInfo); //Hero first
            if (mech == null) mech = SelectMechToEquip(MyPlayer.BattleGroundManager.Soldiers, card.CardInfo.EquipInfo.SlotType, (CardInfo_Equip) card.CardInfo);

            if (mech != null)
            {
                switch (card.CardInfo.EquipInfo.SlotType)
                {
                    case SlotTypes.Weapon:
                    {
                        BattleGameManager.OnClientEquipWeaponRequest(new EquipWeaponRequest(ClientID, card.M_CardInstanceId, mech.M_MechID));
                        return true;
                    }
                    case SlotTypes.Shield:
                    {
                        BattleGameManager.OnClientEquipShieldRequest(new EquipShieldRequest(ClientID, card.M_CardInstanceId, mech.M_MechID));
                        return true;
                    }
                    case SlotTypes.Pack:
                    {
                        BattleGameManager.OnClientEquipPackRequest(new EquipPackRequest(ClientID, card.M_CardInstanceId, mech.M_MechID));
                        return true;
                    }
                    case SlotTypes.MA:
                    {
                        BattleGameManager.OnClientEquipMARequest(new EquipMARequest(ClientID, card.M_CardInstanceId, mech.M_MechID));
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool TryAttack(ModuleMech mech)
    {
        if (EnemyPlayer.CheckModuleMechCanAttackMe(mech)) // Attack ship first
        {
            BattleGameManager.OnClientMechAttackShipRequest(new MechAttackShipRequest(ClientID, mech.M_MechID));
            return true;
        }

        foreach (ModuleMech targetMech in EnemyPlayer.BattleGroundManager.Mechs)
        {
            if (targetMech.CheckMechCanAttackMe(mech))
            {
                BattleGameManager.OnClientMechAttackMechRequest(new MechAttackMechRequest(ClientID, mech.M_MechID, EnemyPlayer.ClientId, targetMech.M_MechID));
                return true;
            }
        }

        return false;
    }

    private HashSet<Type> GetSideEffectFunctionsByCardInfo(CardInfo_Base cardInfo)
    {
        HashSet<Type> res = new HashSet<Type>();

        foreach (SideEffectExecute see in cardInfo.SideEffectBundle.SideEffectExecutes)
        {
            foreach (SideEffectBase se in see.SideEffectBases)
            {
                if (se is IPositive)
                {
                    res.Add(typeof(IPositive));
                }

                if (se is INegative)
                {
                    res.Add(typeof(INegative));
                }

                if (se is IStrengthen)
                {
                    res.Add(typeof(IStrengthen));
                }

                if (se is IWeaken)
                {
                    res.Add(typeof(IWeaken));
                }

                if (se is IPriorUsed)
                {
                    res.Add(typeof(IPriorUsed));
                }

                if (se is IPostUsed)
                {
                    res.Add(typeof(IPostUsed));
                }

                if (se is IDefend)
                {
                    res.Add(typeof(IDefend));
                }

                if (se is IShipEnergy)
                {
                    res.Add(typeof(IShipEnergy));
                }

                if (se is AddLife addLife)
                {
                    if ((addLife.TargetRange & TargetRange.SelfShip) != 0)
                    {
                        res.Add(typeof(IShipLife));
                    }
                }

                if (se is Heal heal)
                {
                    if ((heal.TargetRange & TargetRange.SelfShip) != 0)
                    {
                        res.Add(typeof(IShipLife));
                    }
                }
            }
        }

        return res;
    }

    private ModuleMech GetTargetMechByTargetInfo(HashSet<Type> sefs, TargetInfo ti)
    {
        BattlePlayer targetPlayer = null;
        MechTypes targetMechType = MechTypes.All;
        if ((ti.targetMechRange | TargetRange.EnemyMechs) == TargetRange.EnemyMechs)
        {
            targetPlayer = BattleGameManager.PlayerA;
        }
        else if ((ti.targetMechRange | TargetRange.SelfMechs) == TargetRange.SelfMechs)
        {
            targetPlayer = BattleGameManager.PlayerB;
        }

        if ((ti.targetMechRange | TargetRange.Heroes) == TargetRange.Heroes)
        {
            targetMechType = MechTypes.Hero;
        }
        else if ((ti.targetMechRange | TargetRange.Soldiers) == TargetRange.Soldiers)
        {
            targetMechType = MechTypes.Soldier;
        }
        else
        {
            targetMechType = MechTypes.All;
        }

        ModuleMech mech = null;
        if (targetPlayer != null)
        {
            mech = targetPlayer.BattleGroundManager.GetRandomAliveMechExcept(targetMechType, -1);
        }
        else // According to card beneficial bias, determine exert on which side 
        {
            if ((sefs.Contains(typeof(INegative))) || sefs.Contains(typeof(IWeaken)))
            {
                targetPlayer = BattleGameManager.PlayerA;
                mech = targetPlayer.BattleGroundManager.GetRandomAliveMechExcept(targetMechType, -1);
            }
            else if (sefs.Contains(typeof(IPositive)) || sefs.Contains(typeof(IStrengthen)) || sefs.Contains(typeof(IShipEnergy)) || sefs.Contains(typeof(IShipLife)) || sefs.Contains(typeof(IDefend)))
            {
                targetPlayer = BattleGameManager.PlayerB;
                mech = targetPlayer.BattleGroundManager.GetRandomAliveMechExcept(targetMechType, -1);
            }
            else
            {
                mech = BattleGameManager.GetRandomAliveMechExcept(targetMechType, -1);
            }
        }

        return mech;
    }

    internal bool CheckMechCanEquipMe(ModuleMech mech, CardInfo_Equip equipInfo)
    {
        if (MyPlayer == mech.BattlePlayer && mech.CardInfo.MechInfo.HasSlotType(equipInfo.EquipInfo.SlotType) && !mech.M_IsDead)
        {
            if (equipInfo.EquipInfo.SlotType == SlotTypes.Weapon && equipInfo.WeaponInfo.WeaponType == WeaponTypes.SniperGun)
            {
                if (mech.CardInfo.MechInfo.IsSniper)
                {
                    return true; // Sniper gun only on sniper
                }
                else
                {
                    return false;
                }
            }
            else if (equipInfo.EquipInfo.SlotType == SlotTypes.MA)
            {
                if (mech.IsAllEquipExceptMA)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    private CardCombo FindCardComboInHand()
    {
        List<int> cardIDs = new List<int>();
        foreach (CardBase cb in MyPlayer.HandManager.Cards)
        {
            cardIDs.Add(cb.CardInfo.CardID);
        }

        foreach (CardCombo cardCombo in Enemy.CardComboList)
        {
            if (HasCombo(cardCombo, cardIDs))
            {
                return cardCombo;
            }
        }

        return null;
    }

    public bool HasCombo(CardCombo cardCombo, List<int> cardIDs)
    {
        HashSet<int> tempComboCardIDHashSet = CloneVariantUtils.HashSet(cardCombo.ComboCardIDHashSet);
        foreach (int cardID in cardIDs)
        {
            tempComboCardIDHashSet.Remove(cardID);
        }

        if (tempComboCardIDHashSet.Count == 0)
        {
            int energyLeft = MyPlayer.EnergyLeft;
            int metalLeft = MyPlayer.MetalLeft;

            foreach (int cardID in cardCombo.ComboCardIDList)
            {
                CardInfo_Base ci = AllCards.GetCard(cardID);
                if (ci.BaseInfo.Metal <= metalLeft && ci.BaseInfo.Energy <= energyLeft)
                {
                    energyLeft -= ci.BaseInfo.Energy;
                    metalLeft -= ci.BaseInfo.Metal;
                    energyLeft += CheckEnergyAddedDirectlyInSideEffectBundle(ci.SideEffectBundle);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public int CheckEnergyAddedDirectlyInSideEffectBundle(SideEffectBundle seb)
    {
        int energyAdded = 0;
        foreach (SideEffectExecute see in seb.SideEffectExecutes)
        {
            if (see.ExecuteSettingType == SideEffectExecute.ExecuteSettingTypes.PlayOutEffect || see.ExecuteSettingType == SideEffectExecute.ExecuteSettingTypes.BattleCry)
            {
                foreach (SideEffectBase se in see.SideEffectBases)
                {
                    if (se is AddEnergy addEnergy)
                    {
                        energyAdded += addEnergy.M_SideEffectParam.GetParam_MultipliedInt("Energy") * addEnergy.M_SideEffectParam.GetParam_ConstInt("Times");
                    }
                }
            }
        }

        return energyAdded;
    }

    private CardBase FindCardUsable()
    {
        List<int> noTriedUsableCards = new List<int>();
        foreach (int id in MyPlayer.HandManager.UsableCards)
        {
            if (!TriedCards.Contains(id))
            {
                noTriedUsableCards.Add(id);
            }
        }

        if (noTriedUsableCards.Count == 1) return MyPlayer.HandManager.GetCardByCardInstanceId(noTriedUsableCards[0]);

        // Play card by priority
        int mostPriority = 99999;
        CardBase mostPriorCard = null;
        foreach (int id in noTriedUsableCards)
        {
            CardBase card = MyPlayer.HandManager.GetCardByCardInstanceId(id);
            int cardID = card.CardInfo.CardID;
            int index = Enemy.CardPriority.CardIDListByPriority.IndexOf(cardID);
            if (index != -1 && index < mostPriority)
            {
                mostPriority = index;
                mostPriorCard = card;
            }
        }

        // Fallback stupid play cards
        if (mostPriorCard == null)
        {
            CardBase energyCardID = null;
            CardBase spellCardID = null;
            CardBase equipCardID = null;
            CardBase mechCardID = null;
            foreach (int id in noTriedUsableCards)
            {
                CardBase card = MyPlayer.HandManager.GetCardByCardInstanceId(id);
                if (card.CardInfo.BaseInfo.CardType == CardTypes.Energy)
                {
                    energyCardID = card;
                }

                if (card.CardInfo.BaseInfo.CardType == CardTypes.Spell)
                {
                    spellCardID = card;
                }

                if (card.CardInfo.BaseInfo.CardType == CardTypes.Equip)
                {
                    equipCardID = card;
                }

                if (card.CardInfo.BaseInfo.CardType == CardTypes.Mech)
                {
                    mechCardID = card;
                }
            }

            if (energyCardID != null)
            {
                return energyCardID;
            }

            if (spellCardID != null)
            {
                return spellCardID;
            }

            if (equipCardID != null)
            {
                return equipCardID;
            }

            if (mechCardID != null)
            {
                return mechCardID;
            }
        }

        return mostPriorCard;
    }

    private ModuleMech FindMechMovable()
    {
        foreach (int id in MyPlayer.BattleGroundManager.CanAttackMechs)
        {
            if (!TriedMechs.Contains(id))
            {
                return MyPlayer.BattleGroundManager.GetMech(id);
            }
        }

        return null;
    }

    private bool TryAttack()
    {
        return false;
    }

    #endregion

    #region AI_Mark_System 评分系统

    private ModuleMech SelectMechToEquip(List<ModuleMech> mechs, SlotTypes slotType, CardInfo_Equip cardInfo)
    {
        List<ModuleMech> mechs_NoEquip = new List<ModuleMech>(); // Give priority to those who have no such equipment

        List<ModuleMech> optionalMech = new List<ModuleMech>();
        foreach (ModuleMech mech in mechs) 
        {
            if (CheckMechCanEquipMe(mech, cardInfo))
            {
                optionalMech.Add(mech);
            }
        }

        foreach (ModuleMech mech in optionalMech)
        {
            switch (slotType)
            {
                case SlotTypes.Weapon:
                {
                    if (mech.M_Weapon == null)
                    {
                        mechs_NoEquip.Add(mech);
                    }

                    break;
                }
                case SlotTypes.Shield:
                {
                    if (mech.M_Shield == null)
                    {
                        mechs_NoEquip.Add(mech);
                    }

                    break;
                }
                case SlotTypes.Pack:
                {
                    if (mech.M_Pack == null)
                    {
                        mechs_NoEquip.Add(mech);
                    }

                    break;
                }
                case SlotTypes.MA:
                {
                    if (mech.M_MA == null)
                    {
                        mechs_NoEquip.Add(mech);
                    }

                    break;
                }
            }
        }

        if (mechs_NoEquip.Count != 0) //Give priority to the strongest among those who have no such equipment
        {
            return GetMechByEvaluation(mechs_NoEquip, EvaluationOption.Mech, EvaluationDirection.Max);
        }
        else // Give priority to the weakest one if they all have equipment
        {
            switch (slotType)
            {
                case SlotTypes.Weapon:
                {
                    return GetMechByEvaluation(mechs_NoEquip, EvaluationOption.Weapon, EvaluationDirection.Min);
                }
                case SlotTypes.Shield:
                {
                    return GetMechByEvaluation(mechs_NoEquip, EvaluationOption.Shield, EvaluationDirection.Min);
                }
                case SlotTypes.Pack:
                {
                    return GetMechByEvaluation(mechs_NoEquip, EvaluationOption.Pack, EvaluationDirection.Min);
                }
                case SlotTypes.MA:
                {
                    return GetMechByEvaluation(mechs_NoEquip, EvaluationOption.MA, EvaluationDirection.Min);
                }
            }
        }

        return null;
    }

    enum EvaluationOption
    {
        Mech,
        Weapon,
        Shield,
        Pack,
        MA
    }

    enum EvaluationDirection
    {
        Max,
        Min
    }

    delegate float GetMechByEvaluationDelegate(ModuleMech mech);

    Dictionary<EvaluationOption, GetMechByEvaluationDelegate> EvaluationMethodDict = new Dictionary<EvaluationOption, GetMechByEvaluationDelegate>
    {
        {EvaluationOption.Mech, CountMechValue},
        {EvaluationOption.Weapon, CountMechWeaponValue},
        {EvaluationOption.Shield, CountMechShieldValue},
        {EvaluationOption.Pack, CountMechPackValue},
        {EvaluationOption.MA, CountMechMAValue}
    };

    private ModuleMech GetMechByEvaluation(List<ModuleMech> optionalMech, EvaluationOption evaluationOption, EvaluationDirection evaluationDirection)
    {
        ModuleMech res = null;
        float resMard = 0;
        foreach (ModuleMech mech in optionalMech)
        {
            float mark = EvaluationMethodDict[evaluationOption](mech);

            if (evaluationDirection == EvaluationDirection.Max)
            {
                if (mark > resMard)
                {
                    res = mech;
                    resMard = mark;
                }
            }
            else
            {
                if (mark < resMard)
                {
                    res = mech;
                    resMard = mark;
                }
            }
        }

        return res;
    }

    private static float CountMechValue(ModuleMech mech)
    {
        float mark = 0;

        mark += mech.CardInfo.BaseInfo.BaseValue();
        mark += mech.M_MechLeftLife;
        mark += mech.M_MechShield * 3 + mech.M_MechArmor;

        mark += CountMechWeaponValue(mech);
        mark += CountMechShieldValue(mech);
        mark += CountMechPackValue(mech);
        mark += CountMechMAValue(mech);

        return mark;
    }

    private static float CountMechWeaponValue(ModuleMech mech)
    {
        float mark = 0;
        if (mech.M_Weapon != null)
        {
            mark += mech.M_Weapon.CardInfo.BaseInfo.BaseValue();
            mark += mech.M_MechWeaponEnergy * mech.M_MechAttack;
        }

        return mark;
    }

    private static float CountMechShieldValue(ModuleMech mech)
    {
        float mark = 0;
        if (mech.M_Shield != null)
        {
            mark += mech.M_Shield.CardInfo.BaseInfo.BaseValue();
        }

        return mark;
    }

    private static float CountMechPackValue(ModuleMech mech)
    {
        float mark = 0;
        if (mech.M_Pack != null)
        {
            mark += mech.M_Pack.CardInfo.BaseInfo.BaseValue();
        }

        return mark;
    }

    private static float CountMechMAValue(ModuleMech mech)
    {
        float mark = 0;

        if (mech.M_MA != null)
        {
            mark += mech.M_MA.CardInfo.BaseInfo.BaseValue();
        }

        return mark;
    }

    #endregion
}