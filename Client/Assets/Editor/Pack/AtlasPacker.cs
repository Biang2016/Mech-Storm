using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using System;
using UnityEngine.UI;

public class AtlasPacker : MonoBehaviour
{

    // AB的配置
    static string ab_pre = "atlas_";
    static string ab_ext = "ab";
    static string ATLAS_ROOT = Application.dataPath + "/Resources/Atlas";

    // 路径是以Asset为根目录
    static List<string> atlas_src_dirs = new List<string>();

    static List<string> chunk_no_atlas_lst = new List<string>();
    static List<string> muti_chunk_atlas_lst = new List<string>();
    static Dictionary<string, string> muti_chunk_atlas_dic = new Dictionary<string, string>();

    // DynamicImage目录，按名字前缀打包
    static Dictionary<string, string> prefix_atlas_dic = new Dictionary<string, string>();
    static Dictionary<string, List<string>> black_prefix_atlas_dic = new Dictionary<string, List<string>>();

    // Prefab的黑名单
    static Dictionary<string, bool> prefab_black_dic = new Dictionary<string, bool>(128, StringComparer.OrdinalIgnoreCase);

    // 公用图集，私有图集，单个图集
    static Dictionary<string, List<string>> image_ref_dic = new Dictionary<string, List<string>>();

    // 所有Image的列表
    static Dictionary<string, string> image_dic = new Dictionary<string, string>();

    // 所有TexturePacker的引用字典
    public static Dictionary<string, string> texturePacker_dic = new Dictionary<string, string>();
    

    //需要做成atlas的文件目录
    static List<string> atlas_package_file_list = new List<string>()
    {
        "UI/City/MainPanel_root_new",
        "UI/City/mainpanel_duilie",
        "UI/Common/1Commmon_icon",
        "UI/DynamicImage/daoju",
        "UI/DynamicImage/pinzhi",
        "UI/DynamicImage/icon",
        "UI/DynamicImage/daoju_new",
        "UI/DynamicImage/Tech",
        "UI/DynamicImage/neizheng",
        "UI/DynamicImage/jineng",
        "UI/DynamicImage/jineng_new",
        "UI/DynamicImage/guanjue",
        "UI/DynamicImage/biaoqing",
        "UI/DynamicImage/biaoqingbig",
        "UI/DynamicImage/sandbox",
        "UI/DynamicImage/qizhi",
        "UI/DynamicImage/xianjing",
        "UI/DynamicImage/touxiang",
        "UI/DynamicImage/touxiang84",
        "UI/DynamicImage/touxiang_ext",
        "UI/DynamicImage/touxiang84_ext",
        "UI/DynamicImage/zhuangbei",
        "UI/DynamicImage/junzhubaowu",
        "UI/DynamicImage/WorldCity",
    };


    public struct AtlasPackInfo
    {
        public string atlasAbName;
        public List<string> atlasDirectoryList;
        public bool isForce;

        public AtlasPackInfo(string atlasAbName_, List<string> atlasDirectoryList_, bool isForce_ = false)
        {
            atlasAbName = atlasAbName_;
            atlasDirectoryList = atlasDirectoryList_;
            isForce = isForce_;

        }
    }

    public static List<AtlasPackInfo> atlasPackInfoList = new List<AtlasPackInfo>() {
        //主界面
        new AtlasPackInfo("atlas_MainPanel_root_new",       new List<string>(){ "UI/City/MainPanel_root_new" } ),
        //队列
        new AtlasPackInfo("atlas_mainpanel_duilie",         new List<string>(){ "UI/City/mainpanel_duilie" } ),
        //通用图标
        new AtlasPackInfo("atlas_common_icon",              new List<string>(){ "UI/Common/1Commmon_icon" } ),
        //道具
        new AtlasPackInfo("atlas_daoju",                    new List<string>(){ "UI/DynamicImage/daoju" } ),         
        new AtlasPackInfo("atlas_daoju_ext",                new List<string>(){ "UI/DynamicImage/pinzhi", "UI/DynamicImage/icon" } ), 
        new AtlasPackInfo("atlas_daoju_new",                new List<string>(){ "UI/DynamicImage/daoju_new" } ),
        new AtlasPackInfo("atlas_daoju_new1",               new List<string>(){ "UI/DynamicImage/daoju_new1" } ),
        new AtlasPackInfo("atlas_daoju_baoxiang",           new List<string>(){ "UI/DynamicImage/daoju_baoxiang" } ),
        new AtlasPackInfo("atlas_daoju_duihuan",            new List<string>(){ "UI/DynamicImage/daoju_duihuan" } ),
        new AtlasPackInfo("atlas_daoju_libao",              new List<string>(){ "UI/DynamicImage/daoju_libao" } ),
        new AtlasPackInfo("atlas_daoju_mingpai",            new List<string>(){ "UI/DynamicImage/daoju_mingpai" } ),
        new AtlasPackInfo("atlas_daoju_pifu",               new List<string>(){ "UI/DynamicImage/daoju_pifu" } ),
        new AtlasPackInfo("atlas_daoju_add",                new List<string>(){ "UI/DynamicImage/daoju_add" } ), 
        // 天赋，科技，内政
        new AtlasPackInfo("atlas_tech",                     new List<string>(){ "UI/DynamicImage/Tech", "UI/DynamicImage/neizheng" } ),
        // 技能
        new AtlasPackInfo("atlas_jineng",                   new List<string>(){ "UI/DynamicImage/jineng" } ),
        new AtlasPackInfo("atlas_jineng_new",               new List<string>(){ "UI/DynamicImage/jineng_new" } ),
        // 官爵
        new AtlasPackInfo("atlas_guanjue_biaoqing",         new List<string>(){ "UI/DynamicImage/guanjue",
                                                                                "UI/DynamicImage/biaoqing",
                                                                                "UI/DynamicImage/biaoqingbig"} ), 
        // 表情
        new AtlasPackInfo("atlas_qizhi_sandbox",            new List<string>(){ "UI/DynamicImage/sandbox",
                                                                                "UI/DynamicImage/qizhi",
                                                                                "UI/DynamicImage/xianjing" } ),
        // 头像
        new AtlasPackInfo("atlas_touxiang",                 new List<string>(){ "UI/DynamicImage/touxiang", "UI/DynamicImage/touxiang84" } ),
        new AtlasPackInfo("atlas_touxiang_ext",             new List<string>(){ "UI/DynamicImage/touxiang_ext", "UI/DynamicImage/touxiang84_ext" } ),
        new AtlasPackInfo("atlas_touxiang_add",             new List<string>(){ "UI/DynamicImage/touxiang_add"} ),
        new AtlasPackInfo("atlas_touxiang84_add",           new List<string>(){ "UI/DynamicImage/touxiang84_add"} ),
        new AtlasPackInfo("atlas_mingpai_add",              new List<string>(){ "UI/DynamicImage/mingpai_add",} ),


        // 护罩
        new AtlasPackInfo("atlas_huzhao",                   new List<string>(){ "UI/DynamicImage/HuZhao", } ),
        
        // 计策
        new AtlasPackInfo("atlas_jice",                     new List<string>(){ "UI/DynamicImage/jice", } ),
       
        // 军团官职
        new AtlasPackInfo("atlas_juntuanguanzhi",           new List<string>(){ "UI/DynamicImage/JunTuanGuanZhi", } ),


        // 装备
        new AtlasPackInfo("atlas_zhuangbei",                new List<string>(){ "UI/DynamicImage/zhuangbei" } ),
        // 君主宝物
        new AtlasPackInfo("atlas_junzhubaowu",              new List<string>(){ "UI/DynamicImage/junzhubaowu" } ),
        // world city
        new AtlasPackInfo("atlas_worldcity",                new List<string>(){ "UI/DynamicImage/WorldCity" }, true ),
    };


    public static bool IsImageInAtlas(string imgPath)
    {
        foreach (string path in atlas_package_file_list)
        {
            if (imgPath.IndexOf(path, StringComparison.OrdinalIgnoreCase) > 0)
            {
                return true;
            }
        }
        return false;
    }

    //[MenuItem("AssetBundle/Packer/AtlasMaker")]
    public static void SetAtlasPackGroup()
    {
        // 初始化Packer
        InitPacker();

        // 制作Atlas
        AtlasMaker();
    }

    static void InitPacker()
    {
        // 不参与打包的Prefab
        prefab_black_dic.Clear();
        prefab_black_dic.Add("UI_Pattern", false);

        //ClearPacktag();
    }

    static void InitAtlasMaker()
    {

    }

    // 打包UI资源

    public static void AtlasMaker()
    {
        string atlas_ab_name = "";
        Dictionary<string, string> image_atlas_dic = new Dictionary<string, string>();

        // 获取所有的UI图片列表
        List<string> image_dir = new List<string>();
        image_dir.Add("UI");
        for (int i = 0; i < image_dir.Count; ++i)
        {
            if (Directory.Exists(Application.dataPath + "/" + image_dir[i]))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath + "/" + image_dir[i]);
                FileInfo[] images = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo image in images)
                {
                    if (PackerToolFunc.IsImageFile(image.FullName))
                    {
                        string image_path = PackerToolFunc.NormalizePath(image.FullName);
                        string asset_path = PackerToolFunc.GetAssetsRelativePath(image_path);
                        if (!image_dic.ContainsKey(asset_path))
                        {
                            image_dic.Add(asset_path, image_path);
                        }
                    }
                    else if (PackerToolFunc.IsTpsheetFile(image.FullName))
                    {
                        string image_path = PackerToolFunc.NormalizePath(image.FullName);
                        string asset_path = PackerToolFunc.GetAssetsRelativePath(image_path);
                        string texturePackerImg_path = asset_path.Replace("tpsheet", "png");
                        string image_name = Path.GetFileNameWithoutExtension(image_path);
                        UnityEngine.Object[] imageObjs = AssetDatabase.LoadAllAssetsAtPath(texturePackerImg_path);
                        if (imageObjs != null)
                        {
                            if (imageObjs.Length > 0)
                            {
                                List<string> packerImags = new List<string>();
                                string atlasImgName = null;
                                foreach (UnityEngine.Object obj in imageObjs)
                                {
                                    if (obj is Sprite)
                                    {
                                        texturePacker_dic[obj.name] = image_name;
                                    }
                                }
                            }
                            else
                            {
                                Debug.LogError("texturePacker not find png: " + texturePackerImg_path);
                            }
                        }
                    }
                }
            }
        }

        // 所有不适合打图集的图全部单打
        foreach (KeyValuePair<string, string> image in image_dic)
        {
            bool need_atlas = PackerToolFunc.ShouldImageBuildAtlas(image.Value);
            if (!need_atlas)
            {
                PackerToolFunc.SetSpritePackGroup(image.Value, Path.GetFileNameWithoutExtension(image.Value), true);
                PackerToolFunc.SetAssetBundlePackGroup(image.Value, Path.GetFileNameWithoutExtension(image.Value));
            }
        }


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
                    GameObject goPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefab_asset_path, typeof(GameObject));
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
                                        string prefab_name = Path.GetFileNameWithoutExtension(prefab_asset_path);
                                        if (image_ref_dic.ContainsKey(asset_path))
                                        {
                                            image_ref_dic[asset_path].Add(prefab_name);
                                        }
                                        else
                                        {
                                            List<string> ref_prefabs = new List<string>();
                                            ref_prefabs.Add(prefab_name);
                                            image_ref_dic.Add(asset_path, ref_prefabs);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // 根据引用关系，设置图片的PackTag
        foreach (KeyValuePair<string, List<string>> image_ref in image_ref_dic)
        {
            string asset_path = image_ref.Key;
            string image_path = PackerToolFunc.GetFullPathFromAssetsRelativePath(asset_path);

            bool need_atlas = PackerToolFunc.ShouldImageBuildAtlas(image_path);
            if (1 < image_ref.Value.Count)
            {
                // 被多次引用的图打atals
                if (need_atlas)
                {
                    PackerToolFunc.SetSpritePackGroup(image_path, "atlas_ui_common", true);
                    PackerToolFunc.SetAssetBundlePackGroup(image_path, "atlas_ui_common");

                    if (image_dic.ContainsKey(asset_path))
                    {
                        image_dic.Remove(asset_path);
                    }
                }
            }
        }


        //
        //// 剩下的图片没有直接的Prefab引用，可以定义为动态图片
        // 
        foreach (AtlasPackInfo packInfo in atlasPackInfoList)
        {
            atlas_ab_name = packInfo.atlasAbName;
            image_atlas_dic.Clear();
            foreach (string path in packInfo.atlasDirectoryList)
            {
                image_atlas_dic.Add(path, atlas_ab_name);
            }
            PackImageDirDic(image_atlas_dic, atlas_ab_name, "", 0, 0, packInfo.isForce);
        }


        //// 
        //// 具体文件夹打包
        ////
        //// 原则是同时出现的图尽量出现同一个Atals中
        //{
        //    atlas_ab_name = "atlas_MainPanel_root_new";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/City/MainPanel_root_new", "atlas_MainPanel_root_new");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}

        //{
        //    atlas_ab_name = "atlas_mainpanel_duilie";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/City/mainpanel_duilie", "atlas_mainpanel_duilie");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}

        //{
        //    atlas_ab_name = "atlas_common_icon";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/Common/1Commmon_icon", "atlas_common_icon");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}

        //{
        //    // 道具
        //    atlas_ab_name = "atlas_daoju";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/DynamicImage/daoju", "atlas_daoju");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}

        //{
        //    // 道具品质框
        //    atlas_ab_name = "atlas_daoju_ext";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/DynamicImage/pinzhi", "atlas_daoju_ext");
        //    image_atlas_dic.Add("UI/DynamicImage/icon", "atlas_daoju_ext");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}

        //{
        //    // 道具
        //    atlas_ab_name = "atlas_daoju_new";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/DynamicImage/daoju_new", "atlas_daoju_new");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}

        //{
        //    // 道具
        //    atlas_ab_name = "atlas_daoju_new1";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/DynamicImage/daoju_new1", "atlas_daoju_new1");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}

        //{
        //    // 天赋，科技，内政
        //    atlas_ab_name = "atlas_tech";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/DynamicImage/Tech", "atlas_tech");
        //    image_atlas_dic.Add("UI/DynamicImage/neizheng", "atlas_tech");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}

        //{
        //    // 技能
        //    atlas_ab_name = "atlas_jineng";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/DynamicImage/jineng", "atlas_jineng");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}


        //{
        //    // 新技能
        //    atlas_ab_name = "atlas_jineng_new";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/DynamicImage/jineng_new", "atlas_jineng_new");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);

        //}


        //{
        //    // 官爵
        //    atlas_ab_name = "atlas_guanjue";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/DynamicImage/guanjue", "atlas_guanjue");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}

        //{
        //    // 表情
        //    atlas_ab_name = "atlas_biaoqing_sandbox_qizhi";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/DynamicImage/biaoqing", "atlas_biaoqing_sandbox_qizhi");
        //    image_atlas_dic.Add("UI/DynamicImage/biaoqingbig", "atlas_biaoqing_sandbox_qizhi");
        //    image_atlas_dic.Add("UI/DynamicImage/sandbox", "atlas_biaoqing_sandbox_qizhi");
        //    image_atlas_dic.Add("UI/DynamicImage/qizhi", "atlas_biaoqing_sandbox_qizhi");
        //    //image_atlas_dic.Add("UI/DynamicImage/xianjing", "atlas_biaoqing_sandbox_qizhi");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}

        //{
        //    // 头像
        //    atlas_ab_name = "atlas_touxiang";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/DynamicImage/touxiang", "atlas_touxiang");
        //    image_atlas_dic.Add("UI/DynamicImage/touxiang84", "atlas_touxiang");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}

        //{
        //    // 头像ext
        //    atlas_ab_name = "atlas_touxiang_ext";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/DynamicImage/touxiang_ext", "atlas_touxiang_ext");
        //    image_atlas_dic.Add("UI/DynamicImage/touxiang84_ext", "atlas_touxiang_ext");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}

        //{
        //    // 装备
        //    atlas_ab_name = "atlas_zhuangbei";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/DynamicImage/zhuangbei", "atlas_zhuangbei");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}

        //{
        //    // 君主宝物
        //    atlas_ab_name = "atlas_junzhubaowu";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/DynamicImage/junzhubaowu", "atlas_junzhubaowu");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name);
        //}

        //{
        //    // world city
        //    atlas_ab_name = "atlas_worldcity";

        //    image_atlas_dic.Clear();
        //    image_atlas_dic.Add("UI/DynamicImage/WorldCity", "atlas_worldcity");
        //    PackImageDirDic(image_atlas_dic, atlas_ab_name, "", 0, 0, true);
        //}


        // 所有不打图集的图，哪怕是可以打atals的
        // 只有必须的才打atals
        foreach (KeyValuePair<string, string> image in image_dic)
        {
            PackerToolFunc.SetSpritePackGroup(image.Value, Path.GetFileNameWithoutExtension(image.Value), true);
            PackerToolFunc.SetAssetBundlePackGroup(image.Value, Path.GetFileNameWithoutExtension(image.Value));
        }

        //// 补丁，放最后
        //{

        //}
    }

    // 按指定的Image文件夹映射表生成atlas，同时设置atlas所在的AssetBundle
    private static void PackImageDirDic(Dictionary<string, string> image_dir_atlas_dic, string atlas_ab_name,
        string default_atlas_ab_name = "", int max_w = 0, int max_h = 0, bool force = false)
    {
        foreach (KeyValuePair<string, string> item in image_dir_atlas_dic)
        {
            string dir_path = Application.dataPath + "/" + item.Key;
            if (Directory.Exists(dir_path))
            {
                string[] assets = Directory.GetFiles(dir_path, "*.*", SearchOption.AllDirectories);
                if (null != assets)
                {
                    foreach (string asset in assets)
                    {
                        string full_path = PackerToolFunc.NormalizePath(asset);
                        if ((PackerToolFunc.IsImageFile(full_path))
                            && (PackerToolFunc.ShouldImageBuildAtlas(full_path) || force))
                        {
                            /*bool need_pack = true;
                            string asset_path = PackerToolFunc.GetAssetsRelativePath(full_path);
                            if ((0 != max_h)
                                ||(0 != max_w))
                            {
                                need_pack = SizeFilter(asset_path, max_w, max_h);
                            }*/

                            string atlas_name = "";
                            string r_atlas_ab_name = "";
                            //if (need_pack)
                            {
                                atlas_name = item.Value;
                                r_atlas_ab_name = atlas_ab_name;
                            }
                            //else
                            //{
                            //    atlas_name = Path.GetFileNameWithoutExtension(full_path);
                            //    r_atlas_ab_name = default_atlas_ab_name;
                            //}

                            PackerToolFunc.SetSpritePackGroup(full_path, atlas_name, true);
                            PackerToolFunc.SetAssetBundlePackGroup(full_path, r_atlas_ab_name);

                            string asset_path = PackerToolFunc.GetAssetsRelativePath(full_path);
                            if (image_dic.ContainsKey(asset_path))
                            {
                                image_dic.Remove(asset_path);
                            }
                        }
                    }
                }
            }
        }
    }

    // 按指定的Image文件夹为所在的AssetBundle的名字，不设置atlas
    private static void PackImageDicWithoutAtlas(List<string> image_dir_list, string atlas_ab_name,
        string default_atlas_ab_name = "", int max_w = 0, int max_h = 0)
    {
        foreach (string dir in image_dir_list)
        {
            string path = Application.dataPath + "/" + dir;
            if (Directory.Exists(path))
            {
                string[] assets = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                if (null != assets)
                {
                    foreach (string asset in assets)
                    {
                        string asset_full_path = PackerToolFunc.NormalizePath(asset);
                        if (PackerToolFunc.IsImageFile(asset_full_path))
                        {
                            PackerToolFunc.SetAssetBundlePackGroup(asset_full_path, atlas_ab_name);

                            string asset_path = PackerToolFunc.GetAssetsRelativePath(asset_full_path);
                            if (image_dic.ContainsKey(asset_path))
                            {
                                image_dic.Remove(asset_path);
                            }
                        }
                    }
                }
            }
        }
    }




    // 按指定的Image映射表生成atlas，同时设置atlas所在的AssetBundle
    private static void PackImageDic(Dictionary<string, string> image_atlas_dic, string atlas_ab_name)
    {
        foreach (KeyValuePair<string, string> item in image_atlas_dic)
        {
            string asset_path = Application.dataPath + "/" + item.Key;
            PackerToolFunc.SetSpritePackGroup(asset_path, item.Value, true);
            PackerToolFunc.SetAssetBundlePackGroup(asset_path, atlas_ab_name);

            if (image_dic.ContainsKey(asset_path))
            {
                image_dic.Remove(asset_path);
            }
        }
    }

    private static bool SizeFilter(string asset_path, int max_w, int max_h)
    {
        int w = 0;
        int h = 0;
        PackerToolFunc.GetImageSize(asset_path, ref w, ref h);
        return (w <= max_w) && (h <= max_h);
    }

    // 清理所有UI图片的PackTag
    static void ClearPacktag()
    {
        List<string> image_dir = new List<string>();
        image_dir.Add("UI");
        for (int i = 0; i < image_dir.Count; ++i)
        {
            if (Directory.Exists(Application.dataPath + "/" + image_dir[i]))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath + "/" + image_dir[i]);
                FileInfo[] images = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo image in images)
                {
                    if (PackerToolFunc.IsImageFile(image.FullName))
                    {
                        string image_path = PackerToolFunc.NormalizePath(image.FullName);
                        PackerToolFunc.SetSpritePackGroup(image_path, "", true);
                    }
                }
            }
        }
    }
}
