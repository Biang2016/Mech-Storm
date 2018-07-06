using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ServerGameMatchManager
{
    HashSet<int> preparingClientIds = new HashSet<int>();

    Queue<int> matchingClientIds = new Queue<int>();
    public List<ServerGameManager> SMGS = new List<ServerGameManager>();
    public Dictionary<int, ServerGameManager> PlayerGamesDictionary = new Dictionary<int, ServerGameManager>();


    public void AddClient(int clientId)
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

    public void PlayerReady(int clientId)
    {
        if (preparingClientIds.Contains(clientId))
        {
            return;
        }
        else
        {
            preparingClientIds.Remove(clientId)
            matchingClientIds.Enqueue(clientId);
            if (matchingClientIds.Count == 2)
            {
                Debug.Log("Add Player Success:" + clientId);
                int playerA = matchingClientIds.Dequeue();
                int playerB = matchingClientIds.Dequeue();
                ServerGameManager sgm = new ServerGameManager(playerA, playerB);
                SMGS.Add(sgm);
                sgm.StartGame();
                PlayerGamesDictionary.Add(playerB, sgm);
                PlayerGamesDictionary.Add(playerB, sgm);
            }
        }
    }

    public void ReceivePlayerCardDeckInfo(int clientId, CardDeckInfo cardDeckInfo)
    {
        ClientAndCardDeckInfo ccd = new ClientAndCardDeckInfo(clientId, cardDeckInfo);

            PlayerGamesDictionary[clientId].SetPlayerCardDeckInfo(clientId, cardDeckInfo);
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