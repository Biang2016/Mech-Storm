using UnityEngine;
using UnityEngine.EventSystems;
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
        InitiateStoryCanvas();
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
        }

        private void HideMenu()
        {
            GameManager.Instance.StopBlurBackGround();
            Instance.StoryCanvas.enabled = false;
        }
    }


    [SerializeField] private Canvas StoryCanvas;
    [SerializeField] private Scrollbar StoryBGScrollbar;
    [SerializeField] private Scrollbar StoryScrollbar;
    [SerializeField] private Transform StoryLevelContainer;

    private void InitiateStoryCanvas()
    {
        StoryScrollbar.onValueChanged.AddListener(SyncStoryBGSliderValue);

        StoryCol sc = GameObjectPoolManager.Instance.Pool_StoryLevelColPool.AllocateGameObject<StoryCol>(StoryLevelContainer);
        sc.Initialize();
        StoryCol sc1 = GameObjectPoolManager.Instance.Pool_StoryLevelColPool.AllocateGameObject<StoryCol>(StoryLevelContainer);
        sc1.Initialize();
    }

    private void SyncStoryBGSliderValue(float value)
    {
        StoryBGScrollbar.value = value;
    }
}