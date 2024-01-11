using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftVoxelTerrain {
    public class FPMouseLook : MonoBehaviour {
        //�ýű�����Main Camera��
        //���������Ұ�����������ƶ����ƶ�
        private Transform cameraTransform;//��������ڵ�λ��
        [SerializeField] private Transform charcterTransform;//FPController ��transform
        private Vector3 cameraRotation;//�����Ӧ����ת�ĽǶ�
        public float MouseSensitivity;//���������
        public Vector2 MaxminAngle;//�������������ƶ������Ƕ�
        private void Start() {
            cameraTransform = transform;
        }
        private void Update() {

            var tmp_mouseX = Input.GetAxis("Mouse X");//��ȡ����ƶ���x��
            var tmp_mouseY = Input.GetAxis("Mouse Y");//��ȡ����ƶ���y��

            cameraRotation.y += tmp_mouseX * MouseSensitivity;//�������������*x���ƶ����� = Y������ƫ�ƵĽǶ�
            cameraRotation.x -= tmp_mouseY * MouseSensitivity;//�������������*y���ƶ����� = X������ƫ�ƵĽǶ�

            cameraRotation.x = Mathf.Clamp(cameraRotation.x, MaxminAngle.x, MaxminAngle.y);//Ҫ����ֱ������ƫ�ƵĽǶȿ��������õķ�Χ��

            cameraTransform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);//�ı�������ĽǶ�
            charcterTransform.rotation = Quaternion.Euler(0, cameraRotation.y, 0);//��FPController��rotation��ˮƽ���򣬼�FPController��Ҫ��������ƶ���ˮƽ�����ƶ�
                                                                                  //����Ҫ��ע��ֱ����ı仯������֮������ͷ���죬��Ҳ�����������ߣ�����Ҫ��ˮƽ�����ߡ�

        }
    }
}