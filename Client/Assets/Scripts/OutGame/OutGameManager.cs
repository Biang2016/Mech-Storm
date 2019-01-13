using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OutGameManager : MonoSingleton<OutGameManager>
{
    [SerializeField] private Text GameNameText;
    [SerializeField] private Text ClientVersionText;
    [SerializeField] private Text ServerVersionText;
    [SerializeField] private Text DownloadingText;
    [SerializeField] private Text SizeText;
    [SerializeField] private Text ProgressText;
    [SerializeField] private Slider DownloadSlider;

    private static string ResourcesURL = "www.biangstudio.com/MechStormResources/";


    void Start()
    {
        ClientVersionText.text = "Client Version: " + Client.ClientVersion;
        ServerVersionText.text = "Server Version: Unknown";
        StartCoroutine(GetFileSizeList());
        //StartCoroutine(GetServerFile("res.ab"));
    }

    void Update()
    {
    }

    IEnumerator GetFileSizeList()
    {
        WWW www = new WWW(GetServerFileURL("FileSizeList.txt"));
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
                    long fileSize = long.Parse(line.Split('-')[0]);
                    string[] temp = line.Split('/');
                    string fileName = temp[temp.Length - 1];
                    Debug.Log(FileUtils.ByteToReadableFileSize(fileSize) + "    " + fileName);
                }
            }
        }
    }

    IEnumerator GetServerFile(string url)
    {
        url = ResourcesURL + url;
        WWW www = new WWW(url);
        while (!www.isDone)
        {
            SizeText.text = FileUtils.ByteToReadableFileSize(www.bytesDownloaded) + "/";
            DownloadSlider.value = www.progress;
            ProgressText.text = ((int) (www.progress * 100)) % 100 + "%";
            yield return null;
        }

        if (www.error != null)
        {
            Debug.Log("Loading error:" + www.url + "\n" + www.error);
        }
        else
        {
            SizeText.text = FileUtils.ByteToReadableFileSize(www.bytes.Length) + "/";
            DownloadSlider.value = 1;
            ProgressText.text = "100%";
            Debug.Log("Download complete");
        }
    }

    private string GetServerFileURL(string filename)
    {
        return ResourcesURL + filename;
        ;
    }
}