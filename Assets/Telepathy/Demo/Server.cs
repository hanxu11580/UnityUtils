using Csp;
using ILRuntime.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using Telepathy;
using UnityEngine;

public class Server : MonoBehaviour
{
    Telepathy.Server server = new Telepathy.Server(16 * 2024);
    private int _connectCount = 0;

    private void Awake() {
        server.OnConnected = sOnConnected;
        server.OnDisconnected = (connectionId) => Debug.Log(connectionId + " Disconnected");
        server.OnData = sOnData;

        server.Start(1337);
    }

    private int _curTick;
    int[] _randomTick = new int[] { 10, 50, 80 };
    private int _tick = 60;
    private void Update() {
        if (server.Active) {
            // 波动测试
            //_curTick++;
            //if (_curTick >= _tick) {
            //    _curTick -= _tick;
            //    _tick = _randomTick[UnityEngine.Random.Range(0, 3)];
            //    server.Tick(100);
            //}

            server.Tick(10000);
        }
    }

    void OnApplicationQuit() {
        server.Stop();
    }

    IEnumerator CreatePrefab() {
        yield return new WaitForSeconds(2f);
        var data1 = new DDClientData() {
            head = new DDCClientDataHead() {
                id = 1,
                type = DDClientDataType.None
            }
        };
        var data2 = new DDClientData() {
            head = new DDCClientDataHead() {
                id = 2,
                type = DDClientDataType.None
            }
        };

        CSRelayFrameInputReq req1 = Utils.GenerateFrameInputReq(data1);
        ArraySegment<byte> sendBytes1 = null;
        if (req1 != null) {
            sendBytes1 = new ArraySegment<byte>(req1.ToByteArray());
        }
        server.Send(2, sendBytes1);


        CSRelayFrameInputReq req2 = Utils.GenerateFrameInputReq(data2);
        ArraySegment<byte> sendBytes2 = null;
        if (req2 != null) {
            sendBytes2 = new ArraySegment<byte>(req2.ToByteArray());
        }
        server.Send(1, sendBytes2);
    }

    private void sOnData(int connectionId, ArraySegment<byte> message) {
        var forwarding = connectionId == 1 ? 2 : 1;
        server.Send(forwarding, message);
        Debug.Log($"server转发至{connectionId}");
    }

    private void sOnConnected(int connectionId) {
        Debug.Log(connectionId + " Connected");
        _connectCount++;
        // 2个玩家都连接了，创建对应的预制体
        if (_connectCount != -1 && _connectCount >= 2) {
            StartCoroutine(CreatePrefab());
            _connectCount = -1;
        }
    }

}
