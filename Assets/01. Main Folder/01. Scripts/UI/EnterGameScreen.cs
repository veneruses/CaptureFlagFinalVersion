using System;
using System.Collections;
using System.Collections.Generic;
using CaptureFlagGame;
using UnityEngine;

namespace CaptureFlagGame
{
    public class EnterGameScreen : MonoBehaviour
    {
        [SerializeField] private GameObject m_enterGameScreenPanel;

        private void Start()
        {
            ServerNetworkManager.Instance.OnClientConnected += () => { m_enterGameScreenPanel.SetActive(false); };
            ServerNetworkManager.Instance.OnClientDisconnected += () => { m_enterGameScreenPanel.SetActive(true); };
        }
    }
}