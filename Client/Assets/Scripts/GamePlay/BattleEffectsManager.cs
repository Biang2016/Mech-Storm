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
            IEnumerator effect = BattleEffects.Dequeue();
            StartCoroutine(effect);
            IsExcuting = true;
        }

        if (ResponseExcuteQueue.Count != 0)
        {
            ResponseAndMethod responseAndMethod = ResponseExcuteQueue.Dequeue();
            responseAndMethod.BattleResponse(responseAndMethod.Request);
        }
    }

    #region 非协程效果的队列（如延迟销毁对象）

    public delegate void BattleResponse(ServerRequestBase request);

    public Queue<ResponseAndMethod> ResponseExcuteQueue = new Queue<ResponseAndMethod>();

    public class ResponseAndMethod
    {
        public BattleResponse BattleResponse;
        public ServerRequestBase Request;

        public ResponseAndMethod(BattleResponse battleResponse, ServerRequestBase request)
        {
            BattleResponse = battleResponse;
            Request = request;
        }
    }

    #endregion


    #region 协程效果的队列（如战斗特效）

    private bool IsExcuting = false;

    private Queue<IEnumerator> BattleEffects = new Queue<IEnumerator>();

    public void EffectsShow(IEnumerator enumerator)
    {
        BattleEffects.Enqueue(enumerator);
    }

    public void EffectEnd()
    {
        IsExcuting = false;
    }

    public void AllEffectsEnd()
    {
        BattleEffects.Clear();
        StopAllCoroutines();
    }

    #endregion
}