using UnityEngine;
using System.Collections;
using System.Collections.Generic;

internal class BattleEffectsManager : MonoBehaviour
{
    private static BattleEffectsManager _bem;

    public static BattleEffectsManager BEM
    {
        get
        {
            if (!_bem) _bem = FindObjectOfType<BattleEffectsManager>();
            return _bem;
        }
    }

    private BattleEffectsManager()
    {
    }

    void Start()
    {
    }

    void Update()
    {
        if (!IsExcuting_Main && BattleEffects_Main.Count != 0)
        {
            SideEffect se = BattleEffects_Main.Dequeue();
            StartCoroutine(se.Enumerator);
            IsExcuting_Main = true;
            ClientLog.CL.PrintBattleEffects(se.MethodName);
        }

        if (!IsExcuting_Sub && BattleEffects_Sub.Count != 0)
        {
            SideEffect se = BattleEffects_Sub.Dequeue();
            StartCoroutine(se.Enumerator);
            IsExcuting_Sub = true;
            ClientLog.CL.PrintBattleEffects(se.MethodName);
        }

        if (ResponseExcuteQueue.Count != 0)
        {
            ResponseAndMethod responseAndMethod = ResponseExcuteQueue.Dequeue();
            responseAndMethod.BattleResponse(responseAndMethod.Request);
            ClientLog.CL.PrintBattleEffects(responseAndMethod.MethodName);
        }
    }

    #region 非协程效果的队列（如延迟销毁对象）

    public delegate void BattleResponse(ServerRequestBase request);

    public Queue<ResponseAndMethod> ResponseExcuteQueue = new Queue<ResponseAndMethod>();

    public class ResponseAndMethod
    {
        public BattleResponse BattleResponse;
        public ServerRequestBase Request;
        public string MethodName;
    }

    #endregion


    #region 协程效果的队列（如战斗特效）

    public bool IsExcuting_Main = false;

    private Queue<SideEffect> BattleEffects_Main = new Queue<SideEffect>();

    private class SideEffect
    {
        public IEnumerator Enumerator;
        public string MethodName;

        public SideEffect(IEnumerator enumerator, string methodName)
        {
            Enumerator = enumerator;
            MethodName = methodName;
        }
    }

    public void EffectsShow(IEnumerator enumerator,string methodName)
    {
        SideEffect se=new SideEffect(enumerator,methodName);
        BattleEffects_Main.Enqueue(se);
    }

    public void EffectEnd()
    {
        IsExcuting_Main = false;
    }

    public void AllEffectsEnd()
    {
        BattleEffects_Main.Clear();
        StopAllCoroutines();
        IsExcuting_Main = false;
    }

    #endregion

    #region 协程效果的队列2（如战斗特效）

    public bool IsExcuting_Sub = false;

    private Queue<SideEffect> BattleEffects_Sub = new Queue<SideEffect>();

    public void EffectsShow_Sub(IEnumerator enumerator,string methodName)
    {
        SideEffect se=new SideEffect(enumerator,methodName);
        BattleEffects_Sub.Enqueue(se);
    }

    public void EffectEnd_Sub()
    {
        IsExcuting_Sub = false;
    }

    public void AllEffectsEnd_Sub()
    {
        BattleEffects_Sub.Clear();
        StopAllCoroutines();
        IsExcuting_Sub = false;
    }

    #endregion
}