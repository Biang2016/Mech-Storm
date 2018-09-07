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
            SideEffectBundle.SideEffectExecute se = sees[i];
            if (!seeList.Contains(se)) continue; //防止已经移除的SE再次执行
            switch (se.TriggerRange)
            {
                case SideEffectBundle.TriggerRange.SelfPlayer:
                    if (executerInfo.ClientId == se.SideEffectBase.M_ExecuterInfo.ClientId) se.SideEffectBase.Excute(executerInfo);
                    break;
                case SideEffectBundle.TriggerRange.EnemyPlayer:
                    if (executerInfo.ClientId != se.SideEffectBase.M_ExecuterInfo.ClientId) se.SideEffectBase.Excute(executerInfo);
                    break;
                case SideEffectBundle.TriggerRange.OnePlayer:
                    se.SideEffectBase.Excute(executerInfo);
                    break;
                case SideEffectBundle.TriggerRange.One:
                    se.SideEffectBase.Excute(executerInfo);
                    break;
                case SideEffectBundle.TriggerRange.SelfAnother:
                    if (executerInfo.ClientId == se.SideEffectBase.M_ExecuterInfo.ClientId &&
                        ((se.SideEffectBase.M_ExecuterInfo.RetinueId != -999 && se.SideEffectBase.M_ExecuterInfo.RetinueId != executerInfo.RetinueId) ||
                         (se.SideEffectBase.M_ExecuterInfo.CardInstanceId != -999 && se.SideEffectBase.M_ExecuterInfo.CardInstanceId != executerInfo.CardInstanceId)))
                        se.SideEffectBase.Excute(executerInfo);
                    break;
                case SideEffectBundle.TriggerRange.Another:
                    if ((se.SideEffectBase.M_ExecuterInfo.RetinueId != -999 && se.SideEffectBase.M_ExecuterInfo.RetinueId != executerInfo.RetinueId) ||
                        (se.SideEffectBase.M_ExecuterInfo.CardInstanceId != -999 && se.SideEffectBase.M_ExecuterInfo.CardInstanceId != executerInfo.CardInstanceId))
                        se.SideEffectBase.Excute(executerInfo);
                    break;
                case SideEffectBundle.TriggerRange.Attached:
                    if (se.SideEffectBase.M_ExecuterInfo.RetinueId != -999 && se.SideEffectBase.M_ExecuterInfo.RetinueId == executerInfo.RetinueId)
                        se.SideEffectBase.Excute(executerInfo);
                    break;
                case SideEffectBundle.TriggerRange.Self:
                    if ((se.SideEffectBase.M_ExecuterInfo.RetinueId != -999 && se.SideEffectBase.M_ExecuterInfo.RetinueId == executerInfo.RetinueId) ||
                        (se.SideEffectBase.M_ExecuterInfo.CardInstanceId != -999 && se.SideEffectBase.M_ExecuterInfo.CardInstanceId == executerInfo.CardInstanceId))
                        se.SideEffectBase.Excute(executerInfo);
                    break;
            }
        }

        InvokeStackDepth--;
        if (InvokeStackDepth == 0) OnEventInvokeEndHandler();
    }

    public delegate void OnEventInvokeEnd();

    public OnEventInvokeEnd OnEventInvokeEndHandler;
}