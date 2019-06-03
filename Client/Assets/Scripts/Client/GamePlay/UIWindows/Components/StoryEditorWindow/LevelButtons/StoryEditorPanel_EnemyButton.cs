using UnityEngine;
using UnityEngine.UI;

public class StoryEditorPanel_EnemyButton : StoryEditorPanel_LevelButton
{
    private StoryEditorPanel_EnemyButton()
    {
    }

    [SerializeField] private Color CardCountSliderColor;
    [SerializeField] private Color LifeSliderColor;
    [SerializeField] private Color EnergySliderColor;

    private StoryEditorPanel_LevelButtonSliderBar CardCountSlider;
    private StoryEditorPanel_LevelButtonSliderBar LifeSlider;
    private StoryEditorPanel_LevelButtonSliderBar EnergySlider;

    [SerializeField] private Text DrawCardNumText;
    [SerializeField] private Text BeginMetalNumText;

    protected override void ChildrenInitialize()
    {
        CardCountSlider = AddSlider("StoryEditorPanel_CardCountLabelText", CardCountSliderColor);
        CardCountSlider.SetValueText(((Enemy) Level).BuildInfo.CardCount.ToString());
        CardCountSlider.ChangeSliderValue((float)((Enemy)Level).BuildInfo.CardCount / 50);

        LifeSlider = AddSlider("StoryEditorPanel_LifeLabelText", LifeSliderColor);
        LifeSlider.SetValueText(((Enemy) Level).BuildInfo.Life.ToString());
        SetLifeBarLengthColor(((Enemy) Level).BuildInfo.Life);

        EnergySlider = AddSlider("StoryEditorPanel_EnergyLabelText", EnergySliderColor);
        EnergySlider.SetValueText(((Enemy) Level).BuildInfo.Energy.ToString());
        SetEnergyBarLengthColor(((Enemy) Level).BuildInfo.Energy);

        DrawCardNumText.text = ((Enemy)Level).BuildInfo.DrawCardNum.ToString();
        BeginMetalNumText.text = ((Enemy)Level).BuildInfo.BeginMetal.ToString();
    }

    private int[] lifeBarColorThresholds = new[] {10, 50, 100, 500, 1000, 5000, 10000};
    public Color[] LifeBarColors;

    private void SetLifeBarLengthColor(int life)
    {
        for (int i = 0; i < lifeBarColorThresholds.Length; i++)
        {
            if (life <= lifeBarColorThresholds[i])
            {
                LifeSlider.ChangeSliderValue((float) life / lifeBarColorThresholds[i]);
                LifeSlider.ChangeSliderColor(LifeBarColors[i]);
                return;
            }
        }

        LifeSlider.ChangeSliderValue((float) life / lifeBarColorThresholds[lifeBarColorThresholds.Length - 1]);
        LifeSlider.ChangeSliderColor(LifeBarColors[LifeBarColors.Length - 1]);
    }

    private int[] energyBarColorThresholds = new[] {10, 50, 100, 200};
    public Color[] EnergyBarColors;

    private void SetEnergyBarLengthColor(int energy)
    {
        for (int i = 0; i < energyBarColorThresholds.Length; i++)
        {
            if (energy <= energyBarColorThresholds[i])
            {
                EnergySlider.ChangeSliderValue((float) energy / energyBarColorThresholds[i]);
                EnergySlider.ChangeSliderColor(EnergyBarColors[i]);
                return;
            }
        }

        EnergySlider.ChangeSliderValue((float) energy / energyBarColorThresholds[energyBarColorThresholds.Length - 1]);
        EnergySlider.ChangeSliderColor(EnergyBarColors[EnergyBarColors.Length - 1]);
    }
}