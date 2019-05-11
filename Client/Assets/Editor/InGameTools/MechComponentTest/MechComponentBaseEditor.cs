using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MechComponentBase), true)]
class MechComponentBaseEditor : Editor
{
    protected MechComponentBase MechComponentBase;

    protected virtual void OnEnable()
    {
        MechComponentBase = target as MechComponentBase;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        GUILayout.Label("Controls for Debug");
        int cardOrder = EditorGUILayout.IntField("CardOrder", MechComponentBase.CardOrder);
        if (GUI.changed)
        {
            MechComponentBase.CardOrder = cardOrder;
        }
    }
}