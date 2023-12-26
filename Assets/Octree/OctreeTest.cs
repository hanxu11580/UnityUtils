using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Octree {
    public class OctreeTest : MonoBehaviour {

        OctreeNode _root;
        [SerializeField] BoundingBox _box;
        [SerializeField] int _count;
        [SerializeField] GameObject _prefab;

        private List<BoundingBoxComponent> _allComp;
        private List<BoundingBox> _updateObjects;
        private void Start() {
            var halfX = _box.size.x / 2f;
            var halfY = _box.size.y / 2f;
            var halfZ = _box.size.z / 2f;

            _allComp = new List<BoundingBoxComponent>();
            _updateObjects = new List<BoundingBox>();
            for (int i = 0; i < _count; i++) {
                var rx = Random.Range(-halfX, halfX);
                var ry = Random.Range(-halfY, halfY);
                var rz = Random.Range(-halfZ, halfZ);
                var go = GameObject.Instantiate(_prefab, new Vector3(rx, ry, rz), Quaternion.identity);
                var boxComp = go.GetComponent<BoundingBoxComponent>();
                boxComp.box.center = go.transform.position;
                _updateObjects.Add(boxComp.box);
                _allComp.Add(boxComp);
            }

            _root = new OctreeNode(_box, _updateObjects);
            _root.BuildTree();
        }

        private void Update() {
            _updateObjects.Clear();
            // 更新Box位置
            foreach (var comp in _allComp) {
                comp.box.center = comp.transform.position;
                _updateObjects.Add(comp.box);
            }
            // 将最新的对象数据作为更新数据
            _root.BuildTree(_updateObjects);
        }

        private void OnDrawGizmos() {
            _box.Draw();

            RenderOctree(_root);
        }

        void RenderOctree(OctreeNode tree) {
            if (tree == null)
                return;

            foreach (BoundingBox b in tree.octants) {
                b.Draw();
            }

            if (tree.childrenNodes == null)
                return;

            foreach (OctreeNode child in tree.childrenNodes) {
                if (child == null || child.octants == null)
                    continue;

                foreach (BoundingBox b in child.octants) {
                    b.Draw();
                    RenderOctree(child);
                }
            }
        }
    }
}