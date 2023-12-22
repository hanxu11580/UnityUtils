using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USDT.Utils;

namespace QuadTree {
    public class QuadTreeTestRootNode : MonoBehaviour {
        private QuadTreeNode<GameObject> _quadRoot;

        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _spawnCount;
        [SerializeField] private GameObject _rayStart;
        [SerializeField] private GameObject _rayEnd;
        private List<QuadTreeNode<GameObject>> _rayNodes;
        private List<QuadTreeLeaf<GameObject>> _rayLeafs;

        private void Start() {
            _quadRoot = new QuadTreeNode<GameObject>(0, 0, 100, 100, 1, -1);
            _rayNodes = new List<QuadTreeNode<GameObject>>();
            _rayLeafs = new List<QuadTreeLeaf<GameObject>>();
            // ��quadRoot��Χ�����ɶ���
            for (int i = 0; i < _spawnCount; i++) {
                Vector3 rnd = new Vector3(
                    Random.Range(_quadRoot.bounds.xMin, _quadRoot.bounds.xMax),
                    Random.Range(_quadRoot.bounds.yMin, _quadRoot.bounds.yMax), 0);
                var go = GameObject.Instantiate(_prefab, rnd, Quaternion.identity);
                go.name = i.ToString();
                _quadRoot.AddNode(rnd, go);
            }
        }


        private void OnDrawGizmos() {
            if (Application.isPlaying) {
                _quadRoot.DrwaGizmos();

                Gizmos.DrawLine(_rayStart.transform.position, _rayEnd.transform.position);

                // ����Բ��GameObject������Ż�����ɫ
                Color originalColor = Gizmos.color;
                Gizmos.color = Color.green;

                _rayNodes.Clear();
                _rayLeafs.Clear();
                var prefabRadius = 1f;
                var count = _quadRoot.GetLineIntersectsLeaf(_rayStart.transform.position, _rayEnd.transform.position, prefabRadius, ref _rayLeafs);
                if(count > 0) {
                    foreach (var leaf in _rayLeafs) {
                        GizmosUtils.DrawRect(leaf.Node.bounds);
                    }
                }

                Gizmos.color = originalColor;
            }
        }
    }
}