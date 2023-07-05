using System;
using System.Collections.Generic;
using UnityEngine;


namespace USDT.Core {
    /// <summary>
    /// 事件消息类管理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventPool<T>where T:GlobalEventArgs
    {
        /// <summary>
        /// 事件
        /// </summary>
        private class Event
        {
            public object Sender { get; private set; }
            public T EventArgs { get; private set; }
            public Event(object sender, T eventArgs)
            {
                Sender = sender;
                EventArgs = eventArgs;
            }
        }

        private Dictionary<Type, EventHandler<T>> m_EventHandlers;

        public EventPool()
        {
            m_EventHandlers = new Dictionary<Type, EventHandler<T>>();
        }



        /// <summary>
        /// 检查订阅事件处理方法是否存在
        /// </summary>
        public bool Check(Type type, EventHandler<T> handler)
        {
            if (handler == null)
            {
                Debug.LogError("事件处理方法为空");
                return false;
            }

            if (!m_EventHandlers.TryGetValue(type, out EventHandler<T> handlers))
            {
                return false;
            }

            //遍历委托里的所有方法
            foreach (EventHandler<T> i in handlers.GetInvocationList())
            {
                if (i == handler)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        public void Subscribe(Type type, EventHandler<T> handler)
        {
            if (handler == null)
            {
                Debug.LogError("事件处理方法为空，无法订阅");
                return;
            }

            //检查是否获取处理方法失败或获取到的为空
            if (!m_EventHandlers.TryGetValue(type, out EventHandler<T> eventHandler) 
                || eventHandler == null)
            {
                m_EventHandlers[type] = handler;
            }
            else if (Check(type, handler))
            {
                Debug.LogError("要订阅事件的处理方法已存在");
            }
            else
            {
                eventHandler += handler;
                m_EventHandlers[type] = eventHandler;
            }
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        public void Unsubscribe(Type type, EventHandler<T> handler)
        {
            if (handler == null)
            {
                Debug.LogError("事件处理方法为空，无法取消订阅");
                return;
            }

            if (m_EventHandlers.ContainsKey(type))
            {
                m_EventHandlers[type] -= handler;
            }
        }

        /// <summary>
        /// 抛出事件
        /// </summary>
        public void Throw(Type type, object sender, T e)
        {
            //尝试获取事件的处理方法
            if (m_EventHandlers.TryGetValue(type, out EventHandler<T> handlers))
            {
                handlers?.Invoke(sender, e);
            }
            // 向引用池归还事件引用
            ReferencePool.Release(e);
        }

        /// <summary>
        /// 关闭并清理事件池。
        /// </summary>
        public void Shutdown()
        {
            m_EventHandlers.Clear();
        }
    }
}
