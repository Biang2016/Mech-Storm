using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardManager : MonoSingletion<GameBoardManager>
{
    private GameBoardManager()
    {
    }

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
            WinLostText.text = "You Win!";
            WinImage.enabled = true;
            LostImage.enabled = false;
            PanelAnimator.SetTrigger("Show");
            AudioManager.Instance.SoundPlay("sfx/Victory");
        }
        else
        {
            WinLostText.text = "You Lost!";
            WinImage.enabled = false;
            LostImage.enabled = true;
            AudioManager.Instance.SoundPlay("sfx/Lose");
        }

        PanelAnimator.SetTrigger("Show");
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