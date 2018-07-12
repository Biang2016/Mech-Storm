using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class ServerBattleGroundManager
{
    public bool BattleGroundIsFull;
    public ServerPlayer ServerPlayer;
    private List<ServerModuleRetinue> Retinues = new List<ServerModuleRetinue>();

    public ServerBattleGroundManager(ServerPlayer serverPlayer)
    {
        ServerPlayer = serverPlayer;
    }

    public bool SummonRetinue(SummonRetinueRequest request)
    {
        if (Retinues.Count == GamePlaySettings.MaxRetinueNumber)
        {
            BattleGroundIsFull = true;
            return false;
        }
        else
        {
            ServerModuleRetinue retinue = new ServerModuleRetinue();
            CardInfo_Retinue cardInfoRetinue = request.cardInfo;
            retinue.Initiate(cardInfoRetinue, ServerPlayer);
            Retinues.Insert(request.battleGroundIndex, retinue);


            return true;
        }
    }

    public void RemoveRetinue(ServerModuleRetinue retinue)
    {
        Retinues.Remove(retinue);
        if (Retinues.Count < GamePlaySettings.MaxRetinueNumber)
        {
            BattleGroundIsFull = false;
        }
    }


    internal void BeginRound()
    {
        foreach (ServerModuleRetinue mr in Retinues)
        {
            mr.OnBeginRound();
        }
    }

    internal void EndRound()
    {
        foreach (ServerModuleRetinue mr in Retinues)
        {
            mr.OnEndRound();
        }
    }
}