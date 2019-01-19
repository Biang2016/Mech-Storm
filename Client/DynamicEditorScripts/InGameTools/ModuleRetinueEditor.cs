using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModuleRetinue))]
public class ModuleRetinueEditor : Editor
{
    [DrawGizmo(GizmoType.Selected)]
    void OnSceneGUI()
    {
        ModuleRetinue mr = target as ModuleRetinue;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(1, 0, 0, 1);
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;

        GUIStyle styleDisabled = new GUIStyle(style);
        styleDisabled.normal.textColor = new Color(1, 1, 1, 1);

        Handles.Label(mr.transform.position + Vector3.right * 1.5f,
            "RetID: " + mr.M_RetinueID + "\n"
            + "TmpRetID: " + mr.M_ClientTempRetinueID + "\n"
            + "Attack: " + mr.M_RetinueAttack + "\n"
            + "Energy: " + mr.M_RetinueWeaponEnergy + "\n"
            + "FinalAttack: " + mr.M_RetinueWeaponFinalAttack + "\n"
            , style);
    }
}