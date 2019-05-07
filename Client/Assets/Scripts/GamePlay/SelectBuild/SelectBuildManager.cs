using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 选牌窗口
/// </summary>
public partial class SelectBuildManager : MonoSingleton<SelectBuildManager>
{
    private SelectBuildManager()
    {
    }

    void Awake()
    {
    }

    public GamePlaySettings CurrentGamePlaySettings;

    public SortedDictionary<int, BuildInfo> BuildInfoDict = new SortedDictionary<int, BuildInfo>(); //全卡组信息集合

    public BuildInfo CurrentSelectedBuildInfo; //和UI层的当前选中的BuildInfo始终保持引用一致

    private BuildInfo currentEditBuildInfo;

    public BuildInfo CurrentEditBuildInfo // 该项始终与UI层保持引用一致
    {
        get { return currentEditBuildInfo; }
        set
        {
            if (currentEditBuildInfo != null) OnSaveBuildInfo(currentEditBuildInfo);
            currentEditBuildInfo = value;
            lastSaveBuildInfo = currentEditBuildInfo.Clone();
            lastSaveBuildInfo.BuildID = currentEditBuildInfo.BuildID;
        }
    }

    private BuildInfo lastSaveBuildInfo; //上一次保存的卡组信息，一旦切换编辑卡组，就自动发送服务端保存卡组，并刷新lastSaveBuildInfo

    public enum GameMode
    {
        None,
        Online,
        Single,
    }

    public GameMode CurrentGameMode = GameMode.None;

    public void SwitchGameMode(GameMode gameMode, bool isForce = false)
    {
        if (!isForce && CurrentGameMode == gameMode) return;
        if (!StoryManager.Instance.HasStory && gameMode == GameMode.Single) return;
        CurrentGameMode = gameMode;
        InitBuildInfos();
    }

    private void InitBuildInfos()
    {
        BuildInfoDict.Clear();
        List<BuildInfo> buildInfos = new List<BuildInfo>();
        if (CurrentGameMode == GameMode.Single)
        {
            buildInfos = StoryManager.Instance.GetStory().PlayerBuildInfos.Values.ToList();
            CurrentGamePlaySettings = StoryManager.Instance.GetStory().StoryGamePlaySettings;
        }
        else if (CurrentGameMode == GameMode.Online)
        {
            buildInfos = OnlineManager.Instance.OnlineBuildInfos.Values.ToList();
            CurrentGamePlaySettings = OnlineManager.Instance.OnlineGamePlaySettings;
        }

        foreach (BuildInfo buildInfo in buildInfos)
        {
            buildInfo.GamePlaySettings = CurrentGamePlaySettings;
            CheckBuildInfoValid(buildInfo);
            BuildInfoDict.Add(buildInfo.BuildID, buildInfo);
        }
    }

    private void CheckBuildInfoValid(BuildInfo buildInfo) //由于卡片ID配置更新导致服务器数据老旧，删除此部分卡片
    {
        //TODO 在服务端发送前就删除
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

    public void OnSaveBuildInfo(BuildInfo buildInfo)
    {
        if (lastSaveBuildInfo == null || !lastSaveBuildInfo.EqualsTo(buildInfo))
        {
            BuildRequest request = new BuildRequest(Client.Instance.Proxy.ClientId, buildInfo, CurrentGameMode == GameMode.Single);
            Client.Instance.Proxy.SendMessage(request);

            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("SelectBuildManagerBuild_YourDeckIsSaved"), 0f, 0.5f);
            lastSaveBuildInfo = buildInfo.Clone();
            lastSaveBuildInfo.BuildID = buildInfo.BuildID;
        }
    }

    public void OnCreateNewBuildResponse(BuildInfo buildInfo)
    {
        SortedDictionary<int, int> cld = null;
        if (CurrentGameMode == GameMode.Single)
        {
            cld = StoryManager.Instance.GetStory().Base_CardLimitDict;
        }

        BuildInfoDict.Add(buildInfo.BuildID, buildInfo);
        if (CurrentGameMode == GameMode.Online)
        {
            OnlineManager.Instance.OnlineBuildInfos.Add(buildInfo.BuildID, buildInfo);
        }
        else if (CurrentGameMode == GameMode.Single)
        {
            StoryManager.Instance.GetStory().PlayerBuildInfos.Add(buildInfo.BuildID, buildInfo);
        }

        UIManager.Instance.GetBaseUIForm<SelectBuildPanel>()?.OnCreateNewBuildButton(buildInfo);
    }

    public void OnDeleteBuildResponse(int buildID)
    {
        OnlineManager.Instance.OnlineBuildInfos.Remove(buildID);
        if (StoryManager.Instance.GetStory() != null) StoryManager.Instance.GetStory().PlayerBuildInfos.Remove(buildID);
        BuildInfoDict.Remove(buildID);

        UIManager.Instance.GetBaseUIForm<SelectBuildPanel>()?.OnDeleteBuildButton(buildID);
    }

    public void RefreshSomeBuild(BuildInfo buildInfo)
    {
        if (BuildInfoDict.ContainsKey(buildInfo.BuildID)) BuildInfoDict[buildInfo.BuildID] = buildInfo;

        if (CurrentGameMode == GameMode.Online)
        {
            OnlineManager.Instance.OnlineBuildInfos[buildInfo.BuildID] = buildInfo;
        }
        else if (CurrentGameMode == GameMode.Single)
        {
            StoryManager.Instance.GetStory().PlayerBuildInfos[buildInfo.BuildID] = buildInfo;
        }

        UIManager.Instance.GetBaseUIForm<SelectBuildPanel>()?.RefreshSomeBuild(buildInfo);
    }
}