using System.Collections.Generic;

public class StoryManager : MonoSingleton<StoryManager>
{
    private StoryManager()
    {
    }

    private Story Story;

    public void InitializeStory(Story story)
    {
        if (story == null)
        {
            //TODO ResetAll
            UnlockedCardLevelNum = 9;
        }

        Story = story;
    }

    public Story GetStory()
    {
        return Story;
    }

    public void SetStoryPaceBeated(int storyPaceID)
    {
    }

    public void SetStoryPaceFailed(int storyPaceID)
    {
    }

    internal int UnlockedCardLevelNum = 9;

    internal bool HasStory => Story != null;
    internal bool IsThisLevelNumberUp = false;

    #region Story模式过关奖励记录

    internal bool JustGetSomeCard = false; //刚才是否选择了新卡片
    internal bool JustLifeAdd = false; //刚才是否增加了生命
    internal bool JustLifeLost = false; //刚才是否减少了生命
    internal bool JustEnergyAdd = false; //刚才是否增加了能量
    internal bool JustEnergyLost = false; //刚才是否减少了能量
    internal bool JustBudgetAdd = false; //刚才是否新增了预算
    internal bool JustBudgetLost = false; //刚才是否减少了预算
    internal HashSet<int> JustGetNewCards = new HashSet<int>();
    internal HashSet<int> JustUpgradeCards = new HashSet<int>();

    public void ResetStoryBonusInfo()
    {
        JustGetSomeCard = false;
        JustLifeAdd = false;
        JustLifeLost = false;
        JustEnergyAdd = false;
        JustEnergyLost = false;
        JustBudgetAdd = false;
        JustBudgetLost = false;
        JustGetNewCards.Clear();
        JustUpgradeCards.Clear();
    }

    #endregion
}