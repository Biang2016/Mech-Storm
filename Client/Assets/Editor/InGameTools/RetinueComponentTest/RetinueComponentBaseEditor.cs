using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RetinueComponentBase), true)]
class RetinueComponentBaseEditor : Editor
{
    protected RetinueComponentBase RetinueComponentBase;

    protected virtual void OnEnable()
    {
        RetinueComponentBase = target as RetinueComponentBase;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        GUILayout.Label("Controls for Debug");
        int cardOrder = EditorGUILayout.IntField("CardOrder", RetinueComponentBase.CardOrder);
        if (GUI.changed)
        {
            RetinueComponentBase.CardOrder = cardOrder;
        }
    }
}