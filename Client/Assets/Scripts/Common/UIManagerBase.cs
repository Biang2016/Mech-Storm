using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIManagerBase<T> : MonoSingleton<T> where T : MonoBehaviour
{
    public StateMachine M_StateMachine;

    public abstract class StateMachine
    {
        public StateMachine()
        {
            state = ExitMenuManager.StateMachine.States.Default;
            previousState = ExitMenuManager.StateMachine.States.Default;
        }

        public class State
        {

        }

        public class State_Hide : State
        {

        }
        public class State_Show : State
        {

        }

        public enum States
        {
            Default,
            HideForSetting,
            Hide,
            Show,
        }

        private ExitMenuManager.StateMachine.States state;
        private ExitMenuManager.StateMachine.States previousState;

        public void SetState(ExitMenuManager.StateMachine.States newState)
        {
            if (state != newState)
            {
                switch (newState)
                {
                    case ExitMenuManager.StateMachine.States.Hide:
                        HideMenu();
                        break;
                    case ExitMenuManager.StateMachine.States.HideForSetting:
                        HideMenuForSetting();
                        break;
                    case ExitMenuManager.StateMachine.States.Show:
                        if (state == ExitMenuManager.StateMachine.States.HideForSetting) ShowMenuAfterSettingClose();
                        else if (Client.Instance.IsLogin() || Client.Instance.IsPlaying()) ShowMenu();
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

        public ExitMenuManager.StateMachine.States GetState()
        {
            return state;
        }

        public void Update()
        {
            if (ConfirmWindowManager.Instance.IsConfirmWindowShow) return;
            if (StoryManager.Instance.M_StateMachine.GetState() == StoryManager.StateMachine.States.Show) return;
            if (WinLostPanelManager.Instance.IsShow) return;
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if ((SelectBuildManager.Instance.M_StateMachine.GetState() == SelectBuildManager.StateMachine.States.Hide || SelectBuildManager.Instance.M_StateMachine.GetState() == SelectBuildManager.StateMachine.States.HideForPlay) && SettingMenuManager.Instance.M_StateMachine.GetState() == SettingMenuManager.StateMachine.States.Hide)
                {
                    switch (state)
                    {
                        case ExitMenuManager.StateMachine.States.Default:
                            SetState(ExitMenuManager.StateMachine.States.Show);
                            break;
                        case ExitMenuManager.StateMachine.States.Hide:
                            SetState(ExitMenuManager.StateMachine.States.Show);
                            break;
                        case ExitMenuManager.StateMachine.States.HideForSetting:
                            break;
                        case ExitMenuManager.StateMachine.States.Show:
                            SetState(ExitMenuManager.StateMachine.States.Hide);
                            break;
                    }
                }
            }

            bool isClickElseWhere = (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) || Input.GetMouseButtonDown(1);
            if (isClickElseWhere)
            {
                if (state == ExitMenuManager.StateMachine.States.Show)
                {
                    SetState(ExitMenuManager.StateMachine.States.Hide);
                }
            }
        }

        public abstract void ShowMenu();

        public void HideMenu()
        {
            MouseHoverManager.Instance.M_StateMachine.ReturnToPreviousState();
            if (Client.Instance.IsLogin()) StartMenuManager.Instance.M_StateMachine.ReturnToPreviousState();
        }

        public void HideMenuForSetting()
        {
        }

        public void ShowMenuAfterSettingClose()
        {
        }
    }

}