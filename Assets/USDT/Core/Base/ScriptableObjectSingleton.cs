using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USDT.Core {
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject {
        private static T _Instance = null;
        public static T Instance
        {
            get {
                if (_Instance == null)
                    _Instance = Resources.Load<T>(typeof(T).Name);

                return _Instance;
            }
        }

        /// <summary>
        /// 确保只会初始化一次
        /// </summary>
        protected virtual void Initialize() { }

        private void OnEnable() {
            if (InitializeCalled)
                return;

            InitializeCalled = true;
            Initialize();
        }
        static bool InitializeCalled = false;
    }
}