using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardEquip))]
public class CardEquipEditor : Editor
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
public class CardRetinueEditor : Editor
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
public class CardSpellEditor : Editor
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

[CustomEditor(typeof(SelectBuildManager))]
public class SelectBuildManagerEditor : Editor
{
    [DrawGizmo(GizmoType.Selected)]
    void OnSceneGUI()
    {
        SelectBuildManager sbm = target as SelectBuildManager;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(1, 0, 0, 1);
        style.fontSize = 15;
        style.fontStyle = FontStyle.Bold;

        foreach (KeyValuePair<int, CardBase> kv in sbm.allCards)
        {
            if (kv.Value.CardInfo != null) Handles.Label(kv.Value.transform.position, "CIID: " + kv.Value.M_CardInstanceId + "\n" + "CID: " + kv.Value.CardInfo.CardID, style);
        }
    }
}

[CustomEditor(typeof(HandManager))]
public class HandManagerEditor : Editor
{
    [DrawGizmo(GizmoType.Selected)]
    void OnSceneGUI()
    {
        HandManager hm = target as HandManager;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(1, 0, 0, 1);
        style.fontSize = 15;
        style.fontStyle = FontStyle.Bold;

        foreach (CardBase cb in hm.cards)
        {
            if (cb.CardInfo != null) Handles.Label(cb.transform.position, "CIID: " + cb.M_CardInstanceId + "\n" + "CID: " + cb.CardInfo.CardID, style);
        }
    }
}