using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildRenamePanel : BaseUIForm
{
    [SerializeField] private InputField InputField;
    [SerializeField] private Button ConfirmButton;
    [SerializeField] private Button CancelButton;
    [SerializeField] private Text TitleText;
    [SerializeField] private Text PlaceHolderText;
    [SerializeField] private Text ConfirmButtonText;
    [SerializeField] private Text CancelButtonText;
    private BuildInfo currentEditBuildInfo;

    void Awake()
    {
        UIType.InitUIType(
            isClearStack: false,
            isESCClose: true,
            isClickElsewhereClose: true,
            uiForms_Type: UIFormTypes.PopUp,
            uiForms_ShowMode: UIFormShowModes.Return,
            uiForm_LucencyType: UIFormLucencyTypes.Lucency);

        LanguageManager.Instance.RegisterTextKeys(new List<(Text, string)>
        {
            (TitleText, "SelectBuildManagerBuild_RenameBuild_TitleText"),
            (ConfirmButtonText, "Common_Confirm"),
            (CancelButtonText, "Common_Cancel"),
            (PlaceHolderText, "SelectBuildManagerBuild_RenameBuild_PlaceHolderText"),
        });

        ConfirmButton.onClick.AddListener(OnConfirmBuildNameButtonClick);
        CancelButton.onClick.AddListener(OnCancelButtonClick);
    }

    public void ShowPanel(BuildInfo buildInfo)
    {
        currentEditBuildInfo = buildInfo;
        InputField.text = currentEditBuildInfo.BuildName;
    }

    public void OnConfirmBuildNameButtonClick()
    {
        if (!string.IsNullOrEmpty(InputField.text))
        {
            currentEditBuildInfo.BuildName = InputField.text;
            BuildRequest request = new BuildRequest(Client.Instance.Proxy.ClientID, currentEditBuildInfo, SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Single, UIManager.Instance.GetBaseUIForm<StartMenuPanel>().state == StartMenuPanel.States.Show_Single_HasStory);
            Client.Instance.Proxy.SendMessage(request);
            CloseUIForm();
        }
        else
        {
            NoticeManager.Instance.ShowInfoPanelCenter(LanguageManager.Instance.GetText("SelectBuildManagerBuild_RenameBuild_NoNameWarning"), 0, 0.8f);
        }
    }

    public void OnCancelButtonClick()
    {
        CloseUIForm();
    }
}