using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

public class EventManager
{
    public EventManager()
    {
        foreach (SideEffectBundle.TriggerTime tt in Enum.GetValues(typeof(SideEffectBundle.TriggerTime)))
        {
            Events.Add(tt, new Dictionary<int, SideEffectExecute>());
            RemoveEvents.Add(tt, new Dictionary<int, SideEffectExecute>());
        }
    }

    //事件总表
    private SortedDictionary<SideEffectBundle.TriggerTime, Dictionary<int, SideEffectExecute>> Events = new SortedDictionary<SideEffectBundle.TriggerTime, Dictionary<int, SideEffectExecute>>();

    //移除事件总表(如果触发则移除该SEE)
    private SortedDictionary<SideEffectBundle.TriggerTime, Dictionary<int, SideEffectExecute>> RemoveEvents = new SortedDictionary<SideEffectBundle.TriggerTime, Dictionary<int, SideEffectExecute>>();


    public void RegisterEvent(SideEffectBundle sideEffectBundle)
    {
        foreach (SideEffectExecute see in sideEffectBundle.SideEffectExecutes)
        {
            RegisterEvent(see);
        }
    }

    public void RegisterEvent(SideEffectExecute sideEffectExecute)
    {
        if (sideEffectExecute.TriggerTime != SideEffectBundle.TriggerTime.None && sideEffectExecute.TriggerRange != SideEffectBundle.TriggerRange.None)
        {
            Events[sideEffectExecute.TriggerTime].Add(sideEffectExecute.ID, sideEffectExecute);
            RemoveEvents[sideEffectExecute.RemoveTriggerTime].Add(sideEffectExecute.ID, sideEffectExecute);
        }
    }

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

    public void ClearAllEvents()
    {
        foreach (SideEffectBundle.TriggerTime tt in Enum.GetValues(typeof(SideEffectBundle.TriggerTime)))
        {
            Events[tt] = new Dictionary<int, SideEffectExecute>();
            RemoveEvents[tt] = new Dictionary<int, SideEffectExecute>();
        }

        ObsoleteSEEs.Clear();
    }

    public void Invoke(SideEffectBundle.TriggerTime tt, SideEffectBase.ExecuterInfo executerInfo) //触发事件由位控制，触发所有对应位的事件
    {
        foreach (SideEffectBundle.TriggerTime triggerTime in Enum.GetValues(typeof(SideEffectBundle.TriggerTime)))
        {
            if ((tt & triggerTime) == tt)
            {
                InvokeTriggerTimeQueue.Enqueue(new InvokeInfo(triggerTime, executerInfo)); //所有事件入队
            }
        }

        while (InvokeTriggerTimeQueue.Count > 0) //逐个处理
        {
            InvokeInfo invokeInfo = InvokeTriggerTimeQueue.Dequeue();
            InvokeCore(invokeInfo);
        }

        RemoveAllUselessSEEs(); //移除所有失效的SE
        OnEventInvokeEndHandler(); //可以发送数据包给客户端
    }

    struct InvokeInfo
    {
        public SideEffectBundle.TriggerTime TriggerTime;
        public SideEffectBase.ExecuterInfo ExecuterInfo;

        public InvokeInfo(SideEffectBundle.TriggerTime triggerTime, SideEffectBase.ExecuterInfo executerInfo)
        {
            TriggerTime = triggerTime;
            ExecuterInfo = executerInfo;
        }
    }

    Queue<InvokeInfo> InvokeTriggerTimeQueue = new Queue<InvokeInfo>();

    Stack<SideEffectExecute> InvokeStack = new Stack<SideEffectExecute>();

    private void InvokeCore(InvokeInfo invokeInfo)
    {
        SideEffectBundle.TriggerTime tt = invokeInfo.TriggerTime;
        SideEffectBase.ExecuterInfo executerInfo = invokeInfo.ExecuterInfo;

        Dictionary<int, SideEffectExecute> seeDict = Events[tt];
        SideEffectExecute[] sees = seeDict.Values.ToArray();
        for (int i = 0; i < sees.Length; i++)
        {
            SideEffectExecute see = sees[i];
            if (ObsoleteSEEs.ContainsKey(see.ID)) continue; //防止已经移除的SE再次执行
            if (seeDict.ContainsKey(see.ID))
            {
                bool isTrigger = IsExecuteTrigger(executerInfo, see.SideEffectBase.M_ExecuterInfo, see.TriggerRange);
                if (isTrigger) Trigger(see, executerInfo, tt, see.TriggerRange);
            }
        }

        //进行失效SEE的移除
        Invoke_RemoveSEE(tt, executerInfo);
    }

    public void Invoke_RemoveSEE(SideEffectBundle.TriggerTime tt, SideEffectBase.ExecuterInfo executerInfo)
    {
        Dictionary<int, SideEffectExecute> seeDict = RemoveEvents[tt];
        SideEffectExecute[] sees = seeDict.Values.ToArray();
        for (int i = 0; i < sees.Length; i++)
        {
            SideEffectExecute see = sees[i];
            if (ObsoleteSEEs.ContainsKey(see.ID)) continue; //防止已经移除的SE再次执行
            if (seeDict.ContainsKey(see.ID))
            {
                bool isTrigger = IsExecuteTrigger(executerInfo, see.SideEffectBase.M_ExecuterInfo, see.RemoveTriggerRange);
                if (isTrigger) Trigger_TryRemove(see);
            }
        }

        RemoveAllUselessSEEs(); //移除所有失效的SE
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

            if (kv.Value.SideEffectBase.M_ExecuterInfo.IsPlayerBuff) //PlayerBuff
            {
                if (kv.Value.SideEffectBase is PlayerBuffSideEffects buff)
                {
                    OnEventPlayerBuffRemoveHandler(kv.Value, buff);
                }
            }
        }

        ObsoleteSEEs.Clear();
    }

    private static bool IsExecuteTrigger(SideEffectBase.ExecuterInfo executerInfo, SideEffectBase.ExecuterInfo se_ExecuterInfo, SideEffectBundle.TriggerRange tr)
    {
        bool isTrigger = false;
        switch (tr)
        {
            case SideEffectBundle.TriggerRange.SelfPlayer:
                if (executerInfo.ClientId == se_ExecuterInfo.ClientId) isTrigger = true;
                break;
            case SideEffectBundle.TriggerRange.EnemyPlayer:
                if (executerInfo.ClientId != se_ExecuterInfo.ClientId) isTrigger = true;
                break;
            case SideEffectBundle.TriggerRange.OnePlayer:
                isTrigger = true;
                break;
            case SideEffectBundle.TriggerRange.One:
                isTrigger = true;
                break;
            case SideEffectBundle.TriggerRange.SelfAnother:
                if (executerInfo.ClientId == se_ExecuterInfo.ClientId &&
                    ((se_ExecuterInfo.RetinueId != SideEffectBase.ExecuterInfo.EXECUTE_INFO_NONE && se_ExecuterInfo.RetinueId != executerInfo.RetinueId) ||
                     (se_ExecuterInfo.CardInstanceId != SideEffectBase.ExecuterInfo.EXECUTE_INFO_NONE && se_ExecuterInfo.CardInstanceId != executerInfo.CardInstanceId)))
                    isTrigger = true;
                break;
            case SideEffectBundle.TriggerRange.Another:
                if ((se_ExecuterInfo.RetinueId != SideEffectBase.ExecuterInfo.EXECUTE_INFO_NONE && se_ExecuterInfo.RetinueId != executerInfo.RetinueId) ||
                    (se_ExecuterInfo.CardInstanceId != SideEffectBase.ExecuterInfo.EXECUTE_INFO_NONE && se_ExecuterInfo.CardInstanceId != executerInfo.CardInstanceId))
                    isTrigger = true;
                break;
            case SideEffectBundle.TriggerRange.Attached:
                if (se_ExecuterInfo.RetinueId != SideEffectBase.ExecuterInfo.EXECUTE_INFO_NONE && se_ExecuterInfo.RetinueId == executerInfo.RetinueId)
                    isTrigger = true;
                break;
            case SideEffectBundle.TriggerRange.Self:
                if ((se_ExecuterInfo.RetinueId != SideEffectBase.ExecuterInfo.EXECUTE_INFO_NONE && se_ExecuterInfo.RetinueId == executerInfo.RetinueId) ||
                    (se_ExecuterInfo.CardInstanceId != SideEffectBase.ExecuterInfo.EXECUTE_INFO_NONE && se_ExecuterInfo.CardInstanceId == executerInfo.CardInstanceId) ||
                    (se_ExecuterInfo.EquipId != SideEffectBase.ExecuterInfo.EXECUTE_INFO_NONE && se_ExecuterInfo.EquipId == executerInfo.EquipId))
                    isTrigger = true;
                break;
        }

        return isTrigger;
    }

    private void Trigger(SideEffectExecute see, SideEffectBase.ExecuterInfo ei, SideEffectBundle.TriggerTime tt, SideEffectBundle.TriggerRange tr)
    {
        if (see.TriggerDelayTimes > 0) //触发延迟时间减少，直至0时触发
        {
            see.TriggerDelayTimes--;
            return;
        }
        else
        {
            if (see.TriggerTimes > 0) //触发次数减少，为0时不触发
            {
                //触发的触发 Start
                bool isTriggerTrigger = false;
                if (tt == SideEffectBundle.TriggerTime.OnTrigger) //如果是某个SEE触发时引起触发的triggerSEE，将该SEE传给triggerSEE，供triggerSEE修改属性
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
                //触发的触发 End

                see.TriggerTimes--;
                ShowSideEffectTriggeredRequest request = new ShowSideEffectTriggeredRequest(see.SideEffectBase.M_ExecuterInfo, tt, tr);
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
        if (see.RemoveTriggerTimes > 0) //移除判定剩余次数减少，为0时移除
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