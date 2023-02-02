using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour {
    public GameObject prefab;
    public GameObject CounterPrefab;
    public bool isShoot;

    public int shootInterval;
    private int _curShootInterval;

    private Coroutine _shooterCounterCoroutine;

    private void Update() {
        if (isShoot) {
            if (_curShootInterval < shootInterval) {
                _curShootInterval++;
                return;
            }
            _curShootInterval = 0;
            ShootNormalBullet();
        }
    }

    public void ShootNormalBullet() {
        var ins = GameObject.Instantiate(prefab);
        ins.transform.position = transform.position;
    }

    public void StopShootCounter() {
        if(_shooterCounterCoroutine == null) {
            return;
        }
        StopCoroutine(_shooterCounterCoroutine);
    }

    public void ShootCounterBullet(int shootCount) {
        _shooterCounterCoroutine = StartCoroutine(ShootCounter(shootCount));
    }

    IEnumerator ShootCounter(int shootCount) {
        var count = 0;
        while (count < shootCount) {
            count++;
            yield return new WaitForSeconds(0.25f);
            var ins = GameObject.Instantiate(CounterPrefab);
            ins.transform.position = transform.position;
        }
    }
}
