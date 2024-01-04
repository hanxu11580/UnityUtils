using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCut {
    public class MeshCutter {
        // 切割时被一个平面分成两部分的Mesh
        // 在平面法线一侧
        public TempMesh PositiveMesh { get; private set; }
        public TempMesh NegativeMesh { get; private set; }

        private List<Vector3> addedPairs;

        private readonly List<Vector3> ogVertices;
        private readonly List<int> ogTriangles;
        private readonly List<Vector3> ogNormals;
        private readonly List<Vector2> ogUvs;

        private readonly Vector3[] intersectPair;
        private readonly Vector3[] tempTriangle;

        private Intersections intersect;

        private readonly float threshold = 1e-6f;

        public MeshCutter(int initialArraySize) {
            PositiveMesh = new TempMesh(initialArraySize);
            NegativeMesh = new TempMesh(initialArraySize);

            addedPairs = new List<Vector3>(initialArraySize);
            ogVertices = new List<Vector3>(initialArraySize);
            ogNormals = new List<Vector3>(initialArraySize);
            ogUvs = new List<Vector2>(initialArraySize);
            ogTriangles = new List<int>(initialArraySize * 3);

            intersectPair = new Vector3[2];
            tempTriangle = new Vector3[3];

            intersect = new Intersections();
        }

        public bool SliceMesh(Mesh mesh, ref Plane slice) {
            // Let's always fill the vertices array so that we can access it even if the mesh didn't intersect
            mesh.GetVertices(ogVertices);

            // 如果mesh没有和平面相交就跳过
            if (!Intersections.BoundPlaneIntersect(mesh, ref slice))
                return false;

            mesh.GetTriangles(ogTriangles, 0);
            mesh.GetNormals(ogNormals);
            mesh.GetUVs(0, ogUvs);
            // 清空 重复使用
            PositiveMesh.Clear();
            NegativeMesh.Clear();
            addedPairs.Clear();

            // 将原顶点分别放入2个TempMesh
            for (int i = 0; i < ogVertices.Count; ++i) {
                if (slice.GetDistanceToPoint(ogVertices[i]) >= 0)
                    PositiveMesh.AddVertex(ogVertices, ogNormals, ogUvs, i);
                else
                    NegativeMesh.AddVertex(ogVertices, ogNormals, ogUvs, i);
            }

            // 如果其中一个网格没有顶点，则不相交 跳过
            if (NegativeMesh.vertices.Count == 0 || PositiveMesh.vertices.Count == 0)
                return false;

            // 将原来的三角形分开，如果三角形和平面相交，会将三角形分割后放入对应的Mesh中
            for (int i = 0; i < ogTriangles.Count; i += 3) {
                if (intersect.TrianglePlaneIntersect(ogVertices, ogUvs, ogTriangles, i, ref slice, PositiveMesh, NegativeMesh, intersectPair))
                    addedPairs.AddRange(intersectPair);
            }

            if (addedPairs.Count > 0) {
                // 如果有新交点
                FillBoundaryFace(addedPairs);
                return true;
            }
            else {
                throw new UnityException("Error: if added pairs is empty, we should have returned false earlier");
            }
        }

        public Vector3 GetFirstVertex() {
            if (ogVertices.Count == 0)
                throw new UnityException(
                    "Error: Either the mesh has no vertices or GetFirstVertex was called before SliceMesh.");
            else
                return ogVertices[0];
        }

        #region Boundary fill method

        #region 这个只适用于凸多边形

        private void FillBoundaryGeneral(List<Vector3> added) {
            // 1. Reorder added so in order ot their occurence along the perimeter.
            ReorderList(added);

            Vector3 center = FindCenter(added);

            //Create triangle for each edge to the center
            tempTriangle[2] = center;

            for (int i = 0; i < added.Count; i += 2) {
                // Add fronface triangle in meshPositive
                tempTriangle[0] = added[i];
                tempTriangle[1] = added[i + 1];

                PositiveMesh.AddNewTriangle(tempTriangle);

                // Add backface triangle in meshNegative
                tempTriangle[0] = added[i + 1];
                tempTriangle[1] = added[i];

                NegativeMesh.AddNewTriangle(tempTriangle);
            }
        }

        private static Vector3 FindCenter(List<Vector3> pairs) {
            Vector3 center = Vector3.zero;
            int count = 0;

            for (int i = 0; i < pairs.Count; i += 2) {
                center += pairs[i];
                count++;
            }

            return center / count;
        }

        #endregion

        private void FillBoundaryFace(List<Vector3> added) {
            // 1. Reorder added so in order ot their occurence along the perimeter.
            ReorderList(added);

            // 2. Find actual face vertices
            var face = FindRealPolygon(added);

            // 3. Create triangle fans
            int t_fwd = 0,
                t_bwd = face.Count - 1,
                t_new = 1;
            bool incr_fwd = true;

            while (t_new != t_fwd && t_new != t_bwd) {
                AddTriangle(face, t_bwd, t_fwd, t_new);

                if (incr_fwd) t_fwd = t_new;
                else t_bwd = t_new;

                incr_fwd = !incr_fwd;
                t_new = incr_fwd ? t_fwd + 1 : t_bwd - 1;
            }
        }

        private List<Vector3> FindRealPolygon(List<Vector3> pairs) {
            List<Vector3> vertices = new List<Vector3>();
            Vector3 edge1, edge2;

            // List should be ordered in the correct way
            for (int i = 0; i < pairs.Count; i += 2) {
                edge1 = (pairs[i + 1] - pairs[i]);
                if (i == pairs.Count - 2)
                    edge2 = pairs[1] - pairs[0];
                else
                    edge2 = pairs[i + 3] - pairs[i + 2];

                // Normalize edges
                edge1.Normalize();
                edge2.Normalize();

                if (Vector3.Angle(edge1, edge2) > threshold)
                    // This is a corner
                    vertices.Add(pairs[i + 1]);
            }

            return vertices;
        }

        private void AddTriangle(List<Vector3> face, int t1, int t2, int t3) {
            tempTriangle[0] = face[t1];
            tempTriangle[1] = face[t2];
            tempTriangle[2] = face[t3];
            PositiveMesh.AddNewTriangle(tempTriangle);

            tempTriangle[1] = face[t3];
            tempTriangle[2] = face[t2];
            NegativeMesh.AddNewTriangle(tempTriangle);
        }
        #endregion

        #region utils
        private static void ReorderList(List<Vector3> pairs) {
            int nbFaces = 0;
            int faceStart = 0;
            int i = 0;

            while (i < pairs.Count) {
                // Find next adjacent edge
                for (int j = i + 2; j < pairs.Count; j += 2) {
                    if (pairs[j] == pairs[i + 1]) {
                        // Put j at i+2
                        SwitchPairs(pairs, i + 2, j);
                        break;
                    }
                }


                if (i + 3 >= pairs.Count) {
                    // Why does this happen?
                    Debug.Log("Huh?");
                    break;
                }
                else if (pairs[i + 3] == pairs[faceStart]) {
                    // A face is complete.
                    nbFaces++;
                    i += 4;
                    faceStart = i;
                }
                else {
                    i += 2;
                }
            }
        }

        private static void SwitchPairs(List<Vector3> pairs, int pos1, int pos2) {
            if (pos1 == pos2) return;

            Vector3 temp1 = pairs[pos1];
            Vector3 temp2 = pairs[pos1 + 1];
            pairs[pos1] = pairs[pos2];
            pairs[pos1 + 1] = pairs[pos2 + 1];
            pairs[pos2] = temp1;
            pairs[pos2 + 1] = temp2;
        }
        #endregion
    }
}