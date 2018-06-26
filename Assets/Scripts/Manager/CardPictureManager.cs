using UnityEngine;
using System.Collections;

public class CardPictureManager : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {

    }

    public static void ChangePicture(Renderer ren, int pictureID)
    {
        Texture tx = (Texture)Resources.Load(string.Format("{0:000}", pictureID));
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        ren.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture("_MainTex", tx);
        propertyBlock.SetTexture("_EmissionMap", tx);
        ren.SetPropertyBlock(propertyBlock);
    }
}
