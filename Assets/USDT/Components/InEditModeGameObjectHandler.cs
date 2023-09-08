using System;
using System.Collections.Generic;
using UnityEngine;
using USDT.Core;

namespace USDT.Components {
	[ExecuteInEditMode]
	public sealed class InEditModeGameObjectHandler : MonoBehaviour {

		public event Action OnGUICallback;

		// 这边不能用SingletonMono, 会添加多个InEditModeGameObjectHandler组件
		private static InEditModeGameObjectHandler _instance;
		public static InEditModeGameObjectHandler Instance
        {
            get {
				if(_instance == null) {
                    var go = GameObject.Find(nameof(InEditModeGameObjectHandler));
					if(go == null) {
						go = new GameObject(nameof(InEditModeGameObjectHandler), typeof(InEditModeGameObjectHandler));
						go.hideFlags = HideFlags.DontSave;
					}
					_instance = go.GetComponent<InEditModeGameObjectHandler>();
                }
				return _instance;
            }
        }

        private void OnGUI() {
			OnGUICallback?.Invoke();
		}

		public T GetComp<T>() where T : Component {
            var comp = GetComponent<T>();
			if(comp == null) {
				comp = gameObject.AddComponent<T>();
			}
			return comp;
        }
    }
}