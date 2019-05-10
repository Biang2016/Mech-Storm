using UnityEngine;
using UnityEngine.UI;

internal class BattleUIPanel : BaseUIForm
{
    private BattleUIPanel()
    {
    }

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: false,
            isClickElsewhereClose: false,
            uiForms_Type: UIFormTypes.Fixed,
            uiForms_ShowMode: UIFormShowModes.Normal,
            uiForm_LucencyType: UIFormLucencyTypes.Penetrable);

        DirectlyWinButton.gameObject.SetActive(true);
        DirectlyWinButton.onClick.AddListener(DirectlyWin);
        EndRoundButton.onClick.AddListener(RoundManager.Instance.OnEndRoundButtonClick);
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
        EndRoundButtonText.text = enable ? LanguageManager.Instance.GetText("InGameUIManager_EndTurn") : LanguageManager.Instance.GetText("InGameUIManager_EnemyTurn");
    }

    public void DirectlyWin()
    {
        WinDirectlyRequest request = new WinDirectlyRequest(Client.Instance.Proxy.ClientId);
        Client.Instance.Proxy.SendMessage(request);
    }
}