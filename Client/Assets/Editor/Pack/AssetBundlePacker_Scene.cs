using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AssetBundlePacker_Scene  {

    static Dictionary<string, bool> asset_black_mapper = new Dictionary<string, bool>();
    static Dictionary<string, string> asset_dir_pack_mapper = new Dictionary<string, string>();

    // 初始化Packer
    [MenuItem("AssetBundle/Packer/InitPacker_Scene")]
    public static void InitPacker()
    {
        asset_black_mapper.Clear();

        // 编辑器地图数据不打包
        SafeInsertBlackMapper("Assets/Resources/abfiledic.bytes");

        // 字典不在这里
        SafeInsertBlackMapper("Assets/Resources/MapEditionData.bytes");

        // Shader不参与打包
        SafeInsertBlackMapper("Assets/Resources/Shaders");

        // UI不参与打包
        SafeInsertBlackMapper("Assets/Resources/UIPanel");
        SafeInsertBlackMapper("Assets/Resources/UI_FX");
        SafeInsertBlackMapper("Assets/Resources/UI_ani");

        // 不参与构建的Scene不打包
        string[] use_scenes = BuildTool.GetBuildScenes();
        DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath + "/Scenes");
        FileInfo[] all_scenes = dirInfo.GetFiles("*.unity", SearchOption.AllDirectories);
        foreach (FileInfo scene in all_scenes)
        {
            string scene_name = PackerToolFunc.GetAssetsRelativePath(PackerToolFunc.NormalizePath(scene.FullName));

            bool use_scene = false;
            for (int i = 0; i < use_scenes.Length && !use_scene; ++i)
            {
                use_scene |= (0 <= scene_name.IndexOf(use_scenes[i]));
            }
            if (!use_scene)
            {
                SafeInsertBlackMapper(scene_name);
            }
        }

        // OutGame的Scene不打包
        SafeInsertBlackMapper("Assets/Scenes/FirstScene");
        SafeInsertBlackMapper("Assets/Scenes/LoginScene");

        // OutGame不参与打包
        SafeInsertBlackMapper("Assets/Resources/OutGame");

        // 已Pack的资源不参与打包
        SafeInsertBlackMapper("Assets/Resources/scene_resource/world/w_terrian/bank");
        SafeInsertBlackMapper("Assets/Resources/scene_resource/world/w_terrian/mask");
        SafeInsertBlackMapper("Assets/Resources/scene_resource/world/w_terrian/w_deco");

        // 按目录打包
        asset_dir_pack_mapper.Clear();
        SafeInsertPackMapper("Assets/Resources/AmplifyColor", "package_AmplifyColor".ToLower());
        
        //SafeInsertPackMapper("Assets/Resources/ani/city", "ani/package_city".ToLower());
        CreateL1DirPackCfg("Assets/Resources/ani/city", "ani/package_city".ToLower());
        CreateL2DirPackCfg("Assets/Resources/ani/city", "ani/city".ToLower());

        CreateL1DirPackCfg("Assets/Resources/ani/pc3", "ani/package_pc3".ToLower());
        CreateL2DirPackCfg("Assets/Resources/ani/pc3", "ani/pc3".ToLower());
        //SafeInsertDirPackMapper("Assets/Resources/ani/pc3", "ani/package_pc3".ToLower());

        SafeInsertPackMapper("Assets/Resources/battle", "package_battle".ToLower());
        SafeInsertPackMapper("Assets/Resources/CameraBlur", "package_CameraBlur".ToLower());

        // Assets/Resources/City
        //SafeInsertPackMapper("Assets/Resources/City", "package_City".ToLower());
        CreateDirTexPackCfg("Assets/Resources/City", "City".ToLower());
        CreateDirMatPackCfg("Assets/Resources/City", "City".ToLower());
        CreateDirPrefabPackCfg("Assets/Resources/City", "City".ToLower());

        // Assets/Resources/FX
        CreateDirTexPackCfg("Assets/Resources/FX", "FX".ToLower());
        CreateDirMatPackCfg("Assets/Resources/FX", "FX".ToLower());
        CreateDirPrefabPackCfg("Assets/Resources/FX", "FX".ToLower());
        //SafeInsertPackMapper("Assets/Resources/FX", "package_FX".ToLower());

        SafeInsertPackMapper("Assets/Resources/Guide", "package_Guide".ToLower());

        // Assets/Resources/heros
        CreateL1DirPackCfg("Assets/Resources/heros", "package_heros".ToLower());
        CreateL2DirPackCfg("Assets/Resources/heros", "heros".ToLower());
        //SafeInsertDirPackMapper("Assets/Resources/heros", "package_heros".ToLower());

        SafeInsertPackMapper("Assets/Resources/Material", "package_Material".ToLower());
        SafeInsertPackMapper("Assets/Resources/Prefab", "package_Prefab".ToLower());
        SafeInsertPackMapper("Assets/Resources/Res", "package_Res".ToLower());
        SafeInsertPackMapper("Assets/Resources/SCEN", "package_SCEN".ToLower());

        // Assets/Resources/scene_resource/battle
        //SafeInsertPackMapper("Assets/Resources/scene_resource/battle", "scene_resource/package_battle".ToLower());
        CreateDirTexPackCfg("Assets/Resources/scene_resource/battle", "scene_resource/battle".ToLower());
        CreateDirMatPackCfg("Assets/Resources/scene_resource/battle", "scene_resource/battle".ToLower());
        CreateDirPrefabPackCfg("Assets/Resources/scene_resource/battle", "scene_resource/battle".ToLower());

        SafeInsertPackMapper("Assets/Resources/scene_resource/openingscene", "scene_resource/package_openingscene".ToLower());

        // Assets/Resources/scene_resource/world
        //SafeInsertPackMapper("Assets/Resources/scene_resource/world", "scene_resource/package_world".ToLower());
        CreateDirTexPackCfg("Assets/Resources/scene_resource/world", "scene_resource/world".ToLower());
        CreateDirMatPackCfg("Assets/Resources/scene_resource/world", "scene_resource/world".ToLower());
        CreateDirPrefabPackCfg("Assets/Resources/scene_resource/world", "scene_resource/world".ToLower());

        // Assets/Resources/scene_resource/scenario
        //SafeInsertPackMapper("Assets/Resources/scene_resource/scenario", "scene_resource/package_scenario".ToLower());
        CreateL1DirPackCfg("Assets/Resources/scene_resource/scenario", "scene_resource/package_scenario".ToLower());
        CreateL2DirPackCfg("Assets/Resources/scene_resource/scenario", "scene_resource/scenario".ToLower());

        SafeInsertPackMapper("Assets/Resources/TonemappingColorGrading", "package_TonemappingColorGrading".ToLower());
        SafeInsertPackMapper("Assets/Resources/UIImage", "package_uiimage".ToLower());
        SafeInsertPackMapper("Assets/Resources/WorldMap", "package_worldmap".ToLower());

        // 非IOS则IOSCheck目录不参与打包
    #if UNITY_IOS
        SafeInsertPackMapper("Assets/Resources/iOSCheck", "package_ioscheck".ToLower());
    #endif
    }

    // 打包Scene特有的资源
    [MenuItem("AssetBundle/Packer/Pack_Scene")]
    public static void Pack()
    {
        List<string> pack_dir_lst = new List<string>();
        pack_dir_lst.Add("Assets/Resources");
        pack_dir_lst.Add("Assets/Scenes");

        // 收集要打包的资源
        List<string> pack_asset_lst = new List<string>();
        for (int i = 0; i < pack_dir_lst.Count; ++i)
        {
            if (Directory.Exists(pack_dir_lst[i]))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(pack_dir_lst[i]);
                FileInfo[] assets = dirInfo.GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo asset in assets)
                {
                    string asset_path = PackerToolFunc.GetAssetsRelativePath(PackerToolFunc.NormalizePath(asset.FullName));
                    if (PackerToolFunc.IsAssetFile(asset_path))
                    {
                        pack_asset_lst.Add(asset_path);
                    }
                }
            }
        }

        // 打包资源
        for (int i = 0; i < pack_asset_lst.Count; ++i)
        {
            if (IsAssetPackable(pack_asset_lst[i]))
            {
                EditorUtility.DisplayProgressBar("AssetBundleMaker", "正在生成打包配置:" + pack_asset_lst[i], 1.0f);
                string pack_group = GetAssetPackGroup(pack_asset_lst[i]);
                string asset_full_path = PackerToolFunc.GetFullPathFromAssetsRelativePath(pack_asset_lst[i]);
                PackerToolFunc.SetAssetBundlePackGroup(asset_full_path, pack_group);
            }
        }
    }

    // 指定资源是否要参与打包
    static bool IsAssetPackable(string asset)
    {   
        // 黑名单，CS，Anim，FBX，AnimCtroller不打包
        bool packable = !(PackerToolFunc.IsCodeFile(asset)
            || PackerToolFunc.IsAnimationClipFile(asset)
            || PackerToolFunc.IsFbxFile(asset)
            || PackerToolFunc.IsAnimationControllerFile(asset));
        if (packable)
        {
            if (asset_black_mapper.ContainsKey(asset))
            {
                packable = false;
            }
            else
            {
                foreach (KeyValuePair<string, bool> item in asset_black_mapper)
                {
                    if (0 <= asset.IndexOf(item.Key))
                    {
                        packable = false;
                        break;
                    }
                }
            }
        }

        return packable;
    }

    // 获取指定资源的打包Tag，到这里都是要打包
    static string GetAssetPackGroup(string asset)
    {
        string pack_group = GetAssetDirPackTag(asset);
        if (string.IsNullOrEmpty(pack_group))
        {
            pack_group = Path.GetFileNameWithoutExtension(asset).ToString();
        }

        return pack_group;
    }

    // 添加黑名单
    static void SafeInsertBlackMapper(string asset_path)
    {
        PackerToolFunc.SafeInsertDicItem<string, bool>(asset_black_mapper, 
            new KeyValuePair<string, bool>(asset_path, true));
    }

    // 按文件夹打包列表,其实也支持单独指定文件
    static void SafeInsertPackMapper(string asset_path,string pack_tag)
    {
        PackerToolFunc.SafeInsertDicItem<string, string>(asset_dir_pack_mapper,
            new KeyValuePair<string, string>(asset_path, pack_tag));
    }

    // Asset按文件夹打包，本函数获取按文件夹打包的PackTag
    static string GetAssetDirPackTag(string asset)
    {
        string dir_pack_tag = "";
        if (asset_dir_pack_mapper.ContainsKey(asset))
        {
            dir_pack_tag = asset_dir_pack_mapper[asset];
        }
        else
        {
            foreach (KeyValuePair<string, string> item in asset_dir_pack_mapper)
            {
                if (0 <= asset.IndexOf(item.Key))
                {
                    dir_pack_tag = item.Value;
                    break;
                }
            }
        }

        return dir_pack_tag;
    }

    // 生成二级文件夹打包配置
    static void CreateL2DirPackCfg(string L1Dir,string parent_pack_tag)
    {
        if (Directory.Exists(L1Dir))
        {
            DirectoryInfo dirInfo = new DirectoryInfo(L1Dir);
            DirectoryInfo[] L2DirInfos = dirInfo.GetDirectories("*",SearchOption.TopDirectoryOnly);
            if (null != L2DirInfos)
            {
                for(int i = 0; i < L2DirInfos.Length; ++i)
                {   
                    string L2DirAssetPath = PackerToolFunc.GetAssetsRelativePath(PackerToolFunc.NormalizePath(L2DirInfos[i].FullName));
                    SafeInsertPackMapper(L2DirAssetPath, (parent_pack_tag + "/" + L2DirInfos[i].Name).ToLower());
                }
            }
        }
    }

    // 生成一级文件夹打包配置，只是打一级目录的非目录文件

    static void CreateL1DirPackCfg(string L1Dir,string pack_tag)
    {
        if (Directory.Exists(L1Dir))
        {
            DirectoryInfo dirInfo = new DirectoryInfo(L1Dir);
            FileInfo[] L1DirFileInfo = dirInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
            if (null != L1DirFileInfo)
            {
                for (int i = 0; i < L1DirFileInfo.Length; ++i)
                {
                    string L1DirAssetPath = PackerToolFunc.GetAssetsRelativePath(PackerToolFunc.NormalizePath(L1DirFileInfo[i].FullName));
                    if (PackerToolFunc.IsAssetFile(L1DirAssetPath))
                    {
                        SafeInsertPackMapper(L1DirAssetPath, pack_tag.ToLower());
                    }
                }
            }
        }
    }

    // 生成按文件类型打包配置，Texture
    static void CreateDirTexPackCfg(string Dir, string pack_tag)
    {
        if (Directory.Exists(Dir))
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Dir);
            FileInfo[] FileInfo = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            if (null != FileInfo)
            {
                for (int i = 0; i < FileInfo.Length; ++i)
                {
                    string FileAssetPath = PackerToolFunc.GetAssetsRelativePath(PackerToolFunc.NormalizePath(FileInfo[i].FullName));
                    if (PackerToolFunc.IsImageFile(FileAssetPath))
                    {
                        SafeInsertPackMapper(FileAssetPath, pack_tag + "/img_" + Path.GetFileNameWithoutExtension(FileAssetPath).ToLower());
                    }
                }
            }
        }
    }

    // 生成按文件类型打包配置，Matrial

    static void CreateDirMatPackCfg(string Dir, string pack_tag)
    {
        if (Directory.Exists(Dir))
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Dir);
            FileInfo[] FileInfo = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            if (null != FileInfo)
            {
                for (int i = 0; i < FileInfo.Length; ++i)
                {
                    string FileAssetPath = PackerToolFunc.GetAssetsRelativePath(PackerToolFunc.NormalizePath(FileInfo[i].FullName));
                    if (PackerToolFunc.IsMatFile(FileAssetPath))
                    {
                        SafeInsertPackMapper(FileAssetPath, pack_tag + "/mat_" + Path.GetFileNameWithoutExtension(FileAssetPath).ToLower());
                    }
                }
            }
        }
    }

    // 生成按文件类型打包配置，Prefab
    static void CreateDirPrefabPackCfg(string Dir, string pack_tag)
    {
        if (Directory.Exists(Dir))
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Dir);
            FileInfo[] FileInfo = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            if (null != FileInfo)
            {
                for (int i = 0; i < FileInfo.Length; ++i)
                {
                    string FileAssetPath = PackerToolFunc.GetAssetsRelativePath(PackerToolFunc.NormalizePath(FileInfo[i].FullName));
                    if (PackerToolFunc.IsPrefabFile(FileAssetPath))
                    {
                        SafeInsertPackMapper(FileAssetPath, pack_tag + "/prefab_" + Path.GetFileNameWithoutExtension(FileAssetPath).ToLower());
                    }
                }
            }
        }
    }

    // 生成按文件类型打包配置，Controller
    static void CreateDirControllerPackCfg(string Dir, string pack_tag)
    {
        if (Directory.Exists(Dir))
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Dir);
            FileInfo[] FileInfo = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            if (null != FileInfo)
            {
                for (int i = 0; i < FileInfo.Length; ++i)
                {
                    string FileAssetPath = PackerToolFunc.GetAssetsRelativePath(PackerToolFunc.NormalizePath(FileInfo[i].FullName));
                    if (PackerToolFunc.IsAnimationControllerFile(FileAssetPath))
                    {
                        SafeInsertPackMapper(FileAssetPath, pack_tag + "/controller_" + Path.GetFileNameWithoutExtension(FileAssetPath).ToLower());
                    }
                }
            }
        }
    }
}
