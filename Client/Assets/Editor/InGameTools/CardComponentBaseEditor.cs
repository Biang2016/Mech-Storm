using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardBasicComponent))]
class CardComponentBaseEditor : Editor
{
    private CardBasicComponent cardComponentBase;

    void OnEnable()
    {
        cardComponentBase = target as CardBasicComponent;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        cardComponentBase.CardOrder = EditorGUILayout.IntField("CardOrder", cardComponentBase.CardOrder);
    }
}