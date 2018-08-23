using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 选牌窗口 - 选择卡组部分
/// </summary>
public partial class SelectCardDeckManager
{
    [SerializeField] private Transform AllMyBuildsContent;

    [SerializeField] private Button CreateNewBuildButton;
    [SerializeField] private Button DeleteBuildButton;
    [SerializeField] private Button SelectBuildButton;

    [SerializeField] private Text MyMoneyText;
    [SerializeField] private Text MyLifeText;
    [SerializeField] private Text MyMagicText;

    private BuildButton CurrentEditBuildButton;
    private int CurrentEditBuildID;
    public int CurrentSelectedBuildID;
    private int LeftMoney;
    private int Life;
    private int Magic;
    private Dictionary<int, BuildButton> AllBuildButtons = new Dictionary<int, BuildButton>();


    public void InitAllMyBuildInfos()
    {
        foreach (BuildInfo m_BuildInfo in Client.Instance.Proxy.BuildInfos)
        {
            BuildButton newBuildButton = GenerateNewBuildButton(m_BuildInfo);
            AllBuildButtons.Add(m_BuildInfo.BuildID, newBuildButton);
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

        newBuildButton.Button.onClick.RemoveAllListeners();
        newBuildButton.Button.onClick.AddListener(delegate { OnSwitchEditBuild(newBuildButton); });
        return newBuildButton;
    }

    private void OnSwitchEditBuild(BuildButton buildButton)
    {
        MyMoneyText.text = (GamePlaySettings.PlayerDefaultMoney - buildButton.BuildInfo.BuildConsumeMoney).ToString();
        MyLifeText.text = buildButton.BuildInfo.Life.ToString();
        MyMagicText.text = buildButton.BuildInfo.Magic.ToString();
    }

    public void OnCreateNewBuildButtonClick()
    {
        BuildRequest request = new BuildRequest(Client.Instance.Proxy.ClientId, new BuildInfo(-1, "New Build", new int[] { }, new int[] { }, 0, GamePlaySettings.PlayerDefaultLife, GamePlaySettings.PlayerDefaultMagic));
        Client.Instance.Proxy.SendMessage(request);
        CreateNewBuildButton.enabled = false;//接到回应前锁定
        DeleteBuildButton.enabled = false;
    }

    public void OnCreateNewBuild(BuildRequest request)
    {
        BuildButton newBuildButton = GenerateNewBuildButton(request.BuildInfo);
        AllBuildButtons.Add(request.BuildInfo.BuildID, newBuildButton);
        CreateNewBuildButton.enabled = true;//解锁
        DeleteBuildButton.enabled = true;
    }

    public void OnDeleteBuild()
    {
    }

    public void OnSelectBuild()
    {
        CurrentSelectedBuildID = CurrentEditBuildButton.BuildInfo.BuildID;
    }
}