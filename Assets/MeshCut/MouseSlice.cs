using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MeshCut {
    public class MouseSlice : MonoBehaviour {

        [SerializeField] private ScreenLineRenderer lineRenderer;
        [SerializeField] private GameObject _plane;
        [SerializeField] private bool _drawPlane;
        [SerializeField] private Transform _objectContainer;
        // �ָ�󣬷ֿ��ľ���
        [SerializeField] private float _separation;


        private Plane _slicePlane = new Plane();

        private MeshCutter _meshCutter;
        private TempMesh _biggerMesh, _smallerMesh;

        private void Start() {
            _meshCutter = new MeshCutter(256);
            _plane.SetActive(_drawPlane ? true : false);
        }

        private void OnEnable() {
            lineRenderer.OnLineDrawn += OnLineDrawn;
        }

        private void OnDisable() {
            lineRenderer.OnLineDrawn -= OnLineDrawn;
        }


        private void OnLineDrawn(Vector3 start, Vector3 end, Vector3 depth) {
            // ƽ�����ߣ����ߴ�ֱ����
            var planeTangent = (end - start).normalized;
            // depth�����ָ��end�ķ���
            // �õ�����
            var normalVec = Vector3.Cross(depth, planeTangent);
            if (_drawPlane) {
                DrawPlane(start, end, normalVec);
            }
            SliceObjects(start, normalVec);
        }

        private void SliceObjects(Vector3 point, Vector3 normal) {
            // �ҵ�������Ҫ�и�Ķ���
            var toSlice = GameObject.FindGameObjectsWithTag("Sliceable");
            List<Transform> positive = new List<Transform>();
            List<Transform> negative = new List<Transform>();
            GameObject obj;
            bool slicedAny = false;
            for (int i = 0; i < toSlice.Length; ++i) {
                obj = toSlice[i];
                var transformedNormal = (obj.transform.worldToLocalMatrix * normal).normalized;
                //var transformedNormal = ((Vector3)(obj.transform.localToWorldMatrix.transpose * normal)).normalized;
                _slicePlane.SetNormalAndPosition(transformedNormal, obj.transform.InverseTransformPoint(point));
                slicedAny = SliceObject(ref _slicePlane, obj, positive, negative) || slicedAny;
            }

            if (slicedAny) {
                // �Ὣ�и�ֿ�һ��
                SeparateMeshes(positive, negative, normal);
            }
        }

        private bool SliceObject(ref Plane slicePlane, GameObject obj, List<Transform> positiveObjects, List<Transform> negativeObjects) {
            var mesh = obj.GetComponent<MeshFilter>().mesh;

            if (!_meshCutter.SliceMesh(mesh, ref slicePlane)) {
                // û�к������ཻ�Ķ��󣬷ֱ�����Ӧ���б���
                // Put object in the respective list
                if (slicePlane.GetDistanceToPoint(_meshCutter.GetFirstVertex()) >= 0)
                    positiveObjects.Add(obj.transform);
                else
                    negativeObjects.Add(obj.transform);

                return false;
            }

            // Silly condition that labels which mesh is bigger to keep the bigger mesh in the original gameobject
            bool posBigger = _meshCutter.PositiveMesh.surfacearea > _meshCutter.NegativeMesh.surfacearea;
            if (posBigger) {
                _biggerMesh = _meshCutter.PositiveMesh;
                _smallerMesh = _meshCutter.NegativeMesh;
            }
            else {
                _biggerMesh = _meshCutter.NegativeMesh;
                _smallerMesh = _meshCutter.PositiveMesh;
            }

            // Create new Sliced object with the other mesh
            GameObject newObject = Instantiate(obj, _objectContainer);
            newObject.transform.SetPositionAndRotation(obj.transform.position, obj.transform.rotation);
            var newObjMesh = newObject.GetComponent<MeshFilter>().mesh;

            // Put the bigger mesh in the original object
            // TODO: Enable collider generation (either the exact mesh or compute smallest enclosing sphere)
            ReplaceMesh(mesh, _biggerMesh);
            ReplaceMesh(newObjMesh, _smallerMesh);

            (posBigger ? positiveObjects : negativeObjects).Add(obj.transform);
            (posBigger ? negativeObjects : positiveObjects).Add(newObject.transform);

            return true;
        }

        /// <summary>
        /// ��tempMesh�滻mesh�������
        /// </summary>
        private void ReplaceMesh(Mesh mesh, TempMesh tempMesh, MeshCollider collider = null) {
            mesh.Clear();
            mesh.SetVertices(tempMesh.vertices);
            mesh.SetTriangles(tempMesh.triangles, 0);
            mesh.SetNormals(tempMesh.normals);
            mesh.SetUVs(0, tempMesh.uvs);

            //mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            if (collider != null && collider.enabled) {
                collider.sharedMesh = mesh;
                collider.convex = true;
            }
        }

        private void SeparateMeshes(List<Transform> positives, List<Transform> negatives, Vector3 worldPlaneNormal) {
            int i;
            var separationVector = worldPlaneNormal * _separation;

            for (i = 0; i < positives.Count; ++i)
                positives[i].transform.position += separationVector;

            for (i = 0; i < negatives.Count; ++i)
                negatives[i].transform.position -= separationVector;
        }


        private void DrawPlane(Vector3 start, Vector3 end, Vector3 normalVec) {
            Quaternion rotate = Quaternion.FromToRotation(Vector3.up, normalVec);
            _plane.transform.localRotation = rotate;
            _plane.transform.position = (end + start) / 2;
            _plane.SetActive(true);
        }
    }
}
