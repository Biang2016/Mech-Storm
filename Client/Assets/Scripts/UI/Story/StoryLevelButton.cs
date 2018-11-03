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
        Button.onClick.AddListener(delegate { OnButtonClick(); });
    }

    [SerializeField] private Button Button;
    [SerializeField] private Image Image;
    [SerializeField] private Material ImageGrayMaterial;
    [SerializeField] private Material UIDefault;
    [SerializeField] private Animator Anim;


    public Boss M_BossInfo;

    public void Initialize(Boss bossInfo)
    {
        M_BossInfo = bossInfo;
        ClientUtils.ChangePicture(Image, bossInfo.PicID);
        Anim.SetTrigger("Born");
        BeDim();
    }

    public void BeDim()
    {
        Image.material = ImageGrayMaterial;
    }

    public void BeBright()
    {
        Image.material = UIDefault;
    }

    public void OnHover()
    {
        AudioManager.Instance.SoundPlay("sfx/OnHoverStoryButton");
    }

    public delegate void StoryButtonClickHandler();

    public StoryButtonClickHandler OnButtonClick;
}