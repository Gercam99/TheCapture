using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionsUnity;
using Photon.Pun;

public class WaypointSystem : MonoBehaviour
{
    //Singleton del Script WaypointSystem.
    public static WaypointSystem Instance { get; private set; }

    [Tooltip("Lista Waypoints")]
    public List<Waypoint> Waypoints = new List<Waypoint>();
    [Tooltip("Lista PowerUps")]
    public List<PowerUp> PowerUps = new List<PowerUp>();
    [Tooltip("Capacidad maxima de Powerups en el juego")]
    public int MaxPowerUps;


    private void Awake()
    {
        //Le asignamos el singleton de Waypoint System.
        Instance = this;
    }
    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            SetupPowerUps();
        }
    }

    [PunRPC]
    /// <summary>
    /// Esta funcion crea diferentes powerups, dentro de su maximo.
    /// </summary>
    private void SetupPowerUps()
    {
        Waypoints.Shuffle();
        //Se ejecuta en loop hasta el maximo de power ups.
        //Selecciona un power up aleatorio y un waypoint aleatorio que no este ocupado.
        //Creamos ese power up en el waypoint correspondiente.        
        for (int i = 1; i <= MaxPowerUps; i++)
        {
            PowerUp powerUp = PowerUps.Random();            
            var waypoint = SetWaypoint();
            
            //Si el waypoint es nulo, es decir que los waypoints estan ocupado, entonces el loop del "for" volvera a ejecutarse.
            if(waypoint == null)
            {
                continue;
            }

            InstantiatePowerUp(powerUp, waypoint);
        }
    }
    
    
    [PunRPC]
    /// <summary>
    /// Crea un solo power up en una posicion aleatoria dentro de un waypoint no ocupado.
    /// </summary>
    public void CreatePowerUp()
    {
        Waypoints.Shuffle();
        PowerUp powerUp = PowerUps.Random();
        var waypoint = SetWaypoint();
        
        InstantiatePowerUp(powerUp, waypoint);
    }
    
    
    [PunRPC]
    /// <summary>
    /// Crea la instancia del powerup asignando el waypoint y ocupando la posicion.
    /// </summary>
    /// <param name="powerUp"></param>
    /// <param name="waypoint"></param>
    private void InstantiatePowerUp(PowerUp powerUp, Waypoint waypoint)
    {
        GameObject _powerUp =PhotonNetwork.Instantiate(powerUp.name, waypoint.Position, Quaternion.identity);
        //Assignacion del waypoint al power up.
        if(_powerUp.TryGetComponent(out PowerUp power ))
        {
            power.Waypoint = waypoint;
        }
        waypoint.IsBusy = true;
    }
    
    
    /// <summary>
    /// Devolvemos el waypoint que no este ocupado, si lo esta devolvera null.
    /// </summary>
    /// <returns></returns>
    private Waypoint SetWaypoint()
    {
        Waypoint waypoint = null;
        //Se ejecuta en loop.
        //Por cada waypoint que hay en lista, mirara si ese waypoint se encuentra ocupado.
        //Si es as�, no se creara ningun power up en el.
        foreach (var _waypoint in Waypoints)
        {
            if (_waypoint.IsBusy == false)
            {
                waypoint = _waypoint;
                break;
            }
        }

        return waypoint;
    }
}
