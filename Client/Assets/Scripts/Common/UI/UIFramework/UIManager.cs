using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    public Camera UICamera;

    //UI窗体预设路径(参数1：窗体预设名称，2：表示窗体预设路径)
    private Dictionary<string, string> FormsPathDict = new Dictionary<string, string>();

    //缓存所有UI窗体
    private Dictionary<string, BaseUIForm> AllUIFormDict = new Dictionary<string, BaseUIForm>();

    //当前显示的UI窗体
    private Dictionary<string, BaseUIForm> CurrentShowUIFormDict = new Dictionary<string, BaseUIForm>();

    //定义“栈”集合,存储显示当前所有[反向切换]的窗体类型
    private Stack<BaseUIForm> CurrentUIFormsStack = new Stack<BaseUIForm>();

    [SerializeField] private Transform UIRoot = null; //UI根节点
    [SerializeField] private Transform UINormalRoot = null; //全屏幕显示的节点
    [SerializeField] private Transform UIFixedRoot = null; //固定显示的节点
    [SerializeField] private Transform UIPopUpRoot = null; //弹出节点

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

    public void ClosePeekUIForm()
    {
        BaseUIForm ui = GetPeekUIForm();
        if (ui != null && ui.UIType.IsClickElsewhereClose)
        {
            ui.CloseUIForm();
        }
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
        BaseUIForm baseUIForms = LoadFormsToAllUIFormsCache(uiFormNameStr); //根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
        if (baseUIForms == null) return null;
        if (baseUIForms.UIType.IsClearStack) ClearStackArray(); //是否清空“栈集合”中得数据

        //根据不同的UI窗体的显示模式，分别作不同的加载处理
        switch (baseUIForms.UIType.UIForms_ShowMode)
        {
            case UIFormShowModes.Normal: //“普通显示”窗口模式
                //把当前窗体加载到“当前窗体”集合中。
                LoadUIToCurrentCache(uiFormNameStr);
                break;
            case UIFormShowModes.Return: //需要“反向切换”窗口模式
                PushUIFormToStack(uiFormNameStr);
                break;
            case UIFormShowModes.HideOther: //“隐藏其他”窗口模式
                EnterUIFormsAndHideOther(uiFormNameStr);
                break;
        }

        return (T) baseUIForms;
    }

    /// <summary>
    /// 关闭（返回上一个）窗体
    /// </summary>
    /// <param name="uiFormName"></param>
    public void CloseUIForms<T>() where T : BaseUIForm
    {
        string uiFormNameStr = typeof(T).ToString();
        CloseUIForms(uiFormNameStr);
    }

    public void CloseUIForms(string uiFormNameStr)
    {
        AllUIFormDict.TryGetValue(uiFormNameStr, out BaseUIForm baseUIForm); //“所有UI窗体”集合中，如果没有记录，则直接返回
        if (baseUIForm == null) return;
        //根据窗体不同的显示类型，分别作不同的关闭处理
        switch (baseUIForm.UIType.UIForms_ShowMode)
        {
            case UIFormShowModes.Normal:
                //普通窗体的关闭
                ExitUIForms(uiFormNameStr);
                break;
            case UIFormShowModes.Return:
                //反向切换窗体的关闭
                PopUIForms();
                break;
            case UIFormShowModes.HideOther:
                //隐藏其他窗体关闭
                ExitUIFormsAndDisplayOther(uiFormNameStr);
                break;
        }
    }

    public T GetBaseUIForm<T>() where T : BaseUIForm
    {
        string uiFormNameStr = typeof(T).ToString();
        AllUIFormDict.TryGetValue(uiFormNameStr, out BaseUIForm baseUIForm); //“所有UI窗体”集合中，如果没有记录，则直接返回
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
        BaseUIForm baseUiForm; //UI窗体基类
        BaseUIForm baseUIFormFromAllCache; //从“所有窗体集合”中得到的窗体

        //如果“正在显示”的集合中，存在整个UI窗体，则直接返回
        CurrentShowUIFormDict.TryGetValue(uiFormName, out baseUiForm);
        if (baseUiForm != null) return;
        //把当前窗体，加载到“正在显示”集合中
        AllUIFormDict.TryGetValue(uiFormName, out baseUIFormFromAllCache);
        if (baseUIFormFromAllCache != null)
        {
            CurrentShowUIFormDict.Add(uiFormName, baseUIFormFromAllCache);
            baseUIFormFromAllCache.Display(); //显示当前窗体
        }
    }

    /// <summary>
    /// UI窗体入栈
    /// </summary>
    /// <param name="uiFormName">窗体的名称</param>
    private void PushUIFormToStack(string uiFormName)
    {
        BaseUIForm baseUIForm; //UI窗体

        //判断“栈”集合中，是否有其他的窗体，有则“冻结”处理。
        if (CurrentUIFormsStack.Count > 0)
        {
            BaseUIForm topUIForm = CurrentUIFormsStack.Peek();
            //栈顶元素作冻结处理
            topUIForm.Freeze();
        }

        //判断“UI所有窗体”集合是否有指定的UI窗体，有则处理。
        AllUIFormDict.TryGetValue(uiFormName, out baseUIForm);
        if (baseUIForm != null)
        {
            //当前窗口显示状态
            baseUIForm.Display();
            //把指定的UI窗体，入栈操作。
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
        BaseUIForm baseUIForm; //窗体基类

        //"正在显示集合"中如果没有记录，则直接返回。
        CurrentShowUIFormDict.TryGetValue(strUIFormName, out baseUIForm);
        if (baseUIForm == null) return;
        //指定窗体，标记为“隐藏状态”，且从"正在显示集合"中移除。
        baseUIForm.Hide();
        CurrentShowUIFormDict.Remove(strUIFormName);
    }

    //（“反向切换”属性）窗体的出栈逻辑
    private void PopUIForms()
    {
        if (CurrentUIFormsStack.Count >= 2)
        {
            //出栈处理
            BaseUIForm topUIForms = CurrentUIFormsStack.Pop();
            //做隐藏处理
            topUIForms.Hide();
            //出栈后，下一个窗体做“重新显示”处理。
            BaseUIForm nextUIForms = CurrentUIFormsStack.Peek();
            nextUIForms.Display();
        }
        else if (CurrentUIFormsStack.Count == 1)
        {
            //出栈处理
            BaseUIForm topUIForms = CurrentUIFormsStack.Pop();
            //做隐藏处理
            topUIForms.Hide();
        }
    }

    /// <summary>
    /// (“隐藏其他”属性)打开窗体，且隐藏其他窗体
    /// </summary>
    /// <param name="strUIName">打开的指定窗体名称</param>
    private void EnterUIFormsAndHideOther(string strUIName)
    {
        BaseUIForm baseUIForm; //UI窗体基类
        BaseUIForm baseUIFormFromALL; //从集合中得到的UI窗体基类

        //参数检查
        if (string.IsNullOrEmpty(strUIName)) return;

        CurrentShowUIFormDict.TryGetValue(strUIName, out baseUIForm);
        if (baseUIForm != null) return;

        //把“正在显示集合”与“栈集合”中所有窗体都隐藏。
        foreach (BaseUIForm baseUI in CurrentShowUIFormDict.Values)
        {
            baseUI.Hide();
        }

        foreach (BaseUIForm staUI in CurrentUIFormsStack)
        {
            staUI.Hide();
        }

        //把当前窗体加入到“正在显示窗体”集合中，且做显示处理。
        AllUIFormDict.TryGetValue(strUIName, out baseUIFormFromALL);
        if (baseUIFormFromALL != null)
        {
            CurrentShowUIFormDict.Add(strUIName, baseUIFormFromALL);
            //窗体显示
            baseUIFormFromALL.Display();
        }
    }

    /// <summary>
    /// (“隐藏其他”属性)关闭窗体，且显示其他窗体
    /// </summary>
    /// <param name="strUIName">打开的指定窗体名称</param>
    private void ExitUIFormsAndDisplayOther(string strUIName)
    {
        BaseUIForm baseUIForm; //UI窗体基类

        //参数检查
        if (string.IsNullOrEmpty(strUIName)) return;

        CurrentShowUIFormDict.TryGetValue(strUIName, out baseUIForm);
        if (baseUIForm == null) return;

        //当前窗体隐藏状态，且“正在显示”集合中，移除本窗体
        baseUIForm.Hide();
        CurrentShowUIFormDict.Remove(strUIName);

        //把“正在显示集合”与“栈集合”中所有窗体都定义重新显示状态。
        foreach (BaseUIForm baseUI in CurrentShowUIFormDict.Values)
        {
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