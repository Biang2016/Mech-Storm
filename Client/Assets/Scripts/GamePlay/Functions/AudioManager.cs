using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoSingleton<AudioManager>
{
    private AudioManager()
    {
    }

    private Dictionary<string, int> AudioDictionary = new Dictionary<string, int>();

    private const int MaxAudioCount = 10;
    private const string ResourcePath = "Audios/";
    private AudioSource BGMAudioSource;
    private AudioSource LastAudioSource;

    public AudioMixer AudioMixer;
    public AudioMixerGroup BGMAudioMixerGroup;
    public AudioMixerGroup SoundAudioMixerGroup;

    void Start()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        AudioMixer.SetFloat("MasterVolume", masterVolume.Equals(0.0f) ? -8 : masterVolume);
        float soundVolume = PlayerPrefs.GetFloat("SoundVolume");
        AudioMixer.SetFloat("SoundVolume", soundVolume.Equals(0.0f) ? -8 : soundVolume);
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume");
        AudioMixer.SetFloat("BGMVolume", bgmVolume.Equals(0.0f) ? -8 : bgmVolume);
    }

    public void SoundPlay(string audioName)
    {
        SoundPlay(audioName, 1f);
    }

    internal void SoundPlay(string audioName, float volume = 1)
    {
        if (AudioDictionary.ContainsKey(audioName))
        {
            if (AudioDictionary[audioName] <= MaxAudioCount)
            {
                AudioClip sound = GetAudioClip(audioName);
                if (sound != null)
                {
                    StartCoroutine(PlayClipEnd(sound, audioName));
                    PlayClip(sound, volume);
                    AudioDictionary[audioName]++;
                }
            }
        }
        else
        {
            AudioDictionary.Add(audioName, 1);
            AudioClip sound = GetAudioClip(audioName);
            if (sound != null)
            {
                StartCoroutine(PlayClipEnd(sound, audioName));
                PlayClip(sound, volume);
                AudioDictionary[audioName]++;
            }
        }
    }

    public void SoundPause(string audioName)
    {
        if (LastAudioSource != null)
        {
            LastAudioSource.Pause();
        }
    }

    public void AllSoundPause()
    {
        AudioSource[] allSource = FindObjectsOfType<AudioSource>();
        if (allSource != null && allSource.Length > 0)
        {
            for (int i = 0; i < allSource.Length; i++)
            {
                allSource[i].Pause();
            }
        }
    }

    public void SoundStop(string audioName)
    {
        GameObject obj = transform.Find(audioName).gameObject;
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    public void BGMSetVolume(float volume)
    {
        if (BGMAudioSource != null)
        {
            BGMAudioSource.volume = volume;
        }
    }

    private string currentBGM;

    public void BGMFadeIn(string bgmName, float duration = 1f, float volume = 0.6f)
    {
        if (currentBGM != bgmName)
        {
            if (bgmName != null)
            {
                AudioClip bgmSound = GetAudioClip(bgmName);
                if (bgmSound != null)
                {
                    currentBGM = bgmName;
                    StartCoroutine(Co_BGMFadeOut(duration));
                    PlayOnceBGMAudioClip(bgmSound, volume);
                    StartCoroutine(Co_BGMFadeIn(duration, volume));
                }
            }
        }
    }

    private Coroutine BGMLoop;

    public void BGMLoopInList(List<string> bgmNames, float volume = 1.0f)
    {
        if (BGMLoop != null) StopCoroutine(BGMLoop);
        StartCoroutine(Co_BGMFadeOut(0.5f));
        BGMLoop = StartCoroutine(Co_BGMLoopInList(bgmNames, volume));
    }

    IEnumerator Co_BGMLoopInList(List<string> bgmNames, float volume = 1.0f)
    {
        if (bgmNames.Count == 1)
        {
            while (true)
            {
                while (BGMAudioSource != null && BGMAudioSource.isPlaying)
                {
                    yield return new WaitForSeconds(0.5f);
                }

                BGMFadeIn(bgmNames[0], volume: volume);
            }
        }
        else
        {
            int lastIndex = Random.Range(0, bgmNames.Count);
            while (true)
            {
                int index = Random.Range(0, bgmNames.Count);
                if (index != lastIndex)
                {
                    lastIndex = index;
                    while (BGMAudioSource != null && BGMAudioSource.isPlaying)
                    {
                        yield return new WaitForSeconds(0.5f);
                    }

                    BGMFadeIn(bgmNames[index], volume: volume);
                }
            }
        }
    }

    IEnumerator Co_BGMFadeIn(float duration, float targetVolume)
    {
        if (BGMAudioSource != null && BGMAudioSource.gameObject)
        {
            float increase = targetVolume / 10;
            BGMAudioSource.volume = 0;
            for (int i = 0; i < 10; i++)
            {
                BGMAudioSource.volume += increase;
                yield return new WaitForSeconds(duration / 10);
            }
        }
    }

    IEnumerator Co_BGMFadeOut(float duration)
    {
        if (BGMAudioSource != null && BGMAudioSource.gameObject)
        {
            AudioSource abandonBGM = BGMAudioSource;
            BGMAudioSource = null;
            float decrease = abandonBGM.volume / 10;
            for (int i = 0; i < 10; i++)
            {
                abandonBGM.volume -= decrease;
                yield return new WaitForSeconds(duration / 10);
            }

            Destroy(abandonBGM.gameObject);
        }
    }

    public void BGMPause()
    {
        if (BGMAudioSource != null)
        {
            BGMAudioSource.Pause();
        }
    }

    public void BGMStop()
    {
        if (BGMLoop != null) StopCoroutine(BGMLoop);
        if (BGMAudioSource != null && BGMAudioSource.gameObject)
        {
            Destroy(BGMAudioSource.gameObject);
            BGMAudioSource = null;
        }
    }

    public void BGMReplay()
    {
        if (BGMAudioSource != null)
        {
            BGMAudioSource.Play();
        }
    }

    #region 音效资源路径

    private static Dictionary<string, AudioClip> AudioClipDict_ABModeOnly = new Dictionary<string, AudioClip>();

    public static void ClearAudioClipDict()
    {
        AudioClipDict_ABModeOnly.Clear();
    }

    public static void AddAudioRes(string audioName, AudioClip ac)
    {
        AudioClipDict_ABModeOnly.Add(audioName, ac);
    }

    private AudioClip GetAudioClip(string audioName)
    {
        if (Initialization.Instance.IsABMode)
        {
            if (AudioClipDict_ABModeOnly.ContainsKey(audioName))
            {
                return AudioClipDict_ABModeOnly[audioName];
            }
        }
        else
        {
            AudioClip audioClip = Resources.Load(ResourcePath + audioName) as AudioClip;
            return audioClip;
        }

        return null;
    }

    #endregion

    #region 背景音乐

    private void PlayBGMAudioClip(AudioClip audioClip, float volume = 1f, bool isLoop = false, string name = null)
    {
        if (audioClip != null)
        {
            GameObject obj = new GameObject(name);
            obj.transform.parent = transform;
            AudioSource Clip = obj.AddComponent<AudioSource>();
            Clip.clip = audioClip;
            Clip.volume = volume;
            Clip.loop = isLoop;
            Clip.pitch = 1f;
            Clip.Play();
            Clip.outputAudioMixerGroup = BGMAudioMixerGroup;
            BGMAudioSource = Clip;
        }
    }

    private void PlayOnceBGMAudioClip(AudioClip audioClip, float volume = 1f, string name = null)
    {
        PlayBGMAudioClip(audioClip, volume, false, name ?? "BGMSound");
    }

    private void PlayLoopBGMAudioClip(AudioClip audioClip, float volume = 1f, string name = null)
    {
        PlayBGMAudioClip(audioClip, volume, true, name ?? "LoopSound");
    }

    #endregion

    #region  音效

    private void PlayClip(AudioClip audioClip, float volume = 1f, string name = null)
    {
        if (audioClip == null)
        {
            return;
        }
        else
        {
            GameObject obj = new GameObject(name ?? "SoundClip");
            obj.transform.parent = transform;
            AudioSource source = obj.AddComponent<AudioSource>();
            StartCoroutine(PlayClipEndDestroy(audioClip, obj));
            source.pitch = 1f;
            source.volume = volume;
            source.clip = audioClip;
            source.outputAudioMixerGroup = SoundAudioMixerGroup;
            source.Play();
            LastAudioSource = source;
        }
    }

    private IEnumerator PlayClipEndDestroy(AudioClip audioClip, GameObject soundGO)
    {
        if (soundGO != null && audioClip != null)
        {
            yield return new WaitForSeconds(audioClip.length * Time.timeScale);
            Destroy(soundGO);
        }
    }

    private IEnumerator PlayClipEnd(AudioClip audioClip, string audioName)
    {
        if (audioClip != null)
        {
            yield return new WaitForSeconds(audioClip.length * Time.timeScale);
            AudioDictionary[audioName]--;
            if (AudioDictionary[audioName] <= 0)
            {
                AudioDictionary.Remove(audioName);
            }
        }
    }

    #endregion
}