using System;

namespace USDT.Core {
    public abstract class GlobalEventArgs : EventArgs, IReference
    {
        /// <summary>
        /// 事件类型ID
        /// </summary>
        //public abstract int Id { get; }
        
        /// <summary>
        /// 事件发出后 回收执行
        /// </summary>
        public virtual void ClearRef()
        {
            
        }
    }
}
