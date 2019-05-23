using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Biang/ImageEffect/Blurbox")]
public class ImageEffectBlurBox : MonoBehaviour
{
    #region Variables  

    public Shader BlurBoxShader = null;
    private Material BlurBoxMaterial = null;

    [Range(0.0f, 1.0f)] public float BlurSize = 0.5f;

    #endregion

    #region Properties  

    Material material
    {
        get
        {
            if (BlurBoxMaterial == null)
            {
                BlurBoxMaterial = new Material(BlurBoxShader);
                BlurBoxMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            return BlurBoxMaterial;
        }
    }

    #endregion

    // Use this for initialization  
    void Start()
    {
        BlurBoxShader = Shader.Find("Biang/ImageEffect/Unlit/BlurBox");

        // Disable if we don't support image effects  
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        // Disable the image effect if the shader can't  
        // run on the users graphics card  
        if (!BlurBoxShader || !BlurBoxShader.isSupported)
            enabled = false;
        return;
    }

    public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
    {
        float off = BlurSize * iteration + 0.5f;
        Graphics.BlitMultiTap(source, dest, material,
            new Vector2(-off, -off),
            new Vector2(-off, off),
            new Vector2(off, off),
            new Vector2(off, -off)
        );
    }

    private void DownSample4x(RenderTexture source, RenderTexture dest)
    {
        float off = 1.0f;
        // Graphics.Blit(source, dest, material);  
        Graphics.BlitMultiTap(source, dest, material,
            new Vector2(off, off),
            new Vector2(-off, off),
            new Vector2(off, off),
            new Vector2(off, -off)
        );
    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (BlurSize != 0 && BlurBoxShader != null)
        {
            int rtW = sourceTexture.width / 8;
            int rtH = sourceTexture.height / 8;
            RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);

            DownSample4x(sourceTexture, buffer);

            for (int i = 0; i < 2; i++)
            {
                RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
                FourTapCone(buffer, buffer2, i);
                RenderTexture.ReleaseTemporary(buffer);
                buffer = buffer2;
            }

            Graphics.Blit(buffer, destTexture);

            RenderTexture.ReleaseTemporary(buffer);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }

    // Update is called once per frame  
    void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying != true)
        {
            BlurBoxShader = Shader.Find("Biang/ImageEffect/Unlit/BlurBox");
        }
#endif
    }

    public void OnDisable()
    {
        if (BlurBoxMaterial)
            DestroyImmediate(BlurBoxMaterial);
    }
}