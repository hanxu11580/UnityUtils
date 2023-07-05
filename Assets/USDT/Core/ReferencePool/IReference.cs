namespace USDT.Core {
    /// <summary>
    /// 继承可以后 可以生成并回收 且自动调用Reset
    /// </summary>
    public interface IReference
    {
        void ClearRef();
    }
}
