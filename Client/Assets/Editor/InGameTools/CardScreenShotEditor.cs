using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CardScreenShotEditor : ScriptableObject
{
    [MenuItem("Tools/CardScreenShot")]
    public static void CaptureScreen()
    {
        int i = 1;
        foreach (KeyValuePair<int, CardBase> kv in SelectBuildManager.Instance.allCards)
        {
            SelectBuildManager.Instance.ShowPreviewCardPanel(kv.Value);
            SelectBuildManager.Instance.HideOtherThingsExceptShowCard();

            Camera camera = GameManager.Instance.SelectCardWindowForeCamera;
            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 0);

            camera.targetTexture = rt;
            camera.Render();

            RenderTexture.active = rt;
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);
            Texture2D screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);

            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(rt);

            byte[] bytes = screenShot.EncodeToPNG();
            string filename = Application.dataPath + "/CardScreenShots/" + i + ".png";
            System.IO.File.WriteAllBytes(filename, bytes);
            i++;
        }
    }
}