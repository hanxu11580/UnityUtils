using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Octree {
    [System.Serializable]
    public struct BoundingBox {
        public BoundingBox(Vector3 center, Vector3 bounds) {
            this.center = center;
            this.size = bounds;
        }

        public void Draw() {
            Gizmos.DrawWireCube(center, size);
        }

        public Vector3 center;
        public Vector3 size;

        public bool IsColliding(BoundingBox other) {
            bool xCollision = Mathf.Abs(center.x - other.center.x) - ((size.x / 2) + (other.size.x / 2)) < 0;
            bool yCollision = Mathf.Abs(center.y - other.center.y) - ((size.y / 2) + (other.size.y / 2)) < 0;
            bool zCollision = Mathf.Abs(center.z - other.center.z) - ((size.z / 2) + (other.size.z / 2)) < 0;

            return xCollision && yCollision && zCollision;
        }

        //Returns true if other is fully inside of this bounding box
        public bool Contains(BoundingBox other) {
            bool xContains = (Mathf.Abs(center.x - other.center.x) + (other.size.x / 2 + size.x / 2)) < size.x;
            bool yContains = (Mathf.Abs(center.y - other.center.y) + (other.size.y / 2 + size.x / 2)) < size.y;
            bool zContains = (Mathf.Abs(center.z - other.center.z) + (other.size.z / 2 + size.x / 2)) < size.z;

            return xContains && yContains && zContains;
        }
    }
}