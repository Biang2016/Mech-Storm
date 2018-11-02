using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class StoryLevelButton : PoolObject
{
    public override void PoolRecycle()
    {
        base.PoolRecycle();
    }

    void Awake()
    {
    }

    [SerializeField] private Button Button;
    [SerializeField] private Image Image;
    [SerializeField] private Animator Anim;

    private Color Ori_Color;

    public void Initialize(Color color, int picID)
    {
        Ori_Color = color;
        Button.image.color = color;
        ClientUtils.ChangePicture(Image, picID);
        Anim.SetTrigger("Born");
    }
}