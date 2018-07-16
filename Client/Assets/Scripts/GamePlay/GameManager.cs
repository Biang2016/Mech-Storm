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
        initializeColors();
    }

    private void Start()
    {
        _rbe = Camera.main.GetComponent<ImageEffectBlurBox>();
    }

    private void Update()
    {
    }


    #region GamePlay

    #endregion

    #region 游戏全局参数

    public bool CanTestEnemyCards = true; //开启敌方卡牌可见、可操作

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

    private void initializeColors()
    {
        CardBloomColor = HTMLColorToColor("#00FF6BFF");
        RetinueBloomColor = HTMLColorToColor("#06FF00FF");
        RetinueOnHoverBloomColor = HTMLColorToColor("#FF0000FF");
        Slot1Color = HTMLColorToColor("#FF0000FF");
        Slot2Color = HTMLColorToColor("#FFED00FF");
        Slot3Color = HTMLColorToColor("#00FF6BFF");
        Slot4Color = HTMLColorToColor("#2D37FFFF");
    }

    public Color CardBloomColor;
    public Color RetinueBloomColor;
    public Color RetinueOnHoverBloomColor;
    public Color Slot1Color;
    public Color Slot2Color;
    public Color Slot3Color;
    public Color Slot4Color;

    public static Color HTMLColorToColor(string htmlColor)
    {
        Color cl = new Color();
        ColorUtility.TryParseHtmlString(htmlColor, out cl);
        return cl;
    }

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