using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Waypoint : MonoBehaviour
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

    private void Start()
    {
        PhotonView = GetComponent<PhotonView>();
    }

    
    [PunRPC]
    /// <summary>
    /// Iniciamos la cuenta atras del tiempo de espera.
    /// </summary>
    public void StartCooldown()
    {
        Debug.Log("HOLAAAAAA");
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

        PhotonView.RPC("CallCreatePowerUp", RpcTarget.AllViaServer);
        
    }

    [PunRPC]
    private void CallCreatePowerUp()
    {
        //Creamos la funcion "CreatePowerUp"
        WaypointSystem.Instance.CreatePowerUp();
    }

   
}
