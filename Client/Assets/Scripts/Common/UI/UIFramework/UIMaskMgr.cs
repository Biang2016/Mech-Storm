using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMaskMgr : MonoSingleton<UIMaskMgr>
{
    [SerializeField] private GameObject UIRoot; //UI根节点对象
    [SerializeField] private GameObject MaskPanel; //遮罩面板
    private Image MaskPanel_Image; //遮罩面板Image
    [SerializeField] private GameObject MaskPanel_Blur; //毛玻璃遮罩面板
    private Image MaskPanel_Blur_Image; //毛玻璃遮罩面板Image

    void Awake()
    {
        MaskPanel_Blur_Image = MaskPanel_Blur.GetComponent<Image>();
        MaskPanel_Image = MaskPanel.GetComponent<Image>();
    }

    /// <summary>
    /// 设置遮罩状态
    /// </summary>
    /// <param name="goDisplayUIForms">需要显示的UI窗体</param>
    /// <param name="lucencyType">显示透明度属性</param>
    public void SetMaskWindow(GameObject goDisplayUIForms, UIFormTypes uiFormType, UIFormLucencyTypes lucencyType)
    {
        bool needMask = uiFormType == UIFormTypes.PopUp;
        //顶层窗体下移
        UIRoot.transform.SetAsLastSibling();
        //启用遮罩窗体以及设置透明度
        switch (lucencyType)
        {
            //完全透明，不能穿透
            case UIFormLucencyTypes.Lucency:
                MaskPanel.SetActive(true & needMask);
                MaskPanel_Blur.SetActive(false);
                Color newColor0 = new Color(1, 1, 1, 0);
                MaskPanel_Image.color = newColor0;
                break;
            //毛玻璃效果，不能穿透
            case UIFormLucencyTypes.Blur:
                MaskPanel.SetActive(false);
                MaskPanel_Blur.SetActive(true & needMask);
                BlurShowCoroutine = StartCoroutine(Co_UIMaskPanelBlurTransShow(0.2f));
                break;
            //半透明，不能穿透
            case UIFormLucencyTypes.Translucence:
                MaskPanel.SetActive(true & needMask);
                MaskPanel_Blur.SetActive(false);
                Color newColor2 = new Color(0, 0, 0, 0.2f);
                MaskPanel_Image.color = newColor2;
                break;
            //低透明，不能穿透
            case UIFormLucencyTypes.ImPenetrable:
                MaskPanel.SetActive(true & needMask);
                MaskPanel_Blur.SetActive(false);
                Color newColor3 = new Color(0, 0, 0, 0.8f);
                MaskPanel_Image.color = newColor3;
                break;
            //可以穿透
            case UIFormLucencyTypes.Penetrable:
                if (MaskPanel.activeInHierarchy)
                {
                    MaskPanel.SetActive(false);
                }

                if (MaskPanel_Blur.activeInHierarchy)
                {
                    MaskPanel_Blur.SetActive(false);
                }

                break;
        }

        //遮罩窗体下移
        MaskPanel.transform.SetAsLastSibling();
        MaskPanel_Blur.transform.SetAsLastSibling();
        //显示窗体的下移
        goDisplayUIForms.transform.SetAsLastSibling();
    }

    private Coroutine BlurShowCoroutine;

    IEnumerator Co_UIMaskPanelBlurTransShow(float duration)
    {
        if (BlurShowCoroutine != null) StopCoroutine(BlurShowCoroutine);
        GameManager.Instance.StartBlurBackGround(duration);
        float blurSizeStart = 0f;
        float blurSizeEnd = 128f;
        int frame = Mathf.RoundToInt(duration / 0.05f);
        for (int i = 0; i < frame; i++)
        {
            float blurSize = blurSizeStart + (blurSizeEnd - blurSizeStart) / frame * i;
            MaskPanel_Blur_Image.material.SetFloat("_Size", blurSize);
            yield return new WaitForSeconds(duration / frame);
        }

        MaskPanel_Blur_Image.material.SetFloat("_Size", blurSizeEnd);
    }

    /// <summary>
    /// 取消遮罩状态
    /// </summary>
    public void CancelMaskWindow()
    {
        //顶层窗体上移
        UIRoot.transform.SetAsFirstSibling();
        if (MaskPanel.activeInHierarchy)
        {
            MaskPanel.SetActive(false);
        }

        if (MaskPanel_Blur.activeInHierarchy)
        {
            MaskPanel_Blur.SetActive(false);
        }

        GameManager.Instance.StopBlurBackGround();
    }
}