using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


namespace WaypointController
{
    public class Waypoint : MonoBehaviour, IPunObservable
    {
        public bool IsBusy;
        public Vector3 Position;
        [Tooltip("Tiempo de espera para crearse otro power up")]
        public float Cooldown;
    
        public PhotonView PhotonView;
    
        
        private void Awake()
        {
            //Para saber la posicion de casa waypoint.
            Position = transform.position;
        }
        
        /// <summary>
        /// Iniciamos la cuenta atras del tiempo de espera.
        /// </summary>
        public void StartCooldown()
        {
            Debug.Log("Start Cooldown Waypoint");
            //Iniciamos la "coroutine".
            StartCoroutine(CooldownCoroutine());
        }
    
    
        /// <summary>
        /// Cuando se haya acabado los segundos, el waypoint no estara ocupado y crearemos el power up.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CooldownCoroutine()
        {   
            //Cuenta atras.
            yield return new WaitForSeconds(Cooldown);
            //Le damos el valor a la "booleana" a falso (no esta ocupada).
            IsBusy = false;
    
            
            //Creamos la funcion "CreatePowerUp"
            WaypointSystem.Instance.CreatePowerUp();

        }
    
        public int GetID() => PhotonView.ViewID;
    
    
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(IsBusy);
            }
            else
            {
                IsBusy = (bool) stream.ReceiveNext();
            }
        }
    }
}


