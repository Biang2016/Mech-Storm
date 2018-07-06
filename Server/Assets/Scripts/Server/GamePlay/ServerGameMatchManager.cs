using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ServerGameMatchManager
{
    Queue<int> clientIds = new Queue<int>();
    public List<ServerGameManager> SMGS = new List<ServerGameManager>();

    public void AddClient(Response response)
    {
        EntryGameResponse r = (EntryGameResponse) response;
        if (clientIds.Contains(r.clientId))
        {
            return;
        }
        else
        {
            clientIds.Enqueue(r.clientId);
            if (clientIds.Count == 2)
            {
                Debug.Log("Add Player Success:" + r.clientId);
                ServerGameManager sgm = new ServerGameManager(clientIds.Dequeue(), clientIds.Dequeue());
                SMGS.Add(sgm);
                sgm.StartGame();
            }
        }
    }
}