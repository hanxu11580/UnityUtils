using System;
using UnityEngine;

namespace LockstepDemo {

    public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        protected static T m_Instance = null;
        
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    //场景中找_单例脚本
                    m_Instance = FindObjectOfType<T>();
                    if (FindObjectsOfType<T>().Length > 1)
                    {
                        //找出来多个
                        //Debug.LogWarning("More than 1");
                        return m_Instance;
                    }

                    if (m_Instance == null)
                    {
                        var instanceName = typeof(T).Name;
                        var instanceObj = GameObject.Find(instanceName);
                        if (!instanceObj)
                            instanceObj = new GameObject(instanceName);
                        m_Instance = instanceObj.AddComponent<T>();
                    }

                    DontDestroyOnLoad(m_Instance.gameObject); //保证实例例不不会被释放
                }

                return m_Instance;
            }
        }

        protected virtual void OnDestroy()
        {
            m_Instance = null;
        }
    }
}
