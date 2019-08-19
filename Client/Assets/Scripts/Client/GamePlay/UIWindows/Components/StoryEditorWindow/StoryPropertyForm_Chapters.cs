using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoryPropertyForm_Chapters : PoolObject
{
    public const int MAX_CHAPTER_NUM = 10;

    [SerializeField] private Text Label;
    [SerializeField] private Transform ChaptersRowContainer;
    [SerializeField] private Button AddButton;
    [SerializeField] private Button GotoButton;

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

    public void Initialize(SortedDictionary<int, Chapter> chapters, UnityAction gotoAction, UnityAction<Chapter, bool> onChangeSelectedChapter, UnityAction onSaveChapter, UnityAction onRefreshStory, UnityAction<Chapter> onRefreshChapterTitle)
    {
        Cur_Chapters = chapters;
        LanguageManager.Instance.RegisterTextKey(Label, "StoryEditorPanel_ChaptersLabel");

        AddButton.onClick.RemoveAllListeners();
        AddButton.onClick.AddListener(
            delegate
            {
                if (Cur_Chapters.Count < MAX_CHAPTER_NUM)
                {
                    int newChapterID = -1;
                    for (int i = 0; i < MAX_CHAPTER_NUM; i++)
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
                        chapterAllLevels: new SortedDictionary<int, Level>(),
                        4,
                        allRoutes: new SortedDictionary<int, HashSet<int>>()
                    ));

                    Initialize(Cur_Chapters, gotoAction, onChangeSelectedChapter, onSaveChapter, onRefreshStory, onRefreshChapterTitle);
                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) ChaptersRowContainer));
                    StartCoroutine(ClientUtils.UpdateLayout((RectTransform) UIManager.Instance.GetBaseUIForm<StoryEditorPanel>().StoryPropertiesContainer));
                }
                else
                {
                    NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("Notice_StoryEditorWindow_CannotCreateChapter"), 0, 1f);
                }
            });
        GotoButton.onClick.RemoveAllListeners();
        GotoButton.onClick.AddListener(gotoAction);

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
                    gotoAction();
                    if (SelectedChapter != chapterRow.Cur_Chapter)
                    {
                        foreach (KeyValuePair<int, StoryPropertyForm_Chapter> _kv in StoryChapterRows)
                        {
                            _kv.Value.IsSelected = false;
                        }

                        chapterRow.IsSelected = true;
                        onChangeSelectedChapter(chapterRow.Cur_Chapter, false);
                        SelectedChapter = chapterRow.Cur_Chapter;
                    }
                },
                onMoveUp: delegate
                {
                    int index = chapterRow.transform.GetSiblingIndex();
                    if (index > 0)
                    {
                        Chapter curChapter = chapters[index];
                        Chapter swapChapter = chapters[index - 1];
                        chapters.Remove(index);
                        chapters.Remove(index - 1);
                        chapters.Add(index - 1, curChapter);
                        chapters.Add(index, swapChapter);
                        curChapter.ChapterID = index - 1;
                        swapChapter.ChapterID = index;
                        chapterRow.transform.SetSiblingIndex(index - 1);

                        StoryPropertyForm_Chapter curChapterRow = StoryChapterRows[index];
                        StoryPropertyForm_Chapter swapChapterRow = StoryChapterRows[index - 1];
                        StoryChapterRows.Remove(index);
                        StoryChapterRows.Remove(index - 1);
                        StoryChapterRows.Add(index - 1, curChapterRow);
                        StoryChapterRows.Add(index, swapChapterRow);

                        onRefreshChapterTitle(kv.Value);
                    }
                },
                onMoveDown: delegate
                {
                    int index = chapterRow.transform.GetSiblingIndex();
                    if (index < Cur_Chapters.Count - 1)
                    {
                        Chapter curChapter = chapters[index];
                        Chapter swapChapter = chapters[index + 1];
                        chapters.Remove(index);
                        chapters.Remove(index + 1);
                        chapters.Add(index + 1, curChapter);
                        chapters.Add(index, swapChapter);
                        curChapter.ChapterID = index + 1;
                        swapChapter.ChapterID = index;
                        chapterRow.transform.SetSiblingIndex(index + 1);

                        StoryPropertyForm_Chapter curChapterRow = StoryChapterRows[index];
                        StoryPropertyForm_Chapter swapChapterRow = StoryChapterRows[index + 1];
                        StoryChapterRows.Remove(index);
                        StoryChapterRows.Remove(index + 1);
                        StoryChapterRows.Add(index + 1, curChapterRow);
                        StoryChapterRows.Add(index, swapChapterRow);

                        onRefreshChapterTitle(kv.Value);
                    }
                },
                onDeleteButtonClick: delegate
                {
                    Cur_Chapters.Remove(kv.Key);
                    Initialize(Cur_Chapters, gotoAction, onChangeSelectedChapter, onSaveChapter, onRefreshStory, onRefreshChapterTitle);
                },
                onSaveChapter: onSaveChapter,
                onRefreshStory: onRefreshStory);
            chapterRow.SetChapter(kv.Value);
            StoryChapterRows.Add(kv.Key, chapterRow);
        }

        foreach (KeyValuePair<int, StoryPropertyForm_Chapter> _kv in StoryChapterRows)
        {
            _kv.Value.IsSelected = false;
        }

        foreach (KeyValuePair<int, StoryPropertyForm_Chapter> kv in StoryChapterRows)
        {
            onChangeSelectedChapter(kv.Value.Cur_Chapter, false);
            SelectedChapter = kv.Value.Cur_Chapter;
            kv.Value.IsSelected = true;
            break;
        }
    }

    public void OnSelectedRow(int index)
    {
        StoryChapterRows[index].OnSelected();
    }
}