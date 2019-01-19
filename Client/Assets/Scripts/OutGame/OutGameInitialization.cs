using Boo.Lang;
using UnityEngine;
using UnityEngine.U2D;

public class OutGameInitialization : MonoSingleton<OutGameInitialization>
{
    public bool IsABMode = false;

    private OutGameInitialization()
    {
    }

    void Awake()
    {
#if !UNITY_EDITOR
            IsABMode = true;
#endif

        if (IsABMode)
        {
        }
        else
        {
        }
    }
}