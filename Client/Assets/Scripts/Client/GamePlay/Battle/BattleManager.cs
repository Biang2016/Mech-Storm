using UnityEngine;

public class BattleManager : MonoSingleton<BattleManager>
{
    private BattleManager()
    {
    }

    public bool ShowBEMMessages = false;

    internal BattlePlayer SelfBattlePlayer;
    internal BattlePlayer EnemyBattlePlayer;

    public BattleUIPanel BattleUIPanel;

    public ShowCardDetailInBattleManager ShowCardDetailInBattleManager;

    void Awake()
    {
        BattleUIPanel.gameObject.SetActive(false);
    }

    void Update()
    {
    }

    private void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Client.Instance.IsPlaying())
            {
                if (!UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().IsShow)
                {
                    if (UIManager.Instance.GetPeekUIForm() == null)
                    {
                        if (UIManager.Instance.GetBaseUIForm<BattleResultPanel>() == null || !UIManager.Instance.GetBaseUIForm<BattleResultPanel>().IsShow)
                        {
                            UIManager.Instance.ShowUIForms<ExitMenuPanel>();
                        }
                    }
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (Client.Instance.IsPlaying())
            {
                if (!UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().IsShow)
                {
                    if (UIManager.Instance.GetPeekUIForm() == null)
                    {
                        if (UIManager.Instance.GetBaseUIForm<BattleResultPanel>() == null || !UIManager.Instance.GetBaseUIForm<BattleResultPanel>().IsShow)
                        {
                            UIManager.Instance.ShowUIForms<SelectBuildPanel>();
                        }
                    }
                }
            }
        }
    }

    public void ResetAll()
    {
        SelfBattlePlayer?.PoolRecycle();
        EnemyBattlePlayer?.PoolRecycle();
        ShowCardDetailInBattleManager.HideCardDetail();
        BattleUIPanel.gameObject.SetActive(false);
    }

    public void ShowBattleShips()
    {
        SelfBattlePlayer?.ShowBattleShip();
        EnemyBattlePlayer?.ShowBattleShip();
        BattleUIPanel.gameObject.SetActive(true);
    }

    public void HideBattleShips()
    {
        SelfBattlePlayer?.HideBattleShip();
        EnemyBattlePlayer?.HideBattleShip();
    }

    public void SetLanguage(string languageShort)
    {
        SelfBattlePlayer?.HandManager.SetLanguage(languageShort);
        SelfBattlePlayer?.BattleGroundManager.SetLanguage(languageShort);
        EnemyBattlePlayer?.HandManager.SetLanguage(languageShort);
        EnemyBattlePlayer?.BattleGroundManager.SetLanguage(languageShort);
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