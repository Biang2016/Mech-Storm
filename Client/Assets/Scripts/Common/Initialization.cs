using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Initialization : MonoSingleton<Initialization>
{
    public bool IsABMode = false;
    [SerializeField] private GameObject Manager;
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
        foreach (string warmUpUiPanel in WarmUpUIPanels)
        {
            UIManager.Instance.ShowUIForm(warmUpUiPanel);
            UIManager.Instance.CloseUIForm(warmUpUiPanel);
        }

        UIManager.Instance.ShowUIForm(StartUIPanel);
    }

    private void LoadManager_AB()
    {
        AssetBundle manager_bundle = ABManager.LoadAssetBundle("prefabs");
        GameObject manager = manager_bundle.LoadAsset<GameObject>("Assets/Prefabs/Manager.prefab");
        Instantiate(manager);
        Debug.Log("LoadManager_AB");
    }

    private void LoadManager_Editor()
    {
        Instantiate(Manager);
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