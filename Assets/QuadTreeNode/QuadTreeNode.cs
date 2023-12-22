using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USDT.Utils;

namespace QuadTree {
    public class QuadTreeNode<T> {
        /// <summary>
        /// 所有的叶子节点
        /// </summary>
        protected List<QuadTreeLeaf<T>> items;
        /// <summary>
        /// 节点分支
        /// </summary>
        protected QuadTreeNode<T>[] branch;
        /// <summary>
        /// 节点空间最大容量
        /// </summary>
        protected int maxItems;
        /// <summary>
        /// 节点空间分割最小大小
        /// </summary>
        protected float minSize;

        protected int rank;

        public const float TOLERANCE = 0.001f;

        public Rect bounds;

        public QuadTreeNode(float x, float y, float width, float height, int maximumItems, float minSize = -1, int rank = 1) {
            bounds = new Rect(x, y, width, height);
            maxItems = maximumItems;
            this.minSize = minSize;
            this.rank = rank;
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
        /// 将空间分割4个区域
        /// </summary>
        protected void Split() {
            if(minSize != -1) {
                if(bounds.width <= minSize && bounds.height <= minSize) {
                    return;
                }
            }

            // 不就是一半，为什么还要减去一半
            float nsHalf = bounds.height / 2;
            float ewHalf = bounds.width / 2;

            branch = new QuadTreeNode<T>[4];

            branch[0] = new QuadTreeNode<T>(bounds.x, bounds.y, ewHalf, nsHalf, maxItems, minSize, this.rank + 1);
            branch[1] = new QuadTreeNode<T>(bounds.x + ewHalf, bounds.y, ewHalf, nsHalf, maxItems, minSize, this.rank + 1);
            branch[2] = new QuadTreeNode<T>(bounds.x, bounds.y + nsHalf, ewHalf, nsHalf, maxItems, minSize, this.rank + 1);
            branch[3] = new QuadTreeNode<T>(bounds.x + ewHalf, bounds.y + nsHalf, ewHalf, nsHalf, maxItems, minSize, this.rank + 1);

            foreach (var item in items) {
                AddNode(item);
            }

            items.Clear();
        }

        /// <summary>
        /// 根据位置获得某个区域子节点
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected QuadTreeNode<T> GetChild(Vector2 pos) {
            // 如果根区域（子区域总合）不包含此位置
            if (!bounds.Contains(pos)) {
                return null;
            }

            // 如果不包含子节点，返回自己
            if(branch == null) {
                return this;
            }

            for (int i = 0; i < branch.Length; i++) {
                if (branch[i].bounds.Contains(pos)) {
                    return branch[i].GetChild(pos);
                }
            }

            // 没有找到
            return null;
        }

        public bool AddNode(Vector2 pos, T obj) {
            return AddNode(new QuadTreeLeaf<T>(pos, obj));
        }

        private bool AddNode(QuadTreeLeaf<T> leaf) {
            if(branch == null) {
                items.Add(leaf);
                leaf.Node = this;
                // 超过将会再分4块区域
                if(items.Count > maxItems) {
                    Split();
                }
                return true;
            }
            else {
                // 看看这叶子节点在哪个区域
                QuadTreeNode<T> node = GetChild(leaf.Pos);
                if (node != null) {
                    // 在某个区域
                    return node.AddNode(leaf);
                }
            }
            return false;
        }

        public bool RemoveNode(Vector2 pos, T obj) {
            if(branch == null) {
                for (int i = 0; i < items.Count; i++) {
                    QuadTreeLeaf<T> qtl = items[i];
                    if (qtl.LeafObject.Equals(obj)) {
                        items.RemoveAt(i);
                        return true;
                    }
                }
            }
            else {
                QuadTreeNode<T> node = GetChild(pos);
                if(node != null) {
                    return node.RemoveNode(pos, obj);
                }
            }
            return false;
        }

        /// <summary>
        /// 更新位置？
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool UpdateNode(Vector2 pos, T obj) {
            if (branch == null) {
                for (int i = 0; i < items.Count; i++) {
                    QuadTreeLeaf<T> qtl = items[i];
                    if (qtl.LeafObject.Equals(obj)) {
                        qtl.Pos = pos;
                        return true;
                    }
                }
            }
            else {
                QuadTreeNode<T> node = GetChild(pos);
                if (node != null) {
                    return node.UpdateNode(pos, obj);
                }
            }
            return false;
        }

        /// <summary>
        /// 获得rect区域内所有node数据
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
                    // 区域重叠再去找
                    if (branch[i].bounds.Overlaps(rect)) {
                        branch[i].GetNode(rect, ref nodes);
                    }
                }
            }
            return nodes.Count;
        }

        /// <summary>
        /// 获得和线相交的区域
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
                    // 区域重叠再去找
                    if (MathUtils.LineIntersectsRect(start, end, branch[i].bounds)) {
                        branch[i].GetLineIntersectsNodes(start, end, ref nodes);
                    }
                }
            }
            return nodes.Count;
        }

        /// <summary>
        /// 获得和线相交的区域内对象到线段的最短距离符合要求的对象
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
                    // 区域重叠再去找
                    if (MathUtils.LineIntersectsRect(start, end, branch[i].bounds)) {
                        branch[i].GetLineIntersectsLeaf(start, end, shortestDistance, ref objs);
                    }
                }
            }
            return objs.Count;
        }


        /// <summary>
        /// 根据坐标获得坐标点附近某区域的对象
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="shortestDistance"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public int GetNodeRecRange(Vector2 pos, float shortestDistance, ref List<T> list) {
            float distance;
            if(branch == null) {
                // 如果没有子区域
                // 直接将小于shortestDistance的对象加进去
                foreach (QuadTreeLeaf<T> leaf in items) {
                    distance = Vector2.Distance(pos, leaf.Pos);
                    if(distance < shortestDistance) {
                        list.Add(leaf.LeafObject);
                    }
                }
            }
            else {
                // 如果有子区域
                for (int i = 0; i < branch.Length; i++) {
                    // childDistance = x^2 + y^2
                    float childDistance = PointToBorderDistance(branch[i].bounds, pos);
                    if(childDistance < shortestDistance * shortestDistance) {
                        // 符合的区域将会从这个区域来找对象
                        branch[i].GetNodeRecRange(pos, shortestDistance, ref list);
                    }
                }
            }
            return list.Count;
        }

        /// <summary>
        /// 返回 x^2 + y^2
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private static float PointToBorderDistance(Rect rect, Vector2 pos) {
            float xDis;
            float yDis;

            // x在区域内部
            if (rect.x <= pos.x && pos.x <= rect.xMax) {
                xDis = 0;
            }
            else {
                xDis = Mathf.Min((Mathf.Abs(pos.x - rect.width)), (pos.x - rect.x));
            }

            // y在区域内部
            if (rect.y <= pos.y && pos.y <= rect.yMax) {
                yDis = 0;
            }
            else {
                yDis = Mathf.Min((Mathf.Abs(pos.y - rect.height)), (pos.y - rect.y));
            }
            return xDis * xDis + yDis * yDis;
        }


        /// <summary>
        /// 会冗余，但是没啥大事
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