using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmPanel : BaseUIForm
{
    [SerializeField] private Text DescText;
    [SerializeField] private Text LeftButtonText;
    [SerializeField] private Text RightButtonText;
    [SerializeField] private Button LeftButton;
    [SerializeField] private Button RightButton;

    void Awake()
    {
        UIType.IsClearStack = false;
        UIType.UIForm_LucencyType = UIFormLucencyTypes.Blur;
        UIType.UIForms_ShowMode = UIFormShowModes.Return;
        UIType.UIForms_Type = UIFormTypes.PopUp;
    }

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseUIForm();
            }
        }
    }

    public void Initialize(string descText, string leftButtonText, string rightButtonText, UnityAction leftButtonClick, UnityAction rightButtonClick)
    {
        DescText.text = descText;
        LeftButtonText.text = leftButtonText;
        RightButtonText.text = rightButtonText;
        LeftButton.onClick.RemoveAllListeners();
        RightButton.onClick.RemoveAllListeners();
        LeftButton.onClick.AddListener(leftButtonClick);
        RightButton.onClick.AddListener(rightButtonClick);
    }
}