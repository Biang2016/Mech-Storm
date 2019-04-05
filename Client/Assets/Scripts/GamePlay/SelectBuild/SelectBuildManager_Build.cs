using System.Collections.Generic;
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
        public SortedDictionary<int, BuildInfo> OnlineBuildInfos = new SortedDictionary<int, BuildInfo>();
        public GamePlaySettings OnlineGamePlaySettings;
        public int OnlineGameCurrentBuildID;
    }

    public OnlineCompete M_CurrentOnlineCompete = null;

    private void Awake_Build()
    {
        Proxy.OnClientStateChange += NetworkStateChange_Build;

        RenameConfirmText.text = LanguageManager.Instance.GetText("Common_Confirm");
        RenameCancelText.text = LanguageManager.Instance.GetText("Common_Cancel");
        CreateBuildText.text = LanguageManager.Instance.GetText("SelectBuildManagerBuild_CreateNewDeck");
    }

    public enum GameMode
    {
        None,
        Online,
        Single,
    }

    public GameMode GameMode_State = GameMode.None;

    public void SwitchGameMode(GameMode gameMode, bool isForce = false)
    {
        if (!isForce && GameMode_State == gameMode) return;
        if (!StoryManager.Instance.HasStory && gameMode == GameMode.Single) return;
        GameMode_State = gameMode;
        InitAllMyBuildInfos();
        SetAllCardHideElementsByAccount();
    }

    private void InitAllMyBuildInfos()
    {
        List<BuildInfo> buildInfos;
        if (GameMode_State == GameMode.Online)
        {
            buildInfos = M_CurrentOnlineCompete.OnlineBuildInfos.Values.ToList();
            foreach (BuildInfo buildInfo in buildInfos)
            {
                CheckBuildInfoValid(buildInfo);
            }

            GamePlaySettings = M_CurrentOnlineCompete.OnlineGamePlaySettings;
        }
        else
        {
            buildInfos = StoryManager.Instance.GetStory().PlayerBuildInfos.Values.ToList();
            foreach (BuildInfo buildInfo in buildInfos)
            {
                CheckBuildInfoValid(buildInfo);
            }

            GamePlaySettings = StoryManager.Instance.GetStory().StoryGamePlaySettings;
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

        while (AllMyBuildsContent.childCount > 1)
        {
            BuildButton bb = AllMyBuildsContent.GetChild(0).GetComponent<BuildButton>();
            if (bb != null)
            {
                bb.PoolRecycle();
            }
        }

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
            lastSaveBuildInfo.BuildID = CurrentEditBuildButton.BuildInfo.BuildID;
            CurrentSelectedBuildButton = CurrentEditBuildButton;
            CurrentSelectedBuildButton.IsSelected = true;
            SelectCardsByBuildInfo(CurrentEditBuildButton.BuildInfo);

            UIManager.Instance.GetBaseUIForm<StartMenuPanel>()?.RefreshBuildInfoAbstract();

            ShowSliders();
            RefreshCoinLifeEnergy();
            RefreshCardNum();
        }
        else
        {
            if (GameMode_State == GameMode.Single)
            {
                UnlockedCards(StoryManager.Instance.GetStory().Base_CardLimitDict);
            }
            else
            {
                UnlockAllOnlineCards();
            }

            HideSliders();
        }

        if (GameMode_State == GameMode.Single)
        {
            if (StoryManager.Instance.GetStory().PlayerBuildInfos.Count != 0)
            {
                int buildID = StoryManager.Instance.GetStory().PlayerBuildInfos.Keys.ToList()[0];
                SwitchToBuildButton(buildID);
            }
        }
        else if (GameMode_State == GameMode.Online)
        {
            if (CurrentBuildButtons.ContainsKey(M_CurrentOnlineCompete.OnlineGameCurrentBuildID))
            {
                SwitchToBuildButton(M_CurrentOnlineCompete.OnlineGameCurrentBuildID);
            }
        }
    }

    private void CheckBuildInfoValid(BuildInfo buildInfo) //由于卡片ID配置更新导致服务器数据老旧，删除此部分卡片
    {
        List<int> unExistedCardIDs = new List<int>();

        foreach (int cardID in buildInfo.M_BuildCards.GetCardIDs())
        {
            if (!AllCards.CardDict.ContainsKey(cardID))
            {
                unExistedCardIDs.Add(cardID);
            }
        }

        foreach (int cardID in unExistedCardIDs)
        {
            buildInfo.M_BuildCards.CardSelectInfos.Remove(cardID);
        }
    }

    public void NetworkStateChange_Build(ProxyBase.ClientStates clientState)
    {
        bool isMatching = clientState == ProxyBase.ClientStates.Matching;
        DeleteBuildButton.gameObject.SetActive(!isMatching);
    }

    private BuildButton GenerateNewBuildButton(BuildInfo m_BuildInfo)
    {
        BuildButton newBuildButton = GameObjectPoolManager.Instance.PoolDict["BuildButton"].AllocateGameObject<BuildButton>(AllMyBuildsContent);
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
        lastSaveBuildInfo.BuildID = CurrentEditBuildButton.BuildInfo.BuildID;
        CurrentEditBuildButton.IsEdit = true;
        SelectCardsByBuildInfo(buildButton.BuildInfo);
        RefreshCoinLifeEnergy();
        RefreshCardNum();
        AudioManager.Instance.SoundPlay("sfx/SwitchBuild");

        if (GameMode_State == GameMode.Online)
        {
            M_CurrentOnlineCompete.OnlineGameCurrentBuildID = buildButton.BuildInfo.BuildID;
        }
    }

    private void OnSaveBuildInfo()
    {
        if (!lastSaveBuildInfo.EqualsTo(CurrentEditBuildButton.BuildInfo))
        {
            if (CurrentBuildButtons.ContainsKey(CurrentEditBuildButton.BuildInfo.BuildID))
            {
                BuildRequest request = new BuildRequest(Client.Instance.Proxy.ClientId, CurrentEditBuildButton.BuildInfo, GameMode_State == GameMode.Single);
                Client.Instance.Proxy.SendMessage(request);
            }

            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("SelectBuildManagerBuild_YourDeckIsSaved"), 0f, 0.5f);
        }
    }

    public void OnCreateNewBuildButtonClick()
    {
        SortedDictionary<int, int> ccd = null;
        if (GameMode_State == GameMode.Single)
        {
            ccd = StoryManager.Instance.GetStory().Base_CardLimitDict;
        }

        BuildInfo bi = new BuildInfo(-1, LanguageManager.Instance.GetText("SelectBuildManagerBuild_NewDeck"), new BuildInfo.BuildCards(new SortedDictionary<int, BuildInfo.BuildCards.CardSelectInfo>()), GamePlaySettings.DefaultDrawCardNum, GamePlaySettings.DefaultLife, GamePlaySettings.DefaultEnergy,
            0, false, GamePlaySettings);
        BuildRequest request = new BuildRequest(Client.Instance.Proxy.ClientId, bi, GameMode_State == GameMode.Single);
        Client.Instance.Proxy.SendMessage(request);
        CreateNewBuildButton.enabled = false; //接到回应前锁定
        DeleteBuildButton.enabled = false;
    }

    public void OnCreateNewBuildResponse(BuildInfo buildInfo)
    {
        SortedDictionary<int, int> ccd = null;
        if (GameMode_State == GameMode.Single)
        {
            ccd = StoryManager.Instance.GetStory().Base_CardLimitDict;
        }

        BuildButton newBuildButton = GenerateNewBuildButton(buildInfo);
        CurrentBuildButtons.Add(buildInfo.BuildID, newBuildButton);
        CurrentBuildDict.Add(buildInfo.BuildID, newBuildButton.BuildInfo);
        if (GameMode_State == GameMode.Online)
        {
            M_CurrentOnlineCompete.OnlineBuildInfos.Add(newBuildButton.BuildInfo.BuildID, newBuildButton.BuildInfo);
        }
        else if (GameMode_State == GameMode.Single)
        {
            StoryManager.Instance.GetStory().PlayerBuildInfos.Add(newBuildButton.BuildInfo.BuildID, newBuildButton.BuildInfo);
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
            DeleteBuildRequest request = new DeleteBuildRequest(Client.Instance.Proxy.ClientId, CurrentEditBuildButton.BuildInfo.BuildID, GameMode_State == GameMode.Single);
            Client.Instance.Proxy.SendMessage(request);
        }
        else
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("SelectBuildManagerBuild_NoDeckSelected"), 0f, 0.5f);
        }
    }

    public void OnDeleteBuildResponse(int buildID)
    {
        M_CurrentOnlineCompete.OnlineBuildInfos.Remove(buildID);
        if (StoryManager.Instance.GetStory() != null) StoryManager.Instance.GetStory().PlayerBuildInfos.Remove(buildID);

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
        if (CurrentBuildDict.ContainsKey(buildInfo.BuildID)) CurrentBuildDict[buildInfo.BuildID] = buildInfo;
        if (CurrentBuildButtons.ContainsKey(buildInfo.BuildID)) CurrentBuildButtons[buildInfo.BuildID].Initialize(buildInfo);
        if (CurrentEditBuildButton.BuildInfo.BuildID == buildInfo.BuildID && !CurrentEditBuildButton.BuildInfo.EqualsTo(buildInfo)) SelectCardsByBuildInfo(buildInfo);

        if (GameMode_State == GameMode.Online)
        {
            M_CurrentOnlineCompete.OnlineBuildInfos[buildInfo.BuildID] = buildInfo;
        }
        else if (GameMode_State == GameMode.Single)
        {
            StoryManager.Instance.GetStory().PlayerBuildInfos[buildInfo.BuildID] = buildInfo;
        }
    }
}