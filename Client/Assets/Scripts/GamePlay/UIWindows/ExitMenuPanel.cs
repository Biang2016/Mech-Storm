using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExitMenuPanel : BaseUIForm
{
    private ExitMenuPanel()
    {
    }

    [SerializeField] private Button SurrenderButton;
    [SerializeField] private Button ConsumeButton;
    [SerializeField] private Button QuitGameButton;

    [SerializeField] private Text SettingMenuText;
    [SerializeField] private Text SurrenderText;
    [SerializeField] private Text ConsumeText;
    [SerializeField] private Text QuitText;

    void Awake()
    {
        UIType.IsClearStack = false;
        UIType.UIForm_LucencyType = UIFormLucencyTypes.Blur;
        UIType.UIForms_ShowMode = UIFormShowModes.Return;
        UIType.UIForms_Type = UIFormTypes.PopUp;

        LanguageManager.Instance.RegisterTextKeys(
            new List<(Text, string)>
            {
                (SettingMenuText, "ExitMenu_SettingMenuText"),
                (SurrenderText, "ExitMenu_SurrenderText"),
                (ConsumeText, "ExitMenu_ConsumeText"),
                (QuitText, "ExitMenu_QuitText"),
                (SettingMenuText, "ExitMenu_SettingMenuText"),
                (SurrenderText, "ExitMenu_SurrenderText"),
                (ConsumeText, "ExitMenu_ConsumeText"),
                (QuitText, "ExitMenu_QuitText"),
            });
    }

    void Start()
    {
        SurrenderButton.gameObject.SetActive(false);
        ConsumeButton.gameObject.SetActive(true);
        QuitGameButton.gameObject.SetActive(true);
        Proxy.OnClientStateChange += OnClientChangeState;
    }

    public void OnClientChangeState(ProxyBase.ClientStates clientState)
    {
        switch (clientState)
        {
            case ProxyBase.ClientStates.Offline:
                SurrenderButton.gameObject.SetActive(false);
                ConsumeButton.gameObject.SetActive(false);
                QuitGameButton.gameObject.SetActive(false);
                break;
            case ProxyBase.ClientStates.GetId:
                SurrenderButton.gameObject.SetActive(false);
                ConsumeButton.gameObject.SetActive(false);
                QuitGameButton.gameObject.SetActive(false);
                break;
            case ProxyBase.ClientStates.Login:
                SurrenderButton.gameObject.SetActive(false);
                ConsumeButton.gameObject.SetActive(true);
                QuitGameButton.gameObject.SetActive(true);
                break;
            case ProxyBase.ClientStates.Matching:
                SurrenderButton.gameObject.SetActive(false);
                ConsumeButton.gameObject.SetActive(true);
                QuitGameButton.gameObject.SetActive(true);
                break;
            case ProxyBase.ClientStates.Playing:
                SurrenderButton.gameObject.SetActive(true);
                ConsumeButton.gameObject.SetActive(true);
                QuitGameButton.gameObject.SetActive(true);
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            CloseUIForm();
            return;
        }

        bool isClickElseWhere = (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) || Input.GetMouseButtonDown(1);
        if (isClickElseWhere)
        {
            CloseUIForm();
        }
    }

    public void OnSettingMenuButtonClick()
    {
        UIManager.Instance.ShowUIForms<SettingPanel>();
    }

    public void OnConsumeGameButtonClick()
    {
        CloseUIForm();
    }

    public void OnSurrenderButtonClick()
    {
        ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
        cp.Initialize(
            LanguageManager.Instance.GetText("ExitMenu_SureToSurrender"),
            LanguageManager.Instance.GetText("Common_Yes"),
            LanguageManager.Instance.GetText("Common_Cancel"),
            (new UnityAction(SurrenderCore)) + cp.CloseUIForm,
            cp.CloseUIForm
        );
    }

    private void SurrenderCore()
    {
        Client.Instance.Proxy.LeaveGame();
        SelectBuildManager.Instance.ResetStoryBonusInfo();
        RoundManager.Instance.StopGame();
        ClientLog.Instance.Print("You have quit the game");
        NoticeManager.Instance.ShowInfoPanelTop(LanguageManager.Instance.GetText("ExitMenu_YouHaveQuitGame"), 0, 1f);
    }

    public void OnQuitGameButtonClick()
    {
        NetworkManager.Instance.TerminateConnection();
        Application.Quit();
    }
}