using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardManager : MonoSingletion<GameBoardManager>
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
    public GameObject CardDetailPreview;

    [SerializeField] private Text WinLostText;
    [SerializeField] private Animator PanelAnimator;
    [SerializeField] private Image WinImage;
    [SerializeField] private Image LostImage;

    [SerializeField] private GameObject BattleShip;

    void Awake()
    {
        BattleShip.SetActive(false);
        WinImage.enabled = false;
        LostImage.enabled = false;
        WinLostText.text = "";

        lastBGRenderer = GameBoarderRenderer0;
        idleBGRenderer = GameBoarderRenderer1;
        BGs = Resources.LoadAll<Texture>("BoardBGPictures/");
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

    private Texture[] BGs;
    private int index;
    private Renderer lastBGRenderer;
    private Renderer idleBGRenderer;
    private float changeBGTimeTick = 0;
    [SerializeField] private float ChangeBGTimeInterval = 60f;

    public void ChangeBoardBG()
    {
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
        ChangePictureFadeIn(idleBGRenderer, BGs[index]);
        ChangePictureFadeOut(lastBGRenderer);
        lastBGRenderer = idleBGRenderer;
        idleBGRenderer = temp;
    }


    private void ChangePictureFadeIn(Renderer rd, Texture tx)
    {
        ClientUtils.ChangePicture(rd, tx);
        StartCoroutine(Co_ChangePictureFade(rd, 1f, FadeOption.FadeIn));
    }

    private void ChangePictureFadeOut(Renderer rd)
    {
        StartCoroutine(Co_ChangePictureFade(rd, 0.9f, FadeOption.FadeOut));
    }

    IEnumerator Co_ChangePictureFade(Renderer rd, float duration, FadeOption fadeOption)
    {
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

    public void WinGame()
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_OnGameStopByWin(true), "Co_OnGameStopByWin");
    }


    public void LostGame()
    {
        BattleEffectsManager.Instance.Effect_Main.EffectsShow(Co_OnGameStopByWin(false), "Co_OnGameStopByWin");
    }

    IEnumerator Co_OnGameStopByWin(bool isWin)
    {
        yield return new WaitForSeconds(2);
        AudioManager.Instance.BGMStop();
        if (isWin)
        {
            WinLostText.text = GameManager.Instance.isEnglish ? "You Win!" : "你赢了!";
            WinImage.enabled = true;
            LostImage.enabled = false;
            PanelAnimator.SetTrigger("Show");
            AudioManager.Instance.SoundPlay("sfx/Victory");
        }
        else
        {
            WinLostText.text = GameManager.Instance.isEnglish ? "You Lost!" : "你输了";
            WinImage.enabled = false;
            LostImage.enabled = true;
            AudioManager.Instance.SoundPlay("sfx/Lose");
        }

        PanelAnimator.SetTrigger("Show");
        GameManager.Instance.StartBlurBackGround();
        yield return new WaitForSeconds(4);
        PanelAnimator.SetTrigger("Hide");
        yield return new WaitForSeconds(1);
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();

        RoundManager.Instance.OnGameStop();
        Client.Instance.Proxy.ClientState = ProxyBase.ClientStates.Login;
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