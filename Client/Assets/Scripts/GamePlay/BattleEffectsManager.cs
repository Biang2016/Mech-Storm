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
        if (!IsExcuting && BattleEffects.Count != 0)
        {
            SideEffect se = BattleEffects.Dequeue();
            StartCoroutine(se.Enumerator);
            IsExcuting = true;
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

    private bool IsExcuting = false;

    private Queue<SideEffect> BattleEffects = new Queue<SideEffect>();

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
        BattleEffects.Enqueue(se);
    }

    public void EffectEnd()
    {
        IsExcuting = false;
    }

    public void AllEffectsEnd()
    {
        BattleEffects.Clear();
        StopAllCoroutines();
        IsExcuting = false;
    }

    #endregion
}