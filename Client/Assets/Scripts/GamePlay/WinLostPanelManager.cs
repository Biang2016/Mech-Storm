using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

internal class WinLostPanelManager : MonoSingleton<WinLostPanelManager>
{
    private WinLostPanelManager()
    {
    }

    void Awake()
    {
        WinLostCanvas.enabled = true;
        WinLostText.text = "";
        RewardsTitleText.text = GameManager.Instance.IsEnglish ? "Rewards" : "奖励";
    }


    [SerializeField] private Canvas WinLostCanvas;
    [SerializeField] private Text WinLostText;
    [SerializeField] private Animator PanelAnimator;

    [SerializeField] private Text RewardsTitleText;


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
        yield return new WaitForSeconds(0.5f);
        WinLostCanvas.enabled = true;
        AudioManager.Instance.BGMStop();
        if (isWin)
        {
            WinLostText.text = GameManager.Instance.IsEnglish ? "You Win!" : "你赢了!";
            PanelAnimator.SetTrigger("Show");
            AudioManager.Instance.SoundPlay("sfx/Victory");
        }
        else
        {
            WinLostText.text = GameManager.Instance.IsEnglish ? "You Lost!" : "你输了";
            AudioManager.Instance.SoundPlay("sfx/Lose");
        }

        PanelAnimator.SetTrigger("Show");
        GameManager.Instance.StartBlurBackGround();
        yield return new WaitForSeconds(3);
        PanelAnimator.SetTrigger("Hide");
        yield return new WaitForSeconds(1);
        GameManager.Instance.StopBlurBackGround();
        RoundManager.Instance.OnGameStop();
        Client.Instance.Proxy.ClientState = ProxyBase.ClientStates.Login;
        WinLostCanvas.enabled = false;
        BattleEffectsManager.Instance.Effect_Main.EffectEnd();
    }
}