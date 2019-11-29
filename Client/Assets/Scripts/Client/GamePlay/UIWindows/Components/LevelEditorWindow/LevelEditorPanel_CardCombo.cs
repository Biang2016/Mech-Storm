using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelEditorPanel_CardCombo : PropertyFormRow, ILevelEditorPanel_CardComboPriorityCardContainer
{
    [SerializeField] private Transform CardRowContainer;
    [SerializeField] private Button SelectThisCardListButton;
    [SerializeField] private Image SelectedImage;
    [SerializeField] private Button MoveUpButton;
    [SerializeField] private Button MoveDownButton;
    [SerializeField] private Button DeleteButton;
    [SerializeField] private Button ClearButton;

    private CardCombo Cur_CardCombo;
    private List<PropertyFormRow> My_CardRows = new List<PropertyFormRow>();

    private bool isSelected = false;

    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
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
        DeleteButton.onClick.RemoveAllListeners();
        ClearButton.onClick.RemoveAllListeners();
    }

    protected override void SetValue(string value_str, bool forceChange = false)
    {
    }

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKey(Label, "LevelEditorPanel_CardCombo");
    }

    public void Initialize(CardCombo cardCombo, UnityAction<CardCombo> deleteAction, UnityAction<CardCombo> moveUpButton, UnityAction<CardCombo> moveDownButton)
    {
        Clear();
        Cur_CardCombo = cardCombo;
        MoveUpButton.onClick.RemoveAllListeners();
        MoveUpButton.onClick.AddListener(delegate { moveUpButton(Cur_CardCombo); });
        MoveDownButton.onClick.RemoveAllListeners();
        MoveDownButton.onClick.AddListener(delegate { moveDownButton(Cur_CardCombo); });
        DeleteButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.AddListener(delegate { deleteAction(Cur_CardCombo); });
        ClearButton.onClick.RemoveAllListeners();
        ClearButton.onClick.AddListener(delegate
        {
            Clear();
            Cur_CardCombo.Clear();
            Initialize(Cur_CardCombo, deleteAction, moveUpButton, moveDownButton);
            UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().StartCoroutine(ClientUtils.UpdateLayout(UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().RightPanel));
        });
        SelectThisCardListButton.onClick.RemoveAllListeners();
        SelectThisCardListButton.onClick.AddListener(delegate { IsSelected = true; });
        SetCardCombo(cardCombo);
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

    private void SetCardCombo(CardCombo cardCombo)
    {
        Clear();
        Cur_CardCombo = cardCombo;
        foreach (int id in cardCombo.ComboCardIDList)
        {
            GeneralizeRow(id);
        }

        StartCoroutine(ClientUtils.UpdateLayout(UIManager.Instance.GetBaseUIForm<LevelEditorPanel>().RightPanel));
    }

    public void AddCard(int cardID)
    {
        if (!Cur_CardCombo.ComboCardIDList.Contains(cardID))
        {
            Cur_CardCombo.ComboCardIDList.Add(cardID);
            SetCardCombo(Cur_CardCombo);
        }
    }

    public void RemoveCard(int cardID)
    {
        Cur_CardCombo.ComboCardIDList.Remove(cardID);
        SetCardCombo(Cur_CardCombo);
    }

    public void MoveUpCard(int cardID)
    {
        int index = Cur_CardCombo.ComboCardIDList.IndexOf(cardID);
        if (index > 0)
        {
            Cur_CardCombo.ComboCardIDList.Remove(cardID);
            Cur_CardCombo.ComboCardIDList.Insert(index - 1, cardID);
            SetCardCombo(Cur_CardCombo);
        }
    }

    public void MoveDownCard(int cardID)
    {
        int index = Cur_CardCombo.ComboCardIDList.IndexOf(cardID);
        if (index >= 0 && index < Cur_CardCombo.ComboCardIDList.Count - 1)
        {
            Cur_CardCombo.ComboCardIDList.Remove(cardID);
            Cur_CardCombo.ComboCardIDList.Insert(index + 1, cardID);
            SetCardCombo(Cur_CardCombo);
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