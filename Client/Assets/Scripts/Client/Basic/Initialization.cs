using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Initialization : MonoSingleton<Initialization>
{
    public bool IsABMode = false;
    [SerializeField] private ManagerTypes ManagerType;
    [SerializeField] private string StartUIPanel;
    [SerializeField] private string[] WarmUpUIPanels;

    private Initialization()
    {
    }

    void Awake()
    {
#if !UNITY_EDITOR
            IsABMode = true;
#endif

        if (IsABMode)
        {
            LoadShaders_AB();
            LoadSpriteAtlas_AB();
            LoadAudios_AB();
            LoadPrefabs_AB();
            LoadManager_AB();
        }
        else
        {
            LoadSpriteAtlas_Editor();
            LoadPrefabs_Editor();
            LoadManager_Editor();
        }

        Debug.Log("Start the client...");

        //Scene scene = SceneManager.GetActiveScene();
        //if (scene.name == "MainScene")
        //{
        //    List<PoolObject> POs = new List<PoolObject>();
        //    foreach (KeyValuePair<GameObjectPoolManager.PrefabNames, int> kv in GameObjectPoolManager.Instance.PoolWarmUpDict)
        //    {
        //        for (int i = 0; i < kv.Value; i++)
        //        {
        //            PoolObject PO = GameObjectPoolManager.Instance.PoolDict[kv.Key].AllocateGameObject<PoolObject>(transform);
        //            POs.Add(PO);
        //        }
        //    }

        //    foreach (PoolObject PO in POs)
        //    {
        //        PO.PoolRecycle();
        //    }
        //}
    }

    void Start()
    {
        foreach (string warmUpUiPanel in WarmUpUIPanels)
        {
            UIManager.Instance.ShowUIForm(warmUpUiPanel);
            UIManager.Instance.CloseUIForm(warmUpUiPanel);
        }

        UIManager.Instance.ShowUIForm(StartUIPanel);
    }

    enum ManagerTypes
    {
        MainManager,
        CardEditorManager,
        StoryEditorManager,
    }

    private const string ManagerResourcesPath = "Prefabs/Managers/";

    private void LoadManager_AB()
    {
        AssetBundle manager_bundle = ABManager.LoadAssetBundle("prefabs");
        GameObject manager = manager_bundle.LoadAsset<GameObject>("Assets/Resources/" + ManagerResourcesPath + ManagerType + ".prefab");
        Instantiate(manager);
        Debug.Log("LoadManager_AB");
    }

    private void LoadManager_Editor()
    {
        RootManager manager_go = Resources.Load<RootManager>(ManagerResourcesPath + ManagerType);
        Instantiate(manager_go);
    }

    private void LoadShaders_AB()
    {
        AssetBundle ab = ABManager.LoadAssetBundle("shaders");
        ab.LoadAllAssets();
        Shader.WarmupAllShaders();
        Debug.Log("LoadShaders_AB");
    }

    private void LoadSpriteAtlas_AB()
    {
        AtlasManager.Reset();
        List<AssetBundle> list = ABManager.LoadAllAssetBundleNamedLike("atlas_");
        foreach (AssetBundle assetBundle in list)
        {
            SpriteAtlas[] sas = assetBundle.LoadAllAssets<SpriteAtlas>();
            foreach (SpriteAtlas sp in sas)
            {
                if (!AtlasManager.SpriteAtlasDict.ContainsKey(sp.name))
                {
                    AtlasManager.SpriteAtlasDict.Add(sp.name, sp);
                }
            }
        }

        BackGroundManager.BGs = new Sprite[AtlasManager.LoadAtlas("BGs").spriteCount];
        AtlasManager.LoadAtlas("BGs").GetSprites(BackGroundManager.BGs);

        Debug.Log("LoadSpriteAtlas_AB");
    }

    private void LoadSpriteAtlas_Editor()
    {
        AtlasManager.Reset();
        SpriteAtlas[] sas = Resources.LoadAll<SpriteAtlas>("SpriteAtlas");
        foreach (SpriteAtlas sa in sas)
        {
            if (!AtlasManager.SpriteAtlasDict.ContainsKey(sa.name))
            {
                AtlasManager.SpriteAtlasDict.Add(sa.name.Split('.')[0], sa);
            }
        }

        BackGroundManager.BGs = new Sprite[AtlasManager.LoadAtlas("BGs").spriteCount];
        AtlasManager.LoadAtlas("BGs").GetSprites(BackGroundManager.BGs);

        Debug.Log("LoadSpriteAtlas_Editor");
    }

    private void LoadAudios_AB()
    {
        AudioManager.ClearAudioClipDict();
        List<AssetBundle> list = ABManager.LoadAllAssetBundleNamedLike("audio_");
        foreach (AssetBundle assetBundle in list)
        {
            string prefix = "";
            if (assetBundle.name.StartsWith("audio_sfx"))
            {
                prefix = "sfx/";
            }
            else if (assetBundle.name.StartsWith("audio_bgm"))
            {
                prefix = "bgm/";
            }

            AudioClip[] audioClips = assetBundle.LoadAllAssets<AudioClip>();
            foreach (AudioClip audioClip in audioClips)
            {
                AudioManager.AddAudioRes(prefix + audioClip.name, audioClip);
            }
        }

        Debug.Log("LoadAudios_AB");
    }

    private void LoadPrefabs_Editor()
    {
        PrefabManager.ClearPrefabDict();
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/");
        foreach (GameObject prefab in prefabs)
        {
            PrefabManager.AddPrefabRes(prefab.name, prefab);
        }

        Debug.Log("LoadPrefabs_Editor");
    }

    private void LoadPrefabs_AB()
    {
        PrefabManager.ClearPrefabDict();
        AssetBundle assetBundle = ABManager.LoadAssetBundle("prefabs");

        GameObject[] prefabs = assetBundle.LoadAllAssets<GameObject>();
        foreach (GameObject prefab in prefabs)
        {
            PrefabManager.AddPrefabRes(prefab.name, prefab);
        }

        Debug.Log("LoadPrefabs_AB");
    }
}