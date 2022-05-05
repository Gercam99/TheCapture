using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    public PhotonView PhotonView;
    public Waypoint Waypoint;


    private void Awake()
    {
        PhotonView = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void Settings(int id)
    {
        if (id == null) return;
        
        Waypoint waypoint = Photon.Pun.PhotonView.Find(id).gameObject.GetComponent<Waypoint>();
        Waypoint = waypoint;

    }

    private void OnTriggerEnter(Collider other)
    {
        //Compara el "tag" y si es el del player comenzara la funcion "StartCooldown
        //y nanoseguidamente se destruira.
        if (other.CompareTag("Player"))
        {
            Waypoint.StartCooldown();
            WaypointSystem.Instance.CountPowerUps--;
            PhotonView.RPC("DestroyPowerUp", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    private void DestroyPowerUp()
    {

        Destroy(gameObject);
    }
    
}
