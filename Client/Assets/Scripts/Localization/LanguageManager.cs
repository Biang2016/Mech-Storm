using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        string playerPrefLanguage = PlayerPrefs.GetString("Language");
        if (LanguagesAbbrDict.ContainsKey(playerPrefLanguage))
        {
            CurrentLanguage = LanguagesAbbrDict[playerPrefLanguage];
        }
        else
        {
            CurrentLanguage = DefaultLanguage;
        }

        FontDict = new Dictionary<string, Font>
        {
            {"zh", ChineseFont},
            {"en", EnglishFont},
        };

        SettingMenuManager.Instance.LanguageDropdown.ClearOptions();
        SettingMenuManager.Instance.LanguageDropdown.AddOptions(LanguageDescs);
    }

    void Start()
    {
        SetLanguage(CurrentLanguage, true);
    }

    public Font EnglishFont;
    public Font ChineseFont;

    private string DefaultLanguage = "en";
    private string CurrentLanguage = "zh";

    internal string GetCurrentLanguage()
    {
        return CurrentLanguage;
    }

    public bool IsEnglish
    {
        get { return CurrentLanguage.Equals("en"); }
    }

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
            StreamReader sr = new StreamReader(fi.FullName, Encoding.GetEncoding("gb2312"));
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
    private Dictionary<string, Font> FontDict;
    private List<Text> TextFontBindingList = new List<Text>();

    public void RegisterTextKey(Text text, string s)
    {
        TextKeyMap[text] = s;
    }

    public void RegisterTextKeys(List<(Text, string)> textStringPairs)
    {
        foreach ((Text t, string s) in textStringPairs)
        {
            RegisterTextKey(t, s);
        }
    }

    public void RegisterTextFontBinding(Text text)
    {
        TextFontBindingList.Add(text);
    }

    public void RegisterTextFontBinding(List<Text> texts)
    {
        TextFontBindingList.AddRange(texts);
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

            foreach (Text text in TextFontBindingList)
            {
                if (text != null)
                {
                    text.font = curFont;
                }
            }

            SettingMenuManager.Instance.LanguageDropdown.onValueChanged.RemoveAllListeners();
            SettingMenuManager.Instance.LanguageDropdown.value = LanguagesShorts.IndexOf(languageShort);
            SettingMenuManager.Instance.LanguageDropdown.onValueChanged.AddListener(LanguageDropdownChange);

            SelectBuildManager.Instance.RefreshCardTexts();
            SelectBuildManager.Instance.RefreshSelectCardsLanguage();
        }
    }

    public void LanguageDropdownChange(int index)
    {
        PlayerPrefs.SetString("Language", LanguageIndices[index]);
        NoticeManager.Instance.ShowInfoPanelCenter(GetText("ChangeLanguageNotice"), 0, 1f);
        SetLanguage(LanguagesAbbrDict[LanguageIndices[index]]);
    }
}