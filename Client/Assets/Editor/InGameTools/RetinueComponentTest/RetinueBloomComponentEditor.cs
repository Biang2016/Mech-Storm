using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RetinueBloomComponent), true)]
class RetinueBloomComponentEditor : RetinueComponentBaseEditor
{
    protected RetinueBloomComponent RetinueBloomComponent;

    protected override void OnEnable()
    {
        base.OnEnable();
        RetinueBloomComponent = target as RetinueBloomComponent;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RetinueBloomComponent.OnHoverBloomColor = EditorGUILayout.ColorField("OnHoverBloomColor", RetinueBloomComponent.OnHoverBloomColor);
        RetinueBloomComponent.OnHoverBloomColorIntensity = EditorGUILayout.FloatField("OnHoverBloomColorIntensity", RetinueBloomComponent.OnHoverBloomColorIntensity);
        if (GUI.changed)
        {
            RetinueBloomComponent.SetOnHoverBloomColor(RetinueBloomComponent.OnHoverBloomColor, RetinueBloomComponent.OnHoverBloomColorIntensity);
        }

        RetinueBloomComponent.CanAttackBloomColor = EditorGUILayout.ColorField("CanAttackBloomColor", RetinueBloomComponent.CanAttackBloomColor);
        RetinueBloomComponent.CanAttackBloomColorIntensity = EditorGUILayout.FloatField("CanAttackBloomColorIntensity", RetinueBloomComponent.CanAttackBloomColorIntensity);

        if (GUI.changed)
        {
            RetinueBloomComponent.SetCanAttackBloomColor(RetinueBloomComponent.CanAttackBloomColor, RetinueBloomComponent.CanAttackBloomColorIntensity);
        }

        RetinueBloomComponent.SideEffectTriggerBloomColor = EditorGUILayout.ColorField("SideEffectTriggerBloomColor", RetinueBloomComponent.SideEffectTriggerBloomColor);
        RetinueBloomComponent.SideEffectTriggerBloomColorIntensity = EditorGUILayout.FloatField("SideEffectTriggerBloomColorIntensity", RetinueBloomComponent.SideEffectTriggerBloomColorIntensity);

        if (GUI.changed)
        {
            RetinueBloomComponent.SetSideEffectTriggerBloomColor(RetinueBloomComponent.SideEffectTriggerBloomColor, RetinueBloomComponent.SideEffectTriggerBloomColorIntensity);
        }
    }
}