﻿using System.Collections.Generic;
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


    public Story M_CurrentStory = null;
    public int Current_LevelID;
    public int Current_BossPicID;

    public void InitiateStoryCanvas(Story story)
    {
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
        return M_CurrentStory.Levels[Current_LevelID].Bosses[Current_BossPicID].AlwaysBonusGroup;
    }

    public List<BonusGroup> GetCurrentOptionalBonusGroup()
    {
        return M_CurrentStory.Levels[Current_LevelID].Bosses[Current_BossPicID].OptionalBonusGroup;
    }

    public void SetLevelBeated(int levelID, int bossPicID)
    {
        int beatBossIndex = -1;

        List<StoryLevelButton> slbs = LevelCols[levelID].StoryLevelButtons;
        for (int i = 0; i < slbs.Count; i++)
        {
            if (slbs[i].M_BossInfo.PicID == bossPicID) beatBossIndex = i;
        }

        SetBossState(levelID, beatBossIndex, true);
        if (!M_CurrentStory.LevelBeatBossPicIDs.ContainsKey(levelID))
        {
            M_CurrentStory.LevelBeatBossPicIDs.Add(levelID, bossPicID);
        }

        int beatBoss = M_CurrentStory.LevelBeatBossPicIDs[levelID];
        int bossCount = M_CurrentStory.Levels[levelID].Bosses.Count;
        for (int j = 0; j < bossCount; j++)
        {
            if (j != beatBoss) SetBossState(levelID, j, false);
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
    }

    public void SetLevelKnown(int levelID, List<int> bossPicIDs)
    {
        if (M_CurrentStory.Levels.Count > levelID)
        {
            StoryCol sc = LevelCols[levelID];
            int index = 0;
            foreach (StoryLevelButton button in sc.StoryLevelButtons)
            {
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

    private void SyncStoryBGSliderValue(float value)
    {
        StoryBGScrollbar.value = value;
    }
}