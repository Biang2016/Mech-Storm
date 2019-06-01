using UnityEngine;
using UnityEngine.UI;

public class ChapterMapNode : PoolObject
{
    [SerializeField] private Image PicImage;

//    public void Initialize(Level level)
//    {
//        ClientUtils.ChangeImagePicture(PicImage, level.LevelPicID);
//    }
    internal int NodeIndex;

    public void Initialize(int nodeIndex, LevelType levelType = LevelType.Enemy, EnemyType enemyType = EnemyType.Soldier)
    {
        NodeIndex = nodeIndex;
        int picID = 0;
        transform.localScale = Vector3.one * 1f;
        switch (levelType)
        {
            case LevelType.Enemy:
            {
                switch (enemyType)
                {
                    case EnemyType.Soldier:
                    {
                        picID = 1000;
                        break;
                    }
                    case EnemyType.Boss:
                    {
                        transform.localScale = Vector3.one * 1.5f;
                        picID = 1003;
                        break;
                    }
                }

                break;
            }
            case LevelType.Rest:
            {
                picID = 1006;
                break;
            }
            case LevelType.Shop:
            {
                picID = 1001;
                break;
            }
            case LevelType.Start:
            {
                picID = 1007;
                break;
            }
            case LevelType.Treasure:
            {
                transform.localScale = Vector3.one * 1.2f;
                picID = 1005;
                break;
            }
        }

        ClientUtils.ChangeImagePicture(PicImage, picID);
    }
}