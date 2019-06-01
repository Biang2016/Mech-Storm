using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryEditorPanel_EnemyButton : PoolObject
{
    private StoryEditorPanel_EnemyButton()
    {
    }

    public override void PoolRecycle()
    {
        base.PoolRecycle();
    }

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKeys(
            new List<(Text, string)>
            {
                (CardCountLabelText, "StoryEditorPanel_CardCountLabelText"),
                (LifeLabelText, "SettingMenu_LifeLabelText"),
                (EnergyLabelText, "StoryEditorPanel_EnergyLabelText"),
            });
    }

    [SerializeField] private Image PicImage;
    [SerializeField] private Text LevelNameText;
    [SerializeField] private Slider CardCountSlider;
    [SerializeField] private Text CardCountLabelText;
    [SerializeField] private Text CardCountText;
    [SerializeField] private Image CardCountSliderFill;
    [SerializeField] private Slider LifeSlider;
    [SerializeField] private Text LifeLabelText;
    [SerializeField] private Text LifeValueText;
    [SerializeField] private Image LifeSliderFill;
    [SerializeField] private Slider EnergySlider;
    [SerializeField] private Text EnergyLabelText;
    [SerializeField] private Text EnergyValueText;
    [SerializeField] private Image EnergySliderFill;
    [SerializeField] private OnMouseClick EnemyEditButtonClick;

    public Enemy Cur_Enemy;

    public void Initialize(Enemy enemy)
    {
        Cur_Enemy = enemy;
        ClientUtils.ChangeImagePicture(PicImage, Cur_Enemy.LevelPicID);
        LevelNameText.text = Cur_Enemy.LevelNames[LanguageManager.Instance.GetCurrentLanguage()];
        CardCountText.text = Cur_Enemy.BuildInfo.CardCount.ToString();
        LifeValueText.text = Cur_Enemy.BuildInfo.Life.ToString();
        SetLifeBarLengthColor(Cur_Enemy.BuildInfo.Life);
        EnergyValueText.text = Cur_Enemy.BuildInfo.Energy.ToString();
        SetEnergyBarLengthColor(Cur_Enemy.BuildInfo.Energy);

        EnemyEditButtonClick.LeftDoubleClick.RemoveAllListeners();
        EnemyEditButtonClick.LeftDoubleClick.AddListener(delegate
        {
            UIManager.Instance.CloseUIForm<StoryEditorPanel>();
            UIManager.Instance.ShowUIForms<LevelEditorPanel>().SetLevel(enemy.Clone());
        });
    }

    private int[] lifeBarColorThresholds = new[] {10, 50, 100, 500, 1000, 5000, 10000};
    public Color[] LifeBarColors;

    private void SetLifeBarLengthColor(int life)
    {
        for (int i = 0; i < lifeBarColorThresholds.Length; i++)
        {
            if (life <= lifeBarColorThresholds[i])
            {
                LifeSlider.value = (float) life / lifeBarColorThresholds[i];
                LifeSliderFill.color = LifeBarColors[i];
                return;
            }
        }

        LifeSlider.value = (float) life / lifeBarColorThresholds[lifeBarColorThresholds.Length - 1];
        LifeSliderFill.color = LifeBarColors[LifeBarColors.Length - 1];
    }

    private int[] energyBarColorThresholds = new[] {10, 50, 100, 200};
    public Color[] EnergyBarColors;

    private void SetEnergyBarLengthColor(int energy)
    {
        for (int i = 0; i < energyBarColorThresholds.Length; i++)
        {
            if (energy <= energyBarColorThresholds[i])
            {
                EnergySlider.value = (float) energy / energyBarColorThresholds[i];
                EnergySliderFill.color = EnergyBarColors[i];
                return;
            }
        }

        EnergySlider.value = (float) energy / energyBarColorThresholds[energyBarColorThresholds.Length - 1];
        EnergySliderFill.color = EnergyBarColors[EnergyBarColors.Length - 1];
    }

    public void OnLanguageChange()
    {
        LevelNameText.text = Cur_Enemy.LevelNames[LanguageManager.Instance.GetCurrentLanguage()];
    }
}