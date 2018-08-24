using UnityEngine;
using UnityEngine.EventSystems;

internal class GameManager : MonoSingletion<GameManager>
{
    private GameManager()
    {
    }

    private void Awake()
    {
        InitializeClientGameSettings();
        AllSideEffects.AddAllSideEffects(Application.streamingAssetsPath + "/Config/SideEffects.xml");
        AllCards.AddAllCards(Application.streamingAssetsPath + "/Config/Cards.xml");
    }

    public EventSystem EventSystem;

    public Camera BattleGroundCamera;
    public Camera ForeGroundCamera;
    public Camera SelectCardWindowBackCamera;
    public Camera SelectCardWindowForeCamera;

    #region 游戏全局参数

    internal Vector3 UseCardShowPosition = new Vector3(10, 3, 0);
    internal Vector3 UseCardShowOverlayPosition = new Vector3(10, 3, 0.2f);

    public bool UseInspectorParams = false;
    public bool ShowBEMMessages = false;

    public float HandCardSize = 1.5f;
    public float HandCardInterval = 1.0f;
    public float HandCardRotate = 1.0f;
    public float HandCardOffset = 0.4f;

    public float PullOutCardSize = 3.0f;
    public float PullOutCardDistanceThreshold = 3.0f;

    public float DetailSingleCardSize = 3.0f;
    public float DetailEquipmentCardSize = 2.5f;
    public float DetailRetinueCardSize = 4.0f;

    public float RetinueInterval = 3.5f;
    public float RetinueDetailPreviewDelaySeconds = 0.7f;

    public float CardShowScale = 3f;

    public float ShowCardDuration = 0.7f;
    public float ShowCardRotateDuration = 0.1f;
    public float ShowCardFlyDuration = 0.2f;

    public int CardDeckCardNum = 10;

    public Vector3 Self_CardDeckCardInterval = new Vector3(0.05f, 0.01f, 0.1f);
    public Vector3 Enemy_CardDeckCardInterval = new Vector3(-0.05f, 0.01f, 0.1f);

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

    public Color BuildButtonEditColor;
    public Color BuildButtonDefaultColor;

    private void InitializeClientGameSettings()
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

        DefaultLifeNumberColor = ClientUtils.HTMLColorToColor("#FFFFFF");
        InjuredLifeNumberColor = ClientUtils.HTMLColorToColor("#FF0015");
        OverFlowTotalLifeColor = ClientUtils.HTMLColorToColor("#00FF28");

        BuildButtonEditColor = ClientUtils.HTMLColorToColor("#FF4B00");
        BuildButtonDefaultColor = ClientUtils.HTMLColorToColor("#858585");
    }

    #endregion

    #region 其他

    public AudioSource MainAudioSource;

    public void PlayAudioClip(AudioClip ac)
    {
        MainAudioSource.clip = ac;
        MainAudioSource.Play();
    }

    [SerializeField] private ImageEffectBlurBox ImageEffectBlurBox;

    public void StartBlurBackGround()
    {
        ImageEffectBlurBox.enabled = true;
    }


    public void StopBlurBackGround()
    {
        ImageEffectBlurBox.enabled = false;
    }

    #endregion
}