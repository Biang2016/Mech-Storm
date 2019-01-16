using System.Net.Mime;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class OutGameManager : MonoSingleton<OutGameManager>
{
    void Awake()
    {
        Initialized();
        Awake_Update();
        Awake_Loading();

        EnterGameButton.gameObject.SetActive(false);
    }

    void Start()
    {
        Start_Update();
    }

    void Update()
    {
        if (!UpdateCompleted)
        {
            Update_Update();
        }
        else
        {
            Update_Loading();
        }
    }

    private void Initialized()
    {
        GameNameText.text = "Mech Storm";
        ClientVersionText.text = "Client Version:  " + Client.ClientVersion;
        ServerVersionText.text = "Server Version: Unknown";
        StageText.text = "Preparing...";
        SizeText.text = FileUtils.ByteToReadableFileSize(0);
        ProgressSlider.value = 0;
        ProgressText.text = "0%";
        m_FileListInfos.Clear();
        m_DownloadItems.Clear();
        m_FileListTotalSize = 0;
        m_FileListTotalSize_Readable = "0B";
        m_DownloadFileCount = 0;
        StageText.color = TextDefaultColor;
    }

    public bool ReadyToLaunch
    {
        get { return UpdateCompleted && LoadingCompleted; }
    }

    void FinishedExtract()
    {
        EnterGameButton.gameObject.SetActive(true);
    }


    public void OnEnterGameButtonClick()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }
}