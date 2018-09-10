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
        WinLostText.text = "You Win!";
        WinImage.enabled = true;
        LostImage.enabled = false;
        PanelAnimator.SetTrigger("Show");
        StartCoroutine(ShowWinLostPanel());
    }


    public void LostGame()
    {
        WinLostText.text = "You Lost!";
        WinImage.enabled = false;
        LostImage.enabled = true;
        StartCoroutine(ShowWinLostPanel());
    }

    IEnumerator ShowWinLostPanel()
    {
        PanelAnimator.SetTrigger("Show");
        yield return new WaitForSeconds(5);
        PanelAnimator.SetTrigger("Hide");
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