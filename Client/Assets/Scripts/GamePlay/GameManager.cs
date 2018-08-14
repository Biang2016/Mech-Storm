using UnityEngine;

internal class GameManager : MonoBehaviour
{
    private static GameManager _gm;

    public static GameManager GM
    {
        get
        {
            if (!_gm) _gm = FindObjectOfType<GameManager>();
            return _gm;
        }
    }

    private GameManager()
    {
    }


    private void Awake()
    {
        initialize();
    }

    private void Start()
    {
        _rbe = Camera.main.GetComponent<ImageEffectBlurBox>();
    }

    private void Update()
    {
    }

    #region 游戏全局参数

    public bool UseInspectorParams = false;
    public bool ShowBEMMessages = false;

    public float HandCardSize = 1.0f;
    public float HandCardInterval = 1.0f;
    public float HandCardRotate = 1.0f;
    public float HandCardOffset = 0.6f;
    public float PullOutCardSize = 3.0f;
    public float DetailCardSize = 3.0f;
    public float DetailCardModuleSize = 2.5f;
    public float DetailCardSizeRetinue = 4.0f;

    public float RetinueInterval = 3.5f;
    public float RetinueDetailPreviewDelaySeconds = 0.7f;

    internal Vector3 CardShowPosition;
    public float CardShowScale = 2f;

    public float ShowCardDuration = 0.7f;
    public float ShowCardRotateDuration = 0.1f;
    public float ShowCardFlyTime = 0.2f;

    public int CardDeckCardNum = 10;
    public Vector3 Self_CardDeckCardInterval;
    public Vector3 Enemy_CardDeckCardInterval;

    private void initialize()
    {
        if (UseInspectorParams) return;
        CardBloomColor = ClientUtils.HTMLColorToColor("#FFFFFF");
        RetinueBloomColor = ClientUtils.HTMLColorToColor("#06FF00");
        RetinueOnEnemyHoverBloomColor = ClientUtils.HTMLColorToColor("#FF0000");
        RetinueOnSelfHoverBloomColor = ClientUtils.HTMLColorToColor("#FFF69F");
        Slot1Color = ClientUtils.HTMLColorToColor("#FF0000");
        Slot2Color = ClientUtils.HTMLColorToColor("#FFED00");
        Slot3Color = ClientUtils.HTMLColorToColor("#00FF6B");
        Slot4Color = ClientUtils.HTMLColorToColor("#2D37FF");

        InjuredLifeNumberColor = ClientUtils.HTMLColorToColor("#FF0015");
        DefaultLifeNumberColor = ClientUtils.HTMLColorToColor("#FFFFFF");
        OverFlowTotalLifeColor = ClientUtils.HTMLColorToColor("#00FF28");

        CardShowPosition = new Vector3(10, 3, 0);
    }

    public Color CardBloomColor;
    public Color RetinueBloomColor;
    public Color RetinueOnEnemyHoverBloomColor;
    public Color RetinueOnSelfHoverBloomColor;
    public Color Slot1Color;
    public Color Slot2Color;
    public Color Slot3Color;
    public Color Slot4Color;

    public Color DefaultLifeNumberColor;
    public Color InjuredLifeNumberColor;
    public Color OverFlowTotalLifeColor;

    #endregion

    #region 其他

    public AudioSource MainAudioSource;
    private ImageEffectBlurBox _rbe;

    public void PlayAudioClip(AudioClip ac)
    {
        MainAudioSource.clip = ac;
        MainAudioSource.Play();
    }

    public void StartBlurBackGround()
    {
        _rbe.enabled = true;
    }


    public void StopBlurBackGround()
    {
        _rbe.enabled = false;
    }

    #endregion
}