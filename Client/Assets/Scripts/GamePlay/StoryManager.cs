using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoSingleton<StoryManager>
{
    private StoryManager()
    {
    }

    void Awake()
    {
        M_StateMachine = new StateMachine();
        M_StateMachine.SetState(StateMachine.States.Hide);
    }

    void Update()
    {
        M_StateMachine.Update();
    }

    public StateMachine M_StateMachine;

    public class StateMachine
    {
        public StateMachine()
        {
            state = States.Default;
            previousState = States.Default;
        }

        public enum States
        {
            Default,
            Hide,
            Show,
        }

        private States state;
        private States previousState;

        public void SetState(States newState)
        {
            if (state != newState)
            {
                switch (newState)
                {
                    case States.Hide:
                        HideMenu();
                        break;

                    case States.Show:
                        ShowMenu();
                        break;
                }

                previousState = state;
                state = newState;
            }
        }

        public void ReturnToPreviousState()
        {
            SetState(previousState);
        }

        public States GetState()
        {
            return state;
        }

        public void Update()
        {
            if (state == States.Show)
            {
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    SetState(States.Hide);
                }
            }
        }

        private void ShowMenu()
        {
            GameManager.Instance.StartBlurBackGround();
            Instance.StoryCanvas.enabled = true;
            AudioManager.Instance.SoundPlay("sfx/StoryOpen");
            Instance.Anim.SetTrigger("Show");
            Instance.StartCoroutine(Instance.Co_SetStoryScrollPlace());
        }

        private void HideMenu()
        {
            GameManager.Instance.StopBlurBackGround();
            Instance.StoryCanvas.enabled = false;
            Instance.Anim.SetTrigger("Hide");
        }
    }

    IEnumerator Co_SetStoryScrollPlace()
    {
        yield return new WaitForSeconds(0.1f);
        Instance.StoryScrollRect.horizontalNormalizedPosition = (float) Fighting_LevelID / M_CurrentStory.Levels.Count;
        yield return null;
    }

    [SerializeField] private Canvas StoryCanvas;
    [SerializeField] private ScrollRect StoryScrollRect;
    [SerializeField] private ScrollRect StoryBGScrollRect;
    [SerializeField] private Scrollbar StoryBGScrollbar;
    [SerializeField] private Scrollbar StoryScrollbar;
    [SerializeField] private RectTransform StoryLevelScrollView;
    [SerializeField] private RectTransform StoryLevelContainer;
    [SerializeField] private Animator Anim;

    private SortedDictionary<int, StoryCol> LevelCols = new SortedDictionary<int, StoryCol>();


    public Story M_CurrentStory = null;
    public int Fighting_LevelID;
    public int Fighting_BossPicID;
    public int Current_LevelNum; //目前正在打的这关的等级
    public int Conquered_LevelNum = -1; //已经征服的等级

    public bool IsThisLevelNumberUp
    {
        get
        {
            Level curLevel = M_CurrentStory.Levels[Fighting_LevelID];
            if (!IsFightingLevelFinalLevel)
            {
                Level nextLevel = M_CurrentStory.Levels[Fighting_LevelID + 1];
                return curLevel.LevelNum < nextLevel.LevelNum;
            }
            else
            {
                return false;
            }
        }
    }

    public bool IsFightingLevelFinalLevel
    {
        get { return M_CurrentStory.Levels.Count == Fighting_LevelID + 1; }
    }

    public void InitiateStoryCanvas(Story story)
    {
        Conquered_LevelNum = -1;
        AllCards.ResetCardLevelDictRemain(story.PlayerCurrentUnlockedBuildInfo.CardIDs); //重置解锁卡牌字典
        foreach (StoryCol sc in LevelCols.Values)
        {
            sc.PoolRecycle();
        }

        LevelCols.Clear();

        M_CurrentStory = story;

        for (int i = 0; i < story.Levels.Count; i++)
        {
            int bossCount = story.LevelBossCount[i];
            int nextBossCount = 0;
            if (i != story.Levels.Count - 1)
            {
                nextBossCount = story.LevelBossCount[i + 1];
            }

            StoryCol storyCol = GameObjectPoolManager.Instance.Pool_StoryLevelColPool.AllocateGameObject<StoryCol>(StoryLevelContainer);
            storyCol.Initialize(story.Levels[i], bossCount, nextBossCount);
            LevelCols.Add(storyCol.LevelInfo.LevelID, storyCol);
        }

        foreach (StoryCol sc in LevelCols.Values)
        {
            sc.SetLevelUnknown();
        }

        if (LevelCols.Count > 1)
        {
            LevelCols[0].SetLevelKnown();
        }

        foreach (KeyValuePair<int, List<int>> kv in story.LevelUnlockBossInfo)
        {
            SetLevelKnown(kv.Key, kv.Value);
        }

        int beatLevel = 0;
        foreach (KeyValuePair<int, int> kv in story.LevelBeatBossPicIDs)
        {
            if (kv.Key > beatLevel)
            {
                beatLevel = kv.Key;
            }

            SetLevelBeated(kv.Key, kv.Value);
        }

        SetLevelKnown(0, story.LevelUnlockBossInfo[0]);

        if (M_CurrentStory.Levels.Count < 5)
        {
            StoryScrollRect.onValueChanged.RemoveAllListeners();
        }
        else
        {
            StoryScrollRect.onValueChanged.AddListener(delegate { SyncStoryBGSliderValue(); });
        }
    }

    public List<BonusGroup> GetCurrentBonusGroup(bool isOptional)
    {
        List<BonusGroup> bgs;
        List<BonusGroup> bgs_opt = M_CurrentStory.Levels[Fighting_LevelID].Bosses[Fighting_BossPicID].OptionalBonusGroup;
        List<BonusGroup> bgs_alw = M_CurrentStory.Levels[Fighting_LevelID].Bosses[Fighting_BossPicID].AlwaysBonusGroup;

        List<BonusGroup> removeBgs = new List<BonusGroup>();

        if (isOptional)
        {
            bgs = bgs_opt;
        }
        else
        {
            bgs = bgs_alw;
        }

        foreach (BonusGroup bg in bgs)
        {
            Bonus b = bg.Bonuses[0];
            if (b.M_BonusType == Bonus.BonusType.UnlockCardByID)
            {
                if (M_CurrentStory.PlayerCurrentUnlockedBuildInfo.CardIDs.Contains(b.Value))
                {
                    removeBgs.Add(bg);
                }
                else
                {
                    M_CurrentStory.PlayerCurrentUnlockedBuildInfo.CardIDs.Add(b.Value); //暂时假设这张卡片已经加入到解锁集合中了，这个CardIDs现在可任意修改，因为它在领完奖励就会从服务端同步一遍
                    AllCards.ResetCardLevelDictRemain(M_CurrentStory.PlayerCurrentUnlockedBuildInfo.CardIDs); //这个字典也会重置的
                }
            }
            else if (b.M_BonusType == Bonus.BonusType.UnlockCardByLevelNum)
            {
                CardInfo_Base cb = AllCards.GetRandomCardInfoByLevelNum(b.Value);
                if (cb == null) //该等级的卡片已经全部解锁了
                {
                    removeBgs.Add(bg); //这个bonus就失效了
                }
                else
                {
                    b.M_BonusType = Bonus.BonusType.UnlockCardByID;
                    b.Value = cb.CardID;
                    bg.Bonuses[0] = b;

                    M_CurrentStory.PlayerCurrentUnlockedBuildInfo.CardIDs.Add(b.Value); //暂时假设这张卡片已经加入到解锁集合中了，这个CardIDs现在可任意修改，因为它在领完奖励就会从服务端同步一遍
                    AllCards.ResetCardLevelDictRemain(M_CurrentStory.PlayerCurrentUnlockedBuildInfo.CardIDs); //这个字典也会重置的
                }
            }
        }

        removeBgs.ForEach(bg => { bgs.Remove(bg); });
        return bgs;
    }


    public void SetLevelBeated(int levelID, int bossPicID)
    {
        int beatBossIndex = -1;

        List<StoryLevelButton> slbs = LevelCols[levelID].StoryLevelButtons;
        for (int i = 0; i < slbs.Count; i++)
        {
            if (slbs[i].M_BossInfo.PicID == bossPicID) beatBossIndex = i;
        }

        if (!M_CurrentStory.LevelBeatBossPicIDs.ContainsKey(levelID))
        {
            M_CurrentStory.LevelBeatBossPicIDs.Add(levelID, bossPicID);
        }

        int beatBoss = M_CurrentStory.LevelBeatBossPicIDs[levelID];
        int bossCount = LevelCols[levelID].StoryLevelButtons.Count;
        for (int j = 0; j < bossCount; j++)
        {
            SetBossState(levelID, j, j == beatBossIndex);
        }


        if (levelID > 0)
        {
            int lastBeatBossPicID = M_CurrentStory.LevelBeatBossPicIDs[levelID - 1];
            int lastBeatBossIndex = -1;

            List<StoryLevelButton> slbs_last = LevelCols[levelID - 1].StoryLevelButtons;
            for (int i = 0; i < slbs_last.Count; i++)
            {
                if (slbs_last[i].M_BossInfo.PicID == lastBeatBossPicID) lastBeatBossIndex = i;
            }

            LevelCols[levelID - 1].SetLink_HL_Show(lastBeatBossIndex, beatBossIndex);
        }

        Current_LevelNum = M_CurrentStory.Levels[levelID].LevelNum;
        if (levelID < M_CurrentStory.Levels.Count - 1)
        {
            Level next_Level = M_CurrentStory.Levels[levelID + 1];
            int tempLevelNum = next_Level.LevelNum;
            if (tempLevelNum > Current_LevelNum)
            {
                Conquered_LevelNum = tempLevelNum;
            }

            LevelCols[levelID].SetAsNotCurrentLevel();
            LevelCols[levelID + 1].SetAsCurrentLevel();
        }
    }

    public void SetLevelKnown(int levelID, List<int> bossPicIDs)
    {
        if (M_CurrentStory.Levels.Count > levelID)
        {
            StoryCol sc = LevelCols[levelID];
            int index = 0;
            foreach (StoryLevelButton button in sc.StoryLevelButtons)
            {
                if (!sc.LevelInfo.Bosses.ContainsKey(bossPicIDs[index]))
                {
                    int a = 0;
                }

                foreach (KeyValuePair<int, Boss> kv in sc.LevelInfo.Bosses)
                {
                    Boss boss = kv.Value;
                }

                button.Initialize(sc.LevelInfo.Bosses[bossPicIDs[index]]);
                index++;
            }

            sc.SetLevelKnown();
        }
    }

    private void SetBossState(int levelID, int bossIndex, bool isBeat)
    {
        StoryCol storyCol = LevelCols[levelID];
        storyCol.SetBossState(bossIndex, isBeat);
    }

    private void SyncStoryBGSliderValue()
    {
        StoryBGScrollRect.horizontalNormalizedPosition = StoryScrollRect.horizontalNormalizedPosition;
    }
}