using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

internal class ServerGameMatchManager
{
    List<ServerGameManager> SMGS = new List<ServerGameManager>();
    Dictionary<int, ServerGameManager> clientGameMapping = new Dictionary<int, ServerGameManager>(); //玩家和游戏的映射关系
    Queue<ClientProxy> matchingClients = new Queue<ClientProxy>(); //匹配中的玩家

    public void OnClientMatchGames(ClientProxy clientProxy)
    {
        matchingClients.Enqueue(clientProxy);
        if (matchingClients.Count == 2)
        {
            ServerLog.Print("Add Player Success:" + clientProxy.ClientId);
            ClientProxy playerA = matchingClients.Dequeue();
            ClientProxy playerB = matchingClients.Dequeue();
            ServerGameManager sgm = new ServerGameManager(playerA, playerB);
            SMGS.Add(sgm);
            clientGameMapping.Add(playerA.ClientId, sgm);
            clientGameMapping.Add(playerB.ClientId, sgm);
        }
    }

    public void KickOutClient()
    {
    }
}

public class ClientAndCardDeckInfo
{
    public int ClientId;
    public CardDeckInfo CardDeckInfo;

    public ClientAndCardDeckInfo(int clientId, CardDeckInfo cardDeckInfo)
    {
        ClientId = clientId;
        CardDeckInfo = cardDeckInfo;
    }
}