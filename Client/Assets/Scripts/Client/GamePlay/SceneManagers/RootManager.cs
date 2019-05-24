using System.Collections;
using System.Reflection;
using UnityEngine;

public class RootManager : MonoSingleton<RootManager>
{
    protected RootManager()
    {
    }

    public bool ShowClientLogs = false;

    void Awake()
    {
        Utils.DebugLog = ClientLog.Instance.PrintError;
        AllSideEffects.CurrentAssembly = Assembly.GetAssembly(typeof(CardBase));
        AllBuffs.CurrentAssembly = Assembly.GetAssembly(typeof(CardBase));
        LoadAllBasicXMLFiles.Load(Application.streamingAssetsPath + "/Config/");
    }

    #region 其他

    [SerializeField] private ImageEffectBlurBox ImageEffectBlurBox;

    public void StartBlurBackGround()
    {
        if (ImageEffectBlurBox)
        {
            if (StartBlurBackGroundCoroutine != null) StopCoroutine(StartBlurBackGroundCoroutine);
            ImageEffectBlurBox.enabled = true;
            ImageEffectBlurBox.BlurSize = 0.5f;
        }
    }

    public void StopBlurBackGround()
    {
        if (ImageEffectBlurBox)
        {
            if (StartBlurBackGroundCoroutine != null) StopCoroutine(StartBlurBackGroundCoroutine);
            ImageEffectBlurBox.enabled = false;
            ImageEffectBlurBox.BlurSize = 0.5f;
        }
    }

    public void StartBlurBackGround(float duration)
    {
        if (ImageEffectBlurBox)
        {
            StartBlurBackGroundCoroutine = StartCoroutine(Co_StartBlurBackGroundShow(duration));
        }
    }

    private Coroutine StartBlurBackGroundCoroutine;

    IEnumerator Co_StartBlurBackGroundShow(float duration)
    {
        if (ImageEffectBlurBox)
        {
            if (StartBlurBackGroundCoroutine != null) StopCoroutine(StartBlurBackGroundCoroutine);
            ImageEffectBlurBox.enabled = true;
            float blurSizeStart = 0;
            float blurSizeEnd = 0.5f;
            int frame = Mathf.RoundToInt(duration / 0.05f);
            for (int i = 0; i < frame; i++)
            {
                float blurSize = blurSizeStart + (blurSizeEnd - blurSizeStart) / frame * i;
                ImageEffectBlurBox.BlurSize = blurSize;
                yield return new WaitForSeconds(duration / frame);
            }

            ImageEffectBlurBox.BlurSize = blurSizeEnd;
        }

        yield return null;
    }

    #endregion
}