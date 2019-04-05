public class StoryManager : MonoSingleton<StoryManager>
{
    private StoryManager()
    {
    }

    private Story Story;

    public void InitializeStory(Story story)
    {
        if (story == null)
        {
            //TODO ResetAll
            UnlockedCardLevelNum = 0;
        }
        Story = story;
    }

    public Story GetStory()
    {
        return Story;
    }

    public void SetStoryPaceBeated(int storyPaceID)
    {

    }
    public void SetStoryPaceFailed(int storyPaceID)
    {

    }

    internal int UnlockedCardLevelNum = 0;

    internal bool HasStory => Story != null;
    internal bool IsThisLevelNumberUp =false;
}