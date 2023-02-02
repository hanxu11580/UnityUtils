using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Csp;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ILRuntime.Protobuf;

namespace Telepathy {
    public class DDClient {

        const int DefaultMaxMessageSize = 16 * 1024;

        Client _client;
        public Action<DDClientData> OnDDClientDataCall;
        public Action OnConnectedCall;
        public Action OnDisconnectedCall;
        /// <summary>
        /// 多少帧触发一个Tick，默认一帧触发一次
        /// </summary>
        int _cumulativeUpdateTick;
        int _cumulativeUpdateSend;
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();

        public bool Connected => _client.Connected;

        public int TriggerUpdateTick { get; set; } = 1;
        public int TriggerUpdateSend { get; set; } = 1;


        public DDClient() {
            _client = new Client(DefaultMaxMessageSize);
            Init();
        }
        public DDClient(int MaxMessageSize) {
            _client = new Client(MaxMessageSize);
            Init();
        }

        public void DoConnect(string ip, int port) {
            if (_client != null && _client.Connected) 
                return;
            _client.Connect(ip, port);
        }

        public void DoDisconnect() {
            if (_client != null && _client.Connected) {
                _client.Disconnect();
                _client = null;
                OnDDClientDataCall = null;
                _sendQueue = null;
            }
        }

        public void DoSend(object obj, bool directlySend = true) {
            if (_client != null && !_client.Connected) return;
            CSRelayFrameInputReq req = Utils.GenerateFrameInputReq(obj);
            if (req != null) {
                ArraySegment<byte> sendBytes = new ArraySegment<byte>(req.ToByteArray());
                if (directlySend) {
                    _client.Send(sendBytes);
                }
                else {
                    _sendQueue.Enqueue(sendBytes);
                }
            }
        }

        public void DoUpdateTick() {
            _cumulativeUpdateTick++;
            if(_cumulativeUpdateTick >= TriggerUpdateTick) {
                DoTick(100);
                _cumulativeUpdateTick -= TriggerUpdateTick;
            }
        }

        public void DoUpdateSend() {
            if (_client != null && !_client.Connected) return;
            _cumulativeUpdateSend++;
            if(_cumulativeUpdateSend >= TriggerUpdateSend) {
                while(_sendQueue.Count > 0) {
                    var sendBytes = _sendQueue.Dequeue();
                    _client.Send(sendBytes);
                }
                _cumulativeUpdateSend -= TriggerUpdateSend;
            }
        }


        public void DoTick(int processLimit) {
            if (_client != null && !_client.Connected) return;
            _client.Tick(processLimit);
        }

        void Init() {
            _client.OnConnected = OnConnected;
            _client.OnData = OnData;
            _client.OnDisconnected = OnDisconnected;
        }

        void OnConnected() {
            OnConnectedCall?.Invoke();
        }
        void OnDisconnected() {
            OnDisconnectedCall?.Invoke();
        }

        void OnData(ArraySegment<byte> message) {
            if (message.Count == 0) {
                Debug.LogError($"[DailyDungeonClient OnDataRecv] message.Count == 0");
                return;
            }

            var bytes = new byte[message.Count];
            Array.Copy(message.Array, message.Offset, bytes, 0, message.Count);
            CSRelayFrameInputReq cSRelayFrameInputReq = null;
            try {
                cSRelayFrameInputReq = Csp.CSRelayFrameInputReq.Parser.ParseFrom(bytes);
            } catch (Exception e) {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
                return;
            }
            if (cSRelayFrameInputReq == null) {
                return;
            }

            if (cSRelayFrameInputReq.Frame == null) {
                Debug.LogError($"[DailyDungeonClient OnDataRecv] cSRelayFrameInputReq.Frame == null");
                return;
            }
            foreach (var f in cSRelayFrameInputReq.Frame) {
                ProcessData(f.Data.ToByteArray());
            }
        }

        void ProcessData(byte[] bytes) {
            var obj = Utils.Deserialize(bytes);
            var frameData = obj as DDClientData;
            if (frameData != null) {
                OnDDClientDataCall?.Invoke(frameData);
            }
        }
    }
}
