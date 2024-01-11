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
                // 这里有点怪怪的，冲刺的时候跳，会感觉顿一下，感觉跳跃把冲刺阻断了一样
                // 空中movementDirection除了y都不会变，等待落到地面才会通过awsd改变x,z分量值
                // 所有就是如果按着冲刺跳跃，空中的速度会变成walkSpeed
                // 所以这里直接使用上次的速度就可以
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