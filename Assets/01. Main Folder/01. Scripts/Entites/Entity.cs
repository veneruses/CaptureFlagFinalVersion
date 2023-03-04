using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace CaptureFlagGame
{
    public abstract class Entity : NetworkBehaviour
    {
    
        [SerializeField] private protected MeshRenderer m_mesh;
        [SyncVar(hook=nameof(SyncObjectState))][SerializeField] private protected EntityColorData m_entityColorData;
    
        [Server]
        public virtual void ServerSetColoredMaterial(EntityColorData data)
        {
            m_entityColorData = data;
        }

        [System.Serializable]
        public struct EntityColorData
        {
            public ushort MaterialId;
            public Color Color;
        }
    
        public virtual void SyncObjectState(EntityColorData oldData, EntityColorData newData)
        {
        }
    }
}
