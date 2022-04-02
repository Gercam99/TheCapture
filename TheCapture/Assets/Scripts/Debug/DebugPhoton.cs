using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace CTF.TEST
{
    public class DebugPhoton : MonoBehaviour
    {
        private readonly string connectionStatusMessage = "Connection Status: ";

        private void Update()
        {
            Debug.Log(connectionStatusMessage + PhotonNetwork.NetworkClientState);
        }
    }
}
