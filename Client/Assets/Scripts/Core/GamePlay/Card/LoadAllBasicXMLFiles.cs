public class LoadAllBasicXMLFiles
{
    public static string ConfigFolderPath;

    public static void Load(string configFolderPath)
    {
        ConfigFolderPath = configFolderPath;
        AllColors.AddAllColors();
        AllSideEffects.AddAllSideEffects();
        AllScriptExecuteSettings.AddAllScriptExecuteSettings();
        AllBuffs.AddAllBuffs();
        AllCards.AddAllCards();
        AllBuilds.AddAllBuilds();
        AllLevels.AddAllLevels();
        AllStories.AddAllStories();
        AllCards.RefreshAllCardXML();
    }
}