using System;
using System.Collections.Generic;

public static class SideEffectManager
{
    static Dictionary<string, Type> mSideEffecMap = new Dictionary<string, Type>();

    static SideEffectManager()
    {
    }

    public static void AddSideEffectTypes<T>() where T : SideEffectBase
    {
        if (!mSideEffecMap.ContainsKey(typeof(T).ToString())) mSideEffecMap.Add(typeof(T).ToString(), typeof(T));
    }

    public static SideEffectBase GetNewSideEffect(string SideEffectName)
    {
        Type type = mSideEffecMap[SideEffectName];
        SideEffectBase newSE = (SideEffectBase) type.Assembly.CreateInstance(type.ToString());
        return newSE;
    }
}