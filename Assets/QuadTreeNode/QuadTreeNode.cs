using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USDT.Expand;
using USDT.Utils;

namespace QuadTree {
    public class QuadTreeNode<T> {

        protected QuadTreeNode<T> parent;
        /// <summary>
        /// ���е�Ҷ�ӽڵ�
        /// </summary>
        protected List<QuadTreeLeaf<T>> items;
        /// <summary>
        /// �ڵ��֧
        /// </summary>
        protected QuadTreeNode<T>[] branch;
        /// <summary>
        /// �ڵ�ռ��������
        /// </summary>
        protected int maxItems;
        /// <summary>
        /// �ڵ�ռ�ָ���С��С
        /// </summary>
        protected float minSize;

        public const float TOLERANCE = 0.001f;

        public Rect bounds;

        public QuadTreeNode(float x, float y, float width, float height, int maximumItems, float minSize = -1, QuadTreeNode<T> parent = null) {
            bounds = new Rect(x, y, width, height);
            maxItems = maximumItems;
            this.minSize = minSize;
            this.parent = parent;
            items = new List<QuadTreeLeaf<T>>();
        }

        public bool HasChildren() {
            if(branch != null) {
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// ���ռ�ָ�4������
        /// </summary>
        protected void Split() {
            if(minSize != -1) {
                if(bounds.width <= minSize && bounds.height <= minSize) {
                    return;
                }
            }

            // ������һ�룬Ϊʲô��Ҫ��ȥһ��
            float nsHalf = bounds.height / 2;
            float ewHalf = bounds.width / 2;

            branch = new QuadTreeNode<T>[4];

            branch[0] = new QuadTreeNode<T>(bounds.x, bounds.y, ewHalf, nsHalf, maxItems, minSize, this);
            branch[1] = new QuadTreeNode<T>(bounds.x + ewHalf, bounds.y, ewHalf, nsHalf, maxItems, minSize, this);
            branch[2] = new QuadTreeNode<T>(bounds.x, bounds.y + nsHalf, ewHalf, nsHalf, maxItems, minSize, this);
            branch[3] = new QuadTreeNode<T>(bounds.x + ewHalf, bounds.y + nsHalf, ewHalf, nsHalf, maxItems, minSize, this);

            // �ѵ�ǰ��Ҷ�����·��䵽������ڵ���
            foreach (var item in items) {
                Add(item);
            }

            items.Clear();
        }

        /// <summary>
        /// ����λ�û��ĳ�������ӽڵ�
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected QuadTreeNode<T> GetChild(Vector2 pos) {
            // ����������������ܺϣ���������λ��
            if (!bounds.Contains(pos)) {
                return null;
            }

            // ����������ӽڵ㣬�����Լ�
            if(branch == null) {
                return this;
            }

            for (int i = 0; i < branch.Length; i++) {
                if (branch[i].bounds.Contains(pos)) {
                    return branch[i].GetChild(pos);
                }
            }

            // û���ҵ�
            return null;
        }

        public QuadTreeLeaf<T> Add(Vector2 pos, T obj) {
            return Add(new QuadTreeLeaf<T>(pos, obj));
        }

        private QuadTreeLeaf<T> Add(QuadTreeLeaf<T> leaf) {
            if(branch == null) {
                items.Add(leaf);
                leaf.Node = this;
                // ���������ٷ�4������
                if(items.Count > maxItems) {
                    Split();
                }
                return leaf;
            }
            else {
                // ������Ҷ�ӽڵ����ĸ�����
                QuadTreeNode<T> node = GetChild(leaf.Pos);
                if (node != null) {
                    // ��ĳ������
                    return node.Add(leaf);
                }
            }
            return null;
        }

        List<QuadTreeLeaf<T>> _temp = new List<QuadTreeLeaf<T>>();
        /// <summary>
        /// ����λ�ã�
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public void Update(QuadTreeLeaf<T> leaf, Vector2 pos) {
            if(leaf.Pos == pos) {
                return;
            }

            leaf.Node.items.ExRemove(leaf);
            var parent = leaf.Node.parent;
            while (parent != null) {
                _temp.Clear();
                var allLeafCount = parent.GetLeaf(parent.bounds, ref _temp);

                if (allLeafCount <= maxItems) {
                    // ����
                    parent.items.AddRange(_temp);
                    parent.branch = null;

                    parent = parent.parent;
                }
                else {
                    parent = null;
                }
            }

            leaf.Pos = pos;
            Add(leaf);
        }
        /// <summary>
        /// ���rect����������node����
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public int GetNode(Rect rect, ref List<T> nodes) {
            if (branch == null) {
                foreach (var item in items) {
                    if (rect.Contains(item.Pos)) {
                        nodes.Add(item.LeafObject);
                    }
                }
            }
            else {
                for (int i = 0; i < branch.Length; i++) {
                    // �����ص���ȥ��
                    if (branch[i].bounds.Overlaps(rect)) {
                        branch[i].GetNode(rect, ref nodes);
                    }
                }
            }
            return nodes.Count;
        }

        public int GetLeaf(Rect rect, ref List<QuadTreeLeaf<T>> leafs) {
            if (branch == null) {
                foreach (var item in items) {
                    if (rect.Contains(item.Pos)) {
                        leafs.Add(item);
                    }
                }
            }
            else {
                for (int i = 0; i < branch.Length; i++) {
                    // �����ص���ȥ��
                    if (branch[i].bounds.Overlaps(rect)) {
                        branch[i].GetLeaf(rect, ref leafs);
                    }
                }
            }
            return leafs.Count;
        }

        /// <summary>
        /// ��ú����ཻ������
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public int GetLineIntersectsNodes(Vector2 start, Vector2 end, ref List<QuadTreeNode<T>> nodes) {
            if (branch == null) {
                if(MathUtils.LineIntersectsRect(start, end, bounds)) {
                    nodes.Add(this);
                }
            }
            else {
                for (int i = 0; i < branch.Length; i++) {
                    // �����ص���ȥ��
                    if (MathUtils.LineIntersectsRect(start, end, branch[i].bounds)) {
                        branch[i].GetLineIntersectsNodes(start, end, ref nodes);
                    }
                }
            }
            return nodes.Count;
        }

        /// <summary>
        /// ��ú����ཻ�������ڶ����߶ε���̾������Ҫ��Ķ���
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="shortestDistance"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public int GetLineIntersectsLeaf(Vector2 start, Vector2 end, float shortestDistance, ref List<QuadTreeLeaf<T>> objs) {
            if (branch == null) {
                if (MathUtils.LineIntersectsRect(start, end, bounds)) {
                    foreach (var leaf in items) {
                        if (MathUtils.PointToLineDistance(leaf.Pos, start, end) < shortestDistance) {
                            objs.Add(leaf);
                        }
                    }
                }
            }
            else {
                for (int i = 0; i < branch.Length; i++) {
                    // �����ص���ȥ��
                    if (MathUtils.LineIntersectsRect(start, end, branch[i].bounds)) {
                        branch[i].GetLineIntersectsLeaf(start, end, shortestDistance, ref objs);
                    }
                }
            }
            return objs.Count;
        }


        /// <summary>
        /// ��������������㸽��ĳ����Ķ���
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="shortestDistance"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int GetNodeRecRange(Vector2 pos, float shortestDistance, ref List<T> list) {
            float distance;
            if(branch == null) {
                // ���û��������
                // ֱ�ӽ�С��shortestDistance�Ķ���ӽ�ȥ
                foreach (QuadTreeLeaf<T> leaf in items) {
                    distance = Vector2.Distance(pos, leaf.Pos);
                    if(distance < shortestDistance) {
                        list.Add(leaf.LeafObject);
                    }
                }
            }
            else {
                // �����������
                for (int i = 0; i < branch.Length; i++) {
                    // childDistance = x^2 + y^2
                    float childDistance = PointToBorderDistance(branch[i].bounds, pos);
                    if(childDistance < shortestDistance * shortestDistance) {
                        // ���ϵ����򽫻������������Ҷ���
                        branch[i].GetNodeRecRange(pos, shortestDistance, ref list);
                    }
                }
            }
            return list.Count;
        }

        /// <summary>
        /// ���� x^2 + y^2
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private static float PointToBorderDistance(Rect rect, Vector2 pos) {
            float xDis;
            float yDis;

            // x�������ڲ�
            if (rect.x <= pos.x && pos.x <= rect.xMax) {
                xDis = 0;
            }
            else {
                xDis = Mathf.Min((Mathf.Abs(pos.x - rect.width)), (pos.x - rect.x));
            }

            // y�������ڲ�
            if (rect.y <= pos.y && pos.y <= rect.yMax) {
                yDis = 0;
            }
            else {
                yDis = Mathf.Min((Mathf.Abs(pos.y - rect.height)), (pos.y - rect.y));
            }
            return xDis * xDis + yDis * yDis;
        }


        /// <summary>
        /// �����࣬����ûɶ����
        /// </summary>
        public void DrwaGizmos() {
            GizmosUtils.DrawRect(bounds);

            if (branch != null) {
                foreach (var node in branch) {
                    node.DrwaGizmos();
                }
            }
        }
    }
}