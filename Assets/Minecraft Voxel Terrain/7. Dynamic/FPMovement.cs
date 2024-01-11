using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USDT.Utils;

namespace MinecraftVoxelTerrain {
    public class FPMovement : MonoBehaviour {
        private CharacterController characterController;
        private Transform charcterTransform;
        private float Gravity = 9.8f;
        private Vector3 movementDirection;
        private bool isCrouch;
        public float crouchHeight = 1f;
        private float originHeight;
        public float walkSpeed;
        public float jumpHeight;
        public float sprintingSpeed;
        public float sprintingSpeedCrouched;
        public float walkSpeedCrouched;

        private float _lastSpeed;
        void Start() {
            characterController = GetComponent<CharacterController>();
            charcterTransform = transform;
            originHeight = characterController.height;
            _lastSpeed = walkSpeed;
        }

        void Update() {
            float tmpSpeed = default;
            if (characterController.isGrounded) {
                tmpSpeed = walkSpeed;
                var tmp_Horizontal = Input.GetAxis("Horizontal");
                var tmp_Vertical = Input.GetAxis("Vertical");

                movementDirection =
                    charcterTransform.TransformDirection(new Vector3(tmp_Horizontal, 0, tmp_Vertical));
                if (Input.GetButtonDown("Jump")) {
                    movementDirection.y = jumpHeight;
                }
                if (isCrouch) {
                    tmpSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintingSpeedCrouched : walkSpeedCrouched;
                }
                else {
                    tmpSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintingSpeed : walkSpeed;
                }


                if (Input.GetKeyDown(KeyCode.C)) {
                    var temp_CurentHeight = isCrouch ? originHeight : crouchHeight;
                    StartCoroutine(DoCrouch(temp_CurentHeight));
                    isCrouch = !isCrouch;
                }

                _lastSpeed = tmpSpeed;
            }
            else {
                // �����е�ֵֹģ���̵�ʱ��������о���һ�£��о���Ծ�ѳ�������һ��
                // ����movementDirection����y������䣬�ȴ��䵽����Ż�ͨ��awsd�ı�x,z����ֵ
                // ���о���������ų����Ծ�����е��ٶȻ���walkSpeed
                // ��������ֱ��ʹ���ϴε��ٶȾͿ���
                tmpSpeed = _lastSpeed;
            }

            movementDirection.y -= Gravity * Time.deltaTime;
            characterController.Move(movementDirection * Time.deltaTime * tmpSpeed);

        }
        IEnumerator DoCrouch(float target) {
            float tmp_CurrentHeight = 0;
            while (Mathf.Abs(characterController.height - target) > 0.1f) {
                yield return null;
                characterController.height = Mathf.SmoothDamp(characterController.height, target, ref tmp_CurrentHeight, Time.deltaTime * 5);
            }
        }
    }
}