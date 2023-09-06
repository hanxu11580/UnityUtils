﻿using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using USDT.Components;
using USDT.CustomEditor;
using USDT.Utils;

namespace USDT.CustomEditor.IPhoneXSafeAreaDrawerEditor {
	public static class IPhoneXSafeAreaDrawer {
		private static Texture _portraitImage;
		private static Texture _landscapeImage;
		private static CustomSettingsBaseSO _customSettingsBaseSO;
		private static Texture _PortraitImage {
			get {
				if ( _portraitImage == null ) {
					_portraitImage = AssetDatabase.LoadAssetAtPath<Texture>("Assets/USDT/Editor/iPhoneXSafeAreaDrawer/Editor/iPhoneXSafeAreaDrawer-portrait.png");
				}
				return _portraitImage;
			}
		}

		private static Texture _LandscapeImage {
			get {
				if ( _landscapeImage == null ) {
					_landscapeImage = AssetDatabase.LoadAssetAtPath<Texture>("Assets/USDT/Editor/iPhoneXSafeAreaDrawer/Editor/iPhoneXSafeAreaDrawer-landscape.png");
				}
				return _landscapeImage;
			}
		}

		public static void Init() {
			_customSettingsBaseSO = CustomSettingsBaseSO.GetOrCreateSettings();
			InEditModeGameObjectHandler.Instance.OnGUICallback -= OnGUI;
			InEditModeGameObjectHandler.Instance.OnGUICallback += OnGUI;
		}

        private static void OnGUI() {
			if (!_customSettingsBaseSO.drawIPhoneXSafeArea) {
                return;
            }

            var v2 = Handles.GetMainGameViewSize();
			var aspect = v2.x / v2.y;

			var img = aspect < 1 ? _PortraitImage : _LandscapeImage;
			var pos = new Rect( 0, 0, Screen.width, Screen.height );
			GUI.DrawTexture( pos, img );
		}
	}
}