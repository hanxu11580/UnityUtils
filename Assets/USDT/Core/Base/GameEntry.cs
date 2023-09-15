using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USDT.Utils;

namespace USDT.Core {
    /// <summary>
    /// 模块入口
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class GameEntry : SingletonMono<GameEntry>
    {
        private readonly static LinkedList<ManagerBase> _managers = new LinkedList<ManagerBase>();

        #region lifecycle
        private void Awake() {
            var subManagerBaseTypes = ReflectionUtils.GetSubTypes(typeof(ManagerBase));
            if(subManagerBaseTypes != null && subManagerBaseTypes.Length != 0) {
                foreach (Type mgrType in subManagerBaseTypes) {
                    CreateManager(mgrType);
                }
            }
            else {
                lg.i("没有继承ManagerBase的类");
            }

            // 根据优先级执行
            foreach (var mgr in _managers) {
                mgr.DoAwake();
            }
        }

        private void Start() {
            // 根据优先级执行
            foreach (var mgr in _managers) {
                mgr.DoStart();
            }
        }

        private void Update() {
            foreach (ManagerBase manager in _managers) {
                manager.DoUpdate(Time.deltaTime, Time.unscaledDeltaTime);
            }
        }

        protected override void OnDestroy() {
            for (LinkedListNode<ManagerBase> current = _managers.Last; current != null; current = current.Previous) {
                current.Value.DoDestroy();
            }
            _managers.Clear();
        }

        #endregion

        /// <summary>
        /// 获取对应管理器
        /// </summary>
        /// <typeparam name="T">ManagerBase派生类</typeparam>
        /// <returns></returns>
        public static T GetManager<T>() where T : ManagerBase {
            Type managerType = typeof(T);
            foreach (ManagerBase manager in _managers)
            {
                if (manager.GetType() == managerType)
                {
                    return manager as T;
                }
            }
            throw new Exception($"获取未创建的Manager，请检查是否自动创建");
        }

        /// <summary>
        /// 创建模块实例
        /// </summary>
        /// <param name="managerType">模块类型</param>
        /// <returns></returns>
        private static ManagerBase CreateManager(Type managerType)
        {
            ManagerBase manager = Activator.CreateInstance(managerType) as ManagerBase;
            if (manager == null)
            {
                lg.e("模块" + manager.GetType().FullName + "创建失败");
            }

            //根据优先级放入链表
            LinkedListNode<ManagerBase> current = _managers.First;
            while (current != null)
            {
                if (manager.Priority < current.Value.Priority)
                {
                    break;
                }
                current = current.Next;
            }

            if (current != null)
            {
                _managers.AddBefore(current, manager);
            }
            else
            {
                _managers.AddLast(manager);
            }
            lg.i($"创建{managerType.FullName}");
            return manager;
        }
    }
}
