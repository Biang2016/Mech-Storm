using System.Collections.Generic;
using UnityEngine;

public class ConfirmWindowManager : MonoSingletion<ConfirmWindowManager>
{
    public bool IsConfirmWindowShow
    {
        get { return ConfirmWindows.Count != 0; }
    }

    private List<ConfirmWindow> ConfirmWindows = new List<ConfirmWindow>();

    private void Update()
    {
        if (IsConfirmWindowShow)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                foreach (ConfirmWindow cw in ConfirmWindows)
                {
                    cw.PoolRecycle();
                }
            }
        }
    }

    public void AddConfirmWindow(ConfirmWindow confirmWindow)
    {
        if (!ConfirmWindows.Contains(confirmWindow)) ConfirmWindows.Add(confirmWindow);
    }

    public void RemoveConfirmWindow(ConfirmWindow confirmWindow)
    {
        if (ConfirmWindows.Contains(confirmWindow)) ConfirmWindows.Remove(confirmWindow);
    }
}