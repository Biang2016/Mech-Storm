using System.Collections.Generic;

public class OnlineManager : MonoSingleton<OnlineManager>
{
    private OnlineManager()
    {
    }

    internal SortedDictionary<int, BuildInfo> OnlineBuildInfos = new SortedDictionary<int, BuildInfo>();
    internal GamePlaySettings OnlineGamePlaySettings;
    internal int CurrentOnlineBuildID = -1;
}