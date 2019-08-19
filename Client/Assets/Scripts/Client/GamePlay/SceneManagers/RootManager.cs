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
        Utils.NoticeCenterMsg = delegate(string noticeStr) { NoticeManager.Instance.ShowInfoPanelCenter(noticeStr, 0, 2f); };
        AllSideEffects.CurrentAssembly = Assembly.GetAssembly(typeof(Battle));
        AllBuffs.CurrentAssembly = Assembly.GetAssembly(typeof(Battle));
        LoadAllBasicXMLFiles.Load(Application.streamingAssetsPath + "/Config/");
    }

    #region 其他

    [SerializeField] private ImageEffectBlurBox ImageEffectBlurBox;

    public void StartBlurBackGround()
    {
        if (ImageEffectBlurBox)
        {
            ImageEffectBlurBox.enabled = true;
        }
    }

    public void StopBlurBackGround()
    {
        if (ImageEffectBlurBox)
        {
            ImageEffectBlurBox.enabled = false;
        }
    }

    #endregion
}