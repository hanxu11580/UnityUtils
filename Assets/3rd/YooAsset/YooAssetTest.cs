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

            // 编辑器模式
            //var initParameters = new EditorSimulateModeParameters();
            //initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild("MainPackage");
            //yield return _package.InitializeAsync(initParameters);

            // 单机运行模式
            // 需要构建ab包，并且构建时，还需要选择拷贝到首包内
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