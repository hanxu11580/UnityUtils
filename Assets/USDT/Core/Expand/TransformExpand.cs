using System;
using System.Collections.Generic;
using UnityEngine;

namespace USDT.Expand {
    public static class TransformExpand {
        private static Transform _cachedTrans;

        /// <summary>
        /// Transform归一化
        /// </summary>
        /// <param name="trans"></param>
        public static void TransformIdentity(this Transform trans) {
            trans.localScale = Vector3.one;
            trans.localRotation = Quaternion.identity;
            trans.localPosition = Vector3.zero;
        }


        #region 修改局部坐标和旋转 v3是值传递
        /// <summary>
        /// transform.localPosition是属性，变相相当于在传递他的成员变量
        /// 而值传递在传参是相当于是对其得复制，所以无法直接修改
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="newX"></param>
        public static void SetLocalPosX(this Transform trans, float newX) {
            Vector3 pos = trans.localPosition;
            pos.x = newX;
            trans.localPosition = pos;
        }
        public static void SetLocalPosY(this Transform trans, float newY) {
            Vector3 pos = trans.localPosition;
            pos.y = newY;
            trans.localPosition = pos;
        }
        public static void SetLocalPosZ(this Transform trans, float newZ) {
            Vector3 pos = trans.localPosition;
            pos.z = newZ;
            trans.localPosition = pos;
        }

        public static void SetLocalEulerX(this Transform trans, float newX) {
            Vector3 euler = trans.localEulerAngles;
            euler.x = newX;
            trans.localEulerAngles = euler;
        }
        public static void SetLocalEulerY(this Transform trans, float newY) {
            Vector3 euler = trans.localEulerAngles;
            euler.y = newY;
            trans.localEulerAngles = euler;
        }
        public static void SetLocalEulerZ(this Transform trans, float newZ) {
            Vector3 euler = trans.localEulerAngles;
            euler.z = newZ;
            trans.localEulerAngles = euler;
        }
        #endregion

        #region RectTransform局部坐标修改

        public static void SetLocalAnchorPosX(this RectTransform trans, float newX) {
            Vector2 pos = trans.anchoredPosition;
            pos.x = newX;
            trans.anchoredPosition = pos;
        }
        public static void SetLocalAnchorPosY(this RectTransform trans, float newY) {
            Vector2 pos = trans.anchoredPosition;
            pos.y = newY;
            trans.anchoredPosition = pos;
        }
        #endregion

        #region 遍历Transform节点

        /// <summary>
        /// 广度优先遍历所有节点
        /// </summary>
        /// <param name="prtTrans"></param>
        /// <param name="callback"></param>
        public static void TraverseTransform(this GameObject prtGo, Action<GameObject> callback) {
            Queue<GameObject> que = new Queue<GameObject>();
            que.Enqueue(prtGo);
            while (que.Count > 0) {
                var tempTrans = que.Dequeue();
                callback?.Invoke(tempTrans);
                if (tempTrans.transform.childCount > 0) {
                    foreach (Transform childTrans in prtGo.transform) {
                        que.Enqueue(childTrans.gameObject);
                    }
                }
            }
        }


        /// <summary>
        /// 查找子节点对象
        /// 内部使用“递归算法”
        /// </summary>
        /// <param name="goParent">父对象</param>
        /// <param name="chiildName">查找的子对象名称</param>
        /// <returns></returns>
        public static Transform FindTheChildNode(this Transform transPrt, string chiildName) {
            _cachedTrans = transPrt.Find(chiildName);
            if (_cachedTrans == null) {
                foreach (Transform trans in transPrt) {
                    _cachedTrans = FindTheChildNode(trans, chiildName);
                    if (_cachedTrans != null) {
                        return _cachedTrans;

                    }
                }
            }
            return _cachedTrans;
        }


        #endregion

        #region 快速设置旋转

        /// <summary>获得人物面向目标方向,这个传入的是世界方向 如:Vector3.forward</summary>
        public static void FaceToTargetWorldSpace(this Transform trans, Vector3 faceDir, Vector3 pos) {
            var dir = (pos - trans.position).normalized;
            Quaternion targetRota = Quaternion.FromToRotation(faceDir, dir);
            trans.rotation = targetRota;
        }

        /// <summary>
        /// 将物体某个轴面向某一方向向量(传入单位向量),transAixs传入真实方向如 transform.forward
        /// </summary>
        public static void FaceToDirectionSelfSpace(this Transform trans, Vector3 transAxis, Vector3 dir) {
            Vector3 targetPos = trans.position + dir;
            trans.FaceToTargetSelfSpace(transAxis, targetPos);
        }

        /// <summary>
        /// 将物体某个轴面向目标,transAixs传入真实方向如 transform.forward
        /// </summary>
        public static void FaceToTargetSelfSpace(this Transform trans, Vector3 transAixs, Vector3 targetPos) {
            Vector3 dir = targetPos - trans.position;
            Vector3 _axis = Vector3.Cross(transAixs, dir); //通过方向和需要对齐的方向确定旋转轴
            float angle = Vector3.Angle(transAixs, dir); //得到当前面向方向到目标方向需要旋转的角度
            Quaternion q = Quaternion.AngleAxis(angle, _axis); //按旋转轴旋转角度得到旋转矩阵
            trans.rotation = q * trans.rotation; //最后旋转目标矩阵
        }

        /// <summary>
        /// 将物体某个轴面向目标,transAixs传入真实方向如 transform.forward
        /// </summary>
        /// <returns>返回旋转矩阵</returns>
        public static Quaternion GetFaceToTargetSelfSpace(this Transform trans, Vector3 transAixs, Vector3 targetPos) {
            Vector3 dir = targetPos - trans.position;
            Vector3 _axis = Vector3.Cross(transAixs, dir);
            float angle = Vector3.Angle(transAixs, dir);
            Quaternion q = Quaternion.AngleAxis(angle, _axis);
            return q * trans.rotation;
        }

        public static Quaternion GetFaceToDirectionSelfSpace(this Transform trans, Vector3 transAxis, Vector3 dir) {
            Vector3 targetPos = trans.position + dir;
            return trans.GetFaceToTargetSelfSpace(transAxis, targetPos);
        }


        #endregion

        #region 绕某轴旋转

        /// <summary>
        /// 围绕轴旋转，并注视着
        /// </summary>
        /// <param name="trans">旋转者</param>
        /// <param name="center"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        public static Quaternion GetRotateAround(this Transform trans, Vector3 center, Vector3 axis, float angle) {
            //先旋转位置
            Quaternion rotaMatrix = Quaternion.AngleAxis(angle, axis);        //从这个轴旋转到这个角度 变换矩阵
            Vector3 dir = trans.position - center;                                      //计算从圆心指向摄像头的朝向向量
            dir = rotaMatrix * dir;                                         //旋转此向量
            trans.position = center + dir;                             //移动摄像机位置
            //然后旋转自身旋转
            //myrot是自身旋转矩阵 和 此时位置旋转矩阵右乘
            var myrot = trans.rotation;                               //此摄像机还是原来的角度
            Quaternion resultQ = rotaMatrix * myrot;                     // 设置角度
            return resultQ;

            // // 含义 先还原旋转 然后再旋转 最后在设置自身的旋转矩阵
            // transform.rotation *= Quaternion.Inverse(myrot) * rotaMatrix * myrot;
        }


        #endregion

    }
}