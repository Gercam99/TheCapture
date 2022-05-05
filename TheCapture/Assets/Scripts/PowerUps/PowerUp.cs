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
    public void Settings(Waypoint waypoint)
    {
        Waypoint = waypoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Compara el "tag" y si es el del player comenzara la funcion "StartCooldown
        //y nanoseguidamente se destruira.
        if (other.CompareTag("Player"))
        {
            PhotonView.RPC("DestroyPowerUp", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    private void DestroyPowerUp()
    {
        Waypoint.StartCooldown();
        Destroy(gameObject);
    }
    
}
