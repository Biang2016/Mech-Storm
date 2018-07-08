using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

class ClientLog : MonoBehaviour
{
    private static ClientLog _cl;

    public static ClientLog CL
    {
        get
        {
            return _cl;
        }
    }

    public Text LogText;

    void Awake()
    {
        _cl = FindObjectOfType<ClientLog>();
        LogText.text = "";
        LogMessages = new Queue<string>();
    }

    Queue<string> LogMessages;
    public void Print(string log)
    {
        LogMessages.Enqueue(log);
    }

    private void Update()
    {
        while (LogMessages.Count > 0)
        {
            LogText.text += LogMessages.Dequeue() + "\n";
        }
    }
}