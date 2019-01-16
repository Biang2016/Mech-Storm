using System.IO;
using UnityEngine;

public partial class OutGameManager
{
    void Awake_Loading()
    {
    }

    void Start_Loading()
    {
        string AudioPath = Path.Combine(Application.streamingAssetsPath, "Audio");
        DirectoryInfo di = new DirectoryInfo(AudioPath);
        AudioTotalCount = di.GetFiles("*.mp3", SearchOption.AllDirectories).Length + di.GetFiles("*.wav", SearchOption.AllDirectories).Length;
        SizeText.text = 0 + "/" + AudioTotalCount;
        ProgressSlider.value = 0;
        StageText.text = "Extract Audios...";
        ProgressText.text = "0%";
        //AudioManager.Instance.LoadAllAudio(FinishedExtractAudio);
    }

    void Update_Loading()
    {
    }

    internal int AudioTotalCount = 0;

    private int audioLoadedCount = 0;

    internal int AudioLoadedCount
    {
        get { return audioLoadedCount; }

        set
        {
            audioLoadedCount = value;
            SizeText.text = audioLoadedCount + "/" + AudioTotalCount;
            float progress = (float) audioLoadedCount / AudioTotalCount;
            ProgressSlider.value = progress;
            StageText.text = "Extract Audios...";
            ProgressText.text = string.Format("{0:F}", progress * 100) + "%";
        }
    }

    bool LoadingCompleted
    {
        get { return AudioLoadingComplete; }
    }

    bool AudioLoadingComplete = false;

    void ExtractResources()
    {
    }

    void FinishedExtractAudio()
    {
        AudioLoadingComplete = true;
        FinishedExtract();
    }

}