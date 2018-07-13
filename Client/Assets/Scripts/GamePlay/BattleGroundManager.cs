using System.Collections.Generic;
using UnityEngine;

internal class BattleGroundManager : MonoBehaviour
{
    internal bool BattleGroundIsFull;
    private Vector3 _defaultRetinuePosition = Vector3.zero;

    internal ClientPlayer ClientPlayer;
    private List<ModuleRetinue> Retinues = new List<ModuleRetinue>();

    private void Awake()
    {
    }

    private void Start()
    {
    }

    private void Update()
    {
    }

    public void Reset()
    {
        foreach (ModuleRetinue moduleRetinue in Retinues)
        {
            moduleRetinue.PoolRecycle();
        }

        ClientPlayer = null;
        Retinues.Clear();
    }

    internal int ComputePosition(Vector3 dragLastPosition)
    {
        int index = Mathf.RoundToInt(Mathf.Floor(dragLastPosition.x / GameManager.GM.RetinueInterval - (Retinues.Count + 1) % 2 * 0.5f) + (Retinues.Count / 2 + 1));
        if (index < 0) index = 0;
        if (index >= Retinues.Count) index = Retinues.Count;
        return index;
    }

    internal ModuleRetinue CheckRetinueOnPosition(Vector3 dragLastPosition)
    {
        var index = Mathf.RoundToInt(Mathf.Floor(dragLastPosition.x / GameManager.GM.RetinueInterval - (Retinues.Count + 1) % 2 * 0.5f) + (Retinues.Count / 2 + 1));
        if (index < 0 || index >= Retinues.Count)
            return null;
        return Retinues[index];
    }

    public void AddRetinue(SummonRetinueRequest_Response r)
    {
        if (ClientPlayer == null) return;
        ModuleRetinue retinue = GameObjectPoolManager.GOPM.Pool_ModuleRetinuePool.AllocateGameObject(transform).GetComponent<ModuleRetinue>();
        retinue.Initiate(r.cardInfo, ClientPlayer);
        retinue.transform.Rotate(Vector3.up, 180);
        if (r.battleGroundIndex < 0 || r.battleGroundIndex >= GamePlaySettings.MaxRetinueNumber) ClientLog.CL.Print("Retinue index out of bound");
        Retinues.Insert(r.battleGroundIndex, retinue);
        BattleGroundIsFull |= Retinues.Count == GamePlaySettings.MaxRetinueNumber;
        RefreshBattleGround();
    }

    public void RemoveRetinue(ModuleRetinue retinue)
    {
        if (Retinues.Contains(retinue))
        {
            Retinues.Remove(retinue);
            BattleGroundIsFull |= Retinues.Count == GamePlaySettings.MaxRetinueNumber;
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
        foreach (var retinue in Retinues)
        {
            retinue.transform.localPosition = _defaultRetinuePosition;
            retinue.transform.Translate(Vector3.left * (count - Retinues.Count / 2.0f + 0.5f) * GameManager.GM.RetinueInterval);
            count++;
        }
    }

    internal void BeginRound()
    {
        foreach (var mr in Retinues) mr.OnBeginRound();
    }

    internal void EndRound()
    {
        foreach (var mr in Retinues) mr.OnEndRound();
    }
}