using System;


namespace LockstepDemo {

    /// <summary>
    /// 所有非Mono单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T : class
    {
        private static T _instance;
        private static readonly object SyncObject = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null) //没有第一重 singleton == null 的话，每一次有线程进入 GetInstance()时，均会执行锁定操作来实现线程同步，
                    //非常耗费性能 增加第一重singleton ==null 成立时的情况下执行一次锁定以实现线程同步
                {
                    lock (SyncObject)
                    {
                        if (_instance == null) //Double-Check Locking 双重检查锁定
                        {
                            _instance = (T) Activator.CreateInstance(typeof(T), true);
                        }
                    }
                }

                return _instance;
            }
        }
    }
}
