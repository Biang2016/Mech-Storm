using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenSetting : MonoBehaviour
{
    bool initialized = false;
    int designWidth = 1366;
    int designHeight = 768;
    public int scaleWidth;
    public int scaleHeight;
    private static int originalScreenWidth = 0;
    private static int originalScreenHeight = 0;
    public static int OriginalScreenWidth
    {
        get
        {
            return originalScreenWidth;
        }
    }
    public static int OriginalScreenHeight
    {
        get
        {
            return originalScreenHeight;
        }
    }

    EventSystem current = null;

    void Start()
    {
        StartCoroutine(ScreenSettingCoroutine());
    }

    IEnumerator ScreenSettingCoroutine()
    {
        while (Application.isShowingSplashScreen)
        {
            yield return null;
        }

        // 设置屏幕旋转
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }

    public static void SaveOriginalScreenResolution()
    {
        if (originalScreenHeight == 0 || originalScreenWidth == 0)
        {
            Resolution resolution = Screen.currentResolution;
            originalScreenWidth = resolution.width;
            originalScreenHeight = resolution.height;
        }
    }

    void Update()
    {
        // 设置当前屏幕点击灵敏度
#if UNITY_ANDROID
        if ((current != EventSystem.current)
            &&(null != EventSystem.current))
        {
            int defaultValue = EventSystem.current.pixelDragThreshold;
            EventSystem.current.pixelDragThreshold = Mathf.Max(defaultValue,(int)(defaultValue * Screen.dpi * 1.0f / 160f));
            current = EventSystem.current;
        }
#endif
    }

    public void setDesignContentScale()
    {
#if UNITY_ANDROID
        GraphicQualityHelper.SetResolution();
#endif
    }

    public void SetScreenTimeout(bool sys_default)
    {
        if (sys_default)
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}
