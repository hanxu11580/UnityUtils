using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Octree {
    public class BoundingBoxComponent : MonoBehaviour {
        [SerializeField] public BoundingBox box;

        private void OnDrawGizmos() {
            Gizmos.DrawCube(transform.position, box.size);
        }
    }
}