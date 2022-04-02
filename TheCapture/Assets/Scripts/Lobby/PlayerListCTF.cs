using CTF.Managers;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using CTF.PlayerController;
using  TMPro;

namespace CTF.Lobby
{
    public class PlayerListCTF : MonoBehaviour
    {

      [Header("UI References")]
        public TextMeshProUGUI PlayerNameText;

        public Image PlayerColorImage;
        public Button PlayerReadyButton;
        public Image PlayerReadyImage;


        public Sprite closeImage;
        public Sprite checkImage;

        public int ownerId;
        private bool isPlayerReady;


        #region UNITY

        public void OnEnable()
        {
            PlayerNumbering.OnPlayerNumberingChanged += OnPlayerNumbering;
        }

        public void Start()
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
            {
                PlayerReadyButton.gameObject.SetActive(false);
            }
            else
            {
                Hashtable initialProps = new Hashtable() {{PlayerCTF.PLAYER_READY, isPlayerReady}, {PlayerCTF.PLAYER_LIVES, PlayerCTF.PLAYER_MAX_LIVES}};
                PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
                PhotonNetwork.LocalPlayer.SetScore(0);

                
                PlayerReadyButton.onClick.AddListener(() =>
                {
                    isPlayerReady = !isPlayerReady;
                    SetPlayerReady(isPlayerReady);
                    
                    Hashtable props = new Hashtable() {{PlayerCTF.PLAYER_READY, isPlayerReady}};
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                    if (PhotonNetwork.IsMasterClient)
                    {
                        FindObjectOfType<MainMenuManager>().LocalPlayerPropertiesUpdated();
                    }

                });
            }
        }

        public void OnDisable()
        {
            PlayerNumbering.OnPlayerNumberingChanged -= OnPlayerNumbering;
        }

        #endregion

        public void Initialize(int playerId, string playerName)
        {
            ownerId = playerId;
            PlayerNameText.text = playerName;
        }
        
        private void OnPlayerNumbering()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == ownerId)
                {
                    PlayerColorImage.color = PlayerCTF.GetColor(p.GetPlayerNumber());
                }
            }
        }

        public void SetPlayerReady(bool playerReady)
        {
            PlayerReadyButton.GetComponentInChildren<TextMeshProUGUI>().text = playerReady ? "Ready!" : "Ready?";
            PlayerReadyImage.sprite = playerReady ? checkImage : closeImage;
            PlayerReadyImage.color = playerReady ? Color.white : Color.red;
            PlayerReadyImage.enabled = true;
        }
    }
}

