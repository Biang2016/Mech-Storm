using System.Collections;
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
    public Camera CardSelectCamera;

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

    private void InitializeClientGameSettings()
    {

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