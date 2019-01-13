using UnityEngine;
using System.Collections;
using System.IO;
using System.Diagnostics;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

// 资源类型
public enum ASSET_TYPE
{
    ASSET_TYPE_NONE = 0,
    ASSET_TYPE_IMAGE = 1,   // 图片
    ASSET_TYPE_SHADER = 2,  // Shader
    ASSET_TYPE_PREFAB = 3,  // Prefab
    ASSET_TYPE_FBX = 4,  // FBX
    ASSET_TYPE_MAT = 5,  // 材质
    ASSET_TYPE_ANIM_CLIP = 6, // Animation Clip
    ASSET_TYPE_ANIM_CONTROLLER = 7,  // Animation Controller
    ASSET_TYPE_SCENE = 8,  // Unity场景
    ASSET_TYPE_SHADER_INC = 9,  // Shader头文件
    ASSET_TYPE_CODE = 10, // 代码文件
    ASSET_TYPE_ALL = 16,
}

public class PackerToolFunc : MonoBehaviour {

    // 标准化路径
    public static string NormalizePath(string path)
    {
        return path.Replace("\\", "/");
    }

    public static string GetAssetsRelativePath(string full_path)
    {
        string normalize_path = NormalizePath(full_path);
        if (normalize_path.IndexOf("/Assets/") < 0)
        {
            return normalize_path;
        }
        else
        {
            return normalize_path.Substring(normalize_path.IndexOf("/Assets/") + 1);
        }
    }

    public static string GetFullPathFromAssetsRelativePath(string asset_path)
    {
        string normalize_path = NormalizePath(asset_path);
        return Application.dataPath + "/" + asset_path.Substring(asset_path.IndexOf("/") + 1);
    }

    public static string GetProjectRootPath()
    {
        return Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/Assets"));
    }

    public static string GetLowestDirName(string asset_path)
    {
        string lowest_dir = PackerToolFunc.NormalizePath(Path.GetDirectoryName(asset_path));
        if (lowest_dir.LastIndexOf("/") < 0)
        {
            return lowest_dir;
        }
        else
        {
            lowest_dir = lowest_dir.Substring(lowest_dir.LastIndexOf("/") + 1);
            return lowest_dir;
        }
    }

    public static string GetFilePathWithoutExt(string file_path)
    {
        return Path.GetDirectoryName(file_path) + "/" + Path.GetFileNameWithoutExtension(file_path);
    }

    // 设置打包的分组
    public static void ClearAssetBundlePackGroup(string file_name)
    {
        SetAssetBundlePackGroup(file_name,"","",true);

        //MetaModifer.ModifyMetaKey(file_name + ".meta", "assetBundleName", "");
        //MetaModifer.ModifyMetaKey(file_name + ".meta", "assetBundleVariant", "");
    }

    // AssetBundleName存在同名BUG，全部采用AssetBundleBuild方式
    public static void SetAssetBundlePackGroup(string file_name, string bundle_name,string bundle_ext = "ab",bool need_save = false)
    {
        string asset_name = GetAssetsRelativePath(file_name);
        AssetImporter importer = AssetImporter.GetAtPath(asset_name);
        if (null != importer)
        {   
            // AssetBundleName永远为空
            importer.SetAssetBundleNameAndVariant("","");
            if (!string.IsNullOrEmpty(bundle_name))
            {
                AssetBundlePacker.UpdateAssetBundleBuild(bundle_name, asset_name);
            }

            //if (need_save)
            //{
            //    MetaModifer.ModifyMetaKey(file_name + ".meta", "assetBundleName", bundle_name);
            //    MetaModifer.ModifyMetaKey(file_name + ".meta", "assetBundleVariant", bundle_ext);
            //}
        }

        //MetaModifer.ModifyMetaKey(file_name + ".meta", "assetBundleName", bundle_name);
        //MetaModifer.ModifyMetaKey(file_name + ".meta", "assetBundleVariant", bundle_ext);

        //string bundle_path = GetAssetBundlePath(file_name, bundle_name);

        //MetaModifer.ModifyMetaKey(file_name + ".meta", "assetBundleName", bundle_path);
        //MetaModifer.ModifyMetaKey(file_name + ".meta", "assetBundleVariant", bundle_ext);

        //file_name = GetAssetsRelativePath(file_name);
        //AssetBundlePacker.UpdateAssetBundleMapper(bundle_path, file_name);
    }

    // 设置图片的打包分组
    public static void ClearSpritePackGroup(string file_name)
    {
        SetSpritePackGroup(file_name,"",true);
        //MetaModifer.ModifyMetaKey(file_name + ".meta", "spritePackingTag", "");
    }

    public static void SetSpritePackGroup(string file_name, string atlas_tag,bool need_save = false)
    {
        //todo 由于AtlasPack被关闭，设置tag，会造成图片不压缩，后期需要处理图集打包
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            atlas_tag = "";
        }

        string asset_name = GetAssetsRelativePath(file_name);
        AssetImporter importer = AssetImporter.GetAtPath(asset_name);
        if (importer is TextureImporter)
        {
            TextureImporter texture_importer = importer as TextureImporter;
            texture_importer.spritePackingTag = atlas_tag;
            if (need_save)
            {
                MetaModifer.ModifyMetaKey(file_name + ".meta", "spritePackingTag", atlas_tag);
            }
        }
    }

    // 各种文件类型判断
    public static bool IsAssetFile(string file_name)
    {   
        // 判断扩展名，不能是META,CS,JS
        string invalid_ext = ".META,.CS,.JS,.DS_STORE";

        bool is_asset = true;
        if (Path.HasExtension(file_name))
        {
            is_asset = invalid_ext.IndexOf(Path.GetExtension(file_name).ToUpper()) < 0;
        }
        if (is_asset)
        {
            // 路径判断，Assets/,Plugins/,StreamingAssets/
            is_asset = 0 <= file_name.IndexOf("Assets/");
            if (is_asset)
            {
                is_asset = file_name.IndexOf("Assets/Plugins/") < 0;
                if (is_asset)
                {
                    is_asset = file_name.IndexOf("Assets/StreamingAssets/") < 0;
                }
            }
        }
            
        return is_asset;
    }

    // 是否是Prefab
    public static bool IsPrefabFile(string file_name)
    {
        return file_name.ToUpper().EndsWith(".PREFAB");
    }

    public static bool IsImageFile(string file_name)
    {
        string upperName = file_name.ToUpper();
        return upperName.EndsWith(".PNG") || upperName.EndsWith(".JPG") || upperName.EndsWith(".TGA");
    }

    public static bool IsTpsheetFile(string file_name)
    {
        string upperName = file_name.ToUpper();
        return upperName.EndsWith(".TPSHEET");
    }

    // 判断指定文件是否是Shader
    public static bool IsShaderFile(string file_path)
    {
        return file_path.ToUpper().EndsWith(".SHADER");
    }

    // 判断指定文件是否是Shader 包括CGINC
    public static bool IsShaderIncludeFile(string file_path)
    {
        return file_path.ToUpper().EndsWith(".CGINC");
    }

    // 是否是材质文件
    public static bool IsMatFile(string file_path)
    {
        return file_path.ToUpper().EndsWith(".MAT");
    }

    // 是否是FBX文件
    public static bool IsFbxFile(string file_path)
    {
        return file_path.ToUpper().EndsWith(".FBX");
    }

    // 是否是代码文件
    public static bool IsCodeFile(string file_path)
    {
        string upperName = file_path.ToUpper();
        return upperName.EndsWith(".CS");
    }

    // 是否是场景文件
    public static bool IsSceneFile(string file_path)
    {
        return file_path.ToUpper().EndsWith(".UNITY");
    }

    // 是否是AnimationClip
    public static bool IsAnimationClipFile(string file_path)
    {
        return file_path.ToUpper().EndsWith(".ANIM");
    }

    // 是否是AnimationController
    public static bool IsAnimationControllerFile(string file_path)
    {
        return file_path.ToUpper().EndsWith(".CONTROLLER");
    }

    // 是否是内置资源
    public static bool IsBuiltinFile(string file_path)
    {
        return 0 <= file_path.ToUpper().IndexOf("Resources/unity_builtin_extra".ToUpper());
    }

    // 是否参与Build的文件
    public static bool IsNeedBuildFile(string file_path)
    {
        return !(IsCodeFile(file_path) || IsAssetFile(file_path));
    }

    // 是否是指定类型的资源
    public static bool IsSpecifyFile(string file_name,ASSET_TYPE asset_type)
    {
        bool specify = false;
        switch(asset_type)
        {
            case ASSET_TYPE.ASSET_TYPE_ALL:
                {
                    specify = IsAssetFile(file_name);
                    break;
                }
            case ASSET_TYPE.ASSET_TYPE_IMAGE:
                {
                    specify = IsImageFile(file_name);
                    break;
                }
            case ASSET_TYPE.ASSET_TYPE_MAT:
                {
                    specify = IsMatFile(file_name);
                    break;
                }
            case ASSET_TYPE.ASSET_TYPE_SHADER:
                {
                    specify = IsShaderFile(file_name);
                    break;
                }
            case ASSET_TYPE.ASSET_TYPE_SHADER_INC:
                {
                    specify = IsShaderIncludeFile(file_name);
                    break;
                }
            case ASSET_TYPE.ASSET_TYPE_SCENE:
                {
                    specify = IsSceneFile(file_name);
                    break;
                }
            case ASSET_TYPE.ASSET_TYPE_ANIM_CLIP:
                {
                    specify = IsAnimationClipFile(file_name);
                    break;
                }
            case ASSET_TYPE.ASSET_TYPE_ANIM_CONTROLLER:
                {
                    specify = IsAnimationControllerFile(file_name);
                    break;
                }
            case ASSET_TYPE.ASSET_TYPE_PREFAB:
                {
                    specify = IsPrefabFile(file_name);
                    break;
                }
        }

        return specify;
    }

    // 拷贝文件
    public static void CopyFile(string src, string dst)
    {
        if (File.Exists(src))
        {
            if (File.Exists(dst))
            {
                File.SetAttributes(dst, FileAttributes.Normal);
            }
            File.Copy(src, dst, true);
        }
    }

    // 是否是同一个文件, 只支持Assets目录下文件
    public static bool IsSameFile(string src, string dst)
    {
        src = NormalizePath(src);
        dst = NormalizePath(dst);
        src = src.Substring(src.LastIndexOf("/Assets/") + 1);
        dst = dst.Substring(dst.LastIndexOf("/Assets/") + 1);
        return AssetDatabase.AssetPathToGUID(src) == AssetDatabase.AssetPathToGUID(dst);
    }

    // Unity中拷贝文件，传入全路径
    public static void UnityCopyFile(string src, string dst)
    {
        if (!IsSameFile(src, dst))
        {
            if (File.Exists(src))
            {
                File.SetAttributes(src, FileAttributes.Normal);
                if (File.Exists(dst))
                {
                    File.SetAttributes(dst, FileAttributes.Normal);
                }
                // 转化为Assets路径
                src = GetAssetsRelativePath(src);
                dst = GetAssetsRelativePath(dst);
                AssetDatabase.CopyAsset(src, dst);
            }
        }
    }

    public static void UnityDeleteFile(string src)
    {
        if (File.Exists(src))
        {
            File.SetAttributes(src, FileAttributes.Normal);
            //File.Delete(src);

            // 转换为Assets路径
            AssetDatabase.DeleteAsset(GetAssetsRelativePath(src));
        }
    }

    // 设置指定目录中指定文件的分组
    public static void SetDirAssetsBundlePackGroup(string dir,ASSET_TYPE asset_type,
        string ab_name = "", string ab_ext = "ab", string ab_prefix = "",List<string> exclude_lst = null)
    {   
        if (Directory.Exists(dir))
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            FileInfo[] assets = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            foreach (FileInfo asset in assets)
            {
                string asset_path = PackerToolFunc.NormalizePath(asset.FullName);

                // 判断是否在排除列表
                bool exclude = false;
                if (null != exclude_lst)
                {
                    for (int i = 0; i < exclude_lst.Count && !exclude; ++i)
                    {
                        if (0 <= asset_path.ToUpper().IndexOf(exclude_lst[i].ToUpper()))
                        {
                            exclude = true;
                            break;
                        }
                    }
                }

                if (!exclude)
                {
                    if (PackerToolFunc.IsSpecifyFile(asset_path,asset_type))
                    {
                        string ab_name_r = Path.GetFileNameWithoutExtension(asset_path);
                        if ("" != ab_prefix)
                        {
                            ab_name_r = ab_prefix + "_" + ab_name_r;
                        }
                        if (ab_name != "")
                        {
                            ab_name_r = ab_name;
                        }
                        PackerToolFunc.SetAssetBundlePackGroup(asset_path, ab_name_r, ab_ext);
                    }
                }
            }
        }
    }


    // 
    // 1: 文件单独打包。2:文件夹整体打包。3:多个文件夹文件合并打包
    // 传入的是全路径
    static string GetAssetBundlePath(string asset_path,string ab_name)
    {
        return ab_name;
        /*
        string ab_path = "";
        string lowest_dir_name = GetLowestDirName(asset_path);
        if (lowest_dir_name.ToUpper() == "Assets".ToUpper())
        {   
            // 根目录
            ab_path = ab_name;
        }
        else
        {
            string asset_name = Path.GetFileNameWithoutExtension(asset_path);
            if (asset_name.ToUpper() == ab_name.ToUpper())
            {
                // 1: 文件单独打包。
                ab_path = Path.GetDirectoryName(asset_path) + "/" + ab_name;
            }
            else
            if (lowest_dir_name.ToUpper() == ab_name.ToUpper())
            {
                // 2:文件夹整体打包。
                ab_path = Path.GetDirectoryName(asset_path) + "/" + ab_name;
            }
            else
            {
                // 3:多个文件夹文件合并打包，根目录创建
                ab_path = ab_name;
            }
        }

        string ab_path_in_assets = "";
        if (ab_path.IndexOf("/Assets/") < 0)
        {
            ab_path_in_assets = ab_path.ToLower();
        }
        else
        {
            ab_path_in_assets = ab_path.Substring(ab_path.LastIndexOf("/Assets/") + "/Assets/".Length).ToLower();
        }
        return ab_path_in_assets;*/
    }

    // 设置指定目录图片文件的图集Tag
    /*public static void SetDirImagesSpritePackGroup(string dir,bool altas = false,string sprite_pack_tag = "", bool packTagUseName = false)
    {
        if (Directory.Exists(dir))
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            FileInfo[] assets = dirInfo.GetFiles("*", SearchOption.AllDirectories);
            bool canAltas;
            foreach (FileInfo asset in assets)
            {
                canAltas = altas;
                string asset_path = PackerToolFunc.NormalizePath(asset.FullName);
                string fileName = Path.GetFileNameWithoutExtension(asset_path);
                bool cannotAtlas = UITexturePostProcess.LargerWithOutFile.ContainsKey(fileName);
                if (cannotAtlas)
                    canAltas = false;
                if (IsSpecifyFile(asset_path,ASSET_TYPE.ASSET_TYPE_IMAGE))
                {
                    // 如果长宽比过高，则不参与Atlas制作
                    if ((canAltas)
                        && (ShouldImageBuildAtlas(asset_path)))
                    {   
                        string sprite_pack_tag_r = Path.GetFileNameWithoutExtension(asset_path);
                        if (sprite_pack_tag != "")
                        {
                            sprite_pack_tag_r = sprite_pack_tag;
                        }
                        if (packTagUseName)
                        {
                            sprite_pack_tag_r = fileName;
                        }
                        SetSpritePackGroup(asset_path, sprite_pack_tag_r);
                    }
                    else
                    {
                        SetSpritePackGroup(asset_path, "");
                    }
                }
            }
        }
    }*/

    // 图片是否能参与Altas制作
    public static bool ShouldImageBuildAtlas(string image_path)
    {
        bool can_atlas = false;
        string asset_path = GetAssetsRelativePath(image_path);
        int w = 0;
        int h = 0;
        GetImageSize(asset_path,ref w,ref h);

        // 长度或宽度大于256
        can_atlas = (w <= 256) && (h <= 256);
        if (can_atlas)
        {
            // 长宽比过大，则不参与atlas制作
            //float wh_ratio = w * 1.0f / h;
            //can_atlas = ((4.0f <= wh_ratio) || (wh_ratio <= 0.25f)) && (256);
        }

        return can_atlas;
    }

    // 获取图片的大小
    public static void GetImageSize(string asset_path, ref int w, ref int h)
    {
        w = 0;
        h = 0;
        Texture2D asset = AssetDatabase.LoadAssetAtPath<Texture2D>(asset_path);
        if (null != asset)
        {
            w = asset.width;
            h = asset.height;
        }
    }

    // 是否是2次幂图片
    public static bool IsLog2Image(string asset_path)
    {
        int w = 0;
        int h = 0;
        GetImageSize(asset_path,ref w, ref h);

        if ((w <= 1)
            || (h <= 1))
        {
            return false;
        }
        else
        {
            return ((w == h) && (w & w - 1) == 0) && ((h & h - 1) == 0);
        }
    }

    // 是否是4倍数图片
    public static bool IsMutiple4Image(string asset_path)
    {
        int w = 0;
        int h = 0;
        GetImageSize(asset_path,ref w, ref h);

        return (0 == w % 4) && (0 == h % 4);
    }

    // 字典操作
    public static void SafeInsertDicItem<T1, T2>(Dictionary<T1, T2> dic, 
        KeyValuePair<T1, T2> item, bool force_update = false)
    {
        if (!dic.ContainsKey(item.Key))
        {
            dic.Add(item.Key, item.Value);
        }
        else
        if (force_update)
        {
            dic[item.Key] = item.Value;
        }
    }

    public static bool SafeCreateEmptyFolder(string path)
    {
        if (Directory.Exists(path))
        {
            string[] assets = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            if (null != assets)
            {
                for (int i = 0; i < assets.Length; ++i)
                {
                    File.SetAttributes(assets[i], FileAttributes.Normal);
                }
            }

            Directory.Delete(path, true);
        }
        return (null != Directory.CreateDirectory(path));
    }

    public static bool IsImageSliced(string image_path)
    {
        bool sliced = false;
        Sprite image = AssetDatabase.LoadAssetAtPath<Sprite>(image_path);
        if (null != image)
        {
            sliced = (image.border.x != 0) || (image.border.y != 0) || (image.border.z != 0) || (image.border.w != 0);
        }

        return sliced;
    }

    // 获取图片占用的空间
    public static float GetImageSize(string image_path)
    {
        float size = 0.0f;
        string asset_path = PackerToolFunc.GetAssetsRelativePath(image_path);
        Object goImage = AssetDatabase.LoadAssetAtPath(asset_path, typeof(Object));
        if (null != goImage)
        {
#if UNITY_5_3
            size = Profiler.GetRuntimeMemorySize(goImage);
#else
            size = UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(goImage);
#endif
        }

        return size;
    }

}
