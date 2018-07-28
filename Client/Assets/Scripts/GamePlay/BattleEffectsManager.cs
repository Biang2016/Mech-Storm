using System;
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

    public Effects Effect_Main;
    public Effects Effect_RefreshBattleGroundOnAddRetinue;
    public Effects Effect_TipSlotBloom;

    void Start()
    {
        Effect_Main = new Effects();
        Effect_RefreshBattleGroundOnAddRetinue = new Effects();
        Effect_TipSlotBloom = new Effects();
    }

    void Update()
    {
        Effect_Main.Update();
        Effect_RefreshBattleGroundOnAddRetinue.Update();
        Effect_TipSlotBloom.Update();

        //if (ResponseExcuteQueue.Count != 0)
        //{
        //    ResponseAndMethod responseAndMethod = ResponseExcuteQueue.Dequeue();
        //    responseAndMethod.BattleResponse(responseAndMethod.Request);
        //    ClientLog.CL.PrintBattleEffects(responseAndMethod.MethodName);
        //}
    }

    //#region 非协程效果的队列（如延迟销毁对象）

    //public delegate void BattleResponse(ServerRequestBase request);

    //public Queue<ResponseAndMethod> ResponseExcuteQueue = new Queue<ResponseAndMethod>();

    //public class ResponseAndMethod
    //{
    //    public BattleResponse BattleResponse;
    //    public ServerRequestBase Request;
    //    public string MethodName;
    //}

    //#endregion


    public class Effects
    {
        public bool IsExcuting = false;

        private Queue<SideEffect> EffectsQueue = new Queue<SideEffect>();
        private SideEffect CurrentEffect;

        public void Update()
        {
            if (!IsExcuting && EffectsQueue.Count != 0)
            {
                SideEffect se = EffectsQueue.Dequeue();
                BEM.StartCoroutine(se.Enumerator);
                IsExcuting = true;
                ClientLog.CL.PrintBattleEffects(se.MethodName);
            }
        }

        public SideEffect GetCurrentSideEffect()
        {
            return CurrentEffect;
        }

        public void EffectsShow(IEnumerator enumerator, string methodName)
        {
            SideEffect se = new SideEffect(enumerator, methodName);
            EffectsQueue.Enqueue(se);
        }

        public void EffectEnd()
        {
            if (CurrentEffect != null)
            {
                if (CurrentEffect.Enumerator != null)
                {
                    try
                    {
                        BEM.StopCoroutine(CurrentEffect.Enumerator);
                        ClientLog.CL.PrintBattleEffects("End");
                    }
                    catch (Exception e)
                    {
                        ClientLog.CL.PrintWarning(e.ToString());
                    }
                }
            }

            IsExcuting = false;
        }

        public void AllEffectsEnd()
        {
            EffectEnd();
            EffectsQueue.Clear();
        }
    }

    public class SideEffect
    {
        public IEnumerator Enumerator;
        public string MethodName;

        public SideEffect(IEnumerator enumerator, string methodName)
        {
            Enumerator = enumerator;
            MethodName = methodName;
        }
    }
}