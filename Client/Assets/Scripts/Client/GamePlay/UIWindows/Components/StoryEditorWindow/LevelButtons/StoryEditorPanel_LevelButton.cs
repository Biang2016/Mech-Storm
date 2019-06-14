using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class StoryEditorPanel_LevelButton : PoolObject
{
    public override void PoolRecycle()
    {
        SetButton.onClick.RemoveAllListeners();
        EditButton.onClick.RemoveAllListeners();
        DeleteButton.onClick.RemoveAllListeners();

        foreach (StoryEditorPanel_LevelButtonSliderBar slider in Sliders)
        {
            slider.PoolRecycle();
        }

        Sliders.Clear();
        base.PoolRecycle();
    }

    [SerializeField] private Image PicImage;
    [SerializeField] private Text LevelNameText;
    [SerializeField] private Button SetButton;
    [SerializeField] private Button EditButton;
    [SerializeField] private Button DeleteButton;
    [SerializeField] private Transform SliderBarContainer;

    public Level Cur_Level;

    protected List<StoryEditorPanel_LevelButtonSliderBar> Sliders = new List<StoryEditorPanel_LevelButtonSliderBar>();

    public static StoryEditorPanel_LevelButton BaseInitialize(Level level, Transform parent, UnityAction<Level> onSetButtonClick, UnityAction onEditButtonClick, UnityAction onDeleteButtonClick)
    {
        StoryEditorPanel_LevelButton btn = null;
        switch (level.LevelType)
        {
            case LevelType.Enemy:
            {
                btn = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.StoryEditorPanel_EnemyButton].AllocateGameObject<StoryEditorPanel_EnemyButton>(parent);
                break;
            }
            case LevelType.Shop:
            {
                btn = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.StoryEditorPanel_ShopButton].AllocateGameObject<StoryEditorPanel_ShopButton>(parent);
                break;
            }
        }

        btn.Cur_Level = level;
        btn.SetButton.onClick.RemoveAllListeners();
        btn.SetButton.onClick.AddListener(delegate { onSetButtonClick(btn.Cur_Level); });
        btn.EditButton.onClick.RemoveAllListeners();
        btn.EditButton.onClick.AddListener(onEditButtonClick);
        btn.DeleteButton.onClick.RemoveAllListeners();
        btn.DeleteButton.onClick.AddListener(onDeleteButtonClick);

        ClientUtils.ChangeImagePicture(btn.PicImage, btn.Cur_Level.LevelPicID);
        btn.LevelNameText.text = btn.Cur_Level.LevelNames[LanguageManager.Instance.GetCurrentLanguage()];

        foreach (StoryEditorPanel_LevelButtonSliderBar slider in btn.Sliders)
        {
            slider.PoolRecycle();
        }

        btn.Sliders.Clear();

        btn.ChildrenInitialize();
        return btn;
    }

    protected abstract void ChildrenInitialize();

    protected StoryEditorPanel_LevelButtonSliderBar AddSlider(string labelStrKey, Color color)
    {
        StoryEditorPanel_LevelButtonSliderBar slider = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.StoryEditorPanel_LevelButtonSliderBar].AllocateGameObject<StoryEditorPanel_LevelButtonSliderBar>(SliderBarContainer);
        slider.Initialize(labelStrKey, color);
        Sliders.Add(slider);
        return slider;
    }

    public void OnLanguageChange()
    {
        LevelNameText.text = Cur_Level.LevelNames[LanguageManager.Instance.GetCurrentLanguage()];
    }
}