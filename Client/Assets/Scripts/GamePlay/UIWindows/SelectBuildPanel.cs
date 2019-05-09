using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SelectBuildPanel : BaseUIForm
{
    [SerializeField] private Animator SelectWindowShowAnim;
    [SerializeField] private Transform LeftWindowTransform;
    [SerializeField] private Transform CenterWindowTransform;
    [SerializeField] private Transform RightWindowTransform;

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: true,
            isClickElsewhereClose: false,
            uiForms_Type: UIFormTypes.Normal,
            uiForms_ShowMode: UIFormShowModes.HideOther,
            uiForm_LucencyType: UIFormLucencyTypes.Blur);

        Awake_Bars();
        Awake_Cards();
        Awake_Build();
        Awake_SelectCards();
    }

    void Start()
    {
        Start_Bars();
        Start_Cards();
        Start_Build();
        Start_SelectCards();
    }

    public bool IsInit = false;

    protected override void ChildUpdate()
    {
        base.ChildUpdate();
        Update_Cards();
    }

    private States state;

    public enum States
    {
        Normal,
        ReadOnly
    }

    public void SetState(States newState)
    {
        state = newState;
        SetReadOnly_Bars(IsReadOnly);
        SetReadOnly_Builds(IsReadOnly);
        SetReadOnly_Cards(IsReadOnly);
        SetReadOnly_SelectCards(IsReadOnly);
    }

    public bool IsReadOnly => state == States.ReadOnly;

    private GamePlaySettings CurrentGamePlaySettings => SelectBuildManager.Instance.CurrentGamePlaySettings;

    public override void Display()
    {
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            //TODO 加载等待标志
        }

        UIMaskMgr.Instance.SetMaskWindow(gameObject, UIType.UIForms_Type, UIType.UIForm_LucencyType);
        SelectWindowShowAnim.SetTrigger("Show");
    }

    public override void Hide()
    {
        UIMaskMgr.Instance.CancelAllMaskWindow(UIType.UIForm_LucencyType);
        SelectWindowShowAnim.SetTrigger("Reset");
        if (CurrentEditBuildButton)
        {
            SelectBuildManager.Instance.OnSaveBuildInfo(CurrentEditBuildButton.BuildInfo);
        }

        UIManager.Instance.CloseUIForm<AffixPanel>();
        currentPreviewCard?.PoolRecycle();
        MouseHoverManager.Instance.M_StateMachine.ReturnToPreviousState();
    }

    public Dictionary<int, CardBase> allCards = new Dictionary<int, CardBase>(); // 所有卡片都放入窗口，按需隐藏
    public Dictionary<int, PoolObject> allCardContainers = new Dictionary<int, PoolObject>(); // 每张卡片都有一个容器
    public Dictionary<int, CardBase> allShownCards = new Dictionary<int, CardBase>(); // 所有显示的卡片

    public void Init(bool force = false)
    {
        if (!force && IsInit) return;

        Init_Bars();
        Init_Cards();
        Init_Build();
        Init_SelectCards();

        ShowSliders(CurrentBuildButtons.Count != 0);
        if (CurrentBuildButtons.Count != 0)
        {
            RefreshCoinLifeEnergy();
            RefreshDrawCardNum();
        }
        else
        {
            if (SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Single)
            {
                SetCardLimit(StoryManager.Instance.GetStory().Base_CardLimitDict);
            }
            else if (SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Online)
            {
                ShowAllOnlineCards();
            }
        }

        if (SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Single)
        {
            if (StoryManager.Instance.GetStory().PlayerBuildInfos.Count != 0)
            {
                int buildID = StoryManager.Instance.GetStory().PlayerBuildInfos.Keys.ToList()[0];
                SwitchToBuildButton(buildID);
            }
        }
        else if (SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Online)
        {
            if (CurrentBuildButtons.ContainsKey(OnlineManager.Instance.CurrentOnlineBuildID))
            {
                SwitchToBuildButton(OnlineManager.Instance.CurrentOnlineBuildID);
            }
        }

        IsInit = true;
    }
}