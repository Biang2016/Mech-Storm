using System.Collections;
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
                img.color = new Color(color.r, color.g, color.b, (duration - tick) / duration);
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
}