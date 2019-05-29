using UnityEngine;
using UnityEngine.UI;

public class CostStatBar : PoolObject
{
    private CostStatBar()
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
    [SerializeField] private Transform CountTextPivot;

    public Color[] ColorPresets;

    public void Initialize(int number, int count, int maxCount, ColorTypes colorType)
    {
        BarImage.color = ColorPresets[(int) colorType];
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

        CountText.transform.position = CountTextPivot.position;
    }

    public enum ColorTypes
    {
        Metal = 0,
        Energy = 1,
    }
}