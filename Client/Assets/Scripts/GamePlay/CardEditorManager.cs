using UnityEngine;

public class CardEditorManager : MonoSingleton<CardEditorManager>
{
    private CardEditorManager()
    {
    }

    public bool ShowClientLogs = true;

    private void Awake()
    {
        InitializeClientGameSettings();
        Utils.DebugLog = ClientLog.Instance.PrintError;
        AllColors.AddAllColors(Application.streamingAssetsPath + "/Config/Colors.xml");
        AllSideEffects.AddAllSideEffects(Application.streamingAssetsPath + "/Config/SideEffects.xml");
        AllBuffs.AddAllBuffs(Application.streamingAssetsPath + "/Config/Buffs.xml");
        AllCards.AddAllCards(Application.streamingAssetsPath + "/Config/Cards.xml");
    }

    public Camera BattleGroundCamera;

    private void InitializeClientGameSettings()
    {
    }
}