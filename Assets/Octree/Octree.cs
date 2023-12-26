using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USDT.Core;
using USDT.Expand;

namespace Octree {
    public class OctreeNode {
        public BoundingBox region;
        public List<BoundingBox> objects;
        // 动态创建的子区域
        // octants和childrenNodes是一一对应的
        public BoundingBox[] octants;
        public OctreeNode[] childrenNodes;
        public OctreeNode parent;

        public OctreeNode() {
            childrenNodes = new OctreeNode[8];
            parent = null;
            _buildList = new List<BoundingBox>[8];
            for (int i = 0; i < 8; i++) {
                _buildList[i] = new List<BoundingBox>();
            }
            _rmList = new List<BoundingBox>();
        }


        public OctreeNode(BoundingBox region, List<BoundingBox> objects) {
            this.region = region;
            // 直接赋值，外部更改会直接改变
            this.objects = new List<BoundingBox>(objects);
            childrenNodes = new OctreeNode[8];
            parent = null;

            _buildList = new List<BoundingBox>[8];
            for (int i = 0; i < 8; i++) {
                _buildList[i] = new List<BoundingBox>();
            }

            _rmList = new List<BoundingBox>();
        }

        public static BoundingBox[] CreateOctants(BoundingBox box) {
            BoundingBox[] octants = new BoundingBox[8];
            // 画一个方块
            // Gizmos.DrawWireCube(bounds.center, bounds.size);
            // var fourthBounds = bounds.size / 4;
            // var halfBoundsSize = bounds.size / 2;
            // 画一个 1/8 方块
            // Gizmos.DrawWireCube(bounds.center + fourthBounds, halfBoundsSize);
            Vector3 bounds = box.size;
            Vector3 halfBounds = bounds / 2;
            Vector3 center = box.center;
            Vector3 fourthBounds = bounds / 4;
            //North east top octant
            octants[0] = new BoundingBox(center + fourthBounds, halfBounds);

            //North west top octant
            octants[1] = new BoundingBox(new Vector3(center.x - fourthBounds.x, center.y + fourthBounds.y, center.z + fourthBounds.z), halfBounds);

            //South west top octant
            octants[2] = new BoundingBox(new Vector3(center.x - fourthBounds.x, center.y + fourthBounds.y, center.z - fourthBounds.z), halfBounds);

            //South east top octant
            octants[3] = new BoundingBox(new Vector3(center.x + fourthBounds.x, center.y + fourthBounds.y, center.z - fourthBounds.z), halfBounds);

            //North east bottom octant
            octants[4] = new BoundingBox(new Vector3(center.x + fourthBounds.x, center.y - fourthBounds.y, center.z + fourthBounds.z), halfBounds);

            //North west bottom octant
            octants[5] = new BoundingBox(new Vector3(center.x - fourthBounds.x, center.y - fourthBounds.y, center.z + fourthBounds.z), halfBounds);

            //South west bottom octant
            octants[6] = new BoundingBox(new Vector3(center.x - fourthBounds.x, center.y - fourthBounds.y, center.z - fourthBounds.z), halfBounds);

            //South east bottom octant
            octants[7] = new BoundingBox(new Vector3(center.x + fourthBounds.x, center.y - fourthBounds.y, center.z - fourthBounds.z), halfBounds);

            return octants;
        }

        // 元素用于新建OctreeNode构造函数objects
        private List<BoundingBox>[] _buildList;
        // 需要从objects删除的对象
        private List<BoundingBox> _rmList;
        public void BuildTree(List<BoundingBox> updateObjs = null) {
            if(updateObjs != null) {
                objects.Clear();
                objects.AddRange(updateObjs);
            }

            if(objects.Count <= 1) {
                return;
            }

            if(region.size.x <= 1f && region.size.y <= 1f && region.size.z <= 1) {
                return;
            }

            // 分出来8块区域
            if(octants == null) {
                octants = CreateOctants(region);
            }

            for (int i = 0; i < 8; i++) {
                _buildList[i].Clear();
            }
            _rmList.Clear();

            foreach (BoundingBox obj in objects) {
                for (int i = 0; i < 8; i++) {
                    var octantRegion = octants[i];
                    if (octantRegion.Contains(obj)) {
                        // 如果octantRegion区域包含obj
                        // 加入octantRegion区域内
                        _buildList[i].Add(obj);
                        _rmList.Add(obj);
                        break;
                    }
                }
            }

            // 移除在子区域的对象
            foreach (var rmObj in _rmList) {
                objects.ExRemove(rmObj);
            }

            for (int i = 0; i < 8; i++) {
                if(_buildList[i].Count > 0) {
                    childrenNodes[i] = new OctreeNode(octants[i], _buildList[i]);
                    childrenNodes[i].parent = this;
                    childrenNodes[i].BuildTree();
                }
            }
        }

        public static void GetAllObjects(OctreeNode node, List<BoundingBox> retObjects) {
            if(node.childrenNodes != null) {
                foreach (var cNode in node.childrenNodes) {
                    if(cNode != null) {
                        retObjects.AddRange(cNode.objects);
                        GetAllObjects(cNode, retObjects);
                    }
                }
            }
        }
    }
}