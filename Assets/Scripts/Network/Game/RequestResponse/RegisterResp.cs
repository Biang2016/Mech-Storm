using UnityEngine;
using System.Collections;

public class RegisterResp
{
    //所有response必须要在这里注册
    public static void RegisterAll()
    {
        Server.SV.ServerProtoManager.AddProtocol<EntryGameResponse>(NetProtocols.ENTRY_GAME);
        Client.CS.ClientProtoManager.AddProtocol<EntryGameResponse>(NetProtocols.ENTRY_GAME);
        Server.SV.ServerProtoManager.AddProtocol<TestConnectResponse>(NetProtocols.TEST_CONNECT);
        Client.CS.ClientProtoManager.AddProtocol<TestConnectResponse>(NetProtocols.TEST_CONNECT);
    }
}