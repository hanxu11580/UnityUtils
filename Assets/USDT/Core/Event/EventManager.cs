using System;

namespace USDT.Core {
    /// <summary>
    /// 开放给外界的代理
    /// </summary>
    public class EventManager : ManagerBase {
        //实体
        private EventPool<GlobalEventArgs> m_EventPool;

        public override int Priority => 100;

        public EventManager() {
            m_EventPool = new EventPool<GlobalEventArgs>();
        }

        #region lifecycle

        public override void DoDestroy() {
            base.DoDestroy();
            m_EventPool.Shutdown();
        }

        #endregion

        /// <summary>
        /// 订阅
        /// </summary>
        public void Subscribe<T>(EventHandler<GlobalEventArgs> handler) where T: GlobalEventArgs {
            Type type = typeof(T);
            m_EventPool.Subscribe(type, handler);
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        public void Unsubscribe<T>(EventHandler<GlobalEventArgs> handler) where T : GlobalEventArgs {
            Type type = typeof(T);
            m_EventPool.Unsubscribe(type, handler);
        }

        /// <summary>
        /// 抛出
        /// </summary>
        public void Throw<T>(object sender, GlobalEventArgs e) where T : GlobalEventArgs {
            Type type = typeof(T);
            m_EventPool.Throw(type, sender, e);
        }
    }
}
