using System.Collections.Generic;

public class StoryManager : MonoSingleton<StoryManager>
{
    private StoryManager()
    {
    }

    private Story Story;

    public void InitializeStory(Story story)
    {
        ResetStoryBonusInfo();
        Story = story;
    }

    public Story GetStory()
    {
        return Story;
    }

    public void StartFightEnemy(int levelID)
    {
        CurrentFightingEnemy = (Enemy) Story.CurrentFightingChapter.Levels[levelID];
    }

    public void SetLevelBeated(int levelID)
    {
        Story.CurrentFightingChapter.LevelBeatedDictionary[levelID] = true;
        UIManager.Instance.GetBaseUIForm<StoryPanel>().CurrentStartGameAction = null;
        UIManager.Instance.GetBaseUIForm<StoryPanel>().Cur_ChapterMap.RefreshKnownLevels();
        CurrentFightingEnemy = null;
    }

    public List<BonusGroup> GetCurrentBonusGroup()
    {
        return CurrentFightingEnemy?.BonusGroups;
    }

    internal bool HasStory => Story != null;

    #region Story模式过关奖励记录

    internal bool JustGetSomeCard => JustGetNewCards.Count != 0; //刚才是否选择了新卡片
    internal int JustLifeChange = 0; //获取奖励后生命变化量
    internal int JustEnergyChange = 0; //获取奖励后能量变化量
    internal int JustBudgetChange = 0; //获取奖励后预算变化量

    internal Enemy CurrentFightingEnemy; //正在进行战斗的敌人
    internal bool JustBeatedChapter = false; //刚才是否通过章节

    internal HashSet<int> JustGetNewCards = new HashSet<int>();

    public void ResetStoryBonusInfo()
    {
        JustLifeChange = 0;
        JustEnergyChange = 0;
        JustBudgetChange = 0;
        JustGetNewCards.Clear();
    }

    #endregion
}