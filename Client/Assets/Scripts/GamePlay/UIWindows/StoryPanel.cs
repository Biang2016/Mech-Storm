using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryPanel : BaseUIForm
{
    private StoryPanel()
    {
    }

    void Awake()
    {
        Sprite sp = AtlasManager.LoadAtlas("BGs").GetSprite("StoryBG");
        StoryBG.sprite = sp;
    }

    [SerializeField] private Image StoryBG;
    [SerializeField] private ScrollRect StoryScrollRect;
    [SerializeField] private ScrollRect StoryBGScrollRect;
    [SerializeField] private Scrollbar StoryBGScrollbar;
    [SerializeField] private Scrollbar StoryScrollbar;
    [SerializeField] private RectTransform StoryLevelScrollView;
    [SerializeField] private RectTransform StoryLevelContainer;
    [SerializeField] private Animator Anim;

    public Story M_CurrentStory = null;

    public void InitiateStoryCanvas(Story story)
    {
    }

    public List<BonusGroup> GetCurrentBonusGroup(bool isOptional, int optionalNumber = 0)
    {
        return null;
    }

    private List<int> UnlockedCardIDs()
    {
        return null;
    }
}