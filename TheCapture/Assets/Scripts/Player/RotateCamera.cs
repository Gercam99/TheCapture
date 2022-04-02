using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CTF.PlayerController

{
    public class RotateCamera : MonoBehaviour
    {
        public float sensitivity;
        public Transform character;
        
        private float xRotation;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
            
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90, 90);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            character.Rotate(Vector3.up * mouseX);
        }
    }
}
