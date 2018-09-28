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

    public List<TextFly> TextFlies = new List<TextFly>();

    public void SetText(string text, string textColor, string arrowColor, TextFly.FlyDirection flyDirection, float duration = 1f)
    {
        TextFly textFly = GameObjectPoolManager.Instance.Pool_TextFlyPool.AllocateGameObject<TextFly>(transform);
        textFly.removeTextFlyHandler = OnRemoveTextFly;
        textFly.SetText(text, ClientUtils.HTMLColorToColor(textColor), ClientUtils.HTMLColorToColor(arrowColor), flyDirection,  duration);
        TextFlies.Add(textFly);
    }

    public void OnRemoveTextFly(TextFly textFly)
    {
        TextFlies.Remove(textFly);
    }
}