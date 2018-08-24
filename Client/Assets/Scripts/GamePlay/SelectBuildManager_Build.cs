using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

/// <summary>
/// 选牌窗口 - 选择卡组部分
/// </summary>
public partial class SelectBuildManager
{
    [SerializeField] private Transform AllMyBuildsContent;

    [SerializeField] private Button CreateNewBuildButton;
    [SerializeField] private Button DeleteBuildButton;
    [SerializeField] private Button SelectBuildButton;

    [SerializeField] private Text MyMoneyText;
    [SerializeField] private Text MyLifeText;
    [SerializeField] private Text MyMagicText;

    private BuildInfo lastSaveBuildInfo;
    internal BuildButton CurrentEditBuildButton;
    internal BuildButton CurrentSelectedBuildButton;
    private int LeftMoney;
    private int Life;
    private int Magic;
    private Dictionary<int, BuildButton> AllBuildButtons = new Dictionary<int, BuildButton>();
    private Dictionary<int, BuildInfo> AllBuilds = new Dictionary<int, BuildInfo>();

    public BuildRenamePanel BuildRenamePanel;


    public void InitAllMyBuildInfos(List<BuildInfo> buildInfos)
    {
        while (AllMyBuildsContent.childCount > 0)
        {
            AllMyBuildsContent.GetChild(0).GetComponent<BuildButton>().PoolRecycle();
        }

        AllBuildButtons.Clear();
        AllBuilds.Clear();
        foreach (BuildInfo m_BuildInfo in buildInfos)
        {
            AllBuilds.Add(m_BuildInfo.BuildID, m_BuildInfo);
        }

        Dictionary<int, BuildInfo> ascdic = AllBuilds.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value); //对key进行升序
        foreach (KeyValuePair<int, BuildInfo> kv in ascdic)
        {
            BuildButton newBuildButton = GenerateNewBuildButton(kv.Value);
            AllBuildButtons.Add(kv.Key, newBuildButton);
            newBuildButton.transform.SetParent(AllMyBuildsContent.transform);
        }

        if (AllBuildButtons.Count != 0)
        {
            CurrentEditBuildButton = AllMyBuildsContent.transform.GetChild(0).GetComponent<BuildButton>();
            CurrentEditBuildButton.IsEdit = true;
            lastSaveBuildInfo = CurrentEditBuildButton.BuildInfo.Clone();
            CurrentSelectedBuildButton = CurrentEditBuildButton;
            CurrentSelectedBuildButton.IsSelected = true;
            SelectCardsByBuildInfo(CurrentEditBuildButton.BuildInfo);
        }
    }

    public void NetworkStateChange_Build(ProxyBase.ClientStates clientState)
    {
        bool isMatching = clientState == ProxyBase.ClientStates.Matching;
        DeleteBuildButton.gameObject.SetActive(!isMatching);
        SelectBuildButton.gameObject.SetActive(!isMatching);
    }

    private BuildButton GenerateNewBuildButton(BuildInfo m_BuildInfo)
    {
        BuildButton newBuildButton = GameObjectPoolManager.Instance.Pool_BuildButtonPool.AllocateGameObject(AllMyBuildsContent).GetComponent<BuildButton>();
        newBuildButton.Initialize(m_BuildInfo);

        BuildButtonClick bbc = newBuildButton.Button.GetComponent<BuildButtonClick>();
        bbc.ResetListeners();
        bbc.leftClick.AddListener(delegate { OnSwitchEditBuild(newBuildButton); });
        bbc.rightClick.AddListener(delegate { OnRightClickBuildButtontToRename(newBuildButton.BuildInfo); });
        bbc.leftDoubleClick.AddListener(delegate { OnBuildButtonDoubleClickToSelect(newBuildButton); });
        return newBuildButton;
    }

    public void OnSelectBuildButtonClick()
    {
        if (CurrentSelectedBuildButton) CurrentSelectedBuildButton.IsSelected = false;
        if (CurrentEditBuildButton)
        {
            CurrentSelectedBuildButton = CurrentEditBuildButton;
            CurrentSelectedBuildButton.IsSelected = true;
            M_StateMachine.SetState(StateMachine.States.Hide);
        }
        else
        {
            NoticeManager.Instance.ShowInfoPanelCenter("您未创建卡组", 0f, 0.5f);
        }
    }

    public void OnBuildButtonDoubleClickToSelect(BuildButton buildButton)
    {
        if (CurrentSelectedBuildButton) CurrentSelectedBuildButton.IsSelected = false;
        CurrentSelectedBuildButton = buildButton;
        CurrentSelectedBuildButton.IsSelected = true;
    }

    private void OnSwitchEditBuild(BuildButton buildButton)
    {
        if (buildButton == CurrentEditBuildButton) return;
        if (CurrentEditBuildButton)
        {
            CurrentEditBuildButton.IsEdit = false;
            if (!lastSaveBuildInfo.EqualsTo(CurrentEditBuildButton.BuildInfo))
            {
                BuildRequest request = new BuildRequest(Client.Instance.Proxy.ClientId, CurrentEditBuildButton.BuildInfo);
                Client.Instance.Proxy.SendMessage(request);
                NoticeManager.Instance.ShowInfoPanelCenter("已保存卡组", 0f, 0.5f);
            }
        }

        CurrentEditBuildButton = buildButton;
        lastSaveBuildInfo = CurrentEditBuildButton.BuildInfo.Clone();
        CurrentEditBuildButton.IsEdit = true;
        SelectCardsByBuildInfo(buildButton.BuildInfo);
        MyMoneyText.text = (GamePlaySettings.PlayerDefaultMoney - buildButton.BuildInfo.BuildConsumeMoney).ToString();
        MyLifeText.text = buildButton.BuildInfo.Life.ToString();
        MyMagicText.text = buildButton.BuildInfo.Magic.ToString();
    }

    public void OnCreateNewBuildButtonClick()
    {
        BuildRequest request = new BuildRequest(Client.Instance.Proxy.ClientId, new BuildInfo(-1, "New Build", new List<int>(), new List<int>(), 0, GamePlaySettings.PlayerDefaultLife, GamePlaySettings.PlayerDefaultMagic));
        Client.Instance.Proxy.SendMessage(request);
        CreateNewBuildButton.enabled = false; //接到回应前锁定
        DeleteBuildButton.enabled = false;
    }

    public void OnCreateNewBuildResponse(int buildID)
    {
        BuildButton newBuildButton = GenerateNewBuildButton(new BuildInfo(buildID, "New Build", new List<int>(), new List<int>(), 0, GamePlaySettings.PlayerDefaultLife, GamePlaySettings.PlayerDefaultMagic));
        AllBuildButtons.Add(buildID, newBuildButton);
        AllBuilds.Add(buildID, newBuildButton.BuildInfo);
        OnSwitchEditBuild(newBuildButton);

        if (CurrentSelectedBuildButton == null)
        {
            CurrentSelectedBuildButton = newBuildButton;
            CurrentSelectedBuildButton.IsSelected = true;
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
            NoticeManager.Instance.ShowInfoPanelCenter("未选择卡组", 0f, 0.5f);
        }
    }

    public void OnDeleteBuildResponse(int buildID)
    {
        if (AllBuildButtons.ContainsKey(buildID))
        {
            BuildButton deleteBuildButton = AllBuildButtons[buildID];

            int siblingIndex = deleteBuildButton.transform.GetSiblingIndex();
            int newEditBuildButtonSiblingIndex = 0;
            if (siblingIndex > AllMyBuildsContent.childCount - 2)
            {
                newEditBuildButtonSiblingIndex = AllMyBuildsContent.childCount - 2;
            }
            else
            {
                newEditBuildButtonSiblingIndex = siblingIndex;
            }

            bool isSelected = deleteBuildButton.IsSelected;
            deleteBuildButton.PoolRecycle();
            if (newEditBuildButtonSiblingIndex < 0 || AllMyBuildsContent.childCount == 0)
            {
                CurrentEditBuildButton = null;
                CurrentSelectedBuildButton = null;
                UnSelectAllCard();
                return;
            }

            BuildButton nextCurrentEditBuildButton = AllMyBuildsContent.GetChild(newEditBuildButtonSiblingIndex).GetComponent<BuildButton>();
            OnSwitchEditBuild(nextCurrentEditBuildButton);

            if (isSelected)
            {
                nextCurrentEditBuildButton.IsSelected = true;
                CurrentSelectedBuildButton = nextCurrentEditBuildButton;
            }

            AllBuildButtons.Remove(buildID);
            AllBuilds.Remove(buildID);
        }
    }

    public void OnRightClickBuildButtontToRename(BuildInfo buildInfo)
    {
        BuildRenamePanel.ShowPanel(buildInfo);
    }

    public void RefreshSomeBuild(BuildInfo buildInfo)
    {
        if (buildInfo.EqualsTo(AllBuilds[buildInfo.BuildID])) return;
        AllBuilds[buildInfo.BuildID] = buildInfo;
        AllBuildButtons[buildInfo.BuildID].Initialize(buildInfo);
        SelectCardsByBuildInfo(buildInfo);
    }
}