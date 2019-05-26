using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class SelectBuildPanel
{
    [SerializeField] private Transform AllMyBuildsContainer;
    [SerializeField] private Text AllBuildText;

    [SerializeField] private Button CreateNewBuildButton;
    [SerializeField] private Button DeleteBuildButton;

    [SerializeField] private Text CreateBuildText;
    [SerializeField] private Text DeleteCardDeckText;

    private BuildButton currentEditBuildButton;

    internal BuildButton CurrentEditBuildButton
    {
        get { return currentEditBuildButton; }
        set
        {
            currentEditBuildButton = value;
            if (currentEditBuildButton != null)
            {
                SelectBuildManager.Instance.CurrentEditBuildInfo = currentEditBuildButton.BuildInfo;
            }
            else
            {
                SelectBuildManager.Instance.CurrentEditBuildInfo = null;
            }
        }
    }

    private BuildButton currentSelectedBuildButton;

    internal BuildButton CurrentSelectedBuildButton
    {
        get { return currentSelectedBuildButton; }
        set
        {
            currentSelectedBuildButton = value;
            SelectBuildManager.Instance.CurrentSelectedBuildInfo = currentSelectedBuildButton.BuildInfo;
            UIManager.Instance.GetBaseUIForm<StartMenuPanel>()?.RefreshBuildInfoAbstract(currentSelectedBuildButton.BuildInfo);
        }
    }

    private Dictionary<int, BuildButton> CurrentBuildButtons = new Dictionary<int, BuildButton>();

    void Awake_Build()
    {
        LanguageManager.Instance.RegisterTextKeys(new List<(Text, string)>
        {
            (CardDeckText: AllBuildText, "SelectBuildManagerSelect_CardDeckText"),
            (DeleteCardDeckText, "SelectBuildManagerSelect_DeleteCardDeckText"),
            (CreateBuildText, "SelectBuildManagerBuild_CreateNewDeck"),
        });

        CreateNewBuildButton.onClick.AddListener(OnCreateNewBuildButtonClick);
        DeleteBuildButton.onClick.AddListener(OnDeleteBuildButtonClick);
    }

    void Start_Build()
    {
    }

    void Init_Build()
    {
        InitBuildButtons(SelectBuildManager.Instance.BuildInfoDict);
    }

    void SetReadOnly_Builds(bool isReadOnly)
    {
        CreateNewBuildButton.gameObject.SetActive(!isReadOnly);
        DeleteBuildButton.gameObject.SetActive(!isReadOnly);
    }

    private void ClearAllBuilds()
    {
        while (AllMyBuildsContainer.childCount > 1)
        {
            BuildButton bb = AllMyBuildsContainer.GetChild(0).GetComponent<BuildButton>();
            if (bb != null)
            {
                bb.PoolRecycle();
            }
        }

        CurrentBuildButtons.Clear();
    }

    public void InitBuildButtons(SortedDictionary<int, BuildInfo> buildInfos)
    {
        ClearAllBuilds();

        foreach (KeyValuePair<int, BuildInfo> kv in buildInfos)
        {
            BuildButton newBuildButton = GenerateNewBuildButton(kv.Value);
            CurrentBuildButtons.Add(kv.Key, newBuildButton);
            newBuildButton.transform.SetParent(AllMyBuildsContainer.transform);
        }

        CreateNewBuildButton.transform.parent.SetAsLastSibling();

        if (CurrentBuildButtons.Count != 0)
        {
            CurrentEditBuildButton = AllMyBuildsContainer.transform.GetChild(0).GetComponent<BuildButton>();
            CurrentEditBuildButton.IsEdit = true;
            CurrentSelectedBuildButton = CurrentEditBuildButton;
            CurrentSelectedBuildButton.IsSelected = true;
            SelectCardsByBuildInfo(CurrentEditBuildButton.BuildInfo);
        }
    }

    private BuildButton GenerateNewBuildButton(BuildInfo m_BuildInfo)
    {
        BuildButton newBuildButton = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BuildButton].AllocateGameObject<BuildButton>(AllMyBuildsContainer);
        newBuildButton.Initialize(m_BuildInfo);

        CreateNewBuildButton.transform.parent.SetAsLastSibling();

        OnMouseClick bbc = newBuildButton.Button.GetComponent<OnMouseClick>();
        bbc.ResetListeners();
        bbc.LeftClick.AddListener(delegate { OnSwitchEditBuild(newBuildButton); });
        bbc.RightClick.AddListener(delegate { OnRightClickBuildButtonToRename(newBuildButton.BuildInfo); });
        bbc.LeftDoubleClick.AddListener(delegate { OnBuildButtonDoubleClickToSelect(newBuildButton); });
        return newBuildButton;
    }

    public void OnBuildButtonDoubleClickToSelect(BuildButton buildButton)
    {
        if (IsReadOnly) return;
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
        if (IsReadOnly) return;
        if (CurrentEditBuildButton)
        {
            CurrentEditBuildButton.IsEdit = false;
            SelectBuildManager.Instance.OnSaveBuildInfo(CurrentEditBuildButton.BuildInfo);
        }

        CurrentEditBuildButton = buildButton;
        CurrentEditBuildButton.IsEdit = true;
        SelectCardsByBuildInfo(buildButton.BuildInfo);
        RefreshCoinLifeEnergy();
        RefreshDrawCardNum();
        AudioManager.Instance.SoundPlay("sfx/SwitchBuild");

        if (SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Online)
        {
            OnlineManager.Instance.CurrentOnlineBuildID = buildButton.BuildInfo.BuildID;
        }
    }

    public void OnCreateNewBuildButtonClick()
    {
        BuildInfo bi = new BuildInfo(-1, LanguageManager.Instance.GetText("SelectBuildManagerBuild_NewDeck"), new BuildInfo.BuildCards("New deck", new SortedDictionary<int, BuildInfo.BuildCards.CardSelectInfo>()), CurrentGamePlaySettings.DefaultDrawCardNum, CurrentGamePlaySettings.DefaultLife,
            CurrentGamePlaySettings.DefaultEnergy,
            0, false, CurrentGamePlaySettings);
        BuildRequest request = new BuildRequest(Client.Instance.Proxy.ClientID, bi, SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Single);
        Client.Instance.Proxy.SendMessage(request);
        CreateNewBuildButton.enabled = false; //接到回应前锁定
        DeleteBuildButton.enabled = false;
    }

    public void OnCreateNewBuildButton(BuildInfo buildInfo)
    {
        BuildButton newBuildButton = GenerateNewBuildButton(buildInfo);
        CurrentBuildButtons.Add(buildInfo.BuildID, newBuildButton);
        OnSwitchEditBuild(newBuildButton);

        if (CurrentSelectedBuildButton == null)
        {
            CurrentSelectedBuildButton = newBuildButton;
            CurrentSelectedBuildButton.IsSelected = true;
            ShowSliders(true);
        }

        CreateNewBuildButton.enabled = true; //解锁
        DeleteBuildButton.enabled = true;
    }

    public void OnDeleteBuildButtonClick()
    {
        if (CurrentBuildButtons.Count == 1)
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("SelectBuildManagerBuild_CannotDeleteLastBuild"), 0f, 0.5f);
            return;
        }
        if (CurrentEditBuildButton)
        {
            DeleteBuildRequest request = new DeleteBuildRequest(Client.Instance.Proxy.ClientID, CurrentEditBuildButton.BuildInfo.BuildID, SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Single);
            Client.Instance.Proxy.SendMessage(request);
        }
        else
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("SelectBuildManagerBuild_NoDeckSelected"), 0f, 0.5f);
        }
    }

    public void OnDeleteBuildButton(int buildID)
    {
        if (CurrentBuildButtons.ContainsKey(buildID))
        {
            BuildButton deleteBuildButton = CurrentBuildButtons[buildID];

            int siblingIndex = deleteBuildButton.transform.GetSiblingIndex();
            int newEditBuildButtonSiblingIndex = 0;
            if (siblingIndex > AllMyBuildsContainer.childCount - 3)
            {
                newEditBuildButtonSiblingIndex = AllMyBuildsContainer.childCount - 3;
            }
            else
            {
                newEditBuildButtonSiblingIndex = siblingIndex;
            }

            bool isSelected = deleteBuildButton.IsSelected;
            deleteBuildButton.PoolRecycle();
            CurrentBuildButtons.Remove(buildID);
            if (newEditBuildButtonSiblingIndex < 0 || AllMyBuildsContainer.childCount == 1)
            {
                CurrentEditBuildButton = null;
                CurrentSelectedBuildButton = null;
                UnSelectAllCard(SelectCardMethods.DeleteBuild);
                ShowSliders(false);
                AudioManager.Instance.SoundPlay("sfx/SwitchBuild");
                return;
            }

            BuildButton nextCurrentEditBuildButton = AllMyBuildsContainer.GetChild(newEditBuildButtonSiblingIndex).GetComponent<BuildButton>();
            OnSwitchEditBuild(nextCurrentEditBuildButton);

            if (isSelected)
            {
                nextCurrentEditBuildButton.IsSelected = true;
                CurrentSelectedBuildButton = nextCurrentEditBuildButton;
            }
        }
    }

    public void OnRightClickBuildButtonToRename(BuildInfo buildInfo)
    {
        UIManager.Instance.ShowUIForms<BuildRenamePanel>().ShowPanel(buildInfo);
    }

    public void RefreshSomeBuild(BuildInfo buildInfo)
    {
        if (CurrentBuildButtons.ContainsKey(buildInfo.BuildID)) CurrentBuildButtons[buildInfo.BuildID].Initialize(buildInfo);
        if (CurrentEditBuildButton.BuildInfo.BuildID == buildInfo.BuildID && !CurrentEditBuildButton.BuildInfo.EqualsTo(buildInfo)) SelectCardsByBuildInfo(buildInfo);
    }
}