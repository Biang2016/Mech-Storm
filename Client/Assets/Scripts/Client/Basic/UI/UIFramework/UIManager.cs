using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    public Camera UICamera;

    //缓存所有UI窗体
    private Dictionary<string, BaseUIForm> AllUIFormDict = new Dictionary<string, BaseUIForm>();

    private Dictionary<string, BaseUIForm> CurrentShowUIFormDict = new Dictionary<string, BaseUIForm>();

    //定义“栈”集合,存储显示当前所有[反向切换]的窗体类型
    private Stack<BaseUIForm> CurrentUIFormsStack = new Stack<BaseUIForm>();

    [SerializeField] private Transform UINormalRoot = null; //全屏幕显示窗体的根节点
    [SerializeField] private Transform UIFixedRoot = null; //固定显示窗体的根节点
    [SerializeField] private Transform UIPopUpRoot = null; //弹出窗体的根节点

    public BaseUIForm GetPeekUIForm()
    {
        if (CurrentUIFormsStack.Count > 0)
        {
            return CurrentUIFormsStack.Peek();
        }
        else
        {
            return null;
        }
    }

    public bool IsPeekUIForm<T>() where T : BaseUIForm
    {
        BaseUIForm peek = GetPeekUIForm();
        return peek != null && peek is T;
    }

    /// <summary>
    /// 显示（打开）UI窗体
    /// 功能：
    /// 1: 根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
    /// 2: 根据不同的UI窗体的“显示模式”，分别作不同的加载处理
    /// </summary>
    public T ShowUIForms<T>() where T : BaseUIForm
    {
        string uiFormNameStr = typeof(T).ToString();
        BaseUIForm uiForm = ShowUIForm(uiFormNameStr);
        return (T) uiForm;
    }

    public BaseUIForm ShowUIForm(string uiFormName)
    {
        BaseUIForm baseUIForms = LoadFormsToAllUIFormsCache(uiFormName); //根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
        if (baseUIForms == null) return null;
        if (baseUIForms.UIType.IsClearStack) ClearStackArray(); //是否清空“栈集合”中得数据

        //根据不同的UI窗体的显示模式，分别作不同的加载处理
        switch (baseUIForms.UIType.UIForms_ShowMode)
        {
            case UIFormShowModes.Normal: //“普通显示”窗口模式
                //把当前窗体加载到“当前窗体”集合中。
                LoadUIToCurrentCache(uiFormName);
                break;
            case UIFormShowModes.Return: //“反向切换”窗口模式
                PushUIFormToStack(uiFormName);
                break;
            case UIFormShowModes.ReturnHideOther: //“反向切换且隐藏其他窗口”窗口模式
                EnterUIFormsAndHideOtherAndReturn(uiFormName);
                break;
            case UIFormShowModes.HideOther: //“隐藏其他”窗口模式
                EnterUIFormsAndHideOther(uiFormName);
                break;
        }

        //Debug.Log("showUI  " + uiFormName);
        return baseUIForms;
    }

    /// <summary>
    /// 关闭（返回上一个）窗体
    /// </summary>
    /// <param name="uiFormName"></param>
    public void CloseUIForm<T>() where T : BaseUIForm
    {
        string uiFormNameStr = typeof(T).ToString();
        CloseUIForm(uiFormNameStr);
    }

    public void CloseUIForm(string uiFormName)
    {
        AllUIFormDict.TryGetValue(uiFormName, out BaseUIForm baseUIForm); //“所有UI窗体”集合中，如果没有记录，则直接返回
        if (baseUIForm == null) return;
        //根据窗体不同的显示类型，分别作不同的关闭处理
        switch (baseUIForm.UIType.UIForms_ShowMode)
        {
            case UIFormShowModes.Normal:
                //普通窗体的关闭
                ExitUIForms(uiFormName);
                break;
            case UIFormShowModes.Return:
                //反向切换窗体的关闭
                PopUIForms();
                break;
            case UIFormShowModes.ReturnHideOther:
                //“反向切换且隐藏其他窗口”窗体的的关闭
                ExitUIFormsAndDisplayOtherAndReturn(uiFormName);
                break;
            case UIFormShowModes.HideOther:
                //隐藏其他窗体关闭
                ExitUIFormsAndDisplayOther(uiFormName);
                break;
        }

        //Debug.Log("closeUI  " + uiFormName);
    }

    public T GetBaseUIForm<T>() where T : BaseUIForm
    {
        string uiFormNameStr = typeof(T).ToString();
        AllUIFormDict.TryGetValue(uiFormNameStr, out BaseUIForm baseUIForm);
        return (T) baseUIForm;
    }

    #region  显示“UI管理器”内部核心数据，测试使用

    /// <summary>
    /// 显示"所有UI窗体"集合的数量
    /// </summary>
    /// <returns></returns>
    public int GetAllUIFormCount()
    {
        if (AllUIFormDict != null)
        {
            return AllUIFormDict.Count;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 显示"当前窗体"集合中数量
    /// </summary>
    /// <returns></returns>
    public int GetCurrentUIFormsCount()
    {
        if (CurrentShowUIFormDict != null)
        {
            return CurrentShowUIFormDict.Count;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 显示“当前栈”集合中窗体数量
    /// </summary>
    /// <returns></returns>
    public int GetCurrentStackUIFormsCount()
    {
        if (CurrentUIFormsStack != null)
        {
            return CurrentUIFormsStack.Count;
        }
        else
        {
            return 0;
        }
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
    /// 功能： 检查“所有UI窗体”集合中，是否已经加载过，否则才加载。
    /// </summary>
    /// <param name="uiFormsName">UI窗体（预设）的名称</param>
    /// <returns></returns>
    private BaseUIForm LoadFormsToAllUIFormsCache(string uiFormsName)
    {
        AllUIFormDict.TryGetValue(uiFormsName, out BaseUIForm baseUIResult);
        if (baseUIResult == null)
        {
            //加载指定名称的“UI窗体”
            baseUIResult = LoadUIForm(uiFormsName);
        }

        return baseUIResult;
    }

    /// <summary>
    /// 加载指定名称的“UI窗体”
    /// 功能：
    ///    1：根据“UI窗体名称”，加载预设克隆体。
    ///    2：根据不同预设克隆体中带的脚本中不同的“位置信息”，加载到“根窗体”下不同的节点。
    ///    3：隐藏刚创建的UI克隆体。
    ///    4：把克隆体，加入到“所有UI窗体”（缓存）集合中。
    /// 
    /// </summary>
    /// <param name="uiFormName">UI窗体名称</param>
    private BaseUIForm LoadUIForm(string uiFormName)
    {
        GameObject UIPanel = Instantiate(PrefabManager.Instance.GetPrefab(uiFormName));
        BaseUIForm baseUiForm = UIPanel.GetComponent<BaseUIForm>();
        if (baseUiForm == null)
        {
            Debug.Log("BaseUIForm==null! ,请先确认窗体预设对象上是否加载了BaseUIForm的子类脚本！ 参数 uiFormName=" + uiFormName);
            return null;
        }

        switch (baseUiForm.UIType.UIForms_Type)
        {
            case UIFormTypes.Normal: //普通窗体节点
                UIPanel.transform.SetParent(UINormalRoot, false);
                break;
            case UIFormTypes.Fixed: //固定窗体节点
                UIPanel.transform.SetParent(UIFixedRoot, false);
                break;
            case UIFormTypes.PopUp: //弹出窗体节点
                UIPanel.transform.SetParent(UIPopUpRoot, false);
                break;
        }

        UIPanel.SetActive(false);
        AllUIFormDict.Add(uiFormName, baseUiForm);
        return baseUiForm;
    }

    /// <summary>
    /// 把当前窗体加载到“当前窗体”集合中
    /// </summary>
    /// <param name="uiFormName">窗体预设的名称</param>
    private void LoadUIToCurrentCache(string uiFormName)
    {
        CurrentShowUIFormDict.TryGetValue(uiFormName, out BaseUIForm baseUiForm);
        AllUIFormDict.TryGetValue(uiFormName, out BaseUIForm baseUIFormFromAllCache);
        if (baseUiForm != null) return;
        if (baseUIFormFromAllCache != null)
        {
            CurrentShowUIFormDict.Add(uiFormName, baseUIFormFromAllCache);
            baseUIFormFromAllCache.Display();
        }
    }

    /// <summary>
    /// UI窗体入栈
    /// </summary>
    /// <param name="uiFormName">窗体的名称</param>
    private void PushUIFormToStack(string uiFormName)
    {
        if (CurrentUIFormsStack.Count > 0)
        {
            BaseUIForm topUIForm = CurrentUIFormsStack.Peek();
            topUIForm.Freeze();
        }

        AllUIFormDict.TryGetValue(uiFormName, out BaseUIForm baseUIForm);
        if (baseUIForm != null)
        {
            baseUIForm.Display();
            CurrentUIFormsStack.Push(baseUIForm);
        }
        else
        {
            Debug.Log("baseUIForm==null,Please Check, 参数 uiFormName=" + uiFormName);
        }
    }

    /// <summary>
    /// 退出指定UI窗体
    /// </summary>
    /// <param name="strUIFormName"></param>
    private void ExitUIForms(string strUIFormName)
    {
        CurrentShowUIFormDict.TryGetValue(strUIFormName, out BaseUIForm baseUIForm);
        if (baseUIForm == null) return;
        baseUIForm.Hide();
        CurrentShowUIFormDict.Remove(strUIFormName);
    }

    //（“反向切换”属性）窗体的出栈逻辑
    private void PopUIForms()
    {
        if (CurrentUIFormsStack.Count >= 2)
        {
            BaseUIForm topUIForms = CurrentUIFormsStack.Pop();
            topUIForms.Hide();
            BaseUIForm nextUIForms = CurrentUIFormsStack.Peek();
            nextUIForms.Display();
        }
        else if (CurrentUIFormsStack.Count == 1)
        {
            BaseUIForm topUIForms = CurrentUIFormsStack.Pop();
            topUIForms.Hide();
        }
    }

    /// <summary>
    /// (“隐藏其他”属性)打开窗体，且隐藏其他窗体
    /// </summary>
    /// <param name="strUIName">打开的指定窗体名称</param>
    private void EnterUIFormsAndHideOther(string strUIName)
    {
        if (string.IsNullOrEmpty(strUIName)) return;
        CurrentShowUIFormDict.TryGetValue(strUIName, out BaseUIForm baseUIForm);
        AllUIFormDict.TryGetValue(strUIName, out BaseUIForm baseUIFormFromALL);

        if (baseUIForm != null) return;

        foreach (BaseUIForm baseUI in CurrentShowUIFormDict.Values)
        {
            if (baseUI.UIType.UIForms_Type == UIFormTypes.Fixed) continue;
            baseUI.Hide();
        }

        foreach (BaseUIForm staUI in CurrentUIFormsStack)
        {
            staUI.Hide();
        }

        if (baseUIFormFromALL != null)
        {
            CurrentShowUIFormDict.Add(strUIName, baseUIFormFromALL);
            baseUIFormFromALL.Display();
        }
    }

    /// <summary>
    /// (“隐藏其他且反向切换”属性)打开窗体，且隐藏其他窗体
    /// </summary>
    /// <param name="uiFormName">打开的指定窗体名称</param>
    private void EnterUIFormsAndHideOtherAndReturn(string uiFormName)
    {
        if (string.IsNullOrEmpty(uiFormName)) return;

        CurrentShowUIFormDict.TryGetValue(uiFormName, out BaseUIForm baseUIForm);
        AllUIFormDict.TryGetValue(uiFormName, out BaseUIForm baseUIFormFromALL);
        if (baseUIForm != null) return;

        foreach (BaseUIForm baseUI in CurrentShowUIFormDict.Values)
        {
            if (baseUI.UIType.UIForms_Type == UIFormTypes.Fixed) continue;
            baseUI.Hide();
        }

        CurrentUIFormsStack.Push(baseUIFormFromALL);
        foreach (BaseUIForm staUI in CurrentUIFormsStack)
        {
            if (staUI != baseUIFormFromALL) staUI.Hide();
        }

        baseUIFormFromALL.Display();
        CurrentShowUIFormDict.Add(uiFormName, baseUIFormFromALL);
    }

    /// <summary>
    /// (“隐藏其他”属性)关闭窗体，且显示其他窗体
    /// </summary>
    /// <param name="uiFormName">打开的指定窗体名称</param>
    private void ExitUIFormsAndDisplayOther(string uiFormName)
    {
        if (string.IsNullOrEmpty(uiFormName)) return;
        CurrentShowUIFormDict.TryGetValue(uiFormName, out BaseUIForm baseUIForm);

        if (baseUIForm == null) return;

        baseUIForm.Hide();
        CurrentShowUIFormDict.Remove(uiFormName);

        bool showAll = true;
        foreach (BaseUIForm baseUI in CurrentShowUIFormDict.Values)
        {
            if (baseUI.UIType.UIForms_ShowMode == UIFormShowModes.HideOther)
            {
                baseUI.Display();
                showAll = false;
                break;
            }
        }

        if (showAll)
        {
            foreach (BaseUIForm baseUI in CurrentShowUIFormDict.Values)
            {
                if (baseUI.UIType.UIForms_Type == UIFormTypes.Fixed) continue;
                baseUI.Display();
            }
        }

        foreach (BaseUIForm staUI in CurrentUIFormsStack)
        {
            if (staUI != baseUIForm) staUI.Display();
        }
    }

    /// <summary>
    /// (“反向切换且隐藏其他”属性)关闭窗体，且显示其他窗体，并显示栈顶窗体
    /// </summary>
    /// <param name="uiFormName">打开的指定窗体名称</param>
    private void ExitUIFormsAndDisplayOtherAndReturn(string uiFormName)
    {
        if (string.IsNullOrEmpty(uiFormName)) return;
        CurrentShowUIFormDict.TryGetValue(uiFormName, out BaseUIForm baseUIForm);

        if (baseUIForm == null) return;

        baseUIForm.Hide();
        CurrentShowUIFormDict.Remove(uiFormName);
        if (CurrentUIFormsStack.Count > 0) CurrentUIFormsStack.Pop();

        foreach (BaseUIForm baseUI in CurrentShowUIFormDict.Values)
        {
            if (baseUI.UIType.UIForms_Type == UIFormTypes.Fixed) continue;
            baseUI.Display();
        }

        foreach (BaseUIForm staUI in CurrentUIFormsStack)
        {
            staUI.Display();
        }
    }

    /// <summary>
    /// 是否清空“栈集合”中得数据
    /// </summary>
    /// <returns></returns>
    private bool ClearStackArray()
    {
        if (CurrentUIFormsStack != null && CurrentUIFormsStack.Count >= 1)
        {
            //清空栈集合
            CurrentUIFormsStack.Clear();
            return true;
        }

        return false;
    }

    #endregion
}