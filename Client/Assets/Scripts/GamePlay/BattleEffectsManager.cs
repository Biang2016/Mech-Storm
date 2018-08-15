using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

internal class BattleEffectsManager : MonoSingletion<BattleEffectsManager>
{
    private BattleEffectsManager()
    {
    }

    public Effects Effect_Main;
    public Effects Effect_RefreshBattleGroundOnAddRetinue;
    public Effects Effect_TipSlotBloom;
    public Effects Effect_UsedCardShow;

    public bool isExecuting_Main;
    public bool isExecuting_RefreshBattleGroundOnAddRetinue;
    public bool isExecuting_TipSlotBloom;
    public bool isExecuting_UsedCardShow;

    void Start()
    {
        Effect_Main = new Effects("Effect_M");
        Effect_RefreshBattleGroundOnAddRetinue = new Effects("Effect_R");
        Effect_TipSlotBloom = new Effects("Effect_T");
        Effect_UsedCardShow = new Effects("Effect_U");
    }

    void Update()
    {
        Effect_Main.Update();
        Effect_RefreshBattleGroundOnAddRetinue.Update();
        Effect_TipSlotBloom.Update();
        Effect_UsedCardShow.Update();

        isExecuting_Main = Effect_Main.IsExcuting;
        isExecuting_RefreshBattleGroundOnAddRetinue = Effect_RefreshBattleGroundOnAddRetinue.IsExcuting;
        isExecuting_TipSlotBloom = Effect_TipSlotBloom.IsExcuting;
        isExecuting_UsedCardShow = Effect_UsedCardShow.IsExcuting;
    }

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
                Instance.StartCoroutine(se.Enumerator);
                CurrentEffect = se;
                IsExcuting = true;
                if (GameManager.Instance.ShowBEMMessages) ClientLog.CL.PrintBattleEffectsStart("+ [" + Name + "] StartEffect: " + se.MethodName + " id: " + se.EffectId);
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
            if (GameManager.Instance.ShowBEMMessages) ClientLog.CL.PrintWarning("end");

            if (CurrentEffect != null)
            {
                if (CurrentEffect.Enumerator != null)
                {
                    try
                    {
                        Instance.StopCoroutine(CurrentEffect.Enumerator);
                        if (GameManager.Instance.ShowBEMMessages) ClientLog.CL.PrintBattleEffectsEnd("- [" + Name + "] EndEffect: " + CurrentEffect.MethodName + " id: " + CurrentEffect.EffectId);
                    }
                    catch (Exception e)
                    {
                        if (GameManager.Instance.ShowBEMMessages) ClientLog.CL.PrintWarning(e.ToString());
                    }
                }
                else
                {
                    if (GameManager.Instance.ShowBEMMessages) ClientLog.CL.PrintWarning("CurrentEffect.Enumerator = null");
                }

                CurrentEffect = null;
            }
            else
            {
                if (GameManager.Instance.ShowBEMMessages) ClientLog.CL.PrintWarning("CurrentEffect = null");
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