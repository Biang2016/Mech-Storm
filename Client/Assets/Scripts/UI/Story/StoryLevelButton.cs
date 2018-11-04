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
        Anim.enabled = true;
        Anim.SetTrigger("Hide");
        Button.interactable = true;
        Button.enabled = true;
        interactable = true;
    }

    void Awake()
    {
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
        if (!interactable) return;
        Image.material = ImageGrayMaterial;
    }

    public void BeBright()
    {
        if (!interactable) return;
        Image.material = UIDefault;
    }

    public void OnHover()
    {
        if (!interactable) return;
        AudioManager.Instance.SoundPlay("sfx/OnHoverStoryButton");
    }

    private bool interactable = true;

    public void OnBeated()
    {
        BeBright();
        Anim.SetTrigger("Beat");
        Button.image.color = ClientUtils.HTMLColorToColor("#97FF7E");
        Button.onClick.RemoveAllListeners();
        Button.interactable = false;
        Button.enabled = false;
        Anim.enabled = false;
        interactable = false;
    }

    public void OnDisabled()
    {
        BeDim();
        Anim.SetTrigger("Disabled");
        Button.onClick.RemoveAllListeners();
        Button.interactable = false;
        Button.enabled = false;
        Anim.enabled = false;
        interactable = false;
    }

    public void SetUnknown()
    {
        ClientUtils.ChangePicture(Image, 1000);
        Button.onClick.RemoveAllListeners();
    }

    public void SetKnown()
    {
        ClientUtils.ChangePicture(Image, M_BossInfo.PicID);
        Button.onClick.AddListener(delegate { OnButtonClick(); });
    }

    public delegate void StoryButtonClickHandler();

    public StoryButtonClickHandler OnButtonClick;
}