using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Telepathy {

    [System.Serializable]
    public class DDClientData {
        public DDCClientDataHead head;
        public DDClientPos dragonPos;
        public DDClientDragonNormalBullet dragonNormalBullet;
        public DDClientDragonCounterBullet dragonCounterBullet;
    }

    [System.Serializable]
    public struct DDClientPos {
        public float x;
        public float y;
    }

    [System.Serializable]
    public struct DDClientDragonCounterBullet {
        public int count;
    }

    [System.Serializable]
    public struct DDClientDragonNormalBullet {
        public int count;
    }

    [System.Serializable]
    public struct DDCClientDataHead {
        public int id;
        public DDClientDataType type;
    }

    [Flags]
    [System.Serializable]
    public enum DDClientDataType {
        None = 0,
        DragonPos = 1 << 0,
        DragonNormalBullet = 1 << 1,
        DragonCounterBullet = 1 << 2,
        PlayerMouseDown = 1 << 3,
        PlayerMouseUp = 1 << 4,

    }
}
