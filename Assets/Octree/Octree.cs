using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USDT.Core;
using USDT.Expand;

namespace Octree {
    public class OctreeNode {
        public BoundingBox region;
        public List<BoundingBox> objects;
        // ��̬������������
        // octants��childrenNodes��һһ��Ӧ��
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
            // ֱ�Ӹ�ֵ���ⲿ���Ļ�ֱ�Ӹı�
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
            // ��һ������
            // Gizmos.DrawWireCube(bounds.center, bounds.size);
            // var fourthBounds = bounds.size / 4;
            // var halfBoundsSize = bounds.size / 2;
            // ��һ�� 1/8 ����
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

        // Ԫ�������½�OctreeNode���캯��objects
        private List<BoundingBox>[] _buildList;
        // ��Ҫ��objectsɾ���Ķ���
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

            // �ֳ���8������
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
                        // ���octantRegion�������obj
                        // ����octantRegion������
                        _buildList[i].Add(obj);
                        _rmList.Add(obj);
                        break;
                    }
                }
            }

            // �Ƴ���������Ķ���
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