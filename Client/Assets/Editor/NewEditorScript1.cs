using UnityEngine;
using UnityEditor;

public class NewEditorScript1 : ScriptableObject
{
    [MenuItem("Tools/RandomTest")]
    static void DoIt()
    {
        Random.InitState(255);

        PrintRandom("Step 1");
        PrintRandom("Step 2");

        Random.State oldState = Random.state;

        PrintRandom("Step 3");
        PrintRandom("Step 4");

        Random.state = oldState;

        PrintRandom("Step 5");
        PrintRandom("Step 6");

        Random.InitState(255);

        PrintRandom("Step 7");
        PrintRandom("Step 8");
    }

    static void PrintRandom(string label)
    {
        Debug.Log(string.Format("{0} - RandomValue {1}", label, Random.Range(1, 100)));
        Debug.Log(Random.state.ToString());
    }

    private static System.Random rm = null;

    [MenuItem("Tools/SysRandomTest")]
    static void DoItSys()
    {
        rm = new System.Random(255);

        PrintSysRandom("Step 1");
        PrintSysRandom("Step 2");

        rm = new System.Random(100);

        PrintSysRandom("Step 3");
        PrintSysRandom("Step 4");

        rm = new System.Random(255);

        PrintSysRandom("Step 5");
        PrintSysRandom("Step 6");

        rm = new System.Random(100);

        PrintSysRandom("Step 7");
        PrintSysRandom("Step 8");
    }

    static void PrintSysRandom(string label)
    {
        Debug.Log(string.Format("{0} - RandomValue {1}", label, rm.Next(0, 100)));
    }
}