﻿using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ClientUtils
{
    public static string GetPlatformAbbr()
    {
        string res = "";
        switch (Application.platform)
        {
            case RuntimePlatform.OSXPlayer:
            {
                res = "osx";
                break;
            }
            case RuntimePlatform.OSXEditor:
            {
                res = "osx";
                break;
            }
            case RuntimePlatform.WindowsPlayer:
            {
                res = "windows";
                break;
            }
            case RuntimePlatform.WindowsEditor:
            {
                res = "windows";
                break;
            }
        }

        return res;
    }

    public static string ReplaceWrapSpace(string src)
    {
        src = src.Replace(" ", "\u00A0");
        return src;
    }

    public static Color HTMLColorToColor(string htmlColor)
    {
        Color cl = new Color();
        ColorUtility.TryParseHtmlString(htmlColor, out cl);
        return cl;
    }

    public static Color GetColorFromColorDict(AllColors.ColorType ct)
    {
        string color = AllColors.ColorDict[ct];
        return HTMLColorToColor(color);
    }

    public static bool IsCardPictureValid(int pictureID)
    {
        string pid_str = string.Format("{0:000}", pictureID);
        if (string.IsNullOrEmpty(pid_str))
        {
            return false;
        }

        SpriteAtlas atlas = AtlasManager.LoadAtlas("CardPics_" + (pictureID / 100));
        if (atlas)
        {
            Sprite sprite = atlas.GetSprite(pid_str);
            return sprite != null;
        }
        else
        {
            return false;
        }
    }

    public static void ChangePicture(Renderer rd, Texture tx, Rect textureRect)
    {
        Texture2D t2D = (Texture2D) tx;
        Texture2D tarT2D = new Texture2D((int) textureRect.width, (int) textureRect.height);
        Color[] cor = t2D.GetPixels((int) textureRect.x,
            (int) textureRect.y, (int) textureRect.width,
            (int) textureRect.height);
        tarT2D.SetPixels(cor);
        tarT2D.Apply();
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rd.GetPropertyBlock(mpb);
        mpb.SetTexture("_MainTex", tarT2D);
        mpb.SetTexture("_EmissionMap", tarT2D);
        rd.SetPropertyBlock(mpb);
    }

    public static Sprite GetPicByID(int pictureID)
    {
        string pid_str = string.Format("{0:000}", pictureID);
        SpriteAtlas atlas = AtlasManager.LoadAtlas("CardPics_" + (pictureID / 100));
        Sprite sprite = atlas.GetSprite(pid_str);
        if (sprite == null)
        {
            Debug.LogError("所选卡片没有图片资源：" + pid_str);
            return GetPicByID((int) AllCards.SpecialPicIDs.Empty);
        }

        return sprite;
    }

    public static void ChangeImagePicture(Image image, int pictureID)
    {
        image.sprite = GetPicByID(pictureID);
    }

    public static void ChangeImagePicture(SpriteRenderer image, int pictureID)
    {
        image.sprite = GetPicByID(pictureID);
    }

    public static void ChangeColor(RawImage image, Color newColor)
    {
        if (!image) return;
        image.color = newColor;
    }

    public static void ChangeColor(Image image, Color newColor)
    {
        if (!image) return;
        image.color = newColor;
    }

    public static void ChangeColor(Renderer rd, Color newColor, float intensity = 1.0f)
    {
        if (!rd) return;
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rd.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", newColor);
        mpb.SetColor("_EmissionColor", newColor * intensity);
        rd.SetPropertyBlock(mpb);
    }

    public static void ChangeEmissionColor(Renderer rd, Color newColor, float intensity = 1.0f)
    {
        if (!rd) return;
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rd.GetPropertyBlock(mpb);
        mpb.SetColor("_EmissionColor", newColor * intensity);
        rd.SetPropertyBlock(mpb);
    }

    public static void ChangeSlotColor(Renderer rd, SlotTypes slotTypes)
    {
        if (rd == null) return;
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rd.GetPropertyBlock(mpb);
        switch (slotTypes)
        {
            case SlotTypes.Weapon:
                //mpb.SetColor("_Color", GetColorFromColorDict(AllColors.ColorType.Slot1Color));
                //mpb.SetColor("_EmissionColor", GetColorFromColorDict(AllColors.ColorType.Slot1Color));
                break;
            case SlotTypes.Shield:
                //mpb.SetColor("_Color", GetColorFromColorDict(AllColors.ColorType.Slot2Color));
                //mpb.SetColor("_EmissionColor", GetColorFromColorDict(AllColors.ColorType.Slot2Color));
                break;
            case SlotTypes.Pack:
                //mpb.SetColor("_Color", GetColorFromColorDict(AllColors.ColorType.Slot3Color));
                //mpb.SetColor("_EmissionColor", GetColorFromColorDict(AllColors.ColorType.Slot3Color));
                break;
            case SlotTypes.MA:
                //mpb.SetColor("_Color", GetColorFromColorDict(AllColors.ColorType.Slot4Color));
                //mpb.SetColor("_EmissionColor", GetColorFromColorDict(AllColors.ColorType.Slot4Color));
                break;
            default:
                rd.gameObject.SetActive(false);
                break;
        }

        rd.SetPropertyBlock(mpb);
    }

    public static void ChangeSlotColor(RawImage img, SlotTypes slotTypes, float intensity = 1.0f)
    {
        if (img == null) return;
        switch (slotTypes)
        {
            case SlotTypes.Weapon:
                //img.color = GetColorFromColorDict(AllColors.ColorType.Slot1Color) * intensity;
                break;
            case SlotTypes.Shield:
                //img.color = GetColorFromColorDict(AllColors.ColorType.Slot2Color) * intensity;
                break;
            case SlotTypes.Pack:
                //img.color = GetColorFromColorDict(AllColors.ColorType.Slot3Color) * intensity;
                break;
            case SlotTypes.MA:
                //img.color = GetColorFromColorDict(AllColors.ColorType.Slot4Color) * intensity;
                break;
            default:
                img.enabled = false;
                break;
        }
    }

    public static IEnumerator MoveGameObject(Transform obj, Vector3 oldPosition, Quaternion oldRotation,
        Vector3 oldScale, Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale, float duration,
        float rotateDuration)
    {
        obj.position = oldPosition;
        obj.rotation = oldRotation;

        float tick = 0;
        float tickRotate = 0;
        while (true)
        {
            if (tick > duration && tickRotate > rotateDuration) break;

            tick += Time.deltaTime;
            if (tick < duration)
            {
                obj.position = Vector3.Lerp(oldPosition, targetPosition, tick / duration);
                obj.localScale = Vector3.Lerp(oldScale, targetScale, tick / duration);
            }

            tickRotate += Time.deltaTime;
            if (tickRotate < rotateDuration)
            {
                obj.rotation = Quaternion.Slerp(oldRotation, targetRotation, tickRotate / rotateDuration);
            }

            yield return null;
        }

        obj.position = targetPosition;
        obj.rotation = targetRotation;
        obj.localScale = targetScale;
    }

    public struct PositionAndRotation
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }

    public static float GetClipLength(Animator animator, string clip)
    {
        if (null == animator || string.IsNullOrEmpty(clip) || null == animator.runtimeAnimatorController)
            return 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        AnimationClip[] tAnimationClips = ac.animationClips;
        if (null == tAnimationClips || tAnimationClips.Length <= 0) return 0;
        AnimationClip tAnimationClip;
        for (int tCounter = 0, tLen = tAnimationClips.Length; tCounter < tLen; tCounter++)
        {
            tAnimationClip = ac.animationClips[tCounter];
            if (null != tAnimationClip && tAnimationClip.name == clip)
                return tAnimationClip.length;
        }

        return 0F;
    }

    public static Color ChangeColorToWhite(Color color, float whiteRatio)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        float max = Mathf.Max(r, g, b);

        if (max - r < 0.2f && max - g < 0.2f && max - b < 0.2f) //本来就是灰色
        {
            max = max + 0.3f;
            Color res = Color.Lerp(color, new Color(max, max, max, color.a), 1f);
            return res;
        }
        else
        {
            max = max + 0.3f;
            Color res = Color.Lerp(color, new Color(max, max, max, color.a), whiteRatio);
            return res;
        }
    }

    public static Color HSL_2_RGB(float H, float S, float L)
    {
        //H, S and L input range = 0 ÷ 1.0
        //R, G and B output range = 0 ÷ 255
        float R;
        float G;
        float B;
        if (S.Equals(0))
        {
            R = L;
            G = L;
            B = L;
        }
        else
        {
            float var_1 = 0;
            float var_2 = 0;
            if (L < 0.5)
            {
                var_2 = L * (1 + S);
            }
            else
            {
                var_2 = (L + S) - (S * L);
            }

            var_1 = 2 * L - var_2;

            R = Hue_2_RGB(var_1, var_2, H + (1.0f / 3));
            G = Hue_2_RGB(var_1, var_2, H);
            B = Hue_2_RGB(var_1, var_2, H - (1.0f / 3));
        }

        return new Color(R, G, B);
    }

    static float Hue_2_RGB(float v1, float v2, float vH) //Function Hue_2_RGB
    {
        if (vH < 0) vH += 1;
        if (vH > 1) vH -= 1;
        if ((6 * vH) < 1) return (v1 + (v2 - v1) * 6 * vH);
        if ((2 * vH) < 1) return (v2);
        if ((3 * vH) < 2) return (v1 + (v2 - v1) * ((2.0f / 3.0f) - vH) * 6);
        return v1;
    }

    /// <summary>
    /// 查找子节点对象
    /// 内部使用“递归算法”
    /// </summary>
    /// <param name="goParent">父对象</param>
    /// <param name="childName">查找的子对象名称</param>
    /// <returns></returns>
    public static Transform FindTheChildNode(GameObject goParent, string childName)
    {
        Transform searchTrans = null; //查找结果

        searchTrans = goParent.transform.Find(childName);
        if (searchTrans == null)
        {
            foreach (Transform trans in goParent.transform)
            {
                searchTrans = FindTheChildNode(trans.gameObject, childName);
                if (searchTrans != null)
                {
                    return searchTrans;
                }
            }
        }

        return searchTrans;
    }

    /// <summary>
    /// 给子节点添加父对象
    /// </summary>
    /// <param name="parents">父对象的方位</param>
    /// <param name="child">子对象的方法</param>
    public static void AddChildNodeToParentNode(Transform parents, Transform child)
    {
        child.SetParent(parents, false);
        child.localPosition = Vector3.zero;
        child.localScale = Vector3.one;
        child.localEulerAngles = Vector3.zero;
    }

    public static IEnumerator UpdateLayout(RectTransform rect)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        yield return null;
    }
}