using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

internal class Proxy : ProxyBase
{
    private Queue<ClientRequestBase> SendRequestsQueue = new Queue<ClientRequestBase>();

    protected override void Response()
    {
    }

    public override ClientStates ClientState
    {
        get { return clientState; }
        set
        {
            clientState = value;
            ClientLog.CL.PrintClientStates("Client states: " + ClientState);
        }
    }

    public Proxy(Socket socket, int clientId, bool isStopReceive) : base(socket, clientId, isStopReceive)
    {
    }


    #region 收发基础组件

    public void Send() //每帧调用
    {
        if (SendRequestsQueue.Count > 0)
        {
            ClientRequestBase request = SendRequestsQueue.Dequeue();
            Thread thread = new Thread(Client.CS.Send);
            thread.IsBackground = true;
            thread.Start(request);
        }
    }

    public void SendMessage(ClientRequestBase request)
    {
        SendRequestsQueue.Enqueue(request);
    }

    public void Response(Socket socket, RequestBase r)
    {
        ClientLog.CL.PrintReceive("Server: " + r.DeserializeLog());

        switch (r.GetProtocol())
        {
            #region OutGame

            case NetProtocols.CLIENT_ID_REQUEST:
            {
                ClientIdRequest request = (ClientIdRequest) r;
                ClientId = request.givenClientId;
                ClientState = ClientStates.GetId;
                break;
            }
            case NetProtocols.GAME_STOP_BY_LEAVE_REQUEST:
            {
                GameStopByLeaveRequest request = (GameStopByLeaveRequest) r;
                RoundManager.RM.OnGameStopByLeave(request);
                break;
            }

            #endregion

            #region OperationResponses

            case NetProtocols.GAME_START_RESPONSE:
            {
                GameStart_Response request = (GameStart_Response) r;
                foreach (ServerRequestBase requestSideEffect in request.SideEffects)
                {
                    RoundManager.RM.ResponseToSideEffects(requestSideEffect);
                }

                break;
            }

            case NetProtocols.END_ROUND_REQUEST_RESPONSE:
            {
                EndRoundRequest_Response request = (EndRoundRequest_Response) r;
                foreach (ServerRequestBase requestSideEffect in request.SideEffects)
                {
                    RoundManager.RM.ResponseToSideEffects(requestSideEffect);
                }

                break;
            }
            case NetProtocols.SUMMON_RETINUE_REQUEST_RESPONSE:
            {
                SummonRetinueRequest_Response request = (SummonRetinueRequest_Response) r;
                RoundManager.RM.OnPlayerSummonRetinue(request);
                foreach (ServerRequestBase requestSideEffect in request.SideEffects)
                {
                    RoundManager.RM.ResponseToSideEffects(requestSideEffect);
                }

                break;
            }
            case NetProtocols.RETINUE_ATTACK_RETINUE_REQUEST_RESPONSE:
            {
                RetinueAttackRetinueRequest_Response request = (RetinueAttackRetinueRequest_Response) r;
                RoundManager.RM.OnRetinueAttackRetinue(request);
                foreach (ServerRequestBase requestSideEffect in request.SideEffects)
                {
                    RoundManager.RM.ResponseToSideEffects(requestSideEffect);
                }

                break;
            }
            case NetProtocols.EQUIP_WEAPON_REQUEST_RESPONSE:
            {
                EquipWeaponRequest_Response request = (EquipWeaponRequest_Response) r;
                RoundManager.RM.OnPlayerEquipWeapon(request);
                foreach (ServerRequestBase requestSideEffect in request.SideEffects)
                {
                    RoundManager.RM.ResponseToSideEffects(requestSideEffect);
                }

                break;
            }
            case NetProtocols.EQUIP_SHIELD_REQUEST_RESPONSE:
            {
                EquipShieldRequest_Response request = (EquipShieldRequest_Response) r;
                RoundManager.RM.OnPlayerEquipShield(request);
                foreach (ServerRequestBase requestSideEffect in request.SideEffects)
                {
                    RoundManager.RM.ResponseToSideEffects(requestSideEffect);
                }

                break;
            }

            #endregion
        }
    }

    #endregion

    public void CancelMatch()
    {
        CancelMatchRequest request = new CancelMatchRequest(ClientId);
        SendMessage(request);
        ClientState = ClientStates.SubmitCardDeck;
    }

    public void LeaveGame()
    {
        LeaveGameRequest request = new LeaveGameRequest();
        SendMessage(request);
        ClientState = ClientStates.SubmitCardDeck;
    }

    public void OnSendCardDeck(CardDeckInfo cardDeckInfo)
    {
        CardDeckRequest req = new CardDeckRequest(ClientId, cardDeckInfo);
        SendMessage(req);
        ClientState = ClientStates.SubmitCardDeck;
    }

    public void OnBeginMatch()
    {
        MatchRequest req = new MatchRequest(ClientId);
        SendMessage(req);
        ClientState = ClientStates.Matching;
    }
}