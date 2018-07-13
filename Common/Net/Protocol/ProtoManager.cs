using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

public class ProtoManager
{
    private Dictionary<int, Func<DataStream, Request>> mProtocolMapping;

    public delegate void requestDelegate(Socket socket, Request request);

    private Dictionary<int, List<requestDelegate>> mDelegateMapping;

    public ProtoManager()
    {
        mProtocolMapping = new Dictionary<int, Func<DataStream, Request>>();
        mDelegateMapping = new Dictionary<int, List<requestDelegate>>();

        AddProtocol<CardDeckRequest>(NetProtocols.CARD_DECK_INFO);
        AddProtocol<EndRoundRequest>(NetProtocols.END_ROUND);
        AddProtocol<LeaveGameRequest>(NetProtocols.LEAVE_GAME);
        AddProtocol<MatchRequest>(NetProtocols.Match);
        AddProtocol<CancelMatchRequest>(NetProtocols.CANCEL_MATCH);
        AddProtocol<SummonRetinueRequest>(NetProtocols.SUMMON_RETINUE);

        AddProtocol<ClientIdRequest>(NetProtocols.SEND_CLIENT_ID);
        AddProtocol<DrawCardRequest>(NetProtocols.DRAW_CARD);
        AddProtocol<GameStopByLeaveRequest>(NetProtocols.GAME_STOP_BY_LEAVE);
        AddProtocol<PlayerCostRequest>(NetProtocols.PLAYER_COST_CHANGE);
        AddProtocol<PlayerRequest>(NetProtocols.PLAYER);
        AddProtocol<PlayerTurnRequest>(NetProtocols.PLAYER_TURN);
        AddProtocol<SummonRetinueRequest_Response>(NetProtocols.SUMMON_RETINUE_RESPONSE);
    }


    public void AddProtocol<T>(int protocol) where T : Request, new()
    {
        if (mProtocolMapping.ContainsKey(protocol))
        {
            mProtocolMapping.Remove(protocol);
        }

        mProtocolMapping.Add(protocol,
            (stream) =>
            {
                T data = new T();
                data.Deserialize(stream);
                return data;
            });
    }

    /// <summary>
    /// 添加代理，在接受到数据时会下发数据
    /// </summary>
    /// <param name="protocol">Protocol.</param>
    /// <param name="d">D.</param>
    public void AddRequestDelegate(int protocol, requestDelegate d)
    {
        List<requestDelegate> dels;
        if (mDelegateMapping.ContainsKey(protocol))
        {
            dels = mDelegateMapping[protocol];
            for (int i = 0; i < dels.Count; i++)
            {
                if (dels[i] == d)
                {
                    return;
                }
            }
        }
        else
        {
            dels = new List<requestDelegate>();
            mDelegateMapping.Add(protocol, dels);
        }

        dels.Add(d);
    }

    public void DelRespDelegate(int protocol, requestDelegate d)
    {
        if (mDelegateMapping.ContainsKey(protocol))
        {
            List<requestDelegate> dels = mDelegateMapping[protocol];
            dels.Remove(d);
        }
    }

    public Request TryDeserialize(byte[] data, Socket socket)
    {
        DataStream stream = new DataStream(data, true);

        int protocol = stream.ReadSInt32();
        Request request = null;
        if (mProtocolMapping.ContainsKey(protocol))
        {
            request = mProtocolMapping[protocol](stream);
            if (request != null)
            {
                if (mDelegateMapping.ContainsKey(protocol))
                {
                    List<requestDelegate> dels = mDelegateMapping[protocol];
                    foreach (requestDelegate del in dels)
                    {
                        del(socket, request);
                    }
                }
            }
        }
        else
        {
            throw new Exception("no register protocol : " + protocol + "!please reg to RegisterResp.");
        }

        return request;
    }
}