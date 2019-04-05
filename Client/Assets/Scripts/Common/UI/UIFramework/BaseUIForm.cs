using System;
using UnityEngine;

public class BaseUIForm : MonoBehaviour
{
    public UIType UIType = new UIType();

    #region  窗体的四种(生命周期)状态

    public virtual void Display()
    {
        gameObject.SetActive(true);
        //设置模态窗体调用(必须是弹出窗体)
        UIMaskMgr.Instance.SetMaskWindow(gameObject, UIType.UIForms_Type, UIType.UIForm_LucencyType);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        UIMaskMgr.Instance.CancelMaskWindow();
    }

    public virtual void Freeze()
    {
        gameObject.SetActive(true);
    }

    public void CloseUIForm()
    {
        string UIFormName = GetType().ToString();
        UIManager.Instance.CloseUIForms(UIFormName);
    }

    #endregion
}