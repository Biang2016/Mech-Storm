using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardComponentBase), true)]
class CardComponentBaseEditor : Editor
{
    protected CardComponentBase CardComponentBase;

    protected virtual void OnEnable()
    {
        CardComponentBase = target as CardComponentBase;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        GUILayout.Label("Controls for Debug");
        int cardOrder = EditorGUILayout.IntField("CardOrder", CardComponentBase.CardOrder);
        if (GUI.changed)
        {
            CardComponentBase.CardOrder = cardOrder;
        }
    }
}