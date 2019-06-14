using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardSelectPanel : MonoBehaviour
{
    [SerializeField] private Toggle ShowAllCardToggle;
    [SerializeField] private Image MaskPanelForCardLibrary;

    private Dictionary<int, CardBase> AllCards = new Dictionary<int, CardBase>();
    private Dictionary<int, CardSelectWindowCardContainer> AllCardContainers = new Dictionary<int, CardSelectWindowCardContainer>(); // 每张卡片都有一个容器
    [SerializeField] private GridLayoutGroup CardLibraryGridLayout;

    internal void Initialize(UnityAction<CardBase> leftClickCard, UnityAction<CardBase> rightClickCard, LevelPropertyForm_CardSelection row_CardSelection)
    {
        LeftClickCard = leftClickCard;
        RightClickCard = rightClickCard;
        Row_CardSelection = row_CardSelection;
        ShowAllCardToggle.onValueChanged.RemoveAllListeners();
        ShowAllCardToggle.isOn = false;
        ShowAllCardToggle.onValueChanged.AddListener(ShowAllCardSwitch);
        ShowAllCardToggle.isOn = true;

        foreach (CardInfo_Base cardInfo in global::AllCards.CardDict.Values)
        {
            if (cardInfo.CardID == (int) global::AllCards.EmptyCardTypes.EmptyCard) continue;
            if (cardInfo.CardID == (int) global::AllCards.EmptyCardTypes.NoCard) continue;
            if (cardInfo.BaseInfo.IsHide) continue;
            if (cardInfo.BaseInfo.IsTemp) continue;
            AddCardIntoGridLayout(cardInfo.Clone());
        }
    }

    private BuildCards BuildCards;

    internal void SetBuildCards(BuildCards buildCards, UnityAction gotoAction = null)
    {
        BuildCards = buildCards;
        Row_CardSelection.Initialize(buildCards);
        Row_CardSelection.SetButtonActions(
            gotoAction: gotoAction ?? delegate { },
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
                        buildCards.ClearAllCardCounts();
                        Row_CardSelection.Refresh();
                        SelectCardsByBuildCards(CardStatTypes.Total);
                    },
                    rightButtonClick: delegate { cp.CloseUIForm(); });
            },
            showCardStatTypeChange: SelectCardsByBuildCards);
        SelectCardsByBuildCards(CardStatTypes.Total);
    }

    private void ShowAllCardSwitch(bool showAllCard)
    {
        if (BuildCards != null)
        {
            SelectCardsByBuildCards(Cur_CardStatType);
        }
    }

    private CardBase mouseLeftDownCard;
    private CardBase mouseRightDownCard;
    private Vector3 mouseDownPosition;

    private UnityAction<CardBase> LeftClickCard;
    private UnityAction<CardBase> RightClickCard;

    internal void CardSelectPanelUpdate()
    {
        if (gameObject.activeInHierarchy && MaskPanelForCardLibrary.enabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycast;
                Physics.Raycast(ray, out raycast, 500f, GameManager.Instance.Layer_Cards);
                if (raycast.collider)
                {
                    CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                    if (card)
                    {
                        mouseDownPosition = Input.mousePosition;
                        mouseLeftDownCard = card;
                    }
                }
                else
                {
                    mouseRightDownCard = null;
                    mouseLeftDownCard = null;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycast;
                Physics.Raycast(ray, out raycast, 500f, GameManager.Instance.Layer_Cards);
                if (raycast.collider != null)
                {
                    CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                    if (card)
                    {
                        mouseDownPosition = Input.mousePosition;
                        mouseRightDownCard = card;
                    }
                }
                else
                {
                    mouseRightDownCard = null;
                    mouseLeftDownCard = null;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycast;
                Physics.Raycast(ray, out raycast, 500f, GameManager.Instance.Layer_Cards);
                if (raycast.collider != null)
                {
                    CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                    if (card)
                    {
                        if ((Input.mousePosition - mouseDownPosition).magnitude < 50)
                        {
                            if (mouseLeftDownCard == card)
                            {
                                LeftClickCard(card);
                            }
                        }
                    }
                }

                mouseLeftDownCard = null;
                mouseRightDownCard = null;
            }

            if (Input.GetMouseButtonUp(1))
            {
                Ray ray = UIManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycast;
                Physics.Raycast(ray, out raycast, 500f, GameManager.Instance.Layer_Cards);
                if (raycast.collider != null)
                {
                    CardBase card = raycast.collider.gameObject.GetComponent<CardBase>();
                    if (Input.GetMouseButtonUp(1))
                    {
                        if (card && mouseRightDownCard == card)
                        {
                            RightClickCard(card);
                        }
                    }
                }

                mouseLeftDownCard = null;
                mouseRightDownCard = null;
            }
        }
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

    internal void UnSelectAllCards()
    {
        foreach (KeyValuePair<int, CardBase> kv in AllCards)
        {
            RefreshCard(kv.Key, 0);
        }
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
                    bool show = ((!ShowAllCardToggle.isOn && kv.Value.CardSelectCount > 0) || ShowAllCardToggle.isOn) && typeMatch;
                    AllCardContainers[kv.Key].gameObject.SetActive(show);
                    RefreshCard(kv.Key, kv.Value.CardSelectCount);
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