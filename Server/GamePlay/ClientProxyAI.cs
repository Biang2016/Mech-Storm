using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

/// <summary>
/// 为了兼容联机对战模式，单人模式的AI也用一个ClientProxy来处理逻辑
/// AI不需要任何Socket有关的功能
/// 固定PlayerA为玩家，PlayerB为AI
/// </summary>
internal class ClientProxyAI : ClientProxy
{
    public override ClientStates ClientState
    {
        get => clientState;
        set => clientState = value;
    }

    public ClientProxyAI(int clientId, bool isStopReceive) : base(null, clientId, isStopReceive)
    {
        ClientIdRequest request = new ClientIdRequest(clientId);
        ClientState = ClientStates.GetId;
        SendMessage(request);
    }

    public override void SendMessage(ServerRequestBase request)
    {
        if (request is EndRoundRequest_ResponseBundle r)
        {
            foreach (ServerRequestBase req in r.AttachedRequests)
            {
                if (req is PlayerTurnRequest ptr)
                {
                    if (ptr.clientId == ClientId)
                    {
                        AIOperation();
                    }
                }
            }
        }
    }

    public override void ReceiveMessage(ClientRequestBase request)
    {
    }

    protected override void Response()
    {
    }

    private void AIOperation()
    {
        ServerCardBase[] cards = MyServerGameManager.PlayerB.MyHandManager.Cards.ToArray();

        for (int i = 0; i < cards.Length; i++)
        {
            ServerCardBase card = cards[i];
            if (card.Usable && (card.CardInfo.BaseInfo.CardType == CardTypes.Spell || card.CardInfo.BaseInfo.CardType == CardTypes.Energy))
            {
                if (card.CardInfo.TargetInfo.HasNoTarget)
                {
                    MyServerGameManager.OnClientUseSpellCardRequest(new UseSpellCardRequest(ClientId, card.M_CardInstanceId));
                }

                continue;
            }
        }

        MyServerGameManager.OnEndRoundRequest(new EndRoundRequest(ClientId));
    }
}