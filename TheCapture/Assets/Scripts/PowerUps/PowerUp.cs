using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PowerUp : MonoBehaviour, IPunObservable
{

    public PhotonView PhotonView;
    public Waypoint Waypoint;


    private void Start()
    {
        PhotonView = GetComponent<PhotonView>();
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


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Waypoint);
        }
        else
        {
            Waypoint = (Waypoint) stream.ReceiveNext();
        }
    }
}
