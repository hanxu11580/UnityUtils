using Juce.ImplementationSelector.Example2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USDT.Utils;

namespace YooAsset {
    public class TestLogger : ILogger {
        public void Error(string message) {
            lg.e(message);
        }

        public void Exception(Exception exception) {
            throw exception;
        }

        public void Log(string message) {
            lg.i(message);
        }

        public void Warning(string message) {
            Debug.LogWarning(message);
        }
    }

    public class YooAssetTest : MonoBehaviour {
        private IEnumerator Start() {
            YooAssets.Initialize(new TestLogger());
            var package = YooAssets.CreatePackage("MainPackage");
            YooAssets.SetDefaultPackage(package);

            // �༭��ģʽ
            //var initParameters = new EditorSimulateModeParameters();
            //initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild("MainPackage");
            //yield return _package.InitializeAsync(initParameters);

            // ��������ģʽ
            // ��Ҫ����ab�������ҹ���ʱ������Ҫѡ�񿽱����װ���
            var initParameters = new OfflinePlayModeParameters();
            yield return package.InitializeAsync(initParameters);
        }

        private async void OnGUI() {
            if (GUILayout.Button("Load")) {
                // 
                var handle = YooAssets.LoadAssetAsync<FoodSO>("Assets/3rd/Juce-ImplementationSelector-1.0.4/Examples/Scripts/InterfaceImplementation/Example2/FoodSO.asset");
                await handle.Task;
                var so = handle.AssetObject as FoodSO;


                foreach (var item in so.foods) {
                    item.Print();
                }

                handle.Release();


            }
        }
    }
}