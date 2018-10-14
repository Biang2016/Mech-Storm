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

    private static RandomNumberGenerator1 rm = null;

    [MenuItem("Tools/SysRandomTest")]
    static void DoItSys()
    {
        rm = new RandomNumberGenerator1(15);
        int[] bucket = new int[10];
        for (int i = 0; i < 10000; i++)
        {
            int ran = rm.Range(-5, 5);
            bucket[ran + 5] += 1;
        }

        for (int i = 0; i < 10; i++)
        {
            Debug.Log((i-5) + "," + bucket[i]);
        }

        //for (int i = 0; i < 2; i++)
        //{
        //    Debug.Log(rm.Range(0, i));
        //}
        //for (int i = 0; i < 1; i++)
        //{
        //    Debug.Log(rm.Range(0, i));
        //}
    }
}


public class RandomNumberGenerator1
{
    public RandomNumberGenerator1(int seed)
    {
        InitSeed = seed;
        Seed = seed;
    }

    private int UseTime = 0;
    private int InitSeed = 0;
    private int Seed = 0;

    public int GetInitSeed()
    {
        return InitSeed;
    }

    public int GetSeed()
    {
        return Seed;
    }

    int pre;

    int rand()
    {
        int ret = (Seed * 7361238 + Seed % 20037 * 1244 + pre * 12342 + 378211) * (Seed + 134543);
        pre = Seed;
        Seed = ret;
        return ret;
    }

    public int Range(int low, int high)
    {
        if (low > high) return 0;
        int len = high - low;
        return Mathf.Abs(rand()) % len + low;
    }
}