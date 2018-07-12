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
    internal bool IsExcuting = false;

    internal Queue<IEnumerator> BattleEffects = new Queue<IEnumerator>();


    void Start()
    {
    }

    void Update()
    {
        if (!IsExcuting && BattleEffects.Count != 0)
        {
            StartCoroutine(BattleEffects.Dequeue());
            IsExcuting = true;
        }
    }
}