using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundManager : MonoSingleton<BackGroundManager>
{
    public Camera BattleGroundCamera;

    private BackGroundManager()
    {
    }

    [SerializeField] private Image GameBoardBG0;
    [SerializeField] private Image GameBoardBG1;

    void Awake()
    {
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

        img.DOPause();
        img.transform.DOPause();
        img.transform.localScale = 1.5f * Vector3.one;
        img.DOFade(1, 3f);
        img.transform.DOScale(1, 3f).OnComplete(delegate { img.transform.DOScale(1.2f, 10f).SetLoops(-1, LoopType.Yoyo); });
    }

    private void ChangePictureFadeOut(Image img)
    {
        img.transform.DOScale(3, 3f);
        img.DOFade(0, 3f).OnComplete(delegate
        {
            img.DOPause();
            img.transform.DOPause();
        });
    }
}