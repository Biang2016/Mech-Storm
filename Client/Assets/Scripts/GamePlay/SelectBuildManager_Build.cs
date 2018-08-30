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

    private BuildInfo lastSaveBuildInfo;
    internal BuildButton CurrentEditBuildButton;
    internal BuildButton CurrentSelectedBuildButton;
    private Dictionary<int, BuildButton> AllBuildButtons = new Dictionary<int, BuildButton>();
    private Dictionary<int, BuildInfo> AllBuilds = new Dictionary<int, BuildInfo>();

    public BuildRenamePanel BuildRenamePanel;

    private void Awake_Build()
    {
        Proxy.OnClientStateChange += NetworkStateChange_Build;
        InitializeSliders();
    }

    public void InitAllMyBuildInfos(List<BuildInfo> buildInfos)
    {
        CreateNewBuildButton.transform.SetAsLastSibling();
        while (AllMyBuildsContent.childCount > 1)
        {
            BuildButton bb = AllMyBuildsContent.GetChild(AllMyBuildsContent.childCount - 1).GetComponent<BuildButton>();
            if (bb != null)
            {
                bb.PoolRecycle();
            }
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

        CreateNewBuildButton.transform.SetAsLastSibling();

        if (AllBuildButtons.Count != 0)
        {
            CurrentEditBuildButton = AllMyBuildsContent.transform.GetChild(0).GetComponent<BuildButton>();
            CurrentEditBuildButton.IsEdit = true;
            lastSaveBuildInfo = CurrentEditBuildButton.BuildInfo.Clone();
            CurrentSelectedBuildButton = CurrentEditBuildButton;
            CurrentSelectedBuildButton.IsSelected = true;
            SelectCardsByBuildInfo(CurrentEditBuildButton.BuildInfo);

            ShowSliders();
            RefreshMoneyLifeMagic();
        }
        else
        {
            HideSliders();
        }
    }

    public void NetworkStateChange_Build(ProxyBase.ClientStates clientState)
    {
        bool isMatching = clientState == ProxyBase.ClientStates.Matching;
        DeleteBuildButton.gameObject.SetActive(!isMatching);
    }

    private BuildButton GenerateNewBuildButton(BuildInfo m_BuildInfo)
    {
        BuildButton newBuildButton = GameObjectPoolManager.Instance.Pool_BuildButtonPool.AllocateGameObject(AllMyBuildsContent).GetComponent<BuildButton>();
        newBuildButton.Initialize(m_BuildInfo);

        CreateNewBuildButton.transform.SetAsLastSibling();

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

    private void OnSwitchEditBuild(BuildButton buildButton)
    {
        if (buildButton == CurrentEditBuildButton) return;
        if (M_StateMachine.GetState() == StateMachine.States.Show_ReadOnly) return;
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
        RefreshMoneyLifeMagic();
    }

    public void OnCreateNewBuildButtonClick()
    {
        BuildRequest request = new BuildRequest(Client.Instance.Proxy.ClientId, new BuildInfo(-1, "New Build", new List<int>(), 0, GamePlaySettings.PlayerDefaultLife * GamePlaySettings.LifeToMoney, GamePlaySettings.PlayerDefaultMagic * GamePlaySettings.MagicToMoney, GamePlaySettings.PlayerDefaultLife, GamePlaySettings.PlayerDefaultMagic));
        Client.Instance.Proxy.SendMessage(request);
        CreateNewBuildButton.enabled = false; //接到回应前锁定
        DeleteBuildButton.enabled = false;
    }

    public void OnCreateNewBuildResponse(int buildID)
    {
        BuildButton newBuildButton = GenerateNewBuildButton(new BuildInfo(buildID, "New Build", new List<int>(), 0, GamePlaySettings.PlayerDefaultLife * GamePlaySettings.LifeToMoney, GamePlaySettings.PlayerDefaultMagic * GamePlaySettings.MagicToMoney, GamePlaySettings.PlayerDefaultLife, GamePlaySettings.PlayerDefaultMagic));
        AllBuildButtons.Add(buildID, newBuildButton);
        AllBuilds.Add(buildID, newBuildButton.BuildInfo);
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
            if (newEditBuildButtonSiblingIndex < 0 || AllMyBuildsContent.childCount == 1)
            {
                CurrentEditBuildButton = null;
                CurrentSelectedBuildButton = null;
                UnSelectAllCard();
                HideSliders();
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
        if (CurrentEditBuildButton.BuildInfo.EqualsTo(buildInfo)) SelectCardsByBuildInfo(buildInfo);
    }
}