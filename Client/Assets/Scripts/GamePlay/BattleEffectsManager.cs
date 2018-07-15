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

    //管理战斗时的各种协程
    private bool IsExcuting = false;

    private Queue<IEnumerator> BattleEffects = new Queue<IEnumerator>();

    public void EffectsShow(IEnumerator enumerator)
    {
        BattleEffects.Enqueue(enumerator);
    }

    public void EffectEnd(){
        IsExcuting = false;
    }

    public void AllEffectsEnd(){
        BattleEffects.Clear();
        StopAllCoroutines();
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
    }
}