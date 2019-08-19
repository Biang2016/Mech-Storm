using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardScreenShotEditor : ScriptableObject
{
    [MenuItem("Tools/CardScreenShot")]
    public static void CaptureScreen()
    {
        UIManager.Instance.StartCoroutine(Co_Render());
    }

    static IEnumerator Co_Render()
    {
        CardPreviewPanel cpp = UIManager.Instance.ShowUIForms<CardPreviewPanel>();
        Camera camera = cpp.CardPreviewCamera;
        float ori_FieldOfView = camera.fieldOfView;
        camera.fieldOfView = 36f;
        foreach (KeyValuePair<int, CardBase> kv in UIManager.Instance.GetBaseUIForm<SelectBuildPanel>().AllCards)
        {
            cpp?.ShowPreviewCardPanel(kv.Value, false);
            cpp?.HideOtherThingsExceptShowCard();

            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 0);

            camera.targetTexture = rt;
            yield return new WaitForSeconds(0.1f);
            camera.Render();

            yield return new WaitForSeconds(0.1f);
            RenderTexture.active = rt;
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);
            Texture2D screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGBA32, false);

            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);

            yield return new WaitForSeconds(0.1f);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = Application.dataPath + "/CardScreenShots/" + kv.Key + ".png";
            System.IO.File.WriteAllBytes(filename, bytes);
            yield return new WaitForSeconds(0.1f);
        }

        camera.fieldOfView = ori_FieldOfView;
    }
}