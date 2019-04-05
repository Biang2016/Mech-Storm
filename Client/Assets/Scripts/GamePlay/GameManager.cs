using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private GameManager()
    {
    }

    private void Awake()
    {
        InitializeClientGameSettings();
        Utils.DebugLog = ClientLog.Instance.PrintError;

        AllColors.AddAllColors(Application.streamingAssetsPath + "/Config/Colors.xml");
        AllSideEffects.AddAllSideEffects(Application.streamingAssetsPath + "/Config/SideEffects.xml");
        AllBuffs.AddAllBuffs(Application.streamingAssetsPath + "/Config/Buffs.xml");
        AllCards.AddAllCards(Application.streamingAssetsPath + "/Config/Cards.xml");
    }

    public Camera BattleGroundCamera;
    public Camera ForeGroundCamera;
    public Camera SelectCardWindowBackCamera;
    public Camera SelectCardWindowForeCamera;

    #region 游戏全局参数

    internal Vector3 UseCardShowPosition = new Vector3(10, 3, 0);
    internal Vector3 UseCardShowPosition_Overlay = new Vector3(10, 3, 0.2f);

    public bool ShowBEMMessages = false;
    public bool ShowClientLogs = false;

    public float HandCardSize = 1.5f;
    public float HandCardInterval = 1.0f;
    public float HandCardRotate = 1.0f;
    public float HandCardOffset = 0.4f;

    public float PullOutCardSize = 3.0f;
    public float PullOutCardDistanceThreshold = 0f;

    public float DetailSingleCardSize = 3.0f;
    public float DetailEquipmentCardSize = 2.5f;
    public float DetailRetinueCardSize = 4.0f;

    public float RetinueDefaultSize = 1.75f;
    public float RetinueInterval = 3.5f;
    public float RetinueDetailPreviewDelaySeconds = 0.7f;

    public float CardShowScale = 3f;

    public float ShowCardDuration = 1.2f;
    public float ShowCardFlyDuration = 0.4f;

    public int CardDeckCardNum = 10;

    public float CardDeckCardSize = 1.4f;
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

    public Color SelfMetalBarColor;
    public Color EnemyMetalBarColor;

    public Color SelfCardDeckCardColor;
    public Color EnemyCardDeckCardColor;

    public Color LifeIconColor;
    public Color MetalIconColor;
    public Color EnergyIconColor;

    private void InitializeClientGameSettings()
    {
        CardBloomColor = ClientUtils.HTMLColorToColor("#F1FF74");
        RetinueBloomColor = ClientUtils.HTMLColorToColor("#06FF00");
        RetinueOnEnemyHoverBloomColor = ClientUtils.HTMLColorToColor("#FF0000");
        RetinueOnSelfHoverBloomColor = ClientUtils.HTMLColorToColor("#FFF69F");

        Slot1Color = ClientUtils.HTMLColorToColor("#FF0000");
        Slot2Color = ClientUtils.HTMLColorToColor("#FFED00");
        Slot3Color = ClientUtils.HTMLColorToColor("#0049BC");
        Slot4Color = ClientUtils.HTMLColorToColor("#7F8AFF");

        DefaultLifeNumberColor = ClientUtils.HTMLColorToColor("#FFFFFF");
        InjuredLifeNumberColor = ClientUtils.HTMLColorToColor("#E2FF00");
        OverFlowTotalLifeColor = ClientUtils.HTMLColorToColor("#00FF28");

        BuildButtonEditColor = ClientUtils.HTMLColorToColor("#FF4B00");
        BuildButtonDefaultColor = ClientUtils.HTMLColorToColor("#858585");

        SelfMetalBarColor = ClientUtils.HTMLColorToColor("#9E00FF");
        EnemyMetalBarColor = ClientUtils.HTMLColorToColor("#9E00FF");

        SelfCardDeckCardColor = ClientUtils.HTMLColorToColor("#69B1FF");
        EnemyCardDeckCardColor = ClientUtils.HTMLColorToColor("#FF6C6F");

        LifeIconColor = ClientUtils.HTMLColorToColor("#A22A2E");
        MetalIconColor = ClientUtils.HTMLColorToColor("#8335FF");
        EnergyIconColor = ClientUtils.HTMLColorToColor("#007AFF");
    }

    #endregion

    #region 其他

    [SerializeField] private ImageEffectBlurBox ImageEffectBlurBox;

    public void StartBlurBackGround()
    {
        if (StartBlurBackGroundCoroutine != null) StopCoroutine(StartBlurBackGroundCoroutine);
        ImageEffectBlurBox.enabled = true;
        ImageEffectBlurBox.BlurSize = 0.5f;
    }

    public void StopBlurBackGround()
    {
        if (StartBlurBackGroundCoroutine != null) StopCoroutine(StartBlurBackGroundCoroutine);
        ImageEffectBlurBox.enabled = false;
        ImageEffectBlurBox.BlurSize = 0.5f;
    }

    public void StartBlurBackGround(float duration)
    {
        StartBlurBackGroundCoroutine = StartCoroutine(Co_StartBlurBackGroundShow(duration));
    }

    private Coroutine StartBlurBackGroundCoroutine;

    IEnumerator Co_StartBlurBackGroundShow(float duration)
    {
        if (StartBlurBackGroundCoroutine != null) StopCoroutine(StartBlurBackGroundCoroutine);
        ImageEffectBlurBox.enabled = true;
        float blurSizeStart = 0;
        float blurSizeEnd = 0.5f;
        int frame = Mathf.RoundToInt(duration / 0.05f);
        for (int i = 0; i < frame; i++)
        {
            float blurSize = blurSizeStart + (blurSizeEnd - blurSizeStart) / frame * i;
            ImageEffectBlurBox.BlurSize = blurSize;
            yield return new WaitForSeconds(duration / frame);
        }

        ImageEffectBlurBox.BlurSize = blurSizeEnd;
    }

    #endregion
}