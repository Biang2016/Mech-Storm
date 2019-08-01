using System;
using System.Collections.Generic;
using System.Linq;

public class EventManager
{
    public EventManager()
    {
        // Event Manager is arranged and triggered by (enum)TriggerTime.
        foreach (SideEffectExecute.TriggerTime tt in Enum.GetValues(typeof(SideEffectExecute.TriggerTime)))
        {
            Events.Add(tt, new Dictionary<int, SideEffectExecute>());
            RemoveEvents.Add(tt, new Dictionary<int, SideEffectExecute>());
        }
    }

    /// <summary>
    /// All events are stored in this Dict by (key)TriggerTime
    /// </summary>
    private SortedDictionary<SideEffectExecute.TriggerTime, Dictionary<int, SideEffectExecute>> Events = new SortedDictionary<SideEffectExecute.TriggerTime, Dictionary<int, SideEffectExecute>>();

    /// <summary>
    /// All useless events are stored in this Dict for later remove by (key)TriggerTime
    /// </summary>
    private SortedDictionary<SideEffectExecute.TriggerTime, Dictionary<int, SideEffectExecute>> RemoveEvents = new SortedDictionary<SideEffectExecute.TriggerTime, Dictionary<int, SideEffectExecute>>();

    /// <summary>
    /// Cards, equipments, buffs register their sideeffects
    /// </summary>
    /// <param name="sideEffectBundle"></param>
    public void RegisterEvent(SideEffectBundle sideEffectBundle)
    {
        foreach (SideEffectExecute see in sideEffectBundle.SideEffectExecutes)
        {
            RegisterEvent(see);
        }
    }

    /// <summary>
    /// Register single SideEffectExecute
    /// </summary>
    /// <param name="see"></param>
    public void RegisterEvent(SideEffectExecute see)
    {
        if (see.M_ExecuteSetting.TriggerTime != SideEffectExecute.TriggerTime.None && see.M_ExecuteSetting.TriggerRange != SideEffectExecute.TriggerRange.None)
        {
            Events[see.M_ExecuteSetting.TriggerTime].Add(see.ID, see);
            RemoveEvents[see.M_ExecuteSetting.RemoveTriggerTime].Add(see.ID, see);
        }
    }

    /// <summary>
    /// If cards, buffs, equipments are removed from battle, just call this method to UnRegister all sideeffects of them
    /// </summary>
    /// <param name="sideEffects"></param>
    public void UnRegisterEvent(SideEffectBundle sideEffects)
    {
        foreach (SideEffectExecute see in sideEffects.SideEffectExecutes)
        {
            UnRegisterEvent(see);
        }
    }

    public void UnRegisterEvent(SideEffectExecute see)
    {
        Dictionary<int, SideEffectExecute> sees = Events[see.M_ExecuteSetting.TriggerTime];
        if (sees.ContainsKey(see.ID)) sees.Remove(see.ID);

        Dictionary<int, SideEffectExecute> sees_remove = RemoveEvents[see.M_ExecuteSetting.TriggerTime];
        if (sees_remove.ContainsKey(see.ID)) sees_remove.Remove(see.ID);
    }

    /// <summary>
    /// Clear all events in EventManager. For game ending or restarting.
    /// </summary>
    public void ClearAllEvents()
    {
        foreach (SideEffectExecute.TriggerTime tt in Enum.GetValues(typeof(SideEffectExecute.TriggerTime)))
        {
            Events[tt] = new Dictionary<int, SideEffectExecute>();
            RemoveEvents[tt] = new Dictionary<int, SideEffectExecute>();
        }

        ObsoleteSEEs.Clear();
    }

    /// <summary>
    /// Sometimes several events are triggered at the same time (e.g. OnHeroAttack and OnAttack are triggered at the same time because OnAttack has a bigger range)
    /// So we need a TriggerTimeQueue to manage these.
    /// </summary>
    Queue<InvokeInfo> InvokeTriggerTimeQueue = new Queue<InvokeInfo>();

    struct InvokeInfo
    {
        public SideEffectExecute.TriggerTime TriggerTime;

        /// <summary>
        /// ExecutorInfo reflects as more information as possible of the executor of this invoke.
        /// </summary>
        public ExecutorInfo ExecutorInfo;

        public InvokeInfo(SideEffectExecute.TriggerTime triggerTime, ExecutorInfo executorInfo)
        {
            TriggerTime = triggerTime;
            ExecutorInfo = executorInfo;
        }
    }

    /// <summary>
    /// When something happens in game, invoke events by TriggerTime enum.
    /// Invoker info can be found in executorInfo.
    /// TriggerTime enum is Flag. Master trigger can be triggered by Sub trigger -> e.g. OnHeroInjured also triggers OnMechInjured
    /// This method is often used in game logic.
    /// (e.g. BattlePlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnSoldierKill, new ExecutorInfo(BattlePlayer.ClientId, mechId: M_MechID, targetMechId: targetMech.M_MechID));)
    /// </summary>
    /// <param name="tt"></param>
    /// <param name="executorInfo"></param>
    public void Invoke(SideEffectExecute.TriggerTime tt, ExecutorInfo executorInfo)
    {
        foreach (SideEffectExecute.TriggerTime triggerTime in Enum.GetValues(typeof(SideEffectExecute.TriggerTime)))
        {
            if ((tt & triggerTime) == tt) // this is a Flag Enum compare 
            {
                InvokeTriggerTimeQueue.Enqueue(new InvokeInfo(triggerTime, executorInfo)); //all TriggerTimes enqueue
            }
        }

        while (InvokeTriggerTimeQueue.Count > 0) //Dequeue every trigger time and invoke.
        {
            InvokeInfo invokeInfo = InvokeTriggerTimeQueue.Dequeue();
            InvokeCore(invokeInfo);
        }

        RemoveAllUselessSEEs(); // if some mechs are dead or cards are removed, their SideEffectExecute would be removed from EventManager 
        OnEventInvokeEndHandler(); //Ready to send data to client ends.
    }

    private void InvokeCore(InvokeInfo invokeInfo)
    {
        SideEffectExecute.TriggerTime tt = invokeInfo.TriggerTime;
        ExecutorInfo executorInfo = invokeInfo.ExecutorInfo;

        Dictionary<int, SideEffectExecute> seeDict = Events[tt];
        SideEffectExecute[] sees = seeDict.Values.ToArray();
        for (int i = 0; i < sees.Length; i++)
        {
            SideEffectExecute see = sees[i];
            if (ObsoleteSEEs.ContainsKey(see.ID)) continue; //To prevent executed side effects from being executed again.
            if (seeDict.ContainsKey(see.ID))
            {
                foreach (SideEffectBase se in see.SideEffectBases)
                {
                    bool isTrigger = IsExecuteTrigger(executorInfo, se.M_ExecutorInfo, see.M_ExecuteSetting.TriggerRange); //To check out if this event invokes any side effect.
                    if (isTrigger) Trigger(see, executorInfo, tt, see.M_ExecuteSetting.TriggerRange); // invoke main trigger method. 
                }
            }
        }

        Invoke_RemoveSEE(tt, executorInfo); //Remove executed side effects with zero time left.
    }

    public void Invoke_RemoveSEE(SideEffectExecute.TriggerTime tt, ExecutorInfo executorInfo)
    {
        Dictionary<int, SideEffectExecute> seeDict = RemoveEvents[tt];
        SideEffectExecute[] sees = seeDict.Values.ToArray();
        for (int i = 0; i < sees.Length; i++)
        {
            SideEffectExecute see = sees[i];
            if (ObsoleteSEEs.ContainsKey(see.ID)) continue; //To prevent removed side effects from being removed again.
            if (seeDict.ContainsKey(see.ID))
            {
                foreach (SideEffectBase se in see.SideEffectBases)
                {
                    bool isTrigger = IsExecuteTrigger(executorInfo, se.M_ExecutorInfo, see.M_ExecuteSetting.RemoveTriggerRange);
                    if (isTrigger) Trigger_TryRemove(see); // invoke main trigger_remove method. (some side effects like buffs have a remove_time attribute. e.g. Remove this buff after 3 turns)
                }
            }
        }

        RemoveAllUselessSEEs();
    }

    private Dictionary<int, SideEffectExecute> ObsoleteSEEs = new Dictionary<int, SideEffectExecute>();

    private void RemoveAllUselessSEEs()
    {
        foreach (KeyValuePair<int, SideEffectExecute> kv in ObsoleteSEEs)
        {
            Dictionary<int, SideEffectExecute> event_sees = Events[kv.Value.M_ExecuteSetting.TriggerTime];
            if (event_sees.ContainsKey(kv.Key)) event_sees.Remove(kv.Key);

            Dictionary<int, SideEffectExecute> removeEvent_sees = RemoveEvents[kv.Value.M_ExecuteSetting.TriggerTime];
            if (removeEvent_sees.ContainsKey(kv.Key)) removeEvent_sees.Remove(kv.Key);

            foreach (SideEffectBase se in kv.Value.SideEffectBases)
            {
                if (se.M_ExecutorInfo.IsPlayerBuff) //PlayerBuff
                {
                    if (se is PlayerBuffSideEffects buff)
                    {
                        OnEventPlayerBuffRemoveHandler(kv.Value, buff);
                    }
                }
            }
        }

        ObsoleteSEEs.Clear();
    }

    /// <summary>
    /// This is an important method to check is this Event valid to trigger a series of corresponding sideeffects.
    /// Compare executor's clientID, cardID or other values with those sideeffects recorded in EventManager. If compared successfully, then trigger.
    /// </summary>
    /// <param name="executorInfo"></param>
    /// <param name="se_ExecutorInfo"></param>
    /// <param name="tr">TriggerRange, the key parameter in this method, used to compare executorInfos between events and sideeffects recorded in EventManager</param>
    /// <returns></returns>
    private static bool IsExecuteTrigger(ExecutorInfo executorInfo, ExecutorInfo se_ExecutorInfo, SideEffectExecute.TriggerRange tr)
    {
        bool isTrigger = false;
        switch (tr)
        {
            case SideEffectExecute.TriggerRange.SelfPlayer:
                if (executorInfo.ClientId == se_ExecutorInfo.ClientId) isTrigger = true;
                break;
            case SideEffectExecute.TriggerRange.EnemyPlayer:
                if (executorInfo.ClientId != se_ExecutorInfo.ClientId) isTrigger = true;
                break;
            case SideEffectExecute.TriggerRange.OnePlayer:
                isTrigger = true;
                break;
            case SideEffectExecute.TriggerRange.One:
                isTrigger = true;
                break;
            case SideEffectExecute.TriggerRange.SelfAnother:
                if (executorInfo.ClientId == se_ExecutorInfo.ClientId &&
                    ((se_ExecutorInfo.MechId != ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.MechId != executorInfo.MechId) ||
                     (se_ExecutorInfo.CardInstanceId != ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.CardInstanceId != executorInfo.CardInstanceId)))
                    isTrigger = true;
                break;
            case SideEffectExecute.TriggerRange.Another:
                if ((se_ExecutorInfo.MechId != ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.MechId != executorInfo.MechId) ||
                    (se_ExecutorInfo.CardInstanceId != ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.CardInstanceId != executorInfo.CardInstanceId))
                    isTrigger = true;
                break;
            case SideEffectExecute.TriggerRange.AttachedMech:
                if (se_ExecutorInfo.MechId != ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.MechId == executorInfo.MechId)
                    isTrigger = true;
                break;
            case SideEffectExecute.TriggerRange.AttachedEquip:
                if (se_ExecutorInfo.MechId != ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.MechId == executorInfo.MechId)
                    isTrigger = true;
                break;
            case SideEffectExecute.TriggerRange.Self:
                if ((se_ExecutorInfo.MechId != ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.MechId == executorInfo.MechId) ||
                    (se_ExecutorInfo.CardInstanceId != ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.CardInstanceId == executorInfo.CardInstanceId) ||
                    (se_ExecutorInfo.EquipId != ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.EquipId == executorInfo.EquipId))
                    isTrigger = true;
                break;
        }

        return isTrigger;
    }

    /// <summary>
    /// Because during the process of some effects trigger, new events would happen. (e.g. trigger a damage effect and then a mech is injured and dead, then trigger a OnMechInjured and OnMechDie event)
    /// So we need a Stack data structure to manage where are we in this trigger tree.
    /// </summary>
    Stack<SideEffectExecute> InvokeStack = new Stack<SideEffectExecute>();

    private void Trigger(SideEffectExecute see, ExecutorInfo ei, SideEffectExecute.TriggerTime tt, SideEffectExecute.TriggerRange tr)
    {
        if (see.M_ExecuteSetting.TriggerDelayTimes > 0) //TriggerDelayTimes decreases and trigger the event when it's 0
        {
            see.M_ExecuteSetting.TriggerDelayTimes--;
            return;
        }
        else
        {
            if (see.M_ExecuteSetting.TriggerTimes > 0) //TriggerTimes decreases every time it triggers and stop when it's 0
            {
                //Trigger's trigger  -- which triggers when other events are being triggered.
                bool isTriggerTrigger = false;
                if (tt == SideEffectExecute.TriggerTime.OnTrigger) //Give sideeffect executing info to trigger's trigger for modifying.
                {
                    foreach (SideEffectBase se in see.SideEffectBases)
                    {
                        if (se is PlayerBuffSideEffects buffSEE)
                        {
                            foreach (SideEffectBase sub_se in buffSEE.Sub_SideEffect)
                            {
                                if (sub_se is ITrigger triggerSEE)
                                {
                                    triggerSEE.PeekSEE = InvokeStack.Peek();
                                    if (triggerSEE.IsTrigger(ei))
                                    {
                                        isTriggerTrigger = true;
                                    }
                                }
                            }
                        }
                    }
                }

                if (tt == SideEffectExecute.TriggerTime.OnTrigger && !isTriggerTrigger) return;
                //Trigger's trigger End

                see.M_ExecuteSetting.TriggerTimes--;
                ShowSideEffectTriggeredRequest request = new ShowSideEffectTriggeredRequest(see.SideEffectBases[0].M_ExecutorInfo, tt, tr); //Send request to client
                OnEventInvokeHandler(request);

                InvokeStack.Push(see);
                Invoke(SideEffectExecute.TriggerTime.OnTrigger, ei);
                foreach (SideEffectBase se in see.SideEffectBases)
                {
                    se.Execute(ei);
                }

                InvokeStack.Pop();
            }
            else if (see.M_ExecuteSetting.TriggerTimes == 0)
            {
                if (!ObsoleteSEEs.ContainsKey(see.ID))
                {
                    ObsoleteSEEs.Add(see.ID, see);
                }
            }
        }
    }

    private void Trigger_TryRemove(SideEffectExecute see)
    {
        if (see.M_ExecuteSetting.RemoveTriggerTimes > 0) //RemoveTriggerTimes decreases every time it triggers and removes when it's 0
        {
            see.M_ExecuteSetting.RemoveTriggerTimes--;
            foreach (SideEffectBase se in see.SideEffectBases)
            {
                if (se is PlayerBuffSideEffects buff_se)
                {
                    buff_se.M_SideEffectParam.SetParam_ConstInt("RemoveTriggerTimes", buff_se.M_SideEffectParam.GetParam_ConstInt("RemoveTriggerTimes") - 1);
                }

                if (see.M_ExecuteSetting.RemoveTriggerTimes == 0)
                {
                    ObsoleteSEEs.Add(see.ID, see);
                }
                else
                {
                    if (se is PlayerBuffSideEffects)
                    {
                        OnEventPlayerBuffUpdateHandler(see, false);
                    }
                }
            }
        }
    }

    public delegate void OnEventInvoke(ShowSideEffectTriggeredRequest request);

    public OnEventInvoke OnEventInvokeHandler;

    public delegate void OnEventInvokeEnd();

    public OnEventInvokeEnd OnEventInvokeEndHandler;

    public delegate void OnEventPlayerBuffUpdate(SideEffectExecute sideEffectExecute, bool isAdd);

    public OnEventPlayerBuffUpdate OnEventPlayerBuffUpdateHandler;

    public delegate void OnEventPlayerBuffRemove(SideEffectExecute sideEffectExecute, PlayerBuffSideEffects buff);

    public OnEventPlayerBuffRemove OnEventPlayerBuffRemoveHandler;
}