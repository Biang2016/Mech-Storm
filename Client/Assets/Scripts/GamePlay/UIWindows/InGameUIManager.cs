using UnityEngine;
using UnityEngine.UI;

internal class InGameUIManager : MonoSingleton<InGameUIManager>
{
    private InGameUIManager()
    {
    }

    void Awake()
    {
        BattleCanvas.enabled = false;
//#if DEBUG
        DirectlyWinButton.gameObject.SetActive(true);
        DirectlyWinButton.onClick.AddListener(DirectlyWin);
//#else
        //DirectlyWinButton.gameObject.SetActive(false);
//#endif
    }

    [SerializeField] private Canvas BattleCanvas;
    [SerializeField] private Button EndRoundButton;
    [SerializeField] private Animator EndRoundButtonAnim;
    [SerializeField] private Text EndRoundButtonText;

    [SerializeField] private Button DirectlyWinButton;

    public void ShowInGameUI()
    {
        BattleCanvas.enabled = true;
    }

    public void HideInGameUI()
    {
        BattleCanvas.enabled = false;
    }

    public void SetEndRoundButtonState(bool enable)
    {
        EndRoundButton.enabled = enable;
        EndRoundButtonAnim.SetTrigger(enable ? "OnEnable" : "OnDisable");
        EndRoundButton.interactable = enable;
        EndRoundButton.image.color = enable ? Color.yellow : Color.gray;
        EndRoundButtonText.text = enable ? LanguageManager.Instance.GetText("InGameUIManager_EndTurn") : LanguageManager.Instance.GetText("InGameUIManager_EnemyTurn");
    }

    public void DirectlyWin()
    {
        WinDirectlyRequest request = new WinDirectlyRequest(Client.Instance.Proxy.ClientId);
        Client.Instance.Proxy.SendMessage(request);
    }
}