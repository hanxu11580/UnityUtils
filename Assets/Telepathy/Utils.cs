using Csp;
using ILRuntime.Protobuf;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Telepathy
{
    public static class Utils
    {
        // IntToBytes version that doesn't allocate a new byte[4] each time.
        // -> important for MMO scale networking performance.
        public static void IntToBytesBigEndianNonAlloc(int value, byte[] bytes, int offset = 0)
        {
            bytes[offset + 0] = (byte)(value >> 24);
            bytes[offset + 1] = (byte)(value >> 16);
            bytes[offset + 2] = (byte)(value >> 8);
            bytes[offset + 3] = (byte)value;
        }

        public static int BytesToIntBigEndian(byte[] bytes)
        {
            return (bytes[0] << 24) |
                   (bytes[1] << 16) |
                   (bytes[2] << 8) |
                    bytes[3];
        }

        public static object Deserialize(byte[] bytes) {
            object obj = null;
            try {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream(bytes)) {
                    obj = formatter.Deserialize(mStream);
                }
            }
            catch (Exception e) {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }
            return obj;
        }

        public static byte[] Serialize(object obj) {
            byte[] ret = default;
            try {
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream mStream = new MemoryStream()) {
                    formatter.Serialize(mStream, obj);
                    ret = mStream.GetBuffer();
                }
            }
            catch (Exception e) {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }
            return ret;
        }


        public static CSRelayFrameInputReq GenerateFrameInputReq(object obj) {
            CSRelayFrameInputReq req = null;
            if (obj is DDCFrameData frameData) {
                req = new CSRelayFrameInputReq();
                var frameDataBytes = Utils.Serialize(frameData);
                req.Frame.Add(new CSRelayFrame() {
                    ClientType = (int)CSRelayFrameClientType.KCsrelayFrameClientTypeFrame,
                    Data = ByteString.CopyFrom(frameDataBytes)
                });
            }
            else if (obj is DDCEventData eventData) {
                req = new CSRelayFrameInputReq();
                var eventDataBytes = Utils.Serialize(eventData);
                req.Frame.Add(new CSRelayFrame() {
                    ClientType = (int)CSRelayFrameClientType.KCsrelayFrameClientTypeEvent,
                    Data = ByteString.CopyFrom(eventDataBytes)
                });
            }
            return req;
        }
    }
}
