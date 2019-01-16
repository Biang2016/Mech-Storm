using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;

public partial class OutGameManager
{
    [SerializeField] private Text GameNameText;
    [SerializeField] private Text ClientVersionText;
    [SerializeField] private Text ServerVersionText;
    [SerializeField] private Text StageText;
    [SerializeField] private Text SizeText;
    [SerializeField] private Text ProgressText;
    [SerializeField] private Slider ProgressSlider;
    [SerializeField] private Button EnterGameButton;

    Color TextDefaultColor;

    private static string m_ResourcesURL = "http://www.biangstudio.com/MechStormResources/";
    private static string m_DownloadPath = "";
    Regex m_FileSizeListRegex = new Regex("^(?<size>[0-9]+)-(?<md5>[a-zA-Z0-9]+)-./(?<filepath>.*/)?(?<filename>[^/]+)$");
    List<DownloadFileInfo> m_FileListInfos = new List<DownloadFileInfo>();
    List<string> DownloadIgnoreFileList = new List<string>();
    List<DownloadItem> m_DownloadItems = new List<DownloadItem>();
    long m_FileListTotalSize = 0;
    string m_FileListTotalSize_Readable;
    int m_DownloadFileCount = 0;

    bool UpdateCompleted = false;
    UpdateState updateState = UpdateState.None;

    enum UpdateState
    {
        None,
        Checking,
        Updating,
        Finished,
    }

    private void Awake_Update()
    {
        TextDefaultColor = StageText.color;
        updateState = UpdateState.None;
        m_DownloadPath = Application.streamingAssetsPath + "/AssetBundles/";
    }

    private void Start_Update()
    {
        TryUpdate();
    }

    int frameCount = 0;
    int frameGap = 1;

    private void Update_Update()
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
                    ProgressSlider.value = 0;
                    ProgressText.text = "0%";
                }
                else
                {
                    float process = (float) downloadedFileTotalSize / m_FileListTotalSize;
                    ProgressSlider.value = process;
                    ProgressText.text = string.Format("{0:F}", process * 100) + "%";
                }
            }
        }
    }

    private void TryUpdate()
    {
        Initialized_Update();
        GenerateMD5SumForCurrentDownloadFolder();
        StartCoroutine(UpdateClient());
    }

    private void Initialized_Update()
    {
        GameNameText.text = "Mech Storm (Loading)";
        ClientVersionText.text = "Client Version:  " + Client.ClientVersion;
        ServerVersionText.text = "Server Version: Unknown";
        StageText.text = "Downloading...";
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

    Dictionary<string, string> DownloadFileMD5Sum = new Dictionary<string, string>();

    private void GenerateMD5SumForCurrentDownloadFolder()
    {
        DirectoryInfo di = new DirectoryInfo(m_DownloadPath);
        FileInfo[] fis = di.GetFiles("*", SearchOption.AllDirectories);
        foreach (FileInfo fi in fis)
        {
            if (fi.Extension == ".meta") continue;
            string md5sum = FileUtils.GetMD5WithFilePath(fi.FullName);
            DownloadFileMD5Sum.Add(fi.FullName.Replace("\\", "/").Replace(m_DownloadPath, ""), md5sum);
        }
    }

    IEnumerator UpdateClient()
    {
        yield return GetFileSizeList();
        updateState = UpdateState.Updating;

        if (m_FileListInfos.Count == 0)
        {
            FinishedDownload();
        }

        foreach (DownloadFileInfo fi in m_FileListInfos)
        {
            HttpDownloadItem hdi = new HttpDownloadItem(m_ResourcesURL + fi.FilePath + fi.FileName, m_DownloadPath + fi.FilePath, fi);
            m_DownloadItems.Add(hdi);
            hdi.StartDownload(DownloadFinish);
        }
    }

    void DownloadFinish()
    {
        m_DownloadFileCount--;
        Debug.Log(m_DownloadFileCount);
        if (m_DownloadFileCount == 0)
        {
            StageText.text = "Update Completed!";
            StageText.color = Color.yellow;
            FinishedDownload();
        }
    }

    void FinishedDownload()
    {
        GameNameText.text = "Mech Storm (Ready)";
        updateState = UpdateState.Finished;
        SizeText.text = FileUtils.ByteToReadableFileSize(m_FileListTotalSize) + "/" + m_FileListTotalSize_Readable;
        ProgressSlider.value = 1;
        StageText.text = "OK.";
        ProgressText.text = "100%";
        UpdateCompleted = true;
        Start_Loading();
    }

    IEnumerator GetFileSizeList()
    {
        WWW www_serverVersion = new WWW(m_ResourcesURL + "ServerVersion");
        yield return www_serverVersion;
        if (www_serverVersion.error != null)
        {
            Debug.Log("Loading error:" + www_serverVersion.url + "\n" + www_serverVersion.error);
        }
        else
        {
            string content = www_serverVersion.text;
            string version = content.Split('\n')[0];
            Client.ServerVersion = version;
            ServerVersionText.text = "Server Version: " + version;
        }

        if (Client.ServerVersion == Client.ClientVersion)
        {
            StageText.text = "Checking resources...";
            updateState = UpdateState.Checking;
        }

        WWW www_ignore = new WWW(m_ResourcesURL + ".downloadIgnore");
        yield return www_ignore;
        if (www_ignore.error != null)
        {
            Debug.Log("Loading error:" + www_ignore.url + "\n" + www_ignore.error);
        }
        else
        {
            string content = www_ignore.text;
            string[] lines = content.Split('\n');
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    string fullFileName = line.TrimEnd('\n');
                    DownloadIgnoreFileList.Add(fullFileName);
                }
            }
        }

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
                        string filePath = match.Groups["filepath"].Value;
                        string fileName = match.Groups["filename"].Value;
                        string fileFullPath = filePath + fileName;
                        if (DownloadIgnoreFileList.Contains(fileFullPath)) continue;
                        string md5sum = match.Groups["md5"].Value.ToUpper();
                        if (DownloadFileMD5Sum.ContainsKey(fileFullPath))
                        {
                            string md5sum_local = DownloadFileMD5Sum[fileFullPath];
                            if (md5sum_local.Equals(md5sum))
                            {
                                continue;
                            }
                        }

                        m_FileListTotalSize += fileSize;
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

}