using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingMenuManager : MonoSingletion<SettingMenuManager>
{
    private SettingMenuManager()
    {
    }

    void Awake()
    {
        M_StateMachine = new StateMachine();

        MasterSlider.onValueChanged.AddListener(OnMasterSliderValueChange);
        SoundSlider.onValueChanged.AddListener(OnSoundSliderValueChange);
        BGMSlider.onValueChanged.AddListener(OnBGMSliderValueChange);

        SettingText.text = GameManager.Instance.isEnglish ? "Settings" : "设置";
    }

    void Start()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
        Proxy.OnClientStateChange += OnClientChangeState;

        MasterSlider.value = MasterSlider.maxValue;
        OnMasterSliderValueChange(MasterSlider.maxValue);
        SoundSlider.value = SoundSlider.maxValue;
        OnSoundSliderValueChange(SoundSlider.maxValue);
        BGMSlider.value = BGMSlider.maxValue;
        OnBGMSliderValueChange(BGMSlider.maxValue);
    }

    void Update()
    {
        M_StateMachine.Update();
    }

    public void OnClientChangeState(ProxyBase.ClientStates clientState)
    {
        switch (clientState)
        {
            case ProxyBase.ClientStates.Offline:
                break;
            case ProxyBase.ClientStates.GetId:
                break;
            case ProxyBase.ClientStates.Login:
                break;
            case ProxyBase.ClientStates.Matching:
                break;
            case ProxyBase.ClientStates.Playing:
                break;
        }
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
            ShowFromExitMenu,
            ShowFromStartMenu,
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

                    case States.ShowFromExitMenu:
                        if (Client.Instance.IsLogin() || Client.Instance.IsPlaying()) ShowMenu();
                        break;

                    case States.ShowFromStartMenu:
                        if (Client.Instance.IsLogin() || Client.Instance.IsPlaying()) ShowMenu();
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
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                switch (state)
                {
                    case States.Hide:
                        break;
                    case States.ShowFromExitMenu:
                        SetState(States.Hide);
                        break;
                    case States.ShowFromStartMenu:
                        SetState(States.Hide);
                        break;
                }
            }

            bool isClickElseWhere = (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) || Input.GetMouseButtonDown(1);
            if (isClickElseWhere)
            {
                if (state == States.ShowFromExitMenu)
                {
                    SetState(States.Hide);
                }
                if (state == States.ShowFromStartMenu)
                {
                    SetState(States.Hide);
                }
            }
        }

        public void ShowMenu()
        {
            GameManager.Instance.StartBlurBackGround();
            Instance.SettingMenuCanvas.enabled = true;
            if (state == States.Hide) ExitMenuManager.Instance.M_StateMachine.SetState(ExitMenuManager.StateMachine.States.HideForSetting);
        }

        public void HideMenu()
        {
            GameManager.Instance.StopBlurBackGround();
            Instance.SettingMenuCanvas.enabled = false;
            if (state == States.ShowFromExitMenu) ExitMenuManager.Instance.M_StateMachine.ReturnToPreviousState();
            if (state == States.ShowFromStartMenu) StartMenuManager.Instance.M_StateMachine.ReturnToPreviousState();
        }
    }

    [SerializeField] private Canvas SettingMenuCanvas;

    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider SoundSlider;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private Text SettingText;


    public void OnMasterSliderValueChange(float value)
    {
        if (value.Equals(MasterSlider.minValue))
        {
            AudioManager.Instance.AudioMixer.SetFloat("MasterVolume", -100);
        }
        else
        {
            AudioManager.Instance.AudioMixer.SetFloat("MasterVolume", value);
        }
    }

    public void OnSoundSliderValueChange(float value)
    {
        if (value.Equals(SoundSlider.minValue))
        {
            AudioManager.Instance.AudioMixer.SetFloat("SoundVolume", -100);
        }
        else
        {
            AudioManager.Instance.AudioMixer.SetFloat("SoundVolume", value);
        }
    }

    public void OnBGMSliderValueChange(float value)
    {
        if (value.Equals(BGMSlider.minValue))
        {
            AudioManager.Instance.AudioMixer.SetFloat("BGMVolume", -100);
        }
        else
        {
            AudioManager.Instance.AudioMixer.SetFloat("BGMVolume", value);
        }
    }
}