using System;
using System.Collections;
using System.Collections.Generic;
using CTF.PlayerController;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

using Random = UnityEngine.Random;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace CTF.Managers
{
    public class FlagManager : MonoBehaviour
    {

        private const int TIME_TO_RESPAWN = 3;
        private bool isCaptured;
        public PhotonView PhotonView;

        public List<Transform> spawnPositions;
        public string TeamFlag;

        private void Awake()
        {
            PhotonView = GetComponent<PhotonView>();
        }
        

        private void OnTriggerEnter(Collider other)
        {
            if(isCaptured) return;


            if (other.gameObject.CompareTag("Player"))
            {

                    PlayerController.PlayerController playerController = other.gameObject.GetComponent<PlayerController.PlayerController>();
                    // move.pv.Owner.AddScore(1);
                    //
                    //
                    playerController.pv.Owner.SetCustomProperties(new Hashtable
                    {
                        {PlayerCTF.CAPTURED_THE_FLAG, true}
                    });

                    var nameTeam = playerController.pv.Controller.GetPhotonTeam().Name;

                    if(playerController.pv.Controller.GetPhotonTeam().Name == TeamFlag) return;
                    PhotonView.RPC("CapturedGlobally", RpcTarget.AllViaServer);
                    PhotonView.RPC("InfoTextEnemy", RpcTarget.AllViaServer, 
                        nameTeam, $"El equipo {nameTeam} ha capturado la bandera.");

                    //playerController.pv.Controller.GetPhotonTeam().Name)

            }
        }


        [PunRPC]
        private void CapturedGlobally()
        {
            isCaptured = true;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;

        }

        [PunRPC]
        public void InfoTextEnemy(string nameTeam, string message)
        {
            if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Name != nameTeam)
            {
                GameManager.Instance.ShowInfoText(message);
            }
        }
        
        [PunRPC]
        public void FlagToBase(string nameTeam)
        {
            if (nameTeam == "Blue")
            {
                GameManager.blueScore++;
            }

            if (nameTeam == "Red")
            {
                GameManager.redScore++;
            }
            
            GameManager.Instance.UpdateGameStats();
            
            if (GameManager.redScore >= 3) GameManager.finishGame = true;
            if (GameManager.blueScore >= 3) GameManager.finishGame = true;

            PhotonView.RPC("InfoTextEnemy", RpcTarget.AllViaServer, 
                nameTeam, $"El equipo {nameTeam} ha logrado llevar la bandera a la base.");
            
            StartCoroutine("WaitForRespawn");
        }

        IEnumerator WaitForRespawn()
        {
            Debug.Log("WaitToRespawn");
            yield return new WaitForSeconds(TIME_TO_RESPAWN);
            PhotonView.RPC("RespawnFlag", RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void RespawnFlag()
        {
            Debug.Log($"Flag {TeamFlag}, se ha respawneado");
            isCaptured = false;
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<CapsuleCollider>().enabled = true;
            
            if(!PhotonNetwork.LocalPlayer.IsMasterClient) return;
            
            int numPositionList = Random.Range(0, spawnPositions.Count);

            transform.position = spawnPositions[numPositionList].position;

        }
    }
}
