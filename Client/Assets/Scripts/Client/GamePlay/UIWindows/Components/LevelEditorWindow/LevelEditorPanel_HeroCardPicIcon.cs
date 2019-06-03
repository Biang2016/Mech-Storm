using UnityEngine;
using UnityEngine.UI;

public class LevelEditorPanel_HeroCardPicIcon : PoolObject
{
    private LevelEditorPanel_HeroCardPicIcon()
    {
    }

    public override void PoolRecycle()
    {
        base.PoolRecycle();
    }

    [SerializeField] private Image PicImage;
    [SerializeField] private Text CardIDText;

    public void Initialize(int pictureID, int cardID)
    {
        ClientUtils.ChangeImagePicture(PicImage, pictureID);
        CardIDText.text = cardID.ToString();
    }
}