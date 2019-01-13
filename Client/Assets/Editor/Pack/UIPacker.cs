using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

// UIPanel / FX / ANIM
public class UIPacker : MonoBehaviour {

    // AB的配置
    static string ab_ext = "ab";
    static string fx_mat_ab_name = "ui_fx_mat";
    static string fx_fbx_ab_name = "ui_fx_fbx";

    // 路径是以Asset为根目录
    static List<string> panel_packer_dir_lst = new List<string>();
    static List<string> fx_packer_dir_lst = new List<string>();
    static List<string> anim_packer_dir_lst = new List<string>();
    static List<string> anim_single_packer_dir_lst = new List<string>();

    [MenuItem("AssetBundle/Packer/UIPacker")]
    public static void SetUIPackGroup()
    {
        InitPacker();

        SetUIPanelPackGroup();
        SetUIFxPackGroup();
        SetUIAnimPackGroup();
    }

    static void SetUIPanelPackGroup()
    {
        for (int i = 0; i < panel_packer_dir_lst.Count; ++i)
        {
            string dir = Application.dataPath + "/" + panel_packer_dir_lst[i];

            // Prefab
            PackerToolFunc.SetDirAssetsBundlePackGroup(dir, ASSET_TYPE.ASSET_TYPE_PREFAB, "", ab_ext);
        }
    }

    // Fx
    // Tex一个包，Mat一个包，Prefab一个包,其他全部打入Prefab
    static void SetUIFxPackGroup()
    {   
        // Prefab
        for (int i = 0; i < fx_packer_dir_lst.Count; ++i)
        {
            string dir = Application.dataPath + "/" + fx_packer_dir_lst[i];

            // Prefab
            PackerToolFunc.SetDirAssetsBundlePackGroup(dir, ASSET_TYPE.ASSET_TYPE_PREFAB, "", ab_ext);

            // 图片
            PackerToolFunc.SetDirAssetsBundlePackGroup(dir, ASSET_TYPE.ASSET_TYPE_IMAGE, "", ab_ext,"img");

            // 材质
            PackerToolFunc.SetDirAssetsBundlePackGroup(dir, ASSET_TYPE.ASSET_TYPE_MAT, "", ab_ext,"mat");
        }
    }

    // 公用的动画单打，其他的打入Prefab
    static void SetUIAnimPackGroup()
    {   
        // 非公用目录打入Prefab
        for (int i = 0; i < anim_packer_dir_lst.Count; ++i)
        {
            string dir = Application.dataPath + "/" + anim_packer_dir_lst[i];
           
            // Prefab
            PackerToolFunc.SetDirAssetsBundlePackGroup(dir, ASSET_TYPE.ASSET_TYPE_PREFAB, "", ab_ext, "",anim_single_packer_dir_lst);
        }

        // 公用目录单打
        for (int i = 0; i < anim_single_packer_dir_lst.Count; ++i)
        {
            string dir = Application.dataPath + "/" + anim_single_packer_dir_lst[i];
            
            // 动画Controller
            PackerToolFunc.SetDirAssetsBundlePackGroup(dir, ASSET_TYPE.ASSET_TYPE_ANIM_CONTROLLER, "ui_ani_common", ab_ext);
        }
    }

    static void InitPacker()
    {
        panel_packer_dir_lst.Clear();
        panel_packer_dir_lst.Add("Resources/UIPanel");

        fx_packer_dir_lst.Clear();
        fx_packer_dir_lst.Add("Resources/UI_FX");

        anim_packer_dir_lst.Clear();
        anim_packer_dir_lst.Add("Resources/UI_ani");

        anim_single_packer_dir_lst.Clear();
        anim_single_packer_dir_lst.Add("Resources/UI_ani/Common");
    }
}
