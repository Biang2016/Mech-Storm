using System;
using System.Collections.Generic;
using System.Linq;

public class EventManager
{
    public EventManager()
    {
        // Event Manager is arranged and triggered by (enum)TriggerTime.
        foreach (SideEffectBundle.TriggerTime tt in Enum.GetValues(typeof(SideEffectBundle.TriggerTime)))
        {
            Events.Add(tt, new Dictionary<int, SideEffectExecute>());
            RemoveEvents.Add(tt, new Dictionary<int, SideEffectExecute>());
        }
    }

    /// <summary>
    /// All events are stored in this Dict by (key)TriggerTime
    /// </summary>
    private SortedDictionary<SideEffectBundle.TriggerTime, Dictionary<int, SideEffectExecute>> Events = new SortedDictionary<SideEffectBundle.TriggerTime, Dictionary<int, SideEffectExecute>>();

    /// <summary>
    /// All useless events are stored in this Dict for later remove by (key)TriggerTime
    /// </summary>
    private SortedDictionary<SideEffectBundle.TriggerTime, Dictionary<int, SideEffectExecute>> RemoveEvents = new SortedDictionary<SideEffectBundle.TriggerTime, Dictionary<int, SideEffectExecute>>();

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
    /// <param name="sideEffectExecute"></param>
    public void RegisterEvent(SideEffectExecute sideEffectExecute)
    {
        if (sideEffectExecute.TriggerTime != SideEffectBundle.TriggerTime.None && sideEffectExecute.TriggerRange != SideEffectBundle.TriggerRange.None)
        {
            Events[sideEffectExecute.TriggerTime].Add(sideEffectExecute.ID, sideEffectExecute);
            RemoveEvents[sideEffectExecute.RemoveTriggerTime].Add(sideEffectExecute.ID, sideEffectExecute);
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
        Dictionary<int, SideEffectExecute> sees = Events[see.TriggerTime];
        if (sees.ContainsKey(see.ID)) sees.Remove(see.ID);

        Dictionary<int, SideEffectExecute> sees_remove = RemoveEvents[see.TriggerTime];
        if (sees_remove.ContainsKey(see.ID)) sees_remove.Remove(see.ID);
    }

    /// <summary>
    /// Clear all events in EventManager. For game ending or restarting.
    /// </summary>
    public void ClearAllEvents()
    {
        foreach (SideEffectBundle.TriggerTime tt in Enum.GetValues(typeof(SideEffectBundle.TriggerTime)))
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
        public SideEffectBundle.TriggerTime TriggerTime;
        /// <summary>
        /// ExecutorInfo reflects as more information as possible of the executor of this invoke.
        /// </summary>
        public SideEffectBase.ExecutorInfo ExecutorInfo;

        public InvokeInfo(SideEffectBundle.TriggerTime triggerTime, SideEffectBase.ExecutorInfo executorInfo)
        {
            TriggerTime = triggerTime;
            ExecutorInfo = executorInfo;
        }
    }

    /// <summary>
    /// When something happens in game, invoke events by TriggerTime enum.
    /// Invoker info can be found in executorInfo.
    /// TriggerTime enum is Flag. Master trigger can be triggered by Sub trigger -> e.g. OnHeroInjured also triggers OnRetinueInjured
    /// This method is often used in game logic.
    /// (e.g. ServerPlayer.MyGameManager.EventManager.Invoke(SideEffectBundle.TriggerTime.OnSoldierKill, new SideEffectBase.ExecutorInfo(ServerPlayer.ClientId, retinueId: M_RetinueID, targetRetinueId: targetRetinue.M_RetinueID));)
    /// </summary>
    /// <param name="tt"></param>
    /// <param name="executorInfo"></param>
    public void Invoke(SideEffectBundle.TriggerTime tt, SideEffectBase.ExecutorInfo executorInfo)
    {
        foreach (SideEffectBundle.TriggerTime triggerTime in Enum.GetValues(typeof(SideEffectBundle.TriggerTime)))
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
        SideEffectBundle.TriggerTime tt = invokeInfo.TriggerTime;
        SideEffectBase.ExecutorInfo executorInfo = invokeInfo.ExecutorInfo;

        Dictionary<int, SideEffectExecute> seeDict = Events[tt];
        SideEffectExecute[] sees = seeDict.Values.ToArray();
        for (int i = 0; i < sees.Length; i++)
        {
            SideEffectExecute see = sees[i];
            if (ObsoleteSEEs.ContainsKey(see.ID)) continue; //To prevent executed sideeffects from being executed again.
            if (seeDict.ContainsKey(see.ID))
            {
                bool isTrigger = IsExecuteTrigger(executorInfo, see.SideEffectBase.M_ExecutorInfo, see.TriggerRange); //To check out if this event invokes any sideeffect.
                if (isTrigger) Trigger(see, executorInfo, tt, see.TriggerRange); // invoke main trigger method.
            }
        }

        Invoke_RemoveSEE(tt, executorInfo); //Remove executed sideeffects with zero time left.
    }

    public void Invoke_RemoveSEE(SideEffectBundle.TriggerTime tt, SideEffectBase.ExecutorInfo executorInfo)
    {
        Dictionary<int, SideEffectExecute> seeDict = RemoveEvents[tt];
        SideEffectExecute[] sees = seeDict.Values.ToArray();
        for (int i = 0; i < sees.Length; i++)
        {
            SideEffectExecute see = sees[i];
            if (ObsoleteSEEs.ContainsKey(see.ID)) continue; //To prevent removed sideeffects from being removed again.
            if (seeDict.ContainsKey(see.ID))
            {
                bool isTrigger = IsExecuteTrigger(executorInfo, see.SideEffectBase.M_ExecutorInfo, see.RemoveTriggerRange);
                if (isTrigger) Trigger_TryRemove(see); // invoke main trigger_remove method. (some sideeffects like buffs have a remove_time attribute. e.g. Remove this buff after 3 turns)
            }
        }

        RemoveAllUselessSEEs();
    }

    private Dictionary<int, SideEffectExecute> ObsoleteSEEs = new Dictionary<int, SideEffectExecute>();

    private void RemoveAllUselessSEEs()
    {
        foreach (KeyValuePair<int, SideEffectExecute> kv in ObsoleteSEEs)
        {
            Dictionary<int, SideEffectExecute> event_sees = Events[kv.Value.TriggerTime];
            if (event_sees.ContainsKey(kv.Key)) event_sees.Remove(kv.Key);

            Dictionary<int, SideEffectExecute> removeEvent_sees = RemoveEvents[kv.Value.TriggerTime];
            if (removeEvent_sees.ContainsKey(kv.Key)) removeEvent_sees.Remove(kv.Key);

            if (kv.Value.SideEffectBase.M_ExecutorInfo.IsPlayerBuff) //PlayerBuff
            {
                if (kv.Value.SideEffectBase is PlayerBuffSideEffects buff)
                {
                    OnEventPlayerBuffRemoveHandler(kv.Value, buff);
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
    private static bool IsExecuteTrigger(SideEffectBase.ExecutorInfo executorInfo, SideEffectBase.ExecutorInfo se_ExecutorInfo, SideEffectBundle.TriggerRange tr)
    {
        bool isTrigger = false;
        switch (tr)
        {
            case SideEffectBundle.TriggerRange.SelfPlayer:
                if (executorInfo.ClientId == se_ExecutorInfo.ClientId) isTrigger = true;
                break;
            case SideEffectBundle.TriggerRange.EnemyPlayer:
                if (executorInfo.ClientId != se_ExecutorInfo.ClientId) isTrigger = true;
                break;
            case SideEffectBundle.TriggerRange.OnePlayer:
                isTrigger = true;
                break;
            case SideEffectBundle.TriggerRange.One:
                isTrigger = true;
                break;
            case SideEffectBundle.TriggerRange.SelfAnother:
                if (executorInfo.ClientId == se_ExecutorInfo.ClientId &&
                    ((se_ExecutorInfo.RetinueId != SideEffectBase.ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.RetinueId != executorInfo.RetinueId) ||
                     (se_ExecutorInfo.CardInstanceId != SideEffectBase.ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.CardInstanceId != executorInfo.CardInstanceId)))
                    isTrigger = true;
                break;
            case SideEffectBundle.TriggerRange.Another:
                if ((se_ExecutorInfo.RetinueId != SideEffectBase.ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.RetinueId != executorInfo.RetinueId) ||
                    (se_ExecutorInfo.CardInstanceId != SideEffectBase.ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.CardInstanceId != executorInfo.CardInstanceId))
                    isTrigger = true;
                break;
            case SideEffectBundle.TriggerRange.Attached:
                if (se_ExecutorInfo.RetinueId != SideEffectBase.ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.RetinueId == executorInfo.RetinueId)
                    isTrigger = true;
                break;
            case SideEffectBundle.TriggerRange.Self:
                if ((se_ExecutorInfo.RetinueId != SideEffectBase.ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.RetinueId == executorInfo.RetinueId) ||
                    (se_ExecutorInfo.CardInstanceId != SideEffectBase.ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.CardInstanceId == executorInfo.CardInstanceId) ||
                    (se_ExecutorInfo.EquipId != SideEffectBase.ExecutorInfo.EXECUTE_INFO_NONE && se_ExecutorInfo.EquipId == executorInfo.EquipId))
                    isTrigger = true;
                break;
        }

        return isTrigger;
    }

    /// <summary>
    /// Because during the process of some effects trigger, new events would happen. (e.g. trigger a damage effect and then a mech is injured and dead, then trigger a OnRetinueInjured and OnRetinueDie event)
    /// So we need a Stack data structure to manage where are we in this trigger tree.
    /// </summary>
    Stack<SideEffectExecute> InvokeStack = new Stack<SideEffectExecute>();

    private void Trigger(SideEffectExecute see, SideEffectBase.ExecutorInfo ei, SideEffectBundle.TriggerTime tt, SideEffectBundle.TriggerRange tr)
    {
        if (see.TriggerDelayTimes > 0) //TriggerDelayTimes decreases and trigger the event when it's 0
        {
            see.TriggerDelayTimes--;
            return;
        }
        else
        {
            if (see.TriggerTimes > 0) //TriggerTimes decreases every time it triggers and stop when it's 0
            {
                //Trigger's trigger  -- which triggers when other events are being triggered.
                bool isTriggerTrigger = false;
                if (tt == SideEffectBundle.TriggerTime.OnTrigger) //Give sideeffect executing info to trigger's trigger for modifying.
                {
                    if (see.SideEffectBase is PlayerBuffSideEffects buffSEE)
                    {
                        foreach (SideEffectBase se in buffSEE.Sub_SideEffect)
                        {
                            if (se is ITrigger triggerSEE)
                            {
                                triggerSEE.PeekSEE = InvokeStack.Peek();
                                if (triggerSEE.IsTrigger())
                                {
                                    isTriggerTrigger = true;
                                }
                            }
                        }
                    }
                }

                if (tt == SideEffectBundle.TriggerTime.OnTrigger && !isTriggerTrigger) return;
                //Trigger's trigger End

                see.TriggerTimes--;
                ShowSideEffectTriggeredRequest request = new ShowSideEffectTriggeredRequest(see.SideEffectBase.M_ExecutorInfo, tt, tr); //Send request to client
                OnEventInvokeHandler(request);

                InvokeStack.Push(see);
                Invoke(SideEffectBundle.TriggerTime.OnTrigger, ei);
                see.SideEffectBase.Execute(ei);
                InvokeStack.Pop();
            }
            else if (see.TriggerTimes == 0)
            {
                ObsoleteSEEs.Add(see.ID, see);
            }
        }
    }

    private void Trigger_TryRemove(SideEffectExecute see)
    {
        if (see.RemoveTriggerTimes > 0) //RemoveTriggerTimes decreases every time it triggers and removes when it's 0
        {
            see.RemoveTriggerTimes--;
            if (see.SideEffectBase is PlayerBuffSideEffects buff_se)
            {
                buff_se.RemoveTriggerTimes--;
            }

            if (see.RemoveTriggerTimes == 0)
            {
                ObsoleteSEEs.Add(see.ID, see);
            }
            else
            {
                if (see.SideEffectBase is PlayerBuffSideEffects)
                {
                    OnEventPlayerBuffUpdateHandler(see, false);
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