using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USDT.Utils;

namespace USDT.Test {
    public class Test : MonoBehaviour {
        IUnityCallBack _unityCallBack;

        void Start() {
#if UNITY_ANDROID
            _unityCallBack = new IUnityCallBack();
            var _ajo = new AndroidJavaObject("com.example.test1.AndroidCallBack");
            _ajo.Call("setAndroidCallBack", _unityCallBack);
            _ajo.Call("Execute", 1, 2);
#endif
        }
    }

    public class IUnityCallBack : AndroidJavaProxy {
        public IUnityCallBack() : base("com.example.test1.IUnityCallBack") {
        }

        public void getResult(int result) {
            Debug.LogError($"Android计算结果为:{result}");
        }
    }
}