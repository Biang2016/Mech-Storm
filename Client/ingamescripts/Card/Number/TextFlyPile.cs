using System.Collections.Generic;
using UnityEngine;

public class TextFlyPile : MonoBehaviour
{
    void Awake()
    {
    }

    public List<TextFly> TextFlies = new List<TextFly>();

    public void SetText(string text, string textColor, string arrowColor, TextFly.FlyDirection flyDirection, float duration = 1f, bool showArrow = true)
    {
        TextFly textFly = GameObjectPoolManager.Instance.Pool_TextFlyPool.AllocateGameObject<TextFly>(transform);
        textFly.removeTextFlyHandler = OnRemoveTextFly;
        textFly.SetText(text, ClientUtils.HTMLColorToColor(textColor), ClientUtils.HTMLColorToColor(arrowColor), flyDirection, duration, showArrow);
        TextFlies.Add(textFly);
    }

    public void OnRemoveTextFly(TextFly textFly)
    {
        TextFlies.Remove(textFly);
    }
}