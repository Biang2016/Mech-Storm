using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.SqlServer.Server;

public class EventManager
{
    public EventManager()
    {
        foreach (SideEffectBundle.TriggerTime tt in Enum.GetValues(typeof(SideEffectBundle.TriggerTime)))
        {
            Events.Add(tt, new List<SideEffectBundle.SideEffectExecute>());
        }
    }

    private SortedDictionary<SideEffectBundle.TriggerTime, List<SideEffectBundle.SideEffectExecute>> Events = new SortedDictionary<SideEffectBundle.TriggerTime, List<SideEffectBundle.SideEffectExecute>>();

    public void RegisterEvent(SideEffectBundle sideEffects)
    {
        foreach (SideEffectBundle.SideEffectExecute see in sideEffects.GetSideEffects())
        {
            Events[see.TriggerTime].Add(see);
        }
    }

    public void UnRegisterEvent(SideEffectBundle sideEffects)
    {
        foreach (SideEffectBundle.SideEffectExecute see in sideEffects.GetSideEffects())
        {
            List<SideEffectBundle.SideEffectExecute> sees = Events[see.TriggerTime];
            if (sees.Contains(see)) sees.Remove(see);
        }
    }

    public void RemoveSideEffect(SideEffectBundle.TriggerTime tt, SideEffectBundle.SideEffectExecute see)
    {
        List<SideEffectBundle.SideEffectExecute> te = Events[tt];
        if (te != null && te.Contains(see)) te.Remove(see);
    }

    public void ClearAllListeners()
    {
        foreach (SideEffectBundle.TriggerTime tt in Enum.GetValues(typeof(SideEffectBundle.TriggerTime)))
        {
            Events[tt] = new List<SideEffectBundle.SideEffectExecute>();
        }
    }

    private static int InvokeStackDepth = 0;

    public void Invoke(SideEffectBundle.TriggerTime tt, SideEffectBase.ExecuterInfo executerInfo)
    {
        InvokeStackDepth++;
        List<SideEffectBundle.SideEffectExecute> seeList = Events[tt];
        SideEffectBundle.SideEffectExecute[] sees = seeList.ToArray();
        for (int i = 0; i < sees.Length; i++)
        {
            SideEffectBundle.SideEffectExecute see = sees[i];
            SideEffectBase se = see.SideEffectBase;
            SideEffectBundle.TriggerRange tr = see.TriggerRange;
            if (!seeList.Contains(see)) continue; //防止已经移除的SE再次执行
            switch (tr)
            {
                case SideEffectBundle.TriggerRange.SelfPlayer:
                    if (executerInfo.ClientId == se.M_ExecuterInfo.ClientId) Trigger(se, executerInfo, tt, tr);
                    break;
                case SideEffectBundle.TriggerRange.EnemyPlayer:
                    if (executerInfo.ClientId != se.M_ExecuterInfo.ClientId) Trigger(se, executerInfo, tt, tr);
                    break;
                case SideEffectBundle.TriggerRange.OnePlayer:
                    Trigger(se, executerInfo, tt, tr);
                    break;
                case SideEffectBundle.TriggerRange.One:
                    Trigger(se, executerInfo, tt, tr);
                    break;
                case SideEffectBundle.TriggerRange.SelfAnother:
                    if (executerInfo.ClientId == se.M_ExecuterInfo.ClientId &&
                        ((se.M_ExecuterInfo.RetinueId != -999 && se.M_ExecuterInfo.RetinueId != executerInfo.RetinueId) ||
                         (se.M_ExecuterInfo.CardInstanceId != -999 && se.M_ExecuterInfo.CardInstanceId != executerInfo.CardInstanceId)))
                        Trigger(se, executerInfo, tt, tr);
                    break;
                case SideEffectBundle.TriggerRange.Another:
                    if ((se.M_ExecuterInfo.RetinueId != -999 && se.M_ExecuterInfo.RetinueId != executerInfo.RetinueId) ||
                        (se.M_ExecuterInfo.CardInstanceId != -999 && se.M_ExecuterInfo.CardInstanceId != executerInfo.CardInstanceId))
                        Trigger(se, executerInfo, tt, tr);
                    break;
                case SideEffectBundle.TriggerRange.Attached:
                    if (se.M_ExecuterInfo.RetinueId != -999 && se.M_ExecuterInfo.RetinueId == executerInfo.RetinueId)
                        Trigger(se, executerInfo, tt, tr);
                    break;
                case SideEffectBundle.TriggerRange.Self:
                    if ((se.M_ExecuterInfo.RetinueId != -999 && se.M_ExecuterInfo.RetinueId == executerInfo.RetinueId) ||
                        (se.M_ExecuterInfo.CardInstanceId != -999 && se.M_ExecuterInfo.CardInstanceId == executerInfo.CardInstanceId) ||
                        (se.M_ExecuterInfo.EquipId != -999 && se.M_ExecuterInfo.EquipId == executerInfo.EquipId))
                        Trigger(se, executerInfo, tt, tr);
                    break;
            }
        }

        InvokeStackDepth--;
        if (InvokeStackDepth == 0) OnEventInvokeEndHandler();
    }

    private void Trigger(SideEffectBase se, SideEffectBase.ExecuterInfo ei, SideEffectBundle.TriggerTime tt, SideEffectBundle.TriggerRange tr)
    {
        ShowSideEffectTriggeredRequest request = new ShowSideEffectTriggeredRequest(se.M_ExecuterInfo, tt, tr);
        OnEventInvokeHandler(request);
        se.Excute(ei);
    }

    public delegate void OnEventInvoke(ShowSideEffectTriggeredRequest request);

    public OnEventInvoke OnEventInvokeHandler;

    public delegate void OnEventInvokeEnd();

    public OnEventInvokeEnd OnEventInvokeEndHandler;
}