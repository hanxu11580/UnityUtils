﻿using UnityEngine;
using System.Collections;
using System.Text;
using System;
/*
 * gameLoadType 为 Resource 时 ，所有资源从Resource读取
 * gameLoadType 不为 Resource时，资源读取方式从配置中读取
 * */
public static class ResourceManager 
{
    /// <summary>
    /// 游戏内资源读取类型
    /// </summary>
    public static ResLoadLocation m_gameLoadType = ResLoadLocation.Resource; //默认从resourcePath中读取

    public static ResLoadLocation GetLoadType(ResLoadLocation loadType)
    {
        //如果设置从Resource中加载则忽略打包设置
        if (m_gameLoadType == ResLoadLocation.Resource)
        {
            return ResLoadLocation.Resource;
        }

        return loadType;
    }

    //读取一个文本
    public static string ReadTextFile(string textName)
    {
        TextAsset text = Load<TextAsset>(textName);

        if (text == null)
        {
            throw new Exception("ReadTextFile not find " + textName);
        }
        else
        {
            return text.text;
        }
    }

    ////保存一个文本
    //public static void WriteTextFile(string path,string content ,ResLoadLocation type)
    //{
    //    #if UNITY_EDITOR
    //        ResourceIOTool.WriteStringByFile(PathTool.GetAbsolutePath(type, path), content);
    //    #else
            
    //    #endif
    //}

    public static object Load(string name)
    {
        ResourcesConfig packData  = ResourcesConfigManager.GetBundleConfig(name);

        if(packData == null)
        {
            throw new Exception("Load Exception not find " + name);
        }

        if (m_gameLoadType == ResLoadLocation.Resource)
        {
            return Resources.Load(packData.path);
        }
        else
        {
            return AssetsBundleManager.Load(name);
        }
    }

    public static T Load<T>(string name) where T: UnityEngine.Object
    {
        ResourcesConfig packData = ResourcesConfigManager.GetBundleConfig(name);

        if (packData == null)
        {
            throw new Exception("Load Exception not find " + name);
        }

        if (m_gameLoadType == ResLoadLocation.Resource)
        {
            return Resources.Load<T>(packData.path);
        }
        else
        {
            return AssetsBundleManager.Load<T>(name);
        }
    }

    public static void LoadAsync(string name,LoadCallBack callBack)
    {
        ResourcesConfig packData  = ResourcesConfigManager.GetBundleConfig(name);

        if (packData == null)
        {
            return ;
        }

        if (m_gameLoadType == ResLoadLocation.Resource)
        {
            ResourceIOTool.ResourceLoadAsync(packData.path, callBack);
        }
        else
        {
            AssetsBundleManager.LoadAsync(name,callBack);
        }
    }

    public static void UnLoad(string name)
    {
        if (m_gameLoadType == ResLoadLocation.Resource)
        {

        }
        else
        {
            AssetsBundleManager.UnLoadBundle(name);
        }
    }

    //public static T GetResource<T>(string path)
    //{
    //    T resouce = new T();

    //    return resouce;
    //}
}

public enum ResLoadLocation
{
    Resource,
    Streaming,
    Persistent,
    Catch,
}



