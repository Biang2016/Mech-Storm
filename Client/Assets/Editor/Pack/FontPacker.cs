using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public class FontPacker : MonoBehaviour {

    // 字体AB的配置
    static string ab_name = "golden_font";
    static string ab_ext = "ab";

    // 路径是以Asset为根目录
    static List<string> packer_dir_lst = new List<string>();
    static List<string> single_packer_dir_lst = new List<string>();

    //[MenuItem("AssetBundle/Packer/FontPacker")]
	public static void SetFontPackGroup()
    {   
        InitPacker();

        for(int i = 0; i < packer_dir_lst.Count; ++i)
        {
            string dir = Application.dataPath + "/" + packer_dir_lst[i];
            PackerToolFunc.SetDirAssetsBundlePackGroup(dir, ASSET_TYPE.ASSET_TYPE_ALL, ab_name, ab_ext, "",single_packer_dir_lst);
        }

        for (int i = 0; i < single_packer_dir_lst.Count; ++i)
        {
            string fontPath = Application.dataPath + "/" + single_packer_dir_lst[i];
            PackerToolFunc.SetAssetBundlePackGroup(fontPath, Path.GetFileNameWithoutExtension(single_packer_dir_lst[i]), ab_ext);
        }
    }

    static void InitPacker()
    {
        packer_dir_lst.Clear();
        packer_dir_lst.Add("Font");

        single_packer_dir_lst.Clear();
        //single_packer_dir_lst.Add("Font/HAN_YI_XIAO_LI_SHU_JIAN.ttf");
        //single_packer_dir_lst.Add("Font/fang_zheng_lan_ting_zhun_hei.ttf");
    }
}
