using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class TabControl_TabButton : PoolObject
{
    [SerializeField] private Button Button;
    [SerializeField] private Image SelectedBG;
    [SerializeField] private Text Text;

    public override void PoolRecycle()
    {
        LanguageManager.Instance.UnregisterText(Text);
        Button.onClick.RemoveAllListeners();
        IsSelected = false;
        base.PoolRecycle();
    }

    private bool isSelected;

    public bool IsSelected
    {
        get { return isSelected; }
        set
        {
            isSelected = value;
            SelectedBG.enabled = isSelected;
        }
    }

    public void Initialize(string title_strKey, UnityAction onClick)
    {
        IsSelected = false;
        LanguageManager.Instance.RegisterTextKey(Text, title_strKey);
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(onClick);
    }
}