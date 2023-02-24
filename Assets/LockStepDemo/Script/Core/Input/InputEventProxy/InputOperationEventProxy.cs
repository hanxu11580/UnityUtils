﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InputOperationEventProxy : IInputProxyBase
{

    static List<IInputOperationEventCreater> s_creates = new List<IInputOperationEventCreater>();

    public static void Init()
    {
        ApplicationManager.s_OnApplicationUpdate += Update;
    }

    /// <summary>
    /// 创建一个EventCreater，重复加载直接报错
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IInputOperationEventCreater LoadEventCreater<T>() where T : IInputOperationEventCreater, new()
    {
        for (int i = 0; i < s_creates.Count; i++)
        {
            if (s_creates[i] is T)
            {
                throw new Exception(typeof(T).Name + " Creater has Exits!");
            }
        }

        IInputOperationEventCreater creater = new T();
        s_creates.Add(creater);

        return creater;
    }

    public static void UnLoadEventCreater<T>() where T : IInputOperationEventCreater , new()
    {
        for (int i = 0; i < s_creates.Count; i++)
        {
            if(s_creates[i] is T)
            {
                s_creates.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 每帧调用IInputOperationEventCreater.EventTriggerLogic
    /// </summary>
    public static void Update()
    {
        if(IsActive)
        {
            for (int i = 0; i < s_creates.Count; i++)
            {
                try
                {
                    s_creates[i].EventTriggerLogic();
                }
                catch(Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }
    }


}
