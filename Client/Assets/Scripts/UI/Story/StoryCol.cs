using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StoryCol : PoolObject
{
    public override void PoolRecycle()
    {
        foreach (StoryLevelButton slb in StoryLevelButtons)
        {
            slb.PoolRecycle();
        }
        StoryLevelButtons.Clear();
        base.PoolRecycle();
    }

    void Awake()
    {
        Links11 = new Image[1, 1];
        Links11[0, 0] = Link11_11;

        Links12 = new Image[1, 2];
        Links12[0, 0] = Link12_11;
        Links12[0, 1] = Link12_12;

        Links13 = new Image[1, 3];
        Links13[0, 0] = Link13_11;
        Links13[0, 1] = Link13_12;
        Links13[0, 2] = Link13_13;

        Links21 = new Image[2, 1];
        Links21[0, 0] = Link21_11;
        Links21[1, 0] = Link21_21;

        Links22 = new Image[2, 2];
        Links22[0, 0] = Link22_11;
        Links22[0, 1] = Link22_12;
        Links22[1, 0] = Link22_21;
        Links22[1, 1] = Link22_22;

        Links23 = new Image[2, 3];
        Links23[0, 0] = Link23_11;
        Links23[0, 1] = Link23_12;
        Links23[0, 2] = Link23_13;
        Links23[1, 0] = Link23_21;
        Links23[1, 1] = Link23_22;
        Links23[1, 2] = Link23_23;

        Links31 = new Image[3, 1];
        Links31[0, 0] = Link31_11;
        Links31[1, 0] = Link31_21;
        Links31[2, 0] = Link31_31;

        Links32 = new Image[3, 2];
        Links32[0, 0] = Link32_11;
        Links32[0, 1] = Link32_12;
        Links32[1, 0] = Link32_21;
        Links32[1, 1] = Link32_22;
        Links32[2, 0] = Link32_31;
        Links32[2, 1] = Link32_32;

        Links33 = new Image[3, 3];
        Links33[0, 0] = Link33_11;
        Links33[0, 1] = Link33_12;
        Links33[0, 2] = Link33_13;
        Links33[1, 0] = Link33_21;
        Links33[1, 1] = Link33_22;
        Links33[1, 2] = Link33_23;
        Links33[2, 0] = Link33_31;
        Links33[2, 1] = Link33_32;
        Links33[2, 2] = Link33_33;

        Links = new Link[3, 3];
        Links[0, 0] = new Link(Links11, Links11_Go);
        Links[0, 1] = new Link(Links12, Links12_Go);
        Links[0, 2] = new Link(Links13, Links13_Go);
        Links[1, 0] = new Link(Links21, Links21_Go);
        Links[1, 1] = new Link(Links22, Links22_Go);
        Links[1, 2] = new Link(Links23, Links23_Go);
        Links[2, 0] = new Link(Links31, Links31_Go);
        Links[2, 1] = new Link(Links32, Links32_Go);
        Links[2, 2] = new Link(Links33, Links33_Go);
    }

    internal List<StoryLevelButton> StoryLevelButtons = new List<StoryLevelButton>();

    private class Link
    {
        public Image[,] LinksImage;
        public GameObject Links_Go;

        public Link(Image[,] linksImage, GameObject links_Go)
        {
            LinksImage = linksImage;
            Links_Go = links_Go;
        }
    }

    private Link[,] Links;

    private Image[,] Links11;
    private Image[,] Links12;
    private Image[,] Links13;

    private Image[,] Links21;
    private Image[,] Links22;
    private Image[,] Links23;

    private Image[,] Links31;
    private Image[,] Links32;
    private Image[,] Links33;

    [SerializeField] private GameObject Links11_Go;
    [SerializeField] private Image Link11_11;

    [SerializeField] private GameObject Links12_Go;
    [SerializeField] private Image Link12_11;
    [SerializeField] private Image Link12_12;

    [SerializeField] private GameObject Links13_Go;
    [SerializeField] private Image Link13_11;
    [SerializeField] private Image Link13_12;
    [SerializeField] private Image Link13_13;


    [SerializeField] private GameObject Links21_Go;
    [SerializeField] private Image[,] test;
    [SerializeField] private Image Link21_11;
    [SerializeField] private Image Link21_21;

    [SerializeField] private GameObject Links22_Go;
    [SerializeField] private Image Link22_11;
    [SerializeField] private Image Link22_12;
    [SerializeField] private Image Link22_21;
    [SerializeField] private Image Link22_22;

    [SerializeField] private GameObject Links23_Go;
    [SerializeField] private Image Link23_11;
    [SerializeField] private Image Link23_12;
    [SerializeField] private Image Link23_13;
    [SerializeField] private Image Link23_21;
    [SerializeField] private Image Link23_22;
    [SerializeField] private Image Link23_23;


    [SerializeField] private GameObject Links31_Go;
    [SerializeField] private Image Link31_11;
    [SerializeField] private Image Link31_21;
    [SerializeField] private Image Link31_31;

    [SerializeField] private GameObject Links32_Go;
    [SerializeField] private Image Link32_11;
    [SerializeField] private Image Link32_12;
    [SerializeField] private Image Link32_21;
    [SerializeField] private Image Link32_22;
    [SerializeField] private Image Link32_31;
    [SerializeField] private Image Link32_32;

    [SerializeField] private GameObject Links33_Go;
    [SerializeField] private Image Link33_11;
    [SerializeField] private Image Link33_12;
    [SerializeField] private Image Link33_13;
    [SerializeField] private Image Link33_21;
    [SerializeField] private Image Link33_22;
    [SerializeField] private Image Link33_23;
    [SerializeField] private Image Link33_31;
    [SerializeField] private Image Link33_32;
    [SerializeField] private Image Link33_33;

    [SerializeField] private Text LevelName;


    public Level LevelInfo;

    public void Initialize(Level levelInfo, int endCount)
    {
        LevelInfo = levelInfo;
        LevelName.text = "Level " + levelInfo.LevelID;
        foreach (Boss bossInfo in levelInfo.Bosses)
        {
            StoryLevelButton slb = GameObjectPoolManager.Instance.Pool_StoryLevelButtonPool.AllocateGameObject<StoryLevelButton>(transform);
            slb.Initialize(bossInfo);
            slb.OnButtonClick += delegate { StartMenuManager.Instance.StartGameCore(true, levelInfo.LevelID, bossInfo.BossID); };
            StoryLevelButtons.Add(slb);
        }

        int startCount = levelInfo.Bosses.Count;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Links[i, j].Links_Go.SetActive(false);
            }
        }

        if (endCount != 0)
        {
            Link CurLink = Links[startCount - 1, endCount - 1];
            CurLink.Links_Go.SetActive(true);

            foreach (Image image in CurLink.LinksImage)
            {
                image.enabled = true;
                image.color = ClientUtils.GetColorFromColorDict(AllColors.ColorType.StoryLevelColor2);
            }

            //CurLink.LinksImage[start, end].enabled = true;
        }
    }

    public void SetBossState(int bossID, bool isBeat)
    {
        StoryLevelButton slb = StoryLevelButtons[bossID];
        if (slb != null)
        {
            if (isBeat)
            {
                slb.OnBeated();
            }
            else
            {
                slb.OnDisabled();
            }
        }
    }

    public void SetLevelKnown()
    {
        foreach (StoryLevelButton slb in StoryLevelButtons)
        {
            slb.SetKnown();
        }
    }

    public void SetLevelUnknown()
    {
        foreach (StoryLevelButton slb in StoryLevelButtons)
        {
            slb.SetUnknown();
        }
    }
}