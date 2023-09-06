using System;
using UnityEditor;

namespace USDT.CustomEditor {
    public interface ICondition
    {
        bool Result(EditorCoroutine _coroutine);
    }

    public class WaitForSecondsE : ICondition
    {
        readonly float time;

        public WaitForSecondsE(float _time) { time = _time; }

        public bool Result(EditorCoroutine _coroutine)
        {
            return EditorApplication.timeSinceStartup >= _coroutine.TimeSinceStartup + time;
        }
    }

    public class WaitUntilE : ICondition
    {
        readonly Func<bool> predicate;

        public WaitUntilE(Func<bool> _predicate) { predicate = _predicate; }

        public bool Result(EditorCoroutine _coroutine)
        {
            return predicate();
        }
    }
}