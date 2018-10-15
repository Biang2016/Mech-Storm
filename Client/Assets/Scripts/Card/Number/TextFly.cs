using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextFly : PoolObject
{
    public override void PoolRecycle()
    {
        if (removeTextFlyHandler != null) removeTextFlyHandler(this);
        transform.localScale = Vector3.one;
        base.PoolRecycle();
        transform.position = Vector3.zero;
        Anim.SetTrigger("EndFly");
    }

    void Awake()
    {
        Text.text = "";
        TextBG.text = "";
    }

    public delegate void RemoveTextFlyHandler(TextFly textFly);

    public RemoveTextFlyHandler removeTextFlyHandler;


    public void SetText(string text, Color textColor, Color arrowColor, FlyDirection flyDirection, float duration,bool showArrow=true)
    {
        Text.text = text;
        TextBG.text = text;
        Text.color = new Color(textColor.r, textColor.g, textColor.b, 1);
        TextBG.color = new Color(textColor.r / 1.5f, textColor.g / 1.5f, textColor.b / 1.5f, 1);
        UpArrow.color = arrowColor;
        DownArrow.color = arrowColor;

        UpArrow.gameObject.SetActive(showArrow);
        DownArrow.gameObject.SetActive(showArrow);

        if (flyDirection == FlyDirection.Up)
        {
            Anim.SetTrigger("BeginFly");
            float duration_ori = ClientUtils.GetClipLength(Anim, "TextJump");
            Anim.speed = Anim.speed * duration_ori / duration;
        }
        else if (flyDirection == FlyDirection.Down)
        {
            Anim.SetTrigger("BeginFlyDown");
            float duration_ori = ClientUtils.GetClipLength(Anim, "TextDownJump");
            Anim.speed = Anim.speed * duration_ori / duration;
        }

        Canvas.overrideSorting = true;
        Canvas.sortingOrder = 10;

        StartCoroutine(Fly(duration));
    }

    IEnumerator Fly(float duration)
    {
        yield return new WaitForSeconds(duration - 0.01f);
        PoolRecycle();
    }

    [SerializeField] private Text Text;
    [SerializeField] private Text TextBG;
    [SerializeField] private Image UpArrow;
    [SerializeField] private Image DownArrow;
    [SerializeField] private Animator Anim;
    [SerializeField] private Canvas Canvas;

    public enum FlyDirection
    {
        Up,
        Down,
        None,
    }
}