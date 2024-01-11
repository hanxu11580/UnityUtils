using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftVoxelTerrain {
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour {
        public Transform view;
        public float jumpVelocity = 10;
        public float movementVelocity = 10;
        public float rotateSpeed = 1;
        private Rigidbody rb;

        private void Start() {
            // Òþ²ØÊó±ê
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
            rb = gameObject.GetComponent<Rigidbody>();
        }

        // this code runs once every frame
        private void Update() {
            // look
            float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed;
            float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
            view.Rotate(new Vector3(-mouseY, 0, 0));
            transform.Rotate(new Vector3(0, mouseX, 0));

            // movement and jump
            // A and D keys
            float horizontal = Input.GetAxis("Horizontal") * movementVelocity;
            // W and S keys
            float vertical = Input.GetAxis("Vertical") * movementVelocity;
            // jump multiplier
            float jump = 0;

            // if space key pressed
            if (Input.GetKeyDown(KeyCode.Space)) {
                // apply jump
                jump = jumpVelocity;
            }

            var newVelocity = 
                (transform.forward * vertical) +
                (transform.right * horizontal) +
                (Vector3.up * jump);

            // add to velocity
            rb.velocity += newVelocity * Time.deltaTime;
        }
    }
}