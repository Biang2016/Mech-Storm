using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;
    private Dictionary<string, int> AudioDictionary = new Dictionary<string, int>();

    private const int MaxAudioCount = 10;
    private const string ResourcePath = "Audios/";
    private const string StreamingAssetsPath = "";
    private AudioSource BGMAudioSource;
    private AudioSource LastAudioSource;

    #region Mono Function

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
            {
                Destroy(transform.gameObject);
            }
        }
    }

    #endregion

    public void SoundPlay(string audioname)
    {
        SoundPlay(audioname, 1f);
    }

    /// <summary>
    /// 播放
    /// </summary>
    /// <param name="audioname"></param>
    public void SoundPlay(string audioname, float volume = 1)
    {
        if (AudioDictionary.ContainsKey(audioname))
        {
            if (AudioDictionary[audioname] <= MaxAudioCount)
            {
                AudioClip sound = GetAudioClip(audioname);
                if (sound != null)
                {
                    StartCoroutine(PlayClipEnd(sound, audioname));
                    PlayClip(sound, volume);
                    AudioDictionary[audioname]++;
                }
            }
        }
        else
        {
            AudioDictionary.Add(audioname, 1);
            AudioClip sound = GetAudioClip(audioname);
            if (sound != null)
            {
                StartCoroutine(PlayClipEnd(sound, audioname));
                PlayClip(sound, volume);
                AudioDictionary[audioname]++;
            }
        }
    }

    /// <summary>
    /// 暂停
    /// </summary>
    /// <param name="audioname"></param>
    public void SoundPause(string audioname)
    {
        if (LastAudioSource != null)
        {
            LastAudioSource.Pause();
        }
    }

    /// <summary>
    /// 暂停所有音效音乐
    /// </summary>
    public void SoundAllPause()
    {
        AudioSource[] allsource = FindObjectsOfType<AudioSource>();
        if (allsource != null && allsource.Length > 0)
        {
            for (int i = 0; i < allsource.Length; i++)
            {
                allsource[i].Pause();
            }
        }
    }

    /// <summary>
    /// 停止特定的音效
    /// </summary>
    /// <param name="audioname"></param>
    public void SoundStop(string audioname)
    {
        GameObject obj = transform.Find("audioname").gameObject;
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    /// <summary>
    /// 设置音量
    /// </summary>
    public void BGMSetVolume(float volume)
    {
        if (BGMAudioSource != null)
        {
            BGMAudioSource.volume = volume;
        }
    }

    private string currentBGM;

    public void BGMFadeIn(string bgmname, float duration = 1f, float volume = 0.6f)
    {
        if (currentBGM == bgmname) return;
        else
        {
            if (bgmname != null)
            {
                AudioClip bgmsound = GetAudioClip(bgmname);
                if (bgmsound != null)
                {
                    currentBGM = bgmname;
                    StartCoroutine(Co_BGMFadeOut(duration));
                    PlayLoopBGMAudioClip(bgmsound, volume);
                    StartCoroutine(Co_BGMFadeIn(duration, volume));
                }
            }
        }
    }

    IEnumerator Co_BGMFadeIn(float duration, float targetVolumn)
    {
        float increase = targetVolumn / 10;
        BGMAudioSource.volume = 0;
        for (int i = 0; i < 10; i++)
        {
            BGMAudioSource.volume += increase;
            yield return new WaitForSeconds(duration / 10);
        }
    }

    IEnumerator Co_BGMFadeOut(float duration)
    {
        if (BGMAudioSource != null && BGMAudioSource.gameObject)
        {
            AudioSource abandonBGM = BGMAudioSource;
            float decrease = abandonBGM.volume / 10;
            for (int i = 0; i < 10; i++)
            {
                abandonBGM.volume -= decrease;
                yield return new WaitForSeconds(duration / 10);
            }

            Destroy(abandonBGM.gameObject);
        }
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void BGMPause()
    {
        if (BGMAudioSource != null)
        {
            BGMAudioSource.Pause();
        }
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void BGMStop()
    {
        if (BGMAudioSource != null && BGMAudioSource.gameObject)
        {
            Destroy(BGMAudioSource.gameObject);
            BGMAudioSource = null;
        }
    }


    /// <summary>
    /// 重新播放
    /// </summary>
    public void BGMReplay()
    {
        if (BGMAudioSource != null)
        {
            BGMAudioSource.Play();
        }
    }


    #region 音效资源路径

    enum eResType
    {
        AB = 0,
        CLIP = 1
    }

    /// <summary>
    /// 下载音效
    /// </summary>
    /// <param name="aduioname"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private AudioClip GetAudioClip(string aduioname, eResType type = eResType.CLIP)
    {
        AudioClip audioclip = null;
        switch (type)
        {
            case eResType.AB:
                break;
            case eResType.CLIP:
                audioclip = GetResAudioClip(aduioname);
                break;
            default:
                break;
        }

        return audioclip;
    }

    private IEnumerator GetAbAudioClip(string aduioname)
    {
        yield return null;
    }

    private AudioClip GetResAudioClip(string aduioname)
    {
        return Resources.Load(ResourcePath + aduioname) as AudioClip;
    }

    #endregion

    #region 背景音乐

    /// <summary>
    /// 背景音乐控制器
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="volume"></param>
    /// <param name="isloop"></param>
    /// <param name="name"></param>
    private void PlayBGMAudioClip(AudioClip audioClip, float volume = 1f, bool isloop = false, string name = null)
    {
        if (audioClip == null)
        {
            return;
        }
        else
        {
            GameObject obj = new GameObject(name);
            obj.transform.parent = transform;
            AudioSource LoopClip = obj.AddComponent<AudioSource>();
            LoopClip.clip = audioClip;
            LoopClip.volume = volume;
            LoopClip.loop = true;
            LoopClip.pitch = 1f;
            LoopClip.Play();
            BGMAudioSource = LoopClip;
        }
    }

    /// <summary>
    /// 播放一次的背景音乐
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="volume"></param>
    /// <param name="name"></param>
    private void PlayOnceBGMAudioClip(AudioClip audioClip, float volume = 1f, string name = null)
    {
        PlayBGMAudioClip(audioClip, volume, false, name == null ? "BGMSound" : name);
    }

    /// <summary>
    /// 循环播放的背景音乐
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="volume"></param>
    /// <param name="name"></param>
    private void PlayLoopBGMAudioClip(AudioClip audioClip, float volume = 1f, string name = null)
    {
        PlayBGMAudioClip(audioClip, volume, true, name == null ? "LoopSound" : name);
    }

    #endregion

    #region  音效

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="volume"></param>
    /// <param name="name"></param>
    private void PlayClip(AudioClip audioClip, float volume = 1f, string name = null)
    {
        if (audioClip == null)
        {
            return;
        }
        else
        {
            GameObject obj = new GameObject(name == null ? "SoundClip" : name);
            obj.transform.parent = transform;
            AudioSource source = obj.AddComponent<AudioSource>();
            StartCoroutine(PlayClipEndDestroy(audioClip, obj));
            source.pitch = 1f;
            source.volume = volume;
            source.clip = audioClip;
            source.Play();
            LastAudioSource = source;
        }
    }

    /// <summary>
    /// 播放玩音效删除物体
    /// </summary>
    /// <param name="audioclip"></param>
    /// <param name="soundobj"></param>
    /// <returns></returns>
    private IEnumerator PlayClipEndDestroy(AudioClip audioclip, GameObject soundobj)
    {
        if (soundobj == null || audioclip == null)
        {
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(audioclip.length * Time.timeScale);
            Destroy(soundobj);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayClipEnd(AudioClip audioclip, string audioname)
    {
        if (audioclip != null)
        {
            yield return new WaitForSeconds(audioclip.length * Time.timeScale);
            AudioDictionary[audioname]--;
            if (AudioDictionary[audioname] <= 0)
            {
                AudioDictionary.Remove(audioname);
            }
        }

        yield break;
    }

    #endregion
}