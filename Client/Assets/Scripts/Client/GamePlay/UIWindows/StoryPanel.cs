using System;
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

        ReturnButton.onClick.AddListener(OnReturnButtonClick);
    }

    [SerializeField] private RectTransform ChapterMapContainer;
    [SerializeField] private Button StartButton;
    [SerializeField] private Button ReturnButton;
    [SerializeField] private Text ChapterNameText;

    public void InitiateStoryCanvas()
    {
        Cur_ChapterMap?.PoolRecycle();
        Cur_ChapterMap = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ChapterMap].AllocateGameObject<ChapterMap>(ChapterMapContainer);
        Cur_ChapterMap.Initialize(StoryManager.Instance.GetStory().Chapters[0]);
        ChapterNameText.text = string.Format(LanguageManager.Instance.GetText("StoryEditorPanel_ChapterTitle"), Cur_ChapterMap.Cur_Chapter.ChapterID + 1, Cur_ChapterMap.Cur_Chapter.ChapterNames[LanguageManager.Instance.GetCurrentLanguage()]);
        Cur_ChapterMap.OnSelectChapterNode = SelectNode;
        Cur_ChapterMap.RefreshKnownLevels();
        StartButton.gameObject.SetActive(false);
    }

    public override void Display()
    {
        base.Display();
        AudioManager.Instance.BGMFadeIn("bgm/StoryPanel");
        UIManager.Instance.GetBaseUIForm<StoryPlayerInformationPanel>().Display();
    }

    public override void Hide()
    {
        base.Hide();
        UIManager.Instance.GetBaseUIForm<StoryPlayerInformationPanel>().Hide();
    }

    internal ChapterMap Cur_ChapterMap;

    private void SelectNode(ChapterMapNode node)
    {
        StartButton.gameObject.SetActive(true);
        StartButton.onClick.RemoveAllListeners();
        StartButton.onClick.AddListener(delegate
        {
            if (node.Cur_Level != null)
            {
                if (node.IsBeated)
                {
                    NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("StoryPanel_CannotStartBeatedLevel"), 0, 1f);
                }
                else
                {
                    switch (node.Cur_Level)
                    {
                        case Enemy enemy:
                        {
                            UnityAction action = delegate { UIManager.Instance.GetBaseUIForm<StartMenuPanel>().StartGameCore(RoundManager.PlayMode.Single, Cur_ChapterMap.Cur_Chapter.ChapterID, node.Cur_Level.LevelID); };
                            action.Invoke();
                            CurrentStartGameAction = action;
                            break;
                        }
                        case Shop shop:
                        {
                            StandaloneStartLevelRequest request = new StandaloneStartLevelRequest(Client.Instance.Proxy.ClientID, -1, Cur_ChapterMap.Cur_Chapter.ChapterID, node.Cur_Level.LevelID);
                            Client.Instance.Proxy.SendMessage(request);
                            break;
                        }
                    }
                }
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
        StartButton.gameObject.SetActive(false);
    }

    public UnityAction CurrentStartGameAction;

    private void OnReturnButtonClick()
    {
        CloseUIForm();
    }
}