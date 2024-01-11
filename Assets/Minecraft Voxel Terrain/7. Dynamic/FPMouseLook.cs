using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftVoxelTerrain {
    public class FPMouseLook : MonoBehaviour {
        //该脚本挂在Main Camera上
        //摄像机的视野跟随着鼠标的移动而移动
        private Transform cameraTransform;//摄像机所在的位置
        [SerializeField] private Transform charcterTransform;//FPController 的transform
        private Vector3 cameraRotation;//摄像机应该旋转的角度
        public float MouseSensitivity;//鼠标灵敏度
        public Vector2 MaxminAngle;//限制上下所能移动的最大角度
        private void Start() {
            cameraTransform = transform;
        }
        private void Update() {

            var tmp_mouseX = Input.GetAxis("Mouse X");//获取鼠标移动的x轴
            var tmp_mouseY = Input.GetAxis("Mouse Y");//获取鼠标移动的y轴

            cameraRotation.y += tmp_mouseX * MouseSensitivity;//根据鼠标灵敏度*x轴移动距离 = Y方向上偏移的角度
            cameraRotation.x -= tmp_mouseY * MouseSensitivity;//根据鼠标灵敏度*y轴移动距离 = X方向上偏移的角度

            cameraRotation.x = Mathf.Clamp(cameraRotation.x, MaxminAngle.x, MaxminAngle.y);//要将垂直方向上偏移的角度控制在设置的范围内

            cameraTransform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);//改变摄像机的角度
            charcterTransform.rotation = Quaternion.Euler(0, cameraRotation.y, 0);//将FPController的rotation的水平方向，即FPController需要跟着鼠标移动的水平方向移动
                                                                                  //不需要关注垂直方向的变化，简言之，就是头看天，人也不能忘天上走，还是要在水平方向走。

        }
    }
}