using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MeshCut {
    public class ScreenLineRenderer : MonoBehaviour {
        public Material lineMaterial;
        public event Action<Vector3, Vector3, Vector3> OnLineDrawn;

        private bool _isDragging;
        private Vector3 _start;
        private Vector3 _end;
        private Camera _cam;


        private void OnEnable() {
            //Camera.onPostRender += PostRenderDrawLine;
            RenderPipelineManager.endCameraRendering += PostRenderDrawLine;
        }

        private void OnDisable() {
            //Camera.onPostRender -= PostRenderDrawLine;
            RenderPipelineManager.endCameraRendering -= PostRenderDrawLine;
        }

        private void Start() {
            _cam = Camera.main;
        }

        private void Update() {
            if (!_isDragging && Input.GetMouseButtonDown(0)) {
                // µã»÷
                _isDragging = true;
                _start = _cam.ScreenToViewportPoint(Input.mousePosition);
            }
            else if (_isDragging && Input.GetMouseButtonUp(0)) {
                // Ì§Æð
                _isDragging = false;
                _end = _cam.ScreenToViewportPoint(Input.mousePosition);

                var startRay = _cam.ViewportPointToRay(_start);
                var endRay = _cam.ViewportPointToRay(_end);

                OnLineDrawn?.Invoke(
                    startRay.GetPoint(_cam.nearClipPlane),
                    endRay.GetPoint(_cam.nearClipPlane),
                    endRay.direction.normalized
                    );
            }

            if (_isDragging) {
                _end = _cam.ScreenToViewportPoint(Input.mousePosition);
            }
        }

        private void PostRenderDrawLine(ScriptableRenderContext context, Camera camera) {
            if (_isDragging && lineMaterial) {
                GL.PushMatrix();
                lineMaterial.SetPass(0);
                GL.LoadOrtho();
                GL.Begin(GL.LINES);
                GL.Color(Color.black);
                GL.Vertex(_start);
                GL.Vertex(_end);
                GL.End();
                GL.PopMatrix();
            }
        }
    }
}