using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BattleGroundManager : MonoBehaviour
{
    private float MECH_INTERVAL = 3.5f;

    private int mechCount;

    public int MechCount //接到增加机甲协议就更新数量（实体后面才生成）
    {
        get { return mechCount; }
        set { mechCount = value; }
    }

    private int heroCount;

    public int HeroCount //接到增加机甲协议就更新数量（实体后面才生成）
    {
        get { return heroCount; }
        set { heroCount = value; }
    }

    private int soldierCount;

    public int SoldierCount //接到增加机甲协议就更新数量（实体后面才生成）
    {
        get { return soldierCount; }
        set { soldierCount = value; }
    }

    public bool BattleGroundIsFull
    {
        get { return MechCount == GamePlaySettings.MaxMechNumber; }
    }

    public bool BattleGroundIsEmpty
    {
        get { return MechCount == 0; }
    }

    public bool HasDefenceMech
    {
        get
        {
            foreach (ModuleMech mech in Mechs)
            {
                if (mech.IsDefender) return true;
            }

            return false;
        }
    }

    public bool HerosIsEmpty
    {
        get { return HeroCount == 0; }
    }

    public bool SoldiersIsEmpty
    {
        get { return SoldierCount == 0; }
    }

    [SerializeField] private Transform DefaultMechPivot;

    internal ClientPlayer ClientPlayer;
    internal List<ModuleMech> Mechs = new List<ModuleMech>();
    internal List<ModuleMech> Heros = new List<ModuleMech>();
    internal List<ModuleMech> Soldiers = new List<ModuleMech>();

    public void Initialize(ClientPlayer clientPlayer)
    {
        ResetAll();
        ClientPlayer = clientPlayer;
    }

    public void ResetAll()
    {
        StopAllCoroutines();
        foreach (ModuleMech moduleMech in Mechs)
        {
            moduleMech.PoolRecycle();
        }

        MechCount = 0;
        HeroCount = 0;
        SoldierCount = 0;

        ClientPlayer = null;
        previewMechPlace = PREVIEW_MECH_PLACES_NO_PREVIEW_MECH_NOW;
        Mechs.Clear();
        Heros.Clear();
        Soldiers.Clear();
        MechCount = 0;
        HeroCount = 0;
        SoldierCount = 0;
        RemoveMechs.Clear();
        addPrePassMechQueue.Clear();
        clientMechTempId = 0;
        relatedSlots.Clear();
        if (currentSummonPreviewMechCard) currentSummonPreviewMechCard.PoolRecycle();
        if (CurrentSummonPreviewMech) CurrentSummonPreviewMech.PoolRecycle();
    }

    public void SetLanguage(string languageShort)
    {
        foreach (ModuleMech mech in Mechs)
        {
            mech.SetLanguage(languageShort);
        }
    }

    #region 位置计算

    internal int ComputePosition(Vector3 dragLastPosition)
    {
        int index = Mathf.RoundToInt(Mathf.Floor(dragLastPosition.x / MECH_INTERVAL - (Mechs.Count + 1) % 2 * 0.5f) + (Mechs.Count / 2 + 1));
        if (index < 0) index = 0;
        if (index >= Mechs.Count) index = Mechs.Count;
        return index;
    }

    internal int ComputePositionInAliveMechs(Vector3 dragLastPosition)
    {
        int battleGroundIndex = ComputePosition(dragLastPosition);
        return GetIndexOfAliveMechs(battleGroundIndex);
    }

    internal ModuleMech CheckMechOnPosition(Vector3 dragLastPosition)
    {
        int index = Mathf.RoundToInt(Mathf.Floor(dragLastPosition.x / MECH_INTERVAL - (Mechs.Count + 1) % 2 * 0.5f) + (Mechs.Count / 2 + 1));
        if (index < 0 || index >= Mechs.Count)
            return null;
        return Mechs[index];
    }

    #endregion

    #region 召唤和移除

    public ModuleMech AddMech_PrePass(CardInfo_Mech mechCardInfo, int mechId, int clientMechTempId)
    {
        if (ClientPlayer == null) return null;
        if (previewMechPlace != PREVIEW_MECH_PLACES_NO_PREVIEW_MECH_NOW)
        {
            previewMechPlace = PREVIEW_MECH_PLACES_NO_PREVIEW_MECH_NOW;
        }

        bool isSummonedBeforeByPreview = false;
        if (clientMechTempId >= 0)
        {
            foreach (ModuleMech moduleMech in Mechs)
            {
                if (moduleMech.M_ClientTempMechID == clientMechTempId) //匹配
                {
                    moduleMech.M_MechID = mechId; //赋予正常ID
                    moduleMech.M_ClientTempMechID = Const.CLIENT_TEMP_MECH_ID_NORMAL; //恢复普通
                    isSummonedBeforeByPreview = true;
                    break;
                }
            }
        }

        if (!isSummonedBeforeByPreview)
        {
            ModuleMech mech = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ModuleMech].AllocateGameObject<ModuleMech>(transform);
            mech.transform.position = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ModuleMech].transform.position;
            mech.Initiate(mechCardInfo, ClientPlayer);
            mech.transform.Rotate(Vector3.up, 180);
            mech.M_MechID = mechId;
            addPrePassMechQueue.Enqueue(mech);
            MechCount++;
            if (!mechCardInfo.MechInfo.IsSoldier)
            {
                HeroCount++;
            }
            else
            {
                SoldierCount++;
            }

            return mech;
        }

        return null;
    }

    private Queue<ModuleMech> addPrePassMechQueue = new Queue<ModuleMech>();

    public void AddMech(int mechPlaceIndex)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RefreshBattleGroundAnim(BattleEffectsManager.Instance.Effect_Main, mechPlaceIndex), "Co_RefreshBattleGroundAnim");
    }

    public void RemoveMech(int mechId)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RemoveMech(GetMech(mechId)), "Co_RemoveMech");
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RefreshBattleGroundAnim(BattleEffectsManager.Instance.Effect_Main), "Co_RefreshBattleGroundAnim");
    }

    internal List<ModuleMech> RemoveMechs = new List<ModuleMech>(); //即将要被移除的机甲名单

    public void RemoveMechTogetherAdd(int mechId)
    {
        ModuleMech mech = GetMech(mechId);
        MechCount--;
        if (!mech.CardInfo.MechInfo.IsSoldier)
        {
            HeroCount--;
        }
        else
        {
            SoldierCount--;
        }

        RemoveMechs.Add(mech);
    }

    public void RemoveMechTogether(List<int> removeMechList)
    {
        foreach (int rid in removeMechList)
        {
            ModuleMech mech = GetMech(rid);
            if (mech)
            {
                mech.PoolRecycle();
                Mechs.Remove(mech);
                RemoveMechs.Remove(mech);
                if (!mech.CardInfo.MechInfo.IsSoldier)
                {
                    Heros.Remove(mech);
                }
                else
                {
                    Soldiers.Remove(mech);
                }

                ClientLog.Instance.Print("remove:" + mech.M_MechID);
            }
        }
    }

    public void RemoveMechTogatherEnd()
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RefreshBattleGroundAnim(BattleEffectsManager.Instance.Effect_Main), "Co_RefreshBattleGroundAnim");
    }

    IEnumerator Co_RemoveMech(ModuleMech mech)
    {
        mech.PoolRecycle();
        Mechs.Remove(mech);
        MechCount--;
        if (!mech.CardInfo.MechInfo.IsSoldier)
        {
            Heros.Remove(mech);
            HeroCount--;
        }
        else
        {
            Soldiers.Remove(mech);
            SoldierCount--;
        }

        //PrintMechInfos();
        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #region 出牌召唤预览

    private int previewMechPlace;

    private const int PREVIEW_MECH_PLACES_NO_PREVIEW_MECH_NOW = -1; //无预览召唤机甲

    public void AddMechPreview(int placeIndex)
    {
        if (Mechs.Count == 0) return;
        if (previewMechPlace == PREVIEW_MECH_PLACES_NO_PREVIEW_MECH_NOW || previewMechPlace != placeIndex)
        {
            previewMechPlace = placeIndex;
            BattleEffectsManager.Instance.Effect_RefreshBattleGroundOnAddMech.EffectsShow(Co_RefreshBattleGroundAnim(BattleEffectsManager.Instance.Effect_RefreshBattleGroundOnAddMech), "Co_RefreshBattleGroundAnim");
        }
    }

    public void RemoveMechPreview()
    {
        if (previewMechPlace != PREVIEW_MECH_PLACES_NO_PREVIEW_MECH_NOW)
        {
            previewMechPlace = PREVIEW_MECH_PLACES_NO_PREVIEW_MECH_NOW;
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RefreshBattleGroundAnim(BattleEffectsManager.Instance.Effect_Main), "Co_RefreshBattleGroundAnim");
        }
    }

    #endregion

    #region 能指定目标的机甲的预召唤

    private static int clientMechTempId = 0;

    public static int GenerateClientMechTempId()
    {
        return clientMechTempId++;
    }

    public delegate void SummonMechTarget(int targetMechId, bool isClientMechTempId = false);

    private CardMech currentSummonPreviewMechCard;
    internal ModuleMech CurrentSummonPreviewMech;

    public void SummonMechPreview(CardMech mechCard, int mechPlaceIndex, TargetRange targetRange) //用于具有指定目标的副作用的机甲的召唤预览、显示指定箭头
    {
        currentSummonPreviewMechCard = mechCard;
        ModuleMech mech = AddMech_PrePass((CardInfo_Mech) mechCard.CardInfo, (int) ModuleMech.MechID.Empty, (int) Const.CLIENT_TEMP_MECH_ID_NORMAL);
        CurrentSummonPreviewMech = mech;
        AddMech(mechPlaceIndex);
        DragManager.Instance.SummonMechTargetHandler = SummonMechTargetConfirm;
        DragManager.Instance.StartArrowAiming(mech, targetRange);
    }

    public void SummonMechTargetConfirm(int targetMechId, bool isClientMechTempId)
    {
        if (targetMechId == DragManager.TARGET_SELECT_NONE) //未选择目标
        {
            RemoveMech((int) ModuleMech.MechID.Empty);
            ClientPlayer.BattlePlayer.HandManager.CancelSummonMechPreview();
        }
        else
        {
            StartCoroutine(Co_RetrySummonRequest(CurrentSummonPreviewMech, currentSummonPreviewMechCard.M_CardInstanceId, targetMechId, isClientMechTempId));
        }
    }

    IEnumerator Co_RetrySummonRequest(ModuleMech mech, int cardInstanceId, int targetMechId, bool isClientMechTempId)
    {
        while (true)
        {
            int battleGroundIndex = GetIndexOfAliveMechs(mech); //确定的时候再获取位置信息（召唤的过程中可能会有协议没有跑完，会有机甲生成）

            if (battleGroundIndex != -1)
            {
                mech.M_ClientTempMechID = GenerateClientMechTempId();
                SummonMechRequest request = new SummonMechRequest(Client.Instance.Proxy.ClientId, cardInstanceId, battleGroundIndex, targetMechId, isClientMechTempId, mech.M_ClientTempMechID);
                Client.Instance.Proxy.SendMessage(request);
                break;
            }

            yield return null;
        }

        yield return null;
    }

    private int GetIndexOfAliveMechs(ModuleMech mech)
    {
        int battleGroundIndex = Mechs.IndexOf(mech);
        return GetIndexOfAliveMechs(battleGroundIndex);
    }

    private int GetIndexOfAliveMechs(int battleGroundIndex)
    {
        //去除掉已经死亡但还没移除战场的机甲（避免服务器指针错误）
        int countDieMech = 0;
        for (int i = 0; i < battleGroundIndex; i++)
        {
            if (RemoveMechs.Contains(Mechs[i]))
            {
                countDieMech++;
            }
        }

        int aliveIndex = battleGroundIndex - countDieMech;
        return aliveIndex;
    }

    #endregion

    private enum mechPlaceIndex
    {
        NoNewMech = -1,
    }

    IEnumerator Co_RefreshBattleGroundAnim(BattleEffectsManager.Effects myParentEffects, int mechPlaceIndex = (int) mechPlaceIndex.NoNewMech)
    {
        bool isAddMech = mechPlaceIndex != (int) BattleGroundManager.mechPlaceIndex.NoNewMech;
        if (isAddMech) //新增机甲
        {
            ModuleMech mech = addPrePassMechQueue.Dequeue();

            Mechs.Insert(mechPlaceIndex, mech);
            if (mech.CardInfo.MechInfo.IsSoldier)
            {
                Soldiers.Add(mech);
            }
            else
            {
                Heros.Add(mech);
            }

            mech.OnSummon();
            mech.transform.position = DefaultMechPivot.transform.position;
            mech.transform.transform.Translate(Vector3.left * (Mechs.IndexOf(mech) - Mechs.Count / 2.0f + 0.5f) * MECH_INTERVAL, Space.Self);
        }

        float duration = 0.1f;
        Vector3[] targetPos = new Vector3[Mechs.Count];
        int actualPlaceCount = previewMechPlace == PREVIEW_MECH_PLACES_NO_PREVIEW_MECH_NOW ? Mechs.Count : Mechs.Count + 1;

        for (int i = 0; i < Mechs.Count; i++)
        {
            int actualPlace = i;
            if (previewMechPlace != PREVIEW_MECH_PLACES_NO_PREVIEW_MECH_NOW && i >= previewMechPlace)
            {
                actualPlace += 1;
            }

            Vector3 ori = Mechs[i].transform.position;
            Vector3 offset = Vector3.left * (actualPlace - actualPlaceCount / 2.0f + 0.5f) * MECH_INTERVAL;

            Mechs[i].transform.position = DefaultMechPivot.transform.position;
            Mechs[i].transform.Translate(offset, Space.Self);

            targetPos[i] = Mechs[i].transform.position;
            Mechs[i].transform.position = ori;
            Mechs[i].transform.DOMove(targetPos[i], duration).SetEase(Ease.Linear);
        }

        ClientPlayer.BattlePlayer.HandManager.RefreshAllCardUsable();

        yield return new WaitForSeconds(duration);
        if (isAddMech && DragManager.Instance.IsSummonPreview)
        {
            DragManager.Instance.IsArrowShowBegin = true;
        }

        yield return null;

        myParentEffects.EffectEnd();
    }

    #region 装备牌相关操作会显示提示性Slot边框

    List<Slot> relatedSlots = new List<Slot>();

    private IEnumerator currentShowSlotBloom;

    public void ShowTipSlotBlooms(CardEquip cardEquip)
    {
        StopShowSlotBloom();
        SlotTypes slotType = cardEquip.M_EquipType;
        foreach (ModuleMech mech in Mechs)
        {
            Slot slot1 = mech.CardSlotsComponent.Slots[0];
            if (slot1.MSlotTypes == slotType)
            {
                if (cardEquip.CardInfo.WeaponInfo.WeaponType == WeaponTypes.SniperGun)
                {
                    if (mech.CardInfo.MechInfo.IsSniper) relatedSlots.Add(slot1);
                }
                else
                {
                    relatedSlots.Add(slot1);
                }
            }

            Slot slot2 = mech.CardSlotsComponent.Slots[1];
            if (slot2.MSlotTypes == slotType)
            {
                relatedSlots.Add(slot2);
            }

            Slot slot3 = mech.CardSlotsComponent.Slots[2];
            if (slot3.MSlotTypes == slotType)
            {
                relatedSlots.Add(slot3);
            }

            Slot slot4 = mech.CardSlotsComponent.Slots[3];
            if (slot4.MSlotTypes == slotType)
            {
                relatedSlots.Add(slot4);
            }
        }

        currentShowSlotBloom = Co_ShowSlotBloom(cardEquip);
        BattleEffectsManager.Instance.Effect_TipSlotBloom.EffectsShow(currentShowSlotBloom, "Co_ShowSlotBloom");
    }

    IEnumerator Co_ShowSlotBloom(CardEquip cardEquip)
    {
        while (true)
        {
            foreach (Slot sa in relatedSlots)
            {
                sa.ShowSlotBloom(true, false);
                if (cardEquip.CardInfo.WeaponInfo.WeaponType == WeaponTypes.SniperGun)
                {
                    if (sa.Mech.CardInfo.MechInfo.IsSniper) sa.Mech.ShowSniperTipText(true);
                }
            }

            yield return new WaitForSeconds(0.4f);
            foreach (Slot sa in relatedSlots)
            {
                sa.ShowSlotBloom(false, false);
                sa.Mech.ShowSniperTipText(false);
            }

            yield return new WaitForSeconds(0.4f);
        }
    }

    public void StopShowSlotBloom()
    {
        if (currentShowSlotBloom != null)
        {
            BattleEffectsManager.SideEffect cur_Effect = BattleEffectsManager.Instance.Effect_TipSlotBloom.GetCurrentSideEffect();
            if (cur_Effect != null && cur_Effect.Enumerator == currentShowSlotBloom)
            {
                BattleEffectsManager.Instance.Effect_TipSlotBloom.EffectEnd();
                ClientLog.Instance.PrintWarning("Stop Effect_TipSlotBloom");
                currentShowSlotBloom = null;
            }
        }

        foreach (Slot sa in relatedSlots)
        {
            sa.ShowSlotBloom(false, false);
            sa.Mech.ShowSniperTipText(false);
        }

        relatedSlots.Clear();
    }

    public void ShowTipModuleBloomSE(float seconds)
    {
        foreach (ModuleMech mech in Mechs)
        {
            if (mech.MechEquipSystemComponent.M_Weapon) mech.MechEquipSystemComponent.M_Weapon.ShowEquipBloomSE(seconds);
            if (mech.MechEquipSystemComponent.M_Shield) mech.MechEquipSystemComponent.M_Shield.ShowEquipBloomSE(seconds);
            if (mech.MechEquipSystemComponent.M_Pack) mech.MechEquipSystemComponent.M_Pack.ShowEquipBloomSE(seconds);
            if (mech.MechEquipSystemComponent.M_MA) mech.MechEquipSystemComponent.M_MA.ShowEquipBloomSE(seconds);
        }
    }

    #endregion

    public void EquipWeapon(CardInfo_Equip cardInfo, int mechId, int equipId)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_EquipWeapon(cardInfo, mechId, equipId), "Co_EquipWeapon");
    }

    IEnumerator Co_EquipWeapon(CardInfo_Equip cardInfo, int mechId, int equipId)
    {
        ModuleMech mech = GetMech(mechId);
        if (cardInfo != null)
        {
            mech.MechEquipSystemComponent.EquipWeapon(cardInfo, equipId);
        }
        else
        {
            mech.MechEquipSystemComponent.M_Weapon = null;
        }

        yield return new WaitForSeconds(0.2f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void EquipShield(CardInfo_Equip cardInfo, int mechId, int equipId)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_EquipShield(cardInfo, mechId, equipId), "Co_EquipShield");
    }

    IEnumerator Co_EquipShield(CardInfo_Equip cardInfo, int mechId, int equipId)
    {
        ModuleMech mech = GetMech(mechId);
        if (cardInfo != null)
        {
            mech.MechEquipSystemComponent.EquipShield(cardInfo, equipId);
        }
        else
        {
            mech.MechEquipSystemComponent.M_Shield = null;
        }

        yield return new WaitForSeconds(0.2f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void EquipPack(CardInfo_Equip cardInfo, int mechId, int equipId)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_EquipPack(cardInfo, mechId, equipId), "Co_EquipPack");
    }

    IEnumerator Co_EquipPack(CardInfo_Equip cardInfo, int mechId, int equipId)
    {
        ModuleMech mech = GetMech(mechId);
        if (cardInfo != null)
        {
            mech.MechEquipSystemComponent.EquipPack(cardInfo, equipId);
        }
        else
        {
            mech.MechEquipSystemComponent.M_Pack = null;
        }

        yield return new WaitForSeconds(0.2f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void EquipMA(CardInfo_Equip cardInfo, int mechId, int equipId)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_EquipMA(cardInfo, mechId, equipId), "Co_EquipMA");
    }

    IEnumerator Co_EquipMA(CardInfo_Equip cardInfo, int mechId, int equipId)
    {
        ModuleMech mech = GetMech(mechId);
        if (cardInfo != null)
        {
            mech.MechEquipSystemComponent.EquipMA(cardInfo, equipId);
        }
        else
        {
            mech.MechEquipSystemComponent.M_MA = null;
        }

        yield return new WaitForSeconds(0.2f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void DamageOneMech(int targetMechId, int value)
    {
        GetMech(targetMechId).BeAttacked(value);
    }

    #region Utils

    public ModuleMech GetMech(int mechId)
    {
        foreach (ModuleMech moduleMech in Mechs)
        {
            if (moduleMech.M_MechID == mechId)
            {
                return moduleMech;
            }
        }

        foreach (ModuleMech moduleMech in addPrePassMechQueue) //预加载的机甲也要遍历一遍
        {
            if (moduleMech.M_MechID == mechId)
            {
                return moduleMech;
            }
        }

        return null;
    }

    public ModuleBase GetEquip(int mechId, int equipId)
    {
        foreach (ModuleMech moduleMech in Mechs)
        {
            if (moduleMech.M_MechID == mechId)
            {
                if (moduleMech.MechEquipSystemComponent.M_Weapon && moduleMech.MechEquipSystemComponent.M_Weapon.M_EquipID == equipId) return moduleMech.MechEquipSystemComponent.M_Weapon;
                if (moduleMech.MechEquipSystemComponent.M_Shield && moduleMech.MechEquipSystemComponent.M_Shield.M_EquipID == equipId) return moduleMech.MechEquipSystemComponent.M_Shield;
                if (moduleMech.MechEquipSystemComponent.M_Pack && moduleMech.MechEquipSystemComponent.M_Pack.M_EquipID == equipId) return moduleMech.MechEquipSystemComponent.M_Pack;
                if (moduleMech.MechEquipSystemComponent.M_MA && moduleMech.MechEquipSystemComponent.M_MA.M_EquipID == equipId) return moduleMech.MechEquipSystemComponent.M_MA;
            }
        }

        foreach (ModuleMech moduleMech in addPrePassMechQueue) //预加载的机甲也要遍历一遍
        {
            if (moduleMech.M_MechID == mechId)
            {
                if (moduleMech.MechEquipSystemComponent.M_Weapon && moduleMech.MechEquipSystemComponent.M_Weapon.M_EquipID == equipId) return moduleMech.MechEquipSystemComponent.M_Weapon;
                if (moduleMech.MechEquipSystemComponent.M_Shield && moduleMech.MechEquipSystemComponent.M_Shield.M_EquipID == equipId) return moduleMech.MechEquipSystemComponent.M_Shield;
                if (moduleMech.MechEquipSystemComponent.M_Pack && moduleMech.MechEquipSystemComponent.M_Pack.M_EquipID == equipId) return moduleMech.MechEquipSystemComponent.M_Pack;
                if (moduleMech.MechEquipSystemComponent.M_MA && moduleMech.MechEquipSystemComponent.M_MA.M_EquipID == equipId) return moduleMech.MechEquipSystemComponent.M_MA;
            }
        }

        return null;
    }

    public int GetMechPlaceIndex(ModuleMech moduleMech)
    {
        return Mechs.IndexOf(moduleMech);
    }

    #endregion

    #region GameProcess

    internal void BeginRound()
    {
        foreach (ModuleMech mr in Mechs) mr.OnBeginRound();
    }

    internal void EndRound()
    {
        foreach (ModuleMech mr in Mechs) mr.OnEndRound();
    }

    #endregion
}