using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

public class MetaModifer  {

    // 修改指定Meta的指定字段. 
    // 注意该函数不包含刷新，批量刷新效率更高
	public static void ModifyMetaKey(string meta,string key,string val)
    {
        StreamReader metaReader = null;
        try
        {
            File.SetAttributes(meta, FileAttributes.Normal);
            metaReader = File.OpenText(meta);
        }
        catch (Exception) { }

        if (metaReader != null)
        {
            string metaTxt = metaReader.ReadToEnd();
            if (null != metaTxt)
            {   
                metaReader.Close();
                int begin = metaTxt.IndexOf(key + ":");
                if (0 <= begin) 
                {   
                    int end = metaTxt.IndexOf("\n",begin);
                    string real_key = metaTxt.Substring(begin,end - begin);
                    if (real_key.ToLower() != (key + ": " + val).ToLower())
                    {
                        metaTxt = metaTxt.Replace(real_key, key + ": " + val);

                        StreamWriter metaWrite = null;
                        try
                        {
                            metaWrite = File.CreateText(meta);
                        }
                        catch (Exception) { }
                        if (null != metaWrite)
                        {
                            metaWrite.Write(metaTxt);
                            metaWrite.Close();
                        }
                    }
                }
            }
        }
    }
}
