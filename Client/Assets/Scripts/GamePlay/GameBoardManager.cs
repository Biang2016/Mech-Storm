using System.Collections;
using UnityEngine;

public class GameBoardManager : MonoSingleton<GameBoardManager>
{
    private GameBoardManager()
    {
    }

    [SerializeField] private Renderer GameBoarderRenderer0;
    [SerializeField] private Renderer GameBoarderRenderer1;
    public HandManager SelfHandManager;
    public HandManager EnemyHandManager;
    public BattleGroundManager SelfBattleGroundManager;
    public BattleGroundManager EnemyBattleGroundManager;
    public MetalLifeEnergyManager SelfMetalLifeEnergyManager;
    public MetalLifeEnergyManager EnemyMetalLifeEnergyManager;
    public PlayerBuffManager SelfPlayerBuffManager;
    public PlayerBuffManager EnemyPlayerBuffManager;
    public GameObject CardDetailPreview;

    [SerializeField] private GameObject BattleShip;

    void Awake()
    {
        float screenScale = ((float) Screen.width / Screen.height) / (16.0f / 9.0f);
        BattleShip.transform.localScale = Vector3.one * screenScale;

        BattleShip.SetActive(false);
    }

    void Start()
    {
        lastBGRenderer = GameBoarderRenderer0;
        idleBGRenderer = GameBoarderRenderer1;
        ChangeBoardBG();
    }

    void Update()
    {
        changeBGTimeTick += Time.deltaTime;
        if (changeBGTimeTick > ChangeBGTimeInterval)
        {
            ChangeBoardBG();
        }
    }

    public static Sprite[] BGs;
    private int index;
    private Renderer lastBGRenderer;
    private Renderer idleBGRenderer;
    private float changeBGTimeTick = 0;
    [SerializeField] private float ChangeBGTimeInterval = 60f;

    private bool isChanging = false;

    public void ChangeBoardBG()
    {
        if (isChanging) return;
        changeBGTimeTick = 0;
        if (index < BGs.Length - 1)
        {
            index++;
        }
        else
        {
            index = 0;
        }

        Renderer temp = lastBGRenderer;
        ChangePictureFadeIn(idleBGRenderer, BGs[index].texture, BGs[index].textureRect);
        ChangePictureFadeOut(lastBGRenderer);
        lastBGRenderer = idleBGRenderer;
        idleBGRenderer = temp;
    }

    private void ChangePictureFadeIn(Renderer rd, Texture tx, Rect textureRect)
    {
        ClientUtils.ChangePicture(rd, tx, textureRect);
        StartCoroutine(Co_ChangePictureFade(rd, 1f, FadeOption.FadeIn));
    }

    private void ChangePictureFadeOut(Renderer rd)
    {
        StartCoroutine(Co_ChangePictureFade(rd, 0.9f, FadeOption.FadeOut));
    }

    IEnumerator Co_ChangePictureFade(Renderer rd, float duration, FadeOption fadeOption)
    {
        isChanging = true;
        for (float tick = duration; tick >= 0; tick -= 0.1f)
        {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            rd.GetPropertyBlock(mpb);
            Color color = mpb.GetColor("_Color");
            rd.GetPropertyBlock(mpb);
            if (fadeOption == FadeOption.FadeIn)
            {
                mpb.SetColor("_Color", new Color(color.r, color.g, color.b, (duration - tick) / duration));
            }
            else if (fadeOption == FadeOption.FadeOut)
            {
                mpb.SetColor("_Color", new Color(color.r, color.g, color.b, tick / duration));
            }

            rd.SetPropertyBlock(mpb);
            yield return new WaitForSeconds(0.1f);
        }

        isChanging = false;
    }

    enum FadeOption
    {
        FadeIn,
        FadeOut,
    }

    public void ResetAll()
    {
        HideBattleShip();
        SelfMetalLifeEnergyManager.ResetAll();
        EnemyMetalLifeEnergyManager.ResetAll();
    }

    public void ShowBattleShip()
    {
        BattleShip.SetActive(true);
    }

    public void HideBattleShip()
    {
        BattleShip.SetActive(false);
    }
}

public enum BoardAreaTypes
{
    Others = 0,
    SelfHandArea = 1,
    EnemyHandArea = 2,
    SelfBattleGroundArea = 3,
    EnemyBattleGroundArea = 4,
}