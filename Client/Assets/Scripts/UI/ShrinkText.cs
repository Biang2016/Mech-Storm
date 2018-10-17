using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShrinkText : Text
{
    /// <summary>
    /// 当前可见的文字行数
    /// </summary>
    public int VisibleLines { get; private set; }

    private void _UseFitSettings()
    {
        TextGenerationSettings settings = GetGenerationSettings(rectTransform.rect.size);
        settings.resizeTextForBestFit = false;

        if (!resizeTextForBestFit)
        {
            cachedTextGenerator.Populate(text, settings);
            return;
        }

        int minSize = resizeTextMinSize;
        int txtLen = text.Length;
        for (int i = resizeTextMaxSize; i >= minSize; --i)
        {
            settings.fontSize = i;
            cachedTextGenerator.Populate(text, settings);
            if (cachedTextGenerator.characterCountVisible == txtLen) break;
        }
    }

    private readonly UIVertex[] _tmpVerts = new UIVertex[4];
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        if (null == font) return;

        m_DisableFontTextureRebuiltCallback = true;
        _UseFitSettings();
        Rect rect = rectTransform.rect;

        Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
        Vector2 zero = Vector2.zero;
        zero.x = Mathf.Lerp(rect.xMin, rect.xMax, textAnchorPivot.x);
        zero.y = Mathf.Lerp(rect.yMin, rect.yMax, textAnchorPivot.y);
        Vector2 vector2 = PixelAdjustPoint(zero) - zero;
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float num1 = 1f / pixelsPerUnit;
        int num2 = verts.Count - 4;
        toFill.Clear();
        if (vector2 != Vector2.zero)
        {
            for (int index1 = 0; index1 < num2; ++index1)
            {
                int index2 = index1 & 3;
                _tmpVerts[index2] = verts[index1];
                _tmpVerts[index2].position *= num1;
                _tmpVerts[index2].position.x += vector2.x;
                _tmpVerts[index2].position.y += vector2.y;
                if (index2 == 3)
                    toFill.AddUIVertexQuad(this._tmpVerts);
            }
        }
        else
        {
            for (int index1 = 0; index1 < num2; ++index1)
            {
                int index2 = index1 & 3;
                _tmpVerts[index2] = verts[index1];
                _tmpVerts[index2].position *= num1;
                if (index2 == 3)
                    toFill.AddUIVertexQuad(_tmpVerts);
            }
        }
        m_DisableFontTextureRebuiltCallback = false;
        VisibleLines = cachedTextGenerator.lineCount;
    }
}
