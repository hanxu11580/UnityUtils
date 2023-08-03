using UnityEngine;
using UnityEngine.SceneManagement;

namespace USDT.Expand {
    public static class SceneExpand {
        /// <summary>
        /// 将对象移到某场景，没有将会创建
        /// </summary>
        public  static void GoMove2Scene(string sceneStr, GameObject go)
        {
            //必须要重新获取
            Scene newScene = SceneManager.GetSceneByName(sceneStr);
            if (!newScene.isLoaded)
            {
                newScene = SceneManager.CreateScene(sceneStr);
            }

            if(go.transform.root != go.transform)
            {
                go.transform.parent = null;
            }

            SceneManager.MoveGameObjectToScene(go, newScene);
        }
    }
}
