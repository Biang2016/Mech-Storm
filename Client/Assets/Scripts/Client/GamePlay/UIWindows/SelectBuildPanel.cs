using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
        if (IsShow)
        {
            Update_Cards();
        }
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
        if (IsReadOnly)
        {
            UIType.UIForm_LucencyType = UIFormLucencyTypes.Blur;
        }
        else
        {
            UIType.UIForm_LucencyType = UIFormLucencyTypes.ImPenetrable;
        }

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
        }

        if (Client.Instance.IsPlaying())
        {
            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.SelectCardWindow_ReadOnly);
        }
        else if (Client.Instance.IsLogin())
        {
            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.SelectCardWindow);
        }

        UIMaskMgr.Instance.SetMaskWindow(gameObject, UIType.UIForms_Type, UIType.UIForm_LucencyType);

        SelectWindowShowAnim.SetTrigger("Show");
        IsShow = true;
    }

    internal bool IsShow = false;

    public override void Hide()
    {
        UIMaskMgr.Instance.CancelAllMaskWindow(UIType.UIForm_LucencyType);
        SelectWindowShowAnim.SetTrigger("Reset");
        DragManager.Instance.IsCanceling = false;
        if (CurrentEditBuildButton)
        {
            SelectBuildManager.Instance.OnSaveBuildInfo(CurrentEditBuildButton.BuildInfo);
        }

        if (Client.Instance.IsPlaying())
        {
            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.BattleNormal);
        }
        else
        {
            MouseHoverManager.Instance.M_StateMachine.ReturnToPreviousState();
        }

        UIManager.Instance.GetBaseUIForm<StartMenuPanel>()?.SingleDeckButton.SetTipImageTextShow(StoryManager.Instance.JustGetSomeCard);

        UIManager.Instance.CloseUIForm<AffixPanel>();
        currentPreviewCard?.PoolRecycle();
        IsShow = false;
    }

    public Dictionary<int, CardBase> AllCards = new Dictionary<int, CardBase>(); // 所有卡片都放入窗口，按需隐藏
    public Dictionary<int, PoolObject> AllCardContainers = new Dictionary<int, PoolObject>(); // 每张卡片都有一个容器
    public Dictionary<int, CardBase> AllShownCards = new Dictionary<int, CardBase>(); // 所有显示的卡片

    internal UnityAction StartGameAction;

    public void Init(UnityAction startGameAction, bool force = false)
    {
        StartGameAction = startGameAction;
        if (!force && IsInit) return;

        Init_Bars();
        Init_Cards();
        Init_Build();
        Init_SelectCards();
        IsInit = true;
    }
}