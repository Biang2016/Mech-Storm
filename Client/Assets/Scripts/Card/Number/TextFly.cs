using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TextFly : MonoBehaviour, IGameObjectPool
{
    private GameObjectPool gameObjectPool;

    public void PoolRecycle()
    {
        gameObjectPool.RecycleGameObject(gameObject);
        Anim.SetTrigger("EndFly");
    }

    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.Pool_TextFlyPool;
        Text.text = "";
        TextBG.text = "";
    }


    public void SetText(string text, Color color, int fontSize = 70)
    {
        Text.text = text;
        TextBG.text = text;
        Text.color = new Color(color.r, color.g, color.b, 1);
        TextBG.color = new Color(color.r / 1.5f, color.g / 1.5f, color.b / 1.5f, 1);
        Text.fontSize = fontSize;
        TextBG.fontSize = fontSize;
        Anim.SetTrigger("BeginFly");
        StartCoroutine(Fly());
    }

    IEnumerator Fly()
    {
        yield return new WaitForSeconds(0.3f);
        PoolRecycle();
    }

    [SerializeField] private Text Text;
    [SerializeField] private Text TextBG;
    [SerializeField] private Animator Anim;
}