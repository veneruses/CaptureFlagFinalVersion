using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace CaptureFlagGame
{
    public struct InstantiateCharacterMessage : NetworkMessage
    {
        public string Name;
    }
    
    public struct InstantiateTestCharacterMessage : NetworkMessage
    {
    }
    
    public struct InstantiateFlagMessage : NetworkMessage
    {
        public Player owner;
    }
}

