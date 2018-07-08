using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

public class ProtoManager
{
    private Dictionary<int, Func<DataStream, Response>> mProtocolMapping;

    public delegate void responseDelegate(Socket socket,Response response);

    private Dictionary<int, List<responseDelegate>> mDelegateMapping;

    public ProtoManager()
    {
        mProtocolMapping = new Dictionary<int, Func<DataStream, Response>>();
        mDelegateMapping = new Dictionary<int, List<responseDelegate>>();
    }


    public void AddProtocol<T>(int protocol) where T : Response, new()
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
    public void AddRespDelegate(int protocol, responseDelegate d)
    {
        List<responseDelegate> dels;
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
            dels = new List<responseDelegate>();
            mDelegateMapping.Add(protocol, dels);
        }

        dels.Add(d);
    }

    public void DelRespDelegate(int protocol, responseDelegate d)
    {
        if (mDelegateMapping.ContainsKey(protocol))
        {
            List<responseDelegate> dels = mDelegateMapping[protocol];
            dels.Remove(d);
        }
    }

    public Response TryDeserialize(ClientData clientData)
    {
        DataStream stream = new DataStream(clientData.DataHolder.mRecvData, true);

        int protocol = stream.ReadSInt32();
        Response response = null;
        if (mProtocolMapping.ContainsKey(protocol))
        {
            response = mProtocolMapping[protocol](stream);
            if (response != null)
            {
                if (mDelegateMapping.ContainsKey(protocol))
                {
                    List<responseDelegate> dels = mDelegateMapping[protocol];
                    foreach (responseDelegate del in dels)
                    {
                        del(clientData.Socket, response);
                    }
                }
            }
        }
        else
        {
            //Debug.Log("no register protocol : " + protocol + "!please reg to RegisterResp.");
        }

        return response;
    }
}

public class ClientData
{
    public Socket Socket;
    public int ClientId;
    public DataHolder DataHolder;
    public bool IsStopReceive;

    public ClientData(Socket socket, int clientId, DataHolder dataHolder, bool isStopReceive)
    {
        Socket = socket;
        ClientId = clientId;
        DataHolder = dataHolder;
        IsStopReceive = isStopReceive;
    }
}