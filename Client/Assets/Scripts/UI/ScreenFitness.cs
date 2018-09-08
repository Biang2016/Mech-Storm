using UnityEngine;
using UnityEngine.UI;

public class ScreenFitness : MonoBehaviour
{

    float scaler = 16.0f / 8;

    void Awake()
    {
        CanvasScaler rootCanvasScaler = gameObject.GetComponent<CanvasScaler>();
        if (null != rootCanvasScaler)
        {
            if (scaler <= ScreenSetting.OriginalScreenWidth * 1.0f / ScreenSetting.OriginalScreenHeight)
            {
                rootCanvasScaler.matchWidthOrHeight = 1;
            }
            else
            {
                rootCanvasScaler.matchWidthOrHeight = 0;
            }
        }
    }
}