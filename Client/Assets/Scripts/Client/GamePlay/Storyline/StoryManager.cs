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

    public void SetEnemyBeated(int levelID)
    {
        JustBeatedEnemy = (Enemy) Story.CurrentFightingChapter.Levels[levelID];
        Story.CurrentFightingChapter.LevelBeatedDictionary[levelID] = true;
        UIManager.Instance.GetBaseUIForm<StoryPanel>().Cur_ChapterMap.RefreshKnownLevels();
        NoticeManager.Instance.ShowInfoPanelCenter("打败了" + levelID, 0, 1f);
    }

    public List<BonusGroup> GetCurrentBonusGroup()
    {
        return JustBeatedEnemy?.BonusGroups;
    }

    internal bool HasStory => Story != null;

    #region Story模式过关奖励记录

    internal bool JustGetSomeCard => JustGetNewCards.Count != 0 || JustUpgradeCards.Count != 0; //刚才是否选择了新卡片
    internal int JustLifeChange = 0; //获取奖励后生命变化量
    internal int JustEnergyChange = 0; //获取奖励后能量变化量
    internal int JustBudgetChange = 0; //获取奖励后预算变化量

    internal Enemy JustBeatedEnemy; //刚击败的敌人s
    internal bool JustBeatedChapter = false; //刚才是否通过章节
    internal int JustUnlockedCardRareLevelNum => Story.CurrentFightingChapter.ChapterID; //刚才解锁的卡牌稀有度等级

    internal HashSet<int> JustGetNewCards = new HashSet<int>();
    internal HashSet<int> JustUpgradeCards = new HashSet<int>();

    public void ResetStoryBonusInfo()
    {
        JustLifeChange = 0;
        JustEnergyChange = 0;
        JustBudgetChange = 0;
        JustGetNewCards.Clear();
        JustUpgradeCards.Clear();
    }

    #endregion
}