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
        foreach (var fly in TextFlies)
        {
            fly.transform.Translate(Vector3.forward);
        }
    }

    public List<TextFly> TextFlies = new List<TextFly>();

    public void SetText(string text, Color color)
    {
        TextFly textFly = GameObjectPoolManager.Instance.Pool_TextFlyPool.AllocateGameObject(transform).GetComponent<TextFly>();
        textFly.SetText("+" + text, ClientUtils.HTMLColorToColor("#92FF00"));
        ContinueFly();
        TextFlies.Add(textFly);
    }
}