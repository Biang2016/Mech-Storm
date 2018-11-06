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
        Button.enabled = true;
        Button.interactable = true;
        interactable = true;
        Anim.Play(firstAnimClipName);
        Anim.Update(0);
    }

    void Awake()
    {
        firstAnimClipName = Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }

    [SerializeField] private Button Button;
    [SerializeField] private Image Image;
    [SerializeField] private Material ImageGrayMaterial;
    [SerializeField] private Material UIDefault;
    private string firstAnimClipName;
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
        AudioManager.Instance.SoundPlay("sfx/StoryButtonOnHover");
        Anim.SetBool("Hover", true);
    }

    public void OnExit()
    {
        if (!interactable) return;
        Anim.SetBool("Hover", false);
    }

    private bool interactable = true;

    public void OnBeated()
    {
        BeBright();
        Anim.enabled = true;
        Anim.SetTrigger("Beat");
        Button.onClick.RemoveAllListeners();
        Button.interactable = false;
        Button.enabled = false;
        interactable = false;
    }

    public void OnDisabled()
    {
        BeDim();
        Anim.enabled = true;
        Anim.SetTrigger("Disabled");
        Button.onClick.RemoveAllListeners();
        Button.interactable = false;
        Button.enabled = false;
        interactable = false;
    }

    public void SetUnknown()
    {
        ClientUtils.ChangePicture(Image, 1000);
        Button.onClick.RemoveAllListeners();
    }

    public int M_CurrentLevelID;
    public int M_CurrentBossID;

    public void SetKnown()
    {
        ClientUtils.ChangePicture(Image, M_BossInfo.PicID);
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(delegate { StartMenuManager.Instance.StartGameCore(true, M_CurrentLevelID, M_CurrentBossID); });
        Button.onClick.AddListener(delegate { AudioManager.Instance.SoundPlay("sfx/OnStoryButtonClick"); });
    }
}