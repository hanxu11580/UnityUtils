using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntitiesTest.Tanks {
    public class CameraSingleton : MonoBehaviour {
        public static Camera Instance;

        void Awake() {
            Instance = GetComponent<Camera>();
        }
    }
}