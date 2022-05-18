using System;
using System.Collections;
using System.Collections.Generic;
using CTF.Inventory;
using CTF.Managers;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace CTF.PlayerController
{
    public class PlayerController : MonoBehaviour, IDamageable
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
        public LayerMask GroundMask;
        [Tooltip("Distancia para saber cuando toca el suelo")]
        public float DistanceGround;
        [Tooltip("Variable encargada de ejercer una fuerza de salto")]
        public float JumpForce;

        [Header("HEALTH PARAMETERS")] 
        [SerializeField] private float MaxHealth;
        [SerializeField] private float HealthRegen;
        public HealthSystem HealthSystem;

        private CharacterController playerController;
        private bool isGrounded;
        private Vector3 VelocityDown;
        private Vector3 _move;
        private InventoryController _inventory;

        public PhotonView pv;


        private void Awake()
        {
            playerController = GetComponent<CharacterController>();
            pv = GetComponent<PhotonView>();
            _inventory = GetComponent<InventoryController>();
            
            HealthSystem = new HealthSystem(MaxHealth);
        }

        private void Start()
        {
            Debug.Log(pv.Controller.NickName + pv.Controller.GetPhotonTeam());

        }

        private void Update()
        {
            if (!pv.IsMine)
                return;

            if (GameManager.finishGame)
                return;

            isGrounded = Physics.CheckSphere(CheckGround.position, DistanceGround, GroundMask);
            if (isGrounded && VelocityDown.y < 0)
            {
                VelocityDown.y = -2;
            }

            MoveInput();
            PowerUpInput();


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TakeDamage(10);
            }
        }

        

        /// <summary>
        /// Funcion encargada de mover el personaje por los axis
        /// </summary>
        private void MoveInput()
        {
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");
            
            _move = transform.right * horizontalMovement + transform.forward * verticalMovement;
            _move = Vector3.ClampMagnitude(_move, 1f);
            playerController.Move(_move * speed * Time.deltaTime);
            
            JumpInput();
        }
        
        private void JumpInput()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                VelocityDown.y = Mathf.Sqrt(JumpForce * -2 * Gravity);
            }

            VelocityDown.y += Gravity * Time.deltaTime;
            playerController.Move(VelocityDown * Time.deltaTime);
        }

        private void PowerUpInput()
        {
            if(_inventory.HasPowerUp() == false) return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // Use powerup
                _inventory.PowerUpData = null;
            }
        }

        public void TakeDamage(float _damageAmount)
        {
            HealthSystem.Damage(_damageAmount);
        }
    }
}
