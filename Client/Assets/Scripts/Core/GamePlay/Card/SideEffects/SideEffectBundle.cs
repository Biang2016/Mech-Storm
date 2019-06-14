using System.Collections.Generic;
using System.Text;

/// <summary>
/// a bundle of SideEffectExecute to attach onto cards, mechs, equipment, battleships
/// because some cards usually have more than one side effects.
/// </summary>
public class SideEffectBundle : IClone<SideEffectBundle>
{
    public List<SideEffectExecute> SideEffectExecutes = new List<SideEffectExecute>();

    /// <summary>
    /// SideEffectExecute中定义了各种各样的触发条件，此字典将SideEffectExecute按类整理，以便EventManager执行触发事件
    /// </summary>
    public SortedDictionary<SideEffectExecute.TriggerTime, Dictionary<SideEffectExecute.TriggerRange, List<SideEffectExecute>>> SideEffectExecutes_Dict = new SortedDictionary<SideEffectExecute.TriggerTime, Dictionary<SideEffectExecute.TriggerRange, List<SideEffectExecute>>>();

    public void AddSideEffectExecute(SideEffectExecute see)
    {
        SideEffectExecutes.Add(see);
        AddToSEEDict(see);
    }

    public bool RemoveSideEffectExecute(SideEffectExecute see)
    {
        bool suc1 = SideEffectExecutes.Remove(see);
        bool suc2 = RemoveFromSEEDict(see);
        return suc1 | suc2;
    }

    private void AddToSEEDict(SideEffectExecute see)
    {
        if (!SideEffectExecutes_Dict.ContainsKey(see.M_ExecuteSetting.TriggerTime)) SideEffectExecutes_Dict.Add(see.M_ExecuteSetting.TriggerTime, new Dictionary<SideEffectExecute.TriggerRange, List<SideEffectExecute>>());
        Dictionary<SideEffectExecute.TriggerRange, List<SideEffectExecute>> des = SideEffectExecutes_Dict[see.M_ExecuteSetting.TriggerTime];
        if (!des.ContainsKey(see.M_ExecuteSetting.TriggerRange)) des.Add(see.M_ExecuteSetting.TriggerRange, new List<SideEffectExecute>());
        List<SideEffectExecute> sees = SideEffectExecutes_Dict[see.M_ExecuteSetting.TriggerTime][see.M_ExecuteSetting.TriggerRange];
        sees.Add(see);
    }

    private bool RemoveFromSEEDict(SideEffectExecute see)
    {
        bool suc_1 = false;
        if (SideEffectExecutes_Dict.ContainsKey(see.M_ExecuteSetting.TriggerTime))
        {
            Dictionary<SideEffectExecute.TriggerRange, List<SideEffectExecute>> des = SideEffectExecutes_Dict[see.M_ExecuteSetting.TriggerTime];
            if (des.ContainsKey(see.M_ExecuteSetting.TriggerRange))
            {
                List<SideEffectExecute> sees = SideEffectExecutes_Dict[see.M_ExecuteSetting.TriggerTime][see.M_ExecuteSetting.TriggerRange];
                suc_1 = sees.Remove(see);
            }
        }

        bool suc_2 = false;
        if (!suc_1)
        {
            List<SideEffectExecute> src = null;
            foreach (KeyValuePair<SideEffectExecute.TriggerTime, Dictionary<SideEffectExecute.TriggerRange, List<SideEffectExecute>>> kv in SideEffectExecutes_Dict)
            {
                foreach (KeyValuePair<SideEffectExecute.TriggerRange, List<SideEffectExecute>> _kv in kv.Value)
                {
                    if (_kv.Value.Contains(see))
                    {
                        src = _kv.Value;
                    }
                }
            }

            if (src != null)
            {
                suc_2 = src.Remove(see);
            }
        }

        return suc_1 | suc_2;
    }

    public void RefreshSideEffectExecutesDict()
    {
        SideEffectExecutes_Dict.Clear();
        foreach (SideEffectExecute see in SideEffectExecutes)
        {
            AddToSEEDict(see);
        }
    }

    public List<SideEffectExecute> GetSideEffectExecutes(SideEffectExecute.TriggerTime triggerTime, SideEffectExecute.TriggerRange triggerRange)
    {
        List<SideEffectExecute> res = new List<SideEffectExecute>();
        if (SideEffectExecutes_Dict.ContainsKey(triggerTime))
        {
            Dictionary<SideEffectExecute.TriggerRange, List<SideEffectExecute>> temp = SideEffectExecutes_Dict[triggerTime];
            if (temp.ContainsKey(triggerRange))
            {
                res.AddRange(temp[triggerRange]);
            }
        }

        return res;
    }

    /// <summary>
    /// To generate SideEffects' description automatically for cards.
    /// It can generate all side effects' description at one time with grammar correct.
    /// </summary>
    /// <returns></returns>
    public string GetSideEffectsDesc()
    {
        string res = "";
        foreach (KeyValuePair<SideEffectExecute.TriggerTime, Dictionary<SideEffectExecute.TriggerRange, List<SideEffectExecute>>> kv in SideEffectExecutes_Dict)
        {
            foreach (KeyValuePair<SideEffectExecute.TriggerRange, List<SideEffectExecute>> SEEs in kv.Value)
            {
                foreach (SideEffectExecute see in SEEs.Value)
                {
                    res += see.GenerateDesc() + "; ";
                }
            }
        }

        res = FirstLetterOfSentenceUpper(res);

        return res;
    }

    private string FirstLetterOfSentenceUpper(string src)
    {
        StringBuilder sb = new StringBuilder();
        bool dot = true;
        bool colorStart = false;
        src = src.Replace("</color>", "@");
        for (int i = 0; i < src.Length; i++)
        {
            char c = src[i];
            if (c == '<' && i < src.Length - 1 && src[i + 1] == '#' && !colorStart)
            {
                sb.Append(c);
                colorStart = true;
            }
            else if (c == '>' && colorStart)
            {
                sb.Append(c);
                colorStart = false;
            }
            else if (!colorStart)
            {
                if (c == '.' || c == ':' || c == ';' || c == '[')
                {
                    dot = true;
                    sb.Append(c);
                }
                else if (dot && (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                {
                    sb.Append(c.ToString().ToUpper());
                    dot = false;
                }
                else
                {
                    sb.Append(c);
                }
            }
            else
            {
                sb.Append(c);
            }
        }

        string res = sb.ToString();
        res = res.Replace("@", "</color>");
        return res;
    }

    public SideEffectBundle Clone()
    {
        SideEffectBundle copy = new SideEffectBundle();

        foreach (SideEffectExecute see in SideEffectExecutes)
        {
            copy.AddSideEffectExecute(see.Clone());
        }

        return copy;
    }

    public void Serialize(DataStream writer)
    {
        writer.WriteSInt32(SideEffectExecutes.Count);
        foreach (SideEffectExecute see in SideEffectExecutes)
        {
            see.Serialize(writer);
        }
    }

    public static SideEffectBundle Deserialize(DataStream reader)
    {
        SideEffectBundle res = new SideEffectBundle();
        int SideEffectCount = reader.ReadSInt32();
        for (int i = 0; i < SideEffectCount; i++)
        {
            SideEffectExecute see = SideEffectExecute.Deserialize(reader);
            res.AddSideEffectExecute(see);
        }

        return res;
    }
}