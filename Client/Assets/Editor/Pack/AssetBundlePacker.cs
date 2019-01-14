using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class AssetBundlePacker : ScriptableObject
{
    static string asset_ab_mapper_name = Application.dataPath + "/abfiledic.bytes";
    static string static_ref_mapper_name = Application.dataPath + "/static_ref_dic.bytes";
    static string asset_ab_ext = "ab";

    static void AssetBundlePacker_StandaloneWindows()
    {
        Pack(BuildTarget.StandaloneWindows64);
    }

    static void Pack(BuildTarget build_target)
    {
        EditorUtility.ClearProgressBar();

        // 准备打包 初始化Packer
        string out_path = Application.dataPath + "/../AssetBundle";
        string platform = "";
        if (build_target == BuildTarget.StandaloneWindows64)
        {
            platform = "/pc";
        }

        out_path += platform;

        if (!Directory.Exists(out_path))
        {
            Directory.CreateDirectory(out_path);
        }

        // 保存AssetBundle字典
        SaveAssetBundleMapper();

        // 保存静态引用字典
        SaveStaticRefMapper();

        BuildAssetBundleOptions option = BuildAssetBundleOptions.ChunkBasedCompression
                                         | BuildAssetBundleOptions.DeterministicAssetBundle
                                         | BuildAssetBundleOptions.DisableWriteTypeTree;

        AssetDatabase.ImportAsset(PackerToolFunc.GetAssetsRelativePath(asset_ab_mapper_name));
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        BuildPipeline.BuildAssetBundles(out_path, GetAssetBundleBuild(), option, build_target);

        Debug.Log("Execute Packer Finished " + DateTime.Now.ToShortTimeString());
    }

    public static void GetAuthority(FileInfo file)
    {
        if (!file.Exists) return;
        if (file.Attributes != FileAttributes.Normal)
        {
            file.Attributes = FileAttributes.Normal;
        }
    }

    static void InitPacker()
    {
        Debug.Log("ImportAsset: Assets/OutGame begin " + DateTime.Now.ToShortTimeString());
        AssetDatabase.ImportAsset("Assets/OutGame", ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate); //很快
        Debug.Log("ImportAsset: Assets/UI begin " + DateTime.Now.ToShortTimeString());
        AssetDatabase.ImportAsset("Assets/UI", ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate); //3min
        Debug.Log("ImportAsset: Assets/Resources/UI_FX begin " + DateTime.Now.ToShortTimeString());
        AssetDatabase.ImportAsset("Assets/Resources/UI_FX", ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate); //1min
        Debug.Log("ImportAsset: Assets/Resources/UI_FX end " + DateTime.Now.ToShortTimeString());

        // 初始化字典
        asset_ab_mapper.Clear();
        if (File.Exists(asset_ab_mapper_name))
        {
            File.SetAttributes(asset_ab_mapper_name, FileAttributes.Normal);
        }

        // 初始化动态黑名单
        dynamic_ab_blacklist.Clear();

        // 初始化UI / Scene
        Debug.Log("AssetBundlePacker_UI.InitPacker() begin " + DateTime.Now.ToShortTimeString());
        AssetBundlePacker_UI.InitPacker(); //很快
        Debug.Log("AssetBundlePacker_Scene.InitPacker() begin " + DateTime.Now.ToShortTimeString());
        AssetBundlePacker_Scene.InitPacker(); //很快
        Debug.Log("AssetBundlePacker_Scene.InitPacker() end " + DateTime.Now.ToShortTimeString());
    }

    // AssetBundle打包配置
    static Dictionary<string, List<string>> asset_ab_build_mapper = new Dictionary<string, List<string>>();

    // 字典文件, 指定的资源在哪个包里面
    static Dictionary<string, string> asset_ab_mapper = new Dictionary<string, string>();

    // 动态黑名单，在此名单中的资源，当前不在处理
    static Dictionary<string, bool> dynamic_ab_blacklist = new Dictionary<string, bool>();

    public static void UpdateAssetBundleBuild(string ab_name, string asset_path)
    {
        if (!IsDynamicBlacklist(asset_path))
        {
            // 不在动态黑名单，已在的将会被忽略掉
            if (!asset_ab_mapper.ContainsKey(asset_path))
            {
                asset_ab_mapper.Add(asset_path, ab_name);
            }
            else
            {
                if (asset_ab_build_mapper.ContainsKey(asset_ab_mapper[asset_path]))
                {
                    asset_ab_build_mapper[asset_ab_mapper[asset_path]].Remove(asset_path);
                }

                asset_ab_mapper[asset_path] = ab_name;
            }

            if (asset_ab_build_mapper.ContainsKey(ab_name))
            {
                asset_ab_build_mapper[ab_name].Add(asset_path);
            }
            else
            {
                List<string> assets = new List<string>();
                assets.Add(asset_path);
                asset_ab_build_mapper.Add(ab_name, assets);
            }
        }
    }

    public static bool IsDynamicBlacklist(string asset_path)
    {
        return dynamic_ab_blacklist.ContainsKey(asset_path);
    }

    static AssetBundleBuild[] GetAssetBundleBuild()
    {
        List<AssetBundleBuild> ab_builds = new List<AssetBundleBuild>();
        foreach (KeyValuePair<string, List<string>> build_item in asset_ab_build_mapper)
        {
            AssetBundleBuild ab_build = new AssetBundleBuild();
            ab_build.assetBundleName = build_item.Key;
            ab_build.assetBundleVariant = asset_ab_ext;
            ab_build.assetNames = build_item.Value.ToArray();
            ab_builds.Add(ab_build);
        }

        return ab_builds.ToArray();
    }

    static void SaveAssetBundleMapper()
    {
        Debug.Log("SaveAssetBundleMapper: " + asset_ab_mapper_name);
        List<string> asset_keys = new List<string>(asset_ab_mapper.Keys);
        using (BinaryWriter file = new BinaryWriter(File.Open(asset_ab_mapper_name, FileMode.Truncate)))
        {
            Debug.Log("SaveAssetBundleMapper " + asset_keys.Count);
            file.Seek(0, SeekOrigin.Begin);
            file.Write(asset_keys.Count);

            for (int ix = 0; ix < asset_keys.Count; ix++)
            {
                string asset_path = PackerToolFunc.GetFilePathWithoutExt(asset_keys[ix].ToLower());
                asset_path = asset_path.Replace("assets/resources/", "");
                asset_path = asset_path.Replace("assets/scenes/", "");
                asset_path = asset_path.Replace("assets/", "");

                // 字典中无需路径,ui图片，uipanel
                if ((0 == asset_path.IndexOf("ui/"))
                    || (0 == asset_path.IndexOf("uipanel/"))
                    || (0 == asset_path.IndexOf("ui_fx/")))
                {
                    asset_path = Path.GetFileNameWithoutExtension(asset_path);
                }

                string ab_path = asset_ab_mapper[asset_keys[ix]].ToLower();
                if (!Path.HasExtension(ab_path))
                {
                    ab_path += ".ab";
                }

                file.Write(asset_path);
                file.Write(ab_path);

                string type_str = GetAssetTypeStr(asset_keys[ix]);
                if (string.IsNullOrEmpty(type_str))
                {
                    type_str = asset_path;
                }

                file.Write(type_str);

                //Debug.Log("SaveAssetBundleMapper " + asset_path + " ---> " + ab_path + " ---> " + type_str);
            }

            //写入texturePacker 引用关系
            List<string> texturePacker_keys = new List<string>(AtlasPacker.texturePacker_dic.Keys);
            file.Write(texturePacker_keys.Count);
            string texturePackerImg = null;
            string texturePackerAltas = null;
            Dictionary<string, int> texturePackerPathDic = new Dictionary<string, int>();
            List<string> texturePackerPathList = new List<string>();
            int texturePackerPathIndex = 0;
            for (int ix = 0; ix < texturePacker_keys.Count; ix++)
            {
                texturePackerAltas = texturePacker_keys[ix];
                if (AtlasPacker.texturePacker_dic.TryGetValue(texturePackerAltas, out texturePackerImg))
                {
                    file.Write(texturePackerAltas);
                    if (texturePackerPathDic.TryGetValue(texturePackerImg, out texturePackerPathIndex))
                    {
                        file.Write(texturePackerPathIndex);
                    }
                    else
                    {
                        texturePackerPathIndex = texturePackerPathList.Count;
                        texturePackerPathList.Add(texturePackerImg);
                        texturePackerPathDic[texturePackerImg] = texturePackerPathIndex;
                        file.Write(texturePackerPathIndex);
                    }
                }
                else
                {
                    Debug.Log("texturePacker no atlas img name: " + texturePackerAltas);
                }
            }

            //写入texturepacker path  index dic
            file.Write(texturePackerPathList.Count);
            for (int ix = 0; ix < texturePackerPathList.Count; ix++)
            {
                file.Write(texturePackerPathList[ix]);
            }

            // 哪些包可以不卸载，应该通过Lua文件来配置，
            // 否则每次有变动还得打资源,这个代价就大了
            file.Flush();
            file.Close();
        }

        PackerToolFunc.SetAssetBundlePackGroup(asset_ab_mapper_name, Path.GetFileNameWithoutExtension(asset_ab_mapper_name), "ab");
    }

    static void UpdateDynamicBlacklist()
    {
        dynamic_ab_blacklist.Clear();

        foreach (KeyValuePair<string, List<string>> build_item in asset_ab_build_mapper)
        {
            for (int i = 0; i < build_item.Value.Count; ++i)
            {
                if (!dynamic_ab_blacklist.ContainsKey(build_item.Value[i]))
                {
                    dynamic_ab_blacklist.Add(build_item.Value[i], true);
                }
                else
                {
                    Debug.LogError("Same Asset " + build_item.Value[i]);
                }
            }
        }
    }

    static void SaveStaticRefMapper()
    {
        if (File.Exists(static_ref_mapper_name))
        {
            File.SetAttributes(static_ref_mapper_name, FileAttributes.Normal);
        }

        // Prefab的黑名单
        Dictionary<string, bool> prefab_black_dic = new Dictionary<string, bool>(128, StringComparer.OrdinalIgnoreCase);
        prefab_black_dic.Add("UI_Pattern", false);

        Dictionary<string, List<string>> image_ref_dic = new Dictionary<string, List<string>>();

        // 遍历Prefab获取Prefab上静态图片的引用关系
        List<string> prefab_dir = new List<string>();
        prefab_dir.Add("Resources/UIPanel");
        for (int i = 0; i < prefab_dir.Count; ++i)
        {
            if (Directory.Exists(Application.dataPath + "/" + prefab_dir[i]))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath + "/" + prefab_dir[i]);
                FileInfo[] prefabs = dirInfo.GetFiles("*.prefab", SearchOption.AllDirectories);
                foreach (FileInfo prefab in prefabs)
                {
                    // 在黑名单中不处理
                    if (prefab_black_dic.ContainsKey(Path.GetFileNameWithoutExtension(prefab.Name)))
                    {
                        continue;
                    }

                    string prefab_asset_path = PackerToolFunc.GetAssetsRelativePath(PackerToolFunc.NormalizePath(prefab.FullName));
                    GameObject goPrefab = (GameObject) AssetDatabase.LoadAssetAtPath(prefab_asset_path, typeof(GameObject));
                    if (null != goPrefab)
                    {
                        Image[] images = goPrefab.GetComponentsInChildren<Image>();
                        if (null != images)
                        {
                            for (int j = 0; j < images.Length; ++j)
                            {
                                if ((null != images[j])
                                    && (null != images[j].sprite))
                                {
                                    // 内置资源不处理
                                    string asset_path = AssetDatabase.GetAssetPath(images[j].sprite);
                                    if ((!string.IsNullOrEmpty(asset_path))
                                        && (!PackerToolFunc.IsBuiltinFile(asset_path)))
                                    {
                                        if (image_ref_dic.ContainsKey(prefab_asset_path))
                                        {
                                            image_ref_dic[prefab_asset_path].Add(asset_path);
                                        }
                                        else
                                        {
                                            List<string> ref_images = new List<string>();
                                            ref_images.Add(asset_path);
                                            image_ref_dic.Add(prefab_asset_path, ref_images);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // 保存到字典中
        List<string> image_ref_keys = new List<string>(image_ref_dic.Keys);
        using (BinaryWriter file = new BinaryWriter(File.Open(static_ref_mapper_name, FileMode.Truncate)))
        {
            Debug.Log("SaveStaticRefMapper " + image_ref_keys.Count);
            file.Seek(0, SeekOrigin.Begin);
            file.Write(image_ref_keys.Count);

            for (int i = 0; i < image_ref_keys.Count; i++)
            {
                string prefab_path = image_ref_keys[i];
                string prefab_name = Path.GetFileNameWithoutExtension(prefab_path.ToLower());
                string ref_images_str = "";
                List<string> ref_image_lst = image_ref_dic[prefab_path];
                for (int j = 0; j < ref_image_lst.Count; ++j)
                {
                    ref_images_str += Path.GetFileNameWithoutExtension(ref_image_lst[j]).ToLower() + ",";
                }

                file.Write(prefab_name);
                file.Write(ref_images_str);
                Debug.Log("SaveStaticRefMapper " + prefab_name + " ---> " + ref_images_str);
            }

            file.Flush();
            file.Close();
        }

        PackerToolFunc.SetAssetBundlePackGroup(static_ref_mapper_name, Path.GetFileNameWithoutExtension(static_ref_mapper_name), "ab");
    }

    // 获取Asset的数据类型，根据扩展
    static string GetAssetTypeStr(string asset_path)
    {
        string type_str = "";
        if ((PackerToolFunc.IsPrefabFile(asset_path))
            || (PackerToolFunc.IsSceneFile(asset_path)))
        {
            type_str = "GameObject";
        }
        else if (PackerToolFunc.IsImageFile(asset_path))
        {
            type_str = "Image";
        }
        else if (PackerToolFunc.IsMatFile(asset_path))
        {
            type_str = "Mat";
        }
        else if (PackerToolFunc.IsAnimationControllerFile(asset_path))
        {
            type_str = "AnimationController";
        }
        else if (PackerToolFunc.IsAnimationClipFile(asset_path))
        {
            type_str = "AnimationClip";
        }
        else if (PackerToolFunc.IsShaderFile(asset_path))
        {
            type_str = "Shader";
        }

        return type_str;
    }
}