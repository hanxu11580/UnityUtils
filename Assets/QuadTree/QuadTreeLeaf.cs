using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuadTree {

    /// <summary>
    /// 叶子节点的叶子内容
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QuadTreeLeaf<T> {
        private Vector2 _pos;
        private T _refObject;
        // 所属的节点
        public QuadTreeNode<T> Node { get; set; }

        public QuadTreeLeaf(Vector2 pos, T refObject) {
            _pos = pos;
            _refObject = refObject;
        }

        public T LeafObject => _refObject;

        public Vector2 Pos {
            get => _pos;
            set => _pos = value;
        }
    }
}