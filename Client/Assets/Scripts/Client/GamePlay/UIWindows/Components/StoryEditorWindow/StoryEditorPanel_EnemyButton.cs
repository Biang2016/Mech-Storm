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
    [SerializeField] private Text CardCountLabelText;
    [SerializeField] private Text CardCountText;
    [SerializeField] private Text LifeLabelText;
    [SerializeField] private Text LifeValueText;
    [SerializeField] private Text EnergyLabelText;
    [SerializeField] private Text EnergyValueText;
    [SerializeField] private OnMouseClick EnemyEditButtonClick;

    public Enemy Cur_Enemy;

    public void Initialize(Enemy enemy)
    {
        Cur_Enemy = enemy;
        ClientUtils.ChangeCardPicture(PicImage, Cur_Enemy.LevelPicID);
        LevelNameText.text = Cur_Enemy.LevelNames[LanguageManager.Instance.GetCurrentLanguage()];
        CardCountText.text = Cur_Enemy.BuildInfo.CardCount.ToString();
        LifeLabelText.text = Cur_Enemy.BuildInfo.Life.ToString();
        EnergyLabelText.text = Cur_Enemy.BuildInfo.Energy.ToString();

        EnemyEditButtonClick.LeftDoubleClick.RemoveAllListeners();
        EnemyEditButtonClick.LeftDoubleClick.AddListener(delegate { UIManager.Instance.ShowUIForms<LevelEditorPanel>().SetLevel(enemy.Clone()); });
    }

    public void OnLanguageChange()
    {
        LevelNameText.text = Cur_Enemy.LevelNames[LanguageManager.Instance.GetCurrentLanguage()];
    }
}