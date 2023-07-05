using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USDT.Core {
    public interface IUpdate {
        void DoUpdate(float elapseSeconds, float realElapseSeconds);
    }

    public interface IAwake {
        void DoAwake();
    }

    public interface IStart {
        void DoStart();
    }

    public interface IDestroy {
        void DoDestroy();
    }

    public interface ILifeCycle : IUpdate, IAwake, IStart, IDestroy { }
}