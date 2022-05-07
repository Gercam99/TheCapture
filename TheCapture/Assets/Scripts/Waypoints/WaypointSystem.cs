using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionsUnity;
using Photon.Pun;
using PowerUps;

namespace WaypointController
{
    public class WaypointSystem : MonoBehaviour, IPunObservable
{
    //Singleton del Script WaypointSystem.
    public static WaypointSystem Instance { get; private set; }

    [Tooltip("Lista Waypoints")]
    public List<Waypoint> Waypoints = new List<Waypoint>();
    [Tooltip("Lista PowerUps")]
    public List<PowerUp> PowerUps = new List<PowerUp>();
    [Tooltip("Capacidad maxima de Powerups en el juego")]
    public int MaxPowerUps;
    [Tooltip("Numero de powerups que hay en la escena")]
    public int CountPowerUps;


    private PhotonView _photonView;
    public Dictionary<Waypoint, int> WaypointDictionary = new Dictionary<Waypoint, int>();


    private void Awake()
    {
        //Le asignamos el singleton de Waypoint System.
        Instance = this;
    }
    private void Start()
    {
        WaypointDictionary.Clear();
        foreach (var waypoint in Waypoints)
        {
            WaypointDictionary.Add(waypoint, waypoint.GetID());
        }
        
        _photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            SetupPowerUps();
        }
    }
    
    
    
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

            //_photonView.RPC("InstantiatePowerUp", RpcTarget.AllViaServer, powerUp, waypoint);
            
            GameObject _powerUp = PhotonNetwork.InstantiateRoomObject(powerUp.name, waypoint.Position, Quaternion.identity);
            //Assignacion del waypoint al power up.
            //_powerUp.GetComponent<PowerUp>().Waypoint = waypoint;
            _powerUp.GetComponent<PhotonView>().RPC("Settings", RpcTarget.AllViaServer, waypoint.GetID());
            CountPowerUps++;
            waypoint.IsBusy = true;
        }
    }

    
    
    /// <summary>
    /// Crea un solo power up en una posicion aleatoria dentro de un waypoint no ocupado.
    /// </summary>
    public void CreatePowerUp()
    {
        if (CountPowerUps < MaxPowerUps)
        {
            Waypoints.Shuffle();
            PowerUp powerUp = PowerUps.Random();
            var waypoint = SetWaypoint();
        
            GameObject _powerUp = PhotonNetwork.InstantiateRoomObject(powerUp.name, waypoint.Position, Quaternion.identity);
            //Assignacion del waypoint al power up.
            //_powerUp.GetComponent<PowerUp>().Waypoint = waypoint;
            _powerUp.GetComponent<PhotonView>().RPC("Settings", RpcTarget.AllViaServer, waypoint.GetID());
            CountPowerUps++;
            waypoint.IsBusy = true;
        }
        

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
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(CountPowerUps);
        }
        else
        {
            CountPowerUps = (int) stream.ReceiveNext();
        }
    }
}
}

