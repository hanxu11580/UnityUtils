using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int DestoryTick;
    public float speed;

    private int _curTick = 0;

    // Update is called once per frame
    void Update()
    {
        _curTick++;
        if (_curTick >= DestoryTick) {
            GameObject.Destroy(gameObject);
        }
        else {
            transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);
        }
    }
}
