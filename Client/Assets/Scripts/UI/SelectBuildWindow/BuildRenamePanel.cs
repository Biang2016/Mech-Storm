using UnityEngine;
using UnityEngine.UI;

public class BuildRenamePanel : MonoBehaviour
{
    public InputField InputField;

    private BuildInfo currentEditBuildInfo;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
    }

    public void OnConfirmBuildNameButtonClick()
    {
        if (!string.IsNullOrEmpty(InputField.text))
        {
            currentEditBuildInfo.BuildName = InputField.text;
            BuildRequest request = new BuildRequest(Client.Instance.Proxy.ClientId, currentEditBuildInfo);
            Client.Instance.Proxy.SendMessage(request);
            HidePanel();
        }
    }

    public void OnCancelButtonClick()
    {
        HidePanel();
    }

    public void ShowPanel(BuildInfo buildInfo)
    {
        currentEditBuildInfo = buildInfo;
        gameObject.SetActive(true);
        InputField.text = currentEditBuildInfo.BuildName;
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}