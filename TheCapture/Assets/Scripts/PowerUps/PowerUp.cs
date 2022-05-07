using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CTF.Inventory;
using CTF.PlayerController;
using ExtensionsUnity;
using Photon.Pun;
using UnityEngine;
using WaypointController;


namespace PowerUps
{
    public class PowerUp : MonoBehaviour
    {

        public PowerUpData PowerUpData;
        public PhotonView PhotonView;
        public Waypoint Waypoint;


        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
        }

        [PunRPC]
        public void Settings(int id)
        {
            var waypoint = WaypointSystem.Instance.WaypointDictionary.KeyByValue(id);
            Waypoint = waypoint;

        }

        private void OnTriggerEnter(Collider other)
        {
            //Compara el "tag" y si es el del player comenzara la funcion "StartCooldown
            //y nanoseguidamente se destruira.

            if (other.TryGetComponent(out InventoryController inventory))
            {
                if (inventory.HasPowerUp()) return;
                Waypoint.StartCooldown();
                WaypointSystem.Instance.CountPowerUps--;
                inventory.PowerUpData = PowerUpData;
                PhotonView.RPC("DestroyPowerUp", RpcTarget.AllViaServer);
            }

        }

        [PunRPC]
        private void DestroyPowerUp()
        {
            Destroy(gameObject);
        }
    
    }

}
