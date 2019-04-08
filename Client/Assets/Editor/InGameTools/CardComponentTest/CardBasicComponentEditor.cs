using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardBasicComponent), true)]
class CardBasicComponentEditor : CardComponentBaseEditor
{
    protected CardBasicComponent CardBasicComponent;

    protected override void OnEnable()
    {
        base.OnEnable();
        CardBasicComponent = target as CardBasicComponent;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CardBasicComponent.MainBoardColor = EditorGUILayout.ColorField("MainBoardColor", CardBasicComponent.MainBoardColor);
        CardBasicComponent.MainBoardColorIntensity = EditorGUILayout.FloatField("MainBoardColorIntensity", CardBasicComponent.MainBoardColorIntensity);
        if (GUI.changed)
        {
            CardBasicComponent.SetMainBoardColor(CardBasicComponent.MainBoardColor, CardBasicComponent.MainBoardColorIntensity);
        }

        CardBasicComponent.CardBloomColor = EditorGUILayout.ColorField("CardBloomColor", CardBasicComponent.CardBloomColor);
        CardBasicComponent.CardBloomColorIntensity = EditorGUILayout.FloatField("CardBloomColorIntensity", CardBasicComponent.CardBloomColorIntensity);
        if (GUI.changed)
        {
            CardBasicComponent.SetCardBloomColor(CardBasicComponent.CardBloomColor, CardBasicComponent.CardBloomColorIntensity);
        }

        CardBasicComponent.PictureColor = EditorGUILayout.ColorField("PictureColor", CardBasicComponent.PictureColor);
        CardBasicComponent.PictureColorIntensity = EditorGUILayout.FloatField("PictureColorIntensity", CardBasicComponent.PictureColorIntensity);
        if (GUI.changed)
        {
            CardBasicComponent.SetPictureColor(CardBasicComponent.PictureColor, CardBasicComponent.PictureColorIntensity);
        }
    }
}