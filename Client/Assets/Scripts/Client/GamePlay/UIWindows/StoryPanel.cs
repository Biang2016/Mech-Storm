using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StoryPanel : BaseUIForm
{
    private StoryPanel()
    {
    }

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: true,
            isClickElsewhereClose: false,
            uiForms_Type: UIFormTypes.Normal,
            uiForms_ShowMode: UIFormShowModes.HideOther,
            uiForm_LucencyType: UIFormLucencyTypes.Translucence);
    }

    [SerializeField] private RectTransform ChapterMapContainer;
    [SerializeField] private Button StartButton;

    public void InitiateStoryCanvas()
    {
        Cur_ChapterMap = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ChapterMap].AllocateGameObject<ChapterMap>(ChapterMapContainer);
        Cur_ChapterMap.Initialize(StoryManager.Instance.GetStory().Chapters[0]);
        Cur_ChapterMap.OnSelectChapterNode = SelectNode;
        Cur_ChapterMap.RefreshKnownLevels();
    }

    public ChapterMap Cur_ChapterMap;

    private void SelectNode(ChapterMapNode node)
    {
        StartButton.enabled = true;
        StartButton.onClick.RemoveAllListeners();
        StartButton.onClick.AddListener(delegate
        {
            if (node.Cur_Level != null)
            {
                UnityAction action = delegate { UIManager.Instance.GetBaseUIForm<StartMenuPanel>().StartGameCore(RoundManager.PlayMode.Single, Cur_ChapterMap.Cur_Chapter.ChapterID, node.Cur_Level.LevelID); };
                action.Invoke();
                CurrentStartGameAction = action;
            }
            else
            {
                ConfirmPanel cp = UIManager.Instance.ShowUIForms<ConfirmPanel>();
                cp.Initialize(LanguageManager.Instance.GetText("Notice_SelectEmptyLevel"),
                    LanguageManager.Instance.GetText("Common_Confirm"),
                    null,
                    delegate { cp.CloseUIForm(); },
                    null);
            }
        });
    }

    internal void UnSelectNode()
    {
        StartButton.onClick.RemoveAllListeners();
        StartButton.enabled = false;
    }

    public UnityAction CurrentStartGameAction;
}