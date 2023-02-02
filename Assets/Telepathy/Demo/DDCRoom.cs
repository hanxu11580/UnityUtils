using System;
using System.Collections;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;

public class DDCRoom
{
    public DDClient Client { get; private set; }
    private Dictionary<int, DDCPlayer> _playerDic;
    private TelepathyDemo _domeInstance;
    private int _postponedDoUpdate = 30;
    private int _curPostponedDoUpdate;

    public DDCRoom() {
        _playerDic = new Dictionary<int, DDCPlayer>();
    }

    public void ConnectServer(string ip, int port, TelepathyDemo domeInstance) {
        Client = new DDClient();
        Client.OnConnectedCall += OnConnect;
        Client.OnDisconnectedCall += OnDisConnect;
        Client.OnDDClientDataCall += OnDDClientDataCall;
        Client.DoConnect(ip, port);
        _domeInstance = domeInstance;
    }

    public void BindPlayerGameObject(int id, GameObject gameObject) {
        if(_playerDic.TryGetValue(id, out DDCPlayer player)){
            player.BindGameObject(gameObject);
        }
        else {
            var newPlayer = new DDCPlayer();
            newPlayer.BindGameObject(gameObject);
            _playerDic.Add(id, newPlayer);
        }
    }

    public void DoUpdate() {
        if(_curPostponedDoUpdate != -1) {
            _curPostponedDoUpdate++;
            if (_curPostponedDoUpdate >= _postponedDoUpdate) {
                _curPostponedDoUpdate = -1;
            }
        }
        else {
            foreach (var kv in _playerDic) {
                kv.Value.DoUpdate();
            }
        }

        Client.DoUpdateSend();
        Client.DoUpdateTick();
    }

    public void DoDestory() {
        foreach (var kv in _playerDic) {
            kv.Value.DoDestory();
        }

        Client.DoDisconnect();
        Client.DoDisconnect();
    }

    private void OnConnect() {

    }

    private void OnDisConnect() {

    }

    private void OnDDClientDataCall(DDClientData cdata) {
        if(cdata.head.type == DDClientDataType.None) {
            var joinId = cdata.head.id;
            var newGo = GameObject.Instantiate(_domeInstance.playerPrefab);
            newGo.transform.position = Vector3.one;
            BindPlayerGameObject(joinId, newGo);
            Debug.Log($"º”»ÎÕÊº“:{joinId}");
            return;
        }

        if(_playerDic.TryGetValue(cdata.head.id, out DDCPlayer player)) {
            player.AddData(cdata);
            //player.ProcessData(cdata);
        }
    }
}
