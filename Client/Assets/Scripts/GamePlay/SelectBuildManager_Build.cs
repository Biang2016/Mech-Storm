﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选牌窗口 - 选择卡组部分
/// </summary>
public partial class SelectBuildManager
{
    [SerializeField] private Transform AllMyBuildsContent;

    [SerializeField] private Button CreateNewBuildButton;
    [SerializeField] private Button DeleteBuildButton;

    [SerializeField] private Text RenameConfirmText;
    [SerializeField] private Text RenameCancelText;
    [SerializeField] private Text CreateBuildText;

    private BuildInfo lastSaveBuildInfo;
    internal BuildButton CurrentEditBuildButton;
    internal BuildButton CurrentSelectedBuildButton;
    private Dictionary<int, BuildButton> CurrentBuildButtons = new Dictionary<int, BuildButton>();
    private Dictionary<int, BuildInfo> CurrentBuildDict = new Dictionary<int, BuildInfo>();

    public BuildRenamePanel BuildRenamePanel;

    public class OnlineCompete
    {
        public List<BuildInfo> OnlineBuildInfos = new List<BuildInfo>();
        public GamePlaySettings OnlineGamePlaySettings;
        public int OnlineGameCurrentBuildID;
    }

    public OnlineCompete M_CurrentOnlineCompete = null;

    public class Story
    {
        public List<BuildInfo> SingleBuildInfos = new List<BuildInfo>();
        public GamePlaySettings SingleGamePlaySettings;
        public BuildInfo SingleUnlockedBuildInfo = new BuildInfo();
        public int SingleCurrentBuildID;
    }

    public Story M_CurrentStory = null;

    private void Awake_Build()
    {
        Proxy.OnClientStateChange += NetworkStateChange_Build;

        RenameConfirmText.text = GameManager.Instance.IsEnglish ? "Confirm" : "确定";
        RenameCancelText.text = GameManager.Instance.IsEnglish ? "Cancel" : "取消";
        CreateBuildText.text = GameManager.Instance.IsEnglish ? "New Deck" : "创建新卡组";
    }

    public enum GameMode
    {
        None,
        Online,
        Single,
    }

    public GameMode GameMode_State = GameMode.None;

    public void SwitchGameMode(GameMode gameMode)
    {
        if (GameMode_State == gameMode) return;
        if (M_CurrentStory == null && gameMode == GameMode.Single) return;
        InitAllMyBuildInfos(gameMode);
        GameMode_State = gameMode;
    }

    private void InitAllMyBuildInfos(GameMode gameMode)
    {
        List<BuildInfo> buildInfos;
        if (gameMode == GameMode.Online)
        {
            buildInfos = M_CurrentOnlineCompete.OnlineBuildInfos;
            GamePlaySettings = M_CurrentOnlineCompete.OnlineGamePlaySettings;
            UnlockAllCards();
        }
        else
        {
            buildInfos = M_CurrentStory.SingleBuildInfos;
            GamePlaySettings = M_CurrentStory.SingleGamePlaySettings;
            LockAllCards();
            UnlockedCards(M_CurrentStory.SingleUnlockedBuildInfo.CardIDs);
        }

        InitializeSliders();

        CreateNewBuildButton.transform.parent.SetAsLastSibling();
        while (AllMyBuildsContent.childCount > 1)
        {
            BuildButton bb = AllMyBuildsContent.GetChild(0).GetComponent<BuildButton>();
            if (bb != null)
            {
                bb.PoolRecycle();
            }
        }

        CurrentBuildButtons.Clear();
        CurrentBuildDict.Clear();
        foreach (BuildInfo m_BuildInfo in buildInfos)
        {
            CurrentBuildDict.Add(m_BuildInfo.BuildID, m_BuildInfo);
        }

        Dictionary<int, BuildInfo> ascdic = CurrentBuildDict.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value); //对key进行升序
        foreach (KeyValuePair<int, BuildInfo> kv in ascdic)
        {
            BuildButton newBuildButton = GenerateNewBuildButton(kv.Value);
            CurrentBuildButtons.Add(kv.Key, newBuildButton);
            newBuildButton.transform.SetParent(AllMyBuildsContent.transform);
        }

        CreateNewBuildButton.transform.parent.SetAsLastSibling();

        if (CurrentBuildButtons.Count != 0)
        {
            CurrentEditBuildButton = AllMyBuildsContent.transform.GetChild(0).GetComponent<BuildButton>();
            CurrentEditBuildButton.IsEdit = true;
            lastSaveBuildInfo = CurrentEditBuildButton.BuildInfo.Clone();
            CurrentSelectedBuildButton = CurrentEditBuildButton;
            CurrentSelectedBuildButton.IsSelected = true;
            SelectCardsByBuildInfo(CurrentEditBuildButton.BuildInfo);

            StartMenuManager.Instance.RefreshBuildInfoAbstract();

            ShowSliders();
            RefreshCoinLifeEnergy();
            RefreshCardNum();
        }
        else
        {
            HideSliders();
        }

        if (gameMode == GameMode.Single)
        {
            if (CurrentBuildButtons.ContainsKey(M_CurrentStory.SingleCurrentBuildID))
            {
                SwitchToBuildButton(M_CurrentStory.SingleCurrentBuildID);
            }
        }
        else if (gameMode == GameMode.Online)
        {
            if (CurrentBuildButtons.ContainsKey(M_CurrentOnlineCompete.OnlineGameCurrentBuildID))
            {
                SwitchToBuildButton(M_CurrentOnlineCompete.OnlineGameCurrentBuildID);
            }
        }
    }

    public void NetworkStateChange_Build(ProxyBase.ClientStates clientState)
    {
        bool isMatching = clientState == ProxyBase.ClientStates.Matching;
        DeleteBuildButton.gameObject.SetActive(!isMatching);
    }

    private BuildButton GenerateNewBuildButton(BuildInfo m_BuildInfo)
    {
        BuildButton newBuildButton = GameObjectPoolManager.Instance.Pool_BuildButtonPool.AllocateGameObject<BuildButton>(AllMyBuildsContent);
        newBuildButton.Initialize(m_BuildInfo);

        CreateNewBuildButton.transform.parent.SetAsLastSibling();

        BuildButtonClick bbc = newBuildButton.Button.GetComponent<BuildButtonClick>();
        bbc.ResetListeners();
        bbc.leftClick.AddListener(delegate { OnSwitchEditBuild(newBuildButton); });
        bbc.rightClick.AddListener(delegate { OnRightClickBuildButtontToRename(newBuildButton.BuildInfo); });
        bbc.leftDoubleClick.AddListener(delegate { OnBuildButtonDoubleClickToSelect(newBuildButton); });
        return newBuildButton;
    }

    public void OnBuildButtonDoubleClickToSelect(BuildButton buildButton)
    {
        if (M_StateMachine.GetState() == StateMachine.States.Show_ReadOnly) return;
        if (CurrentSelectedBuildButton) CurrentSelectedBuildButton.IsSelected = false;
        CurrentSelectedBuildButton = buildButton;
        CurrentSelectedBuildButton.IsSelected = true;
    }

    public void SwitchToBuildButton(int buildID)
    {
        OnSwitchEditBuild(CurrentBuildButtons[buildID]);
    }

    private void OnSwitchEditBuild(BuildButton buildButton)
    {
        if (buildButton == CurrentEditBuildButton) return;
        if (M_StateMachine.GetState() == StateMachine.States.Show_ReadOnly) return;
        if (CurrentEditBuildButton)
        {
            CurrentEditBuildButton.IsEdit = false;
            OnSaveBuildInfo();
        }

        CurrentEditBuildButton = buildButton;
        lastSaveBuildInfo = CurrentEditBuildButton.BuildInfo.Clone();
        CurrentEditBuildButton.IsEdit = true;
        SelectCardsByBuildInfo(buildButton.BuildInfo);
        RefreshCoinLifeEnergy();
        RefreshCardNum();
        AudioManager.Instance.SoundPlay("sfx/SwitchBuild");

        if (GameMode_State == GameMode.Online)
        {
            M_CurrentOnlineCompete.OnlineGameCurrentBuildID = buildButton.BuildInfo.BuildID;
        }
        else
        {
            M_CurrentStory.SingleCurrentBuildID = buildButton.BuildInfo.BuildID;
        }
    }

    private void OnSaveBuildInfo()
    {
        if (!lastSaveBuildInfo.EqualsTo(CurrentEditBuildButton.BuildInfo))
        {
            BuildRequest request = new BuildRequest(Client.Instance.Proxy.ClientId, CurrentEditBuildButton.BuildInfo);
            Client.Instance.Proxy.SendMessage(request);
            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "Your deck is saved." : "已保存卡组", 0f, 0.5f);
        }
    }

    public void OnCreateNewBuildButtonClick()
    {
        BuildRequest request = new BuildRequest(Client.Instance.Proxy.ClientId, new BuildInfo(-1, GameManager.Instance.IsEnglish ? "New Deck" : "新卡组", new List<int>(), GamePlaySettings.DefaultDrawCardNum, GamePlaySettings.DefaultLife, GamePlaySettings.DefaultEnergy, GamePlaySettings));
        Client.Instance.Proxy.SendMessage(request);
        CreateNewBuildButton.enabled = false; //接到回应前锁定
        DeleteBuildButton.enabled = false;
    }

    public void OnCreateNewBuildResponse(int buildID)
    {
        BuildButton newBuildButton = GenerateNewBuildButton(new BuildInfo(buildID, GameManager.Instance.IsEnglish ? "New Deck" : "新卡组", new List<int>(), GamePlaySettings.DefaultDrawCardNum, GamePlaySettings.DefaultLife, GamePlaySettings.DefaultEnergy, GamePlaySettings));
        CurrentBuildButtons.Add(buildID, newBuildButton);
        CurrentBuildDict.Add(buildID, newBuildButton.BuildInfo);
        if (GameMode_State == GameMode.Online)
        {
            M_CurrentOnlineCompete.OnlineBuildInfos.Add(newBuildButton.BuildInfo);
        }
        else if (GameMode_State == GameMode.Single)
        {
            M_CurrentStory.SingleBuildInfos.Add(newBuildButton.BuildInfo);
        }

        OnSwitchEditBuild(newBuildButton);

        if (CurrentSelectedBuildButton == null)
        {
            CurrentSelectedBuildButton = newBuildButton;
            CurrentSelectedBuildButton.IsSelected = true;
            ShowSliders();
        }

        CreateNewBuildButton.enabled = true; //解锁
        DeleteBuildButton.enabled = true;
    }

    public void OnDeleteBuildButtonClick()
    {
        if (CurrentEditBuildButton)
        {
            DeleteBuildRequest request = new DeleteBuildRequest(Client.Instance.Proxy.ClientId, CurrentEditBuildButton.BuildInfo.BuildID);
            Client.Instance.Proxy.SendMessage(request);
        }
        else
        {
            NoticeManager.Instance.ShowInfoPanelCenter(GameManager.Instance.IsEnglish ? "No deck is selected" : "未选择卡组", 0f, 0.5f);
        }
    }

    public void OnDeleteBuildResponse(int buildID)
    {
        if (CurrentBuildButtons.ContainsKey(buildID))
        {
            BuildButton deleteBuildButton = CurrentBuildButtons[buildID];

            int siblingIndex = deleteBuildButton.transform.GetSiblingIndex();
            int newEditBuildButtonSiblingIndex = 0;
            if (siblingIndex > AllMyBuildsContent.childCount - 3)
            {
                newEditBuildButtonSiblingIndex = AllMyBuildsContent.childCount - 3;
            }
            else
            {
                newEditBuildButtonSiblingIndex = siblingIndex;
            }

            bool isSelected = deleteBuildButton.IsSelected;
            deleteBuildButton.PoolRecycle();
            CurrentBuildButtons.Remove(buildID);
            CurrentBuildDict.Remove(buildID);
            if (newEditBuildButtonSiblingIndex < 0 || AllMyBuildsContent.childCount == 1)
            {
                CurrentEditBuildButton = null;
                CurrentSelectedBuildButton = null;
                UnSelectAllCard();
                HideSliders();
                AudioManager.Instance.SoundPlay("sfx/SwitchBuild");
                return;
            }

            BuildButton nextCurrentEditBuildButton = AllMyBuildsContent.GetChild(newEditBuildButtonSiblingIndex).GetComponent<BuildButton>();
            OnSwitchEditBuild(nextCurrentEditBuildButton);

            if (isSelected)
            {
                nextCurrentEditBuildButton.IsSelected = true;
                CurrentSelectedBuildButton = nextCurrentEditBuildButton;
            }
        }
    }

    public void OnRightClickBuildButtontToRename(BuildInfo buildInfo)
    {
        BuildRenamePanel.ShowPanel(buildInfo);
    }

    public void RefreshSomeBuild(BuildInfo buildInfo)
    {
        if (buildInfo.EqualsTo(CurrentBuildDict[buildInfo.BuildID])) return;
        CurrentBuildDict[buildInfo.BuildID] = buildInfo;
        CurrentBuildButtons[buildInfo.BuildID].Initialize(buildInfo);
        if (CurrentEditBuildButton.BuildInfo.EqualsTo(buildInfo)) SelectCardsByBuildInfo(buildInfo);
    }
}