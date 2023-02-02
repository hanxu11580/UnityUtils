using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILRuntime.Protobuf;
using Csp;
using System.Linq;

namespace Telepathy {
    public class TelepathyDemo : MonoBehaviour {
        public int delayTick;

        public GameObject playerPrefab;

        private DDCRoom _room1;
        //private DDCRoom _room2;

        public Shooter playerSelf;

        private int playerId;
        public int triggerUpdateTick;

        private bool _isMouse0Downed;
        private bool _isMouse1Downed;

        void Awake() {
            Application.targetFrameRate = 60;
#if UNITY_EDITOR
            playerId = 1;
            triggerUpdateTick = 1;
#endif
            //return;
            // update even if window isn't focused, otherwise we don't receive.
            UnityEngine.Application.runInBackground = true;

            // use Debug.Log functions for Telepathy so we can see it in the console
            Log.Info = Debug.Log;
            Log.Warning = Debug.LogWarning;
            Log.Error = Debug.LogError;

            _room1 = new DDCRoom();
            _room1.ConnectServer("localhost", 1337, this);

            //_room2 = new DDCRoom();
            //_room2.ConnectServer("localhost", 1337, this);
        }

        void FixedUpdate() {
            _room1.DoUpdate();
            //_room2.DoUpdate();

            if (Input.GetMouseButtonDown(0)) {
                _isMouse0Downed = true;
            }
            if (Input.GetMouseButtonDown(1)) {
                _isMouse1Downed = true;
            }

            #region room1

            if (Input.GetMouseButton(0)) {
                // 按下、移动、发射子弹
                DDCClientDataHead head = new DDCClientDataHead() {
                    id = playerId,
                    type = DDClientDataType.PlayerMouseDown
                    | DDClientDataType.DragonPos 
                    | DDClientDataType.DragonNormalBullet,
                };

                var screenPos = Input.mousePosition;
                screenPos.z = 10;
                var worldPos = Camera.main.ScreenToWorldPoint(screenPos);

                var pos = new DDClientPos() { x = worldPos.x, y = worldPos.y };
                // 一帧发送一个子弹
                var normalBullet = new DDClientDragonNormalBullet() { count = 1 };
                _room1.Client.DoSend(new DDClientData() { 
                    head = head,
                    dragonPos = pos,
                    dragonNormalBullet = normalBullet,
                });

                playerSelf.transform.position = new Vector3(pos.x, pos.y);
                playerSelf.transform.localScale = Vector3.one * 0.3f;
                playerSelf.StopShootCounter();
                playerSelf.ShootNormalBullet();

            }
            if(Input.GetMouseButtonUp(0)) {
                if (!_isMouse0Downed) {
                    return;
                }
                _isMouse0Downed = false;
                // 松开
                DDCClientDataHead head = new DDCClientDataHead() {
                    id = playerId,
                    type = DDClientDataType.PlayerMouseUp
                    | DDClientDataType.DragonCounterBullet,
                };

                var screenPos = Input.mousePosition;
                screenPos.z = 10;
                var worldPos = Camera.main.ScreenToWorldPoint(screenPos);

                // 一帧发送一个子弹
                var counterBullet = new DDClientDragonCounterBullet() { count = 5 };
                _room1.Client.DoSend(new DDClientData() {
                    head = head,
                    dragonCounterBullet = counterBullet,
                });

                playerSelf.transform.localScale = Vector3.one * 0.6f;
                playerSelf.ShootCounterBullet(counterBullet.count);
            }
            #endregion
        }

        void OnApplicationQuit() {
            _room1.DoDestory();
            //_room2.DoDestory();
        }
    }
}
