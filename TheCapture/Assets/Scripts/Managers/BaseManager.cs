using System;
using CTF.PlayerController;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace CTF.Managers
{
    public class BaseManager : MonoBehaviour
    {
        public string TeamBase;
        public FlagManager FlagManager;
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController.PlayerController player))
            {
                if(player.pv.Controller.GetPhotonTeam().Name != TeamBase) return;

                object capturedTheFlag;
                if (player.pv.Controller.CustomProperties.TryGetValue(PlayerCTF.CAPTURED_THE_FLAG, out capturedTheFlag))
                {
                    if ((bool)capturedTheFlag)
                    {
                        FlagManager.PhotonView.RPC("FlagToBase", RpcTarget.AllViaServer, player.pv.Controller.GetPhotonTeam().Name);
                        //FlagManager.FlagToBase( player.pv.Controller.GetPhotonTeam().Name);
                        player.pv.Controller.SetCustomProperties(new Hashtable {{PlayerCTF.CAPTURED_THE_FLAG, false}});
                    }
                }
            }
        }
    }
}


