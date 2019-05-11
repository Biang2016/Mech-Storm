using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MechBloomComponent), true)]
class MechBloomComponentEditor : MechComponentBaseEditor
{
    protected MechBloomComponent MechBloomComponent;

    protected override void OnEnable()
    {
        base.OnEnable();
        MechBloomComponent = target as MechBloomComponent;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MechBloomComponent.OnHoverBloomColor = EditorGUILayout.ColorField("OnHoverBloomColor", MechBloomComponent.OnHoverBloomColor);
        MechBloomComponent.OnHoverBloomColorIntensity = EditorGUILayout.FloatField("OnHoverBloomColorIntensity", MechBloomComponent.OnHoverBloomColorIntensity);
        if (GUI.changed)
        {
            MechBloomComponent.SetOnHoverBloomColor(MechBloomComponent.OnHoverBloomColor, MechBloomComponent.OnHoverBloomColorIntensity);
        }

        MechBloomComponent.CanAttackBloomColor = EditorGUILayout.ColorField("CanAttackBloomColor", MechBloomComponent.CanAttackBloomColor);
        MechBloomComponent.CanAttackBloomColorIntensity = EditorGUILayout.FloatField("CanAttackBloomColorIntensity", MechBloomComponent.CanAttackBloomColorIntensity);

        if (GUI.changed)
        {
            MechBloomComponent.SetCanAttackBloomColor(MechBloomComponent.CanAttackBloomColor, MechBloomComponent.CanAttackBloomColorIntensity);
        }

        MechBloomComponent.SideEffectTriggerBloomColor = EditorGUILayout.ColorField("SideEffectTriggerBloomColor", MechBloomComponent.SideEffectTriggerBloomColor);
        MechBloomComponent.SideEffectTriggerBloomColorIntensity = EditorGUILayout.FloatField("SideEffectTriggerBloomColorIntensity", MechBloomComponent.SideEffectTriggerBloomColorIntensity);

        if (GUI.changed)
        {
            MechBloomComponent.SetSideEffectTriggerBloomColor(MechBloomComponent.SideEffectTriggerBloomColor, MechBloomComponent.SideEffectTriggerBloomColorIntensity);
        }
    }
}