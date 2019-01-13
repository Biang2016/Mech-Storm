using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine.UI;

public class ShaderPacker : MonoBehaviour
{
    static string ab_name = "allshader";
    static string ab_ext = "ab";
    static string builtin_shader_path = Application.dataPath + "/builtin_shaders";
    static List<string> packer_dir_lst = new List<string>();
    static List<string> total_shaders = new List<string>();
    static Dictionary<string, string> ref_shader_mapper = new Dictionary<string, string>();

    // 设置Shader的Packer Tag
    [MenuItem("AssetBundle/Packer/ShaderPacker")]
    public static void SetShaderPackGroup()
    {
        total_shaders.Clear();
        ref_shader_mapper.Clear();

        // 刷新Shader引用
        UpdateBuiltinShader();

        // 设置Shader打包分组,全工程
        InitPacker();
        for (int i = 0; i < total_shaders.Count; ++i)
        {
            string shader_path = PackerToolFunc.GetFullPathFromAssetsRelativePath(total_shaders[i]);

            // 不在引用列表中的Shader要删除掉,CGINC默认引用
            if ((!PackerToolFunc.IsShaderIncludeFile(total_shaders[i]))
                && (!ref_shader_mapper.ContainsValue(total_shaders[i])))
            {
                //PackerToolFunc.UnityDeleteFile(shader_path);
            }
            else
            {
                PackerToolFunc.SetAssetBundlePackGroup(shader_path, ab_name, ab_ext);
            }
        }
    }

    // 此处写入所有参与打包的目录
    static void InitPacker()
    {
        packer_dir_lst.Clear();
    }

    static void UpdateBuiltinShader()
    {
        Dictionary<string, string> shader_mapper = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        InitBuiltinShader(ref shader_mapper);

        string[] assets = AssetDatabase.GetAllAssetPaths();
        for (int i = 0; i < assets.Length; ++i)
        {
            string asset = assets[i];
            if (!PackerToolFunc.IsAssetFile(asset))
            {
                continue;
            }

            // 顺路搜集所有的Shader
            if ((PackerToolFunc.IsShaderFile(asset))
                || (PackerToolFunc.IsShaderIncludeFile(asset)))
            {
                total_shaders.Add(asset);

                // 如果非Bulitin目录，则默认引用
                if ((PackerToolFunc.IsShaderFile(asset))
                    && (asset.IndexOf("/builtin_shaders/") < 0))
                {
                    Shader shader = AssetDatabase.LoadAssetAtPath(asset, typeof(Shader)) as Shader;
                    if (!ref_shader_mapper.ContainsKey(shader.name))
                    {
                        ref_shader_mapper.Add(shader.name, asset);
                    }
                }
            }
            else
                if (PackerToolFunc.IsMatFile(asset))
                {
                    // 材质,如果用的是内置Shader，则刷新引用到builtin_shaders目录
                    Material mat = AssetDatabase.LoadAssetAtPath(asset, typeof(Material)) as Material;
                    if ((null != mat)
                        && (null != mat.shader))
                    {
                        UpdateMaterialShader(mat, shader_mapper);
                        //EditorUtility.SetDirty(mat);
                    }
                }
                else
                    if (PackerToolFunc.IsPrefabFile(asset))
                    {
                        // GameObject
                        GameObject goAsset = AssetDatabase.LoadAssetAtPath(asset, typeof(GameObject)) as GameObject;
                        if (null != goAsset)
                        {
                            bool dirty = false;

                            // 处理Renderer材质
                            Renderer[] renders = goAsset.GetComponentsInChildren<Renderer>(true);
                            foreach (Renderer render in renders)
                            {
                                if ((render != null)
                                    && (null != render.sharedMaterials))
                                {
                                    foreach (Material mat in render.sharedMaterials)
                                    {
                                        dirty |= UpdateMaterialShader(mat, shader_mapper);
                                    }
                                }
                            }

                            // 处理UI材质
                            Graphic[] graphics = goAsset.GetComponentsInChildren<Graphic>(true);
                            foreach (Graphic graphic in graphics)
                            {
                                if (graphic != null)
                                {
                                    if (null != graphic.material)
                                    {
                                        dirty |= UpdateMaterialShader(graphic.material, shader_mapper);
                                    }
                                    //if (null != graphic.materialForRendering)
                                    //{
                                    //    UpdateMaterialShader(graphic.materialForRendering, shader_mapper);
                                    //}
                                    if (null != graphic.defaultMaterial)
                                    {
                                        dirty |= UpdateMaterialShader(graphic.defaultMaterial, shader_mapper);
                                    }
                                }
                            }
                            if (dirty)
                            {
                                EditorUtility.SetDirty(goAsset);
                            }
                        }
                    }
        }
        AssetDatabase.SaveAssets();
    }

    static void InitBuiltinShader(ref Dictionary<string, string> shader_mapper)
    {
        shader_mapper.Clear();
        if (Directory.Exists(builtin_shader_path))
        {
            DirectoryInfo dirInfo = new DirectoryInfo(builtin_shader_path);
            FileInfo[] shaders = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);
            for (int i = 0; i < shaders.Length; ++i)
            {
                string shader_path = PackerToolFunc.GetAssetsRelativePath(PackerToolFunc.NormalizePath(shaders[i].FullName));

                if (PackerToolFunc.IsShaderFile(shader_path))
                {
                    Shader shader = AssetDatabase.LoadAssetAtPath(shader_path, typeof(Shader)) as Shader;
                    if (null != shader)
                    {
                        if (!shader_mapper.ContainsKey(shader.name))
                        {
                            shader_mapper.Add(shader.name, shader_path);
                        }
                        else
                        {
                            Debug.LogError("Same Shader Found ---> " + shader.name + "," + shader_path);
                            shader_mapper[shader.name] = shader_path;
                        }
                    }
                }
            }
        }
    }

    // 判断指定Shader是否是内置Shader
    static bool IsShaderBuiltin(Shader shader)
    {
        bool bulitin = false;
        if (null != shader)
        {
            string shader_path = AssetDatabase.GetAssetPath(shader);
            bulitin = 0 <= shader_path.ToUpper().IndexOf("Resources/unity_builtin_extra".ToUpper());
            bulitin |= 0 <= shader_path.ToUpper().IndexOf("Assets/builtin_shaders".ToUpper());
        }
        return bulitin;
    }

    // 材质是否是内置材质
    static bool IsMaterialBuiltin(Material mat)
    {
        bool bulitin = false;
        if (null != mat)
        {
            string mat_path = AssetDatabase.GetAssetPath(mat);
            bulitin = 0 <= mat_path.ToUpper().IndexOf("Resources/unity_builtin_extra".ToUpper());
        }
        return bulitin;
    }


    static bool UpdateMaterialShader(Material mat, Dictionary<string, string> shader_mapper)
    {
        bool dirty = false;
        if ((null != mat)
            && (null != mat.shader))
        {
            if (!IsMaterialBuiltin(mat))
            {
                if (IsShaderBuiltin(mat.shader))
                {
                    if (shader_mapper.ContainsKey(mat.shader.name))
                    {
                        // 该Shader被引用了
                        if (!ref_shader_mapper.ContainsKey(mat.shader.name))
                        {
                            ref_shader_mapper.Add(mat.shader.name, shader_mapper[mat.shader.name]);
                        }

                        Shader new_shader = AssetDatabase.LoadAssetAtPath(shader_mapper[mat.shader.name], typeof(Shader)) as Shader;
                        if ((null != new_shader)
                            && (new_shader != mat.shader))
                        {
                            dirty = true;
                            mat.shader = new_shader;
                            EditorUtility.SetDirty(mat);
                        }
                    }
                }
            }
        }
        return dirty;
    }
}
