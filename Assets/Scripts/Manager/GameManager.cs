using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager _gm;
    public static GameManager GM {
        get {
            if (!_gm) _gm = FindObjectOfType<GameManager>();
            return _gm;
        }
    }
    
    #region 其他
    public AudioSource MainAudioSource;
    private ImageEffectBlurBox _rbe;

    public void PlayAudioClip(AudioClip ac)
    {
        MainAudioSource.clip = ac;
        MainAudioSource.Play();
    }

    #endregion

    #region GamePlay

    internal AllCards AllCard;

    internal Player SelfPlayer;
    internal Player EnemyPlayer;

    #endregion

    private void Awake()
    {
        SelfPlayer = new Player(Players.Self);
        EnemyPlayer = new Player(Players.Enemy);
        AllCard = new AllCards();

        _rbe = Camera.main.GetComponent<ImageEffectBlurBox>();
    }

    private void Start()
    {
    }

    private void Update()
    {
    }



    public void StartBlurBackGround()
    {
        _rbe.enabled = true;
    }


    public void StopBlurBackGround()
    {
        _rbe.enabled = false;
    }

    #region 游戏全局参数

    public bool CanTestEnemyCards = false; //开启敌方卡牌可见、可操作

    public int BeginCost = 0;
    public int MaxCost = 20;

    public float DetailCardSize = 3.0f;
    public float DetailCardModuleSize = 2.5f;
    public float DetailCardSizeRetinue = 4.0f;

    public int MaxRetinueNumber = 7;

    public int DrawCardPerRound = 2;

    #endregion
}