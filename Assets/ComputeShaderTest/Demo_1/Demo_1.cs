using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ComputeShaderTest.Demo1 {
    public class Demo_1 : MonoBehaviour {

        [SerializeField] int _textSize;
        [SerializeField] Material _material;
        [SerializeField] ComputeShader _shader;
        [SerializeField] Camera _cam;
        [SerializeField] Color _wallColor;
        [SerializeField] Color _particleColor;
        [SerializeField] float _brushSize = 2.5f;

        private int _kernel;
        private Vector2Int _dispatchCount;
        private MeshCollider _meshCollider;

        void Start() {
            _meshCollider = GetComponent<MeshCollider>();

            RenderTexture rt = new RenderTexture(_textSize, _textSize, 0);
            rt.wrapMode = TextureWrapMode.Clamp;
            rt.filterMode = FilterMode.Point;
            rt.enableRandomWrite = true;
            _material.SetTexture("_MainTex", rt);

            _kernel = _shader.FindKernel("CSMain");
            _shader.SetTexture(_kernel, "Result", rt);
            _shader.SetInt("_Size", _textSize);
            _shader.SetVector("_WallColor", _wallColor);
            _shader.SetVector("_ParticleColor", _particleColor);

            _shader.GetKernelThreadGroupSizes(_kernel, out uint threadX, out uint threadY, out _);
            // ����[numthreads(8,8,1)]��_textSize = 256
            // ���Է����߳���Ϊ (32, 32, 1)
            _dispatchCount.x = Mathf.CeilToInt(_textSize / threadX);
            _dispatchCount.y = Mathf.CeilToInt(_textSize / threadY);
        }

        void Update() {
            int mouseMode = -1;
            // uv֮��
            Vector2 mouseUV = -Vector2.one;
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
                if (Input.GetMouseButton(0) && Input.GetMouseButton(1)) {
                    // ����ǽ
                    mouseMode = 1;
                }
                else if (Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
                    // ��������
                    mouseMode = 0;
                }
                else if (!Input.GetMouseButton(0) && Input.GetMouseButton(1)) {
                    // ���ǽ
                    mouseMode = 2;
                }

                if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.collider == _meshCollider) {
                    mouseUV = hit.textureCoord;
                }
            }

            // ΪʲôҪÿһ֡��ִ�У�
            // ��Ϊ���²���������������ص������
            _shader.SetFloat("_BrushSize", _brushSize);
            _shader.SetInt("_MouseMode", mouseMode);
            _shader.SetVector("_MousePos", mouseUV);
            _shader.SetFloat("_Time", Time.time);
            _shader.Dispatch(_kernel, _dispatchCount.x, _dispatchCount.y, 1);
        }
    }
}