using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CTF
{
    public class Bullet : MonoBehaviour
    {
        public float TimeToDestruction;
        public float Speed;
        public LayerMask Target;

        // Asignar player cuando se dispara la bala.
        void Start()
        {
            Destroy(gameObject, TimeToDestruction);
        }
        public void Update()
        {
            Collider[] targetColliders = Physics.OverlapSphere(transform.position, 0.5f, Target);
            if (targetColliders.Length != 0)
            {
                Destroy(gameObject);               
            }
        }
        public void InitializeBullet(/*Poner la variable player*/Vector3 direction /*Poner el Lag*/)
        {
            //Asignar el player.
            transform.forward = direction;
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.velocity = direction * Speed;
            //rigidbody.position += rigidbody.velocity;// Multiplicar por el Lag.
        }
    }
}
