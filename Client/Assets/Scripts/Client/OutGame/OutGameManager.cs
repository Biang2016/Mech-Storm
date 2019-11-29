using UnityEngine;
using UnityEngine.SceneManagement;

public partial class OutGameManager : OutGameMonoSingleton<OutGameManager>
{
    public static string ClientVersion = "1.0.4";
    public static string ServerVersion = "";

    void Awake()
    {
        Awake_Update();
        Initialized();

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
    }

    private void Initialized()
    {
        GameNameText.text = "Mech Storm";
        ClientVersionText.text = "Client Version:  " + ClientVersion;
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
        get { return UpdateCompleted; }
    }

    void FinishedExtract()
    {
        EnterGameButton.gameObject.SetActive(true);
        if (JustDownloadNewDLLFiles && OutGameInitialization.Instance.AutoUpdate)
        {
            EnterGameButtonText.text = "Please Restart the Game";
        }
        else
        {
            EnterGameButtonText.text = "Start the Game";
        }
    }

    public void OnEnterGameButtonClick()
    {
        if (JustDownloadNewDLLFiles && OutGameInitialization.Instance.AutoUpdate)
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }
    }
}