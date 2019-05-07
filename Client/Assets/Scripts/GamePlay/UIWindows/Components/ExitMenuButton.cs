using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExitMenuButton : PoolObject
{
    [SerializeField] private Button Button;
    [SerializeField] private Text Text;

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        LanguageManager.Instance.UnregisterText(Text);
    }

    public void BindTextKey(string textKey, UnityAction buttonClick)
    {
        LanguageManager.Instance.RegisterTextKey(Text, textKey);
        if (buttonClick != null) Button.onClick.AddListener(buttonClick);
    }
}