using UnityEngine;

public class StoryEditorPanel_ShopButton : StoryEditorPanel_LevelButton
{
    private StoryEditorPanel_ShopButton()
    {
    }

    [SerializeField] private Color CardCountSliderColor;
    private StoryEditorPanel_LevelButtonSliderBar CardCountSlider;
    [SerializeField] private Color AVGPriceSliderColor;
    private StoryEditorPanel_LevelButtonSliderBar AVGPriceSlider;

    protected override void ChildrenInitialize()
    {
        CardCountSlider = AddSlider("StoryEditorPanel_ShopItemCountLabelText", CardCountSliderColor);
        CardCountSlider.SetValueText(((Shop) Cur_Level).ShopItems.Count.ToString());
        CardCountSlider.ChangeSliderValue((float) ((Shop) Cur_Level).ShopItems.Count / 20);
        AVGPriceSlider = AddSlider("StoryEditorPanel_ShopItemAVGPriceLabelText", AVGPriceSliderColor);
        AVGPriceSlider.SetValueText(((Shop) Cur_Level).AVGPrice.ToString());
        CardCountSlider.ChangeSliderValue((float) ((Shop) Cur_Level).AVGPrice / 200);
    }
}