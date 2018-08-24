using UnityEngine;
using UnityEngine.UI;

public class BuildRenamePanel : MonoBehaviour
{
    public InputField InputField;

    public void OnConfirmBuildNameButtonClick()
    {
        if (!string.IsNullOrEmpty(InputField.text))
        {
            SelectBuildManager.Instance.CurrentEditBuildButton.BuildInfo.BuildName = InputField.text;
            BuildRequest request = new BuildRequest(Client.Instance.Proxy.ClientId, SelectBuildManager.Instance.CurrentEditBuildButton.BuildInfo);
            Client.Instance.Proxy.SendMessage(request);
            HidePanel();
        }
    }

    public void OnCancelButtonClick()
    {
        HidePanel();
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        InputField.text = "";
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}