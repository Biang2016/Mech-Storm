using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoryPropertyForm_Chapter : PoolObject
{
    [SerializeField] private Text Label;
    [SerializeField] private Transform ChapterPropertyRowContainer;
    [SerializeField] private Button MoveUpButton;
    [SerializeField] private Button MoveDownButton;
    [SerializeField] private Button DeleteButton;
    [SerializeField] private Button SelectButton;
    [SerializeField] private Image SelectedBG;

    public override void PoolRecycle()
    {
        base.PoolRecycle();

        foreach (PropertyFormRow cpfr in ChapterPropertyFormRows)
        {
            cpfr.PoolRecycle();
        }

        ChapterPropertyFormRows.Clear();
        IsSelected = false;
        Cur_Chapter = null;
        LanguageManager.Instance.UnregisterText(Label);
    }

    public Chapter Cur_Chapter;
    private bool isSelected;

    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            SelectedBG.enabled = isSelected;
            Row_ChapterMapRoundCount?.SetReadOnly(!isSelected);
        }
    }

    public UnityAction OnSelected;

    private List<PropertyFormRow> ChapterPropertyFormRows = new List<PropertyFormRow>();
    private PropertyFormRow Row_ChapterMapRoundCount = null;

    public void Initialize(UnityAction onSelected, UnityAction onMoveUp, UnityAction onMoveDown, UnityAction onDeleteButtonClick, UnityAction onRefreshStory)
    {
        OnSelected = onSelected;

        LanguageManager.Instance.RegisterTextKey(Label, "StoryEditorPanel_ChapterLabel");
        SelectButton.onClick.RemoveAllListeners();
        SelectButton.onClick.AddListener(onSelected);
        MoveUpButton.onClick.RemoveAllListeners();
        MoveUpButton.onClick.AddListener(onMoveUp);
        MoveDownButton.onClick.RemoveAllListeners();
        MoveDownButton.onClick.AddListener(onMoveDown);
        DeleteButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.AddListener(
            delegate
            {
                onDeleteButtonClick();
                StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<StoryEditorPanel>().StoryPropertiesContainer));
            });

        foreach (PropertyFormRow cpfr in ChapterPropertyFormRows)
        {
            cpfr.PoolRecycle();
        }

        ChapterPropertyFormRows.Clear();

        PropertyFormRow Row_ChapterName_zh = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorPanel_ChapterNameText_zh", OnChapterNameChange_zh, out SetChapterName_zh);
        PropertyFormRow Row_ChapterName_en = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorPanel_ChapterNameText_en", OnChapterNameChange_en, out SetChapterName_en);
        Row_ChapterMapRoundCount = GeneralizeRow(PropertyFormRow.CardPropertyFormRowType.InputField, "StoryEditorPanel_ChapterMapRoundCountLabelText", delegate(string value_str) { OnChapterMapRoundCountChange(value_str, onRefreshStory); }, out SetChapterMapRoundCount);
        IsSelected = false;
    }

    private PropertyFormRow GeneralizeRow(PropertyFormRow.CardPropertyFormRowType type, string labelKey, UnityAction<string> onValueChange, out UnityAction<string> setValue, List<string> dropdownOptionList = null, UnityAction<string> onButtonClick = null)
    {
        PropertyFormRow pfr = PropertyFormRow.BaseInitialize(type, ChapterPropertyRowContainer, labelKey, onValueChange, out setValue, dropdownOptionList, onButtonClick);
        ChapterPropertyFormRows.Add(pfr);
        return pfr;
    }

    public void SetChapter(Chapter chapter)
    {
        Cur_Chapter = chapter;
        SetChapterName_zh(chapter.ChapterNames["zh"]);
        SetChapterName_en(chapter.ChapterNames["en"]);
        SetChapterMapRoundCount(chapter.ChapterMapRoundCount.ToString());
    }

    private UnityAction<string> SetChapterName_zh;

    private void OnChapterNameChange_zh(string value_str)
    {
        Cur_Chapter.ChapterNames["zh"] = value_str;
    }

    private UnityAction<string> SetChapterName_en;

    private void OnChapterNameChange_en(string value_str)
    {
        Cur_Chapter.ChapterNames["en"] = value_str;
    }

    private int RoundCountBefore = 0;
    private UnityAction<string> SetChapterMapRoundCount;
    private bool isReturning = false;

    private void OnChapterMapRoundCountChange(string value_str, UnityAction onRefreshStory)
    {
        if (int.TryParse(value_str, out int value))
        {
            if (isReturning)
            {
                Cur_Chapter.ChapterMapRoundCount = value;
                return;
            }

            if (value < Chapter.SystemMinMapRoundCount)
            {
                SetChapterMapRoundCount(Chapter.SystemMinMapRoundCount.ToString());
            }
            else if (value > Chapter.SystemMaxMapRoundCount)
            {
                SetChapterMapRoundCount(Chapter.SystemMaxMapRoundCount.ToString());
            }
            else
            {
                RoundCountBefore = Cur_Chapter.ChapterMapRoundCount;
                Cur_Chapter.ChapterMapRoundCount = value;
                if (isSelected)
                {
                    ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                    cp.Initialize(LanguageManager.Instance.GetText("StoryEditorPanel_Notice_ChangeChapterRoundCount"),
                        LanguageManager.Instance.GetText("Common_Yes"),
                        LanguageManager.Instance.GetText("Common_No"),
                        delegate
                        {
                            cp.CloseUIForm();
                            Cur_Chapter.Levels.Clear();
                            onRefreshStory();
                        },
                        delegate
                        {
                            cp.CloseUIForm();
                            isReturning = true;
                            SetChapterMapRoundCount(RoundCountBefore.ToString());
                            isReturning = false;
                        });
                    cp.UIType.IsESCClose = false;
                    cp.UIType.IsClickElsewhereClose = false;
                }
            }
        }
        else
        {
            SetChapterMapRoundCount(Chapter.SystemMaxMapRoundCount.ToString());
        }
    }
}