using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChapterMapNode : PoolObject
{
    [SerializeField] private Image PicImage;
    [SerializeField] private Image SelectedBorder;
    [SerializeField] private Button Button;

    [SerializeField] private Image BeatedImage;
    [SerializeField] private Text BeatedText;

    [SerializeField] private GameObject InfoPanel;
    [SerializeField] private Text LevelNameLabel;
    [SerializeField] private Text LevelNameText;
    [SerializeField] private Text LevelTypeLabel;
    [SerializeField] private Text LevelTypeText;
    [SerializeField] private GameObject EnemyLeveGO;
    [SerializeField] private Text DifficultyLevelLabel;
    [SerializeField] private Text DifficultyLevelText;

    public HashSet<int> AdjacentRoutes = new HashSet<int>();

    void Awake()
    {
        LanguageManager.Instance.RegisterTextKey(LevelNameLabel, "ChapterMap_LevelNameLabel");
        LanguageManager.Instance.RegisterTextKey(LevelTypeLabel, "ChapterMap_LevelTypeLabel");
        LanguageManager.Instance.RegisterTextKey(DifficultyLevelLabel, "ChapterMap_DifficultyLevelLabel");
        LanguageManager.Instance.RegisterTextKey(BeatedText, "ChapterMap_BeatedText");
    }

    public override void PoolRecycle()
    {
        Reset();
        base.PoolRecycle();
    }

    private int NodeIndex;

    private bool isSelected;

    public bool IsSelected
    {
        get
        {
            if (isBeated) return false;
            return isSelected;
        }
        set
        {
            if (isBeated) return;
            isSelected = value;
            SelectedBorder.enabled = value;
        }
    }

    private bool isHovered;

    public bool IsHovered
    {
        get
        {
            //if (isBeated) return false;
            return isHovered;
        }
        set
        {
            //if (isBeated) return;
            isHovered = value;
            InfoPanel.SetActive(isHovered);
            if (isHovered)
            {
                transform.SetAsFirstSibling();
                OnHovered?.Invoke(this);
            }
        }
    }

    private bool isBeated;

    public bool IsBeated
    {
        get { return isBeated; }
        set
        {
            isBeated = value;
            Button.interactable = !isBeated;
            BeatedImage.gameObject.SetActive(isBeated);
        }
    }

    public void Reset()
    {
        LevelNameText.text = "";
        LevelTypeText.text = "";
        DifficultyLevelText.text = "";
        IsSelected = false;
        IsHovered = false;
        IsBeated = false;
        OnHovered = null;
        Button.onClick.RemoveAllListeners();
        Cur_Level = null;
    }

    private UnityAction<ChapterMapNode> OnHovered;

    public void Initialize(int nodeIndex, UnityAction<int> onSelected, UnityAction<ChapterMapNode> onHovered, LevelTypes levelType = LevelTypes.Enemy, EnemyType enemyType = EnemyType.Soldier)
    {
        IsSelected = false;
        IsHovered = false;

        OnHovered = onHovered;
        NodeIndex = nodeIndex;

        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(delegate { onSelected(NodeIndex); });

        int picID = 0;
        transform.localScale = Vector3.one * 1f;
        switch (levelType)
        {
            case LevelTypes.Enemy:
            {
                switch (enemyType)
                {
                    case EnemyType.Soldier:
                    {
                        picID = (int) AllCards.SpecialPicIDs.LockedEmeny;
                        break;
                    }
                    case EnemyType.Boss:
                    {
                        Button.transform.localScale = Vector3.one * 1.5f;
                        picID = (int) AllCards.SpecialPicIDs.LockedBoss;
                        break;
                    }
                }

                break;
            }
            case LevelTypes.Rest:
            {
                picID = (int) AllCards.SpecialPicIDs.Rest;
                break;
            }
            case LevelTypes.Shop:
            {
                picID = (int) AllCards.SpecialPicIDs.Shop;
                break;
            }
            case LevelTypes.Start:
            {
                picID = (int) AllCards.SpecialPicIDs.Empty;
                break;
            }
            case LevelTypes.Treasure:
            {
                Button.transform.localScale = Vector3.one * 1.2f;
                picID = (int) AllCards.SpecialPicIDs.Treasure;
                break;
            }
        }

        ClientUtils.ChangeImagePicture(PicImage, picID);
    }

    public Level Cur_Level;

    public void SetLevel(Level level)
    {
        if (level == null)
        {
            Cur_Level = null;
            LevelNameText.text = "";
            LevelTypeText.text = "";
            LevelBadgeNum = 0;
            DifficultyLevelText.text = "";
            ClientUtils.ChangeImagePicture(PicImage, (int) AllCards.SpecialPicIDs.Empty);
        }
        else
        {
            Cur_Level = level;
            LevelNameText.text = level.LevelNames[LanguageManager.Instance.GetCurrentLanguage()];

            if (level.LevelType != LevelTypes.Enemy)
            {
                LevelTypeText.text = Level.GetLevelTypeDesc(level.LevelType);
            }
            else
            {
                LevelTypeText.text = Level.GetLevelTypeDesc(level.LevelType) + " (" + Enemy.GetEnemyTypeDesc(((Enemy) level).EnemyType) + ")";
            }

            LevelBadgeNum = level.DifficultyLevel;
            DifficultyLevelText.text = "Lv." + level.DifficultyLevel;

            ClientUtils.ChangeImagePicture(PicImage, Cur_Level.LevelPicID);
        }
    }

    public void ClearLevel()
    {
        SetLevel(null);
    }

    #region  LevelBadges

    [SerializeField] private Transform LevelBadgeContainer;
    private List<LevelBadge> LevelBadges = new List<LevelBadge>();

    private int levelBadgeNum;

    public int LevelBadgeNum
    {
        get { return levelBadgeNum; }
        set
        {
            levelBadgeNum = value;

            foreach (LevelBadge lb in LevelBadges)
            {
                lb.PoolRecycle();
            }

            LevelBadges.Clear();

            if (levelBadgeNum <= 3)
            {
                for (int i = 0; i < levelBadgeNum; i++)
                {
                    LevelBadge lb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelBadge_Arrow].AllocateGameObject<LevelBadge>(LevelBadgeContainer);
                    LevelBadges.Add(lb);
                }
            }
            else if (levelBadgeNum <= 6)
            {
                for (int i = 0; i < levelBadgeNum - 3; i++)
                {
                    LevelBadge lb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.LevelBadge_Star].AllocateGameObject<LevelBadge>(LevelBadgeContainer);
                    LevelBadges.Add(lb);
                }
            }
        }
    }

    #endregion
}