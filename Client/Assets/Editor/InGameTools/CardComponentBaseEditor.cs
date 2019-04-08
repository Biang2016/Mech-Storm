using UnityEditor;

[CustomEditor(typeof(CardComponentBase), true)]
class CardComponentBaseEditor : Editor
{
    private CardComponentBase cardComponentBase;

    void OnEnable()
    {
        cardComponentBase = target as CardComponentBase;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        cardComponentBase.CardOrder = EditorGUILayout.IntField("CardOrder", cardComponentBase.CardOrder);
    }
}