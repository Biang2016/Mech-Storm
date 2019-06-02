using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoryPropertyForm_Chapters : PoolObject
{
    [SerializeField] private Text Label;
    [SerializeField] private Transform ChaptersRowContainer;
    [SerializeField] private Button AddButton;

    public override void PoolRecycle()
    {
        base.PoolRecycle();

        foreach (KeyValuePair<int, StoryPropertyForm_Chapter> kv in StoryChapterRows)
        {
            kv.Value.PoolRecycle();
        }

        StoryChapterRows.Clear();
        Cur_Chapters = null;
        LanguageManager.Instance.UnregisterText(Label);
    }

    public SortedDictionary<int, Chapter> Cur_Chapters;
    private Chapter SelectedChapter;
    private SortedDictionary<int, StoryPropertyForm_Chapter> StoryChapterRows = new SortedDictionary<int, StoryPropertyForm_Chapter>();

    public void Initialize(SortedDictionary<int, Chapter> chapters, UnityAction<Chapter> onChangeSelectedChapter, UnityAction<int> onChapterMapRoundCountChange)
    {
        Cur_Chapters = chapters;
        LanguageManager.Instance.RegisterTextKey(Label, "StoryEditorPanel_ChaptersLabel");
        AddButton.onClick.RemoveAllListeners();
        AddButton.onClick.AddListener(
            delegate
            {
                if (Cur_Chapters.Count < 3)
                {
                    int newChapterID = -1;
                    for (int i = 0; i < 3; i++)
                    {
                        if (!Cur_Chapters.ContainsKey(i))
                        {
                            newChapterID = i;
                            break;
                        }
                    }

                    Cur_Chapters.Add(newChapterID, new Chapter(
                        newChapterID,
                        new SortedDictionary<string, string> {{"zh", "新章节"}, {"en", "New Chapter"}},
                        new SortedDictionary<int, Level>(),
                        4));

                    Initialize(Cur_Chapters, onChangeSelectedChapter, onChapterMapRoundCountChange);
                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ChaptersRowContainer));
                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<StoryEditorPanel>().StoryPropertiesContainer));
                }
                else
                {
                    NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_StoryEditorWindow_CannotCreateChapter"), 0, 1f);
                }
            });

        foreach (KeyValuePair<int, StoryPropertyForm_Chapter> kv in StoryChapterRows)
        {
            kv.Value.PoolRecycle();
        }

        StoryChapterRows.Clear();

        foreach (KeyValuePair<int, Chapter> kv in Cur_Chapters)
        {
            StoryPropertyForm_Chapter chapterRow = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.StoryPropertyForm_Chapter].AllocateGameObject<StoryPropertyForm_Chapter>(ChaptersRowContainer);
            chapterRow.Initialize(
                onSelected: delegate
                {
                    if (SelectedChapter != chapterRow.Cur_Chapter)
                    {
                        foreach (KeyValuePair<int, StoryPropertyForm_Chapter> _kv in StoryChapterRows)
                        {
                            _kv.Value.IsSelected = false;
                        }

                        chapterRow.IsSelected = true;
                        onChangeSelectedChapter(chapterRow.Cur_Chapter);
                        SelectedChapter = chapterRow.Cur_Chapter;
                    }
                },
                onMoveUp: delegate
                {
                    int index = chapterRow.transform.GetSiblingIndex();
                    if (index > 0)
                    {
                        chapterRow.transform.SetSiblingIndex(index - 1);
                    }
                },
                onMoveDown: delegate
                {
                    int index = chapterRow.transform.GetSiblingIndex();
                    if (index < Cur_Chapters.Count - 1)
                    {
                        chapterRow.transform.SetSiblingIndex(index + 1);
                    }
                },
                onDeleteButtonClick: delegate
                {
                    Cur_Chapters.Remove(kv.Key);
                    Initialize(Cur_Chapters, onChangeSelectedChapter, onChapterMapRoundCountChange);
                },
                onChapterMapRoundCountChange: onChapterMapRoundCountChange);
            chapterRow.SetChapter(kv.Value);
            StoryChapterRows.Add(kv.Key, chapterRow);
        }

        foreach (KeyValuePair<int, StoryPropertyForm_Chapter> _kv in StoryChapterRows)
        {
            _kv.Value.IsSelected = false;
        }

        foreach (KeyValuePair<int, StoryPropertyForm_Chapter> kv in StoryChapterRows)
        {
            onChangeSelectedChapter(kv.Value.Cur_Chapter);
            SelectedChapter = kv.Value.Cur_Chapter;
            kv.Value.IsSelected = true;
            break;
        }
    }
}