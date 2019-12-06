using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalServer : MonoBehaviour
{

    [SerializeField] private DebugConsole DebugConsole;
    void Start()
    {
        DebugConsole.visible = true;

        ServerLog.Instance.LodDelegate = Debug.Log;
        ServerLog.Instance.LogVerbosity = LogVerbosity.All;
        ServerLog.Instance.Print("SERVER START");
        Server.SV = new Server("127.0.0.1", 9999);

        ServerLog.Instance.Print("ServerVersion: " + Server.ServerVersion);
        Server.SV.Start();
    }
}