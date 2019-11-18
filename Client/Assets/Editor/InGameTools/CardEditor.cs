using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

[CustomEditor(typeof(CardBase), true)]
public class CardBaseEditor : Editor
{
    [MenuItem("CardEditor/PrepareForCardEdit")]
    public static void PrepareForCardEdit()
    {
        ReloadConfigs();
        LoadSpriteAtlas_Editor();
    }

    [MenuItem("CardEditor/ReloadConfigs")]
    public static void ReloadConfigs()
    {
        Utils.DebugLog = Debug.Log;
        AllScriptExecuteSettings.CurrentAssembly = Assembly.GetAssembly(typeof(Battle));
        AllSideEffects.CurrentAssembly = Assembly.GetAssembly(typeof(Battle));
        AllBuffs.CurrentAssembly = Assembly.GetAssembly(typeof(Battle));
        LoadAllBasicXMLFiles.Load(Application.streamingAssetsPath + "/Config/");
        Debug.Log("Success Load Configs");
    }

    [MenuItem("CardEditor/LoadSpriteAtlas")]
    public static void LoadSpriteAtlas_Editor()
    {
        AtlasManager.Reset();
        SpriteAtlas[] sas = Resources.LoadAll<SpriteAtlas>("SpriteAtlas");
        foreach (SpriteAtlas sa in sas)
        {
            if (!AtlasManager.SpriteAtlasDict.ContainsKey(sa.name))
            {
                AtlasManager.SpriteAtlasDict.Add(sa.name.Split('.')[0], sa);
            }
        }

        BackGroundManager.BGs = new Sprite[AtlasManager.LoadAtlas("BGs").spriteCount];
        AtlasManager.LoadAtlas("BGs").GetSprites(BackGroundManager.BGs);

        Debug.Log("LoadSpriteAtlas_Editor Success");
    }

    protected CardBase CardBase;

    protected void OnEnable()
    {
        CardBase = target as CardBase;
    }

    int previewCardID = 0;
    string curLang = "zh";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        GUILayout.Label("Controls for Debug");
        int cardOrder = EditorGUILayout.IntField("CardOrder", CardBase.CardOrder);
        previewCardID = EditorGUILayout.IntField("PreviewCardID", previewCardID);
        if (GUI.changed)
        {
            CardBase.CardOrder = cardOrder;
            CardInfo_Base ci = AllCards.GetCard(previewCardID);
            if (ci != null) CardBase.Initiate(ci, CardBase.CardShowMode.ShowCard);
        }
    }
}

[CustomEditor(typeof(CardEquip))]
public class CardEquipEditor : CardBaseEditor
{
    [DrawGizmo(GizmoType.Selected)]
    void OnSceneGUI()
    {
        CardEquip cb = target as CardEquip;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(1, 0, 0, 1);
        style.fontSize = 15;
        style.fontStyle = FontStyle.Bold;

        if (cb.CardInfo != null) Handles.Label(cb.transform.position, "CIID: " + cb.M_CardInstanceId + "\n" + "CID: " + cb.CardInfo.CardID, style);
    }
}

[CustomEditor(typeof(CardMech))]
public class CardMechEditor : CardBaseEditor
{
    [DrawGizmo(GizmoType.Selected)]
    void OnSceneGUI()
    {
        CardMech cb = target as CardMech;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(1, 0, 0, 1);
        style.fontSize = 15;
        style.fontStyle = FontStyle.Bold;

        if (cb.CardInfo != null) Handles.Label(cb.transform.position, "CIID: " + cb.M_CardInstanceId + "\n" + "CID: " + cb.CardInfo.CardID, style);
    }
}

[CustomEditor(typeof(CardSpell))]
public class CardSpellEditor : CardBaseEditor
{
    [DrawGizmo(GizmoType.Selected)]
    void OnSceneGUI()
    {
        CardSpell cb = target as CardSpell;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(1, 0, 0, 1);
        style.fontSize = 15;
        style.fontStyle = FontStyle.Bold;

        if (cb.CardInfo != null) Handles.Label(cb.transform.position, "CIID: " + cb.M_CardInstanceId + "\n" + "CID: " + cb.CardInfo.CardID, style);
    }
}