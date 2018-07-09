using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

class ServerGameMatchManager
{
    public List<ServerGameManager> SMGS = new List<ServerGameManager>();
    HashSet<int> preparingClientIds = new HashSet<int>(); //正在准备中，已提交ClientID的玩家
    Queue<ClientAndCardDeckInfo> matchingClientIds = new Queue<ClientAndCardDeckInfo>(); //匹配中的玩家
    Dictionary<int, ClientAndCardDeckInfo> ClientsDecksDict = new Dictionary<int, ClientAndCardDeckInfo>(); //玩家ClientID和玩家具体信息的映射
    public Dictionary<int, ServerGameManager> PlayerGamesDictionary = new Dictionary<int, ServerGameManager>(); //玩家和游戏的映射关系

    public void OnClientRegister(int clientId)
    {
        if (preparingClientIds.Contains(clientId))
        {
            return;
        }
        else
        {
            preparingClientIds.Add(clientId);
        }
    }

    public void OnReceiveCardDeckInfo(int clientId, CardDeckInfo cardDeckInfo)
    {
        ClientAndCardDeckInfo ccd = new ClientAndCardDeckInfo(clientId, cardDeckInfo);
        if (ClientsDecksDict.ContainsKey(clientId))
        {
            ClientsDecksDict[clientId] = ccd;
        }
        else
        {
            ClientsDecksDict.Add(clientId, ccd);
        }

        ServerInfoRequest request = new ServerInfoRequest(InfoNumbers.INFO_SEND_CLIENT_CARDDECK_SUC);
        Server.SV.SendMessageToClientId(request, clientId);
    }

    public void OnClientMatchGames(int clientId)
    {
        if (!preparingClientIds.Contains(clientId))
        {
            ServerWarningRequest request = new ServerWarningRequest(WarningNumbers.WARNING_NO_CLIENT_ID);
            Server.SV.SendMessageToClientId(request, clientId);
        }
        else if (!ClientsDecksDict.ContainsKey(clientId))
        {
            ServerWarningRequest request = new ServerWarningRequest(WarningNumbers.WARNING_NO_CLIENT_CARDDECK);
            Server.SV.SendMessageToClientId(request, clientId);
        }
        else
        {
            ServerInfoRequest request = new ServerInfoRequest(InfoNumbers.INFO_IS_MATCHING);
            Server.SV.SendMessageToClientId(request, clientId);
            preparingClientIds.Remove(clientId);
            matchingClientIds.Enqueue(ClientsDecksDict[clientId]);
            if (matchingClientIds.Count == 2)
            {
                ServerLog.Print("Add Player Success:" + clientId);
                ClientAndCardDeckInfo playerA = matchingClientIds.Dequeue();
                ClientAndCardDeckInfo playerB = matchingClientIds.Dequeue();
                ServerGameManager sgm = new ServerGameManager(playerA, playerB);
                SMGS.Add(sgm);
                PlayerGamesDictionary.Add(playerA.ClientId, sgm);
                PlayerGamesDictionary.Add(playerB.ClientId, sgm);

                GameStateRequest r2 = new GameStateRequest();
                Server.SV.SendMessageToClientId(r2, playerA.ClientId);
                Server.SV.SendMessageToClientId(r2, playerB.ClientId);
            }
        }
    }

    public void TryInitialized(int clientId)
    {
        if (PlayerGamesDictionary.ContainsKey(clientId) && PlayerGamesDictionary[clientId] != null)
        {
            PlayerGamesDictionary[clientId].TryInitialized(clientId);
        }
    }

    public void TryGameBegin(int clientId)
    {
        if (PlayerGamesDictionary.ContainsKey(clientId) && PlayerGamesDictionary[clientId] != null)
        {
            PlayerGamesDictionary[clientId].TryGameBegin(clientId);
        }
    }

    //下发结束回合请求
    public void TryEndRound(int clientId)
    {
        if (PlayerGamesDictionary.ContainsKey(clientId) && PlayerGamesDictionary[clientId] != null)
        {
            PlayerGamesDictionary[clientId].EndRound();
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