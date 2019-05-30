using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryEditorPanel_LevelButton : PoolObject
{
    private StoryEditorPanel_LevelButton()
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
                (CardCountLabelText, "StoryEditorWindow_CardCountLabelText"),
                (LifeLabelText, "SettingMenu_LifeLabelText"),
                (EnergyLabelText, "StoryEditorWindow_EnergyLabelText"),
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

    public void Initialize(int pictureID, string levelName, int cardCount, int lifeValue, int energyValue)
    {
        ClientUtils.ChangeCardPicture(PicImage, pictureID);
        LevelNameText.text = levelName;
        CardCountText.text = cardCount.ToString();
        LifeLabelText.text = lifeValue.ToString();
        EnergyLabelText.text = energyValue.ToString();
    }
}