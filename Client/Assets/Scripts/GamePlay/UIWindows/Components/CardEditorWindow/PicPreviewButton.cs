using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PicPreviewButton : PoolObject
{
    private PicPreviewButton()
    {
    }

    public override void PoolRecycle()
    {
        base.PoolRecycle();
    }

    [SerializeField] private Button Button;
    [SerializeField] private Image PicImage;
    [SerializeField] private Text PicIDText;

    public void Initialize(Sprite sprite, UnityAction onClick)
    {
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(onClick);
        PicImage.sprite = sprite;
        PicIDText.text = sprite.name.Replace("(Clone)", "");
    }
}