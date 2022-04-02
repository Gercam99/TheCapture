using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.SceneManagement;



namespace CTF
{
    public class TestingShooting : MonoBehaviour
    {

        public static TestingShooting Instance;         
        public int myPlayer;
        public Camera Cam;
        public GameObject Projectile;
        public Transform FirePoint;        
        public float ProjectileSpeed = 30;

        private Vector3 destination;
             

        private void Start()
        {
           
        }

        private void Update()
        {
                        
            if (Input.GetButtonDown("Fire1"))
            {
                ShootProjectile();
                
            }
        }

        private void ShootProjectile()
        {
            Ray ray = Cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                destination = hit.point;
                InstantiateProjectile(FirePoint);
            }
            else
            {
                destination = ray.GetPoint(1000);
                InstantiateProjectile(FirePoint);
            }          
            
        }

        private void InstantiateProjectile(Transform FirePoint)
        {
            var ProjectileObj = Instantiate(Projectile, FirePoint.position, Quaternion.identity) as GameObject;
            ProjectileObj.GetComponent<Rigidbody>().velocity = (destination - FirePoint.position).normalized * ProjectileSpeed;
        }

      
    }
}

