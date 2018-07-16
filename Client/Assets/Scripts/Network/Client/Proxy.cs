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
        //Client.CS.ClientProtoManager.AddRequestDelegate(NetProtocols.HEART_BEAT, Response);
        Client.CS.ClientProtoManager.AddRequestDelegate(NetProtocols.SEND_CLIENT_ID, Response_ClientIdRequest);
        Client.CS.ClientProtoManager.AddRequestDelegate(NetProtocols.GAME_STOP_BY_LEAVE, Response_GameStopByLeaveRequest);

        Client.CS.ClientProtoManager.AddRequestDelegate(NetProtocols.PLAYER, Response_PlayerRequest);
        Client.CS.ClientProtoManager.AddRequestDelegate(NetProtocols.PLAYER_TURN, Response_PlayerTurnRequest);
        Client.CS.ClientProtoManager.AddRequestDelegate(NetProtocols.DRAW_CARD, Response_DrawCardRequest);
        Client.CS.ClientProtoManager.AddRequestDelegate(NetProtocols.PLAYER_COST_CHANGE, Response_PlayerCostRequest);

        Client.CS.ClientProtoManager.AddRequestDelegate(NetProtocols.SUMMON_RETINUE_RESPONSE, Response_SummonRetinueRequest_R);
        Client.CS.ClientProtoManager.AddRequestDelegate(NetProtocols.EQUIP_WEAPON_RESPONSE, Response_EquipWeaponRequest_R);
        Client.CS.ClientProtoManager.AddRequestDelegate(NetProtocols.EQUIP_SHIELD_RESPONSE, Response_EquipShieldRequest_R);

        Client.CS.ClientProtoManager.AddRequestDelegate(NetProtocols.RETINUE_ATTACK_RETINUE_RESPONSE, Response_RetinueAttackRetinueRequest_R);

        Client.CS.ClientProtoManager.AddRequestDelegate(NetProtocols.RETINUE_ATTRIBUTES_CHANGE, Response_RetinueAttributesRequest);
        Client.CS.ClientProtoManager.AddRequestDelegate(NetProtocols.WEAPON_ATTRIBUTES_CHANGE, Response_WeaponAttributesRequest);
        Client.CS.ClientProtoManager.AddRequestDelegate(NetProtocols.SHIELD_ATTRIBUTES_CHANGE, Response_ShieldAttributesRequest);
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

    #endregion

    private void Response_ClientIdRequest(Socket socket, object obj)
    {
        ClientIdRequest request = (ClientIdRequest) obj;
        ClientId = request.givenClientId;
        ClientState = ClientStates.GetId;
    }

    private void Response_PlayerRequest(Socket socket, object obj)
    {
        ClientState = ClientStates.Playing;
        NetworkManager.NM.SuccessMatched();
        RoundManager.RM.Initialize();
        RoundManager.RM.InitializePlayers((PlayerRequest) obj);
    }

    private void Response_PlayerTurnRequest(Socket socket, object obj)
    {
        ExcuteResponse(RoundManager.RM.SetPlayerTurn, obj);
    }

    private void Response_PlayerCostRequest(Socket socket, object obj)
    {
        ExcuteResponse(RoundManager.RM.SetPlayersCost, obj);
    }

    private void Response_DrawCardRequest(Socket socket, object obj)
    {
        ExcuteResponse(RoundManager.RM.OnPlayerDrawCard, obj);
    }

    private void Response_SummonRetinueRequest_R(Socket socket, object obj)
    {
        ExcuteResponse(RoundManager.RM.OnPlayerSummonRetinue, obj);
    }

    private void Response_EquipWeaponRequest_R(Socket socket, object obj)
    {
        ExcuteResponse(RoundManager.RM.OnPlayerEquipWeapon, obj);
    }

    private void Response_EquipShieldRequest_R(Socket socket, object obj)
    {
        ExcuteResponse(RoundManager.RM.OnPlayerEquipShield, obj);
    }

    private void Response_RetinueAttackRetinueRequest_R(Socket socket, object obj)
    {
        ExcuteResponse(RoundManager.RM.OnRetinueAttackRetinue, obj);
    }

    private void Response_WeaponAttributesRequest(Socket socket, object obj)
    {
        ExcuteResponse(RoundManager.RM.OnWeaponAttributesChange, obj);
    }

    private void Response_ShieldAttributesRequest(Socket socket, object obj)
    {
        ExcuteResponse(RoundManager.RM.OnShieldAttributesChange, obj);
    }

    private void Response_RetinueAttributesRequest(Socket socket, object obj)
    {
        ExcuteResponse(RoundManager.RM.OnRetinueAttributesChange, obj);
    }

    private void Response_GameStopByLeaveRequest(Socket socket, object obj)
    {
        ServerRequestBase r = (ServerRequestBase) obj;
        ClientLog.CL.PrintReceive("Server：[" + r.GetProtocolName() + "]    " + r.DeserializeLog());
        GameStopByLeaveRequest request = (GameStopByLeaveRequest) r;
        if (request.clientId == ClientId)
        {
            ClientLog.CL.PrintClientStates("你 " + request.clientId + " 退出了比赛");
        }
        else
        {
            ClientLog.CL.PrintReceive("你的对手 " + request.clientId + " 退出了比赛");
        }

        RoundManager.RM.StopGame();
        ClientState = ClientStates.SubmitCardDeck;
    }

    private static void ExcuteResponse(BattleEffectsManager.BattleResponse responseMethod, object obj)
    {
        ServerRequestBase request = (ServerRequestBase) obj;
        ClientLog.CL.PrintReceive("Server：[" + request.GetProtocolName() + "]    " + request.DeserializeLog());
        BattleEffectsManager.BEM.ResponseExcuteQueue.Enqueue(new BattleEffectsManager.ResponseAndMethod(responseMethod, request));
    }

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