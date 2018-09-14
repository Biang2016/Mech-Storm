using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TextFlyPile : MonoBehaviour
{
    void Awake()
    {
    }

    private void ContinueFly()
    {
        foreach (TextFly fly in TextFlies)
        {
            fly.transform.Translate(Vector3.forward);
        }
    }

    public List<TextFly> TextFlies = new List<TextFly>();

    public void SetText(string text, string textColor, string arrowColor, TextFly.FlyDirection flyDirection, int fontSize = 70)
    {
        TextFly textFly = GameObjectPoolManager.Instance.Pool_TextFlyPool.AllocateGameObject(transform).GetComponent<TextFly>();
        textFly.SetText(text, ClientUtils.HTMLColorToColor(textColor), ClientUtils.HTMLColorToColor(arrowColor), flyDirection, fontSize);
        ContinueFly();
        TextFlies.Add(textFly);
    }
}