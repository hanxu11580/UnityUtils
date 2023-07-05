using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace USDT.Core.Table {
    public abstract class ConfigTable<T> : IConfigTable where T : IConfig
    {
        private Dictionary<int, T> configDict = new Dictionary<int, T>();
        public Type ConfigType
        {
            get
            {
                return typeof(T);
            }
        }
        
        /// <summary>
        /// 初始化数据表
        /// </summary>
        public async void InitConfigTable() {
            var operationHandle = Addressables.LoadAssetAsync<TextAsset>($"Assets/Excels/Jsons/{ConfigType.Name}.txt");
            await operationHandle.Task;
            var list = JsonMapper.ToObject<List<T>>(operationHandle.Task.Result.text);
            foreach (T item in list)
            {
                configDict.Add(item.Id, item);
            }
            Addressables.Release(operationHandle);
        }
        public T GetConfig(int id)
        {
            if (configDict.ContainsKey(id))
            {
                return configDict[id];
            }
            else
            {
                return default;
            }
        }
        public Dictionary<int, T> GetConfigs()
        {
            return configDict;
        }
    }
}