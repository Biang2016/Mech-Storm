using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpdateManager : MonoSingleton<UpdateManager>
{
    [SerializeField] private Text GameNameText;
    [SerializeField] private Text ClientVersionText;
    [SerializeField] private Text ServerVersionText;
    [SerializeField] private Text DownloadingText;
    [SerializeField] private Text SizeText;
    [SerializeField] private Text ProgressText;
    [SerializeField] private Slider DownloadSlider;
    [SerializeField] private Button EnterGameButton;

    Color TextDefaultColor;

    private static string m_ResourcesURL = "http://www.biangstudio.com/MechStormResources/";
    Regex m_FileSizeListRegex = new Regex("^(?<size>[0-9]+)-(?<md5>[a-zA-Z0-9]+)-./(?<filepath>.*/)?(?<filename>[^/]+)$");
    List<DownloadFileInfo> m_FileListInfos = new List<DownloadFileInfo>();
    List<DownloadItem> m_DownloadItems = new List<DownloadItem>();
    long m_FileListTotalSize = 0;
    string m_FileListTotalSize_Readable;
    int m_DownloadFileCount = 0;

    UpdateState updateState = UpdateState.None;
    enum UpdateState
    {
        None,
        Updating,
        Finished,
    }

    private void Awake()
    {
        TextDefaultColor = DownloadingText.color;
        updateState = UpdateState.None;
        EnterGameButton.gameObject.SetActive(false);
    }

    void Start()
    {
        TryUpdate();
    }

    int frameCount = 0;
    int frameGap = 1;
    private void Update()
    {
        if (updateState == UpdateState.Updating)
        {
            frameCount++;
            if (frameCount % frameGap == 0)
            {
                long downloadedFileTotalSize = 0;
                foreach (DownloadItem di in m_DownloadItems)
                {
                    if (di != null)
                    {
                        downloadedFileTotalSize += di.GetCurrentLength();
                    }
                }
                SizeText.text = FileUtils.ByteToReadableFileSize(downloadedFileTotalSize) + "/" + m_FileListTotalSize_Readable;
                if (m_FileListTotalSize == 0)
                {
                    DownloadSlider.value = 0;
                    ProgressText.text = "0%";
                }
                else
                {
                    float process = (float)downloadedFileTotalSize / m_FileListTotalSize;
                    DownloadSlider.value = process;
                    ProgressText.text = string.Format("{0:F}", process * 100) + "%";
                }
            }
        }
    }

    public void Initialized()
    {
        ClientVersionText.text = "Client Version: " + Client.ClientVersion;
        ServerVersionText.text = "Server Version: Unknown";
        SizeText.text = FileUtils.ByteToReadableFileSize(0);
        DownloadSlider.value = 0;
        ProgressText.text = "0%";
        m_FileListInfos.Clear();
        m_DownloadItems.Clear();
        m_FileListTotalSize = 0;
        m_FileListTotalSize_Readable = "0B";
        m_DownloadFileCount = 0;
        DownloadingText.color = TextDefaultColor;
    }

    public void TryUpdate()
    {
        Initialized();
        StartCoroutine(UpdateClient());
    }

    IEnumerator UpdateClient()
    {
        yield return GetFileSizeList();
        updateState = UpdateState.Updating;
        foreach (DownloadFileInfo fi in m_FileListInfos)
        {
            HttpDownloadItem hdi = new HttpDownloadItem(m_ResourcesURL + fi.FilePath + fi.FileName, Application.streamingAssetsPath + "/download/" + fi.FilePath, fi);
            m_DownloadItems.Add(hdi);
            hdi.StartDownload(DownloadFinish);
        }
    }

    void DownloadFinish()
    {
        m_DownloadFileCount--;
        if (m_DownloadFileCount == 0)
        {
            DownloadingText.text = "Update Completed!";
            DownloadingText.color = Color.yellow;
            FinishedDownload();
        }
    }

    void FinishedDownload()
    {
        updateState = UpdateState.Finished;
        SizeText.text = FileUtils.ByteToReadableFileSize(m_FileListTotalSize) + "/" + m_FileListTotalSize_Readable;
        DownloadSlider.value = 1;
        ProgressText.text = "100%";
        EnterGameButton.gameObject.SetActive(true);
        EnterGameButton.enabled = true;
    }

    IEnumerator GetFileSizeList()
    {
        WWW www = new WWW(m_ResourcesURL + "FileSizeList.txt");
        yield return www;
        if (www.error != null)
        {
            Debug.Log("Loading error:" + www.url + "\n" + www.error);
        }
        else
        {
            string content = www.text;
            string[] lines = content.Split('\n');
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    if (m_FileSizeListRegex.IsMatch(line))
                    {
                        Match match = m_FileSizeListRegex.Match(line);
                        long fileSize = long.Parse(match.Groups["size"].Value);
                        m_FileListTotalSize += fileSize;
                        string md5sum = match.Groups["md5"].Value;
                        string filePath = match.Groups["filepath"].Value;
                        string fileName = match.Groups["filename"].Value;
                        DownloadFileInfo fi = new DownloadFileInfo(fileName, filePath, md5sum, fileSize);
                        m_FileListInfos.Add(fi);
                    }
                }
            }
            m_FileListTotalSize_Readable = FileUtils.ByteToReadableFileSize(m_FileListTotalSize);
            m_DownloadFileCount = m_FileListInfos.Count;
        }

        yield return null;
    }

    public class DownloadFileInfo
    {
        public string FileName;
        public string FilePath;
        public string MD5;
        public long FileSize;
        public string FileReadableSize;
        public DownloadFileInfo(string filename, string filePath, string md5sum, long fileSize)
        {
            FileName = filename;
            FilePath = filePath;
            MD5 = md5sum;
            FileSize = fileSize;
            FileReadableSize = FileUtils.ByteToReadableFileSize(fileSize);
        }
    }

    public void OnEnterGameButtonClick()
    {
        Debug.Log("123");
    }
}
