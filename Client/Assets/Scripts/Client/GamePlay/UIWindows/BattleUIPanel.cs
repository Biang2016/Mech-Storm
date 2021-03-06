﻿using UnityEngine;
using UnityEngine.UI;

public class BattleUIPanel : MonoBehaviour
{
    private BattleUIPanel()
    {
    }

    void Awake()
    {
        DirectlyWinButton.gameObject.SetActive(true);
        DirectlyWinButton.onClick.AddListener(DirectlyWin);
        EndRoundButton.onClick.AddListener(RoundManager.Instance.OnEndRoundButtonClick);
        LanguageManager.Instance.RegisterTextKey(EndRoundButtonText, "InGameUIManager_EndTurn");
    }

    [SerializeField] private Button DirectlyWinButton;
    [SerializeField] private Button EndRoundButton;
    [SerializeField] private Animator EndRoundButtonAnim;
    [SerializeField] private Text EndRoundButtonText;

    public void SetEndRoundButtonState(bool enable)
    {
        EndRoundButton.enabled = enable;
        EndRoundButtonAnim.SetTrigger(enable ? "OnEnable" : "OnDisable");
        EndRoundButton.interactable = enable;
        EndRoundButton.image.color = enable ? Color.yellow : Color.gray;
    }

    public void DirectlyWin()
    {
        WinDirectlyRequest request = new WinDirectlyRequest(Client.Instance.Proxy.ClientID);
        Client.Instance.Proxy.SendMessage(request);
    }
}