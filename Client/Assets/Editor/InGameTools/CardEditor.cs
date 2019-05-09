using System.Collections.Generic;
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
        AllColors.AddAllColors(Application.streamingAssetsPath + "/Config/Colors.xml");
        AllSideEffects.CurrentAssembly = Assembly.GetAssembly(typeof(CardBase));
        AllSideEffects.AddAllSideEffects(Application.streamingAssetsPath + "/Config/SideEffects.xml");
        AllBuffs.CurrentAssembly = Assembly.GetAssembly(typeof(CardBase));
        AllBuffs.AddAllBuffs(Application.streamingAssetsPath + "/Config/Buffs.xml");
        AllCards.AddAllCards(Application.streamingAssetsPath + "/Config/Cards.xml");
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

    protected virtual void OnEnable()
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

[CustomEditor(typeof(CardRetinue))]
public class CardRetinueEditor : CardBaseEditor
{
    [DrawGizmo(GizmoType.Selected)]
    void OnSceneGUI()
    {
        CardRetinue cb = target as CardRetinue;
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