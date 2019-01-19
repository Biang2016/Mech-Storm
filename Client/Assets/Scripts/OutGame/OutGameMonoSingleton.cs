using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(T)) as T;
                if (instance == null)
                {
                    if (ClientLog.instance != null)
                    {
                        ClientLog.Instance.PrintError("找不到" + typeof(T).ToString());
                    }
                }
            }

            return instance;
        }
        set { instance = value; }
    }
}