public class LoadAllBasicXMLFiles
{
    public static void Load(string configFolderPath)
    {
        AllColors.AddAllColors();
        AllSideEffects.AddAllSideEffects();
        AllBuffs.AddAllBuffs();
        AllCards.AddAllCards();
        AllBuilds.AddAllBuilds();
        AllStories.AddAllStories();
        AllCards.RefreshAllCardXML();
    }
}