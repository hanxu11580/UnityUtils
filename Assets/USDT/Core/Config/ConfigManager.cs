using System;
using System.Collections;
using System.Collections.Generic;
using USDT.Utils;

namespace USDT.Core.Table {

    public class ConfigManager : ManagerBase {
        public Hashtable configs = new Hashtable();
        public override void DoAwake() {
            base.DoAwake();
            foreach (Type item in ReflectionUtils.GetTypes()) {
                if (item.IsAbstract) continue;
                ConfigAttribute[] configAttributes = (ConfigAttribute[])item.GetCustomAttributes(TypeInfo<ConfigAttribute>.Type, false);
                if (configAttributes.Length > 0) {
                    IConfigTable iConfigTable = (IConfigTable)Activator.CreateInstance(item);
                    //初始化数据表
                    iConfigTable.InitConfigTable();
                    configs.Add(iConfigTable.ConfigType, iConfigTable);
                }
            }
        }
        public override void DoDestroy() {
            base.DoDestroy();
            configs.Clear();
        }

        public T GetConfig<T>(int id) where T : IConfig {
            //Type type = typeof(T);
            if (configs.ContainsKey(TypeInfo<T>.Type)) {
                ConfigTable<T> ConfigTable = (ConfigTable<T>)configs[TypeInfo<T>.Type];
                return ConfigTable.GetConfig(id);
            }
            else {
                LogUtils.Log($"Config不存在{TypeInfo<T>.TypeName}");
                return default;
            }
        }
        public Dictionary<int, T> GetConfigs<T>() where T : IConfig {
            //Type type = typeof(T);
            if (configs.ContainsKey(TypeInfo<T>.Type)) {
                ConfigTable<T> ConfigTable = (ConfigTable<T>)configs[TypeInfo<T>.Type];
                return ConfigTable.GetConfigs();
            }
            else {
                LogUtils.LogError($"Config不存在{TypeInfo<T>.TypeName}");
                return default;
            }
        }

    }
}