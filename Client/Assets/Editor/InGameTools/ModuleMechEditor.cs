using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModuleMech))]
public class ModuleMechEditor : Editor
{
    [DrawGizmo(GizmoType.Selected)]
    void OnSceneGUI()
    {
        ModuleMech mr = target as ModuleMech;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(1, 0, 0, 1);
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;

        GUIStyle styleDisabled = new GUIStyle(style);
        styleDisabled.normal.textColor = new Color(1, 1, 1, 1);

        Handles.Label(mr.transform.position + Vector3.right * 1.5f,
            "RetID: " + mr.M_MechID + "\n"
            + "TmpRetID: " + mr.M_ClientTempMechID + "\n"
            + "Attack: " + mr.M_MechAttack + "\n"
            + "Energy: " + mr.M_MechWeaponEnergy + "\n"
            + "FinalAttack: " + mr.M_MechWeaponFinalAttack + "\n"
            , style);
    }
}