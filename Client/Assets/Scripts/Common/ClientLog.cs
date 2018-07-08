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
    }


    public void Print(string log)
    {
        LogText.text += log + "\n";
    }
}