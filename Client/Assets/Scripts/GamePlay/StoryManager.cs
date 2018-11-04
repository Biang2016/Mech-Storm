using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;
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
    [SerializeField] private RectTransform StoryLevelContainer;
    [SerializeField] private Animator Anim;

    private SortedDictionary<int, StoryCol> LevelCols = new SortedDictionary<int, StoryCol>();

    public Story M_CurrentStory = null;

    public void InitiateStoryCanvas(Story story)
    {
        foreach (StoryCol sc in LevelCols.Values)
        {
            sc.PoolRecycle();
        }
        LevelCols.Clear();

        M_CurrentStory = story;
        StoryScrollbar.onValueChanged.AddListener(SyncStoryBGSliderValue);

        for (int i = 0; i < story.Levels.Count; i++)
        {
            int nextBossCount = 0;
            if (i != story.Levels.Count - 1)
            {
                nextBossCount = story.Levels[i + 1].Bosses.Count;
            }

            Level level = story.Levels[i];
            StoryCol storyCol = GameObjectPoolManager.Instance.Pool_StoryLevelColPool.AllocateGameObject<StoryCol>(StoryLevelContainer);
            storyCol.Initialize(level, nextBossCount);
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
        foreach (KeyValuePair<int, int> kv in story.PlayerBeatBossID)
        {
            if (kv.Key > beatLevel)
            {
                beatLevel = kv.Key;
            }

            SetLevelBeated(kv.Key, kv.Value);
        }
    }

    private void SyncStoryBGSliderValue(float value)
    {
        StoryBGScrollbar.value = value;
    }

    public void SetLevelBeated(int levelID, int bossID)
    {
        SetBossState(levelID, bossID, true);
        if (!M_CurrentStory.PlayerBeatBossID.ContainsKey(levelID))
        {
            M_CurrentStory.PlayerBeatBossID.Add(levelID, bossID);
        }

        int beatBoss = M_CurrentStory.PlayerBeatBossID[levelID];
        int bossCount = M_CurrentStory.Levels[levelID].Bosses.Count;
        for (int j = 0; j < bossCount; j++)
        {
            if (j != beatBoss) SetBossState(levelID, j, false);
        }

        if (M_CurrentStory.Levels.Count > levelID + 1)
        {
            LevelCols[levelID + 1].SetLevelKnown();
        }
    }

    private void SetBossState(int levelID, int bossID, bool isBeat)
    {
        StoryCol storyCol = LevelCols[levelID];
        storyCol.SetBossState(bossID, isBeat);
    }
}