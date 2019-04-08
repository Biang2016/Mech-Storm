using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardCoinComponent), true)]
class CardCoinComponentEditor : CardComponentBaseEditor
{
    private CardCoinComponent CardCoinComponent;

    protected override void OnEnable()
    {
        base.OnEnable();
        CardCoinComponent = target as CardCoinComponent;
    }

    private CardCoinComponent.Position pos;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        pos = (CardCoinComponent.Position) EditorGUILayout.EnumPopup("CardPanelPos", pos);
        if (GUI.changed)
        {
            CardCoinComponent.SetPosition(pos);
        }
    }
}