using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelEditorPanel_CardPriority : PropertyFormRow, ILevelEditorPanel_CardComboPriorityCardContainer
{
    [SerializeField] private Transform CardRowContainer;
    [SerializeField] private Button SelectThisCardListButton;
    [SerializeField] private Image SelectedImage;
    [SerializeField] private Button ClearButton;

    private CardPriority Cur_CardPriority;
    private List<PropertyFormRow> My_CardRows = new List<PropertyFormRow>();

    private bool isSelected = false;

    public bool IsSelected {
        get { return isSelected; }
        set {
            if (isSelected != value)
            {
                if (!isSelected && value)
                {
                    OnSelect?.Invoke(this);
                }

                isSelected = value;
            }

            SelectedImage.enabled = isSelected;
        }
    }

    public UnityAction<ILevelEditorPanel_CardComboPriorityCardContainer> OnSelect { get; set; }

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        Clear();
        ClearButton.onClick.RemoveAllListeners();
    }

    protected override void SetValue(string value_str, bool forceChange = false)
    {
    }

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKey(Label, "LevelEditorPanel_CardPriority");
    }

    public void Initialize(CardPriority cardPriority)
    {
        Clear();
        Cur_CardPriority = cardPriority;
        ClearButton.onClick.RemoveAllListeners();
        ClearButton.onClick.AddListener(delegate
        {
            Clear();
            Cur_CardPriority.Clear();
            Initialize(Cur_CardPriority);
            UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().StartCoroutine(ClientUtils.UpdateLayout(UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().RightPanel));
        });
        SelectThisCardListButton.onClick.RemoveAllListeners();
        SelectThisCardListButton.onClick.AddListener(delegate { IsSelected = true; });
        SetCardPriority(Cur_CardPriority);
    }

    private PropertyFormRow GeneralizeRow(int cardID)
    {
        CardInfo_Base ci = AllCards.GetCard(cardID);
        if (ci != null)
        {
            LevelEditorPanel_CardRow cr = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelEditorPanel_CardRow].AllocateGameObject<LevelEditorPanel_CardRow>(CardRowContainer);
            cr.Initialize(
                ci,
                onMoveUp: MoveUpCard,
                onMoveDown: MoveDownCard,
                onRemove: RemoveCard
            );
            My_CardRows.Add(cr);
            return cr;
        }
        else
        {
            return null;
        }
    }

    public void Clear()
    {
        foreach (LevelEditorPanel_CardRow cr in My_CardRows)
        {
            cr.PoolRecycle();
        }

        My_CardRows.Clear();
    }

    private void SetCardPriority(CardPriority cardPriority)
    {
        Clear();
        Cur_CardPriority = cardPriority;
        foreach (int id in cardPriority.CardIDListByPriority)
        {
            GeneralizeRow(id);
        }

        StartCoroutine(ClientUtils.UpdateLayout(UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().RightPanel));
    }

    public void AddCard(int cardID)
    {
        if (!Cur_CardPriority.CardIDListByPriority.Contains(cardID))
        {
            Cur_CardPriority.CardIDListByPriority.Add(cardID);
            SetCardPriority(Cur_CardPriority);
        }
    }

    public void RemoveCard(int cardID)
    {
        Cur_CardPriority.CardIDListByPriority.Remove(cardID);
        SetCardPriority(Cur_CardPriority);
    }

    public void MoveUpCard(int cardID)
    {
        int index = Cur_CardPriority.CardIDListByPriority.IndexOf(cardID);
        if (index > 0)
        {
            Cur_CardPriority.CardIDListByPriority.Remove(cardID);
            Cur_CardPriority.CardIDListByPriority.Insert(index - 1, cardID);
            SetCardPriority(Cur_CardPriority);
        }
    }

    public void MoveDownCard(int cardID)
    {
        int index = Cur_CardPriority.CardIDListByPriority.IndexOf(cardID);
        if (index >= 0 && index < Cur_CardPriority.CardIDListByPriority.Count - 1)
        {
            Cur_CardPriority.CardIDListByPriority.Remove(cardID);
            Cur_CardPriority.CardIDListByPriority.Insert(index + 1, cardID);
            SetCardPriority(Cur_CardPriority);
        }
    }

    public void OnLanguageChange()
    {
        foreach (LevelEditorPanel_CardRow cr in My_CardRows)
        {
            cr.OnLanguageChange();
        }
    }
}