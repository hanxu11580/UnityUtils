using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

// Usage:
//    yield return new WaitForEndOfFrame();     =>      yield return Yielders.EndOfFrame;
//    yield return new WaitForFixedUpdate();    =>      yield return Yielders.FixedUpdate;
//    yield return new WaitForSeconds(1.0f);    =>      yield return Yielders.GetWaitForSeconds(1.0f);

// http://forum.unity3d.com/threads/c-coroutine-waitforseconds-garbage-collection-tip.224878/

namespace Framework.Tools {
    public static class Yielders {
        public static bool Enabled = true;

        private static int internalCounter = 0; // counts how many times the app yields

        static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
        public static WaitForEndOfFrame EndOfFrame
        {
            get { internalCounter++; return Enabled ? _endOfFrame : new WaitForEndOfFrame(); }
        }

        static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
        public static WaitForFixedUpdate FixedUpdate
        {
            get { internalCounter++; return Enabled ? _fixedUpdate : new WaitForFixedUpdate(); }
        }

        public static WaitForSeconds GetWaitForSeconds(float seconds) {
            internalCounter++;

            if (!Enabled)
                return new WaitForSeconds(seconds);

            WaitForSeconds wfs;
            if (!_waitForSecondsYielders.TryGetValue(seconds, out wfs))
                _waitForSecondsYielders.Add(seconds, wfs = new WaitForSeconds(seconds));
            return wfs;
        }

        public static void ClearWaitForSeconds() {
            _waitForSecondsYielders.Clear();
        }

        static Dictionary<float, WaitForSeconds> _waitForSecondsYielders = new Dictionary<float, WaitForSeconds>(100, new FloatComparer());
    }
}