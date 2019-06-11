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
    [SerializeField] private InputField InputField;
    [SerializeField] private Text InputFieldPlaceHolderText;

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: true,
            isClickElsewhereClose: true,
            uiForms_Type: UIFormTypes.PopUp,
            uiForms_ShowMode: UIFormShowModes.Return,
            uiForm_LucencyType: UIFormLucencyTypes.Blur);
    }

    protected override void ChildUpdate()
    {
        base.ChildUpdate();
        if (Input.GetKeyUp(KeyCode.Return))
        {
            ConfirmClick?.Invoke();
        }
    }

    private UnityAction ConfirmClick = null;

    public void Initialize(string descText, string leftButtonText, string rightButtonText, UnityAction leftButtonClick, UnityAction rightButtonClick, string inputFieldPlaceHolderText = null)
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: true,
            isClickElsewhereClose: true,
            uiForms_Type: UIFormTypes.PopUp,
            uiForms_ShowMode: UIFormShowModes.Return,
            uiForm_LucencyType: UIFormLucencyTypes.Blur);
        ConfirmClick = leftButtonClick;
        DescText.text = descText;
        LeftButtonText.text = leftButtonText;
        RightButtonText.text = rightButtonText;
        LeftButton.onClick.RemoveAllListeners();
        RightButton.onClick.RemoveAllListeners();
        LeftButton.onClick.AddListener(leftButtonClick);
        RightButton.onClick.AddListener(rightButtonClick);

        if (inputFieldPlaceHolderText != null)
        {
            InputFieldPlaceHolderText.text = inputFieldPlaceHolderText;
            InputField.ActivateInputField();
        }

        InputField.gameObject.SetActive(inputFieldPlaceHolderText != null);
    }

    public string InputText => InputField.text;
}