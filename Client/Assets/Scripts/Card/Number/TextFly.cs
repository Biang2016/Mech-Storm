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
        transform.position = Vector3.zero;
        Anim.SetTrigger("EndFly");
    }

    void Awake()
    {
        gameObjectPool = GameObjectPoolManager.Instance.Pool_TextFlyPool;
        Text.text = "";
        TextBG.text = "";
    }


    public void SetText(string text, Color textColor, Color arrowColor, FlyDirection flyDirection, int fontSize = 70)
    {
        Text.text = text;
        TextBG.text = text;
        Text.color = new Color(textColor.r, textColor.g, textColor.b, 1);
        TextBG.color = new Color(textColor.r / 1.5f, textColor.g / 1.5f, textColor.b / 1.5f, 1);
        UpArrow.color = arrowColor;
        DownArrow.color = arrowColor;
        Text.fontSize = fontSize;
        TextBG.fontSize = fontSize;
        if (flyDirection == FlyDirection.Up)
        {
            Anim.SetTrigger("BeginFly");
        }
        else if (flyDirection == FlyDirection.Down)
        {
            Anim.SetTrigger("BeginFlyDown");
        }

        StartCoroutine(Fly());
    }

    IEnumerator Fly()
    {
        yield return new WaitForSeconds(0.3f);
        PoolRecycle();
    }

    [SerializeField] private Text Text;
    [SerializeField] private Text TextBG;
    [SerializeField] private Image UpArrow;
    [SerializeField] private Image DownArrow;
    [SerializeField] private Animator Anim;

    public enum FlyDirection
    {
        Up,
        Down,
    }
}