using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    public Waypoint Waypoint;

    private void OnTriggerEnter(Collider other)
    {
        //Compara el "tag" y si es el del player comenzara la funcion "StartCooldown
        //y nanoseguidamente se destruira.
        if (other.CompareTag("Player"))
        {
            Waypoint.StartCooldown();
            Destroy(gameObject);
        }
        

    }

   
}
