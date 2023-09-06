using System;
using System.Collections.Generic;
using UnityEngine;
using USDT.Core;

namespace USDT.Components {
	[ExecuteInEditMode]
	public sealed class InEditModeGameObjectHandler : SingletonMono<InEditModeGameObjectHandler> {

		public event Action OnGUICallback;

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