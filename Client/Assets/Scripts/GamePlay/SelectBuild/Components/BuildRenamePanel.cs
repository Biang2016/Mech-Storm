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
        UIType.IsClearStack = false;
        UIType.IsClickElsewhereClose = true;
        UIType.IsESCClose = true;
        UIType.UIForms_Type = UIFormTypes.PopUp;
        UIType.UIForm_LucencyType = UIFormLucencyTypes.ImPenetrable;
        UIType.UIForms_ShowMode = UIFormShowModes.Return;

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
            BuildRequest request = new BuildRequest(Client.Instance.Proxy.ClientId, currentEditBuildInfo, SelectBuildManager.Instance.CurrentGameMode == SelectBuildManager.GameMode.Single);
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