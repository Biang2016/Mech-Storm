using UnityEngine;
using UnityEngine.UI;

public class LevelEditorPanel_CostStatBar : PoolObject
{
    private LevelEditorPanel_CostStatBar()
    {
    }

    public override void PoolRecycle()
    {
        base.PoolRecycle();
    }

    [SerializeField] private Image BarImage;
    [SerializeField] private Text NumberText;
    [SerializeField] private Text CountText;
    [SerializeField] private Slider SliderBar;

    public Color[] ColorPresets;

    public void Initialize(int number, int count, int maxCount, ColorTypes colorType, Image baseLine)
    {
        BarImage.color = ColorPresets[(int) colorType];
        baseLine.color = ColorPresets[(int) colorType];
        NumberText.color = ColorPresets[(int) colorType];
        NumberText.text = number != 10 ? number.ToString() : "10+";
        CountText.text = count > 0 ? count.ToString() : "";
        if (maxCount == 0)
        {
            SliderBar.value = 0;
        }
        else
        {
            SliderBar.value = (float) count / maxCount;
        }
    }

    public enum ColorTypes
    {
        Metal = 0,
        Energy = 1,
    }
}