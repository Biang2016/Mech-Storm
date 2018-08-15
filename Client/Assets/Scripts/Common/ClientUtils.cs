using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ClientUtils
{
    public static Color HTMLColorToColor(string htmlColor)
    {
        Color cl = new Color();
        ColorUtility.TryParseHtmlString(htmlColor, out cl);
        return cl;
    }

    public static void ChangePicture(Renderer rd, int pictureID)
    {
        Texture tx = (Texture) Resources.Load(string.Format("{0:000}", pictureID));
        if (tx == null) Debug.LogError("所选卡片没有图片资源：" + pictureID);
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rd.GetPropertyBlock(mpb);
        mpb.SetTexture("_MainTex", tx);
        mpb.SetTexture("_EmissionMap", tx);
        rd.SetPropertyBlock(mpb);
    }
}