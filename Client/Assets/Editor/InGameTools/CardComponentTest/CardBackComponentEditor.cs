using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardBackComponent), true)]
class CardBackComponentEditor : CardComponentBaseEditor
{
    protected CardBackComponent CardBackComponent;

    protected override void OnEnable()
    {
        base.OnEnable();
        CardBackComponent = target as CardBackComponent;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CardBackComponent.CardBackColor = EditorGUILayout.ColorField("MainBoardColor", CardBackComponent.CardBackColor);
        CardBackComponent.CardBackColorIntensity = EditorGUILayout.FloatField("MainBoardColorIntensity", CardBackComponent.CardBackColorIntensity);
        if (GUI.changed)
        {
            CardBackComponent.SetCardBackColor(CardBackComponent.CardBackColor, CardBackComponent.CardBackColorIntensity);
        }

        CardBackComponent.CardBackBloomColor = EditorGUILayout.ColorField("CardBloomColor", CardBackComponent.CardBackBloomColor);
        CardBackComponent.CardBackBloomColorIntensity = EditorGUILayout.FloatField("CardBloomColorIntensity", CardBackComponent.CardBackBloomColorIntensity);
        if (GUI.changed)
        {
            CardBackComponent.SetCardBloomColor(CardBackComponent.CardBackBloomColor, CardBackComponent.CardBackBloomColorIntensity);
        }
    }
}