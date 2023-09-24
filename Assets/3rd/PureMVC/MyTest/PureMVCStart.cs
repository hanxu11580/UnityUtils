using PureMVC.Patterns.Facade;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace PureMVC.MyTest {

    public class PureMVCStart : MonoBehaviour {

        public const string Msg_ClickAddScoreButton = nameof(Msg_ClickAddScoreButton);
        public const string Msg_AddScore = nameof(Msg_AddScore);

        [SerializeField] private Transform _uiRoot;


        private IEnumerator Start() {
            var packageName = "MainPackage";
            YooAssets.Initialize();
            var package = YooAssets.CreatePackage(packageName);
            YooAssets.SetDefaultPackage(package);

            // ±à¼­Æ÷Ä£Ê½
            var initParameters = new EditorSimulateModeParameters();
            initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(packageName);
            yield return package.InitializeAsync(initParameters);

            var handle = YooAssets.LoadAssetAsync<GameObject>("Assets/3rd/PureMVC/MyTest/Panel.prefab");
            yield return handle;

            if (handle.IsDone) {
                var prefab = (handle.AssetObject as GameObject);
                var veiwGameObject = GameObject.Instantiate(prefab, _uiRoot);
                var comp = veiwGameObject.GetComponent<UIPanelView>();
                new AppFacade(comp);
            }

        }
    }

    public class AppFacade : Facade {


        public AppFacade(object obj) {

            RegisterCommand(PureMVCStart.Msg_ClickAddScoreButton, () => new DataCommand());
            RegisterMediator(new DataMediator(obj));
            RegisterProxy(new DataProxy(new ScoreData() { Score = 0 }));

        }
    }
}