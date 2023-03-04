using System;
using System.Collections;
using System.Collections.Generic;
using CaptureFlagGame;
using UnityEngine;
using Mirror;

namespace CaptureFlagGame
{
    public class FlagSpawnPosition : MonoBehaviour
    {
        public bool IsAvailable = true;
        public void Start()
        {
            ServerNetworkManager.Instance.RegisterFlagSpawnPosition(this);
        }
    
        public void OnDestroy()
        {
            ServerNetworkManager.Instance.UnregisterFlagSpawnPosition(this);
        }
    } 
}

