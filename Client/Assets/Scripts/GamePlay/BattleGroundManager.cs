using System.Collections.Generic;
using UnityEngine;

public class BattleGroundManager : MonoBehaviour
{
    internal bool BattleGroundIsFull;
    private Vector3 _defaultRetinuePosition = Vector3.zero;

    internal ClientPlayer ClientPlayer;
    private int _retinueCount;

    private List<ModuleRetinue> _retinues = new List<ModuleRetinue>();

    private void Awake()
    {
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    internal int ComputePosition(Vector3 dragLastPosition)
    {
        int index = Mathf.RoundToInt(Mathf.Floor(dragLastPosition.x / GameManager.GM.RetinueInterval - (_retinueCount + 1) % 2 * 0.5f) + (_retinueCount / 2 + 1));
        if (index < 0) index = 0;
        if (index >= _retinueCount) index = _retinueCount;
        return index;
    }

    internal ModuleRetinue CheckRetinueOnPosition(Vector3 dragLastPosition)
    {
        var index = Mathf.RoundToInt(Mathf.Floor(dragLastPosition.x / GameManager.GM.RetinueInterval - (_retinueCount + 1) % 2 * 0.5f) + (_retinueCount / 2 + 1));
        if (index < 0 || index >= _retinueCount)
            return null;
        return _retinues[index];
    }

    internal void AddRetinue(ModuleRetinue newRetinue, int index)
    {
        newRetinue.transform.Rotate(Vector3.up, 180);
        if (index < 0 || index >= GamePlaySettings.MaxRetinueNumber) ClientLog.CL.Print("Retinue index out of bound");
        _retinues.Insert(index, newRetinue);
        BattleGroundIsFull |= ++_retinueCount == GamePlaySettings.MaxRetinueNumber;
        RefreshBattleGround();
    }

    internal void RemoveRetinue(ModuleRetinue retinue)
    {
        if (_retinues.Contains(retinue))
        {
            _retinues.Remove(retinue);
            BattleGroundIsFull |= --_retinueCount == GamePlaySettings.MaxRetinueNumber;
            RefreshBattleGround();
        }
        else
        {
            Debug.LogWarning("战场上不存在该对象 " + retinue);
        }
    }

    internal void RefreshBattleGround()
    {
        var count = 0;
        foreach (var retinue in _retinues)
        {
            retinue.transform.localPosition = _defaultRetinuePosition;
            retinue.transform.Translate(Vector3.left * (count - _retinueCount / 2.0f + 0.5f) * GameManager.GM.RetinueInterval);
            count++;
        }
    }

    internal void BeginRound()
    {
        foreach (var mr in _retinues) mr.OnBeginRound();
    }

    internal void EndRound()
    {
        foreach (var mr in _retinues) mr.OnEndRound();
    }
}