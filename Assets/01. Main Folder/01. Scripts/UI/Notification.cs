using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using TMPro;
using Unity.VisualScripting;


namespace CaptureFlagGame
{
    public class Notification : NetworkBehaviour
    {
        public static Notification Instance;
        [SerializeField] public TextMeshProUGUI textLabel;
        private Coroutine m_messageCoroutine;
        

        private void Awake()
        {
            //SINGLETON
            if (Instance == null)
            {
                Instance = this;
            }
            else
                Destroy(this);
        }
        
        [TargetRpc]
        public void SendMessageToTargetPlayer(NetworkConnectionToClient conn, string msg)
        {
            ActivateMessage(msg);
        }

        /// <summary>
        /// If connection don't include connectionToExclude, then send message to this connection
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="connectionsToExclude"></param>
        [Server]
        public void SendAllExceptSomebody(string msg, params NetworkConnectionToClient[] connectionsToExclude)
        {
            List<NetworkConnectionToClient> allPlayersConnections = new List<NetworkConnectionToClient>();
            EntityManager.Instance.GetAllPlayers().ForEach(
                (player) => { allPlayersConnections.Add(player.connectionToClient); });

            allPlayersConnections.ForEach((connection) =>
            {
                if (connectionsToExclude.Contains(connection) == false)
                {
                    SendMessageToTargetPlayer(connection, msg);
                }
            });
        }

        [ClientRpc]
        public void RpcSendMessageToAll(string msg)
        {
            ActivateMessage(msg);
        }

        public void ActivateMessage(string message)
        {
            if (m_messageCoroutine != null)
            {
                StopCoroutine(m_messageCoroutine);
            }

            m_messageCoroutine = StartCoroutine(ActivateMessageCoroutine(message, 0.5f));
        }

        private IEnumerator ActivateMessageCoroutine(string message, float timeToBeActiveOnScreen)
        {
            textLabel.text = message;
            while (textLabel.alpha < 1)
            {
                textLabel.alpha += 0.2f;
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(timeToBeActiveOnScreen);
            while (textLabel.alpha > 0)
            {
                textLabel.alpha -= 0.2f;
                yield return new WaitForSeconds(0.1f);
            }

            m_messageCoroutine = null;
        }
    }
}