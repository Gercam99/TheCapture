using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using  TMPro;
using UnityEngine.UI;

namespace CTF.Managers
{
    public class ChatManager : MonoBehaviourPun, IChatClientListener
    {
        
        public static ChatManager Instance { get; private set; }
        [SerializeField] private TMP_InputField inputMessage;
        [SerializeField] private TextMeshProUGUI textMessage;
        [SerializeField] private Button sendBtn;
        [SerializeField] private Scrollbar scrollBar;
        public string actualChannelName;

        [SerializeField] private GameObject chatWindows;


        private ChatClient client;


        #region Setup
        private void Awake() => Instance = this;

        private void Start()
        {
            inputMessage.onEndEdit.AddListener(OnSendMessageEnter);
            sendBtn.onClick.AddListener(OnSendMessageButtonClicked);
            MainMenuManager.Instance.mainCanvas.SetActive(true);
            MainMenuManager.Instance.RestartPlayer();
            client = new ChatClient(this);
        }
        

        #endregion


        private void Update()
        {
            client?.Service(); // If client is not null, call Services

            switch (client.State)
            {
                case ChatState.Disconnected:
                    chatWindows.SetActive(false);
                    break;
                case ChatState.Uninitialized:
                    chatWindows.SetActive(false);
                    break;
                case ChatState.ConnectedToNameServer:
                    chatWindows.SetActive(true);
                    break;
                        
            }
        }


        #region UI FUNCTIONS

        /// <summary>
        /// Funcion encargada de preparar el mensaje cuando se da enter
        /// </summary>
        /// <param name="data"> Input field text</param>
        private void OnSendMessageEnter(string data)
        {
            PrepareMessage();
        }
        

        /// <summary>
        /// Funcion encargada de preparar el mensaje cuando le das al boton
        /// </summary>
        private void OnSendMessageButtonClicked()
        {
            PrepareMessage();
        }

        #endregion
        

        #region CHAT FUNCTIONS

        #region MESSAGE FUNCTIONS

        /// <summary>
        /// Funcion encargada de analizar el mensaje preparado y dependiendo de lo que
        /// ejecutara la función correspondiente
        /// </summary>
        private void PrepareMessage()
        {
            string message = inputMessage.text;

            if (message != string.Empty)
            {
                if (message.Equals("/clear"))
                {
                    ClearChat();
                }
                else
                {
                    SendMessagePUN(message);
                }

            }
            
            inputMessage.text = string.Empty;
        }

        
        /// <summary>
        /// Enviamos el mensaje ya preparado al server de photon
        /// </summary>
        /// <param name="message"></param>
        private void SendMessagePUN(string message)
        {
            scrollBar.value = 0;
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                client.PublishMessage(actualChannelName, $"<color=#436499>{message}</color>");
            }
            else
            {
                client.PublishMessage(actualChannelName, message);
            }

        }
        
        
        /// <summary>
        /// Funcion encargada de limpiar el chat
        /// </summary>
        private void ClearChat()
        {
            ChatChannel chatChannel;
            if (this.client.TryGetChannel(actualChannelName, false, out chatChannel))
            {
                chatChannel.ClearMessages();
            }

            textMessage.text = string.Empty;
        }

        #endregion
        
        
        /// <summary>
        /// Funcion encargada de iniciar el servidor de chat a partir del ID, verison.
        /// </summary>
        public void Connection()
        {
            textMessage.text = string.Empty;
            client = new ChatClient(this);
            client.UseBackgroundWorkerForSending = true;
            client.Connect("4e72c9bc-f24e-4d61-9c5d-034b395913b6", "1.0",
                new AuthenticationValues(MainMenuManager.Instance.playerName));
        }

        
        /// <summary>
        /// Funcion encargada de desconectar el servidor de chat
        /// </summary>
        public void Disconnect()
        {
            if(client==null) return;
            client.PublishMessage(actualChannelName, $"<color=#E07B00>{MainMenuManager.Instance.playerName} out of chanel.</color>");
            
            //if(client!= null) client.Disconnect();

            StartCoroutine(WaitToDisconnect(.5f));
        }

        IEnumerator WaitToDisconnect(float delay)
        {
            yield return new WaitForSeconds(delay);
            if(client!= null) client.Disconnect();
        }

        
        public void DebugReturn(DebugLevel level, string message)
        {
            switch (level)
            {
                case DebugLevel.ERROR:
                    Debug.LogError(message);
                    break;
                case DebugLevel.WARNING:
                    Debug.LogWarning(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }

        public void OnDisconnected()
        {
            //client.PublishMessage(actualChannelName, $"<color=#E07B00>{MainMenuManager.Instance.playerName} out of chanel.</color>");
        }

        public void OnConnected()
        {
            if (actualChannelName != string.Empty)
            {
                client.Subscribe(actualChannelName);
            }
            client.SetOnlineStatus(ChatUserStatus.Online);
            textMessage.text = string.Empty;
        }

        public void OnChatStateChange(ChatState state)
        {
            Debug.Log(state.ToString());
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            ShowChannel(channelName);
        }

        private void ShowChannel(string channelName)
        {
            if(string.IsNullOrEmpty(channelName)) return;


            ChatChannel channel = null;
            bool channelFound = client.TryGetChannel(channelName, out channel);

            if (!channelFound)
            {
                textMessage.text = $"Channel not found {channelName}";
            }

            actualChannelName = channelName;
            textMessage.text = channel.ToStringMessages();

        }
        
        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            Debug.Log("MENSAJE PRIVADO");
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            foreach (var channel in channels)
            {
                client.PublishMessage(channel, $"{MainMenuManager.Instance.playerName} join to channel. ");
            }
            
            Debug.Log($"Has subscribed {string.Join(",", channels)}");
            
            ClearChat();
            ShowChannel(channels[0]);
        }

        public void OnUnsubscribed(string[] channels)
        {
            Debug.Log("Has Unsubscribed");
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            Debug.Log($"OnStatusUpdate:  user={user} status={status}");
        }

        public void OnUserSubscribed(string channel, string user)
        {
            Debug.Log($"OnUserSubscribed: user={user} channel={channel}");
        }

        public void OnUserUnsubscribed(string channel, string user)
        {
            Debug.Log($"OnUserUnsubscribed: user={user} channel={channel}");
        }

        #endregion


        private void OnApplicationQuit()
        {
            Disconnect();
        }
    }
}
