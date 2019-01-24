using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardManager : MonoSingleton<GameBoardManager>
{
    private GameBoardManager()
    {
    }

    [SerializeField] private Image GameBoardBG0;
    [SerializeField] private Image GameBoardBG1;
    public HandManager SelfHandManager;
    public HandManager EnemyHandManager;
    public BattleGroundManager SelfBattleGroundManager;
    public BattleGroundManager EnemyBattleGroundManager;
    public MetalLifeEnergyManager SelfMetalLifeEnergyManager;
    public MetalLifeEnergyManager EnemyMetalLifeEnergyManager;
    public PlayerBuffManager SelfPlayerBuffManager;
    public PlayerBuffManager EnemyPlayerBuffManager;
    public CoolDownCardManager SelfPlayerCoolDownCardManager;
    public CoolDownCardManager EnemyPlayerCoolDownCardManager;
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
        lastBG = GameBoardBG0;
        idleBG = GameBoardBG1;
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
    private Image lastBG;
    private Image idleBG;
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

        Image temp = lastBG;
        ChangePictureFadeIn(idleBG, BGs[index]);
        ChangePictureFadeOut(lastBG);
        lastBG = idleBG;
        idleBG = temp;
    }

    private void ChangePictureFadeIn(Image img, Sprite sp)
    {
        img.sprite = sp;
        StartCoroutine(Co_ChangePictureFade(img, 1f, FadeOption.FadeIn));
    }

    private void ChangePictureFadeOut(Image img)
    {
        StartCoroutine(Co_ChangePictureFade(img, 0.9f, FadeOption.FadeOut));
    }

    IEnumerator Co_ChangePictureFade(Image img, float duration, FadeOption fadeOption)
    {
        isChanging = true;
        for (float tick = duration; tick >= 0; tick -= 0.1f)
        {
            Color color = img.color;
            if (fadeOption == FadeOption.FadeIn)
            {
                img.color=new Color(color.r, color.g, color.b, (duration - tick) / duration);
            }
            else if (fadeOption == FadeOption.FadeOut)
            {
                img.color = new Color(color.r, color.g, color.b, tick / duration);
            }

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