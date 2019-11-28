using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardSelectPanel : MonoBehaviour
{
    [SerializeField] private Toggle ShowAllCardToggle;
    [SerializeField] private Text ShowAllCardToggleLabel;
    [SerializeField] private Button UnselectAllButton;
    [SerializeField] private Text UnselectAllButtonLabel;
    [SerializeField] private Button SelectOneOfEachButton;
    [SerializeField] private Text SelectOneOfEachButtonLabel;
    [SerializeField] private Image MaskPanelForCardLibrary;

    private Dictionary<int, CardBase> AllCards = new Dictionary<int, CardBase>();
    private Dictionary<int, CardSelectWindowCardContainer> AllCardContainers = new Dictionary<int, CardSelectWindowCardContainer>(); // 每张卡片都有一个容器
    [SerializeField] private GridLayoutGroup CardLibraryGridLayout;

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKeys(new List<ValueTuple<Text, string>>
        {
            (ShowAllCardToggleLabel, "CardSelectPanel_ShowAllCardToggleLabel"),
            (UnselectAllButtonLabel, "CardSelectPanel_UnselectAllButtonLabel"),
            (SelectOneOfEachButtonLabel, "CardSelectPanel_SelectOneOfEachButtonLabel"),
        });
    }

    private Editor_CardSelectModes M_SelectMode;

    internal void Initialize(Editor_CardSelectModes selectModes, bool showHideCards, bool showTempCards, UnityAction<CardBase> leftClickCard, UnityAction<CardBase> rightClickCard, UnityAction<CardBase> middleClickCard, UnityAction<List<int>> selectOneOfEachActiveCards, UnityAction<List<int>> unselectAllActiveCards, LevelPropertyForm_CardSelection row_CardSelection)
    {
        M_SelectMode = selectModes;

        LeftClickCard = leftClickCard;
        RightClickCard = rightClickCard;
        MiddleClickCard = middleClickCard;
        SelectOneOfEachActiveCardsAction = selectOneOfEachActiveCards;
        UnselectAllActiveCardsAction = unselectAllActiveCards;

        SelectOneOfEachButton.gameObject.SetActive(SelectOneOfEachActiveCardsAction != null);
        UnselectAllButton.gameObject.SetActive(UnselectAllActiveCardsAction != null);

        Row_CardSelection = row_CardSelection;
        UnselectAllButton.onClick.RemoveAllListeners();
        UnselectAllButton.onClick.AddListener(UnselectAllActiveCards);
        SelectOneOfEachButton.onClick.RemoveAllListeners();
        SelectOneOfEachButton.onClick.AddListener(SelectOneOfEachActiveCards);
        ShowAllCardToggle.onValueChanged.RemoveAllListeners();
        ShowAllCardToggle.isOn = true;
        ShowAllCardToggle.onValueChanged.AddListener(ShowAllCardSwitchForDeckSelect);
        ShowAllCardToggle.isOn = false;

        foreach (CardInfo_Base cardInfo in global::AllCards.CardDict.Values)
        {
            if (cardInfo.CardID == (int) global::AllCards.EmptyCardTypes.EmptyCard) continue;
            if (cardInfo.CardID == (int) global::AllCards.EmptyCardTypes.NoCard) continue;
            if (!showHideCards && cardInfo.BaseInfo.IsHide) continue;
            if (!showTempCards && cardInfo.BaseInfo.IsTemp) continue;
            AddCardIntoGridLayout(cardInfo.Clone());
        }
    }

    internal void SwitchSingleSelect(bool isSingleSelect)
    {
        ShowAllCardsForSingleSelect(isSingleSelect);
        if (isSingleSelect) UnselectAllCards();
        ShowAllCardToggle.gameObject.SetActive(!isSingleSelect);
        SelectOneOfEachButton.gameObject.SetActive(!isSingleSelect);
        UnselectAllButton.gameObject.SetActive(!isSingleSelect);
    }

    private BuildCards BuildCards;

    internal void SetBuildCards(BuildCards buildCards, UnityAction gotoAction)
    {
        BuildCards = buildCards;
        Row_CardSelection.Initialize(M_SelectMode, buildCards);
        Row_CardSelection.SetButtonActions(
            gotoAction: delegate
            {
                gotoAction();
                SwitchSingleSelect(false);
                SelectCardsByBuildCards(CardStatTypes.Total);
            },
            clearAction: delegate
            {
                ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                cp.Initialize(
                    descText: LanguageManager.Instance.GetText("LevelEditorPanel_ClearConfirm"),
                    leftButtonText: LanguageManager.Instance.GetText("Common_Yes"),
                    rightButtonText: LanguageManager.Instance.GetText("Common_No"),
                    leftButtonClick: delegate
                    {
                        cp.CloseUIForm();
                        if (M_SelectMode == Editor_CardSelectModes.SelectCount)
                        {
                            buildCards.ClearAllCardCounts();
                        }
                        else if (M_SelectMode == Editor_CardSelectModes.UpperLimit)
                        {
                            buildCards.ClearAllCardUpperLimit();
                        }

                        Row_CardSelection.Refresh();
                        SelectCardsByBuildCards(CardStatTypes.Total);
                    },
                    rightButtonClick: delegate { cp.CloseUIForm(); });
            },
            showCardStatTypeChange: SelectCardsByBuildCards);
        SelectCardsByBuildCards(CardStatTypes.Total);
    }

    private void ShowAllCardSwitchForDeckSelect(bool showAllCard)
    {
        if (BuildCards != null)
        {
            SelectCardsByBuildCards(Cur_CardStatType);
        }
    }

    private void ShowAllCardsForSingleSelect(bool showAllCard)
    {
        UnselectAllCards();
        foreach (KeyValuePair<int, CardSelectWindowCardContainer> kv in AllCardContainers)
        {
            kv.Value.gameObject.SetActive(true);
        }
    }

    private CardBase mouseLeftDownCard;
    private CardBase mouseMiddleDownCard;
    private CardBase mouseRightDownCard;
    private Vector3 mouseDownPosition;

    private UnityAction<CardBase> LeftClickCard;
    private UnityAction<CardBase> RightClickCard;
    private UnityAction<CardBase> MiddleClickCard;
    private UnityAction<List<int>> SelectOneOfEachActiveCardsAction;
    private UnityAction<List<int>> UnselectAllActiveCardsAction;

    internal void CardSelectPanelUpdate()
    {
        if (UIManager.Instance.GetBaseUIForm<ConfirmPanel>())
        {
            if (UIManager.Instance.GetBaseUIForm<ConfirmPanel>().isActiveAndEnabled) return;
        }

        if (gameObject.activeInHierarchy && MaskPanelForCardLibrary.enabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                CardBase card = RaycastGetCard();
                if (card)
                {
                    mouseDownPosition = Input.mousePosition;
                    mouseLeftDownCard = card;
                }
                else
                {
                    ResetClickRecord();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                CardBase card = RaycastGetCard();
                if (card)
                {
                    mouseDownPosition = Input.mousePosition;
                    mouseRightDownCard = card;
                }
                else
                {
                    ResetClickRecord();
                }
            }

            if (Input.GetMouseButtonDown(2))
            {
                CardBase card = RaycastGetCard();
                if (card)
                {
                    mouseDownPosition = Input.mousePosition;
                    mouseMiddleDownCard = card;
                }
                else
                {
                    ResetClickRecord();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                CardBase card = RaycastGetCard();
                if (card)
                {
                    if ((Input.mousePosition - mouseDownPosition).magnitude < 50)
                    {
                        if (mouseLeftDownCard == card)
                        {
                            LeftClickCard(card);
                        }
                        else
                        {
                            ResetClickRecord();
                        }
                    }
                    else
                    {
                        ResetClickRecord();
                    }
                }
                else
                {
                    ResetClickRecord();
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                CardBase card = RaycastGetCard();
                if (card && mouseRightDownCard == card)
                {
                    RightClickCard(card);
                }
                else
                {
                    ResetClickRecord();
                }
            }

            if (Input.GetMouseButtonUp(2))
            {
                CardBase card = RaycastGetCard();
                if (card && mouseMiddleDownCard == card)
                {
                    MiddleClickCard(card);
                }
                else
                {
                    ResetClickRecord();
                }
            }
        }
    }

    private CardBase RaycastGetCard()
    {
        Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycast;
        Physics.Raycast(ray, out raycast, 500f, GameManager.Instance.Layer_Cards);
        if (raycast.collider != null)
        {
            CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
            return card;
        }

        return null;
    }

    private void ResetClickRecord()
    {
        mouseLeftDownCard = null;
        mouseMiddleDownCard = null;
        mouseRightDownCard = null;
    }

    internal void SetCardLibraryPanelEnable(bool enable)
    {
        MaskPanelForCardLibrary.enabled = enable;
    }

    internal void RecycleAllCards()
    {
        foreach (KeyValuePair<int, CardSelectWindowCardContainer> kv in AllCardContainers)
        {
            kv.Value.PoolRecycle();
        }

        AllCardContainers.Clear();
        AllCards.Clear();
    }

    private void AddCardIntoGridLayout(CardInfo_Base cardInfo)
    {
        CardSelectWindowCardContainer newCardContainer = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CardSelectWindowCardContainer].AllocateGameObject<CardSelectWindowCardContainer>(CardLibraryGridLayout.transform);
        cardInfo.BaseInfo.LimitNum = 0;
        newCardContainer.Initialize(cardInfo);
        newCardContainer.M_ChildCard.BeDimColor();
        AllCards.Add(cardInfo.CardID, newCardContainer.M_ChildCard);
        AllCardContainers.Add(cardInfo.CardID, newCardContainer);
    }

    internal void RefreshCard(int cardID, int count)
    {
        if (AllCards.ContainsKey(cardID))
        {
            CardBase card = AllCards[cardID];
            card.SetBlockCountValue(count);
            if (count > 0)
            {
                card.BeBrightColor();
            }
            else
            {
                card.BeDimColor();
            }

            card.ShowCardBloom(count > 0);
        }
    }

    internal void UnselectAllCards()
    {
        foreach (KeyValuePair<int, CardBase> kv in AllCards)
        {
            RefreshCard(kv.Key, 0);
        }
    }

    private void UnselectAllActiveCards()
    {
        List<int> activeCardIDs = new List<int>();
        foreach (KeyValuePair<int, CardSelectWindowCardContainer> kv in AllCardContainers)
        {
            if (kv.Value.gameObject.activeInHierarchy)
            {
                activeCardIDs.Add(kv.Key);
            }
        }

        UnselectAllActiveCardsAction.Invoke(activeCardIDs);
    }

    private void SelectOneOfEachActiveCards()
    {
        List<int> activeCardIDs = new List<int>();
        foreach (KeyValuePair<int, CardSelectWindowCardContainer> kv in AllCardContainers)
        {
            if (kv.Value.gameObject.activeInHierarchy)
            {
                activeCardIDs.Add(kv.Key);
            }
        }

        SelectOneOfEachActiveCardsAction?.Invoke(activeCardIDs);
    }

    private LevelPropertyForm_CardSelection Row_CardSelection;
    private CardStatTypes Cur_CardStatType;

    internal void SelectCardsByBuildCards(CardStatTypes cardStatType)
    {
        Cur_CardStatType = cardStatType;
        if (BuildCards != null)
        {
            foreach (KeyValuePair<int, BuildCards.CardSelectInfo> kv in BuildCards.CardSelectInfos)
            {
                if (AllCards.ContainsKey(kv.Key))
                {
                    bool typeMatch = cardStatType == CardStatTypes.Total || AllCards[kv.Key].CardInfo.CardStatType == cardStatType;
                    if (M_SelectMode == Editor_CardSelectModes.SelectCount)
                    {
                        bool show = ((!ShowAllCardToggle.isOn && kv.Value.CardSelectCount > 0) || ShowAllCardToggle.isOn) && typeMatch;
                        AllCardContainers[kv.Key].gameObject.SetActive(show);
                        RefreshCard(kv.Key, kv.Value.CardSelectCount);
                    }
                    else if (M_SelectMode == Editor_CardSelectModes.UpperLimit)
                    {
                        bool show = ((!ShowAllCardToggle.isOn && kv.Value.CardSelectUpperLimit > 0) || ShowAllCardToggle.isOn) && typeMatch;
                        AllCardContainers[kv.Key].gameObject.SetActive(show);
                        RefreshCard(kv.Key, kv.Value.CardSelectUpperLimit);
                    }
                }
            }
        }
    }

    internal void OnLanguageChange(int _)
    {
        foreach (KeyValuePair<int, CardBase> kv in AllCards)
        {
            kv.Value.RefreshCardTextLanguage();
        }
    }
}