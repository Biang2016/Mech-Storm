using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExitMenuPanel : BaseUIForm
{
    private ExitMenuPanel()
    {
    }

    [SerializeField] private Transform ButtonContainer;

    void Awake()
    {
        UIType.IsClearStack = false;
        UIType.IsESCClose = true;
        UIType.IsClickElsewhereClose = true;
        UIType.UIForm_LucencyType = UIFormLucencyTypes.Blur;
        UIType.UIForms_ShowMode = UIFormShowModes.Return;
        UIType.UIForms_Type = UIFormTypes.PopUp;

        AddButton("Surrender", "ExitMenu_SurrenderText", OnSurrenderButtonClick);
        AddButton("Consume", "ExitMenu_ConsumeText", OnConsumeGameButtonClick);
        AddButton("Setting", "ExitMenu_SettingMenuText", OnSettingMenuButtonClick);
        AddButton("Quit", "ExitMenu_QuitText", OnQuitGameButtonClick);

        ExitMenuButtonListDict.Add(States.Show_MainMenu, new List<string> {"Consume", "Setting", "Quit"});
        ExitMenuButtonListDict.Add(States.Show_Playing, new List<string> {"Consume", "Surrender", "Setting", "Quit"});
    }

    private Dictionary<string, ExitMenuButton> ExitMenuButtonDict = new Dictionary<string, ExitMenuButton>();
    private Dictionary<States, List<string>> ExitMenuButtonListDict = new Dictionary<States, List<string>>();

    [Flags]
    public enum States
    {
        Show_MainMenu,
        Show_Playing
    }

    private void AddButton(string goName, string textKey, UnityAction buttonClick)
    {
        ExitMenuButton emb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ExitMenuButton].AllocateGameObject<ExitMenuButton>(ButtonContainer);
        emb.name = goName + "Button";
        emb.BindTextKey(textKey, buttonClick);
        ExitMenuButtonDict.Add(goName, emb);
    }

    void Start()
    {
        Proxy.OnClientStateChange += OnClientChangeState;
    }

    public void OnClientChangeState(ProxyBase.ClientStates clientState)
    {
        switch (clientState)
        {
            case ProxyBase.ClientStates.Offline:
                UIManager.Instance.CloseUIForms<ExitMenuPanel>();
                break;
            case ProxyBase.ClientStates.GetId:
                UIManager.Instance.CloseUIForms<ExitMenuPanel>();
                break;
            case ProxyBase.ClientStates.Login:
                SetState(States.Show_MainMenu);
                break;
            case ProxyBase.ClientStates.Matching:
                SetState(States.Show_MainMenu);
                break;
            case ProxyBase.ClientStates.Playing:
                SetState(States.Show_Playing);
                break;
        }
    }

    private States state;

    public void SetState(States newState)
    {
        List<string> showButtons = ExitMenuButtonListDict[newState];
        foreach (KeyValuePair<string, ExitMenuButton> kv in ExitMenuButtonDict)
        {
            bool isShow = showButtons.Contains(kv.Key);
            kv.Value.gameObject.SetActive(isShow);
        }

        foreach (string btnName in showButtons)
        {
            ExitMenuButtonDict[btnName].transform.SetAsLastSibling();
        }

        state = newState;
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
        StoryManager.Instance.ResetStoryBonusInfo();
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