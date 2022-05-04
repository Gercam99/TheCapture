using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public bool IsBusy;
    public Vector3 Position;
    [Tooltip("Tiempo de espera para crearse otro power up")]
    public float Cooldown;

    
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
        //Iniciamos la "coroutine".
        StartCoroutine(CooldownCoroutine());
    }


    /// <summary>
    /// Cuando se haya acabado los segundos, el waypoint no estara ocupado y crearemos el power up.
    /// </summary>
    /// <returns></returns>
    IEnumerator CooldownCoroutine()
    {   
        //Cuenta atras.
        yield return new WaitForSeconds(Cooldown);
        //Le damos el valor a la "booleana" a falso (no esta ocupada).
        IsBusy = false;
        //Creamos la funcion "CreatePowerUp"
        WaypointSystem.Instance.CreatePowerUp();
        
    }

   
}
