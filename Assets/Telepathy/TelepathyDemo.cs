using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILRuntime.Protobuf;
using Csp;

namespace Telepathy {
    public class TelepathyDemo : MonoBehaviour {
        DailyDungeonClient client1 = new DailyDungeonClient(16 * 2024);
        DailyDungeonClient client2 = new DailyDungeonClient(16 * 2024);
        Server server = new Server(16 * 2024);

        void Awake() {
            // update even if window isn't focused, otherwise we don't receive.
            Application.runInBackground = true;

            // use Debug.Log functions for Telepathy so we can see it in the console
            Log.Info = Debug.Log;
            Log.Warning = Debug.LogWarning;
            Log.Error = Debug.LogError;

            client1.RegisterFrameDataCall(OnFrameData1)
                .RegisterEventDataCall(OnEventData1)
                .Start();

            client2.RegisterFrameDataCall(OnFrameData2)
                .RegisterEventDataCall(OnEventData2)
                .Start();


            server.OnConnected = (connectionId) => Debug.Log(connectionId + " Connected");
            server.OnDisconnected = (connectionId) => Debug.Log(connectionId + " Disconnected");
            server.OnData = sOnData;
        }

        void OnEventData1(DDCEventData obj) {
            Debug.Log($"1 收到一个事件");
        }

        void OnEventData2(DDCEventData obj) {
            Debug.Log($"2 收到一个事件");
        }

        void OnFrameData1(DDCFrameData frameData) {
            Debug.Log($"1 收到{frameData.posX}-{frameData.posY}");
        }
        void OnFrameData2(DDCFrameData frameData) {
            Debug.Log($"2 收到{frameData.posX}-{frameData.posY}");
        }

        private void sOnData(int connectionId, ArraySegment<byte> message) {
            var forwarding = connectionId == 1 ? 2 : 1;
            server.Send(forwarding, message);
            Debug.Log($"server转发至{connectionId}");
        }

        void Update() {
            // client
            if (client1.Connected) {
                // send message on key press
                if (Input.GetKeyDown(KeyCode.Space)) {
                    //client1.Send(Csp.CSRelayFrameClientType.KCsrelayFrameClientTypeFrame, new DDCFrameData() {
                    //    posX = 100,
                    //    posY = 100,
                    //});
                    client1.Send(new DDCEventData());
                }
                if (Input.GetKeyDown(KeyCode.C)) {
                    client1.Tick(100);
                }
            }

            if (client2.Connected) {
                // send message on key press
                if (Input.GetKeyDown(KeyCode.Space)) {
                    //client2.Send(Csp.CSRelayFrameClientType.KCsrelayFrameClientTypeFrame, new DDCFrameData() {
                    //    posX = 200,
                    //    posY = 200,
                    //});
                    client2.Send(new DDCEventData());
                }
                if (Input.GetKeyDown(KeyCode.V)) {
                    client2.Tick(100);
                }
            }



            // server
            if (server.Active) {
                if (Input.GetKeyDown(KeyCode.S)) {
                    server.Tick(100);
                }
            }
        }

        void OnGUI() {
            // client
            GUI.enabled = !client1.Connected;
            if (GUI.Button(new Rect(0, 0, 120, 20), "Connect Client")) {
                client1.Connect("localhost", 1337);
                client2.Connect("localhost", 1337);
            }

            GUI.enabled = client1.Connected;
            if (GUI.Button(new Rect(130, 0, 120, 20), "Disconnect Client")) {
                client1.Disconnect();
                client2.Disconnect();
            }

            // server
            GUI.enabled = !server.Active;
            if (GUI.Button(new Rect(0, 25, 120, 20), "Start Server"))
                server.Start(1337);

            GUI.enabled = server.Active;
            if (GUI.Button(new Rect(130, 25, 120, 20), "Stop Server"))
                server.Stop();

            GUI.enabled = true;
        }

        void OnApplicationQuit() {
            client1.Disconnect();
            server.Stop();
        }
    }
}
