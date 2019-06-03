using UnityEngine;

public class StoryEditorPanel_ShopButton : StoryEditorPanel_LevelButton
{
    private StoryEditorPanel_ShopButton()
    {
    }

    [SerializeField] private Color CardCountSliderColor;
    private StoryEditorPanel_LevelButtonSliderBar CardCountSlider;

    protected override void ChildrenInitialize()
    {
        CardCountSlider = AddSlider("StoryEditorPanel_CardCountLabelText", CardCountSliderColor);
        CardCountSlider.SetValueText(((Shop) Level).ItemPrices.Count.ToString());
    }
}