using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

public static class ProtoManager
{
    private static Dictionary<int, Func<DataStream, RequestBase>> mProtocolMapping;

    public delegate void requestDelegate(Socket socket, RequestBase requestBase);

    private static Dictionary<int, List<requestDelegate>> mDelegateMapping;

    static ProtoManager()
    {
        mProtocolMapping = new Dictionary<int, Func<DataStream, RequestBase>>();
        mDelegateMapping = new Dictionary<int, List<requestDelegate>>();

        #region  OutGame

        #region Server

        AddProtocol<ClientIdRequest>(NetProtocols.CLIENT_ID_REQUEST);
        AddProtocol<HeartBeatRequest>(NetProtocols.HEART_BEAT_REQUEST);
        AddProtocol<GameStopByLeaveRequest>(NetProtocols.GAME_STOP_BY_LEAVE_REQUEST);

        #endregion

        #region Client

        AddProtocol<CardDeckRequest>(NetProtocols.CARD_DECK_REQUEST);
        AddProtocol<MatchRequest>(NetProtocols.MATCH_REQUEST);
        AddProtocol<CancelMatchRequest>(NetProtocols.CANCEL_MATCH_REQUEST);
        AddProtocol<LeaveGameRequest>(NetProtocols.LEAVE_GAME_REQUEST);

        #endregion

        #endregion

        #region InGame

        #region Server

        AddProtocol<GameStart_Response>(NetProtocols.GAME_START_RESPONSE);
        AddProtocol<SetPlayerRequest>(NetProtocols.SE_SET_PLAYER);
        AddProtocol<PlayerTurnRequest>(NetProtocols.SE_PLAYER_TURN);
        AddProtocol<DrawCardRequest>(NetProtocols.SE_DRAW_CARD);
        AddProtocol<PlayerCostChangeRequest>(NetProtocols.SE_PLAYER_COST_CHANGE);

        AddProtocol<SummonRetinueRequest_Response>(NetProtocols.SUMMON_RETINUE_REQUEST_RESPONSE);
        AddProtocol<EquipWeaponRequest_Response>(NetProtocols.EQUIP_WEAPON_REQUEST_RESPONSE);
        AddProtocol<EquipShieldRequest_Response>(NetProtocols.EQUIP_SHIELD_REQUEST_RESPONSE);

        AddProtocol<RetinueAttackRetinueRequest_Response>(NetProtocols.RETINUE_ATTACK_RETINUE_REQUEST_RESPONSE);

        AddProtocol<WeaponAttributesChangeRequest>(NetProtocols.SE_WEAPON_ATTRIBUTES_CHANGE);
        AddProtocol<ShieldAttributesChangeRequest>(NetProtocols.SE_SHIELD_ATTRIBUTES_CHANGE);
        AddProtocol<RetinueAttributesChangeRequest>(NetProtocols.SE_RETINUE_ATTRIBUTES_CHANGE);

        AddProtocol<EndRoundRequest_Response>(NetProtocols.END_ROUND_REQUEST_RESPONSE);

        #endregion

        #region Client

        AddProtocol<SummonRetinueRequest>(NetProtocols.SUMMON_RETINUE_REQUEST);
        AddProtocol<EquipWeaponRequest>(NetProtocols.EQUIP_WEAPON_REQUEST);
        AddProtocol<EquipShieldRequest>(NetProtocols.EQUIP_SHIELD_REQUEST);

        AddProtocol<RetinueAttackRetinueRequest>(NetProtocols.RETINUE_ATTACK_RETINUE_REQUEST);

        AddProtocol<EndRoundRequest>(NetProtocols.END_ROUND_REQUEST);

        #endregion

        #endregion
    }


    public static void AddProtocol<T>(int protocol) where T : RequestBase, new()
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
    public static void AddRequestDelegate(int protocol, requestDelegate d)
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

    public static void DelRespDelegate(int protocol, requestDelegate d)
    {
        if (mDelegateMapping.ContainsKey(protocol))
        {
            List<requestDelegate> dels = mDelegateMapping[protocol];
            dels.Remove(d);
        }
    }

    public static RequestBase TryDeserialize(DataStream stream, Socket socket)
    {
        int protocol = stream.ReadSInt32();
        RequestBase requestBase = null;
        if (mProtocolMapping.ContainsKey(protocol))
        {
            requestBase = mProtocolMapping[protocol](stream);
            if (requestBase != null && socket != null)
            {
                if (mDelegateMapping.ContainsKey(protocol))
                {
                    List<requestDelegate> dels = mDelegateMapping[protocol];
                    foreach (requestDelegate del in dels)
                    {
                        del(socket, requestBase);
                    }
                }
            }
        }
        else
        {
            throw new Exception("no register protocol : " + protocol + "!please reg to RegisterResp.");
        }

        return requestBase;
    }
}