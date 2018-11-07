using System.Collections.Generic;
using System.Linq;
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
            Instance.StoryBGScrollbar.value = 0;
            Instance.StoryScrollbar.value = 0;
        }

        private void HideMenu()
        {
            GameManager.Instance.StopBlurBackGround();
            Instance.StoryCanvas.enabled = false;
            Instance.Anim.SetTrigger("Hide");
        }
    }


    [SerializeField] private Canvas StoryCanvas;
    [SerializeField] private Scrollbar StoryBGScrollbar;
    [SerializeField] private Scrollbar StoryScrollbar;
    [SerializeField] private RectTransform StoryLevelScrollView;
    [SerializeField] private RectTransform StoryLevelContainer;
    [SerializeField] private Animator Anim;

    private SortedDictionary<int, StoryCol> LevelCols = new SortedDictionary<int, StoryCol>();
    SortedDictionary<int, int> LevelFightTimes = new SortedDictionary<int, int>(); //每个等级对应需要过关次数,Key : LevelNum
    SortedDictionary<int, List<Boss>> LevelBossRemain = new SortedDictionary<int, List<Boss>>(); //每个等级Boss库剩余  Key : LevelNum
    SortedDictionary<int, int> LevelBossCount = new SortedDictionary<int, int>(); //每关Boss按钮数量  Key : LevelID

    public Story M_CurrentStory = null;
    public int Current_LevelID;
    public int Current_BossID;

    public void InitiateStoryCanvas(Story story)
    {
        foreach (StoryCol sc in LevelCols.Values)
        {
            sc.PoolRecycle();
        }

        LevelCols.Clear();

        M_CurrentStory = story;

        LevelFightTimes.Clear();
        LevelBossRemain.Clear();
        foreach (Level level in story.Levels)
        {
            if (!LevelFightTimes.ContainsKey(level.LevelNum))
            {
                LevelFightTimes.Add(level.LevelNum, 1);
            }
            else
            {
                LevelFightTimes[level.LevelNum]++;
            }

            if (!LevelBossRemain.ContainsKey(level.LevelNum))
            {
                LevelBossRemain.Add(level.LevelNum, level.Bosses.Values.ToList());
            }
        }

        for (int i = 0; i < story.Levels.Count; i++)
        {
            int levelNum = story.Levels[i].LevelNum;
            int curLevelBossTryCount = Random.Range(Mathf.Min(LevelBossRemain[levelNum].Count, 2), Mathf.Min(4, LevelBossRemain[levelNum].Count + 1)); //每个level尽量选出2~3个boss
            List<Boss> bosses = Utils.GetRandomFromList(LevelBossRemain[levelNum], curLevelBossTryCount);
            HashSet<int> bossPicIDs = new HashSet<int>();
            bosses.ForEach(boss => { bossPicIDs.Add(boss.PicID); });
            bosses.ForEach(boss => { LevelBossRemain[levelNum].Remove(boss); });
            LevelBossCount.Add(i, bossPicIDs.Count);
        }

        for (int i = 0; i < story.Levels.Count; i++)
        {
            int bossCount = LevelBossCount[i];
            int nextBossCount = 0;
            if (i != story.Levels.Count - 1)
            {
                nextBossCount = LevelBossCount[i + 1];
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

        //Todo 啊啊啊啊额

        int beatLevel = 0;
        foreach (KeyValuePair<int, int> kv in story.PlayerBeatBossIDs)
        {
            if (kv.Key > beatLevel)
            {
                beatLevel = kv.Key;
            }

            SetLevelBeated(kv.Key, kv.Value);
        }

        StoryBGScrollbar.value = 0;
        StoryScrollbar.value = 0;
        if (StoryLevelContainer.rect.width < StoryLevelScrollView.rect.width)
        {
            StoryScrollbar.onValueChanged.RemoveAllListeners();
        }
        else
        {
            StoryScrollbar.onValueChanged.AddListener(SyncStoryBGSliderValue);
        }
    }

    public List<BonusGroup> GetCurrentAlwaysBonusGroup()
    {
        return M_CurrentStory.Levels[Current_LevelID].Bosses[Current_BossID].AlwaysBonusGroup;
    }

    public List<BonusGroup> GetCurrentOptionalBonusGroup()
    {
        return M_CurrentStory.Levels[Current_LevelID].Bosses[Current_BossID].OptionalBonusGroup;
    }

    public void SetLevelBeated(int levelID, int bossPicID)
    {
        SetBossState(levelID, bossPicID, true);
        if (!M_CurrentStory.PlayerBeatBossIDs.ContainsKey(levelID))
        {
            M_CurrentStory.PlayerBeatBossIDs.Add(levelID, bossPicID);
        }

        int beatBoss = M_CurrentStory.PlayerBeatBossIDs[levelID];
        int bossCount = M_CurrentStory.Levels[levelID].Bosses.Count;
        for (int j = 0; j < bossCount; j++)
        {
            if (j != beatBoss) SetBossState(levelID, j, false);
        }

        if (M_CurrentStory.Levels.Count > levelID + 1)
        {
            LevelCols[levelID + 1].SetLevelKnown();
        }

        if (levelID > 0)
        {
            int lastBeatBossID = M_CurrentStory.PlayerBeatBossIDs[levelID - 1];
            LevelCols[levelID - 1].SetLink_HL_Show(lastBeatBossID, bossPicID);
        }
    }

    private void SetBossState(int levelID, int bossID, bool isBeat)
    {
        StoryCol storyCol = LevelCols[levelID];
        storyCol.SetBossState(bossID, isBeat);
    }

    private void SyncStoryBGSliderValue(float value)
    {
        StoryBGScrollbar.value = value;
    }
}