using System;
using System.Collections;
using System.Collections.Generic;
using CTF.Managers;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace CTF.PlayerController
{
    public class Move : MonoBehaviour
    {
        [Header("MOVE PARAMETERS")]
        [Tooltip("Variable para asignar la velocidad")] 
        public float speed;
        
        [Header("JUMP PARAMETERS")]
        [Tooltip("Variable encargada de ejercer la gravedad al jugador")]
        public float Gravity;
        [Tooltip("Variable para saber la posicion de si el player toca el suelo, poner en los pies")]
        public Transform CheckGround;
        [Tooltip("Layers que detectara el jugador para ver si toca el suelo o no")]
        public LayerMask groundMask;
        [Tooltip("Distancia para saber cuando toca el suelo")]
        public float distanceGround;
        [Tooltip("Variable encargada de ejercer una fuerza de salto")]
        public float jumpForce;

        private CharacterController playerController;
        private bool isGrounded;
        private Vector3 VelocityDown;
        private Vector3 _move;

        public GameObject[] destroyObjects;
        public Component[] destroyComponents;

        public PhotonView pv;


        private void Awake()
        {
            playerController = GetComponent<CharacterController>();
            pv = GetComponent<PhotonView>();
        }

        private void Start()
        {
            Debug.Log(pv.Controller.NickName + pv.Controller.GetPhotonTeam());

            if (!pv.IsMine)
            {
                foreach (var objects in destroyObjects)
                {
                    Destroy(objects);
                }

                foreach (var component in destroyComponents)
                {
                    Destroy(component);
                }
            }
        }

        private void Update()
        {
            if(!pv.IsMine)
                return;
            
            if(GameManager.finishGame)
                return;
            
            isGrounded = Physics.CheckSphere(CheckGround.position, distanceGround, groundMask);
            if ( isGrounded  && VelocityDown.y < 0)
            {
                VelocityDown.y = -2;
            }
            
            MoveController();
            
            if(Input.GetButtonDown("Jump") && isGrounded)
            {
                VelocityDown.y = Mathf.Sqrt(jumpForce * -2 * Gravity);
            }
            
            VelocityDown.y += Gravity * Time.deltaTime;
            playerController.Move(VelocityDown * Time.deltaTime);


        }
        
        
        /// <summary>
        /// Funcion encargada de mover el personaje por los axis
        /// </summary>
        private void MoveController()
        {
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");
            
            _move = transform.right * horizontalMovement + transform.forward * verticalMovement;
            _move = Vector3.ClampMagnitude(_move, 1f);
            playerController.Move(_move * speed * Time.deltaTime);
        }
    }
}
