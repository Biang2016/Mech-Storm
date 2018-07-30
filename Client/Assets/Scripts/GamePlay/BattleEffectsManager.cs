using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

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

    public bool isExecuting_Main;
    public bool isExecuting_RefreshBattleGroundOnAddRetinue;
    public bool isExecuting_TipSlotBloom;

    void Start()
    {
        Effect_Main = new Effects("Effect_M");
        Effect_RefreshBattleGroundOnAddRetinue = new Effects("Effect_R");
        Effect_TipSlotBloom = new Effects("Effect_T");
    }

    void Update()
    {
        Effect_Main.Update();
        Effect_RefreshBattleGroundOnAddRetinue.Update();
        Effect_TipSlotBloom.Update();

        isExecuting_Main = Effect_Main.IsExcuting;
        isExecuting_RefreshBattleGroundOnAddRetinue = Effect_RefreshBattleGroundOnAddRetinue.IsExcuting;
        isExecuting_TipSlotBloom = Effect_TipSlotBloom.IsExcuting;

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
        public string Name;

        public Effects(string name)
        {
            Name = name;
        }

        public void Update()
        {
            if (!IsExcuting && EffectsQueue.Count != 0)
            {
                SideEffect se = EffectsQueue.Dequeue();
                BEM.StartCoroutine(se.Enumerator);
                CurrentEffect = se;
                IsExcuting = true;
                ClientLog.CL.PrintBattleEffectsStart("+ [" + Name + "] StartEffect: " + se.MethodName + " id: " + se.EffectId);
            }
        }

        public SideEffect GetCurrentSideEffect()
        {
            return CurrentEffect;
        }

        private int effectId = 0;

        public int GenerateEffectId()
        {
            return effectId++;
        }

        public int EffectsShow(IEnumerator enumerator, string methodName)
        {
            int id = GenerateEffectId();
            SideEffect se = new SideEffect(enumerator, methodName, id);
            EffectsQueue.Enqueue(se);
            return id;
        }

        public void EffectEnd()
        {
            ClientLog.CL.PrintWarning("end");

            if (CurrentEffect != null)
            {
                if (CurrentEffect.Enumerator != null)
                {
                    try
                    {
                        BEM.StopCoroutine(CurrentEffect.Enumerator);
                        ClientLog.CL.PrintBattleEffectsEnd("- [" + Name + "] EndEffect: " + CurrentEffect.MethodName + " id: " + CurrentEffect.EffectId);
                    }
                    catch (Exception e)
                    {
                        ClientLog.CL.PrintWarning(e.ToString());
                    }
                }
                else
                {
                    ClientLog.CL.PrintWarning("CurrentEffect.Enumerator = null");
                }

                CurrentEffect = null;
            }
            else
            {
                ClientLog.CL.PrintWarning("CurrentEffect = null");
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
        public int EffectId;

        public SideEffect(IEnumerator enumerator, string methodName, int effectId)
        {
            Enumerator = enumerator;
            MethodName = methodName;
            EffectId = effectId;
        }
    }
}