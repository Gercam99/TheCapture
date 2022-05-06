using System;
using System.Collections;
using System.Collections.Generic;
using CTF.PlayerController;
using UnityEngine;

using  Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace CTF.Managers
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance { get; private set; }
        public Text WinText;
        public TextMeshProUGUI InfoText;
        public List<Transform> spawnPointsRed = new List<Transform>();
        public List<Transform> spawnPointsBlue = new List<Transform>();
        [SerializeField] private GameObject cam;

        public static int blueScore = 0;
        public static int redScore = 0;
        public  TextMeshProUGUI blueScoreText;
        public  TextMeshProUGUI redScoreText;
        public static bool finishGame = false;
        public Button exitButton;
        public bool optionsOpen;
        
        private void Awake()
        {
            Instance = this;
            SetScoreText();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
        }

        private void Start()
        {
            finishGame = false;
            blueScore = 0;
            redScore = 0;
            MainMenuManager.Instance.mainCanvas.SetActive(false);
            Hashtable props = new Hashtable
            {
                {PlayerCTF.PLAYER_LOADED_LEVEL, true},
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            WinText.text = "Waiting to other players...";
            
            
            exitButton.onClick.AddListener(()=>PhotonNetwork.LeaveRoom());

        }

        private void Update()
        {
            SetScoreText();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (optionsOpen)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    optionsOpen = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    optionsOpen = true;
                }
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
           
        }

        public override void OnLeftRoom()
        {
            if (SceneSystem.Instance != null)
            {
                SceneSystem.Instance.LoadScene("MainMenu", false);
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            //
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            // {
            //     PhotonNetwork.LeaveRoom();
            // }
            
            PhotonNetwork.DestroyPlayerObjects(otherPlayer);
        }
        
        public void UpdateGameStats()
        {
            if (blueScore >= 3)
            {
                CheckEndOfGame();
            }

            if (redScore >= 3)
            {
                CheckEndOfGame();
            }
        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {

            // if (changedProps.ContainsKey(PlayerCTF.CAPTURED_THE_FLAG))
            // {
            //     return;
            // }

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
                
            }



            int startTimestamp;
            bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);

            if (changedProps.ContainsKey(PlayerCTF.PLAYER_LOADED_LEVEL))
            {
                if (CheckAllPlayerLoadedLevel())
                {
                    if (!startTimeIsSet)
                    {
                        CountdownTimer.SetStartTime();
                    }
                }
                else
                {
                    Debug.Log("Setting text waiting for players", this.WinText);
                    WinText.text = "Waiting for other players...";
                }
            }
        }

        private void OnCountdownTimerIsExpired()
        {
            StartGame();
        }
        
        private void StartGame()
        {
            Debug.Log("START THE GAME!");
            cam.SetActive(false);

            if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Name == "Blue")
            {
                object numPlayerInTeam;
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerCTF.PLAYER_NUM_TEAM, out numPlayerInTeam))
                {
                    int i = ((int) numPlayerInTeam == 1) ? 0 : (int) numPlayerInTeam - 1;
                    PhotonNetwork.Instantiate("PlayerCTF", spawnPointsBlue[i].position, Quaternion.identity);
                }
            }
            if (PhotonNetwork.LocalPlayer.GetPhotonTeam().Name == "Red")
            {
                object numPlayerInTeam;
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerCTF.PLAYER_NUM_TEAM, out numPlayerInTeam))
                {
                    int i = ((int)numPlayerInTeam == 1) ? 0 : (int)numPlayerInTeam -1;
                    PhotonNetwork.Instantiate("PlayerCTF", spawnPointsRed[i].position, Quaternion.identity);
                }

            }

            
        }

        private IEnumerator EndOfGame(string winner)
        {
            float timer = 5.0f;

            while (timer > 0.0f)
            {
                WinText.text = $"{winner} won. Returning to login screen in{timer:n2}";

                yield return new WaitForEndOfFrame();

                timer -= Time.deltaTime;
            }

            PhotonNetwork.LeaveRoom();
        }

        private void CheckEndOfGame()
        {
            bool allDestroyed = redScore >= 3 || blueScore >=3;

            // foreach (var p in PhotonNetwork.PlayerList)
            // {
            //     object capturedFlag;
            //
            //     if (p.CustomProperties.TryGetValue(PlayerCTF.CAPTURED_THE_FLAG, out capturedFlag))
            //     {
            //         if ((bool) capturedFlag == false)
            //         {
            //             allDestroyed = false;
            //             break;
            //             
            //         }
            //     }
            // }

            if (allDestroyed)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StopAllCoroutines();
                }
                

                string winner = "";


                if (redScore >= 3)
                {
                    winner = "Red Team";
                }

                if (blueScore >= 3)
                {
                    winner = "Blue Team";
                }
                // foreach (var p in PhotonNetwork.PlayerList)
                // {
                //     object capturedFlag;
                //     if (p.CustomProperties.TryGetValue(PlayerCTF.CAPTURED_THE_FLAG, out capturedFlag))
                //     {
                //         if ((bool) capturedFlag)
                //         {
                //             winner = p.NickName;
                //         }
                //     }
                // }

                StartCoroutine(EndOfGame(winner));


            }
        }
        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(PlayerCTF.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                {
                    if ((bool) playerLoadedLevel)
                    {
                        continue;
                    }
                }

                return false;
            }

            return true;
        }

        public void SetScoreText()
        {
            blueScoreText.text = blueScore.ToString();
            redScoreText.text = redScore.ToString();
        }

        [PunRPC]
        public void ShowInfoText(string message)
        {
            InfoText.text = message;
            StartCoroutine(DisableInfoText());
        }

        private IEnumerator DisableInfoText()
        {
            yield return new WaitForSeconds(5);
            InfoText.text = String.Empty;
        }
    }
}
