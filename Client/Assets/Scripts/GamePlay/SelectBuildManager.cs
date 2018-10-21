using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 选牌窗口
/// </summary>
public partial class SelectBuildManager : MonoSingletion<SelectBuildManager>
{
    private SelectBuildManager()
    {
    }

    private int cardSelectLayer;

    void Awake()
    {
        AddAllCards();
        cardSelectLayer = 1 << LayerMask.NameToLayer("CardSelect");
        M_StateMachine = new StateMachine();
        Canvas.gameObject.SetActive(false);

        Awake_Select();
        Awake_Build();
        Awake_PreviewAndUpgrade();
    }

    void Start()
    {
        M_StateMachine.SetState(StateMachine.States.Hide);
        Start_Select();
    }

    [SerializeField] private Transform AllCardsContent;

    [SerializeField] private Canvas Canvas;

    [SerializeField] private Canvas Canvas_BG;

    [SerializeField] private Camera Camera;

    [SerializeField] private Animator SelectWindowShowAnim;

    void Update()
    {
        M_StateMachine.Update();
    }

    public StateMachine M_StateMachine;

    public class StateMachine
    {
        public StateMachine()
        {
            state = States.Default;
            previousState = States.Default;
        }

        public enum States
        {
            Default,
            Hide,
            HideForPlay,
            Show,
            Show_ReadOnly,
        }

        private States state;
        private States previousState;

        public void SetState(States newState)
        {
            if (state != newState)
            {
                switch (newState)
                {
                    case States.Hide:
                        if (Client.Instance.IsLogin()) HideWindow();
                        else if (Client.Instance.IsPlaying()) HideWindowToPlaying();
                        break;
                    case States.HideForPlay:
                        if (Client.Instance.IsLogin()) HideWindowToPlaying();
                        break;
                    case States.Show:
                        if (Client.Instance.IsLogin() && !Client.Instance.IsMatching()) ShowWindow();
                        break;
                    case States.Show_ReadOnly:
                        if (Client.Instance.IsPlaying() || Client.Instance.IsMatching())
                        {
                            ShowWindowReadOnly();
                        }

                        break;
                }

                previousState = state;
                state = newState;
            }
        }

        public void ReturnToPreviousState()
        {
            SetState(previousState);
        }

        public States GetState()
        {
            return state;
        }

        public void Update()
        {
            if (ConfirmWindowManager.Instance.IsConfirmWindowShow) return;
            if (ExitMenuManager.Instance.M_StateMachine.GetState() == ExitMenuManager.StateMachine.States.Show) return;
            if (SettingMenuManager.Instance.M_StateMachine.GetState() == SettingMenuManager.StateMachine.States.ShowFromExitMenu) return;
            if (SettingMenuManager.Instance.M_StateMachine.GetState() == SettingMenuManager.StateMachine.States.ShowFromStartMenu) return;
            if (state == States.Hide || state == States.HideForPlay)
            {
                if (Input.GetKeyUp(KeyCode.Tab))
                {
                    if (Client.Instance.IsLogin() && !Client.Instance.IsMatching()) SetState(States.Show);
                    else if (Client.Instance.IsPlaying() || Client.Instance.IsMatching()) SetState(States.Show_ReadOnly);
                }
            }
            else
            {
                bool isMouseDown = ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && EventSystem.current.IsPointerOverGameObject());
                bool isMouseUp = ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && EventSystem.current.IsPointerOverGameObject());
                if (Instance.PreviewCardOriginCardSelect)
                {
                    if (Input.GetKeyUp(KeyCode.Escape))
                    {
                        Instance.HidePreviewCardPanel();
                    }
                    else if (Input.GetKeyUp(KeyCode.Tab))
                    {
                        SetState(States.Hide);
                        Instance.HidePreviewCardPanel();
                    }
                }
                else if (Instance.BuildRenamePanel.gameObject.activeSelf)
                {
                    if (Input.GetKeyUp(KeyCode.Escape)) Instance.BuildRenamePanel.HidePanel();
                    else if (Input.GetKeyUp(KeyCode.Tab))
                    {
                        Instance.BuildRenamePanel.HidePanel();
                    }
                }
                else
                {
                    if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Tab))
                    {
                        SetState(States.Hide);
                    }
                    else if (isMouseDown)
                    {
                        Instance.OnMouseDown();
                    }
                    else if (isMouseUp)
                    {
                        Instance.OnMouseUp();
                    }
                }
            }
        }

        private void ShowWindow()
        {
            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.SelectCardWindow);
            StartMenuManager.Instance.M_StateMachine.SetState(StartMenuManager.StateMachine.States.Hide);
            AudioManager.Instance.BGMLoopInList(new List<string> {"bgm/SelectCardMenu0", "bgm/SelectCardMenu1"});

            GameManager.Instance.StartBlurBackGround();
            Instance.Canvas.gameObject.SetActive(true);
            Instance.Canvas_BG.gameObject.SetActive(true);

            Instance.SelectWindowShowAnim.SetTrigger("Show");

            Instance.SelectAllButton.gameObject.SetActive(true);
            Instance.UnSelectAllButton.gameObject.SetActive(true);
            Instance.ConfirmButton.gameObject.SetActive(true);
            Instance.DeleteBuildButton.gameObject.SetActive(true);
            Instance.CreateNewBuildButton.enabled = true;

            Instance.UpgradeCardButton.enabled = true;
            Instance.DegradeCardButton.enabled = true;
            Instance.UpgradeCoinText.gameObject.SetActive(true);
            Instance.UpgradeCoin.gameObject.SetActive(true);
            Instance.DegradeCoinText.gameObject.SetActive(true);
            Instance.DegradeCoin.gameObject.SetActive(true);

            Instance.LifeSlider.interactable = true;
            Instance.EnergySlider.interactable = true;
            Instance.CoinSlider.interactable = false;
            Instance.CardNumberSlider.interactable = true;
        }

        private void ShowWindowReadOnly()
        {
            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.SelectCardWindow);
            if (StartMenuManager.Instance.M_StateMachine.GetState() == StartMenuManager.StateMachine.States.Show) StartMenuManager.Instance.M_StateMachine.SetState(StartMenuManager.StateMachine.States.Hide);
            GameManager.Instance.StartBlurBackGround();
            Instance.Canvas.gameObject.SetActive(true);
            Instance.Canvas_BG.gameObject.SetActive(true);

            Instance.SelectWindowShowAnim.SetTrigger("Show");

            Instance.SelectAllButton.gameObject.SetActive(false);
            Instance.UnSelectAllButton.gameObject.SetActive(false);
            Instance.ConfirmButton.gameObject.SetActive(false);
            Instance.DeleteBuildButton.gameObject.SetActive(false);
            Instance.CreateNewBuildButton.enabled = false;

            Instance.UpgradeCardButton.enabled = false;
            Instance.DegradeCardButton.enabled = false;
            Instance.UpgradeCoinText.gameObject.SetActive(false);
            Instance.UpgradeCoin.gameObject.SetActive(false);
            Instance.DegradeCoinText.gameObject.SetActive(false);
            Instance.DegradeCoin.gameObject.SetActive(false);

            Instance.LifeSlider.interactable = false;
            Instance.EnergySlider.interactable = false;
            Instance.CoinSlider.interactable = false;
            Instance.CardNumberSlider.interactable = false;
        }

        private void HideWindow()
        {
            HideWindowCore();
            if (Client.Instance.IsLogin()) StartMenuManager.Instance.M_StateMachine.SetState(StartMenuManager.StateMachine.States.Show);
        }


        private void HideWindowToPlaying()
        {
            HideWindowCore();
            MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.BattleNormal);
        }

        private void HideWindowCore()
        {
            Instance.SelectWindowShowAnim.SetTrigger("Reset");

            if (Instance.M_StateMachine.GetState() != StateMachine.States.Show_ReadOnly && Instance.CurrentEditBuildButton != null)
            {
                Instance.OnSaveBuildInfo();
            }

            //Instance.Canvas.gameObject.SetActive(false);
            Instance.Canvas_BG.gameObject.SetActive(false);
            AffixManager.Instance.HideAffixPanel();
            if (Instance.currentPreviewCard) Instance.currentPreviewCard.PoolRecycle();
            GameManager.Instance.StopBlurBackGround();
            MouseHoverManager.Instance.M_StateMachine.ReturnToPreviousState();
        }
    }

    private CardBase mouseLeftDownCard;
    private CardBase mouseRightDownCard;
    private Vector3 mouseDownPosition;

    private void OnMouseDown()
    {
        if (BuildRenamePanel.gameObject.activeSelf) return;

        if (PreviewCardPanel.activeSelf) return;

        mouseDownPosition = Input.mousePosition;
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycast;
        Physics.Raycast(ray, out raycast, 10f, cardSelectLayer);
        if (raycast.collider != null)
        {
            CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
            if (Input.GetMouseButtonDown(1))
            {
                if (card)
                {
                    mouseRightDownCard = card;
                }
                else
                {
                    mouseRightDownCard = null;
                }
            }
            else if (Input.GetMouseButtonDown(0) && card)
            {
                mouseLeftDownCard = card;
            }
        }
        else
        {
            mouseRightDownCard = null;
            mouseLeftDownCard = null;
        }
    }

    private void OnMouseUp()
    {
        if (BuildRenamePanel.gameObject.activeSelf) return;

        if (PreviewCardPanel.activeSelf) return;

        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycast;
        Physics.Raycast(ray, out raycast, 10f, cardSelectLayer);
        if (raycast.collider != null)
        {
            CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
            if (Input.GetMouseButtonUp(1))
            {
                if (card && mouseRightDownCard == card)
                {
                    ShowPreviewCardPanel(card);
                }
                else
                {
                    HidePreviewCardPanel();
                }
            }
            else if (Input.GetMouseButtonUp(0) && card)
            {
                if ((Input.mousePosition - mouseDownPosition).magnitude < 50)
                {
                    if (mouseLeftDownCard == card)
                    {
                        SelectCard(card, false);
                    }
                }
            }
        }
        else
        {
            HidePreviewCardPanel();
        }

        mouseLeftDownCard = null;
        mouseRightDownCard = null;
    }

    #region 卡片初始化

    public void AddAllCards()
    {
        foreach (CardInfo_Base cardInfo in AllCards.CardDict.Values)
        {
            if (cardInfo.CardID == 999 || cardInfo.CardID == 99) continue;
            if (cardInfo.UpgradeInfo.CardLevel > 1) continue;
            if (cardInfo.BaseInfo.Hide) continue;
            AddCardIntoCardSelectWindow(cardInfo);
        }
    }

    public void AddCardIntoCardSelectWindow(CardInfo_Base cardInfo)
    {
        CardBase newCard = CardBase.InstantiateCardByCardInfo(cardInfo, AllCardsContent, null, true);
        RefreshCardInSelectWindow(newCard, false);
        allCards.Add(newCard.CardInfo.CardID, newCard);
    }

    private static void RefreshCardInSelectWindow(CardBase newCard, bool isSelected)
    {
        newCard.transform.localScale = Vector3.one * 120;
        newCard.transform.rotation = Quaternion.Euler(90, 180, 0);
        if (isSelected)
        {
            newCard.BeBrightColor();
        }
        else
        {
            newCard.BeDimColor();
        }
    }

    #endregion
}