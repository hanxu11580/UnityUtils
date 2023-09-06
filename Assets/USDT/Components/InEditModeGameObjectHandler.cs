using System;
using System.Collections.Generic;
using UnityEngine;

namespace USDT.Components {
	[ExecuteInEditMode]
	public sealed class InEditModeGameObjectHandler : MonoBehaviour
	{
		public static readonly string HierarchyName = nameof(InEditModeGameObjectHandler);

		public static InEditModeGameObjectHandler Instance { get; private set; }

		public event Action OnGUICallback;

		private void OnGUI()
		{
			OnGUICallback?.Invoke();
		}

		/// <summary>
		/// 初始化后才能使用
		/// </summary>
		public static void Init() {
			var obj = GameObject.Find(HierarchyName);

			if (obj == null) {
				obj = new GameObject(HierarchyName, typeof(InEditModeGameObjectHandler));
			}

			Instance = obj.GetComponent<InEditModeGameObjectHandler>();
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