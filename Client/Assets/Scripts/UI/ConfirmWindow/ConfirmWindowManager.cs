using System.Collections.Generic;
using UnityEngine;

public class ConfirmWindowManager : MonoSingleton<ConfirmWindowManager>
{
    public bool IsConfirmWindowShow
    {
        get { return ConfirmWindows.Count != 0; }
    }

    private Stack<ConfirmWindow> ConfirmWindows = new Stack<ConfirmWindow>();

    private void Update()
    {
        if (IsConfirmWindowShow)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                RemoveConfirmWindow();
            }
        }
    }

    public void AddConfirmWindow(ConfirmWindow confirmWindow)
    {
        if (!ConfirmWindows.Contains(confirmWindow)) ConfirmWindows.Push(confirmWindow);
    }

    public void RemoveConfirmWindow()
    {
        if (ConfirmWindows.Peek() != null) ConfirmWindows.Pop().PoolRecycle();
    }
}