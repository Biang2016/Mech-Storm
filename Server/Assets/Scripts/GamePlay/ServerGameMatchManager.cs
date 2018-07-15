﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

internal class ServerGameMatchManager
{
    List<ServerGameManager> SMGS = new List<ServerGameManager>();
    Dictionary<int, ServerGameManager> clientGameMapping = new Dictionary<int, ServerGameManager>(); //玩家和游戏的映射关系
    List<ClientProxy> matchingClients = new List<ClientProxy>(); //匹配中的玩家

    public void OnClientMatchGames(ClientProxy clientProxy)
    {
        matchingClients.Add(clientProxy);
        ServerLog.PrintServerStates("玩家 " + clientProxy.ClientId + " 开始匹配. 当前 " + matchingClients.Count + " 人在匹配中");
        if (matchingClients.Count == 2)
        {
            ClientProxy clientA = matchingClients[0];
            ClientProxy clientB = matchingClients[1];
            matchingClients.Remove(clientA);
            matchingClients.Remove(clientB);
            ServerGameManager sgm = new ServerGameManager(clientA, clientB);
            SMGS.Add(sgm);
            clientGameMapping.Add(clientA.ClientId, sgm);
            clientGameMapping.Add(clientB.ClientId, sgm);
            ServerLog.PrintServerStates("玩家 " + clientA.ClientId + " 和 " + clientB.ClientId + " 开始游戏,当前 " + matchingClients.Count + " 人在游戏中");
        }
    }

    public void OnClientCancelMatch(ClientProxy clientProxy)
    {
        if (matchingClients.Contains(clientProxy))
        {
            matchingClients.Remove(clientProxy);
            ServerLog.PrintServerStates("玩家 " + clientProxy.ClientId + " 取消匹配. 当前 " + matchingClients.Count + " 人在匹配中");
        }
    }

    public void RemoveGame(ServerGameManager serverGameManager, ClientProxy clientA, ClientProxy clientB)
    {
        SMGS.Remove(serverGameManager);
        clientGameMapping.Remove(clientA.ClientId);
        clientGameMapping.Remove(clientB.ClientId);
        ServerLog.PrintServerStates("玩家 " + clientA.ClientId + " 和 " + clientB.ClientId + " 停止游戏,当前 " + matchingClients.Count + " 人在游戏中");
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