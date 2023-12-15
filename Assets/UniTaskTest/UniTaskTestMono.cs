using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USDT.Utils;

public class UniTaskTestMono : MonoBehaviour {


    private void Start() {
        lg.i("Start");
        UniTask.WhenAll(
            LoadGameObject("UniTaskTestSphere", 1),
            LoadGameObject("UniTaskTestSphere", 2),
            LoadGameObject("UniTaskTestSphere", 3));
        lg.i("End");
    }

    async UniTask<GameObject> LoadGameObject(string path, int step) {
        var res = await Resources.LoadAsync<GameObject>(path);
        lg.i(step);
        return res as GameObject;
    }
}
