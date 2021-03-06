﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageManager : MonoSingleton<LanguageManager>
{
    private LanguageManager()
    {
        foreach (KeyValuePair<int, string> kv in LanguageIndices)
        {
            Languages.Add(kv.Value);
            LanguageDescs.Add(LanguagesDesc[LanguagesAbbrDict[kv.Value]]);
            LanguagesShorts.Add(LanguagesAbbrDict[kv.Value]);
        }

        foreach (string ls in LanguagesShorts)
        {
            LanguageDict.Add(ls, new Dictionary<string, string>());
        }

        LanguageManager_Common.GetText = GetText;
        LanguageManager_Common.GetCurrentLanguage = GetCurrentLanguage;

        LoadLanguageDictJson();
    }

    void Awake()
    {
        FontDict = new Dictionary<string, Font>
        {
            {"zh", ChineseFont},
            {"en", EnglishFont},
        };
        string playerPrefLanguage = PlayerPrefs.GetString("Language");
        if (LanguagesAbbrDict.ContainsKey(playerPrefLanguage))
        {
            CurrentLanguage = LanguagesAbbrDict[playerPrefLanguage];
        }
        else
        {
            CurrentLanguage = DefaultLanguage;
        }

        SetLanguage(CurrentLanguage, true);
    }

    void Start()
    {
    }

    public Font EnglishFont;
    public Font ChineseFont;

    private string DefaultLanguage = "en";
    private string CurrentLanguage = "zh";

    internal string GetCurrentLanguage()
    {
        return CurrentLanguage;
    }

    public bool IsEnglish => CurrentLanguage.Equals("en");

    public SortedDictionary<string, string> LanguagesAbbrDict = new SortedDictionary<string, string>
    {
        {"Chinese", "zh"},
        {"English", "en"},
    };

    public Dictionary<string, string> LanguagesDesc = new Dictionary<string, string>
    {
        {"zh", "简体中文"},
        {"en", "English"},
    };

    public SortedDictionary<int, string> LanguageIndices = new SortedDictionary<int, string>
    {
        {0, "English"},
        {1, "Chinese"}
    };

    internal List<string> Languages = new List<string>();
    internal List<string> LanguageDescs = new List<string>();
    internal List<string> LanguagesShorts = new List<string>();
    public static string LanguageDictFolder = Application.streamingAssetsPath + "/Config/Languages/";
    private Dictionary<string, Dictionary<string, string>> LanguageDict = new Dictionary<string, Dictionary<string, string>>();
    private Dictionary<string, string> CurrentLanguageDict;

    private void LoadLanguageDictJson()
    {
        DirectoryInfo di = new DirectoryInfo(LanguageDictFolder);
        foreach (FileInfo fi in di.GetFiles("*.json"))
        {
            StreamReader sr = new StreamReader(fi.FullName, Encoding.UTF8);
            string content = sr.ReadToEnd();
            JObject jo = (JObject) JsonConvert.DeserializeObject(content);
            foreach (KeyValuePair<string, JToken> kv in jo)
            {
                foreach (JToken jToken in kv.Value)
                {
                    if (jToken is JProperty jProperty)
                    {
                        string ls = jProperty.Name;
                        string text = jProperty.Value.Value<string>();
                        if (LanguagesShorts.Contains(ls))
                        {
                            LanguageDict[ls].Add(kv.Key, text);
                        }
                        else
                        {
                            Debug.Log("Key error in file " + fi.FullName + " key: " + ls);
                        }
                    }
                }
            }
        }

        CurrentLanguageDict = LanguageDict[CurrentLanguage];
    }

    private Dictionary<Text, string> TextKeyMap = new Dictionary<Text, string>();
    private Dictionary<TextMeshProUGUI, string> TextKeyMap_TextMeshProUGUI = new Dictionary<TextMeshProUGUI, string>();
    private Dictionary<TextMeshPro, string> TextKeyMap_TextMeshPro = new Dictionary<TextMeshPro, string>();
    private Dictionary<string, Font> FontDict;
    private HashSet<Text> TextFontBindingList = new HashSet<Text>();
    private Dictionary<Text, Dictionary<string, FontStyle>> TextFontStyleMap = new Dictionary<Text, Dictionary<string, FontStyle>>();

    public void RegisterTextKey(Text text, string s)
    {
        if (text)
        {
            TextKeyMap[text] = s;
            text.text = GetText(s);
        }
    }

    public void RegisterTextKey(TextMeshProUGUI text, string s)
    {
        if (text)
        {
            TextKeyMap_TextMeshProUGUI[text] = s;
            text.text = GetText(s);
        }
    }

    public void RegisterTextKey(TextMeshPro text, string s)
    {
        if (text)
        {
            TextKeyMap_TextMeshPro[text] = s;
            text.text = GetText(s);
        }
    }

    public void UnregisterText(Text text)
    {
        if (text)
        {
            if (TextKeyMap.ContainsKey(text))
            {
                TextKeyMap.Remove(text);
            }

            if (TextFontBindingList.Contains(text))
            {
                TextFontBindingList.Remove(text);
            }

            if (TextFontStyleMap.ContainsKey(text))
            {
                TextFontStyleMap.Remove(text);
            }
        }
    }

    public void UnregisterText(TextMeshProUGUI text)
    {
        if (text)
        {
            if (TextKeyMap_TextMeshProUGUI.ContainsKey(text))
            {
                TextKeyMap_TextMeshProUGUI.Remove(text);
            }
        }
    }

    public void UnregisterText(TextMeshPro text)
    {
        if (text)
        {
            if (TextKeyMap_TextMeshPro.ContainsKey(text))
            {
                TextKeyMap_TextMeshPro.Remove(text);
            }
        }
    }

    public void RegisterTextKeys(List<(Text, string)> textStringPairs)
    {
        foreach ((Text t, string s) in textStringPairs)
        {
            RegisterTextKey(t, s);
        }
    }

    public void RegisterTextFontBinding(Text text, Dictionary<string, FontStyle> fontStyles = null)
    {
        if (text)
        {
            TextFontBindingList.Add(text);
            if (fontStyles != null)
            {
                if (!TextFontStyleMap.ContainsKey(text))
                {
                    TextFontStyleMap.Add(text, fontStyles);
                }
            }
        }
    }

    public void RegisterTextFontBinding(List<Text> texts)
    {
        foreach (Text text in texts)
        {
            if (text)
            {
                TextFontBindingList.Add(text);
            }
        }
    }

    public string GetText(string key)
    {
        CurrentLanguageDict.TryGetValue(key, out string text);
        if (text == null)
        {
            ClientLog.Instance.PrintWarning("LanguageKey [" + key + "] not exists -- GetText()");
        }

        return text;
    }

    public void SetLanguage(string languageShort, bool forceRefresh = false)
    {
        if (CurrentLanguage.Equals(languageShort) && !forceRefresh) return;

        if (LanguagesShorts.Contains(languageShort))
        {
            CurrentLanguage = languageShort;
            CurrentLanguageDict = LanguageDict[CurrentLanguage];
            Font curFont = FontDict[CurrentLanguage];
            foreach (KeyValuePair<Text, string> kv in TextKeyMap)
            {
                CurrentLanguageDict.TryGetValue(kv.Value, out string text);
                if (text != null)
                {
                    kv.Key.text = text;
                    kv.Key.font = curFont;
                }
                else
                {
                    ClientLog.Instance.PrintWarning("LanguageKey [" + kv.Value + "] not exists --SetLanguage()");
                }
            }

            foreach (KeyValuePair<TextMeshProUGUI, string> kv in TextKeyMap_TextMeshProUGUI)
            {
                CurrentLanguageDict.TryGetValue(kv.Value, out string text);
                if (text != null)
                {
                    kv.Key.text = text;
                }
                else
                {
                    ClientLog.Instance.PrintWarning("LanguageKey [" + kv.Value + "] not exists --SetLanguage()");
                }
            }

            foreach (KeyValuePair<TextMeshPro, string> kv in TextKeyMap_TextMeshPro)
            {
                CurrentLanguageDict.TryGetValue(kv.Value, out string text);
                if (text != null)
                {
                    kv.Key.text = text;
                }
                else
                {
                    ClientLog.Instance.PrintWarning("LanguageKey [" + kv.Value + "] not exists --SetLanguage()");
                }
            }

            foreach (Text text in TextFontBindingList)
            {
                if (text != null)
                {
                    text.font = curFont;
                }
            }

            foreach (KeyValuePair<Text, Dictionary<string, FontStyle>> kv in TextFontStyleMap)
            {
                if (kv.Key != null)
                {
                    kv.Key.fontStyle = kv.Value[languageShort];
                }
            }

            UIManager.Instance.GetBaseUIForm<SelectBuildPanel>()?.RefreshCardTextLanguage();
            UIManager.Instance.GetBaseUIForm<SelectBuildPanel>()?.RefreshSelectCardTextLanguage();
            UIManager.Instance.GetBaseUIForm<ShopPanel>()?.OnLanguageChange();
            BattleManager.Instance?.SetLanguage(languageShort);
        }
    }

    public void LanguageDropdownChange(int index)
    {
        PlayerPrefs.SetString("Language", LanguageIndices[index]);
        SetLanguage(LanguagesAbbrDict[LanguageIndices[index]]);
        NoticeManager.Instance.ShowInfoPanelCenter(GetText("SettingMenu_ChangeLanguageNotice"), 0, 1f);
    }
}