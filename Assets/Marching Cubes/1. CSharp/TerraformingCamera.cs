using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarchingCubes_CSharp {
    public class TerraformingCamera : MonoBehaviour {

        public float BrushSize = 2f;


        private Vector3 _hitPoint;
        private Camera _cam;

        private void Awake() {
            _cam = GetComponent<Camera>();
        }

        private void Start() {

        }

        private void Update() {

        }

        private void LateUpdate() {
            if (Input.GetMouseButton(0)) {
                Terraform(true);
            }
            else if (Input.GetMouseButton(1)) {
                Terraform(false);
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_hitPoint, BrushSize);
        }

        private void Terraform(bool add) {
            if(Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000)) {
                Chunk hitChunk = hit.collider.gameObject.GetComponent<Chunk>();
                _hitPoint = hit.point;
            }
        }
    }
}