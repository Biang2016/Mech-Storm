using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmWindow : PoolObject
{
    [SerializeField] private Text DescText;
    [SerializeField] private Text LeftButtonText;
    [SerializeField] private Text RightButtonText;
    [SerializeField] private Button LeftButton;
    [SerializeField] private Button RightButton;

    void Awake()
    {
        DescText.text = "";
        LeftButtonText.text = "";
        RightButtonText.text = "";
    }

    public override void PoolRecycle()
    {
        base.PoolRecycle();
        gameObject.SetActive(false);
        DescText.text = "";
        LeftButtonText.text = "";
        RightButtonText.text = "";
    }

    public static bool IsConfirmWindowShow = false;

    public void Initialize(string descText, string leftButtonText, string rightButtonText, UnityAction leftButtonClick, UnityAction rightButtonClick)
    {
        gameObject.SetActive(true);
        ConfirmWindowManager.Instance.AddConfirmWindow(this);
        DescText.text = descText;
        LeftButtonText.text = leftButtonText;
        RightButtonText.text = rightButtonText;
        LeftButton.onClick.RemoveAllListeners();
        RightButton.onClick.RemoveAllListeners();
        LeftButton.onClick.AddListener(leftButtonClick);
        RightButton.onClick.AddListener(rightButtonClick);
    }
}