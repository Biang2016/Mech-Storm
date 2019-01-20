public class OutGameInitialization : OutGameMonoSingleton<OutGameInitialization>
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