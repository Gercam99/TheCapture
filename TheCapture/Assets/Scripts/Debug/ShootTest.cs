using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CTF
{
    public class ShootTest : MonoBehaviour
    {
        [Header("Parametros Generales")]
        public Camera Cam;
        public Transform FirePoint;
        public GameObject Bullet;

        [Header("Parametros Arma")]
        [Tooltip("Cadencia Disparo")]public float FireRate;
        public float AmmoReloadTime;
        public int MaxAmmo;
        public int CurrentAmmo;    
        public bool IsCharging { get; private set; }

        // Variables Privadas
        private Vector3 _destination;
        private float _nextTimeToShoot;
        //private float _lastTimeToShoot = Mathf.NegativeInfinity;
        private float _lastTimeToReload;


        private void Awake()
        {
            CurrentAmmo = MaxAmmo;
        }
        void Start()
        {
            GetAmmoNeededToShoot();
            Cam = Camera.main;
        }
        public float GetAmmoNeededToShoot()
        {
            return (CurrentAmmo != MaxAmmo ? 1 : Mathf.Max(1, 1)) / (MaxAmmo * 1);
        }
        
        void Update()
        {
            Reload();
            if (IsCharging == true)
            {
                return;
            }
            //WARNING!!!!!! Hasta que no inicie la partida no se puede disparar o si estas muerto tampoco puedes disparar!!!!!!!!!
            Shoot();
        }

        /// <summary>
        /// Funcion encargada de disparar
        /// </summary>
        private void Shoot()
        {
            //Cuando presionamos el boton y el tiempo de juego es superior al tiempo de disparo podremos
            //ejecutar el disparo
            if (Input.GetButton("Fire1") && Time.time > _nextTimeToShoot)
            {
                //Si la municion es igual a cero no se va a ejecutar el resto de funcion.
                if (CurrentAmmo == 0)
                {
                    return;
                }

                //Esta variable se encarga de calcular el punto exacto de disparo en ese frame (tiempo exacto)
                //mas el tiempo que hay entre disparo y disparo (FireRate).                
                _nextTimeToShoot = Time.time + FireRate;
                
                Ray ray = Cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); //EL rayo es lanzado desde el punto de vista de la camara.

                //Creamos una fisica de rayo. Si colisionamos con un objeto se creara un hit y alcanzara su objetivo
                //sino colisiona con nada, obtendrá el punto objetivo establecido(valor) 
                _destination = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(1000);

                Instantiate(Bullet, FirePoint.position, Quaternion.identity);
                CurrentAmmo = CurrentAmmo - 1;
            }
        }

        /// <summary>
        /// Funcion encargada de recargar de la arma
        /// </summary>
        private void Reload()
        {
            //Si presionamos la tecla R y nuestra municion es menor a la municion maxima
            //y no estas recargando podras recargar el arma
            if (Input.GetKeyDown(KeyCode.R) && CurrentAmmo < MaxAmmo && IsCharging == false)
            {
                IsCharging = true;
                _lastTimeToReload = Time.time;
            }
            //Si estoy cargando es true se ejecuta siguiente "if".
            if (IsCharging == true)
            {
                //Si la ultima vez de recarga mas el tiempo de recarga es menor que el tiempo jugado
                //entonces la municion sera igual a la maxima municion y ya no estaremos recargando.  
                if (_lastTimeToReload + AmmoReloadTime < Time.time)
                {
                    CurrentAmmo = MaxAmmo;
                    IsCharging = false;
                }

            }
        }
    }
}

