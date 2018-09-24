using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGroundManager : MonoBehaviour
{
    private int retinueCount;

    public int RetinueCount //接到增加随从协议就更新数量（实体后面才生成）
    {
        get { return retinueCount; }
        set
        {
            retinueCount = value;
            BattleGroundIsFull = retinueCount == GamePlaySettings.MaxRetinueNumber;
            BattleGroundIsEmpty = retinueCount == 0;
        }
    }

    private int heroCount;

    public int HeroCount //接到增加随从协议就更新数量（实体后面才生成）
    {
        get { return heroCount; }
        set
        {
            heroCount = value;
            HerosIsEmpty = heroCount == 0;
        }
    }

    private int soldierCount;

    public int SoldierCount //接到增加随从协议就更新数量（实体后面才生成）
    {
        get { return soldierCount; }
        set
        {
            soldierCount = value;
            SoldiersIsEmpty = soldierCount == 0;
        }
    }

    public bool BattleGroundIsFull;
    public bool BattleGroundIsEmpty;
    public bool HerosIsEmpty;
    public bool SoldiersIsEmpty;
    private Vector3 _defaultRetinuePosition = Vector3.zero;

    internal ClientPlayer ClientPlayer;
    internal List<ModuleRetinue> Retinues = new List<ModuleRetinue>();
    internal List<ModuleRetinue> Heros = new List<ModuleRetinue>();
    internal List<ModuleRetinue> Soldiers = new List<ModuleRetinue>();

    public void ResetAll()
    {
        foreach (ModuleRetinue moduleRetinue in Retinues)
        {
            moduleRetinue.PoolRecycle();
        }

        RetinueCount = 0;
        HeroCount = 0;
        SoldierCount = 0;

        ClientPlayer = null;
        previewRetinuePlace = PREVIEW_RETINUE_PLACES_NO_PREVIEW_RETINUE_NOW;
        Retinues.Clear();
        Heros.Clear();
        Soldiers.Clear();
        RetinueCount = 0;
        HeroCount = 0;
        SoldierCount = 0;
        RemoveRetinues.Clear();
        addPrePassRetinueQueue.Clear();
        clientRetinueTempId = 0;
        relatedSlots.Clear();
        if (currentSummonPreviewRetinueCard) currentSummonPreviewRetinueCard.PoolRecycle();
        if (CurrentSummonPreviewRetinue) CurrentSummonPreviewRetinue.PoolRecycle();
    }

    #region 位置计算

    internal int ComputePosition(Vector3 dragLastPosition)
    {
        int index = Mathf.RoundToInt(Mathf.Floor(dragLastPosition.x / GameManager.Instance.RetinueInterval - (Retinues.Count + 1) % 2 * 0.5f) + (Retinues.Count / 2 + 1));
        if (index < 0) index = 0;
        if (index >= Retinues.Count) index = Retinues.Count;
        return index;
    }

    internal int ComputePositionInAliveRetinues(Vector3 dragLastPosition)
    {
        int battleGroundIndex = ComputePosition(dragLastPosition);
        return GetIndexOfAliveRetinues(battleGroundIndex);
    }

    internal ModuleRetinue CheckRetinueOnPosition(Vector3 dragLastPosition)
    {
        int index = Mathf.RoundToInt(Mathf.Floor(dragLastPosition.x / GameManager.Instance.RetinueInterval - (Retinues.Count + 1) % 2 * 0.5f) + (Retinues.Count / 2 + 1));
        if (index < 0 || index >= Retinues.Count)
            return null;
        return Retinues[index];
    }

    #endregion

    #region 召唤和移除

    public ModuleRetinue AddRetinue_PrePass(CardInfo_Retinue retinueCardInfo, int retinueId, int clientRetinueTempId)
    {
        if (ClientPlayer == null) return null;
        if (previewRetinuePlace != PREVIEW_RETINUE_PLACES_NO_PREVIEW_RETINUE_NOW)
        {
            previewRetinuePlace = PREVIEW_RETINUE_PLACES_NO_PREVIEW_RETINUE_NOW;
        }

        bool isSummonedBeforeByPreview = false;
        if (clientRetinueTempId >= 0)
        {
            foreach (ModuleRetinue moduleRetinue in Retinues)
            {
                if (moduleRetinue.M_ClientTempRetinueID == clientRetinueTempId) //匹配
                {
                    moduleRetinue.M_RetinueID = retinueId; //赋予正常ID
                    moduleRetinue.M_ClientTempRetinueID = Const.CLIENT_TEMP_RETINUE_ID_NORMAL; //恢复普通
                    isSummonedBeforeByPreview = true;
                    break;
                }
            }
        }

        if (!isSummonedBeforeByPreview)
        {
            ModuleRetinue retinue = GameObjectPoolManager.Instance.Pool_ModuleRetinuePool.AllocateGameObject(transform).GetComponent<ModuleRetinue>();
            retinue.transform.position = GameObjectPoolManager.Instance.Pool_ModuleRetinuePool.transform.position;
            retinue.Initiate(retinueCardInfo, ClientPlayer);
            retinue.transform.Rotate(Vector3.up, 180);
            retinue.M_RetinueID = retinueId;
            addPrePassRetinueQueue.Enqueue(retinue);
            RetinueCount++;
            if (!retinueCardInfo.BaseInfo.IsSoldier)
            {
                HeroCount++;
            }
            else
            {
                SoldierCount++;
            }

            return retinue;
        }

        return null;
    }


    private Queue<ModuleRetinue> addPrePassRetinueQueue = new Queue<ModuleRetinue>();

    public void AddRetinue(int retinuePlaceIndex)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RefreshBattleGroundAnim(BattleEffectsManager.Instance.Effect_Main, retinuePlaceIndex), "Co_RefreshBattleGroundAnim");
    }

    public void RemoveRetinue(int retinueId)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RemoveRetinue(GetRetinue(retinueId)), "Co_RemoveRetinue");
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RefreshBattleGroundAnim(BattleEffectsManager.Instance.Effect_Main), "Co_RefreshBattleGroundAnim");
    }

    public List<ModuleRetinue> RemoveRetinues = new List<ModuleRetinue>(); //即将要被移除的随从名单

    public void RemoveRetinueTogatherAdd(int retinueId)
    {
        ModuleRetinue retinue = GetRetinue(retinueId);
        retinue.CannotAttackBecauseDie = true;
        RetinueCount--;
        if (!retinue.CardInfo.BaseInfo.IsSoldier)
        {
            HeroCount--;
        }
        else
        {
            SoldierCount--;
        }

        RemoveRetinues.Add(retinue);
    }

    public void RemoveRetinueTogather(List<int> removeRetinueList)
    {
        foreach (int rid in removeRetinueList)
        {
            ModuleRetinue retinue = GetRetinue(rid);
            if (retinue)
            {
                retinue.PoolRecycle();
                Retinues.Remove(retinue);
                RemoveRetinues.Remove(retinue);
                if (!retinue.CardInfo.BaseInfo.IsSoldier)
                {
                    Heros.Remove(retinue);
                }
                else
                {
                    Soldiers.Remove(retinue);
                }

                ClientLog.Instance.Print("remove:" + retinue.M_RetinueID);
            }
        }
    }

    public void RemoveRetinueTogatherEnd()
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RefreshBattleGroundAnim(BattleEffectsManager.Instance.Effect_Main), "Co_RefreshBattleGroundAnim");
    }

    IEnumerator Co_RemoveRetinue(ModuleRetinue retinue)
    {
        retinue.PoolRecycle();
        Retinues.Remove(retinue);
        RetinueCount--;
        if (!retinue.CardInfo.BaseInfo.IsSoldier)
        {
            Heros.Remove(retinue);
            HeroCount--;
        }
        else
        {
            Soldiers.Remove(retinue);
            SoldierCount--;
        }

        PrintRetinueInfos();
        yield return null;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    #endregion

    #region 出牌召唤预览

    private int previewRetinuePlace;

    private const int PREVIEW_RETINUE_PLACES_NO_PREVIEW_RETINUE_NOW = -1; //无预览召唤随从

    public void AddRetinuePreview(int placeIndex)
    {
        if (Retinues.Count == 0) return;
        if (previewRetinuePlace == PREVIEW_RETINUE_PLACES_NO_PREVIEW_RETINUE_NOW || previewRetinuePlace != placeIndex)
        {
            previewRetinuePlace = placeIndex;
            BattleEffectsManager.Instance.Effect_RefreshBattleGroundOnAddRetinue.EffectsShow(Co_RefreshBattleGroundAnim(BattleEffectsManager.Instance.Effect_RefreshBattleGroundOnAddRetinue), "Co_RefreshBattleGroundAnim");
        }
    }


    public void RemoveRetinuePreview()
    {
        if (previewRetinuePlace != PREVIEW_RETINUE_PLACES_NO_PREVIEW_RETINUE_NOW)
        {
            previewRetinuePlace = PREVIEW_RETINUE_PLACES_NO_PREVIEW_RETINUE_NOW;
            BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_RefreshBattleGroundAnim(BattleEffectsManager.Instance.Effect_Main), "Co_RefreshBattleGroundAnim");
        }
    }

    #endregion

    #region 能指定目标的随从的预召唤

    private static int clientRetinueTempId = 0;

    public static int GenerateClientRetinueTempId()
    {
        return clientRetinueTempId++;
    }

    public delegate void SummonRetinueTarget(int targetRetinueId, bool isClientRetinueTempId = false);

    private CardRetinue currentSummonPreviewRetinueCard;
    public ModuleRetinue CurrentSummonPreviewRetinue;

    public void SummonRetinuePreview(CardRetinue retinueCard, int retinuePlaceIndex, TargetSideEffect.TargetRange targetRange) //用于具有指定目标的副作用的随从的召唤预览、显示指定箭头
    {
        currentSummonPreviewRetinueCard = retinueCard;
        ModuleRetinue retinue = AddRetinue_PrePass((CardInfo_Retinue) retinueCard.CardInfo, (int) ModuleRetinue.RetinueID.Empty, (int) Const.CLIENT_TEMP_RETINUE_ID_NORMAL);
        CurrentSummonPreviewRetinue = retinue;
        AddRetinue(retinuePlaceIndex);
        DragManager.Instance.SummonRetinueTargetHandler = SummonRetinueTargetConfirm;
        DragManager.Instance.StartArrowAiming(retinue, targetRange);
    }

    public void SummonRetinueTargetConfirm(int targetRetinueId, bool isClientRetinueTempId)
    {
        if (targetRetinueId == DragManager.TARGET_SELECT_NONE) //未选择目标
        {
            RemoveRetinue((int) ModuleRetinue.RetinueID.Empty);
            ClientPlayer.MyHandManager.CancelSummonRetinuePreview();
        }
        else
        {
            StartCoroutine(Co_RetrySummonRequest(CurrentSummonPreviewRetinue, currentSummonPreviewRetinueCard.M_CardInstanceId, targetRetinueId, isClientRetinueTempId));
        }
    }

    IEnumerator Co_RetrySummonRequest(ModuleRetinue retinue, int cardInstanceId, int targetRetinueId, bool isClientRetinueTempId)
    {
        while (true)
        {
            int battleGroundIndex = GetIndexOfAliveRetinues(retinue); //确定的时候再获取位置信息（召唤的过程中可能会有协议没有跑完，会有随从生成）

            if (battleGroundIndex != -1)
            {
                retinue.M_ClientTempRetinueID = GenerateClientRetinueTempId();
                SummonRetinueRequest request = new SummonRetinueRequest(Client.Instance.Proxy.ClientId, cardInstanceId, battleGroundIndex, new MyCardGameCommon.Vector3(0, 0, 0), targetRetinueId, isClientRetinueTempId, retinue.M_ClientTempRetinueID);
                Client.Instance.Proxy.SendMessage(request);
                break;
            }

            yield return null;
        }

        yield return null;
    }

    private int GetIndexOfAliveRetinues(ModuleRetinue retinue)
    {
        int battleGroundIndex = Retinues.IndexOf(retinue);
        return GetIndexOfAliveRetinues(battleGroundIndex);
    }

    private int GetIndexOfAliveRetinues(int battleGroundIndex)
    {
        //去除掉已经死亡但还没移除战场的随从（避免服务器指针错误）
        int countDieRetinue = 0;
        for (int i = 0; i < battleGroundIndex; i++)
        {
            if (RemoveRetinues.Contains(Retinues[i]))
            {
                countDieRetinue++;
            }
        }

        int aliveIndex = battleGroundIndex - countDieRetinue;
        return aliveIndex;
    }

    #endregion

    private enum retinuePlaceIndex
    {
        NoNewRetinue = -1,
    }

    IEnumerator Co_RefreshBattleGroundAnim(BattleEffectsManager.Effects myParentEffects, int retinuePlaceIndex = (int) retinuePlaceIndex.NoNewRetinue)
    {
        bool isAddRetinue = retinuePlaceIndex != (int) BattleGroundManager.retinuePlaceIndex.NoNewRetinue;
        if (isAddRetinue) //新增随从
        {
            ModuleRetinue retinue = addPrePassRetinueQueue.Dequeue();

            Retinues.Insert(retinuePlaceIndex, retinue);
            if (retinue.CardInfo.BaseInfo.IsSoldier)
            {
                Soldiers.Add(retinue);
            }
            else
            {
                Heros.Add(retinue);
            }

            retinue.OnSummon();
            retinue.transform.localPosition = _defaultRetinuePosition;
            retinue.transform.transform.Translate(Vector3.left * (Retinues.IndexOf(retinue) - Retinues.Count / 2.0f + 0.5f) * GameManager.Instance.RetinueInterval, Space.Self);
            //PrintRetinueInfos();
        }

        float duration = 0.05f;
        float tick = 0;

        Vector3[] targetPos = new Vector3[Retinues.Count];

        int actualPlaceCount = previewRetinuePlace == PREVIEW_RETINUE_PLACES_NO_PREVIEW_RETINUE_NOW ? Retinues.Count : Retinues.Count + 1;

        List<ModuleRetinue> movingRetinues = new List<ModuleRetinue>();

        for (int i = 0; i < Retinues.Count; i++)
        {
            movingRetinues.Add(Retinues[i]);

            int actualPlace = i;
            if (previewRetinuePlace != PREVIEW_RETINUE_PLACES_NO_PREVIEW_RETINUE_NOW && i >= previewRetinuePlace)
            {
                actualPlace += 1;
            }

            Vector3 ori = Retinues[i].transform.position;
            Vector3 offset = Vector3.left * (actualPlace - actualPlaceCount / 2.0f + 0.5f) * GameManager.Instance.RetinueInterval;

            Retinues[i].transform.localPosition = _defaultRetinuePosition;
            Retinues[i].transform.Translate(offset, Space.Self);

            targetPos[i] = Retinues[i].transform.position;
            Retinues[i].transform.position = ori;

            Hashtable args = new Hashtable();
            args.Add("position", targetPos[i]);
            args.Add("time", duration);
            args.Add("easeType", iTween.EaseType.linear);
            iTween.MoveTo(Retinues[i].gameObject, args);
        }

        yield return new WaitForSeconds(duration);
        if (isAddRetinue && DragManager.Instance.IsSummonPreview)
        {
            DragManager.Instance.IsArrowShowBegin = true;
        }

        yield return null;

        myParentEffects.EffectEnd();
    }

    #region 装备牌相关操作会显示提示性Slot边框

    List<Slot> relatedSlots = new List<Slot>();

    private IEnumerator currentShowSlotBloom;

    public void ShowTipSlotBlooms(SlotTypes slotType)
    {
        StopShowSlotBloom();
        foreach (ModuleRetinue retinue in Retinues)
        {
            if (retinue.Slot1.MSlotTypes == slotType)
            {
                relatedSlots.Add(retinue.Slot1);
            }

            if (retinue.Slot2.MSlotTypes == slotType)
            {
                relatedSlots.Add(retinue.Slot2);
            }

            if (retinue.Slot3.MSlotTypes == slotType)
            {
                relatedSlots.Add(retinue.Slot3);
            }

            if (retinue.Slot4.MSlotTypes == slotType)
            {
                relatedSlots.Add(retinue.Slot4);
            }
        }

        currentShowSlotBloom = Co_ShowSlotBloom();
        BattleEffectsManager.Instance.Effect_TipSlotBloom.EffectsShow(currentShowSlotBloom, "Co_ShowSlotBloom");
    }

    IEnumerator Co_ShowSlotBloom()
    {
        while (true)
        {
            foreach (Slot sa in relatedSlots)
            {
                sa.ShowHoverGO();
            }

            yield return new WaitForSeconds(0.4f);
            foreach (Slot sa in relatedSlots)
            {
                sa.HideHoverShowGO();
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
            sa.HideHoverShowGO();
        }

        relatedSlots.Clear();
    }

    #endregion

    public void EquipWeapon(CardInfo_Equip cardInfo, int retinueId, int equipId)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_EquipWeapon(cardInfo, retinueId, equipId), "Co_EquipWeapon");
    }

    IEnumerator Co_EquipWeapon(CardInfo_Equip cardInfo, int retinueId, int equipId)
    {
        ModuleRetinue retinue = GetRetinue(retinueId);
        if (cardInfo != null)
        {
            ModuleWeapon newModueWeapon = GameObjectPoolManager.Instance.Pool_ModuleWeaponPool.AllocateGameObject(retinue.transform).GetComponent<ModuleWeapon>();
            newModueWeapon.M_ModuleRetinue = retinue;
            newModueWeapon.Initiate(cardInfo, ClientPlayer);
            newModueWeapon.M_EquipID = equipId;
            retinue.M_Weapon = newModueWeapon;
        }
        else
        {
            retinue.M_Weapon = null;
        }

        yield return new WaitForSeconds(0.2f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void EquipShield(CardInfo_Equip cardInfo, int retinueId, int equipId)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_EquipShield(cardInfo, retinueId, equipId), "Co_EquipShield");
    }

    IEnumerator Co_EquipShield(CardInfo_Equip cardInfo, int retinueId, int equipId)
    {
        ModuleRetinue retinue = GetRetinue(retinueId);
        if (cardInfo != null)
        {
            ModuleShield newModuleShield = GameObjectPoolManager.Instance.Pool_ModuleShieldPool.AllocateGameObject(retinue.transform).GetComponent<ModuleShield>();
            newModuleShield.M_ModuleRetinue = retinue;
            newModuleShield.Initiate(cardInfo, ClientPlayer);
            newModuleShield.M_EquipID = equipId;
            retinue.M_Shield = newModuleShield;
        }
        else
        {
            retinue.M_Shield = null;
        }

        yield return new WaitForSeconds(0.2f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void EquipPack(CardInfo_Equip cardInfo, int retinueId, int equipId)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_EquipPack(cardInfo, retinueId, equipId), "Co_EquipPack");
    }

    IEnumerator Co_EquipPack(CardInfo_Equip cardInfo, int retinueId, int equipId)
    {
        ModuleRetinue retinue = GetRetinue(retinueId);
        if (cardInfo != null)
        {
            ModulePack newModulePack = GameObjectPoolManager.Instance.Pool_ModulePackPool.AllocateGameObject(retinue.transform).GetComponent<ModulePack>();
            newModulePack.M_ModuleRetinue = retinue;
            newModulePack.Initiate(cardInfo, ClientPlayer);
            newModulePack.M_EquipID = equipId;
            retinue.M_Pack = newModulePack;
        }
        else
        {
            retinue.M_Pack = null;
        }

        yield return new WaitForSeconds(0.2f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void EquipMA(CardInfo_Equip cardInfo, int retinueId, int equipId)
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_EquipMA(cardInfo, retinueId, equipId), "Co_EquipMA");
    }

    IEnumerator Co_EquipMA(CardInfo_Equip cardInfo, int retinueId, int equipId)
    {
        ModuleRetinue retinue = GetRetinue(retinueId);
        if (cardInfo != null)
        {
            ModuleMA newModuleMA = GameObjectPoolManager.Instance.Pool_ModuleMAPool.AllocateGameObject(retinue.transform).GetComponent<ModuleMA>();
            newModuleMA.M_ModuleRetinue = retinue;
            newModuleMA.Initiate(cardInfo, ClientPlayer);
            newModuleMA.M_EquipID = equipId;
            retinue.M_MA = newModuleMA;
        }
        else
        {
            retinue.M_MA = null;
        }

        yield return new WaitForSeconds(0.2f);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }

    public void DamageOneRetinue(int targetRetinueId, int value)
    {
        GetRetinue(targetRetinueId).BeAttacked(value);
    }

    #region Utils

    public ModuleRetinue GetRetinue(int retinueId)
    {
        foreach (ModuleRetinue moduleRetinue in Retinues)
        {
            if (moduleRetinue.M_RetinueID == retinueId)
            {
                return moduleRetinue;
            }
        }

        foreach (ModuleRetinue moduleRetinue in addPrePassRetinueQueue) //预加载的随从也要遍历一遍
        {
            if (moduleRetinue.M_RetinueID == retinueId)
            {
                return moduleRetinue;
            }
        }

        return null;
    }

    public ModuleBase GetEquip(int retinueId, int equipId)
    {
        foreach (ModuleRetinue moduleRetinue in Retinues)
        {
            if (moduleRetinue.M_RetinueID == retinueId)
            {
                if (moduleRetinue.M_Weapon && moduleRetinue.M_Weapon.M_EquipID == equipId) return moduleRetinue.M_Weapon;
                if (moduleRetinue.M_Shield && moduleRetinue.M_Shield.M_EquipID == equipId) return moduleRetinue.M_Shield;
                if (moduleRetinue.M_Pack && moduleRetinue.M_Pack.M_EquipID == equipId) return moduleRetinue.M_Pack;
                if (moduleRetinue.M_MA && moduleRetinue.M_MA.M_EquipID == equipId) return moduleRetinue.M_MA;
            }
        }

        foreach (ModuleRetinue moduleRetinue in addPrePassRetinueQueue) //预加载的随从也要遍历一遍
        {
            if (moduleRetinue.M_RetinueID == retinueId)
            {
                if (moduleRetinue.M_Weapon && moduleRetinue.M_Weapon.M_EquipID == equipId) return moduleRetinue.M_Weapon;
                if (moduleRetinue.M_Shield && moduleRetinue.M_Shield.M_EquipID == equipId) return moduleRetinue.M_Shield;
                if (moduleRetinue.M_Pack && moduleRetinue.M_Pack.M_EquipID == equipId) return moduleRetinue.M_Pack;
                if (moduleRetinue.M_MA && moduleRetinue.M_MA.M_EquipID == equipId) return moduleRetinue.M_MA;
            }
        }

        return null;
    }


    public void PrintRetinueInfos()
    {
        string log = "BattleGroundInfo: [ClientID]" + ClientPlayer.ClientId + " [Username]" + ClientPlayer.Username;
        foreach (ModuleRetinue retinue in Retinues)
        {
            log += " [RID]" + retinue.M_RetinueID + " [Name]" + retinue.CardInfo.BaseInfo.CardName;
        }

        ClientLog.Instance.Print(log);
    }

    public int GetRetinuePlaceIndex(ModuleRetinue moduleRetinue)
    {
        return Retinues.IndexOf(moduleRetinue);
    }

    #endregion

    #region GameProcess

    internal void BeginRound()
    {
        foreach (ModuleRetinue mr in Retinues) mr.OnBeginRound();
    }

    internal void EndRound()
    {
        foreach (ModuleRetinue mr in Retinues) mr.OnEndRound();
    }

    #endregion
}