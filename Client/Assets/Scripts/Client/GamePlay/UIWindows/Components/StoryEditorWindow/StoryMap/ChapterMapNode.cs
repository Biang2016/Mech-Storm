using UnityEngine;
using UnityEngine.UI;

public class ChapterMapNode : PoolObject
{
    [SerializeField] private Image PicImage;

//    public void Initialize(Level level)
//    {
//        ClientUtils.ChangeImagePicture(PicImage, level.LevelPicID);
//    }
    public void Initialize(int id)
    {
        ClientUtils.ChangeImagePicture(PicImage, id);
    }
}