namespace USDT.Core {
    public abstract class ManagerBase : ILifeCycle
    {
        /// <summary> 优先级 </summary>
        public virtual int Priority => 0;

        public virtual void DoAwake() { }
        public virtual void DoStart() { }
        public virtual void DoUpdate(float elapseSeconds, float realElapseSeconds) { }
        public virtual void DoDestroy() { }
    }
}
