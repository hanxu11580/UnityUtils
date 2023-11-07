using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using USDT.Utils;

namespace USDT.CustomEditor.ProjectWindowDetails {

	public static class ProjectWindowDetails {
		private static readonly List<ProjectWindowDetailBase> _details = new List<ProjectWindowDetailBase>();
		private static GUIStyle _rightAlignedStyle;
		private const int SpaceBetweenColumns = 10;
        private const int MenuIconWidth = 20;
		private const string MenuIconStyle = "d_ViewToolOrbit On";
		static ProjectWindowDetails() {
			foreach (var type in GetAllDetailTypes()) {
				_details.Add((ProjectWindowDetailBase)Activator.CreateInstance(type));
			}
		}

		private static IEnumerable<Type> GetAllDetailTypes() {
			var types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (var type in types) {
				if (type.BaseType == typeof(ProjectWindowDetailBase)) {
					yield return type;
				}
			}
		}

		public static void Init() {
			EditorApplication.projectWindowItemOnGUI -= DrawAssetDetails;
			EditorApplication.projectWindowItemOnGUI += DrawAssetDetails;
		}

		public static void DrawAssetDetails(string guid, Rect rect) {
            if (!CustomSettingsBaseSO.SO.drawAssetDetails) {
				return;
            }

			if (Application.isPlaying) {
				return;
			}

			if (!IsMainListAsset(rect)) {
				return;
			}

            var isSelected = Array.IndexOf(Selection.assetGUIDs, guid) >= 0;
			// ����ע���ˣ�ͼ���������ߡ����о������ұ�
			rect.x += rect.width;
			// ���ұ�����һ������ͼ����ʾ
			rect.x -= MenuIconWidth;
			// ����ͼ����
			rect.width = MenuIconWidth;
			if (isSelected) {
                DrawMenuIcon(rect);

				// ֻ����ʾͼ�꣬�ż�����˵�
				if (Event.current.type == EventType.MouseDown &&
					Event.current.button == 0 &&
					Event.current.mousePosition.x > rect.xMax - MenuIconWidth) {
					Event.current.Use();
					ShowContextMenu(Event.current.mousePosition);
				}

				if (Event.current.type != EventType.Repaint) {
					return;
				}
			}

            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			if (AssetDatabase.IsValidFolder(assetPath)) {
				return;
			}

			var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
			if (asset == null) {
				// ����Ŀ�������ղؼл����������
				return;
            }

			for (var i = _details.Count - 1; i >= 0; i--) {
				var detail = _details[i];
				if (!detail.Visible) {
					continue;
				}

				rect.width = detail.ColumnWidth;
				rect.x -= detail.ColumnWidth + SpaceBetweenColumns;

				string label = null;
				try {
					label = detail.GetLabel(guid, assetPath, asset);
				}
				catch (Exception e) {
					lg.e(e);
					continue;
				}

				if (string.IsNullOrEmpty(label)) {
					continue;
				}

				GUI.Label(rect, new GUIContent(label, detail.Name),
					GetStyle(detail.Alignment));
			}
		}

		private static void DrawMenuIcon(Rect rect) {
			var icon = EditorGUIUtility.IconContent(MenuIconStyle);
            EditorGUI.LabelField(rect, icon);
		}

		private static GUIStyle GetStyle(TextAlignment alignment) {
			return alignment == TextAlignment.Left ? EditorStyles.label : RightAlignedStyle;
		}

		private static GUIStyle RightAlignedStyle
		{
			get {
				if (_rightAlignedStyle == null) {
					_rightAlignedStyle = new GUIStyle(EditorStyles.label);
					_rightAlignedStyle.alignment = TextAnchor.MiddleRight;
				}

				return _rightAlignedStyle;
			}
		}

		private static void ShowContextMenu(Vector2 position) {
			var menu = new GenericMenu();
			foreach (var detail in _details) {
				menu.AddItem(new GUIContent(detail.Name), detail.Visible, ToggleMenu, detail);
			}
			menu.AddSeparator("");
			menu.AddSeparator("");
			menu.AddItem(new GUIContent("None"), false, HideAllDetails);
			menu.AddSeparator("");
			menu.AddSeparator("");
			menu.AddItem(new GUIContent("ClearLabelCache"), false, ClearLabelCache);
			menu.DropDown(new Rect(position, Vector2.zero));
        }

		private static void HideAllDetails() {
			foreach (var detail in _details) {
				detail.Visible = false;
			}
		}

		private static void ToggleMenu(object data) {
			var detail = (ProjectWindowDetailBase)data;
			detail.Visible = !detail.Visible;
		}

		private static bool IsMainListAsset(Rect rect) {
			// �����Ŀ��ͼ��ʾ��Ԥ��ͼ�꣬�򲻻���ϸ��
			if (rect.height > 20) {
				return false;
			}

			// ������ʲ������ʲ����򲻻�����ϸ��Ϣ
			if (rect.x > 16) {
				return false;
			}

			return true;
		}

		private static void ClearLabelCache() {
            foreach (var detail in _details) {
				if(detail == null) {
					continue;
                }
				detail.ClearLabelCache();
            }
        }

		[MenuItem("Assets/ProjectWindowDetails", false)]
		private static void ShowContextMenu() {
			ShowContextMenu(Vector2.zero);

		}
	}
}
