using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Csp;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ILRuntime.Protobuf;

namespace Telepathy {
    public class DailyDungeonClient {

        const int DefaultMaxMessageSize = 16 * 1024;

        Client _client;
        Action<DDCFrameData> _onFrameData;
        Action<DDCEventData> _onEventData;
        /// <summary>
        /// 多少帧触发一个Tick，默认一帧触发一次
        /// </summary>
        int _cumulativeUpdateTick;
        int _cumulativeUpdateSend;
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();

        /**/

        public bool IsNull => _client == null;
        public bool Connected => !IsNull && _client.Connected;

        public int TriggerUpdateTick { get; set; } = 1;
        public int TriggerUpdateSend { get; set; } = 1;


        public DailyDungeonClient() {
            _client = new Client(DefaultMaxMessageSize);
        }
        public DailyDungeonClient(int MaxMessageSize) {
            _client = new Client(MaxMessageSize);
        }

        public void Start() {
            _client.OnConnected = OnConnected;
            _client.OnData = OnData;
            _client.OnDisconnected = OnDisconnected;
        }

        public void Connect(string ip, int port) {
            if (IsNull) return;
            _client.Connect(ip, port);
        }

        public void Disconnect() {
            if (!IsNull && _client.Connected) {
                _client.Disconnect();
                _client = null;
                _onFrameData = null;
                _onEventData = null;
                _sendQueue = null;
            }
        }

        public void Send(object obj, bool directlySend = true) {
            if (IsNull) return;
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

        public void UpdateTick() {
            _cumulativeUpdateTick++;
            if(_cumulativeUpdateTick >= TriggerUpdateTick) {
                Tick(100);
                _cumulativeUpdateTick -= TriggerUpdateTick;
            }
        }

        public void UpdateSend() {
            _cumulativeUpdateSend++;
            if(_cumulativeUpdateSend >= TriggerUpdateSend) {
                while(_sendQueue.Count > 0) {
                    var sendBytes = _sendQueue.Dequeue();
                    _client.Send(sendBytes);
                }
                _cumulativeUpdateSend -= TriggerUpdateSend;
            }
        }


        public void Tick(int processLimit) {
            if (IsNull) return;
            _client.Tick(processLimit);
        }

        public DailyDungeonClient RegisterFrameDataCall(Action<DDCFrameData> call) {
            if (!IsNull) {
                _onFrameData += call;
            }
            return this;
        }

        public DailyDungeonClient UnRegisterFrameDataCall(Action<DDCFrameData> call) {
            if (!IsNull) {
                _onFrameData -= call;
            }
            return this;
        }

        public DailyDungeonClient RegisterEventDataCall(Action<DDCEventData> call) {
            if (!IsNull) {
                _onEventData += call;
            }
            return this;
        }

        public DailyDungeonClient UnRegisterEventDataCall(Action<DDCEventData> call) {
            if (!IsNull) {
                _onEventData -= call;
            }
            return this;
        }


        void OnConnected() { }
        void OnDisconnected() { }

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
                ProcessData(f.ClientType, f.Data.ToByteArray());
            }
        }

        void ProcessData(int clientType, byte[] bytes) {
            if (IsNull) return;
            var obj = Utils.Deserialize(bytes);
            switch ((CSRelayFrameClientType)clientType) {
                case CSRelayFrameClientType.KCsrelayFrameClientTypeFrame: {
                        var frameData = obj as DDCFrameData;
                        if(frameData != null) {
                            _onFrameData?.Invoke(frameData);
                        }
                        break;
                    }
                case CSRelayFrameClientType.KCsrelayFrameClientTypeEvent: {
                        var eventData = obj as DDCEventData;
                        if (eventData != null) {
                            _onEventData?.Invoke(eventData);
                        }
                        break;
                    }
            }
        }
    }
}
