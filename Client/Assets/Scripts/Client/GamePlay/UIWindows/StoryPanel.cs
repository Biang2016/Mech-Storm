using UnityEngine;

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
            uiForms_ShowMode: UIFormShowModes.ReturnHideOther,
            uiForm_LucencyType: UIFormLucencyTypes.ImPenetrable);
    }

    [SerializeField] private RectTransform ChapterMapContainer;

    public void InitiateStoryCanvas()
    {
        ChapterMap ChapterMap = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.ChapterMap].AllocateGameObject<ChapterMap>(ChapterMapContainer);
        ChapterMap.Initialize(StoryManager.Instance.GetStory().Chapters[0]);
    }
}