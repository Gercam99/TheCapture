using System;
using System.Collections;
using System.Collections.Generic;
using CTF.PlayerController;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

using Random = UnityEngine.Random;
using Photon.Pun.UtilityScripts;


namespace CTF.Managers
{
    public class FlagManager : MonoBehaviour
    {

        private const int TIME_TO_RESPAWN = 3;
        private bool isCaptured;
        private PhotonView photonView;

        public List<Transform> spawnPositions;
        public string TeamFlag;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
        }
        

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other);
            if(isCaptured) return;


            if (other.gameObject.CompareTag("Player"))
            {

                    Move move = other.gameObject.GetComponent<Move>();
                    // move.pv.Owner.AddScore(1);
                    //
                    //
                    //     move.pv.Owner.SetCustomProperties(new Hashtable
                    //     {
                    //         {PlayerCTF.CAPTURED_THE_FLAG, true}
                    //     });
                    
                    Debug.Log(move.pv.Controller.GetPhotonTeam());

                    if(move.pv.Controller.GetPhotonTeam().Name == TeamFlag) return;
                    photonView.RPC("CapturedGlobally", RpcTarget.AllViaServer,  move.pv.Controller.GetPhotonTeam().Name);
                
            }
        }


        [PunRPC]
        private void CapturedGlobally(string nameTeam)
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
            isCaptured = true;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            if (GameManager.redScore >= 3) GameManager.finishGame = true;
            if (GameManager.blueScore >= 3) GameManager.finishGame = true;

            StartCoroutine("WaitForRespawn");
        }


        IEnumerator WaitForRespawn()
        {
            yield return new WaitForSeconds(TIME_TO_RESPAWN);
            photonView.RPC("RespawnFlag", RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void RespawnFlag()
        {
            isCaptured = false;
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<CapsuleCollider>().enabled = true;
            
            if(!PhotonNetwork.LocalPlayer.IsMasterClient) return;
            
            int numPositionList = Random.Range(0, spawnPositions.Count);

            transform.position = spawnPositions[numPositionList].position;

        }
    }
}
