using UnityEngine;
using UnityEditor;
using System.Collections;

public class AssetBundlePacker_UI 
{
    // 初始化Packer
    public static void InitPacker()
    {

    }

    // 打包Scene特有的资源
    public static void Pack()
    {
        // 生成ATLAS
        EditorUtility.DisplayProgressBar("AssetBundleMaker", "正在生成Atals打包配置", 1.0f);
        AtlasPacker.SetAtlasPackGroup();

        // UI资源分组
        EditorUtility.DisplayProgressBar("AssetBundleMaker", "正在设置UI资源打包配置", 1.0f);
        UIPacker.SetUIPackGroup();
    }
}
