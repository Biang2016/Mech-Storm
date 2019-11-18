using System;
using System.Collections.Generic;

public static class ScriptExecuteSettingManager
{
    static Dictionary<string, Type> mScriptsExecuteSettingMap = new Dictionary<string, Type>();

    static ScriptExecuteSettingManager()
    {
    }

    public static void AddScriptsExecuteSettingTypes<T>() where T : ScriptExecuteSettingBase
    {
        if (!mScriptsExecuteSettingMap.ContainsKey(typeof(T).ToString()))
        {
            mScriptsExecuteSettingMap.Add(typeof(T).ToString(), typeof(T));
        }
    }

    public static ScriptExecuteSettingBase GetNewScriptExecuteSetting(string ScriptsExecuteSettingName)
    {
        Type type = mScriptsExecuteSettingMap[ScriptsExecuteSettingName];
        ScriptExecuteSettingBase newSESB = (ScriptExecuteSettingBase) type.Assembly.CreateInstance(type.ToString());
        return newSESB;
    }
}