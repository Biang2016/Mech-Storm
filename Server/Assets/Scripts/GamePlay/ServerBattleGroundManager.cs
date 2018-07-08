using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ServerBattleGroundManager
{
    public bool BattleGroundIsFull;
    public ServerPlayer ServerPlayer;
    private int _retinueCount;
    private List<ServerRetinue> _retinues = new List<ServerRetinue>();

    internal void BeginRound()
    {
        foreach (ServerRetinue mr in _retinues)
        {
            mr.OnBeginRound();
        }
    }

    internal void EndRound()
    {
        foreach (ServerRetinue mr in _retinues)
        {
            mr.OnEndRound();
        }
    }
}