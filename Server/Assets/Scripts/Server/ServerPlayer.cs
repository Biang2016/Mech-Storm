using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ServerPlayer : Player
{
    public int ClientId;
    public int CostMax;
    public int CostLeft;
    public ServerGameManager MyGameManager;
    public ServerHandManager MyHandManager;
    public ServerCardDeckManager MyCardDeckManager;
    public ServerBattleGroundManager MyBattleGroundManager;

    public ServerPlayer(int clientId, int costMax, int costLeft) : base(costMax, costLeft)
    {
        ClientId = clientId;
        MyHandManager = new ServerHandManager();
        MyBattleGroundManager = new ServerBattleGroundManager();
    }
}