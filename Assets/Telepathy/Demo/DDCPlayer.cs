using System.Collections;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;

public class DDCPlayer
{
    private GameObject _gameObject;
    private Shooter _shooter;


    private DDClientPos _pos;
    // 目前一帧存在1个DDClientData
    // 如果存在多个改下这里的结构就行了
    private Queue<DDClientData> _delayProcessData;

    public DDCPlayer() {
        _delayProcessData = new Queue<DDClientData>();
    }

    public void BindGameObject(GameObject gameObject) {
        _gameObject = gameObject;
        _shooter = _gameObject.GetComponent<Shooter>();
    }

    public void DoUpdate() {
        if(_delayProcessData.Count > 0) {
            var cdata = _delayProcessData.Dequeue();
            ProcessData(cdata);
        }

        if (_gameObject != null) {
            _gameObject.transform.position = new Vector3(_pos.x, _pos.y);
        }
    }

    public void DoDestory() {
        if (_gameObject != null) {
            GameObject.Destroy(_gameObject);
            _gameObject = null;
        }
    }

    public void AddData(DDClientData cdata) {
        _delayProcessData.Enqueue(cdata);
    }

    public void ProcessData(DDClientData cdata) {
        if(InFlags(cdata.head.type, DDClientDataType.DragonPos)) {
            _pos = new DDClientPos() { x = cdata.dragonPos.x, y = cdata.dragonPos.y };
        }
        if(InFlags(cdata.head.type, DDClientDataType.PlayerMouseDown)) {
            _gameObject.transform.localScale = Vector3.one * 0.3f;
        }
        if (InFlags(cdata.head.type, DDClientDataType.PlayerMouseUp)) {
            _gameObject.transform.localScale = Vector3.one * 0.6f;
        }
        if (InFlags(cdata.head.type, DDClientDataType.DragonNormalBullet)) {
            _shooter.StopShootCounter();
            _shooter.ShootNormalBullet();
        }
        if (InFlags(cdata.head.type, DDClientDataType.DragonCounterBullet)) {
            _shooter.ShootCounterBullet(cdata.dragonCounterBullet.count);
        }
    }

    private bool InFlags(DDClientDataType dataType, DDClientDataType hasType) {
        return (dataType & hasType) != 0;
    }
}
