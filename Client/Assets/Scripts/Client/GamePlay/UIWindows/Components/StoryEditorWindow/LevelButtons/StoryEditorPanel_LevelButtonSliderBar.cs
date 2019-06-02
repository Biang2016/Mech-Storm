using UnityEngine;
using UnityEngine.UI;

public class StoryEditorPanel_LevelButtonSliderBar : PoolObject
{
    public override void PoolRecycle()
    {
        LanguageManager.Instance.UnregisterText(LabelText);
        base.PoolRecycle();
    }

    [SerializeField] private Slider Slider;
    [SerializeField] private Text LabelText;
    [SerializeField] private Text ValueText;
    [SerializeField] private Image SliderFill;

    public void Initialize(string labelStrKey, Color color)
    {
        LanguageManager.Instance.RegisterTextKey(LabelText, labelStrKey);
        SliderFill.color = color;
    }

    public void ChangeSliderValue(float value)
    {
        Slider.value = value;
    }

    public void SetValueText(string value)
    {
        ValueText.text = value;
    }

    public void ChangeSliderColor(Color color)
    {
        SliderFill.color = color;
    }
}