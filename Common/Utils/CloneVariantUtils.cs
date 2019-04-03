using System.Collections.Generic;

public static class CloneVariantUtils
{
    public enum OperationType
    {
        Clone,
        Variant,
        None,
    }

    private static T GetOperationResult<T>(T src, OperationType operationType = OperationType.Clone)
    {
        T res_t = src;
        switch (operationType)
        {
            case OperationType.Clone:
            {
                if (src is IClone<T> t_Clone)
                {
                    res_t = t_Clone.Clone();
                }

                break;
            }
            case OperationType.Variant:
            {
                if (src is IVariant<T> t_Variant)
                {
                    res_t = t_Variant.Variant();
                }

                break;
            }
            case OperationType.None:
            {
                break;
            }
        }

        return res_t;
    }

    public static HashSet<T> HashSet<T>(HashSet<T> src, OperationType operationType = OperationType.Clone)
    {
        HashSet<T> res = new HashSet<T>();
        if (src == null) return res;
        foreach (T t in src)
        {
            res.Add(GetOperationResult(t, operationType));
        }

        return res;
    }

    public static List<T> List<T>(List<T> src, OperationType operationType = OperationType.Clone)
    {
        List<T> res = new List<T>();
        if (src == null) return res;
        foreach (T t in src)
        {
            res.Add(GetOperationResult(t, operationType));
        }

        return res;
    }

    public static Dictionary<T1, T2> Dictionary<T1, T2>(Dictionary<T1, T2> src, OperationType operationType = OperationType.Clone)
    {
        Dictionary<T1, T2> res = new Dictionary<T1, T2>();
        if (src == null) return res;
        foreach (KeyValuePair<T1, T2> kv in src)
        {
            res.Add(GetOperationResult(kv.Key, operationType), GetOperationResult(kv.Value, operationType));
        }

        return res;
    }

    public static SortedDictionary<T1, T2> SortedDictionary<T1, T2>(SortedDictionary<T1, T2> src, OperationType operationType = OperationType.Clone)
    {
        SortedDictionary<T1, T2> res = new SortedDictionary<T1, T2>();
        if (src == null) return res;
        foreach (KeyValuePair<T1, T2> kv in src)
        {
            res.Add(GetOperationResult(kv.Key, operationType), GetOperationResult(kv.Value, operationType));
        }

        return res;
    }
}